using Nococid_API.Data.Models;
using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class InsightTestM
    {
        public int Passed { get; set; }
        public int Failed { get; set; }
        public string Side { get; set; }
        public ProjectType ProjectType { get; set; }
        public IList<InsightTaskSubmissionM> TaskSubmissions { get; set; }
    }

    public class InsightProjectTypeM
    {
        public string Name { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
    }

    public class InsightTaskSubmissionM
    {
        public int Passed { get; set; }
        public int Failed { get; set; }
        public string Side { get; set; }
    }

    public class InsightFrameworkM
    {
        public int Passed { get; set; }
        public int Failed { get; set; }
        public string Side { get; set; }
        public Project Project { get; set; }
        public IList<ProjectFramework> ProjectFrameworks { get; set; }
        public IList<InsightTaskSubmissionM> TaskSubmissions { get; set; }
    }

    public class TestResultM
    {
        public int Passed { get; set; }
        public int Failed { get; set; }
    }
}
