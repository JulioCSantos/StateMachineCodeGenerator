using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMachineMetadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            var actual = TargetFilesDirectory.GetNamespaceValue(line);
            //var actual = MainViewModel.GetNamespaceValue(line);
            Assert.AreEqual(expected, actual);
        }

 
        [TestMethod]
        public void TargetSolutionFileNameValidTest2() {
            var target = new DerivedTargetFilesDirectory();
            target.SolutionFileName = @"C:\Folder\SolutionFileName.sln";
            Assert.IsNotNull(target.SolutionFileName);
            Assert.IsNotNull(target.SolutionFileInfo);
            Assert.AreEqual(@"C:\Folder", target.TargetFilesDirectoryName);
            Assert.IsNotNull(target.TargetFilesDirectoryPath);
        }

        [TestMethod]
        public void TargetFilesPathTest() {
            var target = new DerivedTargetFilesDirectory();
            target.SolutionFileName = @"C:\Folder\SolutionFileName.sln";
            target.SelectedEaModelName = "ModelName";
            Assert.IsNotNull(target.SolutionFileName);
            Assert.IsNotNull(target.SolutionFileInfo);
            Assert.AreEqual(@"C:\Folder\ModelName", target.TargetFilesDirectoryName);
            Assert.IsNotNull(target.TargetFilesDirectoryPath);
            Assert.AreEqual(@"C:\Folder\ModelName", target.TargetFilesDirectoryPath.FullName);
        }

        [TestMethod]
        public void EaXmlFileSetTest() {
            var target = new DerivedTargetFilesDirectory();
            Assert.IsFalse(target.EaXmlFileInfo?.Exists == true);
            var origFile = new FileInfo(
                @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\InputTestsFiles\LaserProcessing Model new.xml");
            target.EaXmlFileName = origFile.FullName;
            Assert.IsTrue(target.EaXmlFileInfo?.Exists == true);
            target.EaXmlFileName = null;
            Assert.IsFalse(target.EaXmlFileInfo?.Exists == true);
        }

        //[TestMethod]
        //public async Task EaXmlFileRefreshTest() {
        //    var target = new DerivedTargetFilesDirectory();
        //    var origFile = new FileInfo(
        //            @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\InputTestsFiles\LaserProcessing Model new.xml");
        //    var testFile = new FileInfo(Path.Combine(origFile.DirectoryName ?? "", "LaserProcessing Model test.xml"));
        //    var otherFile = new FileInfo(Path.Combine(origFile.DirectoryName ?? "", "WocGuide Model.xml"));
        //    File.Copy(origFile.FullName, testFile.FullName, true);
        //    Assert.IsNull(target.SelectedEaModel);
        //    target.EaXmlFileName = testFile.FullName;
        //    Assert.IsNotNull(target.SelectedEaModel);
        //    var prevModelName = target.SelectedEaModelName;
        //    //File.Delete(testFile.FullName);
        //    target.PropertyChanged += Target_PropertyChanged;
        //    testFile.CreationTime = DateTime.Now;
        //    var propSet = false;
        //    do {
        //        var pceArgs =  await PropertyChangedAsync(target);
        //        if (pceArgs.PropertyName == nameof(target.SelectedEaModelName)) { propSet = true; }

        //    } while (propSet == false);
        //    File.Copy(otherFile.FullName, testFile.FullName, true);
        //    var newModelName = target.SelectedEaModelName;
        //    Assert.AreNotEqual(prevModelName, newModelName);
        //}

        //public Task<PropertyChangedEventArgs> PropertyChangedAsync(INotifyPropertyChanged sender) {

        //    var tcs = new TaskCompletionSource<PropertyChangedEventArgs>();
        //    var handler = new Action<object, PropertyChangedEventArgs>(
        //        (s, p) => {
        //            tcs.TrySetResult(p);
        //        }
                
        //    );
        //    sender.PropertyChanged += (s, p) => handler(s, p);
        //    return tcs.Task.ContinueWith(p => {
        //        sender.PropertyChanged -= (s, p) => handler(s, p); 
        //        return p.Result;
        //    });
        //}

        //[TestMethod]
        //public void TargetFilesNamesTest() {
        //    var target = new DerivedTargetFilesDirectory();
        //    var eAXMLFilePathLiteral = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata new\Dependencies\LaserProcessing Model new.xml";
        //    target.SolutionFileName = eAXMLFilePathLiteral;
        //    Assert.IsNotNull(target);
        //}
    }

    public class DerivedTargetFilesDirectory : TargetFilesDirectory {
        public new string SelectedEaModelName {
            get => base.SelectedEaModelName;
            set => base.SelectedEaModelName = value;
        }
    }
}
