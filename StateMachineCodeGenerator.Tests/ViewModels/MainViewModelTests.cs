using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMachineCodeGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Tests.ViewModels
{
    [TestClass]
    public class MainViewModelTests {

        [TestMethod]
        public void InstantiationTest() {
            var target = new MainViewModel();
            Assert.IsNotNull(target);
        }

        [DataTestMethod]
        [DataRow("namespace Common", "Common")]
        [DataRow("namespace StateMachineCodeGenerator.ViewModels", "StateMachineCodeGenerator.ViewModels")]
        [DataRow(" namespace StateMachineCodeGenerator.ViewModels", "StateMachineCodeGenerator.ViewModels")]
        [DataRow("namespace StateMachineCodeGenerator.ViewModels  ", "StateMachineCodeGenerator.ViewModels")]
        [DataRow("namespace StateMachineCodeGenerator.ViewModels  {", "StateMachineCodeGenerator.ViewModels")]
        [DataRow("  namespace StateMachineCodeGenerator.ViewModels  {", "StateMachineCodeGenerator.ViewModels")]
        [DataRow("using system;", null)]
        [DataRow("using system; namespace MyNamespace", "MyNamespace")]
        public void GetNamespaceValueTest1(string line, string expected) {
            //var target = new MainViewModel();
            var actual = MainViewModel.GetNamespaceValue(line);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void HashSetTest() {
            var set = new HashSet<string>();
            set.Add("value");
            set.Add("value");
            set.Add("value");
            Assert.AreEqual(1, set.Count);
        }
    }
}
