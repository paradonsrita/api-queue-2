using ApiIsocare2.Data;
using ApiIsocare2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiIsocare2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        
        private readonly AppDbContext _db;
        public BookingController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingQueue>>> GetBookingQueues()
        {
            try
            {
                var result = await (from q in _db.BookingQueues
                                    join qs in _db.QueueStatuses on q.queue_status_id equals qs.queue_status_id
                                    join qt in _db.QueueTypes on q.queue_type_id equals qt.queue_type_id
                                    join qu in _db.Users on q.user_id equals qu.user_id
                                    where q.appointment_date.Date == DateTime.Today && qs.queue_status_id != -9
                                    select new
                                    {
                                        q.queue_id,
                                        q.appointment_date,
                                        QueueStatus = qs.queue_status_name,
                                        QueueType = qt.type_name,
                                        QueueNumber = q.queue_type_id.ToUpper() + q.queue_number.ToString("000"),
                                        q.counter
                                    })
                               .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error : {ex.Message}");
            }
            
        }


        [HttpGet("profile/{userId}")]
        public async Task<ActionResult> getQueueInThisProfile(int userId)
        {
            try
            {
                var result = await (from q in _db.BookingQueues
                                    join qs in _db.QueueStatuses on q.queue_status_id equals qs.queue_status_id
                                    join qt in _db.QueueTypes on q.queue_type_id equals qt.queue_type_id
                                    join qu in _db.Users on q.user_id equals qu.user_id
                                    select new
                                    {
                                        q.queue_id,
                                        q.appointment_date,
                                        QueueStatus = qs.queue_status_name,
                                        QueueType = qt.type_name,
                                        QueueNumber = q.queue_type_id.ToUpper() + q.queue_number.ToString("000"),
                                        q.counter,
                                        q.user_id,
                                        Name = qu.firstname,
                                        qu.lastname,
                                        qu.phone_number,
                                        qu.citizen_id_number
                                    })
                               .Where(q => q.user_id == userId)
                               .FirstOrDefaultAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error : {ex.Message}");
            }
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetBookingQueue(int id)
        {
            try
            {
                var result = await (from q in _db.BookingQueues
                                    join qs in _db.QueueStatuses on q.queue_status_id equals qs.queue_status_id
                                    join qt in _db.QueueTypes on q.queue_type_id equals qt.queue_type_id
                                    join qu in _db.Users on q.user_id equals qu.user_id
                                    select new
                                    {
                                        q.queue_id,
                                        q.appointment_date,
                                        QueueStatus = qs.queue_status_name,
                                        QueueType = qt.type_name,
                                        QueueNumber = q.queue_type_id.ToUpper() + q.queue_number.ToString("000"),
                                        q.counter,
                                        q.user_id,
                                        Name = qu.firstname,
                                        qu.lastname,
                                        qu.phone_number,
                                        qu.citizen_id_number
                                    })
                               .Where(q => q.queue_id == id)
                               .FirstOrDefaultAsync();


                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ("Error : " + ex.Message));
            }


        }

        [HttpGet("calendar/{transaction}")]
        public IActionResult GetQueueOnDate(string transaction)
        {
            try
            {
                var today = DateTime.Today;
                var maxDate = today.AddDays(30);

                var result = _db.BookingQueues
                                .Where(q => q.appointment_date <= maxDate && q.appointment_date > today && q.queue_type_id == transaction)
                                .GroupBy(q => q.appointment_date)
                                .Select(g => new
                                {
                                    QueueTypeId = transaction,
                                    Date = g.Key,
                                    Total = g.Count()
                                })
                                .ToList();

               
                return Ok(result);
                

            }
            catch (Exception ex)
            {
                return StatusCode(500, ("Error : " + ex.Message));
            }
            
        }

        
        [HttpPost("add-queue")]
        public async Task<IActionResult> AddBooking(int userId, string type, DateTime appointmentDate)
        {
            try
            {

                var number = await _db.BookingQueues
                                .Where(q => q.appointment_date.Date == appointmentDate.Date && q.queue_type_id == type)
                                .OrderByDescending(q => q.queue_number)
                                .Select(q => q.queue_number)
                                .FirstOrDefaultAsync();

                number = number == 0 ? 1 : number + 1;

                var queue = new BookingQueue
                {
                    queue_type_id = type,
                    queue_number = number,
                    queue_status_id = 0,
                    user_id = userId,
                    booking_date = DateTime.Now,
                    appointment_date = appointmentDate,
                    counter = 0
                };
                _db.BookingQueues.Add(queue);
                await _db.SaveChangesAsync();


                return CreatedAtAction(nameof(GetBookingQueue), new { id = queue.queue_id }, queue);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ("Error : " + ex.Message));
            }

        }

        [HttpPut("cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var queue = await _db.BookingQueues.FindAsync(id);

                if (queue == null)
                {
                    return BadRequest("Not found");
                }
                
                queue.queue_status_id = -9;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ("Error : " + ex.Message));
            }
            
        }
        
    }
}
