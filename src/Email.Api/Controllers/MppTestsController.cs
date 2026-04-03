using Email.Api.AppStart;
using Email.Api.BLL.Abstract;
using Email.Api.BLL.Models;
using Email.Api.Extensions;
using Email.Models.MppTests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;

namespace Email.Api.Controllers
{
    [Route("mpptests")]
    [ApiController]    
    public class MppTestsController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly IAnalyticsTrackingService _analyticsTrackingService;
        private readonly ILogger<MppTestsController> _logger;        
        private readonly IValidator<EmailRequest> _emailRequestValidator;

        public MppTestsController(
            IEmailSender emailSender,
            IAnalyticsTrackingService analyticsTrackingService,            
            IValidator<EmailRequest> emailRequestValidator,
            ILogger<MppTestsController> logger)
        {
            _emailSender = emailSender;
            _analyticsTrackingService = analyticsTrackingService;            
            _emailRequestValidator = emailRequestValidator;
            _logger = logger;
        }        

        [HttpPost("send")]
        [EnableRateLimiting("EmailRequests")]
        [RequestSizeLimit(7 * 1024 * 1024)] // 8MB limit - общий лимит запроса
        public async Task<IActionResult> SendEmail([FromForm] EmailRequest request)
        {
            _logger.LogInformation("mpptests send email");

            try
            {
                var validationResult = await _emailRequestValidator.ValidateAsync(request);                
                if (!validationResult.IsValid)
                {                    
                    return BadRequest(validationResult.ToProblemDetails());
                }

                TrackVisitMppTests();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Игнорировать регистр
                };

                var userData = JsonSerializer.Deserialize<UserDataDto>(request.UserData, options);
                var results = JsonSerializer.Deserialize<AnalysisResultDto>(request.Results, options);
                var stats = JsonSerializer.Deserialize<ColorStatisticDto[]>(request.Stats, options);
                
                var emailData = new EmailDataDto
                {
                    UserData = userData,
                    //Stats = stats,
                    Stats2 = stats,
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

        private void TrackVisitMppTests(CancellationToken cancellationToken = default)
        {
            var clientIp = HttpContext.GetRealClientIp();
            var userAgent = Request.Headers["User-Agent"].ToString();

            _ = _analyticsTrackingService.TrackVisitAsync("send email", clientIp, userAgent, cancellationToken);
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogInformation("mpptest");

            //var options = new JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true // Игнорировать регистр
            //};
            //var test = "[{\"name\":\"Красный\",\"count\":7533,\"percentage\":48.7320481304179,\"hex\":\"#ef4444\"},{\"name\":\"Желтый\",\"count\":6659,\"percentage\":43.07801785483245,\"hex\":\"#eab308\"},{\"name\":\"Коричневый\",\"count\":1266,\"percentage\":8.189934014749644,\"hex\":\"#92400e\"}]";
            //var results = JsonSerializer.Deserialize<ColorStatisticDto[]>(test, options);

            return Ok("mpptest");
        }
    }
}
