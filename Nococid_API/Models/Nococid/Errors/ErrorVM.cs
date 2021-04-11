using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid.Errors
{
    public class ErrorVM
    {
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public string Side { get; set; }
        public string Where { get; set; }
        public string ErrorMessage { get; set; }
        public string InnerErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }
}
