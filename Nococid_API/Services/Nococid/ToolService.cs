using Newtonsoft.Json;
using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Models.Nococid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nococid_API.Data.Static;

namespace Nococid_API.Services.Nococid
{
    public interface IToolService
    {
        ToolM Get(Guid id);
        IList<StageToolsM> GetAll();
        dynamic GetConfig(Guid tool_id);
        string[] GetStages();
    }

    public class ToolService : ServiceBase, IToolService
    {
        private readonly IContext<Tool> _tool;
        private readonly IContext<Account> _account;
        private readonly IContext<ProjectTool> _projectTool;
        private readonly IContext<Executor> _executor;

        public ToolService(IContext<Executor> executor, IContext<ProjectTool> projectTool, IContext<Account> account, IContext<Tool> tool, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _executor = executor;
            _projectTool = projectTool;
            _account = account;
            _tool = tool;
        }

        public ToolM Get(Guid id)
        {
            try
            {
                var result = _tool.Where(t => t.Id.Equals(id))
                    .Select(t => new ToolM
                    {
                        Id = t.Id,
                        ToolType = t.ToolType.Split(new char[] { ',' }),
                        Name = t.Name
                    }).FirstOrDefault();
                if (result == null) throw NotFound(id, "tool id");
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get a tool!",
                       e, DateTime.Now, "Server", "Service_Tool_Get");
            }
        }

        public IList<StageToolsM> GetAll()
        {
            try
            {
                IList<StageToolsM> result = new List<StageToolsM>();
                IList<ToolM> tools_result;
                IList<Tool> tools = _tool.GetAll();

                foreach (var stage in Data.Static.Stage.All)
                {
                    tools_result = new List<ToolM>();
                    result.Add(new StageToolsM
                    {
                        Name = stage,
                        Tools = tools_result
                    });
                    foreach (var tool in tools)
                    {
                        if (tool.ToolType.Contains(stage))
                        {
                            tools_result.Add(new ToolM
                            {
                                Id = tool.Id,
                                Name = tool.Name,
                                ToolType = tool.ToolType.Split(',')
                            });
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all tools!",
                    e, DateTime.Now, "Server", "Service_Tool_GetAll");
            }
        }

        public dynamic GetConfig(Guid tool_id)
        {
            try
            {
                var config = _tool.Where(t => t.Id.Equals(tool_id)).Select(t => new
                {
                    t.Name, t.ToolType,
                    Executors = t.Executors.Select(e => new
                    {
                        e.Id, e.Name, e.Language,
                        Images = e.ExecutorImages.Select(ei => new
                        {
                            ei.Id, ei.Name, ei.Value
                        }).ToList(),
                        ResourceClasses = e.ResourceClasses.Select(rc => new
                        {
                            rc.Id, rc.Name
                        }).ToList(),
                        Steps = e.Steps.Select(s => new
                        {
                            s.Id, s.Stage, s.Name, s.Text, s.ReplacedText
                        }).ToList()
                    }).ToList()
                }).FirstOrDefault();

                return new
                {
                    config.Name,
                    ToolType = config.ToolType.Split(","),
                    Executors = config.Executors.Select(e => new
                    {
                        e.Id, e.Name, e.Language,
                        e.Images, e.ResourceClasses,
                        Stages = e.Steps.GroupBy(s => s.Stage).Select(s => new
                        {
                            Name = s.Key,
                            Steps = s.Select(step => new
                            {
                                step.Id,
                                step.Name,
                                step.Text,
                                step.ReplacedText
                            }).ToList()
                        }).ToList()
                    })
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Tool-GetConfig");
            }
        }

        public string[] GetStages()
        {
            try
            {
                return Data.Static.Stage.All;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all stages!",
                    e, DateTime.Now, "Server", "Service_Tool_GetStages");
            }
        }
    }
}
