using ApiIsocare2.Data;
using Microsoft.AspNetCore.Mvc;
using ApiIsocare2.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiIsocare2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly AppDbContext _db;
        public QueueController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Queue
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CounterQueue>>> GetCounterQueues()
        {
            try
            {
                var result = await (from q in _db.CounterQueues
                                    join qs in _db.QueueStatuses on q.queue_status_id equals qs.queue_status_id
                                    join qt in _db.QueueTypes on q.queue_type_id equals qt.queue_type_id
                                    select new
                                    {
                                        q.queue_id,
                                        q.queue_date,
                                        QueueStatus = qs.queue_status_name,
                                        QueueType = qt.type_name,
                                        QueueNumber = qt.queue_type_id.ToUpper() + q.queue_number.ToString("000"),
                                        q.counter
                                    })
                                .Where(q => q.queue_date.Date == DateTime.Today)
                                .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }
        }
            

        

        // GET: api/Queue/?id
        [HttpGet("{id}")]
        public async Task<ActionResult<CounterQueue>> GetQueue(int id)
        {
            try
            {
                var result = await (from q in _db.CounterQueues
                                    join qs in _db.QueueStatuses on q.queue_status_id equals qs.queue_status_id
                                    join qt in _db.QueueTypes on q.queue_type_id equals qt.queue_type_id
                                    select new
                                    {
                                        q.queue_id,
                                        q.queue_date,
                                        QueueStatus = qs.queue_status_name,
                                        QueueType = qt.type_name,
                                        QueueNumber = qt.queue_type_id.ToUpper() + q.queue_number.ToString("000"),
                                        q.counter
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
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }


        }


        // POST: api/Queue
        [HttpPost]
        public async Task<IActionResult> PostAsync(string type)
        {
            try
            {
                var count = await _db.CounterQueues
                                    .Where(q => q.queue_date.Date == DateTime.Today && q.queue_type_id == type)
                                    .OrderByDescending(q => q.queue_number)
                                    .Select(q => q.queue_number)
                                    .FirstOrDefaultAsync();

                count = count == 0 ? 1 : count + 1;

                
                var queue = new CounterQueue
                {
                    queue_number = count,
                    queue_date = DateTime.Now,
                    queue_type_id = type,
                    queue_status_id = 0,
                    counter = 0
                };
                
                _db.CounterQueues.Add(queue);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetQueue), new { id = queue.queue_id }, queue);

            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }


        }
        
    }
}
