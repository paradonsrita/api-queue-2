using ApiIsocare2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiIsocare2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public StatisticsController(AppDbContext db) 
        {
            _db = db;
        }

        [HttpGet("daily-statistics")]
        public IActionResult DailyStatistics()
        {
            try
            {
                var bookingStatistics = _db.BookingQueues
                    .Include(q => q.QueueType)
                    .Include(q => q.QueueStatus) // Include QueueStatus if needed

                    .Where(q => q.appointment_date.Date == DateTime.Today)

                    .Select(q => new
                    {
                        q.queue_id,
                        queue_date = q.appointment_date,
                        QueueStatus = q.QueueStatus.queue_status_name,
                        QueueType = q.QueueType.type_name,
                        QueueNumber = q.queue_type_id.ToUpper() + q.queue_number.ToString("000"),
                        q.counter,
                        Source = "booking"
                    })
                    .OrderBy(q => q.queue_date)
                    .ToList();

                var counterStatistics = _db.CounterQueues
                    .Include(q => q.QueueType)
                    .Include(q => q.QueueStatus) // Include QueueStatus if needed
                    .Where(q => q.queue_date.Date == DateTime.Today)
                    .Select(q => new
                    {
                        q.queue_id,
                        q.queue_date,
                        QueueStatus = q.QueueStatus.queue_status_name,
                        QueueType = q.QueueType.type_name,
                        QueueNumber = q.queue_type_id.ToUpper() + q.queue_number.ToString("000"),
                        q.counter,
                        Source = "counter"
                    })
                    .ToList();

                var totalStatistics = bookingStatistics
                    .Concat(counterStatistics)
                    .OrderBy(q => q.Source)
                    .ThenBy(q => q.queue_date)
                    .ToList();

                
                return Ok(totalStatistics);
                
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }

        }

        [HttpGet("booking-statistics")]
        public IActionResult BookingStatistics([FromQuery] DateTime minDate, [FromQuery] DateTime maxDate)
        {
            try
            {
                if (minDate > maxDate)
                {
                    return BadRequest("minDate should be less then maxDate");
                }

                var bookingQueue = _db.BookingQueues
                    .Include(q => q.QueueType)
                    .Where(q => q.appointment_date >= minDate && q.appointment_date <= maxDate)
                    .GroupBy(q => new 
                    { 
                        q.QueueType.type_name,
                        Source = "booking",
                        Date = q.appointment_date.Date,
                        /*Time = q.appointment_date.TimeOfDay == TimeSpan.FromHours(8)
                                ? "08:00:00"
                                : q.appointment_date.TimeOfDay == TimeSpan.FromHours(13)
                                    ? "13:00:00"
                                    : "Other"*/
                    })
                    .Select(group => new
                    {
                        group.Key.Date,
                        group.Key.Source,
                        group.Key.type_name,
                        Total = group.Count()
                    })
                    .ToList();

                var counterQueue = _db.CounterQueues
                    .Include(q => q.QueueType)
                    .Where(q => q.queue_date >= minDate && q.queue_date <= maxDate)
                    .GroupBy(q => new
                    {
                        q.QueueType.type_name,
                        Source = "counter",
                        Date = q.queue_date.Date,
                    })
                    .Select(group => new
                    {
                        group.Key.Date,
                        group.Key.Source,
                        group.Key.type_name,
                        Total = group.Count()
                    })
                    .ToList();

                var totalQueue = bookingQueue
                    .Concat(counterQueue)
                    .OrderBy(q => q.Date)
                    .ThenBy(q => q.Source)
                    .ThenBy (q => q.type_name)
                    .ToList();

                return Ok(totalQueue);
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }

        }
    }
}


