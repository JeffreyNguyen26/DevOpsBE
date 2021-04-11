using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface ITestResultService
    {
        void AddManyCircleCI(Guid test_id, IList<CircleCIJobTestItemM> test_items);
    }

    public class TestResultService : ServiceBase, ITestResultService
    {
        private readonly IContext<TestResult> _testResult;

        public TestResultService(IContext<TestResult> testResult, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _testResult = testResult;
        }

        public void AddManyCircleCI(Guid test_id, IList<CircleCIJobTestItemM> test_items)
        {
            try
            {
                foreach (var test_item in test_items)
                {
                    _testResult.Add(new TestResult
                    {
                        ClassName = test_item.Classname,
                        Duration = TimeSpan.FromMilliseconds(test_item.Run_time * 1000),
                        Message = test_item.Message,
                        Name = test_item.Name,
                        Passed = test_item.Result.Equals("success"),
                        TestId = test_id
                    });
                }
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add a test result!",
                    e, DateTime.Now, "Server", "Service_TestResult_Add");
            }
        }

        private int SaveChanges()
        {
            return _testResult.SaveChanges();
        }
    }
}
