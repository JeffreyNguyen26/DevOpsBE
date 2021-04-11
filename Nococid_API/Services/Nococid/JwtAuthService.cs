using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Nococid_API.Data.Static;
using Nococid_API.Enums;
using Nococid_API.Models.Nococid.Auth;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IJwtAuthService
    {
        string GenerateJwt(Guid admin_user_id, Guid user_id, string application_role);
        JwtClaimM GetClaims(HttpRequest request);
        void RemoveAudience(Guid admin_user_id, Guid user_id);
    }

    public class JwtAuthService : ServiceBase, IJwtAuthService
    {
        public JwtAuthService(IErrorHandlerService errorHandler) : base(errorHandler) { }

        public string GenerateJwt(Guid admin_user_id, Guid user_id, string application_role)
        {
            try
            {
                RemoveAudience(admin_user_id, user_id);
                DateTime login_time = DateTime.Now;
                JwtAuth.ValidAudiences.Add(admin_user_id.ToString() + "&" + user_id.ToString() + "&" + login_time.Ticks);
                JwtSecurityTokenHandler token_handler = new JwtSecurityTokenHandler();
                return token_handler.WriteToken(token_handler.CreateToken(new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtClaim.Admin_User_Id, admin_user_id.ToString()),
                        new Claim(JwtClaim.User_Id, user_id.ToString()),
                        new Claim(JwtClaim.Application_Role, application_role),
                        new Claim(JwtClaim.Login_Time, DateTime.Now.Ticks.ToString())
                    }),
                    Audience = admin_user_id.ToString() + "&" + user_id.ToString() + "&" + login_time.Ticks,
                    Expires = DateTime.Now.AddDays(7),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtAuth.JwtKey)),
                        SecurityAlgorithms.HmacSha256Signature)
                }));
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while generate jason web token!",
                    e, DateTime.Now, "Server", "Service_JwtAuth_GenerateJwt");
            }
        }

        public JwtClaimM GetClaims(HttpRequest request)
        {
            try
            {
                if (request.Headers.TryGetValue("Authorization", out StringValues value))
                {
                    string token = value.ToString();
                    if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        token = token.Substring("Bearer ".Length).Trim();
                    }

                    IEnumerable<Claim> claims = (new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken).Claims;
                    return new JwtClaimM
                    {
                        AdminUserId = new Guid(claims.FirstOrDefault(c => c.Type.Equals(JwtClaim.Admin_User_Id)).Value),
                        UserId = new Guid(claims.FirstOrDefault(c => c.Type.Equals(JwtClaim.User_Id)).Value),
                        ApplicationRole = claims.FirstOrDefault(c => c.Type.Equals(JwtClaim.Application_Role)).Value,
                    };
                }
                return null;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get claim values!",
                    e, DateTime.Now, "Server", "Service_JwtAuth_GetClaims");
            }
        }

        public void RemoveAudience(Guid admin_user_id, Guid user_id)
        {
            try
            {
                string head = admin_user_id.ToString() + "&" + user_id.ToString() + "&";
                string audience = JwtAuth.ValidAudiences.FirstOrDefault(a => a.StartsWith(head));
                JwtAuth.ValidAudiences.Remove(audience);
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while remove audience!",
                    e, DateTime.Now, "Server", "Service_JwtAuth_RemoveAudiance");
            }
        }
    }

    public class JwtAuth
    {
        public static readonly string JwtKey = "Capstone_DevOps_Process_Implemantation_Support_System";
        public static IList<string> ValidAudiences { get; }
        public static TokenValidationParameters TokenValidationParameters { get; }

        static JwtAuth()
        {
            ValidAudiences = new List<string>();
            TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudiences = ValidAudiences,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
                ClockSkew = TimeSpan.Zero, // remove delay of token when expire

                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = true
            };
        }
    }
}
