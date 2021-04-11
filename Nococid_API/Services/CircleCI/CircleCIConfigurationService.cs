using Nococid_API.Data.Static;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.CircleCI
{
    public interface ICircleCIConfigurationService
    {
        string CreateConfigurationContent(string content);
    }

    public class CircleCIConfigurationService : CircleCIServiceBase, ICircleCIConfigurationService
    {
        public CircleCIConfigurationService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public string CreateConfigurationContent(string content)
        {
            StringReader sr = null;
            try
            {
                string result = "";

                sr = new StringReader(content);
                string line = null;
                int line_number = -1;
                bool has_command = false;
                int number_of_jobs = 0;
                do
                {
                    line = sr.ReadLineAsync().GetAwaiter().GetResult();
                    if (line == null) break;
                    result += line + "\n";
                    line_number++;
                    if (line.StartsWith("commands:"))
                    {
                        result += nococid_command + "\n";
                        has_command = true;
                    } else if (line.StartsWith("    jobs:"))
                    {
                        result += UpdateWorkflowJobs(ref sr, out int value);
                        number_of_jobs = value;
                    } else if (line.StartsWith("jobs:"))
                    {
                        result += nococid_job + "\n";
                    }
                } while (true);

                if (!has_command)
                {
                    result += "commands:\n" + nococid_command;
                }

                result = result.Replace("{nococid-number-of-job}", number_of_jobs.ToString());
                return result;
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("Failed to create configuration content!",
                    e, DateTime.Now, "Server", "Service_CircleCIConfiguration_CreateConfigurationContent");
            } finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }

        private string UpdateWorkflowJobs(ref StringReader sr, out int number_of_jobs)
        {
            try
            {
                IList<string> jobs = new List<string>();
                string result = "";
                string line = sr.ReadLineAsync().GetAwaiter().GetResult();
                if (!line.Contains(":"))
                {
                    string job;
                    int index = line.IndexOf("- ");
                    for (int i = index + 2; i < line.Length; i++)
                    {
                        if (line.ElementAt(i).Equals(' '))
                        {
                            result += line.Substring(0, i) + ":" + line.Substring(i) + "\n";
                            job = line.Substring(index + 2, i - index - 2);
                            if (!jobs.Any(j => j.Equals(job))) jobs.Add(job);
                            break;
                        }
                        else if (i == (line.Length - 1))
                        {
                            result += line + ":\n";
                            job = line.Substring(index + 2);
                            if (!jobs.Any(j => j.Equals(job))) jobs.Add(job);
                        }
                    }
                }
                else
                {
                    result += line + "\n";
                    int index = line.IndexOf("- ");
                    string job = line.Substring(index + 2, line.IndexOf(":") - index - 2);
                    if (!jobs.Any(j => j.Equals(job))) jobs.Add(job);
                }
                line = sr.ReadLineAsync().GetAwaiter().GetResult();
                if (line == null)
                {
                    number_of_jobs = 1;
                    return result +=
                            "          requires:\n" +
                            "            - " + start_workflow_job_name + "\n" +
                            "          post-steps:\n" +
                            "            - " + job_done_command_name + "\n" +
                            "      - " + start_workflow_job_name;
                }

                bool has_requires = false;
                bool has_post_steps = false;
                do
                {
                    has_requires = false;
                    has_post_steps = false;
                    while (line.StartsWith("        "))
                    {
                        result += line + "\n";
                        if (line.Trim().StartsWith("requires:"))
                        {
                            result += line.Substring(0, line.IndexOf("requires:")) + "  - " + start_workflow_job_name + "\n";
                            has_requires = true;
                        }
                        else if (line.Trim().StartsWith("post-steps:"))
                        {
                            result += line.Substring(0, line.IndexOf("post-steps:")) + "  - " + job_done_command_name + "\n";
                            has_post_steps = true;
                        }
                        line = sr.ReadLineAsync().GetAwaiter().GetResult();
                        if (line == null) break;
                    }
                    if (!has_requires)
                    {
                        result += 
                            "          requires:\n" +
                            "            - " + start_workflow_job_name + "\n";
                    }
                    if (!has_post_steps)
                    {
                        result += 
                            "          post-steps:\n" +
                            "            - " + job_done_command_name + "\n";
                    }
                    if (line == null) break;
                    if (!line.Contains(":"))
                    {
                        string job;
                        int index = line.IndexOf("- ");
                        for (int i = index + 2; i < line.Length; i++)
                        {
                            if (line.ElementAt(i).Equals(' '))
                            {
                                result += line.Substring(0, i) + ":" + line.Substring(i) + "\n";
                                job = line.Substring(index + 2, i - index - 2);
                                if (!jobs.Any(j => j.Equals(job))) jobs.Add(job);
                                break;
                            } else if (i == (line.Length - 1))
                            {
                                result += line + ":\n";
                                job = line.Substring(index + 2);
                                if (!jobs.Any(j => j.Equals(job))) jobs.Add(job);
                            }
                        }
                    } else
                    {
                        result += line + "\n";
                        int index = line.IndexOf("- ");
                        string job = line.Substring(index + 2, line.IndexOf(":") - index - 2);
                        if (!jobs.Any(j => j.Equals(job))) jobs.Add(job);
                    }
                    line = sr.ReadLineAsync().GetAwaiter().GetResult();
                    if (line == null)
                    {
                        result +=
                            "          requires:\n" +
                            "            - " + start_workflow_job_name + "\n" +
                            "          post-steps:\n" +
                            "            - " + job_done_command_name + "\n";
                        break;
                    }
                } while (line.StartsWith("      "));

                result += "      - " + start_workflow_job_name + "\n";
                number_of_jobs = jobs.Count();
                return result;
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("Failed to update workflow jobs!",
                    e, DateTime.Now, "Server", "Service_CircleCIConfiguration_UpdateWorkflowJobs");
            }
        }

        private static readonly string curl = "curl --header \"Content-Type:application/json\" --data '' -X POST";
        private static readonly string start_workflow_url = 
            "\"http://toan0701.ddns.net:9080/Nococid/api/Listeners/CircleCI/start-workflow?" +
            "sprint_id={nococid-sprint-id}&" +
            "number_of_jobs={nococid-number-of-job}&" +
            "gh_username=${CIRCLE_PROJECT_USERNAME}&" +
            "circle_workflow_id=${CIRCLE_WORKFLOW_ID}\"";
        private static readonly string job_done_url =
            "\"http://toan0701.ddns.net:9080/Nococid/api/Listeners/CircleCI/job-done?" +
            "circle_workflow_id=${CIRCLE_WORKFLOW_ID}&" +
            "circle_job_num=${CIRCLE_BUILD_NUM}&" +
            "stage=${CIRCLE_STAGE}&" +
            "gh_username=${CIRCLE_PROJECT_USERNAME}&" +
            "gh_repository=${CIRCLE_PROJECT_REPONAME}\"";

        private static readonly string start_workflow_command_name = "nococid-command-start-workflow";
        private static readonly string job_done_command_name = "nococid-command-job-done";
        private static readonly string nococid_command =
            "  " + start_workflow_command_name + ":\n" +
            "    steps:\n" +
            "      - run: " + curl + " " + start_workflow_url + "\n" +
            "  " + job_done_command_name + ":\n" +
            "    steps:\n" +
            "      - run:\n" +
            "          when: always\n" +
            "          command: " + curl + " " + job_done_url;

        private static readonly string start_workflow_job_name = "nococid-start-workflow";

        private static readonly string nococid_job =
            "  " + start_workflow_job_name + ":\n" +
            "    machine:\n" +
            "      image: windows-server-2019-vs2019:stable\n" +
            "    resource_class: windows.medium\n" +
            "    steps:\n" +
            "    - " + start_workflow_command_name;
    }
}
