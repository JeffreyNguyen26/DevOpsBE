using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Enums
{
    public enum StageEnum : int
    {
        Planning = 0,
        Coding = 1,
        Build = 2,
        Test = 3,
        Release = 4,
        Deploy = 5,
        Operate = 6,
        Monitor = 7,
        Custom = 8
    }
}
