using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Examples.Submission
{
    public class ReportErrorMExample
    {
        public Guid TaskId = Guid.Empty;
        public int Passed = 5;
        public int Failed = 2;
        public string Message = "Tester write note";
    }
}
