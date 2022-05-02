using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StateMachineCodeGenerator.Common.Extensions;

namespace StateMachineCodeGenerator.Tests.Common.Extensions
{
    [TestClass]
    public class DirectoryInfoExtensionsTests
    {
        [TestMethod]
        public void GetFilesTest() {
            var dir = new DirectoryInfo(@"C:\Users\santosj25\source\GenSysTest\MotionSolutions\LLSystemNew");
            if (dir.Exists == false) { Assert.Inconclusive("directory not found"); }

            var actual = dir.GetFiles("*.sln");
            Assert.IsTrue(actual.Any());
        }

        [TestMethod]
        public void GetFilesInSiblingsTest() {
            var dir = new DirectoryInfo(@"C:\Users\santosj25\source\GenSysTest\MotionSolutions\LLSystemNew\Modeling");
            if (dir.Exists == false) { Assert.Inconclusive("directory not found"); }

            var actual = dir.GetFilesInSiblings("*.xml");
            Assert.IsTrue(actual.Any());
        }

        [TestMethod]
        public void FindFirstFilesInAncestorsTest() {
            var dir = new DirectoryInfo(@"C:\Users\santosj25\source\GenSysTest\MotionSolutions\LLSystemNew\Modeling");
            if (dir.Exists == false) { Assert.Inconclusive("directory not found"); }

            var actual = dir.FindFirstFilesInAncestors("*.sln");
            Assert.IsTrue(actual.Any());
        }
    }
}
