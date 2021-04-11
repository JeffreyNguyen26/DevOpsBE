using Microsoft.EntityFrameworkCore;
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
    public interface ILanguageService
    {
        LanguageM Add(LanguageCreateM model);
        void Delete(Guid language_id);
        IList<LanguageM> GetAll(string side);
        LanguageDM GetDetail(Guid language_id);
        LanguageM Update(Guid language_id, LanguageUpdateM model);
    }

    public class LanguageService : ServiceBase, ILanguageService
    {
        private readonly IContext<Language> _language;
        private readonly IContext<Framework> _framework;
        private readonly IContext<ProjectFramework> _projectFramework;

        public LanguageService(IContext<ProjectFramework> projectFramework, IContext<Framework> framework, IContext<Language> language, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _projectFramework = projectFramework;
            _framework = framework;
            _language = language;
        }

        public LanguageM Add(LanguageCreateM model)
        {
            try
            {
                if (!("Server".Equals(model.Side) || "Client".Equals(model.Side) || "Database".Equals(model.Side)))
                    throw BadRequest("Side value of framework must be 'Server', 'Client' or 'Database'!");
                if (_language.Any(l => l.Name.Equals(model.Name) && l.Side.Equals(model.Side))) throw BadRequest("This language is existed!");

                var language = _language.Add(new Language
                {
                    Name = model.Name,
                    Side = model.Side
                });
                SaveChanges();
                return new LanguageM
                {
                    Id = language.Id,
                    Name = language.Name,
                    Side = language.Side
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add a language!",
                    e, DateTime.Now, "Server", "Service_Language_Add");
            }
        }

        public void Delete(Guid language_id)
        {
            try
            {
                if (_projectFramework.Any(pf => pf.Framework.LanguageId.Equals(language_id))) throw BadRequest("A project is refering to a framework of this language!");
                Language language = _language.Where(l => l.Id.Equals(language_id)).Include(l => l.Frameworks).FirstOrDefault();
                _framework.DeleteAll(language.Frameworks);
                _language.Remove(language);
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while delete a language!",
                    e, DateTime.Now, "Server", "Service_Language_Delete");
            }
        }

        public IList<LanguageM> GetAll(string side)
        {
            try
            {
                if (!("Server".Equals(side) || "Client".Equals(side) || "Database".Equals(side)))
                    throw BadRequest("Side value of parameter side must be 'Server', 'Client' or 'Database'!");

                return _language.Where(l => l.Side.Equals(side))
                    .Select(l => new LanguageM
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Side = l.Side
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all languages!",
                    e, DateTime.Now, "Server", "Service_Language_GetAll");
            }
        }

        public LanguageDM GetDetail(Guid language_id)
        {
            try
            {
                var result = _language.Where(l => l.Id.Equals(language_id))
                    .Select(l => new LanguageDM
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Frameworks = l.Frameworks.Select(f => new FrameworkM
                        {
                            Id = f.Id,
                            Name = f.Name
                        }).ToList(),
                        Side = l.Side
                    }).FirstOrDefault();
                if (result == null) throw NotFound(language_id, "language id");

                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get a language detail!",
                    e, DateTime.Now, "Server", "Service_Language_GetAll");
            }
        }

        public LanguageM Update(Guid language_id, LanguageUpdateM model)
        {
            try
            {
                Language language = _language.GetOne(language_id);
                language.Name = model.Name;
                language.Side = language.Side;
                SaveChanges();
                return new LanguageM
                {
                    Id = language_id,
                    Name = language.Name,
                    Side = language.Side
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while update a language!",
                    e, DateTime.Now, "Server", "Service_Language_Update");
            }
        }

        private int SaveChanges()
        {
            return _language.SaveChanges();
        }
    }
}
