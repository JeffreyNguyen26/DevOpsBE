using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class LanguageM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Side { get; set; }
    }

    public class LanguageCreateM
    {
        public string Name { get; set; }
        public string Side { get; set; }
    }

    public class LanguageDM : LanguageM
    {
        public IList<FrameworkM> Frameworks { get; set; }
    }

    public class LanguageUpdateM
    {
        public string Name { get; set; }
        public string Side { get; set; }
    }
}
