using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class FrameworkM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class FrameworkDM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public LanguageM Language { get; set; }
    }

    public class FrameworkCreateM
    {
        public string Name { get; set; }
    }
}
