using Nococid_API.Authentication;
using Nococid_API.Data.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Examples.Users
{
    public class UserLoginMExample
    {
        public string Username = "ThangNLD";
        public string Password = "zaq@123";
    }

    public class UserLoginMExamples : ExampleBase, IExampleBase
    {
        public UserLoginMExample ApplicationAdmin
            = new UserLoginMExample
            {
                Username = ApplicationAuth.Nococid_Application_Admin,
                Password = ApplicationAuth.Nococid_Application_Admin_Password
            };
        public UserLoginMExample Prm
            = new UserLoginMExample
            {
                Username = "ThangNLD",
                Password = "zaq@123"
            };
        public UserLoginMExample Tech
            = new UserLoginMExample
            {
                Username = "ToanLD",
                Password = "zaq@123"
            };

        public string GetExamples()
        {
            return base.ConvertExample(this);
        }
    }
}
