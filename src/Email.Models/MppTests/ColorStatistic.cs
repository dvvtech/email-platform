using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email.Models.MppTests
{
    public class ColorStatistic
    {
        /// <summary>
        /// Number of pixels of this color
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Percentage of this color in the total colored area
        /// </summary>
        public int Percentage { get; set; }

        /// <summary>
        /// HEX color code (e.g., #ff0000)
        /// </summary>
        public string Hex { get; set; }
    }
}
