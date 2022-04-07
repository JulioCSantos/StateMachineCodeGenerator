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
        //#region singleton
        //public static TargetFilesDirectory Instance { get; } = new TargetFilesDirectory();

        //public static TargetFilesDirectory GetInstance() { return Instance; }

        #region constructor
        public TargetFilesDirectory() {
            this.PropertyChanged += TargetFilesDirectory_PropertyChanged;
        }

        private void TargetFilesDirectory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(EAModelName):
                case nameof(TargetFilesPath):
                case nameof(NamespaceValue):

                    break;
            }
        }
        #endregion constructor

        //#endregion singleton

        #region SolutionFileInfo
        private FileInfo _solutionFileInfo;
        public FileInfo SolutionFileInfo {
            get => _solutionFileInfo;
            set => SetProperty(ref _solutionFileInfo, value);
        }
        #endregion SolutionFileInfo

        #region StateMachineBaseFileInfo
        private FileInfo _stateMachineBaseFileInfo;
        public FileInfo StateMachineBaseFileInfo {
            get => _stateMachineBaseFileInfo ??= new FileInfo(TargetFilesPath.FullName + $".C{EAModelName}StateMachineBase_gen.cs");
            set => SetProperty(ref _stateMachineBaseFileInfo, value);
        }
        #endregion StateMachineBaseFileInfo

        #region StateMachineDerivedFileInfo
        private FileInfo _stateMachineDerivedFileInfo;
        public FileInfo StateMachineDerivedFileInfo {
            get => _stateMachineDerivedFileInfo ??= new FileInfo(TargetFilesPath.FullName + $".C{EAModelName}StateMachineBase.cs");
            set => SetProperty(ref _stateMachineDerivedFileInfo, value);
        }
        #endregion StateMachineDerivedFileInfo

        #region MainModelBaseFileInfo
        private FileInfo _mainModelBaseFileInfo;
        public FileInfo MainModelBaseFileInfo {
            get => _mainModelBaseFileInfo ??= new FileInfo(TargetFilesPath.FullName + $".C{EAModelName}ModelBase_gen.cs");
            set => SetProperty(ref _mainModelBaseFileInfo, value);
        }
        #endregion MainModelBaseFileInfo

        #region MainModelDerivedFileInfo
        private FileInfo _mainModelDerivedFileInfo;
        public FileInfo MainModelDerivedFileInfo {
            get => _mainModelDerivedFileInfo ??= new FileInfo(TargetFilesPath.FullName + $".C{EAModelName}Model.cs");
            set => SetProperty(ref _mainModelDerivedFileInfo, value);
        }
        #endregion MainModelDerivedFileInfo



        #region TargetFilesPath
        private DirectoryInfo _targetFilesPath;
        public DirectoryInfo TargetFilesPath {
            get => _targetFilesPath;
            set => SetProperty(ref _targetFilesPath, value);
        }
        #endregion TargetFilesPath

        #region EAModelName
        private string _eAModelName;
        public string EAModelName {
            get => _eAModelName;
            set => SetProperty(ref _eAModelName, value);
        }
        #endregion EAModelName

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
            StateMachineBaseFilePath,
            StateMachineDerivedFilePath,
            MainModelBaseFilePath,
            MainModelDerivedFilePath
        }

        public FileInfo this[TargetPath fileKey] {
            get {
                switch (fileKey) {
                    case TargetPath.unkown:
                        return null;
                    case TargetPath.Solution:
                        return SolutionFileInfo;
                    case TargetPath.StateMachineBaseFilePath:
                        return StateMachineBaseFileInfo;
                    case TargetPath.StateMachineDerivedFilePath:
                        return StateMachineDerivedFileInfo;
                    case TargetPath.MainModelBaseFilePath:
                        return MainModelBaseFileInfo;
                    case TargetPath.MainModelDerivedFilePath:
                        return MainModelDerivedFileInfo;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fileKey), fileKey, null);
                }
            }
            set {
                switch (fileKey) {
                    case TargetPath.unkown:
                        break;
                    case TargetPath.Solution:
                        SolutionFileInfo = value;
                        break;
                    case TargetPath.StateMachineBaseFilePath:
                        StateMachineBaseFileInfo = value;
                        break;
                    case TargetPath.StateMachineDerivedFilePath:
                        StateMachineDerivedFileInfo = value;
                        break;
                    case TargetPath.MainModelBaseFilePath:
                        MainModelBaseFileInfo = value;
                        break;
                    case TargetPath.MainModelDerivedFilePath:
                        MainModelDerivedFileInfo = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fileKey), fileKey, null);
                }
            }
        }
    }
}
