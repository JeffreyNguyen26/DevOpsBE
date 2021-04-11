using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Nococid_API.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data
{
    public class NococidContext : DbContext
    {
        public NococidContext(DbContextOptions<NococidContext> option) : base(option) { }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Approval> Approval { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<Collaborator> Collaborator { get; set; }
        public virtual DbSet<Commit> Commit { get; set; }
        public virtual DbSet<Executor> Executor { get; set; }
        public virtual DbSet<ExecutorImage> ExecutorImage { get; set; }
        public virtual DbSet<Framework> Framework { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Pipeline> Pipeline { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectFramework> ProjectFramework { get; set; }
        public virtual DbSet<ProjectRepository> ProjectRepository { get; set; }
        public virtual DbSet<ProjectTool> ProjectTool { get; set; }
        public virtual DbSet<ProjectType> ProjectType { get; set; }
        public virtual DbSet<Repository> Repository { get; set; }
        public virtual DbSet<ResourceClass> ResourceClass { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Sprint> Sprint { get; set; }
        public virtual DbSet<Step> Step { get; set; }
        public virtual DbSet<Submission> Submission { get; set; }
        public virtual DbSet<Models.Task> Task { get; set; }
        public virtual DbSet<TaskSubmission> TaskSubmission { get; set; }
        public virtual DbSet<Test> Test { get; set; }
        public virtual DbSet<TestResult> TestResult { get; set; }
        public virtual DbSet<Tool> Tool { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Workflow> WorkFlow { get; set; }

        public void Save()
        {
            Directory.CreateDirectory("DataBaseData");
            
            SaveTable("User", User);
            SaveTable("Account", Account);
            SaveTable("Repository", Repository);
            SaveTable("Step", Step);
            SaveTable("ResourceClass", ResourceClass);
            SaveTable("Branch", Branch);
            SaveTable("ProjectType", ProjectType);
            SaveTable("Project", Project);
            SaveTable("Sprint", Sprint);
            SaveTable("Approval", Approval);
            SaveTable("Commit", Commit);
            SaveTable("Executor", Executor);
            SaveTable("Submission", Submission);
            SaveTable("Tool", Tool);
            SaveTable("ExecutorImage", ExecutorImage);
            SaveTable("Workflow", WorkFlow);
            SaveTable("Pipeline", Pipeline);
            SaveTable("Collaborator", Collaborator);
            SaveTable("Language", Language);
            SaveTable("Framework", Framework);
            SaveTable("Role", Role);
            SaveTable("Permission", Permission);
            SaveTable("ProjectFramework", ProjectFramework);
            SaveTable("ProjectRepository", ProjectRepository);
            SaveTable("ProjectTool", ProjectTool);
            SaveTable("TaskSubmission", TaskSubmission);
            SaveTable("Task", Task);
            SaveTable("Test", Test);
            SaveTable("TestResult", TestResult);
        }

        private void SaveTable<T>(string name, DbSet<T> table) where T : TableBase
        {
            IList<T> list;
            Directory.CreateDirectory("DataBaseData/" + name);
            int skip = 0, file_num = 0;
            do
            {
                list = table.OrderBy(t => t.Id).Skip(skip).Take(30).ToList();
                File.WriteAllTextAsync("DataBaseData/" + name + "/" + file_num + ".txt", JsonConvert.SerializeObject(list)).GetAwaiter().GetResult();
                skip += 30;
                file_num++;
            } while (list.Count == 30);
        }

        public void AddDataFromBackup()
        {
            Database.EnsureDeletedAsync().GetAwaiter().GetResult();
            Database.EnsureCreatedAsync().GetAwaiter().GetResult();

            AddTableDataFromBackup("User", User);
            AddTableDataFromBackup("Tool", Tool);
            AddTableDataFromBackup("Account", Account);
            AddTableDataFromBackup("Repository", Repository);
            AddTableDataFromBackup("Branch", Branch);
            AddTableDataFromBackup("ProjectType", ProjectType);
            AddTableDataFromBackup("Project", Project);
            AddTableDataFromBackup("Sprint", Sprint);
            AddTableDataFromBackup("Approval", Approval);
            AddTableDataFromBackup("Commit", Commit);
            AddTableDataFromBackup("Submission", Submission);
            AddTableDataFromBackup("Executor", Executor);
            AddTableDataFromBackup("Step", Step);
            AddTableDataFromBackup("ResourceClass", ResourceClass);
            AddTableDataFromBackup("ExecutorImage", ExecutorImage);
            AddTableDataFromBackup("Workflow", WorkFlow);
            AddTableDataFromBackup("Pipeline", Pipeline);
            AddTableDataFromBackup("Collaborator", Collaborator);
            AddTableDataFromBackup("Language", Language);
            AddTableDataFromBackup("Framework", Framework);
            AddTableDataFromBackup("Role", Role);
            AddTableDataFromBackup("Permission", Permission);
            AddTableDataFromBackup("ProjectFramework", ProjectFramework);
            AddTableDataFromBackup("ProjectRepository", ProjectRepository);
            AddTableDataFromBackup("ProjectTool", ProjectTool);
            AddTableDataFromBackup("Task", Task);
            AddTableDataFromBackup("TaskSubmission", TaskSubmission);
            AddTableDataFromBackup("Test", Test);
            AddTableDataFromBackup("TestResult", TestResult);
        }

        private void AddTableDataFromBackup<T>(string name, DbSet<T> table) where T : TableBase
        {
            string s = "";
            try
            {
                string[] file_paths = Directory.GetFiles("DataBaseData/" + name);
                foreach (var file_path in file_paths)
                {
                    s = File.ReadAllTextAsync(file_path).GetAwaiter().GetResult();
                    var entities = JsonConvert.DeserializeObject<IList<T>>(s);
                    table.AddRangeAsync(entities).GetAwaiter().GetResult();
                }
                SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new Exception(name + "-----" + s + "\n----\n" + e.Message);
            }
        }
    }
}
