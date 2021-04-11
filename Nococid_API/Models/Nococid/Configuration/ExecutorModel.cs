using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid.Configuration
{
    public class ExecutorCreateM
    {
        public string Name { get; set; }
        public string Language { get; set; }
    }

    public class ExecutorUpdateM
    {
        public string Name { get; set; }
        public string Language { get; set; }
    }
}
