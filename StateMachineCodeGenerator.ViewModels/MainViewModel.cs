using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.Generator;
using StateMachineCodeGeneratorSystem.Templates;
using StateMachineMetadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class MainViewModel : SetPropertyBase
    {
        #region properties

        #region PreviousInputFiles
        private ObservableCollection<string> _previousInputFiles;
        public ObservableCollection<string> PreviousInputFiles
        {
            get { return _previousInputFiles ?? (PreviousInputFiles = new ObservableCollection<string>()); }
            protected set {
                if (_previousInputFiles != null) { PreviousInputFiles.CollectionChanged -= PreviousInputFiles_CollectionChanged; }
                SetProperty( ref _previousInputFiles, value);
                if (_previousInputFiles != null) { PreviousInputFiles.CollectionChanged += PreviousInputFiles_CollectionChanged; }
            }
        }

        public Dictionary<string, TargetFilesDirectory> InputFilesDirectory { get; } = new ();

        public void PreviousInputFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
            if (args.Action == NotifyCollectionChangedAction.Add) {
                var key = TargetFilesDirectory.EaXmlFileInfo.Name;
                if (InputFilesDirectory.ContainsKey(key)) { InputFilesDirectory[key] = TargetFilesDirectory.Clone; }
                else { InputFilesDirectory.Add(key, TargetFilesDirectory.Clone); }
            }
        }

        #endregion PreviousInputFiles

        #region SelectedInputFiles
        private string _selectedInputFileKey;
        public string SelectedInputFileKey {
            get => _selectedInputFileKey;
            set {
                TargetFilesDirectory = InputFilesDirectory[value];
                SetProperty(ref _selectedInputFileKey, value);
            }
        }

        #endregion SelectedInputFiles

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

        #region Messages animation

        #region Messages
        private ObservableCollection<string> _messages;
        public ObservableCollection<string> Messages
        {
            get { return _messages ?? (Messages = new ObservableCollection<string>()); }
            set {
                if (_messages != null) {_messages.CollectionChanged -= MessagesOnCollectionChanged; }
                SetProperty(ref _messages, value);
                if (_messages != null) { _messages.CollectionChanged += MessagesOnCollectionChanged; }
            }
        }
        #endregion Messages

        #region MessagesOnCollectionChanged
        private async void MessagesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            await ShowVanishingMessages();
        }
        #endregion MessagesOnCollectionChanged

        #region ShowVanishingMessages
        public async Task ShowVanishingMessages() {
            MessagesHeight = Messages.Count * 25;
            var refreshRate = 75;
            var showDurMilliseconds = 3000;
            var iterations = showDurMilliseconds / refreshRate;
            for (int i = 1; i <= iterations; i++) {
                await Task.Delay(refreshRate);
                MessagesOpacity = (iterations - (double)i) / iterations;
                if (i == iterations/ 4 * 3) {
                    await StartCollapsingMessages(iterations / 4 * refreshRate);

                }
            }
            MessagesHeight = 0;
        }
        #endregion ShowVanishingMessages

        #region StartCollapsingMessages
        public async Task StartCollapsingMessages(double durationInMilliseconds) {
            var interval = (int)durationInMilliseconds / 10;
            var origHeight = MessagesHeight;
            for (int i = 1; i <= 10; i++) {
                await Task.Delay(interval);
                MessagesHeight = origHeight / 10 * (10 - i);
            }
        }
        #endregion StartCollapsingMessages

        #region LogMessage
        public void LogMessage(string message) {
            var msgNbr = Messages.Count.ToString().PadLeft(3, '0');
            var messageWithPrefix = msgNbr + ": " + message;
            if (Messages.Any() && Messages[0].Contains(message)) { Messages[0] = messageWithPrefix; }
            else {Messages.Insert(0, messageWithPrefix);}
        }
        #endregion LogMessage

        #region MessagesHeight
        private double _messagesHeight;
        public double MessagesHeight {
            get => _messagesHeight;
            set => SetProperty(ref _messagesHeight, value);
        }
        #endregion MessagesHeight

        #region MessagesOpacity
        private double _messagesOpacity;
        public double MessagesOpacity {
            get => _messagesOpacity;
            set => SetProperty(ref _messagesOpacity, value);
        }
        #endregion MessagesOpacity

        #endregion Messages animation

        #endregion properties


        #region constructor
        public MainViewModel() {
            //this.PropertyChanged += MainViewModel_PropertyChanged;
            this.TargetFilesDirectory.PropertyChanged += TargetFilesDirectory_PropertyChanged;
        }

        private void TargetFilesDirectory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                //case nameof(TargetFilesDirectory.EaXmlFileName):
                //    TargetFilesDirectory.EaModelsList = Main.GetStateMachineModelFromEAXMLFile(TargetFilesDirectory.EaXmlFileName);
                //    TargetFilesDirectory.SelectedEaModel = TargetFilesDirectory.EaModelsList?.FirstOrDefault();
                //    break;
                case nameof(TargetFilesDirectory.EaXmlFileInfo):
                case nameof(TargetFilesDirectory.TargetFilesDirectoryInfo):
                    RaisePropertyChanged(nameof(CanGenerateCode));
                    RaisePropertyChanged(nameof(GenerateCodeTooltip));
                    break;
            }
        }

        #endregion constructor

        #region commands

        public RelayCommand GenerateCodeCommand => new RelayCommand((o) => GenerateCode());
        public RelayCommand LocateEaXmlFileCommand => new RelayCommand(LocateEaXmlFile);
        public RelayCommand LocateSolutionFileCommand => new RelayCommand(LocateSolutionFile);
        public RelayCommand LocateTargetFolderCommand => new RelayCommand(LocateTargetFolder);
        public RelayCommand LocateCSharpFilesCommand => new RelayCommand(LocateCSharpFiles);

        public const string EnterpriseArchitectFilterLiteral = "EA files (*.xml)|*.xml|All files (*.*)|*.*";

        public void LocateEaXmlFile(object path) {

            IPopupView view = DialogServices.Instance.Dialogs[nameof(LocateFileViewModel)];
            var vm = view.Vm as LocateFileViewModel;
            if (vm == null) throw new Exception(); // will not happen
            var eaXmlFileName = string.IsNullOrEmpty(TargetFilesDirectory.EaXmlFileName) ? 
                TargetFilesDirectory.EaXmlFileNameLiteral : TargetFilesDirectory.EaXmlFileName;
            var fileLocated = vm.ShowDialog(eaXmlFileName
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

        #region CanGenerateCode
        public bool CanGenerateCode {
            get => TargetFilesDirectory.EaXmlFileInfo?.Exists == true && TargetFilesDirectory.TargetFilesDirectoryInfo != null;
        }

        public string GenerateCodeTooltip {
            get {
                if (CanGenerateCode) {
                    //return "Generate code in the 'Target Folder for generated files'";
                    return "Generate State Machine code in the 'Target Folder ...'";
                }
                return "Must select 'EA Exported xml file' and 'Solution (.sln) file' to enable button";
            }
        }

        #endregion CanGenerateCode

        public async void GenerateCode() {
            CursorHandler.Instance.AddBusyMember();
            var codeGenerator = new TemplatesGenerator(TargetFilesDirectory.EaXmlFileInfo);
            var filesGenerated = await codeGenerator.GenerateFiles(TargetFilesDirectory.GetMetadataTargetPaths());
            if (filesGenerated) {
                var key = TargetFilesDirectory.EaXmlFileInfo.Name;
                PreviousInputFiles.Add(key);
                TargetFilesDirectory.TargetFilesDirectoryName = 
                    TargetFilesDirectory.TargetFilesDirectoryName; // refresh derived files cache
                LogMessage("State machine files generated for " + key);
                await Task.Delay(300);
            }
            CursorHandler.Instance.RemoveBusyMember();
        }
        #endregion commands

    }
}
