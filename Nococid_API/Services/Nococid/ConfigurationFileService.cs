using Newtonsoft.Json;
using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IConfigurationFileService
    {
        IList<object> GetConfiguration(Guid project_id);
        string GetHerokuFile(Guid project_id);
        void SaveConfiguration(Guid project_id, string config_content);
        void SaveHerokuAuth(TokenCreateM model);
    }

    public class ConfigurationFileService : ServiceBase, IConfigurationFileService
    {
        public ConfigurationFileService(IErrorHandlerService errorHandler) : base(errorHandler) { }

        public IList<object> GetConfiguration(Guid project_id)
        {
            try
            {
                IList<object> result = new List<object>();
                string config_content = File.ReadAllTextAsync("Configuration/" + project_id.ToString() + ".txt").GetAwaiter().GetResult();
                IList<ConfigDM> configs = JsonConvert.DeserializeObject<IList<ConfigDM>>(config_content);
                foreach (var config in configs)
                {
                    if (!string.IsNullOrEmpty(config.deployName))
                    {
                        result.Add(new ConfigDeployM
                        {
                            deployName = config.deployName,
                            deployCommand = config.deployCommand,
                            deployStorage = config.deployStorage,
                            disable = config.disable,
                            machinePath = config.machinePath,
                            stageCode = config.stageCode,
                            stageDefaultContent = config.stageDefaultContent,
                            stageName = config.stageName
                        });
                    }
                    else if (!string.IsNullOrEmpty(config.buildName))
                    {
                        result.Add(new ConfigBuildM
                        {
                            buildName = config.buildName,
                            buildCommand = config.buildCommand,
                            buildStorage = config.buildStorage,
                            disable = config.disable,
                            machinePath = config.machinePath,
                            stageCode = config.stageCode,
                            stageDefaultContent = config.stageDefaultContent,
                            stageName = config.stageName
                        });
                    }
                    else if (!string.IsNullOrEmpty(config.testName))
                    {
                        result.Add(new ConfigTestM
                        {
                            testName = config.testName,
                            testCommand = config.testCommand,
                            testStorage = config.testStorage,
                            disable = config.disable,
                            machinePath = config.machinePath,
                            stageCode = config.stageCode,
                            stageDefaultContent = config.stageDefaultContent,
                            stageName = config.stageName
                        });
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-ConfigurationFile-GetConfiguration");
            }
        }

        public string GetHerokuFile(Guid project_id)
        {
            try
            {
                return File.ReadAllTextAsync("Heroku/" + project_id.ToString() + ".txt").GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                       e, DateTime.Now, "Server", "Service-ConfigurationFile-GetHerokuFile");
            }
        }

        public void SaveConfiguration(Guid project_id, string config_content)
        {
            try
            {
                Directory.CreateDirectory("Configuration");
                File.WriteAllTextAsync("Configuration/" + project_id.ToString() + ".txt", config_content).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-ConfigurationFile-SaveConfiguration");
            }
        }

        public void SaveHerokuAuth(TokenCreateM model)
        {
            try
            {
                //if (model.Pairs == null) throw BadRequest("Must have 'pair'");
                //if (model.Pairs.Count == 0) throw BadRequest("Must have 'pair'");
                //if (model.Pairs.Count != 2) throw BadRequest("Pairs for Heroku must have 2 pairs");
                //if (!model.Pairs[0].Key.Equals("Login") || !model.Pairs[1].Key.Equals("Password")) throw BadRequest("Unidentified key");
                //if (string.IsNullOrEmpty(model.Pairs[0].Value) || string.IsNullOrEmpty(model.Pairs[1].Value)) throw BadRequest("Value must not null!");

                //Directory.CreateDirectory("Heroku");
                //string content = "machine api.heroku.com\n" +
                //    "  login " + model.Pairs[0].Value + "\n" +
                //    "  password " + model.Pairs[1].Value + "\n" +
                //    "machine git.heroku.com\n" +
                //    "  login " + model.Pairs[0].Value + "\n" +
                //    "  password " + model.Pairs[1].Value;
                //File.WriteAllTextAsync("Heroku/" + model.ProjectId.ToString() + ".txt", content).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                       e, DateTime.Now, "Server", "Service-ConfigurationFile-SaveHerokuAuth");
            }
        }
    }
}
