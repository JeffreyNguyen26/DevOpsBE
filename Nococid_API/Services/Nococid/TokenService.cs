using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface ITokenService
    {
        object Get(Guid user_id, Guid project_id);
        object Add(string third_party_account_id, string email, string name, string avatar_url, string access_token, Guid tool_id, Guid user_id);
        string GetToken(Guid tool_id, Guid sprint_id);
        string GetToken(Guid tool_id, string account_username, Guid vsc_id);
    }

    public class TokenService : ServiceBase, ITokenService
    {
        private readonly IContext<Tool> _tool;
        private readonly IContext<Account> _account;
        private readonly IContext<ProjectTool> _projectTool;

        public TokenService(IContext<ProjectTool> projectTool, IContext<Account> account, IContext<Tool> tool, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _projectTool = projectTool;
            _account = account;
            _tool = tool;
        }

        public object Add(string third_party_account_id, string email, string name, string avatar_url, string access_token, Guid tool_id, Guid user_id)
        {
            try
            {
                Account account = _account.GetOne(a => a.ThirdPartyAccountId.Equals(third_party_account_id) && a.ToolId.Equals(tool_id));
                if (account == null)
                {
                    account = _account.Add(new Account
                    {
                        ToolId = tool_id,
                        AccessToken = access_token,
                        AvatarUrl = avatar_url,
                        Email = email,
                        IsMain = false,
                        Name = name,
                        ThirdPartyAccountId = third_party_account_id,
                        UserId = user_id
                    });
                } else
                {
                    if (account.UserId != null)
                    {
                        if (!account.UserId.Value.Equals(user_id))
                        {
                            throw BadRequest("This third party account has been connect to a Nococid account!\nKindly cancel the connection first!");
                        }
                    }
                    account.AccessToken = access_token;
                    account.AvatarUrl = avatar_url;
                    account.Email = email;
                    account.Name = name;
                    account.UserId = user_id;
                }
                SaveChanges();
                return new
                {
                    account.Email,
                    account.Name,
                    account.AvatarUrl
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add token for a tool!",
                       e, DateTime.Now, "Server", "Service_Token_Add");
            }
        }

        public object Get(Guid user_id, Guid project_id)
        {
            try
            {
                var accounts = _account.Where(a => a.UserId.Equals(user_id))
                    .Select(a => new
                    {
                        a.Id, a.Email, a.Name, a.AvatarUrl, a.ToolId
                    }).ToList();

                var project_tools = _projectTool.Where(pt => pt.ProjectId.Equals(project_id) && pt.Stages != null)
                    .Select(pt => new
                    {
                        pt.AccountId,
                        Tool = new
                        {
                            pt.Tool.Id,
                            pt.Tool.Name
                        }
                    }).ToList();

                IList<object> tools_result = new List<object>();
                IList<object> accounts_result;
                bool flag = false;
                foreach (var project_tool in project_tools)
                {
                    accounts_result = new List<object>();
                    foreach (var account in accounts)
                    {
                        flag = false;
                        if (account.ToolId.Equals(project_tool.Tool.Id))
                        {
                            if (project_tool.AccountId != null)
                            {
                                if (project_tool.AccountId.Value.Equals(account.Id))
                                {
                                    flag = true;
                                    accounts_result.Add(new
                                    {
                                        account.Id,
                                        account.Email,
                                        account.Name,
                                        account.AvatarUrl,
                                        IsSet = true
                                    });
                                }
                            }

                            if (!flag)
                            {
                                accounts_result.Add(new
                                {
                                    account.Id,
                                    account.Email,
                                    account.Name,
                                    account.AvatarUrl,
                                    IsSet = false
                                });
                            }
                        }
                    }
                    tools_result.Add(new
                    {
                        project_tool.Tool.Id,
                        project_tool.Tool.Name,
                        Accounts = accounts_result
                    });
                }
                return tools_result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Tool-Get");
            }
        }

        public string GetToken(Guid tool_id, Guid sprint_id)
        {
            try
            {
                return null;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get access token!",
                       e, DateTime.Now, "Server", "Service_Token_GetToken");
            }
        }

        public string GetToken(Guid tool_id, string account_username, Guid vsc_id)
        {
            try
            {
                return "";
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get access token!",
                       e, DateTime.Now, "Server", "Service_Token_GetToken");
            }
        }

        private int SaveChanges()
        {
            return _projectTool.SaveChanges();
        }
    }
}
