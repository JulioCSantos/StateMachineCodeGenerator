using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StateMachineCodeGenerator.Common;
using StateMachineMetadata;

namespace StateMachineCodeGenerator.ViewModels {

    public class FilesSelectionViewModel :SetPropertyBase {

        public RelayCommand StartParsingCommand => new RelayCommand((o) => StartParsing());
        #region FilePath
        private string _filePath = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata\Dependencies\LaserProcessing Model.xml";
        public string FilePath {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }
        #endregion FilePath

        #region GeneratedFilesPath
        private string _generatedFilesPath = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";
        public string GeneratedFilesPath {
            get => _generatedFilesPath;
            set => SetProperty(ref _generatedFilesPath, value);
        }
        #endregion GeneratedFilesPath

        public void StartParsing() {
            Main.MainEntryPoint(new string[] {FilePath});
            var models = Main.MainModels;
        }

    }
}
