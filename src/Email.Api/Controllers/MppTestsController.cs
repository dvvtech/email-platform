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
    //[EnableCors("AllowAll")]
    public class MppTestsController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<MppTestsController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public MppTestsController(
            IEmailSender emailSender,
            IHttpClientFactory httpClientFactory,
            ILogger<MppTestsController> logger)
        {
            _emailSender = emailSender;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost("send")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
        public async Task<IActionResult> SendEmail2([FromForm] EmailRequest request)
        {
            _logger.LogInformation("mpptests send2");

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

                _ = TrackVisitMppTestsAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Игнорировать регистр
                };
                var userData = JsonSerializer.Deserialize<UserData>(request.UserData, options);
                var results = JsonSerializer.Deserialize<AnalysisResult>(request.Results, options);
                var stats = JsonSerializer.Deserialize<Dictionary<string, ColorStatistic>>(request.Stats, options);

                _logger.LogInformation(userData.Gender);

                var emailData = new EmailData
                {
                    UserData = userData,
                    Stats = stats,
                    Results = results
                };

                // Convert IFormFile to byte[]
                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await request.Image.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }

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
                _logger.LogError("error deserialize", ex);
            }

            return Ok(new { success = true, message = "Email sent successfully" });
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

        private async Task TrackVisitMppTestsAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();

            var clientIp = GetRealClientIp(HttpContext);

            // Создаем запрос к analytics
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "http://analytics-api-container:8080/v1/analytics/track-mpptests");

            request.Headers.Add("X-Forwarded-For", clientIp);
            request.Headers.Add("X-Real-IP", clientIp);
            request.Headers.Add("X-Operation-Type", "send email");

            // Прокидываем оригинальный User-Agent
            var userAgent = Request.Headers["User-Agent"].ToString();
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.Headers.Add("User-Agent", userAgent);
            }

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Analytics tracking failed: {response.StatusCode}");
            }
        }

        private string GetRealClientIp(HttpContext context)
        {
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // Берем первый IP из цепочки (реальный клиентский)
                return forwardedFor.Split(',').First().Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // Если нет заголовков, используем RemoteIpAddress
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
