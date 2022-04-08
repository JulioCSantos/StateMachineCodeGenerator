using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMachineMetadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Tests.Metadata
{
    [TestClass]
    public class TargetFilesDirectoryTests
    {
        [TestMethod]
        public void InstantiationTest() {
            var target = new DerivedTargetFilesDirectory();
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void TargetFilesNamesTest() {
            var target = new DerivedTargetFilesDirectory();
            Assert.IsNull(target.StateMachineBaseFileName);
            Assert.IsNull(target.StateMachineDerivedFileName);
            Assert.IsNull(target.MainModelBaseFileName);
            Assert.IsNull(target.MainModelDerivedFileName);
            target.EAModelName = "MyModelName";
            target.TargetFilesPath =
                new FileInfo(
                    @"C:\Users\julio\Documents\Visual Studio 2019\Projects\MyCompanies\Corning\TemplateGrid\TemplateGrid.sln").Directory;
            Assert.IsNotNull(target.StateMachineBaseFileName);
            Assert.IsNotNull(target.StateMachineDerivedFileName);
            Assert.IsNotNull(target.MainModelBaseFileName);
            Assert.IsNotNull(target.MainModelDerivedFileName);
        }

        [TestMethod]
        public void TargetFilesEventsTest() {
            var target = new DerivedTargetFilesDirectory();
            var eventsSet = new HashSet<string>();
            target.PropertyChanged += (sender, args) => eventsSet.Add(args.PropertyName);
            Assert.AreEqual(0, eventsSet.Count);
            target.EAModelName = "MyModelName";
            Assert.AreEqual(1, eventsSet.Count);
            target.TargetFilesPath =
                new FileInfo(
                    @"C:\Users\julio\Documents\Visual Studio 2019\Projects\MyCompanies\Corning\TemplateGrid\TemplateGrid.sln").Directory;
            Assert.AreEqual(6, eventsSet.Count);
        }

        [TestMethod]
        public void TargetFilesEventsTest2() {
            var target = new DerivedTargetFilesDirectory();
            var eventsSet = new HashSet<string>();
            target.PropertyChanged += (sender, args) => eventsSet.Add(args.PropertyName);
            target.TargetFilesPath =
                new FileInfo(
                    @"C:\Users\julio\Documents\Visual Studio 2019\Projects\MyCompanies\Corning\TemplateGrid\TemplateGrid.sln").Directory;
            Assert.AreEqual(1, eventsSet.Count);
            target.EAModelName = "MyModelName";
            Assert.AreEqual(6, eventsSet.Count);
        }

        //[TestMethod]
        //public void TargetFilesNamesTest() {
        //    var target = new DerivedTargetFilesDirectory();
        //    var eAXMLFilePathLiteral = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata new\Dependencies\LaserProcessing Model new.xml";
        //    target.SolutionFileName = eAXMLFilePathLiteral;
        //    Assert.IsNotNull(target);
        //}
    }

    public class DerivedTargetFilesDirectory : TargetFilesDirectory
    {
        public DerivedTargetFilesDirectory() :base() {
            
        }

    }
}
