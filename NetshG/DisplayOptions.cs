using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetshG
{
    public class DisplayOptions
    {
        /// <summary>
        /// Switch for showing the text output versus the table output.
        /// </summary>
        public enum ShowWhat { Output, Table };
        public ShowWhat? CurrShowWhat = null;
    }
}
