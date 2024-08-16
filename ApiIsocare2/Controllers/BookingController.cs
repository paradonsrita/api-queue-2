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

                // 1. สร้างรายการของทุกวันและเวลาที่เป็นไปได้
                var timesOfInterest = new[] { new TimeSpan(8, 0, 0), new TimeSpan(13, 0, 0) };
                var allDatesAndTimes = Enumerable.Range(0, (maxDate - today).Days + 1)
                    .SelectMany(d => timesOfInterest.Select(t => today.AddDays(d).Date.Add(t)))
                    .ToList();

                // 2. ดึงข้อมูลจากฐานข้อมูลและจัดกลุ่มตามวันที่และเวลา
                var bookingData = _db.BookingQueues
                    .Where(q => q.appointment_date <= maxDate && q.appointment_date > today && q.queue_type_id == transaction)
                    .GroupBy(q => new
                    {
                        Date = q.appointment_date.Date,
                        Time = q.appointment_date.TimeOfDay
                    })
                    .Select(g => new
                    {
                        Date = g.Key.Date,
                        Time = g.Key.Time,
                        Total = g.Count()
                    })
                    .ToList();

                // 3. รวมผลลัพธ์เข้ากับรายการของทุกวันและเวลาที่เป็นไปได้
                var result = allDatesAndTimes
                    .Select(d => new
                    {
                        Date = d.Date,
                        Time = d.TimeOfDay,
                        Total = bookingData.FirstOrDefault(b => b.Date == d.Date && b.Time == d.TimeOfDay)?.Total ?? 0
                    })
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // คำแนะนำในการจัดการข้อผิดพลาดที่เหมาะสม
                return StatusCode(500, new { message = "An error occurred", detail = ex.Message });
            }
        }


        [HttpPost("add-queue")]
        public async Task<IActionResult> AddBooking(int userId, string type, DateTime appointmentDate)
        {
            try
            {
                // ตรวจสอบและปรับเวลา appointmentDate
                var timeOfDay = appointmentDate.TimeOfDay;
                if (timeOfDay != new TimeSpan(8, 0, 0) && timeOfDay != new TimeSpan(13, 0, 0))
                {
                    return BadRequest("Appointment time must be 08:00 or 13:00.");
                }

                // แก้ไขค่าเวลาให้เป็น 08:00 หรือ 13:00
                var correctedAppointmentDate = appointmentDate.Date.Add(timeOfDay);

                var number = await _db.BookingQueues
                                .Where(q => q.appointment_date.Date == correctedAppointmentDate.Date && q.queue_type_id == type)
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
                    appointment_date = correctedAppointmentDate,
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
