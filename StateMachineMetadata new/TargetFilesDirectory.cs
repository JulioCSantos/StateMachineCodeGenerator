﻿using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.Common.Extensions;
using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StateMachineMetadata
{
    public class TargetFilesDirectory : SetPropertyBase
    {

        #region properties

        #region Input files

        #region EA XML Model

        #region EaXmlFileName
        public readonly string EaXmlFileNameLiteral = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        //public const string EaXmlFileNameLiteral = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\InputTestsFiles\LaserProcessing Model new test.xml";
        //public const string EaXmlFileNameLiteral = @"C:\Users\santosj25\source\repos\StateMachineCodeGenerator\InputTestsFiles\LaserProcessing Model new test.xml";
        //public const string EaXmlFileNameLiteral = @"C:\Users\santosj25\source\GenSysTest\MotionSolutions\LLSystemNew\Modeling\LLSystem State Machine Export.xml";
        private string _eaXmlFileName;
        public string EaXmlFileName {
            get => _eaXmlFileName;
            set {
                SetProperty(ref _eaXmlFileName, value);
                CursorHandler.Instance.AddBusyMember();
                EaXmlFileInfo = null; // reset cache
                EaXmlParsed = false;
                RaisePropertyChanged(nameof(EaXmlFileCueColor));
                if (EaXmlFileInfo?.Exists == true) {
                    try {
                        CursorHandler.Instance.AddBusyMember();
                        EaModelsList = Main.GetStateMachineModelFromEAXMLFile(EaXmlFileName);
                        EaXmlParsed = true;
                    }
                    catch (Exception) {
                        EaXmlParsed = false;
                        EaModelsList = null;
                        SelectedEaModel = null;
                        SolutionFileName = null;
                        CleanUpTargetFilesDirectory();
                        CursorHandler.Instance.RemoveBusyMember();
                        throw ;
                    }
                    EaXmlParsed = true;

                }
                else { EaModelsList = null; }
            }
        }
        #endregion EaXmlFileName

        #region EaXmlFileInfo
        public FileInfo _eaXmlFileInfo;
        public FileInfo EaXmlFileInfo {
            get {
                if (_eaXmlFileInfo != null) { return _eaXmlFileInfo; }

                if (string.IsNullOrEmpty(EaXmlFileName)) { return null; }

                try { EaXmlFileInfo = new FileInfo(EaXmlFileName); }
                catch (Exception) { return _eaXmlFileInfo = null; }

                return _eaXmlFileInfo;
            }
            protected set {
                SetProperty( ref _eaXmlFileInfo, value);
              }
        }
        #endregion EaXmlFileInfo

        #region EaXmlFileCueColor
        public string EaXmlFileCueColor {
            get {
                if (EaXmlFileInfo == null) { return "Black"; }

                return EaXmlFileInfo.Exists ? "LightGreen" : "Red";
            }
        }
        #endregion EaXmlFileCueColor

        #region EaModelsList
        private List<StateMachineMetadata.Model.MainModel> _eaModelsList;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public List<StateMachineMetadata.Model.MainModel> EaModelsList {
            get => _eaModelsList;
            protected set { SetProperty(ref _eaModelsList, value); 
                RaisePropertyChanged(nameof(IsModelSelectable));
                RaisePropertyChanged(nameof(SelectingEaModelVisibility));
                if (_eaModelsList?.Any() == true) { SelectedEaModel = EaModelsList.FirstOrDefault(); }
                else { SelectedEaModel = null; }
            }
        }
        #endregion EaModelsList

        #region EaXmlParsed
        private bool _eaXmlParsed;
        public bool EaXmlParsed {
            get => _eaXmlParsed;
            set => SetProperty(ref _eaXmlParsed, value);
        }
        #endregion EaXmlParsed

        #region IsModelSelectable
        public bool IsModelSelectable => (EaModelsList?.Count ?? 0) > 1;
        #endregion IsModelSelectable

        #region SelectingEaModelVisibility
        public string SelectingEaModelVisibility => (EaModelsList?.Any() == true) ? "Visible" : "Collapsed";
        #endregion SelectingEaModelVisibility

        #region SelectedEaModel
        private Model.MainModel _selectedEaModel;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Model.MainModel SelectedEaModel {
            get => _selectedEaModel;
            set {
                SetProperty(ref _selectedEaModel, value);
                SelectedEaModelName = SelectedEaModel?.Name; // reset cache
            }
        }
        #endregion SelectedEaModel

        #region SelectedEaModelName
        private string _selectedEaModelName;
        public string SelectedEaModelName {
            get => _selectedEaModelName ??= SelectedEaModel?.Name;
            set {
                // remove previous model name from Target Files' directory name
                if (string.IsNullOrEmpty(_selectedEaModelName) == false && TargetFilesDirectoryName?.EndsWith(_selectedEaModelName) == true) {
                    var targetDirectoryName = new DirectoryInfo(TargetFilesDirectoryName);
                    TargetFilesDirectoryName = targetDirectoryName?.Parent?.FullName;
                }

                SetProperty(ref _selectedEaModelName, value);
            }
        }

        #endregion SelectedEaModelName

        #endregion EA XML Model

        #region SolutionFileName & SolutionFileInfo

        #region SolutionFileName
        public readonly string TargetSolutionLiteral = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //public const string TargetSolutionLiteral = @"C:\Users\julio\Documents\Visual Studio 2019\Projects\MyCompanies\Corning\TemplateGrid\TemplateGrid.sln";
        //public const string TargetSolutionLiteral = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject\TestsSubject.sln";
        //public const string TargetSolutionLiteral = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";

        private string _solutionFileName;
        public string SolutionFileName {
            get => _solutionFileName;
            set {
                SetProperty(ref _solutionFileName, value);
                RaisePropertyChanged(nameof(SolutionFileInfo));
                RaisePropertyChanged(nameof(SolutionFileCueColor));
            }
        }
        #endregion SolutionFileName

        #region SolutionFileInfo
        public FileInfo SolutionFileInfo {
            get {
                if (string.IsNullOrEmpty(SolutionFileName)) { return null;}

                return new FileInfo(SolutionFileName);
            }
        }
        #endregion SolutionFileInfo

        #region SolutionFileCueColor
        public string SolutionFileCueColor {
            get {
                if (SolutionFileInfo == null) { return "Black"; }

                return SolutionFileInfo.Exists ? "LightGreen" : "Red";
            }
        }
        #endregion SolutionFileCueColor

        #endregion SolutionFileName & SolutionFileInfo

        #region TargetFilesDirectory & TargetFilesDirectoryPath

        #region TargetFilesDirectoryName
        private string _targetFilesDirectoryName;
        public string TargetFilesDirectoryName {
            get => _targetFilesDirectoryName;
            set {
                SetProperty(ref _targetFilesDirectoryName, value);
                RaisePropertyChanged(nameof(TargetFilesDirectoryInfo));
                RaisePropertyChanged(nameof(TargetFilesDirectoryCueColor));
                RaisePropertyChanged(nameof(TargetFilesOpacity));
            }
        }
        #endregion TargetFilesDirectoryName

        #region TargetFilesDirectoryPath
        public DirectoryInfo TargetFilesDirectoryInfo {
            get {
                if (string.IsNullOrEmpty(TargetFilesDirectoryName)) { return null; }

                try { return new DirectoryInfo(Path.GetFullPath(TargetFilesDirectoryName)); }
                catch { return null; }
            }
        }
        #endregion TargetFilesDirectoryPath

        #region TargetFilesDirectoryCueColor
        public string TargetFilesDirectoryCueColor {
            get {
                if (TargetFilesDirectoryInfo == null) { return "Black"; }

                return TargetFilesDirectoryInfo.Exists ? "LightGreen" : "Yellow";
            }
        }
        #endregion TargetFilesDirectoryCueColor

        #region TargetFilesOpacity
        public double TargetFilesOpacity => string.IsNullOrEmpty(TargetFilesDirectoryName) ? 0.7 : 1;
        #endregion TargetFilesOpacity

        #endregion TargetFilesDirectoryName & TargetFilesDirectoryPath

        //#region CsFiles
        //private List<FileInfo> _csFiles;
        //[JsonIgnore(Condition = JsonIgnoreCondition.Always)]

        //public List<FileInfo> CsFiles {
        //    get => _csFiles;
        //    protected set => SetProperty(ref _csFiles, value);
        //}
        //#endregion CsFiles

        #region NamespacesList
        private List<string> _namespacesList;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public List<string> NamespacesList {
            get => _namespacesList;
            protected set {
                SetProperty(ref _namespacesList, value);
                if (_namespacesList?.Any() == true) { SelectedNameSpace = _namespacesList.First(); }
                RaisePropertyChanged(nameof(NamespacesVisibility));
            }
        }
        #endregion NamespacesList

        #region NamespacesVisibility
        public string NamespacesVisibility => (NamespacesList?.Any() == true) ? "Visible" : "Collapsed";
        #endregion NamespacesVisibility

        #region SelectedNameSpace
        private string _selectedNameSpace;
        public string SelectedNameSpace {
            get => _selectedNameSpace;
            set => SetProperty(ref _selectedNameSpace, value);
        }
        #endregion SelectedNameSpace

        #endregion Input files

        #region generated files

        #region StateMachineBaseFileName & StateMachineBaseFileInfo

        #region StateMachineBaseFileName
        private string _stateMachineBaseFileName;
        public string StateMachineBaseFileName {
            get => _stateMachineBaseFileName;
            set {
                SetProperty(ref _stateMachineBaseFileName, value); 
                RaisePropertyChanged(nameof(StateMachineBaseFileName));
                RaisePropertyChanged(nameof(StateMachineBaseFileCueColor));
            }
        }
        #endregion StateMachineBaseFileName

        #region StateMachineBaseFileInfo
        public FileInfo StateMachineBaseFileInfo {
            get {
                try { return new FileInfo(StateMachineBaseFileName); }
                catch (Exception) { return null; }
            }
        }
        #endregion StateMachineBaseFileInfo

        #region StateMachineBaseFileCueColor
        public string StateMachineBaseFileCueColor {
            get {
                if (TargetFilesDirectoryInfo == null) { return "Black"; }
                if (StateMachineBaseFileInfo == null) { return "LightCoral"; }
                else { return "LightGreen"; }
            }
        }
        #endregion StateMachineBaseFileCueColor

        #endregion StateMachineBaseFileName & StateMachineBaseFileInfo

        #region StateMachineDerivedFileName & StateMachineDerivedFileInfo

        #region StateMachineDerivedFileName
        private string _stateMachineDerivedFileName;
        public string StateMachineDerivedFileName {
            get => _stateMachineDerivedFileName;
            set {
                SetProperty(ref _stateMachineDerivedFileName, value); 
                RaisePropertyChanged(nameof(StateMachineDerivedFileInfo));
                RaisePropertyChanged(nameof(StateMachineDerivedFileCueColor));
            }
        }
        #endregion StateMachineDerivedFileName

        #region StateMachineDerivedFileInfo
        public FileInfo StateMachineDerivedFileInfo {
            get {
                try { return new FileInfo(StateMachineDerivedFileName); }
                catch (Exception) { return null; }
            }
        }
        #endregion StateMachineDerivedFileInfo

        #region StateMachineDerivedFileCueColor
        public string StateMachineDerivedFileCueColor {
            get {
                if (TargetFilesDirectoryInfo == null) { return "Black"; }
                if (StateMachineDerivedFileInfo == null) { return "LightCoral"; }
                else { return "LightGreen"; }
            }
        }
        #endregion StateMachineDerivedFileCueColor

        #endregion StateMachineDerivedFileName & StateMachineDerivedFileInfo

        #region MainModelBaseFileName & MainModelBaseFileInfo

        #region MainModelBaseFileName
        private string _mainModelBaseFileName;
        public string MainModelBaseFileName {
            get => _mainModelBaseFileName;
            set {
                SetProperty(ref _mainModelBaseFileName, value);
                RaisePropertyChanged(nameof(MainModelBaseFileInfo));
                RaisePropertyChanged(nameof(MainModelBaseFileCueColor));
            }
        }
        #endregion MainModelBaseFileName

        #region MainModelBaseFileInfo
        public FileInfo MainModelBaseFileInfo {
            get {
                try { return new FileInfo(MainModelBaseFileName); }
                catch (Exception) { return null; }
            }
        }
        #endregion MainModelBaseFileInfo

        #region MainModelBaseFileCueColor
        public string MainModelBaseFileCueColor {
            get {
                if (TargetFilesDirectoryInfo == null) { return "Black"; }
                if (MainModelBaseFileInfo == null) { return "LightCoral"; }
                else { return "LightGreen"; }
            }
        }
        #endregion MainModelBaseFileCueColor

        #endregion MainModelBaseFileName  & MainModelBaseFileInfo

        #region MainModelDerivedFileName & MainModelDerivedFileInfo

        #region MainModelDerivedFileName
        private string _mainModelDerivedFileName;
        public string MainModelDerivedFileName {
            get => _mainModelDerivedFileName;
            set {
                SetProperty(ref _mainModelDerivedFileName, value);
                RaisePropertyChanged(nameof(MainModelDerivedFileInfo));
                RaisePropertyChanged(nameof(MainModelDerivedFileCueColor));
            }
        }
        #endregion MainModelDerivedFileName

        #region MainModelDerivedFileInfo
        public FileInfo MainModelDerivedFileInfo {
            get {
                try { return new FileInfo(MainModelDerivedFileName); }
                catch (Exception) { return null; }
            }
        }
        #endregion MainModelDerivedFileInfo

        #region MainModelDerivedFileCueColor
        public string MainModelDerivedFileCueColor {
            get {
                if (TargetFilesDirectoryInfo == null) { return "Black"; }
                if (MainModelDerivedFileInfo == null) { return "LightCoral"; }
                else { return "LightGreen"; }
            }
        }
        #endregion MainModelDerivedFileCueColor

        #endregion MainModelDerivedFileName & MainModelDerivedFileInfo

        #region GenrtdFileNamesFunc
        protected Func<string, string> GenrtdFileNamesFunc => new((sufix) => {
            if (TargetFilesDirectoryName == null) return @$"\C{SelectedEaModelName}{sufix}";

            return TargetFilesDirectoryName + @$"\C{SelectedEaModelName}{sufix}";
        });
        #endregion GenrtdFileNamesFunc

        #endregion generated files

        #region Clone
        public TargetFilesDirectory Clone {
            get {
                var clone = new TargetFilesDirectory();
                clone.EaXmlFileName = new string(EaXmlFileName);
                clone.SolutionFileName = new string(SolutionFileName);
                clone.SelectedEaModelName = new string(SelectedEaModelName);
                clone.SelectedNameSpace = new string(SelectedNameSpace);
                clone.TargetFilesDirectoryName = new string(TargetFilesDirectoryName);
                clone.StateMachineBaseFileName = new string(StateMachineBaseFileName);
                clone.StateMachineDerivedFileName = new string(StateMachineDerivedFileName);
                clone.MainModelBaseFileName = new string(MainModelBaseFileName);
                clone.MainModelDerivedFileName = new string(MainModelDerivedFileName);

                return clone;
            }
        }

        #endregion Clone

        public void CleanUpTargetFilesDirectory() {
            TargetFilesDirectoryName = null;
            StateMachineBaseFileName = null;
            StateMachineDerivedFileName = null;
            MainModelBaseFileName = null;
            MainModelDerivedFileName = null;
            SelectedEaModelName = null;
            SelectedNameSpace = null;
        }
        #endregion properties

        #region TargetPath enum
        public enum TargetPath
        {
            unkown,
            Solution,
            CodeGeneratedPath,
            StateMachineBaseFilePath,
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
                        return SolutionFileName;
                    case TargetPath.StateMachineBaseFilePath:
                        return StateMachineBaseFileName;
                    case TargetPath.StateMachineDerivedFilePath:
                        return StateMachineDerivedFileName;
                    case TargetPath.MainModelBaseFilePath:
                        return MainModelBaseFileName;
                    case TargetPath.MainModelDerivedFilePath:
                        return MainModelDerivedFileName;
                    case TargetPath.CodeGeneratedPath:
                        return TargetFilesDirectoryInfo.FullName;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fileKey), fileKey, null);
                }
            }
            set {
                switch (fileKey) {
                    case TargetPath.unkown:
                        break;
                    case TargetPath.Solution:
                        SolutionFileName = value;
                        break;
                    case TargetPath.StateMachineBaseFilePath:
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
                    case TargetPath.CodeGeneratedPath:
                        break;
                        default:
                        throw new ArgumentOutOfRangeException(nameof(fileKey), fileKey, null);
                }
            }
        }
        #endregion this index

        #region constructor
        public TargetFilesDirectory() {
            this.PropertyChanged += TargetFilesDirectory_PropertyChanged;
        }

        private async void TargetFilesDirectory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(EaXmlFileInfo):
                    //fuzzy logic that finds solution in the ancestors folder of the EA XML Model file
                    if (EaXmlFileInfo?.Exists == true &&
                        string.IsNullOrEmpty(SolutionFileName)) {
                        var solutionFiles = EaXmlFileInfo.Directory
                            .FindFirstFilesInAncestors("*.sln", 2);
                        if (solutionFiles.Any()) {
                            SolutionFileName = solutionFiles.First().FullName;
                        }
                    }
                    break;
                case nameof(SelectedEaModelName):
                    if (string.IsNullOrEmpty(SelectedEaModelName)) { break; }
                    StateMachineBaseFileName = StateMachineDerivedFileName =
                        MainModelBaseFileName = MainModelDerivedFileName = null;
                    SetTargetDirectoryName();
                    StateMachineBaseFileName = GenrtdFileNamesFunc("StateMachineBase_gen.cs");
                    StateMachineDerivedFileName = GenrtdFileNamesFunc("StateMachine.cs");
                    MainModelBaseFileName = GenrtdFileNamesFunc("ModelBase_gen.cs");
                    MainModelDerivedFileName = GenrtdFileNamesFunc("Model.cs");
                    NamespacesList = GetNameSpaces(SolutionFileInfo).ToList();
                    break;
                case nameof(TargetFilesDirectoryName):
                    RaisePropertyChanged(nameof(TargetFilesDirectoryInfo));
                    //Reset Target files names
                    StateMachineBaseFileName = GenrtdFileNamesFunc("StateMachineBase_gen.cs");
                    StateMachineDerivedFileName = GenrtdFileNamesFunc("StateMachine.cs");
                    MainModelBaseFileName = GenrtdFileNamesFunc("ModelBase_gen.cs");
                    MainModelDerivedFileName = GenrtdFileNamesFunc("Model.cs");
                    break;
                case nameof(SolutionFileName):
                    if (string.IsNullOrEmpty(SolutionFileName)) {
                        TargetFilesDirectoryName = null;
                        NamespacesList = new List<string>();
                    }
                    SetTargetDirectoryName();

                    NamespacesList = GetNameSpaces(SolutionFileInfo).ToList();
                    break;
            }
        }

        #endregion constructor

        #region methods

        #region SetTargetDirectoryName
        private void SetTargetDirectoryName() {
            if (SolutionFileInfo?.DirectoryName == null) { return; }
            if (SelectedEaModelName == null) {
                TargetFilesDirectoryName = Path.GetDirectoryName(SolutionFileName);
                TargetFilesDirectoryName = SolutionFileInfo.DirectoryName;
                TargetFilesDirectoryName = Path.Combine(TargetFilesDirectoryName, @"Main\Model\MainModel");
            }
            else {
                TargetFilesDirectoryName = Path.Combine(SolutionFileInfo.DirectoryName, @"Main\Model\MainModel");
            }
        }
        #endregion SetTargetDirectoryName

        #region GetNameSpaces
        protected HashSet<string> GetNameSpaces(FileInfo solutionFileInfo) {
            var namespaces = new HashSet<string>();
            if (string.IsNullOrEmpty(SelectedEaModelName) == false) {namespaces.Add(SelectedEaModelName + ".Model");}
            if (TargetFilesDirectoryInfo != null) {
                var name2 = TargetFilesDirectoryInfo.Name;
                var name1 = TargetFilesDirectoryInfo.Parent?.Name;
                namespaces.Add(name1 + "." + name2);
            }

            return namespaces;
        }
        #endregion GetNameSpaces

        //#region GetNameSpaces
        //private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

        //protected HashSet<string> GetNameSpaces(FileInfo solutionFileInfo) { 
        //    if (solutionFileInfo?.Exists != true) { return new HashSet<string>(); }

        //    //var solutionFileInfo = new FileInfo(targetSolution); //TODO replace with TargetSolutionFileInfo
        //    //if (solutionFileInfo == null) { throw new ArgumentException(nameof(targetSolution)); }
        //    var targetDir = solutionFileInfo.Directory;
        //    if (targetDir == null) { return new HashSet<string>(); }

        //    CsFiles = targetDir.EnumerateFiles("*.cs", SearchOption.AllDirectories)
        //        .Select(f => new FileInfo(f.FullName)).ToList();

        //    var solutionName = SolutionFileInfo.Name.Split('.')[0]; //name without extensions
        //    var namespaces = new HashSet<string>();
        //    namespaces.Add(solutionName);
        //    SelectedNameSpace = namespaces.First();
        //    Parallel.ForEach(CsFiles
        //        , new ParallelOptions { MaxDegreeOfParallelism = 3 }
        //        , body: f => {
        //            var namespaceValue = GetNameSpaceAsync(f);
        //            _lock.EnterWriteLock();
        //            try {
        //                if (string.IsNullOrEmpty(namespaceValue) == false) {
        //                    namespaces.Add(namespaceValue);
        //                    if (namespaceValue == solutionName) { SelectedNameSpace = namespaceValue; }
        //                }
        //            }
        //            finally { if (_lock.IsWriteLockHeld) { _lock.ExitWriteLock(); } }
        //        });

        //    return namespaces;
        //}
        //#endregion GetNameSpaces

        //#region GetNameSpaceAsync
        //public const string CSharpExtension = ".cs";

        //public static string GetNameSpaceAsync(FileInfo csFile) {

        //    string namespaceResult = null;
        //    var lines = File.ReadAllLines(csFile.FullName);
        //    foreach (var line in lines) {
        //        namespaceResult = GetNamespaceValue(line);
        //        if (string.IsNullOrEmpty(namespaceResult) == false) {
        //            return namespaceResult;
        //        }
        //    }

        //    return namespaceResult;
        //}

        ////public const string NamespacePatternLiteral = @"(?<=namespace )\b\w+(\.\w+)+\b";
        //public const string NamespacePatternLiteral = @"(?<=namespace )\b\w+(\.\w+)*\b";
        //public static string GetNamespaceValue(string line) {
        //    if (string.IsNullOrEmpty(line)) { return null; }

        //    //var namespacePattern = @"(?<=namespace )\b\w+\b";
        //    var match = Regex.Match(line, NamespacePatternLiteral);
        //    return match.Success ? match.Value : null;
        //}
        //#endregion GetNameSpaceAsync

        #region GetMetadataTargetPaths
        public Dictionary<StateMachineMetadata.TargetPath, string> GetMetadataTargetPaths() {
            var targetPaths = new Dictionary<StateMachineMetadata.TargetPath, string>();
            targetPaths[StateMachineMetadata.TargetPath.Solution] = this[TargetPath.Solution];
            targetPaths[StateMachineMetadata.TargetPath.CodeGeneratedPath] = this[TargetPath.CodeGeneratedPath];
            targetPaths[StateMachineMetadata.TargetPath.StateMachineBaseFilePath] = this[TargetPath.StateMachineBaseFilePath];
            targetPaths[StateMachineMetadata.TargetPath.StateMachineDerivedFilePath] = this[TargetPath.StateMachineDerivedFilePath];
            targetPaths[StateMachineMetadata.TargetPath.MainModelBaseFilePath] = this[TargetPath.MainModelBaseFilePath];
            targetPaths[StateMachineMetadata.TargetPath.MainModelDerivedFilePath] = this[TargetPath.MainModelDerivedFilePath];
            targetPaths[StateMachineMetadata.TargetPath.NameSpace] = SelectedNameSpace ?? SelectedEaModelName;

            return targetPaths;
        }
        #endregion GetMetadataTargetPaths

        #endregion methods
    }
}
