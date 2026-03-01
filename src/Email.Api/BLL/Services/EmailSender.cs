using Email.Api.BLL.Abstract;
using Email.Api.BLL.Models;
using Email.Api.BLL.Services.MppTests;
using Email.Core.OperationResults.Base;
using Email.Core.OperationResults.Errors;
using MailKit.Net.Smtp;
using MimeKit;

namespace Email.Api.BLL.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _emailPassword;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(string smtpServer,
                           int smtpPort,
                           string fromEmail,
                           string emailPassword,
                           ILogger<EmailSender> logger)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _fromEmail = fromEmail;
            _emailPassword = emailPassword;
            _logger = logger;
        }

        public async Task<Result> SendEmail(string to, string subject, string body)
        {
            try
            {
                var mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress("DVV", _fromEmail));
                mailMessage.To.Add(new MailboxAddress("dvv", to));
                mailMessage.Subject = subject;
                mailMessage.Body = new TextPart("plain")
                {
                    Text = body
                };

                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(_smtpServer, _smtpPort, useSsl: true);
                    await smtpClient.AuthenticateAsync(_fromEmail, _emailPassword);
                    await smtpClient.SendAsync(mailMessage);
                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email. Error: {ex.Message}");
                return Result.Failure(EmailErrors.SendError());
            }

            return Result.Success();
        }

        public async Task<Result> SendTestResults(string to, EmailDataDto emailData, byte[] imageBytes, string imageFileName = "test-result.png")
        {
            try
            {
                // Создаем тело письма
                var bodyBuilder = new EmailBodyBuilder("Уважаемый пользователь");
                string emailBody = bodyBuilder.BuildEmailBody(emailData);

                // Создаем HTML версию письма
                string htmlBody = bodyBuilder.BuildHtmlEmail(emailData);

                var mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress("Медико-педагого-психологические тесты", _fromEmail));
                mailMessage.To.Add(new MailboxAddress("", to));
                mailMessage.Subject = $"Результаты тестирования от {DateTime.Now:dd.MM.yyyy}";

                // Создаем multipart сообщение
                var multipart = new Multipart("mixed");

                // Создаем alternative часть для текста и HTML
                var alternative = new Multipart("alternative");

                // Добавляем текстовую версию
                alternative.Add(new TextPart("plain") { Text = emailBody });

                // Добавляем HTML версию
                alternative.Add(new TextPart("html") { Text = htmlBody });

                // Добавляем alternative в основное сообщение
                multipart.Add(alternative);

                // Добавляем изображение как вложение
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    var attachment = new MimePart("image", "png")
                    {
                        Content = new MimeContent(new MemoryStream(imageBytes)),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = imageFileName
                    };

                    multipart.Add(attachment);

                    // Также добавляем изображение как embedded для отображения в HTML (опционально)
                    var embeddedImage = new MimePart("image", "png")
                    {
                        Content = new MimeContent(new MemoryStream(imageBytes)),
                        ContentId = "test-image",
                        ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = imageFileName
                    };

                    // Обновляем HTML, чтобы показать изображение
                    string htmlWithImage = htmlBody.Replace("</body>",
                        $"<div style='margin-top: 30px; text-align: center;'>" +
                        $"<h3 style='color: #667eea;'>Раскрашенное изображение:</h3>" +
                        $"<img src='cid:test-image' style='max-width: 100%; border: 1px solid #e0e0e0; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);' />" +
                        $"</div></body>");

                    // Заменяем HTML версию
                    alternative.Clear();
                    alternative.Add(new TextPart("plain") { Text = emailBody });
                    alternative.Add(new TextPart("html") { Text = htmlWithImage });
                    multipart.Add(embeddedImage);
                }

                mailMessage.Body = multipart;

                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(_smtpServer, _smtpPort, useSsl: true);
                    await smtpClient.AuthenticateAsync(_fromEmail, _emailPassword);
                    await smtpClient.SendAsync(mailMessage);
                    await smtpClient.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email sent successfully to {to}");
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {to}. Error: {ex.Message}");
                return Result.Failure(EmailErrors.SendError());
            }
        }
    }
}
