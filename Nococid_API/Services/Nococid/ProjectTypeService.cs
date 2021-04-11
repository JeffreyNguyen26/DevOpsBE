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
    public interface IProjectTypeService
    {
        IList<ProjectTypeM> GetAll();
        ProjectTypeM GetDetail(Guid id);
    }

    public class ProjectTypeService : ServiceBase, IProjectTypeService
    {
        private readonly IContext<ProjectType> _projectType;

        public ProjectTypeService(IContext<ProjectType> projectType, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _projectType = projectType;
        }

        public IList<ProjectTypeM> GetAll()
        {
            try
            {
                IList<ProjectTypeM> result = new List<ProjectTypeM>();
                IList<ProjectType> project_types = _projectType.GetAll();
                foreach (var project_type in project_types)
                {
                    result.Add(new ProjectTypeM
                    {
                        Id = project_type.Id,
                        Name = project_type.Name
                    });
                }
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all project type!",
                    e, DateTime.Now, "Server", "Service_ProjectType_GetAll");
            }
        }

        public ProjectTypeM GetDetail(Guid id)
        {
            try
            {
                ProjectType project_type = _projectType.GetOne(id);
                return new ProjectTypeM
                {
                    Id = project_type.Id,
                    Name = project_type.Name
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all project type detail!",
                    e, DateTime.Now, "Server", "Service_ProjectType_GetDetail");
            }
        }
    }
}
