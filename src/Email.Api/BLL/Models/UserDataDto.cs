namespace Email.Api.BLL.Models
{
    public class UserDataDto
    {
        /// <summary>
        /// User gender (Male/Female)
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// User birth date (YYYY-MM-DD)
        /// </summary>
        public string BirthDate { get; set; }

        /// <summary>
        /// User age (calculated from birth date)
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// User zodiac sign (based on birth date)
        /// </summary>
        public string ZodiacSign { get; set; }

        /// <summary>
        /// Selected test name
        /// </summary>
        public string SelectedTest { get; set; }
    }
}
