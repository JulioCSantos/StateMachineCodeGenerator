using StateMachineCodeGenerator.Common;
using StateMachineMetadata;
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

        public void OpenFileExplorer(object path) {

        }

        public void StartParsing() {
            Main.MainEntryPoint(new string[] { SourceFilePath, CodeTargetFolderPath });
            var models = Main.MainModels;
            var codeGenerator = new StateMachineGenerator();
            codeGenerator.Generate(models.FirstOrDefault(), CodeTargetFolderPath);
        }

    }
}
