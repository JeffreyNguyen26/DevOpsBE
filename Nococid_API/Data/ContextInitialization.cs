using Newtonsoft.Json;
using Nococid_API.Authentication;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Services.Nococid;
using Nococid_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nococid_API.Data
{
    public class ContextInitialization
    {
        public static NococidContext NoccidContext;
        public void Init(NococidContext context)
        {
            NoccidContext = context;
            try
            {
                int a = 0;
                short b = 1;
                short c = 2;
                byte[] bytes = new byte[] { 3, 4, 5, 6, 7, 8, 9, 10 };

                VSCID.Github = new Guid(a++, b, c, bytes);
                RoleID.Admin = new Guid(a++, b, c, bytes);
                RoleID.Project_Manager = new Guid(a++, b, c, bytes);
                RoleID.Technical_Manager = new Guid(a++, b, c, bytes);
                RoleID.Project_Tester = new Guid(a++, b, c, bytes);
                RoleID.Developer = new Guid(a++, b, c, bytes);
                RoleID.All = new Guid[] { RoleID.Technical_Manager, RoleID.Project_Tester, RoleID.Developer };
                ToolID.CircleCI = new Guid(a++, b, c, bytes);
                ToolID.Github = VSCID.Github;
                ToolID.Heroku = new Guid(a++, b, c, bytes);
                

                bool created = context.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
                context.SaveChangesAsync().GetAwaiter().GetResult();

                if (created)
                {
                    #region Role
                    context.Role.Add(new Role
                    {
                        Id = RoleID.Project_Manager,
                        Name = "Project Manager"
                    });
                    context.Role.Add(new Role
                    {
                        Id = RoleID.Technical_Manager,
                        Name = "Technical Manager"
                    });
                    context.Role.Add(new Role
                    {
                        Id = RoleID.Project_Tester,
                        Name = "Tester"
                    });
                    context.Role.Add(new Role
                    {
                        Id = RoleID.Developer,
                        Name = "Developer"
                    });
                    context.Role.Add(new Role
                    {
                        Id = RoleID.Admin,
                        Name = "Admin"
                    });
                    #endregion

                    #region Tool
                    context.Tool.Add(new Tool
                    {
                        Id = ToolID.CircleCI,
                        Name = "CircleCI",
                        ToolType = "Build,Test,Deploy"
                    });
                    context.Tool.Add(new Tool
                    {
                        Id = ToolID.Github,
                        Name = "Github",
                        ToolType = "Coding"
                    });
                    context.Tool.Add(new Tool
                    {
                        Id = ToolID.Heroku,
                        Name = "Heroku",
                        ToolType = "Deploy"
                    });
                    #endregion

                    #region Project Type
                    context.ProjectType.Add(new ProjectType
                    {
                        Name = "Desktop"
                    });
                    context.ProjectType.Add(new ProjectType
                    {
                        Name = "Web Application"
                    });
                    context.ProjectType.Add(new ProjectType
                    {
                        Name = "Mobile"
                    });
                    #endregion

                    #region User
                    context.User.Add(new User
                    {
                        Username = ApplicationAuth.Nococid_Application_Admin,
                        Password = NococidAuthentication.GetHashedPassword(ApplicationAuth.Nococid_Application_Admin, ApplicationAuth.Nococid_Application_Admin_Password)
                    });
                    context.User.Add(new User
                    {
                        Username = "ThangNLD",
                        Password = NococidAuthentication.GetHashedPassword("ThangNLD", "zaq@123")
                    });
                    context.User.Add(new User
                    {
                        Username = "ToanLD",
                        Password = NococidAuthentication.GetHashedPassword("ToanLD", "zaq@123")
                    });
                    #endregion

                    context.SaveChangesAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
