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

        #region TargetFilesDirectory
        private TargetFilesDirectory _targetFilesDirectory;
        public TargetFilesDirectory TargetFilesDirectory {
            get => _targetFilesDirectory ??= TargetFilesDirectory.Instance;
            set => SetProperty(ref _targetFilesDirectory, value);
        }
        #endregion TargetFilesDirectory

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
