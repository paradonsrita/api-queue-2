using System.Linq;
using ApiIsocare2.Data;
using ApiIsocare2.Models;
using Microsoft.AspNetCore.Http;
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
        public IActionResult DailyStatistics([FromQuery] string type = "total")
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

                if (type == "counter")
                {
                    return Ok(counterStatistics);
                }
                else if (type == "booking")
                {
                    return Ok(bookingStatistics);
                }
                else
                {
                    return Ok(totalStatistics);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ("Error : " + ex.Message));
            }
            
        }

        [HttpGet("booking-statistics")]
        public IActionResult BookingStatistics([FromQuery] DateTime minDate, [FromQuery] DateTime maxDate)
        {
            try
            {
                if (minDate > maxDate)
                {
                    return BadRequest("minDate should be less maxDate");
                }

                var bookingQueue = _db.BookingQueues
                    .Where(q => q.appointment_date >= minDate && q.appointment_date <= maxDate)
                    .GroupBy(q => new { q.queue_type_id, Date = q.appointment_date.Date })
                    .Select(group => new
                    {
                        group.Key.queue_type_id,
                        group.Key.Date,
                        Total = group.Count()
                    })
                    .OrderBy(group => group.queue_type_id)
                    .ThenBy(group => group.Date)
                    .ToList();
                return Ok(bookingQueue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ("Error : " + ex.Message));
            }
            
        }
    }
}
