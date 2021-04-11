using Nococid_API.Authentication;
using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Models.Https;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IUserService
    {
        UserAuthorizationM Login(UserLoginM model);
        UserAuthorizationM Register(UserCreateM model, Guid? admin_user_id);
        IList<UserM> Search(string username);
        IList<UserM> GetMembers(Guid admin_user_id, Guid user_id);
    }

    public class UserService : ServiceBase, IUserService
    {
        private readonly IContext<User> _user;
        private readonly IContext<Account> _account;

        public UserService(IContext<Account> account, IContext<User> user, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _account = account;
            _user = user;
        }

        public IList<UserM> GetMembers(Guid admin_user_id, Guid user_id)
        {
            try
            {
                if (!admin_user_id.Equals(Guid.Empty)) user_id = admin_user_id;
                return _user.Where(u => u.AdminUserId.Equals(user_id))
                    .Select(u => new UserM
                    {
                        Id = u.Id,
                        Username = u.Username
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get members!",
                   e, DateTime.Now, "Server", "Service_User_GetMembers");
            }
        }

        public UserAuthorizationM Login(UserLoginM model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password)) throw BadRequest("Username and Password must not empty!");
                if (model.Username.Length < 3 || model.Password.Length < 3) throw BadRequest("Username and Password must have more than 3 characters!");

                User user = _user.Where(u => u.Username.Equals(model.Username))
                    .Select(u => new User { 
                        Id = u.Id,
                        Username = u.Username,
                        Password = u.Password,
                        AdminUserId = u.AdminUserId
                    }).FirstOrDefault();
                if (user == null) throw BadRequest("Username or password is incorrect!");
                bool result = NococidAuthentication.VerifyHashedPassword(user.Username, user.Password, model.Password, out string rehashed_password);
                if (!result) throw BadRequest("Username or password is incorrect!");

                if (rehashed_password != null)
                {
                    user.Password = rehashed_password;
                }
                SaveChanges();

                return new UserAuthorizationM
                {
                    User = new UserM
                    {
                        Id = user.Id,
                        Username = user.Username
                    },
                    HasVscAccount = _account.Any(a => a.UserId.Equals(user.Id)),
                    AdminUser = user.AdminUserId == null ? null : _user.Where(u => u.Id.Equals(user.AdminUserId.Value)).Select(u => new UserM
                    {
                        Id = u.Id,
                        Username = u.Username
                    }).FirstOrDefault()
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while log in!",
                    e, DateTime.Now, "Server", "Service_User_Login");
            }
        }

        public UserAuthorizationM Register(UserCreateM model, Guid? admin_user_id)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Username) | string.IsNullOrEmpty(model.Password)) throw BadRequest("The username or password not must emplty!");
                if (_user.Any(u => u.Username.Equals(model.Username))) throw BadRequest("The username has been used!");

                User user = _user.Add(new User
                {
                    Username = model.Username,
                    Password = NococidAuthentication.GetHashedPassword(model.Username, model.Password),
                    AdminUserId = admin_user_id
                });
                SaveChanges();
                UserAuthorizationM result = new UserAuthorizationM
                {
                    HasVscAccount = false,
                    User = new UserM
                    {
                        Id = user.Id,
                        Username = user.Username
                    }
                };
                if (admin_user_id != null)
                {
                    result.AdminUser = _user.Where(u => u.Id.Equals(admin_user_id))
                        .Select(u => new UserM
                        {
                            Id = u.Id,
                            Username = u.Username
                        }).FirstOrDefault();
                }

                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while register!",
                    e, DateTime.Now, "Server", "Service_User_Register");
            }
        }

        public IList<UserM> Search(string value)
        {
            try
            {
                if (value.Length < 3) throw BadRequest("Username needs more than 3 characters for searching!");

                return _user.Where(u => u.Username.Contains(value))
                    .Select(u => new UserM
                    {
                        Id = u.Id,
                        Username = u.Username
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while search user!",
                    e, DateTime.Now, "Server", "Service_User_Search");
            }
        }

        private void SaveChanges()
        {
            _user.SaveChanges();
        }
    }
}
