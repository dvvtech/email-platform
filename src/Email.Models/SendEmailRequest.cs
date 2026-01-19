
namespace Email.Models
{
    public class SendEmailRequest
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string FromEmail { get; set; }
        //public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string RecaptchaToken { get; set; }

        public int Type { get; set; }
    }
}
