using StateMachineCodeGenerator.Common;
using StateMachineMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels {
    public class FilesSelectionViewModel  : SetPropertyBase {
        public RelayCommand StartParsingCommand => new RelayCommand((o) => StartParsing());
        #region FilePath
        //private string _filePath = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\StateMachineMetadata\Dependencies\LaserProcessing Model new";
        private string _filePath = @"C:\AppData\InputTestsFiles";
        public string FilePath {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }
        #endregion FilePath

        #region TargetPath
        private string _targetPath = @"C:\Users\santosj25\source\repos\JulioCSantos\StateMachineCodeGenerator\TestsSubject";
        public string TargetPath {
            get => _targetPath;
            set => SetProperty(ref _targetPath, value);
        }
        #endregion TargetPath

        public void StartParsing() {
            Main.MainEntryPoint(new string[] { FilePath, TargetPath });
            var models = Main.MainModels;
        }
    }
}
