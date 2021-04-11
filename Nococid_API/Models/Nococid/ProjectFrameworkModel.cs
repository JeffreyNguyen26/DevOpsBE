using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{

    public class ProjectFrameworkM : ProjectM
    {
        public IList<LanguageDM> Languages { get; set; }
    }
}
