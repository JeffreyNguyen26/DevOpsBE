using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.CircleCIs
{
    public class ConfigM
    {
        public string stageCode { get; set; }
        public string stageName { get; set; }
        public string stageDefaultContent { get; set; }
        public string machinePath { get; set; }
        public bool disable { get; set; }
    }

    public class ConfigTestM : ConfigM
    {
        public string testName { get; set; }
        public string testCommand { get; set; }
        public string testStorage { get; set; }
    }

    public class ConfigBuildM : ConfigM
    {
        public string buildName { get; set; }
        public string buildCommand { get; set; }
        public string buildStorage { get; set; }
    }

    public class ConfigDeployM : ConfigM
    {
        public string deployName { get; set; }
        public string deployCommand { get; set; }
        public string deployStorage { get; set; }
    }

    public class ConfigDM : ConfigM
    {
        public string testName { get; set; }
        public string testCommand { get; set; }
        public string testStorage { get; set; }

        public string buildName { get; set; }
        public string buildCommand { get; set; }
        public string buildStorage { get; set; }

        public string deployName { get; set; }
        public string deployCommand { get; set; }
        public string deployStorage { get; set; }
    }
}
