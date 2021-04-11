using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class CommitM
    {
        public Guid Id { get; set; }
        public string CurrentCommitId { get; set; }
        public bool IsSubmit { get; set; }
        public string Message { get; set; }
        public string MessageBody { get; set; }
    }

    public class CommitDM
    {
        public Guid Id { get; set; }
        public string CurrentCommitId { get; set; }
        public bool IsSubmit { get; set; }
        public string Message { get; set; }
        public string MessageBody { get; set; }
        public CommitAccountM Account { get; set; }
    }

    public class CommitAccountM : AccountM
    {
        public CommitRepositoryM Repository { get; set; }
    }

    public class CommitRepositoryM : RepositoryM
    {
        public CommitBranchM Branch { get; set; }
    }

    public class CommitBranchM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CommitTasksM : CommitM
    {
        public IList<TaskCommitDM> Tasks { get; set; }
    }
}
