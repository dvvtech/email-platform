using System.ComponentModel.DataAnnotations;

namespace Email.Api.Configuration
{
    public class SmtpConfig
    {
        public const string SectionName = "SmtpConfig";

        [Required]
        public string Host { get; init; }

        [Required]
        public int Port { get; init; }

        [Required]
        public string Username { get; init; }

        [Required]
        public string Password { get; init; }
    }
}
