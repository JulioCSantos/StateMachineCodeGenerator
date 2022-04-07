using StateMachineCodeGenerator.Common;
using StateMachineMetadata;
using StateMachineMetadata.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class MainViewModel : SetPropertyBase
    {
        #region Commands
        public RelayCommand StartParsingCommand => new RelayCommand((o) => StartParsing());
        public RelayCommand OpenFileExplorerCommand => new RelayCommand(OpenFileExplorer);
        #endregion Commands

        #region EAXMLFileName
        public const string EAXMLFilePathLiteral = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata new\Dependencies\LaserProcessing Model new.xml";
        //public const string EAXMLFilePathLiteral = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata\Dependencies\LaserProcessing Model new.xml";
        //private string _eaxmlFileName = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata\Dependencies\LaserProcessing Model new.xml";
        private string _eaxmlFileName;
        public string EAXMLFileName {
            get => _eaxmlFileName;
            set => SetProperty(ref _eaxmlFileName, value);
        }
        #endregion EAXMLFileName

        #region EAXMLFileInfo
        public FileInfo EAXMLFileInfo => EAXMLFileInfo == null ? null : new FileInfo(EAXMLFileName);
        #endregion EAXMLFileInfo

        #region TargetSolutionFileName
        public const string TargetSolutionLiteral = @"C:\Users\julio\Documents\Visual Studio 2019\Projects\MyCompanies\Corning\TemplateGrid\TemplateGrid.sln";
        //public const string TargetSolutionLiteral = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";
        //public const string TargetSolutionLiteral = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";
        private string _targetSolutionFileName;
        public string TargetSolutionFileName {
            get => _targetSolutionFileName;
            set => SetProperty(ref _targetSolutionFileName, value);
        }
        #endregion TargetSolutionFileName

        #region TargetSolutionFileInfo
        public FileInfo TargetSolutionFileInfo => TargetSolutionFileName == null ? null : new FileInfo(TargetSolutionFileName);
        #endregion TargetSolutionFileInfo

        #region EAModelsList
        private List<StateMachineMetadata.Model.MainModel> _eAModelsList;
        public List<StateMachineMetadata.Model.MainModel> EAModelsList {
            get => _eAModelsList;
            set => SetProperty(ref _eAModelsList, value);
        }
        #endregion EAModelsList

        #region SelectedEAModel
        private StateMachineMetadata.Model.MainModel _selectedEAModel;
        public StateMachineMetadata.Model.MainModel SelectedEAModel {
            get => _selectedEAModel;
            set => SetProperty(ref _selectedEAModel, value);
        }
        #endregion SelectedEAModel

        #region NamespacesList
        private List<string> _namespacesList;
        public List<string> NamespacesList {
            get => _namespacesList;
            set => SetProperty(ref _namespacesList, value);
        }
        #endregion NamespacesList

        #region SelectedNameSpace
        private string _selectedNameSpace;
        public string SelectedNameSpace {
            get => _selectedNameSpace;
            set => SetProperty(ref _selectedNameSpace, value);
        }
        #endregion SelectedNameSpace

        #region CsFiles
        private List<FileInfo> _csFiles;
        public List<FileInfo> CsFiles {
            get => _csFiles;
            set => SetProperty(ref _csFiles, value);
        }
        #endregion CsFiles

        #region IsModelSelectable
        public bool IsModelSelectable => (EAModelsList?.Count ?? 0) > 1;
        #endregion IsModelSelectable

        #region StateMachineBaseFileName
        private string _stateMachineBaseFileName;
        public string StateMachineBaseFileName {
            get => _stateMachineBaseFileName;
            set => SetProperty(ref _stateMachineBaseFileName, value);
        }
        #endregion StateMachineBaseFileName

        #region StateMachineDerivedFileName
        private string _stateMachineDerivedFileName;
        public string StateMachineDerivedFileName {
            get => _stateMachineDerivedFileName;
            set => SetProperty(ref _stateMachineDerivedFileName, value);
        }
        #endregion StateMachineDerivedFileName

        #region MainModelBaseFilePathString
        private string _mainModelBaseFilePathString;
        public string MainModelBaseFilePathString {
            get => _mainModelBaseFilePathString;
            set => SetProperty(ref _mainModelBaseFilePathString, value);
        }
        #endregion MainModelBaseFilePathString

        #region MainModelDerivedFileName
        private string _mainModelDerivedFileName;
        public string MainModelDerivedFileName {
            get => _mainModelDerivedFileName;
            set => SetProperty(ref _mainModelDerivedFileName, value);
        }
        #endregion MainModelDerivedFileName

        #region constructor
        public MainViewModel() {
            this.PropertyChanged += MainViewModel_PropertyChanged;
        }

        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(EAXMLFileName):
                    EAModelsList = Main.GetStateMachineModelFromEAXMLFile(EAXMLFileName);
                    SelectedEAModel = EAModelsList?.FirstOrDefault();
                    break;
                case nameof(EAModelsList):
                    RaisePropertyChanged(nameof(IsModelSelectable));
                    break;
                case nameof(TargetSolutionFileName):
                    NamespacesList = GetNameSpaces(TargetSolutionFileName).ToList();
                    break;
            }
        }
        #endregion constructor

        #region GetNameSpaces
        private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

        private HashSet<string> GetNameSpaces(string targetSolution) {
            var solutionFileInfo = new FileInfo(targetSolution);
            if (solutionFileInfo == null) { throw new ArgumentException(nameof(targetSolution)); }
            var targetDir = solutionFileInfo.Directory;
            if (targetDir == null) { throw new ArgumentException(nameof(targetSolution)); }

            CsFiles = targetDir.EnumerateFiles("*.cs", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f.FullName)).ToList();

            var solutionName = TargetSolutionFileInfo.Name.Split('.')[0]; //name without extensions
            var namespaces = new HashSet<string>();
            namespaces.Add(solutionName);
            SelectedNameSpace = namespaces.First();
            Parallel.ForEach(CsFiles
                , new ParallelOptions { MaxDegreeOfParallelism = 3 }
                , body: f => {
                    var namespaceValue = GetNamepaceAsync(f);
                    _lock.EnterWriteLock();
                    try {
                        if (string.IsNullOrEmpty(namespaceValue) == false) {
                            namespaces.Add(namespaceValue);
                            if (namespaceValue == solutionName) { SelectedNameSpace = namespaceValue; }
                        }
                    }
                    finally { if (_lock.IsWriteLockHeld) { _lock.ExitWriteLock(); } }
                });

            return namespaces;
        }
        #endregion GetNameSpaces

        #region GetNamepaceAsync
        public const string CSharpExtension = ".cs";

        public static string GetNamepaceAsync(FileInfo csFile) {

            string namespaceResult = null;
            var lines = File.ReadAllLines(csFile.FullName);
            foreach (var line in lines) {
                namespaceResult = GetNamespaceValue(line);
                if (string.IsNullOrEmpty(namespaceResult) == false) {
                    return namespaceResult;
                }
            }

            return namespaceResult;
        }

        //public const string NamespacePatternLiteral = @"(?<=namespace )\b\w+(\.\w+)+\b";
        public const string NamespacePatternLiteral = @"(?<=namespace )\b\w+(\.\w+)*\b";
        public static string GetNamespaceValue(string line) {
            if (string.IsNullOrEmpty(line)) { return null; }

            //var namespacePattern = @"(?<=namespace )\b\w+\b";
            var match = Regex.Match(line, NamespacePatternLiteral);
            var asdf = match.Value;
            return match.Success ? match.Value : null;
        }
        #endregion GetNamepaceAsync

        public string[] GetGenFileNames(string selectedEAModel, FileInfo targetSolutionFileInfo, string namespaceValue) {
            if (string.IsNullOrEmpty(selectedEAModel)) { throw new ArgumentException(nameof(GetGenFileNames)); }
            if (targetSolutionFileInfo == null) { throw new ArgumentException(nameof(targetSolutionFileInfo)); }
            if (string.IsNullOrEmpty(namespaceValue)) { throw new ArgumentException(nameof(namespaceValue)); }
            var fileNames = new string[4];


            return fileNames;
        }

        #region commands
        public void OpenFileExplorer(object path) {

            if (string.IsNullOrEmpty(EAXMLFileName) == false && string.IsNullOrEmpty(TargetSolutionFileName)) {
                TargetSolutionFileName = TargetSolutionLiteral;
            }
            if (string.IsNullOrEmpty(EAXMLFileName)) { EAXMLFileName = EAXMLFilePathLiteral; }

        }

        public void StartParsing() {
            //Main.MainEntryPoint(new string[] { EAXMLFileName, TargetSolutionFileName });
            //var models = Main.MainModels;
            //var codeGenerator = new StateMachineGenerator();
            //codeGenerator.Generate(models.FirstOrDefault(), TargetSolutionFileName);
        }
        #endregion commands

    }
}
