// See https://aka.ms/new-console-template for more information
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MailKit.Net.Smtp;
using MimeKit;

try
{
    var smtpServer = "smtp.mail.ru";
    var smtpPort = 465;//587
    var fromEmail = "mpptests@mail.ru";
    var emailPassword = "";
    var to = "dvvtech@gmail.com";

    //var emailPassword = "nekw szdp sdam dmev";

    var mailMessage = new MimeMessage();
    mailMessage.From.Add(new MailboxAddress("DVV", fromEmail));
    mailMessage.To.Add(new MailboxAddress("dvv", to));
    mailMessage.Subject = "";
    mailMessage.Body = new TextPart("plain")
    {
        Text = "body"
    };

    using (var smtpClient = new SmtpClient())
    {
        await smtpClient.ConnectAsync(smtpServer, smtpPort, useSsl: true);
        await smtpClient.AuthenticateAsync(fromEmail, emailPassword);
        await smtpClient.SendAsync(mailMessage);
        await smtpClient.DisconnectAsync(true);
    }
}
catch (Exception ex)
{
}

Console.ReadLine();