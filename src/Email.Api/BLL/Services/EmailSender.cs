using Email.Api.BLL.Abstract;
using Email.Api.BLL.Services.MppTests;
using Email.Core.OperationResults.Base;
using Email.Core.OperationResults.Errors;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;

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

        public async Task<Result> SendTestResults(string to, EmailData emailData, byte[] imageBytes, string imageFileName = "test-result.png")
        {
            try
            {
                // Создаем тело письма
                var bodyBuilder = new EmailBodyBuilder("Уважаемый пользователь");
                string emailBody = bodyBuilder.BuildEmailBody(emailData);

                // Создаем HTML версию письма
                string htmlBody = BuildHtmlEmail(emailData);

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

        // Перегрузка метода для работы с IFormFile
        public async Task<Result> SendTestResults(string to, EmailData emailData, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return Result.Failure(EmailErrors.NoImage());
            }

            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            return await SendTestResults(to, emailData, imageBytes, imageFile.FileName);
        }

        private string BuildHtmlEmail(EmailData emailData)
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
            html.AppendLine(".container { max-width: 600px; margin: 0 auto; padding: 20px; }");
            html.AppendLine(".header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px 10px 0 0; }");
            html.AppendLine(".content { background: #fff; padding: 20px; border: 1px solid #e0e0e0; border-radius: 0 0 10px 10px; }");
            html.AppendLine(".section { margin-bottom: 30px; }");
            html.AppendLine(".section-title { font-size: 18px; font-weight: bold; color: #667eea; margin-bottom: 15px; border-bottom: 2px solid #667eea; padding-bottom: 5px; }");
            html.AppendLine(".color-item { margin-bottom: 10px; }");
            html.AppendLine(".color-bar { height: 20px; border-radius: 10px; margin-top: 5px; }");
            html.AppendLine(".footer { margin-top: 30px; text-align: center; color: #999; font-size: 12px; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div class='container'>");
            html.AppendLine("<div class='header'><h2>Результаты тестирования</h2></div>");
            html.AppendLine("<div class='content'>");

            // Информация о пользователе
            html.AppendLine("<div class='section'>");
            html.AppendLine("<div class='section-title'>📋 Информация о пользователе</div>");
            html.AppendLine($"<p><strong>Пол:</strong> {GetGenderText(emailData.UserData.Gender)}</p>");
            html.AppendLine($"<p><strong>Дата рождения:</strong> {emailData.UserData.BirthDate}</p>");
            html.AppendLine($"<p><strong>Возраст:</strong> {emailData.UserData.Age} лет</p>");
            html.AppendLine($"<p><strong>Знак зодиака:</strong> {emailData.UserData.ZodiacSign}</p>");
            html.AppendLine($"<p><strong>Выбранный тест:</strong> {emailData.UserData.SelectedTest}</p>");
            html.AppendLine("</div>");

            // Статистика цветов
            html.AppendLine("<div class='section'>");
            html.AppendLine("<div class='section-title'>🎨 Использованные цвета</div>");

            foreach (var stat in emailData.Stats.OrderByDescending(s => s.Value.Percentage))
            {
                html.AppendLine("<div class='color-item'>");
                html.AppendLine("<div style='display: flex; justify-content: space-between;'>");
                html.AppendLine($"<span><strong>{stat.Key}</strong></span>");
                html.AppendLine($"<span>{stat.Value.Percentage}% ({stat.Value.Count:N0} пикселей)</span>");
                html.AppendLine("</div>");
                html.AppendLine($"<div class='color-bar' style='background: linear-gradient(90deg, {stat.Value.Hex} {stat.Value.Percentage}%, #e0e0e0 {stat.Value.Percentage}%);'></div>");
                html.AppendLine("</div>");
            }

            int totalPixels = emailData.Stats.Sum(s => s.Value.Count);
            html.AppendLine($"<p style='margin-top: 15px;'><strong>Всего раскрашено пикселей:</strong> {totalPixels:N0}</p>");
            html.AppendLine("</div>");

            // Результаты анализа
            html.AppendLine("<div class='section'>");
            html.AppendLine("<div class='section-title'>📊 Результаты анализа</div>");
            html.AppendLine($"<p><strong>Основная характеристика:</strong><br>{emailData.Results.MainCharacteristic}</p>");

            if (emailData.Results.Strengths?.Any() == true)
            {
                html.AppendLine("<p><strong>Сильные стороны:</strong></p>");
                html.AppendLine("<ul>");
                foreach (var strength in emailData.Results.Strengths)
                {
                    html.AppendLine($"<li>✓ {strength}</li>");
                }
                html.AppendLine("</ul>");
            }

            if (emailData.Results.Recommendations?.Any() == true)
            {
                html.AppendLine("<p><strong>Рекомендации:</strong></p>");
                html.AppendLine("<ul>");
                foreach (var recommendation in emailData.Results.Recommendations)
                {
                    html.AppendLine($"<li>• {recommendation}</li>");
                }
                html.AppendLine("</ul>");
            }

            html.AppendLine("</div>");

            // Подвал
            html.AppendLine("<div class='footer'>");
            html.AppendLine("<p>С уважением,<br>Команда Медико-педагого-психологических тестов<br>Яшара Ибадова</p>");
            html.AppendLine("<p style='font-size: 10px;'>Это письмо сформировано автоматически. Пожалуйста, не отвечайте на него.</p>");
            html.AppendLine("</div>");

            html.AppendLine("</div>");
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private string GetGenderText(string gender)
        {
            return gender?.ToLower() switch
            {
                "male" => "Мужской",
                "female" => "Женский",
                "м" => "Мужской",
                "ж" => "Женский",
                _ => "Не указан"
            };
        }
    }
}
