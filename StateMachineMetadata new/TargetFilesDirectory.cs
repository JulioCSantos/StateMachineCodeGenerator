using StateMachineCodeGenerator.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StateMachineMetadata
{
    public class TargetFilesDirectory : SetPropertyBase
    {
        #region singleton
        public static TargetFilesDirectory Instance { get; } = new TargetFilesDirectory();

        public static TargetFilesDirectory GetInstance() { return Instance; }

        #region constructor
        protected TargetFilesDirectory() {
            this.PropertyChanged += TargetFilesDirectory_PropertyChanged;
        }

        private void TargetFilesDirectory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(SelectedEaModel):
                case nameof(SelectedEaModelName):
                case nameof(TargetFilesPath):
                    if (string.IsNullOrEmpty(SelectedEaModelName) || TargetFilesPath == null) {}
                    else {
                        //NamespacesList = GetNameSpaces(TargetSolutionFileName).ToList();

                        RaisePropertyChanged(nameof(StateMachineBaseFileName));
                        RaisePropertyChanged(nameof(StateMachineDerivedFileName));
                        RaisePropertyChanged(nameof(MainModelBaseFileName));
                        RaisePropertyChanged(nameof(MainModelDerivedFileName));
                    }
                    break;
                case nameof(TargetSolutionFileName):
                    if (TargetSolutionFileInfo == null) { break;}

                    TargetFilesPath = null; // reset cache
                    NamespacesList = GetNameSpaces(TargetSolutionFileName).ToList();
                    RaisePropertyChanged(nameof(TargetFilesPath));
                    break;
            }
        }
        #endregion constructor

        #endregion singleton

        #region GenrtdFileNamesFunc
        protected Func<string, string> GenrtdFileNamesFunc => new((sufix) => {
            if (string.IsNullOrEmpty(SelectedEaModelName) || TargetFilesPath?.FullName == null) { return null;}
            return TargetFilesPath.FullName + @$"\C{SelectedEaModelName}{sufix}";
        });
        #endregion GenrtdFileNamesFunc

        #region StateMachineBaseFileName
        private string _stateMachineBaseFileName;
        public string StateMachineBaseFileName {
            get => _stateMachineBaseFileName ??= GenrtdFileNamesFunc("StateMachineBase_gen.cs");
            set { SetProperty(ref _stateMachineBaseFileName, value); RaisePropertyChanged(nameof(StateMachineBaseFileName));}
        }
        public FileInfo StateMachineBaseFileInfo {
            get {
                try { return new FileInfo(StateMachineBaseFileName); }
                catch (Exception) { return null; }
            }
        }
        #endregion StateMachineBaseFileName

        #region StateMachineDerivedFileName
        private string _stateMachineDerivedFileName;
        public string StateMachineDerivedFileName {
            get => _stateMachineDerivedFileName ??= GenrtdFileNamesFunc("StateMachine.cs");
            set { SetProperty(ref _stateMachineDerivedFileName, value); RaisePropertyChanged(nameof(StateMachineDerivedFileInfo)); }
        }
        public FileInfo StateMachineDerivedFileInfo {
            get {
                try { return new FileInfo(StateMachineDerivedFileName); }
                catch (Exception) { return null; }
            }
        }
        #endregion StateMachineDerivedFileName

        #region MainModelBaseFileName
        private string _mainModelBaseFileName;
        public string MainModelBaseFileName {
            get => _mainModelBaseFileName ??= GenrtdFileNamesFunc("ModelBase_gen.cs");
            set => SetProperty(ref _mainModelBaseFileName, value);
        }
        #endregion MainModelBaseFileName

        #region MainModelDerivedFileName
        private string _mainModelDerivedFileName;
        public string MainModelDerivedFileName {
            get => _mainModelDerivedFileName ??= GenrtdFileNamesFunc("Model.cs");
            set => SetProperty(ref _mainModelDerivedFileName, value);
        }
        #endregion MainModelDerivedFileName


        #region EA XML Model

        #region EaXmlFileName
        public const string EaXmlFilePathLiteral = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata new\Dependencies\LaserProcessing Model new.xml";
        //public const string EAXMLFilePathLiteral = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata\Dependencies\LaserProcessing Model new.xml";
        //private string _eaxmlFileName = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata\Dependencies\LaserProcessing Model new.xml";
        private string _eaXmlFileName;
        public string EaXmlFileName {
            get => _eaXmlFileName;
            set { SetProperty(ref _eaXmlFileName, value); RaisePropertyChanged(nameof(EaXmlFileInfo));  }
        }
        #endregion EaXmlFileName

        #region EaXmlFileInfo
        public FileInfo EaXmlFileInfo {
            get {
                try { return new FileInfo(EaXmlFileName); }
                catch (Exception) { return null; }
            }
        }
        #endregion EaXmlFileInfo
        
        #region EaModelsList
        private List<StateMachineMetadata.Model.MainModel> _eaModelsList;
        public List<StateMachineMetadata.Model.MainModel> EaModelsList {
            get => _eaModelsList;
            set { SetProperty(ref _eaModelsList, value); RaisePropertyChanged(nameof(IsModelSelectable)); }
        }
        #endregion EaModelsList

        #region IsModelSelectable
        public bool IsModelSelectable => (EaModelsList?.Count ?? 0) > 1;
        #endregion IsModelSelectable

        #region SelectedEaModel
        private Model.MainModel _selectedEaModel;
        public Model.MainModel SelectedEaModel {
            get => _selectedEaModel;
            set => SetProperty(ref _selectedEaModel, value);
        }
        #endregion SelectedEaModel

        #region SelectedEaModelName
        private string _selectedEaModelName;
        public string SelectedEaModelName {
            get => _selectedEaModelName ??= SelectedEaModel?.Name;
            protected set => SetProperty(ref _selectedEaModelName, value);
        }
        #endregion SelectedEaModelName

        #endregion EA XML Model

        #region Targetted Solution

        #region TargetFilesPath
        private DirectoryInfo _targetFilesPath;
        public DirectoryInfo TargetFilesPath {
            get {
                if (_targetFilesPath != null) { return _targetFilesPath;}
                try { return new DirectoryInfo(new FileInfo(TargetSolutionFileName).Directory?.FullName + @"\" + SelectedEaModelName) ; }
                catch (Exception) { return null; }
            }
            set => SetProperty(ref _targetFilesPath, value);
        }

        #endregion TargetFilesPath

        #region SolutionFileName. TBD if it is needed
        public const string TargetSolutionLiteral = @"C:\Users\julio\Documents\Visual Studio 2019\Projects\MyCompanies\Corning\TemplateGrid\TemplateGrid.sln";
        //public const string TargetSolutionLiteral = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";
        //public const string TargetSolutionLiteral = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";

        private string _targetSolutionFileName;
        public string TargetSolutionFileName {
            get => _targetSolutionFileName;
            set { SetProperty(ref _targetSolutionFileName, value); RaisePropertyChanged(nameof(TargetSolutionFileInfo)); }
        }

        public FileInfo TargetSolutionFileInfo {
            get {
                try { return new FileInfo(TargetSolutionFileName); }
                catch (Exception) { return null; }
            }
        }
        #endregion SolutionFileName. TBD if it is needed

        #region CsFiles
        private List<FileInfo> _csFiles;
        public List<FileInfo> CsFiles {
            get => _csFiles;
            set => SetProperty(ref _csFiles, value);
        }
        #endregion CsFiles

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

        #endregion Targetted Solution


        #region TargetPath enum
        public enum TargetPath
        {
            unkown,
            Solution,
            StateMachineBaseFileName,
            StateMachineDerivedFilePath,
            MainModelBaseFilePath,
            MainModelDerivedFilePath
        }
        #endregion TargetPath enum

        #region this index
        public string this[TargetPath fileKey] {
            get {
                switch (fileKey) {
                    case TargetPath.unkown:
                        return null;
                    case TargetPath.Solution:
                        return TargetSolutionFileName;
                    case TargetPath.StateMachineBaseFileName:
                        return StateMachineBaseFileName;
                    case TargetPath.StateMachineDerivedFilePath:
                        return StateMachineDerivedFileName;
                    case TargetPath.MainModelBaseFilePath:
                        return MainModelBaseFileName;
                    case TargetPath.MainModelDerivedFilePath:
                        return MainModelDerivedFileName;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fileKey), fileKey, null);
                }
            }
            set {
                switch (fileKey) {
                    case TargetPath.unkown:
                        break;
                    case TargetPath.Solution:
                        TargetSolutionFileName = value;
                        break;
                    case TargetPath.StateMachineBaseFileName:
                        StateMachineBaseFileName = value;
                        break;
                    case TargetPath.StateMachineDerivedFilePath:
                        StateMachineDerivedFileName = value;
                        break;
                    case TargetPath.MainModelBaseFilePath:
                        MainModelBaseFileName = value;
                        break;
                    case TargetPath.MainModelDerivedFilePath:
                        MainModelDerivedFileName = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fileKey), fileKey, null);
                }
            }
        }
        #endregion this index

        #region methods

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
            return match.Success ? match.Value : null;
        }
        #endregion GetNamepaceAsync

        #endregion methods
    }
}
