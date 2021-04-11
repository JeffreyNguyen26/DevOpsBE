using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Static
{
    public class RoleID
    {
        public static Guid Project_Manager { get; set; }
        public static Guid Technical_Manager { get; set; }
        public static Guid Project_Tester { get; set; }
        public static Guid Developer { get; set; }
        public static Guid Admin { get; set; }
        public static Guid[] All { get; set; }
    }
}
