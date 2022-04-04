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
        public RelayCommand StartParsingCommand => new RelayCommand((o) => StartParsing());
        public RelayCommand OpenFileExplorerCommand => new RelayCommand(OpenFileExplorer);

        #region EAXMLFilePath
        public const string EAXMLFilePathLiteral = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata new\Dependencies\LaserProcessing Model new.xml";
        //public const string EAXMLFilePathLiteral = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata\Dependencies\LaserProcessing Model new.xml";
        //private string _eaxmlFilePath = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata\Dependencies\LaserProcessing Model new.xml";
        private string _eaxmlFilePath;
        public string EAXMLFilePath {
            get => _eaxmlFilePath;
            set => SetProperty(ref _eaxmlFilePath, value);
        }
        #endregion EAXMLFilePath

        #region TargetSolution
        public const string TargetSolutionLiteral = @"C:\Users\julio\Documents\Visual Studio 2019\Projects\MyCompanies\Corning\TemplateGrid\TemplateGrid.sln";
        //public const string TargetSolutionLiteral = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";
        //public const string TargetSolutionLiteral = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";
        private string _targetSolution;
        public string TargetSolution {
            get => _targetSolution;
            set => SetProperty(ref _targetSolution, value);
        }
        #endregion TargetSolution

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

        public bool IsModelSelectable => (EAModelsList?.Count ?? 0) > 1;

        #region constructor
        public MainViewModel() {
            this.PropertyChanged += MainViewModel_PropertyChanged;
            //RaisePropertyChanged(nameof(EAXMLFilePath));
            //RaisePropertyChanged(nameof(TargetSolution));
        }

        private async void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(EAXMLFilePath):
                    EAModelsList = Main.GetStateMachineModelFromEAXMLFile(EAXMLFilePath);
                    SelectedEAModel = EAModelsList?.FirstOrDefault();
                    break;
                case nameof(EAModelsList):
                    RaisePropertyChanged(nameof(IsModelSelectable));
                    break;
                case nameof(TargetSolution):
                    NamespacesList = (await GetNameSpaces(TargetSolution)).ToList();
                    break;

            }

        }

        private readonly ReaderWriterLockSlim _lock = new (LockRecursionPolicy.SupportsRecursion);


        private async Task<HashSet<string>> GetNameSpaces(string targetSolution) {
            var solutionFileInfo = new FileInfo(targetSolution);
            if (solutionFileInfo == null) {throw new ArgumentException(nameof(targetSolution));}
            var targetDir = solutionFileInfo.Directory;
            if (targetDir == null) { throw new ArgumentException(nameof(targetSolution)); }

            CsFiles = targetDir.EnumerateFiles("*.cs", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f.FullName)).ToList();

            //var timer = new Stopwatch(); timer.Start();
            //var namespaces = new HashSet<string>();
            //foreach (FileInfo file in csFiles) {
            //    var namespaceValue = GetNamepaceAsync(file).Result;
            //    if (string.IsNullOrEmpty(namespaceValue) == false) { namespaces.Add(namespaceValue);}
            //}
            //Debug.WriteLine("====> " + timer.Elapsed);

            var namespaces = new HashSet<string>();
            Parallel.ForEach(CsFiles
                , new ParallelOptions { MaxDegreeOfParallelism = 3 }
                , async f => {
                    var  namespaceValue =  await GetNamepaceAsync(f);
                    _lock.EnterWriteLock();
                    try { if (string.IsNullOrEmpty(namespaceValue) == false) { namespaces.Add(namespaceValue); } }
                    finally{ if (_lock.IsWriteLockHeld) {_lock.ExitWriteLock();} }
                });

            return namespaces;
        }
        #endregion constructor

        public const string CSharpExtension = ".cs";

        public async Task<string> GetNamepaceAsync(FileInfo csFile) {

            string namespaceResult = null;
            //var lines = await File.ReadAllLinesAsync(csFile.FullName, CancellationToken.None);
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
        //public string GetNamespaceValue(string line) {
        //    if (string.IsNullOrEmpty(line)) { return null;}

        //    var namespaceSplits = line.Split(NamespaceLiteral, StringSplitOptions.RemoveEmptyEntries);
        //    if (namespaceSplits.Any() == false || namespaceSplits[0] != NamespaceLiteral) { return null; }

        //    var namespaceStart = namespaceSplits[^1].TrimStart();
        //    var namespaceValue = namespaceStart.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        //    return namespaceValue[0];
        //}

        //public MainModel GetEAModelsList(string EA_XML_FilePath) {
        //    EAModelsList = StateMachineMetadata.Main.GetStateMachineModelFromEAXMLFile(EA_XML_FilePath);
        //    return EAModelsList?.FirstOrDefault();
        //}

        public void OpenFileExplorer(object path) {

            if (string.IsNullOrEmpty(EAXMLFilePath) == false && string.IsNullOrEmpty(TargetSolution)) {
                TargetSolution = TargetSolutionLiteral;
            }
            if (string.IsNullOrEmpty(EAXMLFilePath)) { EAXMLFilePath = EAXMLFilePathLiteral; }

        }

        public void StartParsing() {
            //Main.MainEntryPoint(new string[] { EAXMLFilePath, TargetSolution });
            //var models = Main.MainModels;
            //var codeGenerator = new StateMachineGenerator();
            //codeGenerator.Generate(models.FirstOrDefault(), TargetSolution);
        }

    }
}
