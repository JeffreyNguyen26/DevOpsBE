using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IExecutorService
    {
        object GetAll(Guid tool_id);
        object Get(Guid executor_id);
    }

    public class ExecutorService : ServiceBase, IExecutorService
    {
        private readonly IContext<Executor> _executor;

        public ExecutorService(IContext<Executor> executor, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _executor = executor;
        }

        public object GetAll(Guid tool_id)
        {
            try
            {
                return _executor.Where(e => e.ToolId.Equals(tool_id))
                    .Select(e => new
                    {
                        e.Id,
                        e.Name,
                        e.Language,
                        e.Version
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Executor-GetAll");
            }
        }

        public object Get(Guid executor_id)
        {
            try
            {
                var query_result = _executor.Where(e => e.Id.Equals(executor_id))
                    .Select(e => new
                    {
                        Images = e.ExecutorImages.Select(ei => new
                        {
                            ei.Name, ei.Value
                        }).ToList(),
                        ResourceClasses = e.ResourceClasses.Select(r => r.Name).ToList(),
                        Steps = e.Steps.Select(s => new
                        {
                            s.Stage, s.Name, s.Text, s.ReplacedText
                        }).ToList()
                    }).FirstOrDefault();

                var stages = query_result.Steps.GroupBy(s => s.Stage).Select(g => new
                {
                    Name = g.Key,
                    Steps = g.Select(s => new
                    {
                        s.Name, s.Text, s.ReplacedText
                    }).ToList()
                }).ToList();

                return new
                {
                    query_result.Images,
                    query_result.ResourceClasses,
                    Stages = stages
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Executor-Get");
            }
        }
    }
}
