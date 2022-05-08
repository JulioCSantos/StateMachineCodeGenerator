using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMachineMetadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
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
            target.SelectedEaModelName = "MyModelName";
            target.TargetFilesDirectoryName =
                    @"C:\Users\julio\Documents\Visual Studio 2019\Projects\MyCompanies\Corning\TemplateGrid\TemplateGrid.sln";
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
            target.SelectedEaModelName = "MyModelName";
            target.TargetFilesDirectoryName =
                @"C:\Users\julio\Documents\Visual Studio 2019\Projects\MyCompanies\Corning\TemplateGrid\TemplateGrid.sln";
        }

        [TestMethod]
        public void TargetFilesEventsTest2() {
            var target = new DerivedTargetFilesDirectory();
            var eventsSet = new HashSet<string>();
            target.PropertyChanged += (sender, args) => eventsSet.Add(args.PropertyName);
            target.TargetFilesDirectoryName =
                @"C:\Users\julio\Documents\Visual Studio 2019\Projects\MyCompanies\Corning\TemplateGrid\TemplateGrid.sln";
            target.SelectedEaModelName = "MyModelName";
        }

        //[DataTestMethod]
        //[DataRow("namespace Common", "Common")]
        //[DataRow("namespace StateMachineCodeGenerator.ViewModels", "StateMachineCodeGenerator.ViewModels")]
        //[DataRow(" namespace StateMachineCodeGenerator.ViewModels", "StateMachineCodeGenerator.ViewModels")]
        //[DataRow("namespace StateMachineCodeGenerator.ViewModels  ", "StateMachineCodeGenerator.ViewModels")]
        //[DataRow("namespace StateMachineCodeGenerator.ViewModels  {", "StateMachineCodeGenerator.ViewModels")]
        //[DataRow("  namespace StateMachineCodeGenerator.ViewModels  {", "StateMachineCodeGenerator.ViewModels")]
        //[DataRow("using system;", null)]
        //[DataRow("using system; namespace MyNamespace", "MyNamespace")]
        //public void GetNamespaceValueTest1(string line, string expected) {
        //    var actual = TargetFilesDirectory.GetNamespaceValue(line);
        //    //var actual = MainViewModel.GetNamespaceValue(line);
        //    Assert.AreEqual(expected, actual);
        //}
 
        [TestMethod]
        public void TargetSolutionFileNameValidTest2() {
            var target = new DerivedTargetFilesDirectory();
            target.SolutionFileName = @"C:\Folder\SolutionFileName.sln";
            Assert.IsNotNull(target.SolutionFileName);
            Assert.IsNotNull(target.SolutionFileInfo);
            Assert.AreEqual(@"C:\Folder\Main\Model\MainModel", target.TargetFilesDirectoryName);
            Assert.IsNotNull(target.TargetFilesDirectoryInfo);
        }

        [TestMethod]
        public void TargetFilesPathTest() {
            var target = new DerivedTargetFilesDirectory();
            target.SolutionFileName = @"C:\Folder\SolutionFileName.sln";
            target.SelectedEaModelName = "ModelName";
            Assert.IsNotNull(target.SolutionFileName);
            Assert.IsNotNull(target.SolutionFileInfo);
            Assert.AreEqual(@"C:\Folder\Main\Model\MainModel", target.TargetFilesDirectoryName);
            Assert.IsNotNull(target.TargetFilesDirectoryInfo);
            Assert.AreEqual(@"C:\Folder\Main\Model\MainModel", target.TargetFilesDirectoryInfo.FullName);
        }

        [TestMethod]
        public void TargetFilesPathTest2() {
            var target = new DerivedTargetFilesDirectory();
            target.SolutionFileName = @"C:\Folder\SolutionFileName.sln";
            target.SelectedEaModelName = "ModelName";
            Assert.IsNotNull(target.SolutionFileName);
            Assert.IsNotNull(target.SolutionFileInfo);
            Assert.AreEqual(@"C:\Folder\Main\Model\MainModel", target.TargetFilesDirectoryName);
            Assert.IsNotNull(target.TargetFilesDirectoryInfo);
            Assert.AreEqual(@"C:\Folder\Main\Model\MainModel", target.TargetFilesDirectoryInfo.FullName);
            target.SelectedEaModelName = "ModelName2";
            Assert.AreEqual(@"C:\Folder\Main\Model\MainModel", target.TargetFilesDirectoryName);
            Assert.AreEqual(@"C:\Folder\Main\Model\MainModel", target.TargetFilesDirectoryInfo.FullName);
        }

        [TestMethod]
        public void EaXmlFileNameSetupTest() {
            var target = new DerivedTargetFilesDirectory();

            var eaXmlFileInfo = TestsBootstrapper.InputTestsFileInfo.GetFiles("WocGuide Model test.xml").FirstOrDefault();
            target.EaXmlFileName = eaXmlFileInfo?.FullName;

            Assert.IsTrue(target.EaXmlFileInfo.Exists);
            Assert.IsNotNull(target.EaModelsList.Any());
            Assert.IsNotNull(target.SelectedEaModel);
            Assert.IsNotNull(target.SelectedEaModelName);

            target.EaXmlFileName = null;
            Assert.IsFalse(target.EaXmlFileInfo?.Exists == true);
        }

        [TestMethod]
        public void SolutionNameSetupTest1() {
            var target = new DerivedTargetFilesDirectory();

            var solutionFile = TestsBootstrapper.InputTestsFileInfo.GetFiles("TemplateGrid.sln").FirstOrDefault();
            target.SolutionFileName = solutionFile?.FullName;

            Assert.IsNotNull(target.SolutionFileInfo);
            Assert.IsTrue(target.SolutionFileInfo.Exists);
            Assert.IsTrue(target.NamespacesList.Any());
            Assert.IsTrue(string.IsNullOrEmpty(target.SelectedNameSpace) == false);
            Assert.IsTrue(string.IsNullOrEmpty(target.TargetFilesDirectoryName) == false);
            Assert.IsNotNull(target.TargetFilesDirectoryInfo);
        }

        [TestMethod]
        public void GeneratedFilesTest1() {
            var target = new DerivedTargetFilesDirectory();

            var eaXmlFileInfo = TestsBootstrapper.InputTestsFileInfo.GetFiles("WocGuide Model test.xml").FirstOrDefault();
            target.TargetFilesDirectoryName = TestsBootstrapper.SolutionInfo.FullName;
            var solutionFile = TestsBootstrapper.InputTestsFileInfo.GetFiles("TemplateGrid.sln").FirstOrDefault();
            target.SolutionFileName = solutionFile?.FullName;


            Assert.IsTrue(string.IsNullOrEmpty(target.StateMachineBaseFileName) == false);
            Assert.IsTrue(string.IsNullOrEmpty(target.StateMachineDerivedFileName) == false);
            Assert.IsTrue(string.IsNullOrEmpty(target.MainModelBaseFileName) == false);
            Assert.IsTrue(string.IsNullOrEmpty(target.MainModelDerivedFileName) == false);
        }

        [TestMethod]
        public void CloneTest1() {
            var target = new DerivedTargetFilesDirectory();

            target.EaXmlFileName = TestsBootstrapper.InputTestsFileInfo.GetFiles("WocGuide Model test.xml").FirstOrDefault()?.FullName;
            target.SolutionFileName = TestsBootstrapper.InputTestsFileInfo.GetFiles("TemplateGrid.sln").FirstOrDefault()?.FullName;

            var clone = target.Clone;
            Assert.IsNotNull(clone);
            Assert.AreNotEqual(target, clone);
            Assert.AreEqual(target.EaXmlFileName, clone.EaXmlFileName);
            Assert.AreNotSame( target.EaXmlFileName, clone.EaXmlFileName);
            Assert.AreEqual(target.SolutionFileName, clone.SolutionFileName);
            Assert.AreNotSame(target.SolutionFileName, clone.SolutionFileName);
            Assert.AreEqual(target.TargetFilesDirectoryName, clone.TargetFilesDirectoryName);
            Assert.AreNotSame(target.TargetFilesDirectoryName, clone.TargetFilesDirectoryName);
            Assert.AreEqual(target.SelectedEaModelName, clone.SelectedEaModelName);
            Assert.AreNotSame(target.SelectedEaModelName, clone.SelectedEaModelName);
            Assert.AreEqual(target.SelectedNameSpace, clone.SelectedNameSpace);
            Assert.AreNotSame(target.SelectedNameSpace, clone.SelectedNameSpace);
            Assert.AreEqual(target.StateMachineBaseFileName, clone.StateMachineBaseFileName);
            Assert.AreNotSame(target.StateMachineBaseFileName, clone.StateMachineBaseFileName);
            Assert.AreEqual(target.StateMachineDerivedFileName, clone.StateMachineDerivedFileName);
            Assert.AreNotSame(target.StateMachineDerivedFileName, clone.StateMachineDerivedFileName);
            Assert.AreEqual(target.MainModelBaseFileName, clone.MainModelBaseFileName);
            Assert.AreNotSame(target.MainModelBaseFileName, clone.MainModelBaseFileName);
            Assert.AreEqual(target.MainModelDerivedFileName, clone.MainModelDerivedFileName);
            Assert.AreNotSame(target.MainModelDerivedFileName, clone.MainModelDerivedFileName);
        }

        [TestMethod]
        public void CloneTest2() {
            var target = new DerivedTargetFilesDirectory();

            target.EaXmlFileName = TestsBootstrapper.InputTestsFileInfo.GetFiles("WocGuide Model test.xml").FirstOrDefault()?.FullName;
            target.SolutionFileName = TestsBootstrapper.InputTestsFileInfo.GetFiles("TemplateGrid.sln").FirstOrDefault()?.FullName;
            target.SelectedEaModelName = target.EaModelsList.Skip(1).FirstOrDefault()?.ToString();
            target.SelectedNameSpace += "New";

            var clone = target.Clone;
            Assert.IsNotNull(clone);
            Assert.AreNotEqual(target, clone);
            Assert.AreEqual(target.EaXmlFileName, clone.EaXmlFileName);
            Assert.AreNotSame(target.EaXmlFileName, clone.EaXmlFileName);
            Assert.AreEqual(target.SolutionFileName, clone.SolutionFileName);
            Assert.AreNotSame(target.SolutionFileName, clone.SolutionFileName);
            Assert.AreEqual(target.TargetFilesDirectoryName, clone.TargetFilesDirectoryName);
            Assert.AreNotSame(target.TargetFilesDirectoryName, clone.TargetFilesDirectoryName);
            Assert.AreEqual(target.SelectedEaModelName, clone.SelectedEaModelName);
            Assert.AreNotSame(target.SelectedEaModelName, clone.SelectedEaModelName);
            Assert.AreEqual(target.SelectedNameSpace, clone.SelectedNameSpace);
            Assert.AreNotSame(target.SelectedNameSpace, clone.SelectedNameSpace);
            Assert.AreEqual(target.StateMachineBaseFileName, clone.StateMachineBaseFileName);
            Assert.AreNotSame(target.StateMachineBaseFileName, clone.StateMachineBaseFileName);
            Assert.AreEqual(target.StateMachineDerivedFileName, clone.StateMachineDerivedFileName);
            Assert.AreNotSame(target.StateMachineDerivedFileName, clone.StateMachineDerivedFileName);
            Assert.AreEqual(target.MainModelBaseFileName, clone.MainModelBaseFileName);
            Assert.AreNotSame(target.MainModelBaseFileName, clone.MainModelBaseFileName);
            Assert.AreEqual(target.MainModelDerivedFileName, clone.MainModelDerivedFileName);
            Assert.AreNotSame(target.MainModelDerivedFileName, clone.MainModelDerivedFileName);
        }


    }

    public class DerivedTargetFilesDirectory : TargetFilesDirectory {
        public new string SelectedEaModelName {
            get => base.SelectedEaModelName;
            set => base.SelectedEaModelName = value;
        }
    }
}
