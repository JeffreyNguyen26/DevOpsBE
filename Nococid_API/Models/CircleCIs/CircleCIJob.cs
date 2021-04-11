using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.CircleCIs
{
    public class CircleCIJob
    {
        public string Status { get; set; }
        public long? Duration { get; set; }
    }

    public class CircleCIJobTestMetaDataM
    {
        public string Next_page_token { get; set; }
        public IList<CircleCIJobTestItemM> Items { get; set; }
    }

    public class CircleCIJobTestItemM
    {
        public string Name { get; set; }
        public string Classname { get; set; }
        public string Result { get; set; }
        public double Run_time { get; set; }
        public string Message { get; set; }
    }
}
