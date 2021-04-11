using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IFrameworkService
    {
        FrameworkDM Add(Guid language_id, FrameworkCreateM model);
    }

    public class FrameworkService : ServiceBase, IFrameworkService
    {
        private readonly IContext<Framework> _framework;
        private readonly IContext<Language> _language;

        public FrameworkService(IContext<Language> language, IContext<Framework> framework, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _language = language;
            _framework = framework;
        }

        public FrameworkDM Add(Guid language_id, FrameworkCreateM model)
        {
            try
            {
                Language language = _language.GetOne(l => l.Id.Equals(language_id));
                if (language == null) throw NotFound(language_id, "language id");
                if (_framework.Any(f => f.Name.Equals(model.Name) && f.LanguageId.Equals(language_id))) throw BadRequest("This language is already existed!");
                
                var framework = _framework.Add(new Framework
                {
                    LanguageId = language_id,
                    Name = model.Name
                });
                SaveChanges();

                return new FrameworkDM
                {
                    Language = new LanguageDM
                    {
                        Id = language.Id,
                        Name = language.Name,
                        Side = language.Side
                    },
                    Name = framework.Name,
                    Id = framework.Id
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add a framework!",
                    e, DateTime.Now, "Server", "Service_Framework_Add");
            }
        }

        private int SaveChanges()
        {
            return _framework.SaveChanges();
        }
    }
}