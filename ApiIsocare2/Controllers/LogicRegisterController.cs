using ApiIsocare2.Data;
using ApiIsocare2.Models;
using ApiIsocare2.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiIsocare2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogicRegisterController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public LogicRegisterController(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;

        }

        [HttpPost("login")]
        public IActionResult Login(string citizenId,string password)
        {
            try
            {
                var hashedPassword = PasswordHasher.HashPassword(password);
                var user = _db.Users
                    .Where(u => u.citizen_id_number == citizenId && u.password == hashedPassword)
                    .Select(u => new { u.citizen_id_number, u.password })
                    .SingleOrDefault();


                if (user != null)
                {
                    var key = _configuration["Jwt:Key"];
                    if (key.Length < 16)
                    {
                        return StatusCode(500, "ข้อผิดพลาด: คีย์ JWT สั้นเกินไป.");
                    }
                    var token = JwtHelper.GenerateJwtToken(
                       user.citizen_id_number,
                        key,
                       _configuration["Jwt:Issuer"],
                       _configuration["Jwt:Audience"]
                    );
                    return Ok(new { Token = token });
                }

                return Unauthorized();

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error : {ex.Message}");
            }

        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User registerModel)
        {
            try
            {
                if (_db.Users.Any(u => u.citizen_id_number == registerModel.citizen_id_number))
                {
                    return BadRequest("Username already exists.");
                }

                var hashedPassword = PasswordHasher.HashPassword(registerModel.password);

                var newUser = new User
                {
                    firstname = registerModel.firstname,
                    lastname = registerModel.lastname,
                    phone_number = registerModel.phone_number,
                    citizen_id_number = registerModel.citizen_id_number,
                    user_email = registerModel.user_email,
                    password = hashedPassword
                };

                _db.Users.Add(newUser);
                _db.SaveChanges();

                return Ok(new { Message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}. Inner Exception: {innerException}");
            }

        }
    }
}
