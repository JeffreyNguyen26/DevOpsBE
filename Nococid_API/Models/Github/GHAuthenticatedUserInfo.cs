using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Github
{
    public class GHAuthenticatedUserInfo
    {
        public GHToken Token { get; set; }
        public GHUser User { get; set; }
        public string Email { get; set; }
    }

    public class GHToken
    {
        public string Access_token { get; set; }
        public string Scope { get; set; }
        public string Token_type { get; set; }
    }

    public class GHUser
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string Avatar_url { get; set; }
    }

    public class GHEmail
    {
        public string Email { get; set; }
        public bool Primary { get; set; }
        public bool Verified { get; set; }
        public string Visibility { get; set; }
    }
}
