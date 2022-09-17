using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMachineCodeGenerator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Tests.Common
{
    [TestClass]
    public class ErrorSeverityTests
    {
        [TestMethod]
        public void InstantiationTest() {
            Assert.AreEqual(0, ErrorSeverity.Info.Key);
            Assert.AreEqual(1, ErrorSeverity.Warning.Key);
            Assert.AreEqual(2, ErrorSeverity.Error.Key);
        }

        [TestMethod]
        public void ErrorsCollectionTest() {
            Assert.AreEqual(3, ErrorSeverity.Severities.Count);
            Assert.AreEqual("Info", ErrorSeverity.Severities[0].Value);
            Assert.AreEqual("Warning", ErrorSeverity.Severities[1].Value);
            Assert.AreEqual("Error", ErrorSeverity.Severities[2].Value);
        }
    }
}
