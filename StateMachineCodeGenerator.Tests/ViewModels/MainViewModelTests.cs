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
