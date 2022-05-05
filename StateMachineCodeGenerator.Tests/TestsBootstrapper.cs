using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StateMachineCodeGenerator.Tests
{
    [TestClass]
    public class TestsBootstrapper
    {
        public static TestContext TestContext { get; private set; }
        public static DirectoryInfo SolutionInfo { get; private set; }
        public static DirectoryInfo InputTestsFileInfo { get; private set; }

        [AssemblyInitialize]
        public static void MyTestInitialize(TestContext testContext) {
            TestContext = testContext;
            
            var solutionPath = Directory
                .GetParent(Assembly.GetExecutingAssembly().Location)
                ?.Parent?.Parent?.Parent?.Parent;
            if (solutionPath == null) { throw new Exception();};

            testContext.Properties.Add("SolutionInfo", solutionPath);
            SolutionInfo = solutionPath;
            var inputTestsFileInfo = solutionPath.GetDirectories("InputTestsFiles").FirstOrDefault();
            Assert.IsTrue(inputTestsFileInfo?.Exists);
            InputTestsFileInfo = inputTestsFileInfo;
        }
    }
}
