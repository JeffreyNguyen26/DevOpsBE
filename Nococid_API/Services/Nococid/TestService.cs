using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface ITestService
    {
        Guid Add(Guid pipeline_id, Guid submission_id, string test_type);
        void Update(Guid test_id, int passed, int failed, TimeSpan duration);
    }

    public class TestService : ServiceBase, ITestService
    {
        private readonly IContext<Test> _test;

        public TestService(IContext<Test> test, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _test = test;
        }

        public Guid Add(Guid pipeline_id, Guid submission_id, string test_type)
        {
            try
            {
                Test test = _test.Add(new Test
                {
                    Id = pipeline_id,
                    Type = test_type
                });
                SaveChanges();
                return test.Id;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add a test!",
                    e, DateTime.Now, "Server", "Service_Test_Add");
            }
        }

        public void Update(Guid test_id, int passed, int failed, TimeSpan duration)
        {
            try
            {
                Test test = _test.GetOne(test_id);
                test.Passed = passed;
                test.Failed = failed;
                test.Duration = duration;
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while update a test!",
                    e, DateTime.Now, "Server", "Service_Test_Update");
            }
        }

        private int SaveChanges()
        {
            return _test.SaveChanges();
        }
    }
}
