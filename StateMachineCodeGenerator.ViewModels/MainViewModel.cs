using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.Generator;
using StateMachineCodeGeneratorSystem.Templates;
using StateMachineMetadata;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class MainViewModel : SetPropertyBase
    {
        #region properties

        #region file labels properties
        public string StateMachineBaseFileLbl => "State Machine base file";
        public string StateMachineDerivedFileLbl => "State Machine derived file";
        public string MainModelBaseFileLbl => "Main Model base file";
        public string MainModelDerivedFileLbl => "Main Model derived file";
        #endregion file labels properties

        #region TargetFilesDirectory
        private TargetFilesDirectory _targetFilesDirectory;
        public TargetFilesDirectory TargetFilesDirectory {
            get => _targetFilesDirectory ??= TargetFilesDirectory.Instance;
            set => SetProperty(ref _targetFilesDirectory, value);
        }
        #endregion TargetFilesDirectory

        #endregion properties


        #region constructor
        public MainViewModel() {
            //this.PropertyChanged += MainViewModel_PropertyChanged;
            this.TargetFilesDirectory.PropertyChanged += TargetFilesDirectory_PropertyChanged;
        }

        private void TargetFilesDirectory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(TargetFilesDirectory.EaXmlFileName):
                    TargetFilesDirectory.EaModelsList = Main.GetStateMachineModelFromEAXMLFile(TargetFilesDirectory.EaXmlFileName);
                    TargetFilesDirectory.SelectedEaModel = TargetFilesDirectory.EaModelsList?.FirstOrDefault();
                    break;
            }
        }

        #endregion constructor

        #region commands

        public RelayCommand StartParsingCommand => new RelayCommand((o) => StartParsing());
        public RelayCommand LocateEaXmlFileCommand => new RelayCommand(LocateEaXmlFile);
        public RelayCommand LocateSolutionFileCommand => new RelayCommand(LocateSolutionFile);
        public RelayCommand LocateTargetFolderCommand => new RelayCommand(LocateTargetFolder);
        public RelayCommand LocateCSharpFilesCommand => new RelayCommand(LocateCSharpFiles);

        public const string EnterpriseArchitectFilterLiteral = "EA files (*.xml)|*.xml|All files (*.*)|*.*";

        public void LocateEaXmlFile(object path) {

            IPopupView view = DialogServices.Instance.Dialogs[nameof(LocateFileViewModel)];
            var vm = view.Vm as LocateFileViewModel;
            if (vm == null) throw new Exception(); // will not happen
            var fileLocated = vm.ShowDialog(TargetFilesDirectory.EaXmlFileNameLiteral
                , EnterpriseArchitectFilterLiteral) ?? view.Vm.ClosingResult;
            if (fileLocated == true) {
                TargetFilesDirectory.EaXmlFileName = vm.LocatedFileName;
            }
        }

        public const string SolutionFilterLiteral = "solution files (*.sln)|*.sln|All files (*.*)|*.*";
        public void LocateSolutionFile(object path) {
            IPopupView view = DialogServices.Instance.Dialogs[nameof(LocateFileViewModel)];
            var vm = view.Vm as LocateFileViewModel;
            if (vm == null) throw new Exception(); // will not happen
            var fileLocated = vm.ShowDialog(TargetFilesDirectory.TargetSolutionLiteral
                , SolutionFilterLiteral) ?? view.Vm.ClosingResult;
            if (fileLocated == true) {
                TargetFilesDirectory.SolutionFileName = vm.LocatedFileName;
            }
        }

        public void LocateTargetFolder(object path) {
            IPopupView view = DialogServices.Instance.Dialogs[nameof(LocateFolderViewModel)];
            var vm = view.Vm as LocateFolderViewModel;
            if (vm == null) throw new Exception(); // will not happen
            var fileLocated = vm.ShowDialog(TargetFilesDirectory.TargetFilesDirectoryName) ?? view.Vm.ClosingResult;
            if (fileLocated == true) {
                TargetFilesDirectory.TargetFilesDirectoryName = vm.LocatedFolderName;
            }
        }

        public const string CSharpFilterLiteral = "C# files (*.cs)|*.cs|All files (*.*)|*.*";
        public void LocateCSharpFiles(object fileLabel) {
            IPopupView view = DialogServices.Instance.Dialogs[nameof(LocateFileViewModel)];
            var vm = view.Vm as LocateFileViewModel;
            if (vm == null) throw new Exception(); // will not happen
            var fileLocated = vm.ShowDialog(TargetFilesDirectory[GetFileIx(fileLabel.ToString())]
                , CSharpFilterLiteral) ?? view.Vm.ClosingResult;
            if (fileLocated == true) {
                TargetFilesDirectory[GetFileIx(fileLabel.ToString())] = vm.LocatedFileName;
            }
        }

        public TargetFilesDirectory.TargetPath GetFileIx(string fileLabel) {
            TargetFilesDirectory.TargetPath fileIx = TargetFilesDirectory.TargetPath.unkown;
            if (fileLabel == StateMachineBaseFileLbl) { fileIx = TargetFilesDirectory.TargetPath.StateMachineBaseFilePath; }
            if (fileLabel == StateMachineDerivedFileLbl) { fileIx = TargetFilesDirectory.TargetPath.StateMachineDerivedFilePath; }
            if (fileLabel == MainModelBaseFileLbl) { fileIx = TargetFilesDirectory.TargetPath.MainModelBaseFilePath; }
            if (fileLabel == MainModelDerivedFileLbl) { fileIx = TargetFilesDirectory.TargetPath.MainModelDerivedFilePath; }

            return fileIx;
        }

        public async void StartParsing() {
            CursorHandler.Instance.AddBusyMember();
            var CodeGenerator = new TemplatesGenerator(TargetFilesDirectory.EaXmlFileInfo);
            //var filesGenerated = await CodeGenerator.GenerateFiles();
            var filesGenerated = await CodeGenerator.GenerateFiles(TargetFilesDirectory.GetMetadataTargetPaths());
            CursorHandler.Instance.RemoveBusyMember();
        }
        #endregion commands

    }
}
