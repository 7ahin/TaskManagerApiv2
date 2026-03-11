using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Models;

namespace TaskManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TodoContext _context;

        public AuthController(TodoContext context)
        {
            _context = context;
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] string credential)
        {
            try
            {
                // 1. Verify the Google Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential);

                // 2. Check if user exists
                var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);

                if (user == null)
                {
                    // 3. Create new user if not exists
                    user = new User
                    {
                        GoogleId = payload.Subject,
                        Email = payload.Email,
                        Name = payload.Name
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // 4. Return the user info (For now. Later we will return a Session Token/JWT)
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Invalid Google Token", error = ex.Message });
            }
        }
    }
}