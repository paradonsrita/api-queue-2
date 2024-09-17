using ApiIsocare2.Data;
using ApiIsocare2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiIsocare2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EmployeeController(AppDbContext db)
        {
            _db = db;
        }

        //  PUT: api/Employee/callCounterQueue
        [HttpPut("callCounterQueue")]
        public async Task<IActionResult> CallCounterQueue(string transaction, int counter)
        {
            try
            {
                var waitQueue = await _db.CounterQueues
                    .Where(q => q.queue_type_id == transaction && q.queue_status_id == 0 && q.queue_date.Date == DateTime.Today)
                    .OrderBy(q => q.queue_number)
                    .FirstOrDefaultAsync();
                var nowQueue = await _db.CounterQueues
                    .Where(q => q.counter == counter && q.queue_status_id == 2 && q.queue_date.Date == DateTime.Today)
                    .FirstOrDefaultAsync();

                if (nowQueue != null)
                {
                    nowQueue.queue_status_id = 1;
                }
                if (waitQueue != null)
                {
                    if (counter != 0)
                    {
                        waitQueue.queue_status_id = 2;
                        waitQueue.counter = counter;
                        
                    }
                    else
                    {
                        throw new Exception("Counter cannot zero");
                    }
                }
                if (nowQueue != null || waitQueue != null)
                {
                    await _db.SaveChangesAsync();
                    return NoContent();
                }
                throw new Exception("No wait queue or now queue");


            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }
        }

        

        //  PUT: api/Employee/skipCounterQueue
        [HttpPut("skipCounterQueue")]
        public async Task<IActionResult> SkipCounterQueue(string transaction, int counter)
        {
            try
            {
                var waitQueue = await _db.CounterQueues
                .Where(q => q.queue_type_id == transaction && q.queue_status_id == 0 && q.queue_date.Date == DateTime.Today)
                .OrderBy(q => q.queue_number)
                .FirstOrDefaultAsync();
                var nowQueue = await _db.CounterQueues
                    .Where(q => q.counter == counter && q.queue_status_id == 2 && q.queue_date.Date == DateTime.Today)
                    .FirstOrDefaultAsync();

                if (nowQueue != null)
                {
                    nowQueue.queue_status_id = 3;
                    nowQueue.counter = 0;

                    if (waitQueue != null)
                    {
                        waitQueue.queue_status_id = 2; 
                        waitQueue.counter = counter;

                    }
                    await _db.SaveChangesAsync();
                    return NoContent();
                }

                throw new Exception("No now queue");
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }
        }



        //  PUT: api/Employee/callBookingQueue
        [HttpPut("callBookingQueue")]
        public async Task<IActionResult> CallBookingQueue(string transaction, int counter)
        {
            try
            {
                var waitQueue = await _db.BookingQueues
                    .Where(q => q.queue_type_id == transaction && q.queue_status_id == 0 && q.appointment_date.Date == DateTime.Now.Date)
                    .OrderBy(q => q.queue_number)
                    .FirstOrDefaultAsync();
                var nowQueue = await _db.BookingQueues
                    .Where(q => q.counter == counter && q.queue_status_id == 2 && q.appointment_date.Date == DateTime.Now.Date)
                    .FirstOrDefaultAsync();

                if (nowQueue != null)
                {
                    nowQueue.queue_status_id = 1;
                }
                if (waitQueue != null)
                {
                    if (counter != 0)
                    {
                        waitQueue.queue_status_id = 2;
                        waitQueue.counter = counter;
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Counter cannot zero");
                    }
                }
                

                return NoContent();
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }
        }



        //  PUT: api/Employee/skipQueue
        [HttpPut("skipBookingQueue")]
        public async Task<IActionResult> SkipBookingQueue(string transaction, int counter)
        {
            try
            {
                var waitQueue = await _db.BookingQueues
                    .Where(q => q.queue_type_id == transaction && q.queue_status_id == 0 
                        && q.appointment_date.Date == DateTime.Now.Date)
                    .OrderBy(q => q.queue_number)
                    .FirstOrDefaultAsync();
                var nowQueue = await _db.BookingQueues
                    .Where(q => q.counter == counter && q.queue_status_id == 2 
                        && q.appointment_date.Date == DateTime.Now.Date)
                    .FirstOrDefaultAsync();

                if (nowQueue != null)
                {
                    nowQueue.queue_status_id = 3;
                    nowQueue.counter = 0;

                    if (waitQueue != null)
                    {
                        waitQueue.queue_status_id = 2;
                        waitQueue.counter = counter;

                    }
                    await _db.SaveChangesAsync();
                    return NoContent();
                }

                throw new Exception("No now queue");
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }
        }

    }
}
