using System.Security.Claims;
using ApiIsocare2.Data;
using ApiIsocare2.Models;
using ApiIsocare2.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiIsocare2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        public UserController(AppDbContext db, IConfiguration Configuration)
        {
            _db = db;
            _configuration = Configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserData()
        {
            try
            {


                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = JwtHelper.GetUserIdFromToken(token, _configuration);

                if (!userId.HasValue)
                {
                    return Unauthorized("Invalid token.");
                }

                

                var result = await _db.Users
                    .Where(u => u.user_id == userId.Value)
                    .Select(u => new 
                    {
                        u.user_id,
                        u.citizen_id_number,
                        u.firstname,
                        u.lastname,
                        u.phone_number,
                        u.user_email
                    })
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(result);

            }
            catch(Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditProfile(User newUser)
        {
            try
            {
                var oldUser = _db.Users
                                .Where(u => u.user_id == newUser.user_id)
                                .FirstOrDefault();


                if (oldUser == null)
                {
                    return NotFound();
                }

                oldUser.phone_number = newUser.phone_number;
                oldUser.user_email = newUser.user_email;
                await _db.SaveChangesAsync();

                return Ok("แก้ไขเรียบร้อย");
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(500, $"Error : {ex.Message}, Inner Exception : {innerExceptionMessage}");
            }

        }
    }
}
