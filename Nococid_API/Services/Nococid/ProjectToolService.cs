using Newtonsoft.Json;
using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Enums;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IProjectToolService
    {
        object Add(Guid project_id, Guid tool_id, string stage);
        object GetProjectTools(Guid project_id);
        object Delete(Guid project_id, Guid tool_id, string stage);
        void SetAuth(Guid project_id, Guid tool_id, bool has_auth);
        void SetAccount(TokenProjectAccountM model);
    }

    public class ProjectToolService : ServiceBase, IProjectToolService
    {
        private readonly IContext<Tool> _tool;
        private readonly IContext<Project> _project;
        private readonly IContext<ProjectTool> _projectTool;

        public ProjectToolService(IContext<Project> project, IContext<Tool> tool, IContext<ProjectTool> projectTool, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _project = project;
            _tool = tool;
            _projectTool = projectTool;
        }

        public object Add(Guid project_id, Guid tool_id, string stage)
        {
            try
            {
                string[] tool_type = _tool.Where(t => t.Id.Equals(tool_id))
                    .Select(t => t.ToolType).FirstOrDefault().Split(",");
                if (!tool_type.Any(tt => tt.Equals(stage))) throw BadRequest("Unidentified stage!");

                ProjectTool project_tool = _projectTool.GetOne(pt => pt.ProjectId.Equals(project_id) && pt.ToolId.Equals(tool_id));
                if (project_tool == null)
                {
                    project_tool = _projectTool.Add(new ProjectTool
                    {
                        ToolId = tool_id,
                        ProjectId = project_id,
                        Stages = stage
                    });
                }
                else
                {
                    if (string.IsNullOrEmpty(project_tool.Stages))
                    {
                        project_tool.Stages = stage;
                    } else if (!project_tool.Stages.Contains(stage))
                    {
                        project_tool.Stages += "," + stage;
                    }
                }
                SaveChanges();
                return GetProjectTools(project_id);
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-ProjectTool-Add");
            }
        }

        public object Delete(Guid project_id, Guid tool_id, string stage)
        {
            try
            {
                string[] tool_type = _tool.Where(t => t.Id.Equals(tool_id))
                    .Select(t => t.ToolType).FirstOrDefault().Split(",");
                if (!tool_type.Any(tt => tt.Equals(stage))) throw BadRequest("Unidentified stage!");

                ProjectTool project_tool = _projectTool.GetOne(pt => pt.ProjectId.Equals(project_id) && pt.ToolId.Equals(tool_id));
                if (project_tool == null) throw NotFound();

                if (!string.IsNullOrEmpty(project_tool.Stages))
                {
                    project_tool.Stages = project_tool.Stages.Replace("," + stage, "").Replace(stage + ",", "").Replace(stage, "");
                    if (string.IsNullOrEmpty(project_tool.Stages)) project_tool.Stages = null;
                }
                SaveChanges();
                return GetProjectTools(project_id);
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while delete a project's tool!",
                    e, DateTime.Now, "Server", "Service_ProjectTool_DeleteProjectTool");
            }
        }

        public object GetProjectTools(Guid project_id)
        {
            try
            {
                var project = _project.Where(p => p.Id.Equals(project_id))
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        ProjectType = new
                        {
                            p.ProjectType.Id,
                            p.ProjectType.Name
                        },
                        Tools = p.ProjectTools.Where(pt => pt.Stages != null).Select(pt => new
                        {
                            pt.Stages,
                            Tool = new
                            {
                                pt.Tool.Id,
                                pt.Tool.Name
                            }
                        }).ToList()
                    }).FirstOrDefault();

                List<object> list = new List<object>();
                foreach (var stage in Data.Static.Stage.All)
                {
                    foreach (var tool in project.Tools)
                    {
                        if (tool.Stages.Contains(stage))
                        {
                            list.Add(new
                            {
                                ToolName = tool.Tool.Name,
                                Stage = stage
                            });
                        }
                    }
                }
                
                return new
                {
                    project.Id,
                    project.Name,
                    project.ProjectType,
                    Tools = list
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get project's tools!",
                    e, DateTime.Now, "Server", "Service_ProjectTool_GetProjectTools");
            }
        }

        public void SetAccount(TokenProjectAccountM model)
        {
            try
            {
                ProjectTool project_tool = _projectTool.GetOne(pt => pt.ProjectId.Equals(model.ProjectId) && pt.ToolId.Equals(model.ToolId));
                if (project_tool == null) throw NotFound();

                project_tool.AccountId = model.AccountId;
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-ProjectTool-SetAccount");
            }
        }

        public void SetAuth(Guid project_id, Guid tool_id, bool has_auth)
        {
            try
            {
                ProjectTool project_tool = _projectTool.GetOne(pt => pt.ProjectId.Equals(project_id) && pt.ToolId.Equals(tool_id));
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                       e, DateTime.Now, "Server", "Service-ProjectTool-SetAuth");
            }
        }

        private int SaveChanges()
        {
            return _projectTool.SaveChanges();
        }
    }
}