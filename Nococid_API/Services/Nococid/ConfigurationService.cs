using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Models.Nococid.Configuration;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IConfigurationService
    {
        dynamic AddExecutor(Guid tool_id, ExecutorCreateM model);
        dynamic AddExecutorImage(Guid tool_id, Guid executor_id, ExecutorImageCreateM model);
        dynamic AddResourceClass(Guid tool_id, Guid executor_id, ResourceClassCreateM model);
        dynamic AddStep(Guid tool_id, Guid executor_id,StepCreateM model);
        void DeleteExecutor(Guid tool_id, Guid executor_id);
        void DeleteExecutorImage(Guid tool_id, Guid executor_id, Guid executor_image_id);
        void DeleteStep(Guid tool_id, Guid executor_id, Guid step_id);
        void DeleteResourceClass(Guid tool_id, Guid executor_id, Guid resouce_classes_id);
        Executor EnsureExecutorExisted(Guid tool_id, Guid executor_id);
        ExecutorImage EnsureExecutorImageExisted(Guid tool_id, Guid executor_id, Guid executor_image_id);
        ResourceClass EnsureResourceClassExisted(Guid tool_id, Guid executor_id, Guid resouce_classes_id);
        Step EnsureStepExisted(Guid tool_id, Guid executor_id, Guid step_id);
        void EnsureToolExisted(Guid tool_id);
        dynamic GetExecutors(Guid tool_id);
        dynamic GetExecutorImages(Guid tool_id, Guid executor_id);
        dynamic GetResourceClasses(Guid tool_id, Guid executor_id);
        dynamic GetSteps(Guid tool_id, Guid executor_id);
        dynamic UpdateExecutor(Guid tool_id, Guid executor_id, ExecutorUpdateM model);
        dynamic UpdateExecutorImage(Guid tool_id, Guid executor_id, Guid executor_image_id, ExecutorImageUpdateM model);
        dynamic UpdateResourceClass(Guid tool_id, Guid executor_id, Guid resouce_classes_id, ResourceClassUpdateM model);
        dynamic UpdateStep(Guid tool_id, Guid executor_id, Guid step_id, StepUpdateM model);
    }

    public class ConfigurationService : ServiceBase, IConfigurationService
    {
        private readonly IContext<Tool> _tool;
        private readonly IContext<Executor> _executor;
        private readonly IContext<ExecutorImage> _executorImage;
        private readonly IContext<ResourceClass> _resourceClass;
        private readonly IContext<Step> _step;

        public ConfigurationService(IContext<Step> step, IContext<ResourceClass> resourceClass, IContext<ExecutorImage> executorImage, IContext<Tool> tool, IContext<Executor> executor, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _step = step;
            _resourceClass = resourceClass;
            _executorImage = executorImage;
            _tool = tool;
            _executor = executor;
        }

        public dynamic AddExecutor(Guid tool_id, ExecutorCreateM model)
        {
            try
            {
                EnsureToolExisted(tool_id);

                Executor executor = _executor.Add(new Executor
                {
                    Language = model.Language,
                    ToolId = tool_id,
                    Name = model.Name
                });
                SaveChanges();

                return new
                {
                    executor.Id,
                    executor.Name,
                    executor.Language
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-AddExecutor");
            }
        }

        public dynamic AddExecutorImage(Guid tool_id, Guid executor_id, ExecutorImageCreateM model)
        {
            try
            {
                EnsureExecutorExisted(tool_id, executor_id);

                ExecutorImage executor_image = _executorImage.Add(new ExecutorImage
                {
                    ExecutorId = executor_id,
                    Name = model.Name,
                    Value = model.Value
                });
                SaveChanges();

                return new
                {
                    executor_image.Id,
                    executor_image.Name,
                    executor_image.Value
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-AddExecutorImage");
            }
        }

        public dynamic AddResourceClass(Guid tool_id, Guid executor_id, ResourceClassCreateM model)
        {
            try
            {
                EnsureExecutorExisted(tool_id, executor_id);

                ResourceClass resource_class = _resourceClass.Add(new ResourceClass
                {
                    ExecutorId = executor_id,
                    Name = model.Name
                });
                SaveChanges();

                return new
                {
                    resource_class.Id,
                    resource_class.Name
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-AddResourceClass");
            }
        }

        public dynamic AddStep(Guid tool_id, Guid executor_id, StepCreateM model)
        {
            try
            {
                EnsureExecutorExisted(tool_id, executor_id);

                Step step = _step.Add(new Step
                {
                    ExecutorId = executor_id,
                    ReplacedText = model.ReplacedText,
                    Stage = model.Stage,
                    Name = model.Name,
                    Text = model.Text
                });
                SaveChanges();

                return new
                {
                    step.Id,
                    step.Name,
                    step.ReplacedText,
                    step.Stage,
                    step.Text
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-AddStep");
            }
        }

        public void DeleteExecutor(Guid tool_id, Guid executor_id)
        {
            try
            {
                Executor executor = EnsureExecutorExisted(tool_id, executor_id);
                _executor.Remove(executor);
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-DeleteExecutor");
            }
        }

        public void DeleteExecutorImage(Guid tool_id, Guid executor_id, Guid executor_image_id)
        {
            try
            {
                ExecutorImage executor_image = EnsureExecutorImageExisted(tool_id, executor_id, executor_image_id);
                _executorImage.Remove(executor_image);
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-DeleteExecutorImage");
            }
        }

        public void DeleteResourceClass(Guid tool_id, Guid executor_id, Guid resouce_classes_id)
        {
            try
            {
                ResourceClass resource_class = EnsureResourceClassExisted(tool_id, executor_id, resouce_classes_id);
                _resourceClass.Remove(resource_class);
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-DeleteResourceClass");
            }
        }

        public void DeleteStep(Guid tool_id, Guid executor_id, Guid step_id)
        {
            try
            {
                Step step = EnsureStepExisted(tool_id, executor_id, step_id);
                _step.Remove(step);
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-DeleteStep");
            }
        }

        public Executor EnsureExecutorExisted(Guid tool_id, Guid executor_id)
        {
            try
            {
                Executor executor = _executor.GetOne(e => e.ToolId.Equals(tool_id) && e.Id.Equals(executor_id));
                if (executor == null) throw NotFound();
                return executor;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-EnsureExecutorExisted");
            }
        }

        public ExecutorImage EnsureExecutorImageExisted(Guid tool_id, Guid executor_id, Guid executor_image_id)
        {
            try
            {
                ExecutorImage executor_image = _executorImage.GetOne(ei => ei.Executor.ToolId.Equals(tool_id) && ei.ExecutorId.Equals(executor_id) && ei.Id.Equals(executor_image_id));
                if (executor_image == null) throw NotFound();
                return executor_image;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-EnsureExecutorImageExisted");
            }
        }

        public ResourceClass EnsureResourceClassExisted(Guid tool_id, Guid executor_id, Guid resouce_classes_id)
        {
            try
            {
                ResourceClass resource_class = _resourceClass.GetOne(rc => rc.Executor.ToolId.Equals(tool_id) && rc.ExecutorId.Equals(executor_id) && rc.Id.Equals(resouce_classes_id));
                if (resource_class == null) throw NotFound();
                return resource_class;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-EnsureResourceClassExisted");
            }
        }

        public Step EnsureStepExisted(Guid tool_id, Guid executor_id, Guid step_id)
        {
            try
            {
                Step step = _step.GetOne(s => s.Executor.ToolId.Equals(tool_id) && s.ExecutorId.Equals(executor_id) && s.Id.Equals(step_id));
                if (step == null) throw NotFound();
                return step;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-EnsureStepExisted");
            }
        }

        public void EnsureToolExisted(Guid tool_id)
        {
            try
            {
                if (!_tool.Any(t => t.Id.Equals(tool_id))) throw NotFound();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-EnsureToolExisted");
            }
        }

        public dynamic GetExecutorImages(Guid tool_id, Guid executor_id)
        {
            try
            {
                EnsureExecutorExisted(tool_id, executor_id);

                return _executorImage.Where(ei => ei.ExecutorId.Equals(executor_id)).Select(ei => new
                {
                    ei.Id,
                    ei.Name,
                    ei.Value
                }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-GetExecutorImages");
            }
        }

        public dynamic GetExecutors(Guid tool_id)
        {
            try
            {
                EnsureToolExisted(tool_id);

                return _executor.Where(e => e.ToolId.Equals(tool_id)).Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Language
                }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-GetExecutors");
            }
        }

        public dynamic GetResourceClasses(Guid tool_id, Guid executor_id)
        {
            try
            {
                EnsureExecutorExisted(tool_id, executor_id);

                return _resourceClass.Where(rc => rc.ExecutorId.Equals(executor_id)).Select(rc => new
                {
                    rc.Id,
                    rc.Name
                }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-GetResourceClasses");
            }
        }

        public dynamic GetSteps(Guid tool_id, Guid executor_id)
        {
            try
            {
                EnsureExecutorExisted(tool_id, executor_id);
                return _step.Where(s => s.ExecutorId.Equals(executor_id)).Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Stage,
                    s.Text,
                    s.ReplacedText
                }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-GetSteps");
            }
        }

        public dynamic UpdateExecutor(Guid tool_id, Guid executor_id, ExecutorUpdateM model)
        {
            try
            {
                Executor executor = EnsureExecutorExisted(tool_id, executor_id);
                executor.Name = model.Name;
                executor.Language = model.Language;
                SaveChanges();
                return new
                {
                    executor.Id,
                    executor.Name,
                    executor.Language
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-UpdateExecutor");
            }
        }

        public dynamic UpdateExecutorImage(Guid tool_id, Guid executor_id, Guid executor_image_id, ExecutorImageUpdateM model)
        {
            try
            {
                ExecutorImage executor_image = EnsureExecutorImageExisted(tool_id, executor_id, executor_image_id);
                executor_image.Name = model.Name;
                executor_image.Value = model.Value;
                SaveChanges();
                return new
                {
                    executor_image.Id,
                    executor_image.Name,
                    executor_image.Value
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-UpdateExecutorImage");
            }
        }

        public dynamic UpdateResourceClass(Guid tool_id, Guid executor_id, Guid resouce_classes_id, ResourceClassUpdateM model)
        {
            try
            {
                ResourceClass resource_class = EnsureResourceClassExisted(tool_id, executor_id, resouce_classes_id);
                resource_class.Name = model.Name;
                SaveChanges();
                return new
                {
                    resource_class.Id,
                    resource_class.Name
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-UpdateResourceClass");
            }
        }

        public dynamic UpdateStep(Guid tool_id, Guid executor_id, Guid step_id, StepUpdateM model)
        {
            try
            {
                Step step = EnsureStepExisted(tool_id, executor_id, step_id);
                step.Stage = model.Stage;
                step.Name = model.Name;
                step.Text = model.Text;
                step.ReplacedText = model.ReplacedText;
                SaveChanges();
                return new
                {
                    step.Id,
                    step.Name,
                    step.Stage,
                    step.Text,
                    step.ReplacedText
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Configuration-UpdateStep");
            }
        }

        private int SaveChanges()
        {
            return _tool.SaveChanges();
        }
    }
}
