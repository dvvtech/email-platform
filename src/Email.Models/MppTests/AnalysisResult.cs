using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email.Models.MppTests
{
    public class AnalysisResult
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
