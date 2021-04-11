using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
using Nococid_API.Data;
using Nococid_API.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nococid_API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[AuthorizedExtensionFilter(typeof(ApplicationAdminFilter))]
    public class DatabaseContextsController : ControllerBase
    {
        private readonly NococidContext _context;

        public DatabaseContextsController(NococidContext context)
        {
            _context = context;
        }

        [HttpPost("save-database")]
        public IActionResult UpdateDatabase()
        {
            try
            {
                _context.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(e.Message + "\n" + e.StackTrace);
            }
        }

        [HttpPost("init-database")]
        public IActionResult Init()
        {
            try
            {
                _context.AddDataFromBackup();
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(e.Message + "\n" + e.StackTrace);
            }
        }

    }
}
