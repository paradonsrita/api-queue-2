using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIsocare2.Models
{
    public class BookingQueue
    {
        [Key]
        [Required]
        public int queue_id { get; set; }
        [Required]
        public string queue_type_id { get; set; }
        [ForeignKey("queue_type_id")]
        public QueueType QueueType { get; set; }
        [Required]
        public int queue_number { get; set; }
        [Required]
        public int queue_status_id { get; set; }

        [ForeignKey("queue_status_id")]
        public QueueStatus QueueStatus { get; set; }
        [Required]
        public int user_id { get; set; }
        [ForeignKey("user_id")]
        public User User { get; set; }
        [Required]
        public DateTime booking_date { get; set; }
        [Required]
        public DateTime appointment_date { get; set; }

        public int counter { get; set; }
    }
}
