using ApiIsocare2.Data;
using ApiIsocare2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;

namespace ApiIsocare2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UserController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserData(int userId)
        {
            try
            {
                var result = _db.Users
                    .Where(u => u.user_id == userId)
                    .Select(u => new 
                    {
                        u.user_id,
                        u.citizen_id_number,
                        u.firstname,
                        u.lastname,
                        u.phone_number,
                        u.user_email
                    })
                    .FirstOrDefault();

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return StatusCode(500, ex.Message);
            }
            
        }
    }
}
