using System.ComponentModel.DataAnnotations;

namespace ApiIsocare2.Models
{
    public class QueueType
    {
        [Key]
        public string queue_type_id { get; set; }
        public string type_name { get; set; }
    }
}
