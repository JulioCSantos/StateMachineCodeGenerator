using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.Generator;
using StateMachineCodeGeneratorSystem.Templates;
using StateMachineMetadata;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class MainViewModel : SetPropertyBase
    {
        #region Commands
 
        #endregion Commands

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
        public RelayCommand OpenFileExplorerCommand => new RelayCommand(OpenFileExplorer);

        public void OpenFileExplorer(object path) {

            IPopupView view = DialogServices.Instance.Dialogs[nameof(LocateFileViewModel)];
            var result = view.Vm.ShowDialog() ?? view.Vm.ClosingResult;
            var dialogResult = result?.ToString() ?? "null";
            //if (result == false) /* If Clear Button pressed set all columns to visible */ {
            //    System.Diagnostics.Debugger.Break();
            //}

            if (string.IsNullOrEmpty(TargetFilesDirectory.EaXmlFileName) == false && string.IsNullOrEmpty(TargetFilesDirectory.SolutionFileName)) {
                TargetFilesDirectory.SolutionFileName = TargetFilesDirectory.TargetSolutionLiteral;
            }

            if (string.IsNullOrEmpty(TargetFilesDirectory.EaXmlFileName)) {
                TargetFilesDirectory.EaXmlFileName = TargetFilesDirectory.EaXmlFilePathLiteral;
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
