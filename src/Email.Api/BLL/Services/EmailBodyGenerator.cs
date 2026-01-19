using Email.Api.BLL.Abstract;
using Email.Models;
using System.Text;

namespace Email.Api.BLL.Services
{
    public class EmailBodyGenerator : IEmailBodyGenerator
    {
        public string GenerateEmailBody(SendEmailRequest request)
        {
            return request.Type switch
            {
                1 => GenerateBodyForOxfordAp(request),
                2 => GenerateTypeFromYashelCenter(request),
                3 => GenerateTypeFromPingmetasks(request),
                _ => GenerateDefaultBody(request)
            };
        }

        private string GenerateBodyForOxfordAp(SendEmailRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Contact Form https://oxford-ap.com/");
            sb.AppendLine($"Name: {request.Name}");
            sb.AppendLine($"Surname: {request.Surname}");
            sb.AppendLine($"Email: {request.FromEmail}");
            sb.AppendLine($"Message: {request.Body}");
            return sb.ToString();
        }

        private string GenerateTypeFromYashelCenter(SendEmailRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Contact Form https://yashel.center/");
            sb.AppendLine($"Name: {request.Name}");
            sb.AppendLine($"Surname: {request.Surname}");
            sb.AppendLine($"Email: {request.FromEmail}");
            sb.AppendLine($"Message: {request.Body}");
            return sb.ToString();
        }

        private string GenerateTypeFromPingmetasks(SendEmailRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Ping Me Tasks");
            sb.AppendLine($"email: {request.FromEmail}</p>");
            return sb.ToString();
        }

        private string GenerateDefaultBody(SendEmailRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<h3>New Message</h3>");
            sb.AppendLine($"<p><strong>From:</strong> {request.Name} {request.Surname}</p>");
            sb.AppendLine($"<p><strong>Email:</strong> {request.FromEmail}</p>");
            sb.AppendLine($"<p><strong>Content:</strong></p><p>{request.Body}</p>");
            return sb.ToString();
        }
    }
}
