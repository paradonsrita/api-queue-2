using System.ComponentModel.DataAnnotations;

namespace ApiIsocare2.Models
{
    public class LoginModel
    {
        [Required]
        public string citizenId { get; set; }
        [Required]
        public string password { get; set; }
    }
}
