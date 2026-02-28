using Microsoft.AspNetCore.Http;

namespace Email.Models.MppTests
{
    public class EmailRequest2
    {
        /// <summary>
        /// Recipient email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User data (gender, birth date, age, zodiac sign, selected test)
        /// </summary>
        public string UserData { get; set; }

        public string Results { get; set; }

        /// <summary>
        /// Statistics of colors used in the test
        /// </summary>
        //public Dictionary<string, ColorStatistic> Stats { get; set; }
        //public string Stats { get; set; }
    }

        public class EmailRequest
    {
        /// <summary>
        /// Recipient email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User data (gender, birth date, age, zodiac sign, selected test)
        /// </summary>
        public UserData UserData { get; set; }

        /// <summary>
        /// Statistics of colors used in the test
        /// </summary>
        public Dictionary<string, ColorStatistic> Stats { get; set; }

        /// <summary>
        /// Analysis results (main characteristic, strengths, recommendations)
        /// </summary>
        public AnalysisResult Results { get; set; }

        /// <summary>
        /// Image file of the colored test
        /// </summary>
        //public IFormFile Image { get; set; }
    }
}
