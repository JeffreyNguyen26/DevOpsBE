using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Examples.Workflows
{
    public class WorkflowCreateMExample
    {
        public Guid AccountId = Guid.Empty;
        public Guid RepositoryId = Guid.Empty;
        public IList<Guid> BranchIds = new List<Guid>
        {
            Guid.Empty,
            Guid.Empty
        };
        public IList<WorkflowCreateConfigMExample> Configs = new List<WorkflowCreateConfigMExample>
        {
            new WorkflowCreateConfigMExample()
        };
    }

    public class WorkflowCreateConfigMExample
    {
        public string ConfigTool = Data.Static.ConfigTool.CircleCI;
        public string Content = "version: 2.1\n" +
            "commands:\n" +
            "  abc:\n" +
            "    steps:\n" +
            "      - run: abcxyz\n" +
            "jobs:\n" +
            "  build:\n" +
            "    machine:\n" +
            "      image: windows-server-2019-vs2019:stable\n" +
            "    resource_class: windows.medium\n" +
            "    steps:\n" +
            "    - checkout\n" +
            "    - run:\n" +
            "        name: \"Build\"\n" +
            "        command: dotnet build ./CalculatorVS\n" +
            "    - store_artifacts:\n" +
            "        path: ./CalculatorVS/bin/Debug/netcoreapp3.1\n" +
            "        destination: BuiltFile\n" +
            "  test:\n" +
            "    machine:\n" +
            "      image: windows-server-2019-vs2019:stable\n" +
            "    resource_class: windows.medium\n" +
            "    steps:\n" +
            "    - checkout\n" +
            "    - run:\n" +
            "        name: \"Test\"\n" +
            "        command: |\n" +
            "            dotnet build ./NUnit.Test\n" +
            "            dotnet test ./NUnit.Test --no-build --logger \"trx\"\n" +
            "    - run:\n" +
            "        shell: powershell.exe -ExecutionPolicy Bypass\n" +
            "        name: \"Test Results\"\n" +
            "        when: always\n" +
            "        command: |\n" +
            "            dotnet tool install -g trx2junit\n" +
            "            trx2junit ./NUnit.Test/TestResults/*.trx\n" +
            "    - store_test_results:\n" +
            "        path: ./NUnit.Test/TestResults\n" +
            "    - store_artifacts:\n" +
            "        path: ./NUnit.Test/TestResults\n" +
            "        destination: TestResults\n" +
            "workflows:\n" +
            "  version: 2\n" +
            "  workflow:\n" +
            "    jobs:\n" +
            "      - build\n" +
            "      - test:\n" +
            "          requires:\n" +
            "            - build\n";
    }
}
