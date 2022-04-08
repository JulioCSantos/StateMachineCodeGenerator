using StateMachineCodeGenerator.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                case nameof(EAModelName):
                case nameof(TargetFilesPath):
                    if (string.IsNullOrEmpty(EAModelName) || TargetFilesPath == null) {}
                    else {
                        RaisePropertyChanged(nameof(StateMachineBaseFileName));
                        RaisePropertyChanged(nameof(StateMachineDerivedFileName));
                        RaisePropertyChanged(nameof(MainModelBaseFileName));
                        RaisePropertyChanged(nameof(MainModelDerivedFileName));
                    }
                    break;
            }
        }
        #endregion constructor

        #endregion singleton

        protected Func<string, string> GenrtdFileNamesFunc => new((sufix) => {
            if (string.IsNullOrEmpty(EAModelName) || TargetFilesPath?.FullName == null) { return null;}
            return TargetFilesPath.FullName + @$"\C{EAModelName}{sufix}";
        });

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
            get => _stateMachineDerivedFileName ??= GenrtdFileNamesFunc("StateMachineBase.cs");
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
            get => _mainModelDerivedFileName ??= GenrtdFileNamesFunc("EAModelName}Model.cs");
            set => SetProperty(ref _mainModelDerivedFileName, value);
        }
        #endregion MainModelDerivedFileName



        #region SolutionFileName
        private string _solutionFileName;
        public string SolutionFileName {
            get => _solutionFileName;
            set { SetProperty(ref _solutionFileName, value); RaisePropertyChanged(nameof(SolutionFileInfo)); }
        }

        public FileInfo SolutionFileInfo {
            get {
                try { return new FileInfo(SolutionFileName); }
                catch (Exception) { return null; }
            }
        }
        #endregion SolutionFileName

        #region EAModelName
        private string _eAModelName;
        public string EAModelName {
            get => _eAModelName;
            set => SetProperty(ref _eAModelName, value);
        }
        #endregion EAModelName

        #region TargetFilesPath
        private DirectoryInfo _targetFilesPath;
        public DirectoryInfo TargetFilesPath {
            get => _targetFilesPath;
            set => SetProperty(ref _targetFilesPath, value);
        }
        #endregion TargetFilesPath

        #region NamespaceValue
        private string _namespaceValue;
        public string NamespaceValue {
            get => _namespaceValue;
            set => SetProperty(ref _namespaceValue, value);
        }
        #endregion NamespaceValue

        public enum TargetPath
        {
            unkown,
            Solution,
            StateMachineBaseFileName,
            StateMachineDerivedFilePath,
            MainModelBaseFilePath,
            MainModelDerivedFilePath
        }

        public string this[TargetPath fileKey] {
            get {
                switch (fileKey) {
                    case TargetPath.unkown:
                        return null;
                    case TargetPath.Solution:
                        return SolutionFileName;
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
                        SolutionFileName = value;
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
    }
}
