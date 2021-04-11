using Newtonsoft.Json;
using Nococid_API.Data.Static;
using Nococid_API.Enums;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nococid_API.Services.Github
{
    public interface IGHContentService
    {
        void CreateFile(string gh_username, string repository_name, string access_token, string branch, string path, string content, bool is_encoded, string message);
        void CreateConfigurationFiles(string gh_username, string repository_name, string access_token, IList<BranchM> branches, string path, IList<WorkflowCreateConfigM> configs, Guid sprint_id);
        void RunWorkflow(string gh_username, string repository_name, string access_token, string path);
        void DeleteFile(string gh_username, string repository_name, string access_token, string branch, string path, string message, string sha);
        GHFileDetail GetFile(string gh_username, string repository_name, string access_token, string branch, string path);
        IList<GHFile> GetFiles(string gh_username, string repository_name, string access_token, string branch, string path);
        void UpdateFile(string gh_username, string repository_name, string access_token, string branch, string path, string content, bool is_encoded, string message, string sha);
    }

    public class GHContentService : GHServiceBase, IGHContentService
    {
        public GHContentService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public void CreateFile(string gh_username, string repository_name, string access_token, string branch, string path, string content, bool is_encoded, string message)
        {
            try
            {
                GHCreateFile data = new GHCreateFile
                {
                    Branch = branch,
                    Content = is_encoded ? content : Convert.ToBase64String(Encoding.UTF8.GetBytes(content)),
                    Message = message
                };
                _httpRequest.Send(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name + "/contents/" + path,
                    HttpRequestMethod.Put, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    }, data
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while create file!",
                    e, DateTime.Now, "Server", "Service_GHContent_CreateFile");
            }
        }

        public void RunWorkflow(string gh_username, string repository_name, string access_token, string path)
        {
            try
            {
                IList<GHFile> gh_files = GetFiles(gh_username, repository_name, access_token, ConfigTool.Nococid, path);
                foreach (var gh_file in gh_files)
                {
                    if (gh_file.Type.Equals("file"))
                    {
                        switch (gh_file.Name)
                        {
                            case "circleci_config.yml":
                                GHFileDetail gh_file_content = GetFile(gh_username, repository_name, access_token, ConfigTool.Nococid, path + "/" + gh_file.Name);
                                IList<GHFile> circleci_files = GetFiles(gh_username, repository_name, access_token, ConfigTool.Nococid, ConfigTool.CircleCIPath);

                                GHFile circleci_file = circleci_files.Where(c => c.Name.Equals(ConfigTool.CircleCIFileName)).FirstOrDefault();
                                if (circleci_file != null)
                                {
                                    UpdateFile(gh_username, repository_name, access_token,
                                        ConfigTool.Nococid, ConfigTool.CircleCIPath + "/" + ConfigTool.CircleCIFileName,
                                        gh_file_content.Content, true, "Update CircleCI Config File", circleci_file.Sha);
                                }
                                else
                                {
                                    CreateFile(gh_username, repository_name, access_token,
                                        ConfigTool.Nococid, ConfigTool.CircleCIPath + "/" + ConfigTool.CircleCIFileName,
                                        gh_file_content.Content, true, "Create CircleCI Config File");
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while run workflow!",
                    e, DateTime.Now, "Server", "Service_GHContent_CreateWorkflowFiles");
            }
        }

        public void DeleteFile(string gh_username, string repository_name, string access_token, string branch, string path, string message, string sha)
        {
            try
            {
                _httpRequest.Send(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name + "/contents/" + path,
                    HttpRequestMethod.Delete, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    },
                    new GHDeleteFile
                    {
                        Branch = branch,
                        Message = message,
                        Sha = sha
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get file!",
                    e, DateTime.Now, "Server", "Service_GHContent_GetFile");
            }
        }

        public GHFileDetail GetFile(string gh_username, string repository_name, string access_token, string branch, string path)
        {
            try
            {
                return _httpRequest.Send<GHFileDetail>(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name + "/contents/" + path + "?ref=" + branch,
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get file!",
                    e, DateTime.Now, "Server", "Service_GHContent_GetFile");
            }
        }

        public IList<GHFile> GetFiles(string gh_username, string repository_name, string access_token, string branch, string path)
        {
            try
            {
                return _httpRequest.Send<IList<GHFile>>(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name +"/contents/" + path + "?ref=" + branch,
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    }
                );
            }
            catch (Exception e)
            {
                if (e.Message.Contains("NotFound"))
                {
                    return new List<GHFile>();
                }

                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get files!",
                    e, DateTime.Now, "Server", "Service_GHContent_GetFiles");
            }
        }

        public void UpdateFile(string gh_username, string repository_name, string access_token, string branch, string path, string content, bool is_encoded, string message, string sha)
        {
            try
            {
                GHUpdateFile data = new GHUpdateFile
                {
                    Branch = branch,
                    Content = is_encoded ? content : Convert.ToBase64String(Encoding.UTF8.GetBytes(content)),
                    Message = message,
                    Sha = sha
                };
                _httpRequest.Send(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name + "/contents/" + path,
                    HttpRequestMethod.Put, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    }, data
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while update a file!",
                    e, DateTime.Now, "Server", "Service_GHContent_UpdateFile");
            }
        }

        public void CreateConfigurationFiles(string gh_username, string repository_name, string access_token, IList<BranchM> branches, string path, IList<WorkflowCreateConfigM> configs, Guid sprint_id)
        {
            try
            {
                foreach (var config in configs)
                {
                    if (config.ConfigTool.Equals(ConfigTool.CircleCI))
                    {
                        config.Content = config.Content.Replace("{nococid-sprint-id}", sprint_id.ToString());
                        foreach (var branch in branches)
                        {
                            IList<GHFile> circleci_files = GetFiles(gh_username, repository_name, access_token, branch.Name, ConfigTool.CircleCIPath);
                            if (circleci_files.Count != 0)
                            {
                                GHFile circleci_file = circleci_files.Where(c => c.Name.Equals(ConfigTool.CircleCIFileName)).FirstOrDefault();
                                if (circleci_file != null)
                                {
                                    UpdateFile(gh_username, repository_name, access_token,
                                        branch.Name, ".circleci/config.yml",
                                        config.Content, false, "Update circleci configuration file", circleci_file.Sha);
                                }
                                else
                                {
                                    CreateFile(gh_username, repository_name, access_token,
                                        branch.Name, ".circleci/config.yml",
                                        config.Content, false, "Add circleci workflow content");
                                }
                            }
                            else
                            {
                                CreateFile(gh_username, repository_name, access_token,
                                    branch.Name, ".circleci/config.yml",
                                    config.Content, false, "Add circleci workflow content");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while create workflow files!",
                    e, DateTime.Now, "Server", "Service_GHContent_CreateWorkflowFiles");
            }
        }
    }
}
