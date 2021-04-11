using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IBranchService
    {
        void Add(Guid repository_id, string branch_name, string sha);
        void AddMany(IList<GHBranch> branches, Guid repository_id, string default_branch_name);
        void DeleteAll(Guid repository_id);
        IList<BranchM> GetAll(Guid repository_id);
        IList<BranchM> GetAll(Guid repository_id, IList<Guid> branch_ids);
        GHBranchRequirement GetGHBranchRequirement(Guid repository_id, string branch_name = null);
    }

    public class BranchService : ServiceBase, IBranchService
    {
        private readonly IContext<Branch> _branch;

        public BranchService(IContext<Branch> branch, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _branch = branch;
        }

        public void Add(Guid repository_id, string branch_name, string sha)
        {
            try
            {
                _branch.Add(new Branch
                {
                    Sha = sha,
                    IsDefault = false,
                    Name = branch_name,
                    RepositoryId = repository_id
                });
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add a branch!",
                    e, DateTime.Now, "Server", "Service_Branch_Add");
            }
        }

        public void AddMany(IList<GHBranch> gh_branches, Guid repository_id, string default_branch_name)
        {
            try
            {
                IList<Branch> branches = _branch.GetAll(b => b.RepositoryId.Equals(repository_id));
                foreach (var branch in branches)
                {
                    branch.IsDelete = true;
                }
                if (gh_branches.Count != 0)
                {
                    foreach (var gh_branch in gh_branches)
                    {
                        Branch branch = branches.FirstOrDefault(b => b.Name.Equals(gh_branch));
                        if (branch == null)
                        {
                            _branch.Add(new Branch
                            {
                                Name = gh_branch.Name,
                                IsDefault = gh_branch.Name.Equals(default_branch_name),
                                RepositoryId = repository_id,
                                Sha = gh_branch.Commit.Sha,
                                IsDelete = false
                            });;
                        } else
                        {
                            branch.IsDelete = false;
                        }
                    }
                    SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add branches!",
                    e, DateTime.Now, "Server", "Service_Branch_AddMany");
            }
        }

        public void DeleteAll(Guid repository_id)
        {
            try
            {
                _branch.DeleteAll(_branch.Where(b => b.RepositoryId.Equals(repository_id)));
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while delete all branches!",
                    e, DateTime.Now, "Server", "Service_Branch_DeleteAll");
            }
        }

        public IList<BranchM> GetAll(Guid repository_id, IList<Guid> branch_ids)
        {
            try
            {
                if (branch_ids != null)
                {
                    if (branch_ids.Count != 0)
                    {
                        IList<BranchM> result = new List<BranchM>();
                        IList<Branch> branches = _branch.GetAll(b => b.RepositoryId.Equals(repository_id));

                        foreach (var branch in branches)
                        {
                            if (branch_ids.Any(id => id.Equals(branch.Id)))
                            {
                                result.Add(new BranchM
                                {
                                    Id = branch.Id,
                                    Name = branch.Name
                                });
                            }
                        }

                        if (result.Count != 0) return result;
                    }
                }

                throw NotFound();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all branches for creating workflow file(s)!",
                    e, DateTime.Now, "Server", "Service_Branch_GetAll");
            }
        }

        public IList<BranchM> GetAll(Guid repository_id)
        {
            try
            {
                return _branch.Where(b => b.RepositoryId.Equals(repository_id))
                    .Select(b => new BranchM
                    {
                        Id = b.Id,
                        Name = b.Name
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all branches!",
                    e, DateTime.Now, "Server", "Service_Branch_GetAll");
            }
        }

        public GHBranchRequirement GetGHBranchRequirement(Guid repository_id, string branch_name = null)
        {
            try
            {
                if (branch_name == null)
                {
                    return _branch.Where(b => b.RepositoryId.Equals(repository_id) && b.IsDefault.Equals(true))
                        .Select(b => new GHBranchRequirement
                        {
                            Name = b.Name,
                            Sha = b.Sha
                        }).FirstOrDefault();
                } else
                {
                    return _branch.Where(b => b.RepositoryId.Equals(repository_id) && b.Name.Equals(branch_name))
                        .Select(b => new GHBranchRequirement
                        {
                            Name = b.Name,
                            Sha = b.Sha
                        }).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get github branch requirement for calling API!",
                    e, DateTime.Now, "Server", "Service_Branch_GetGHBranchRequirement");
            }
        }

        private int SaveChanges()
        {
            return _branch.SaveChanges();
        }
    }
}
