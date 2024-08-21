namespace ApiIsocare2.Models
{
    public class ResetPasswordModel
    {
        public string otp { get; set; }
        public string? newPassword { get; set; }
    }
}
