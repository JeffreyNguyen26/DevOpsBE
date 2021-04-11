using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class ToolM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string[] ToolType { get; set; }
    }

    public class StageToolsM
    {
        public string Name { get; set; }
        public IList<ToolM> Tools { get; set; }
    }
}
