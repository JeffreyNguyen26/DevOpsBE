using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Attributes
{
    public class AuthorizedExtensionFilterAttribute : TypeFilterAttribute
    {
        public AuthorizedExtensionFilterAttribute(Type implementation) : base(implementation) { }
    }
}
