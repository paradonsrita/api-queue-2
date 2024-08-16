using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIsocare2.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }

        [Required]
        [StringLength(50)]
        public string firstname { get; set; }
        [Required]
        [StringLength(50)]
        public string lastname { get; set; }
        [Required]
        [StringLength(15)]
        public string phone_number { get; set; }

        [Required]
        [StringLength(13)]
        public string citizen_id_number { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string user_email { get; set; }
        
        [Required]
        [StringLength(255)] // ขนาดของ hash password
        public string password { get; set; }


        /*{
  "firstname": "paradon",
  "lastname": "srita",
  "phone_number": "0951294423",
  "citizen_id_number": "1500701309052",
  "user_email": "paradonsrita2003@gmail.com",
  "username": "zone",
  "password": "asd",

}*/
        //forget password
        public string? reset_token { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }
}
