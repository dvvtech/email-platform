using Email.Models.MppTests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Email.Api.Controllers
{
    [Route("mpptests")]
    [ApiController]
    public class MppTestsController : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromForm] EmailRequest request)
        {
            try
            {
                // Validate email
                if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
                {
                    return BadRequest(new { success = false, message = "Invalid email address" });
                }

                // Validate image
                if (request.Image == null || request.Image.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Image is required" });
                }

                // Process the request (send email, save to database, etc.)
                // ...

                return Ok(new { success = true, message = "Email sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
