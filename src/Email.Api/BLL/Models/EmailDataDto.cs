using Email.Models.MppTests;

namespace Email.Api.BLL.Models
{
    /// <summary>
    /// Data models for email content
    /// </summary>
    public class EmailDataDto
    {
        public UserDataDto UserData { get; set; }
        public Dictionary<string, ColorStatisticDto> Stats { get; set; }
        public AnalysisResultDto Results { get; set; }
    }    
}
