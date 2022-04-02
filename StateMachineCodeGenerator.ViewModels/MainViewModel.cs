using StateMachineCodeGenerator.Common;
using StateMachineMetadata;
using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class MainViewModel : SetPropertyBase
    {
        public RelayCommand StartParsingCommand => new RelayCommand((o) => StartParsing());
        public RelayCommand OpenFileExplorerCommand => new RelayCommand(OpenFileExplorer);

        #region SourceFilePath
        //private string _sourceFilePath = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata\Dependencies\LaserProcessing Model new.xml";
        private string _sourceFilePath = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata new\Dependencies\LaserProcessing Model new.xml";
        public string SourceFilePath {
            get => _sourceFilePath;
            set => SetProperty(ref _sourceFilePath, value);
        }
        #endregion SourceFilePath

        #region m_oStateMachineGeneratedCodeFolderPath_textBox
        //private string _codeTargetFolderPath = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";
        private string _codeTargetFolderPath = @"C:\Users\julio\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";
        public string CodeTargetFolderPath {
            get => _codeTargetFolderPath;
            set => SetProperty(ref _codeTargetFolderPath, value);
        }
        #endregion m_oStateMachineGeneratedCodeFolderPath_textBox

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

        public bool IsModelSelectable => EAModelsList.Count > 1;

        #region constructor
        public MainViewModel() {
            this.PropertyChanged += MainViewModel_PropertyChanged;
            RaisePropertyChanged(nameof(SourceFilePath));
        }

        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(SourceFilePath):
                    EAModelsList = Main.GetStateMachineModelFromEAXMLFile(SourceFilePath);
                    SelectedEAModel = EAModelsList?.FirstOrDefault();
                    break;
                case nameof(EAModelsList):
                    RaisePropertyChanged(nameof(IsModelSelectable));
                    break;
            }

        }
        #endregion constructor


        //public MainModel GetEAModelsList(string EA_XML_FilePath) {
        //    EAModelsList = StateMachineMetadata.Main.GetStateMachineModelFromEAXMLFile(EA_XML_FilePath);
        //    return EAModelsList?.FirstOrDefault();
        //}

        public void OpenFileExplorer(object path) {

        }

        public void StartParsing() {
            //Main.MainEntryPoint(new string[] { SourceFilePath, CodeTargetFolderPath });
            //var models = Main.MainModels;
            //var codeGenerator = new StateMachineGenerator();
            //codeGenerator.Generate(models.FirstOrDefault(), CodeTargetFolderPath);
        }

    }
}
