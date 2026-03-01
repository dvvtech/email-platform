namespace Email.Api.BLL.Models
{
    public class AnalysisResultDto
    {
        /// <summary>
        /// Main characteristic of the test result
        /// </summary>
        public string MainCharacteristic { get; set; }

        /// <summary>
        /// List of strengths identified in the test
        /// </summary>
        public List<string> Strengths { get; set; }

        /// <summary>
        /// List of recommendations based on the test
        /// </summary>
        public List<string> Recommendations { get; set; }
    }
}
