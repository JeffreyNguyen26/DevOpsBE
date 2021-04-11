using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Enums;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IReportService
    {
        object GetFrameWorkInsight(Guid user_id);
        object GetInsights(Guid user_id);
    }

    public class ReportService : ServiceBase, IReportService
    {
        private readonly IContext<TaskSubmission> _report;
        private readonly IContext<Data.Models.Task> _task;
        private readonly IContext<Commit> _commit;
        private readonly IContext<Submission> _submission;
        private readonly IContext<Pipeline> _pipeline;
        private readonly IContext<Test> _test;
        private readonly IContext<Workflow> _workflow;
        private readonly IContext<ProjectType> _projectType;
        private readonly IContext<Project> _project;

        public ReportService(IContext<Project> project, IContext<ProjectType> projectType, IContext<Test> test, IContext<Workflow> workflow, IContext<Pipeline> pipeline, IContext<Submission> submission, IContext<Commit> commit, IContext<Data.Models.Task> task, IContext<TaskSubmission> report, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _project = project;
            _projectType = projectType;
            _test = test;
            _workflow = workflow;
            _pipeline = pipeline;
            _submission = submission;
            _commit = commit;
            _task = task;
            _report = report;
        }

        public object GetFrameWorkInsight(Guid user_id)
        {
            try
            {
                var projects = _project.Where(p => p.Permissions.Any(permission => permission.UserId.Equals(user_id) && permission.RoleId.Equals(RoleID.Admin)))
                    .Select(p => new
                    {
                        ProjectId = p.Id,
                        ProjectFrameworks = p.ProjectFrameworks.Select(pf => new
                        {
                            pf.Framework.Name,
                            Language = pf.Framework.Language.Name,
                            pf.Framework.Language.Side
                        }),
                        Sprints = p.Sprints.Select(s => new
                        {
                            Submissions = s.Submissions.Select(sub => new
                            {
                                sub.Side,
                                AutoTests = sub.Tests.Select(at => new
                                {
                                    at.Passed,
                                    at.Failed
                                }).ToList(),
                                ManualTests = sub.Reports.Select(r => new
                                {
                                    r.Passed,
                                    r.Failed,
                                    r.Task.Side
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    }).ToList();

                IList<KeyValuePair<string, TestResultM>?> result = new List<KeyValuePair<string, TestResultM>?>();
                foreach (var project in projects)
                {
                    foreach (var project_framework in project.ProjectFrameworks)
                    {
                        string name = project_framework.Name + " (" + project_framework.Language + ")";
                        KeyValuePair<string, TestResultM>? framework_result = result.FirstOrDefault(r => r.Value.Key.Equals(name));
                        if (framework_result == null)
                        {
                            framework_result = new KeyValuePair<string, TestResultM>(name, new TestResultM
                            {
                                Passed = 0,
                                Failed = 0
                            });
                            result.Add(framework_result);
                        }
                        foreach (var sprint in project.Sprints)
                        {
                            foreach (var submission in sprint.Submissions)
                            {
                                if (project_framework.Side.Equals(submission.Side) || project_framework.Side.Equals("Both"))
                                {
                                    foreach (var test in submission.AutoTests)
                                    {
                                        framework_result.Value.Value.Passed += test.Passed;
                                        framework_result.Value.Value.Failed += test.Failed;
                                    }
                                }

                                foreach (var test in submission.ManualTests)
                                {
                                    if (project_framework.Side.Equals(test.Side))
                                    {
                                        framework_result.Value.Value.Passed += test.Passed;
                                        framework_result.Value.Value.Failed += test.Failed;
                                    }
                                }
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Report-GetFrameWorkInsight");
            }
        }

        public object GetInsights(Guid user_id)
        {
            try
            {
                IList<InsightTestM> tests = _test.Where(t => t.Submission.Sprint.Project.Permissions.Any(p => p.UserId.Equals(user_id) && p.RoleId.Equals(RoleID.Admin))).Select(t => new InsightTestM
                {
                    Failed = t.Failed,
                    Passed = t.Passed,
                    ProjectType = t.Submission.Sprint.Project.ProjectType,
                    Side = t.Submission.Side,
                    TaskSubmissions = t.Submission.Reports.Select(r => new InsightTaskSubmissionM
                    {
                        Failed = r.Failed,
                        Passed = r.Passed,
                        Side = r.Task.Side
                    }).ToList()
                }).ToList();

                IList<InsightTestM> server = new List<InsightTestM>();
                IList<InsightTestM> client = new List<InsightTestM>();
                foreach (var test in tests)
                {
                    switch (test.Side)
                    {
                        case "Server":
                            server.Add(test);
                            break;
                        case "Client":
                            client.Add(test);
                            break;
                        default:
                            server.Add(test);
                            client.Add(test);
                            break;
                    }
                }

                return new
                {
                    Server = GetProjectTypeInsights(server, "Server"),
                    Client = GetProjectTypeInsights(client, "Client")
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Report-GetInsights");
            }
        }

        private IList<InsightProjectTypeM> GetProjectTypeInsights(IList<InsightTestM> tests, string side)
        {
            try
            {
                var project_types = tests.GroupBy(t => t.ProjectType).Select(pt => new
                {
                    pt.Key.Name,
                    Tests = pt.ToList()
                }).ToList();

                IList<InsightProjectTypeM> result = new List<InsightProjectTypeM>();
                foreach (var project_type in project_types)
                {
                    int passed = 0;
                    int failed = 0;
                    foreach (var test in project_type.Tests)
                    {
                        foreach (var task_submission in test.TaskSubmissions)
                        {
                            if (task_submission.Side.Equals(side))
                            {
                                passed += task_submission.Passed;
                                failed += task_submission.Failed;
                            }
                        }
                        passed += test.Passed;
                        failed += test.Failed;
                    }
                    result.Add(new InsightProjectTypeM
                    {
                        Name = project_type.Name,
                        Passed = passed,
                        Failed = failed
                    });
                }

                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                       e, DateTime.Now, "Server", "Service-Report-GetProjectTypeInsights");
            }
        }
    }
}
