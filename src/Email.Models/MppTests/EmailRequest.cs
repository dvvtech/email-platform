using Microsoft.AspNetCore.Http;

namespace Email.Models.MppTests
{
    public class EmailRequest
    {
        /// <summary>
        /// Recipient email address
        /// </summary>
        public string Email { get; set; }
        
        public string UserData { get; set; }

        public string Results { get; set; }

        public IFormFile Image { get; set; }
                
        public string Stats { get; set; }
    }
}
