using ApiIsocare2.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                        name = $"{u.firstname} {u.lastname}",
                        u.phone_number,
                        u.user_email
                    })
                    .FirstOrDefault();

                return Ok(result);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
