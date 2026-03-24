//using MailKit.Net.Smtp;
//using MailKit.Security;
//using MimeKit;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TestSendEmail
//{
//    public class GmailSmtpService
//    {
//        private readonly GmailOAuthService _oauthService;
//        private readonly string _senderEmail;

//        public GmailSmtpService(GmailOAuthService oauthService, string senderEmail)
//        {
//            _oauthService = oauthService;
//            _senderEmail = senderEmail;
//        }

//        public async Task SendEmailAsync(string toEmail, string subject, string body)
//        {
//            // 1. Создаем сообщение
//            var message = new MimeMessage();
//            message.From.Add(new MailboxAddress("Your Name", _senderEmail));
//            message.To.Add(new MailboxAddress("", toEmail));
//            message.Subject = subject;
//            message.Body = new TextPart("html") { Text = body };

//            // 2. Получаем access token
//            var accessToken = await _oauthService.GetAccessTokenAsync();

//            // 3. Отправляем через SMTP
//            using var client = new SmtpClient();
//            try
//            {
//                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

//                // Используем XOAUTH2 механизм аутентификации
//                var oauth2 = new SaslMechanismOAuth2(_senderEmail, accessToken);
//                await client.AuthenticateAsync(oauth2);

//                await client.SendAsync(message);
//                Console.WriteLine("Email sent successfully!");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error sending email: {ex.Message}");
//                throw;
//            }
//            finally
//            {
//                await client.DisconnectAsync(true);
//            }
//        }
//    }
//}
