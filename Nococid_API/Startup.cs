using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nococid_API.Data;
using Nococid_API.Services;
using NSwag.Generation.Processors.Security;
using NSwag;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nococid_API.Data.Models;
using Nococid_API.Services.CircleCI;
using Nococid_API.Services.Nococid;
using System.Text;
using Nococid_API.Services.Github;
using Nococid_API.Services.Thread;
using Nococid_API.Services.Heroku;

namespace Nococid_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //services.AddDbContext<NococidContext>(option =>
            //option.UseSqlServer(@"data source=.\sqlserverdev;initial catalog=nococid;
            //        persist security info=true;integrated security=false;trustservercertificate=false;
            //        uid=sa;password=maxsulapro0701;trusted_connection=false;multipleactiveresultsets=true;"));
            services.AddDbContext<NococidContext>(option =>
            option.UseSqlServer(@"data source=.\sqlexpress;initial catalog=nococid;
                    persist security info=true;integrated security=false;trustservercertificate=false;
                    uid=sa;password=nguyenleduythang;trusted_connection=false;multipleactiveresultsets=true;"));
            //services.AddDbContext<NococidContext>(option =>
            //option.UseSqlServer(@"data source=tcp:172.0.0.1,32123\sqlserverdev;initial catalog=nococid;
            //        persist security info=true;integrated security=false;trustservercertificate=false;
            //        uid=sa;password=maxsulapro0701;trusted_connection=false;multipleactiveresultsets=true;"));

            #region Add Context
            services.AddScoped<IContext<Account>, Context<Account>>();
            services.AddScoped<IContext<Approval>, Context<Approval>>();
            services.AddScoped<IContext<Branch>, Context<Branch>>();
            services.AddScoped<IContext<Collaborator>, Context<Collaborator>>();
            services.AddScoped<IContext<Step>, Context<Step>>();
            services.AddScoped<IContext<Commit>, Context<Commit>>();
            services.AddScoped<IContext<Executor>, Context<Executor>>();
            services.AddScoped<IContext<Framework>, Context<Framework>>();
            services.AddScoped<IContext<Language>, Context<Language>>();
            services.AddScoped<IContext<Permission>, Context<Permission>>();
            services.AddScoped<IContext<Pipeline>, Context<Pipeline>>();
            services.AddScoped<IContext<Project>, Context<Project>>();
            services.AddScoped<IContext<ProjectFramework>, Context<ProjectFramework>>();
            services.AddScoped<IContext<ProjectRepository>, Context<ProjectRepository>>();
            services.AddScoped<IContext<ProjectTool>, Context<ProjectTool>>();
            services.AddScoped<IContext<ProjectType>, Context<ProjectType>>();
            services.AddScoped<IContext<TaskSubmission>, Context<TaskSubmission>>();
            services.AddScoped<IContext<Repository>, Context<Repository>>();
            services.AddScoped<IContext<ResourceClass>, Context<ResourceClass>>();
            services.AddScoped<IContext<Role>, Context<Role>>();
            services.AddScoped<IContext<Sprint>, Context<Sprint>>();
            services.AddScoped<IContext<Submission>, Context<Submission>>();
            services.AddScoped<IContext<Data.Models.Task>, Context<Data.Models.Task>>();
            services.AddScoped<IContext<Test>, Context<Test>>();
            services.AddScoped<IContext<TestResult>, Context<TestResult>>();
            services.AddScoped<IContext<Tool>, Context<Tool>>();
            services.AddScoped<IContext<ExecutorImage>, Context<ExecutorImage>>();
            services.AddScoped<IContext<User>, Context<User>>();
            services.AddScoped<IContext<Workflow>, Context<Workflow>>();
            #endregion

            #region Services
            services.AddSingleton<IHttpRequestService, HttpRequestService>();
            services.AddSingleton<IErrorHandlerService, ErrorHandlerService>();

            #region CircleCI
            services.AddSingleton<ICircleCIConfigurationService, CircleCIConfigurationService>();
            services.AddSingleton<ICircleCIJobService, CircleCIJobService>();
            services.AddSingleton<ICircleCIProjectService, CircleCIProjectService>();
            services.AddSingleton<ICircleCIWorkflowService, CircleCIWorkflowService>();
            services.AddSingleton<ICircleCIUserService, CircleCIUserService>();
            services.AddSingleton<ICircleCIPipelineService, CircleCIPipelineService>();
            #endregion

            #region Heroku
            services.AddScoped<IHerokuAccountService, HerokuAccountService>();
            services.AddScoped<IHerokuAppService, HerokuAppService>();
            services.AddScoped<IHerokuRegionService, HerokuRegionService>();
            services.AddScoped<IHerokuStackService, HerokuStackService>();
            #endregion

            #region Github
            services.AddSingleton<IGHAuthService, GHAuthService>();
            services.AddSingleton<IGHBranchService, GHBranchService>();
            services.AddSingleton<IGHCollaboratorService, GHCollaboratorService>();
            services.AddSingleton<IGHContentService, GHContentService>();
            services.AddSingleton<IGHInvitationService, GHInvitationService>();
            services.AddSingleton<IGHReferenceService, GHReferenceService>();
            services.AddSingleton<IGHRepositoryService, GHRepositoryService>();
            services.AddSingleton<IGHUserService, GHUserService>();
            services.AddSingleton<IGHWebhookService, GHWebhookService>();
            #endregion

            #region Nococid
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<ICollaboratorService, CollaboratorService>();
            services.AddScoped<ICommitService, CommitService>();
            services.AddSingleton<IConfigurationFileService, ConfigurationFileService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IExecutorService, ExecutorService>();
            services.AddScoped<IFrameworkService, FrameworkService>();
            services.AddSingleton<IJwtAuthService, JwtAuthService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IPipelineService, PipelineService>();
            services.AddScoped<IProjectFrameworkService, ProjectFrameworkService>();
            services.AddScoped<IProjectRepositoryService, ProjectRepositoryService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectToolService, ProjectToolService>();
            services.AddScoped<IProjectTypeService, ProjectTypeService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IRepositoryService, RepositoryService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISprintService, SprintService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITestResultService, TestResultService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IToolService, ToolService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWorkflowService, WorkflowService>();
            #endregion

            #region Thead
            services.AddSingleton<IPipelineThreadService, PipelineThreadService>();
            services.AddSingleton<IWorkflowThreadService, WorkflowThreadService>();
            #endregion
            #endregion

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(option => {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config => {
                config.RequireHttpsMetadata = false;
                config.SaveToken = false;
                config.TokenValidationParameters = JwtAuth.TokenValidationParameters;
            });
            /*
            services.AddOpenApiDocument(d =>
            {
                d.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Copy 'Bearer ' + valid JWT token into field"
                });
                d.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });*/

            services.AddCors();

            services.AddSwaggerDocument(c =>
            {
                c.DocumentName = "Nococid-Api-Docs";
                c.Title = "Nococid-API";
                c.Version = "v1";
                c.Description = "The Nococid API documentation description";
                c.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT Token", new OpenApiSecurityScheme{ 
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    Description = "Copy 'Bearer ' + valid JWT token into field",
                    In = OpenApiSecurityApiKeyLocation.Header
                }));
                c.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().WithMethods("PATCH").AllowAnyHeader().Build());
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi(o => o.DocumentName = "Nococid-Api-Docs") ;
            app.UseAuthentication();
            app.UseSwaggerUi3();

            IServiceScope serviceScope = app.ApplicationServices.CreateScope();
            NococidContext context = serviceScope.ServiceProvider.GetRequiredService<NococidContext>();
            new ContextInitialization().Init(context);
        }
    }
}
