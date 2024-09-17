using ApiIsocare2.Data;
using ApiIsocare2.Models;
using ApiIsocare2.Utilities;
using ApiIsocare2.Utilities.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiIsocare2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _emailService;


        public ResetPasswordController(AppDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("ต้องระบุที่อยู่อีเมล.");
                }

                var trimmedEmail = email.Trim().ToLower();
                var user = await _db.Users
                    .SingleOrDefaultAsync(u => !string.IsNullOrEmpty(u.user_email) && u.user_email.Trim().ToLower() == trimmedEmail);

                if (user == null)
                {
                    return BadRequest("ไม่พบอีเมลนี้ในระบบ.");
                }

                // สร้าง OTP
                var otp = new Random().Next(100000, 999999).ToString();
                user.reset_token = otp;
                user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // รหัสใช้ได้ 1 ชั่วโมง

                // บันทึกการเปลี่ยนแปลงในฐานข้อมูล
                await _db.SaveChangesAsync();

                // ส่ง OTP ไปยังอีเมล
                var subject = "คำขอรีเซ็ตรหัสผ่าน";
                var message = $"รหัส OTP ของคุณสำหรับรีเซ็ตรหัสผ่านคือ: {otp}";
                await _emailService.SendEmailAsync(email, subject, message);
                
                return Ok(new { Message = "ส่ง OTP ไปยังอีเมลเรียบร้อยแล้ว." });
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}. Inner Exception: {innerException}");
            }
        }

        [HttpPost("otp-check")]
        public IActionResult OtpCheck([FromBody] string Otp)
        {
            try
            {
                var user = _db.Users.SingleOrDefault(u => u.reset_token == Otp && u.ResetTokenExpiry > DateTime.UtcNow);
                if (user == null)
                {
                    // หาก OTP ไม่ถูกต้องหรือหมดอายุ ให้ลบ otp และ ResetTokenExpiry
                    var failedUser = _db.Users.SingleOrDefault(u => u.reset_token == Otp);
                    if (failedUser != null)
                    {
                        failedUser.reset_token = null;
                        failedUser.ResetTokenExpiry = null;
                        _db.SaveChanges();
                    }
                    return BadRequest("OTP ไม่ถูกต้องหรือหมดอายุ. กรุณาขอ OTP ใหม่.");
                }
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [HttpPost("resetpassword")]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            try
            {
                var user = _db.Users.SingleOrDefault(u => u.reset_token == model.otp && u.ResetTokenExpiry > DateTime.UtcNow);
                if (user == null)
                {
                    return BadRequest("OTP ไม่ถูกต้องหรือหมดอายุ.");
                }
                var hashedPassword = PasswordHasher.HashPassword(model.newPassword);
                user.password = hashedPassword;
                user.reset_token = null; // ลบรหัส OTP
                user.ResetTokenExpiry = null;
                _db.SaveChanges();

                return Ok(new { Message = "รีเซ็ตรหัสผ่านเรียบร้อยแล้ว." });
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }

        }
    }
}
