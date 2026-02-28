using Email.Api.BLL.Abstract;
using Email.Api.BLL.Services.MppTests;
using Email.Models.MppTests;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Email.Api.Controllers
{
    [Route("mpptests")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class MppTestsController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<MppTestsController> _logger;

        public MppTestsController(
            IEmailSender emailSender,
            ILogger<MppTestsController> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpPost("send2")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
        public async Task<IActionResult> SendEmail2([FromForm] EmailRequest2 request)
        {
            _logger.LogInformation("mpptests send2");            

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Игнорировать регистр
                };
                var userData = JsonSerializer.Deserialize<UserData>(request.UserData, options);
                _logger.LogInformation(userData.SelectedTest);                
                var results = JsonSerializer.Deserialize<AnalysisResult>(request.UserData, options);
                _logger.LogInformation(results.MainCharacteristic);

            }
            catch (Exception ex)
            {
                _logger.LogError("error deserialize", ex);
            }

            return Ok(new { success = true, message = "Email sent successfully" });
        }

        [HttpPost("send")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
        public async Task<IActionResult> SendEmail([FromForm] EmailRequest request)
        {
            try
            {
                _logger.LogInformation("mpptests send");                
                // Validate email
                if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
                {
                    return BadRequest(new { success = false, message = "Invalid email address" });
                }

                // Validate image
                //if (request.Image == null || request.Image.Length == 0)
                //{
                //    return BadRequest(new { success = false, message = "Image is required" });
                //}

                // Convert IFormFile to byte[]
                byte[] imageBytes = new byte[] { 1,2,3,4,5,6,7};
                //using (var memoryStream = new MemoryStream())
                //{
                //    await request.Image.CopyToAsync(memoryStream);
                //    imageBytes = memoryStream.ToArray();
                //}

                // Prepare email data
                var emailData = new EmailData
                {
                    UserData = request.UserData,
                    Stats = request.Stats,
                    Results = request.Results
                };

                // Send email with attachment
                var result = await _emailSender.SendTestResults(
                    request.Email,
                    emailData,
                    imageBytes
                );

                if (result.IsSuccess)
                {
                    return Ok(new { success = true, message = "Email sent successfully" });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = result.Error.Description });
                }                
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

        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogInformation("test");
            return Ok("123");
        }
    }
}
