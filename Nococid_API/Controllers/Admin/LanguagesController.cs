using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Models.Nococid;
using Nococid_API.Services;
using Nococid_API.Services.Nococid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController : NococidControllerBase
    {
        private readonly ILanguageService _language;

        public LanguagesController(ILanguageService language, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _language = language;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] LanguageCreateM model)
        {
            try
            {
                return Created("", _language.Add(model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPut("{language_id}/update")]
        public IActionResult Update([FromRoute] Guid language_id, [FromBody] LanguageUpdateM model)
        {
            try
            {
                return Ok(_language.Update(language_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpDelete("{language_id}/delete")]
        public IActionResult Delete([FromRoute] Guid language_id)
        {
            try
            {
                _language.Delete(language_id);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
