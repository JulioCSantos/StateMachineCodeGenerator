using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.Generator;
using StateMachineCodeGeneratorSystem.Templates;
using StateMachineMetadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class MainViewModel : SetPropertyBase
    {
        #region properties

        #region PreviousInputFiles asssociated properties

        #region PreviousInputFiles
        private ObservableCollection<string> _previousInputFiles;
        public ObservableCollection<string> PreviousInputFiles {
            get { return _previousInputFiles ?? (PreviousInputFiles = new ObservableCollection<string>()); }
            protected set {
                if (_previousInputFiles != null) { PreviousInputFiles.CollectionChanged -= PreviousInputFiles_CollectionChanged; }
                SetProperty( ref _previousInputFiles, value);
                RaisePropertyChanged(nameof(PreviousInputFilesVisibility));
                if (_previousInputFiles != null) { PreviousInputFiles.CollectionChanged += PreviousInputFiles_CollectionChanged; }
            }
        }
        #endregion PreviousInputFiles

        #region InputFilesDirectory
        public Dictionary<string, TargetFilesDirectory> InputFilesDirectory { get; } = new ();
        #endregion InputFilesDirectory

        #region PreviousInputFiles_CollectionChanged
        public void PreviousInputFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
            if (args.Action == NotifyCollectionChangedAction.Add) {
                var key = (string)args.NewItems?[0];
                if (key == null) { return; }
                if (InputFilesDirectory.ContainsKey(key)) { InputFilesDirectory[key] = TargetFilesDirectory.Clone; }
                else { InputFilesDirectory.Add(key, TargetFilesDirectory.Clone); }

                var options = new JsonSerializerOptions {
                    IgnoreReadOnlyProperties = true
                    , IgnoreReadOnlyFields = true
                    , WriteIndented = true
                };
                var serializedInputFiles = JsonSerializer.Serialize(InputFilesDirectory, options);
                File.WriteAllText(SerializedInputFilesPath, serializedInputFiles);
                _selectedInputFileKey = key;
                RaisePropertyChanged(nameof(SelectedInputFileKey));
            }
        }
        #endregion PreviousInputFiles_CollectionChanged

        #region PreviousInputFilesVisibility
        public string PreviousInputFilesVisibility {
            get {
                if (PreviousInputFiles == null) { return "Collapsed"; }
                if (PreviousInputFiles.Any() == false) { return "Collapsed"; }

                return "Visible";

            }
        }

        #endregion PreviousInputFilesVisibility

        #endregion PreviousInputFiles asssociated properties

        #region SelectedInputFiles
        private string _selectedInputFileKey;
        public string SelectedInputFileKey {
            get => _selectedInputFileKey;
            set {
                TargetFilesDirectory = InputFilesDirectory[value];
                SetProperty(ref _selectedInputFileKey, value);
                RaisePropertyChanged(nameof(CanGenerateCode));
            }
        }
        #endregion SelectedInputFiles

        #region SerializedinputFilesPath
        private string _serializedInputFilesPath;
        public string SerializedInputFilesPath {
            get {
                if (string.IsNullOrEmpty(_serializedInputFilesPath) == false) { return _serializedInputFilesPath;}

                var directoryName = new FileInfo(Assembly.GetEntryAssembly()?.Location ?? string.Empty).DirectoryName;
                _serializedInputFilesPath = Path.Combine(directoryName ?? string.Empty, nameof(InputFilesDirectory) + ".json");

                return _serializedInputFilesPath;
            }
        }
        #endregion SerializedinputFilesPath

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

        #region MsgNbr
        private static int _messagesCount;
        public string MsgNbr {
            get {
                var msgNbr = _messagesCount.ToString().PadLeft(3, '0') + ": ";
                _messagesCount++;
                return msgNbr;
            }
        }
        #endregion MsgNbr

        #region LogMessage
        public void LogMessage(string message) {
            var messageWithPrefix = MsgNbr + message;
            if (Messages.Any() && Messages[0].Contains(message)) { Messages[0] = messageWithPrefix; }
            else {
                if (Messages.Count >= 5) { Messages.Remove(Messages.Last()); }
                Messages.Insert(0, messageWithPrefix);
            }
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
            if (new FileInfo(SerializedInputFilesPath).Exists) {
                var serializedInputFiles = File.ReadAllText(SerializedInputFilesPath);
                InputFilesDirectory = JsonSerializer.Deserialize<Dictionary<string, TargetFilesDirectory>>(serializedInputFiles);
                if (InputFilesDirectory != null) {
                    //instantiate through backing field to avoid collection Changed handling
                    _previousInputFiles = new ObservableCollection<string>(); 
                    InputFilesDirectory.Keys.ToList().ForEach(k => _previousInputFiles.Add(k));
                    PreviousInputFiles = _previousInputFiles; //this activates CollectionChanged event
                }
                RaisePropertyChanged(nameof(PreviousInputFilesVisibility));
            }

            this.TargetFilesDirectory.PropertyChanged += TargetFilesDirectory_PropertyChanged;

        }

        private void TargetFilesDirectory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
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

        #region LocateEaXmlFile
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
        #endregion LocateEaXmlFile

        #region LocateSolutionFile
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
        #endregion LocateSolutionFile

        #region LocateTargetFolder
        public void LocateTargetFolder(object path) {
            IPopupView view = DialogServices.Instance.Dialogs[nameof(LocateFolderViewModel)];
            var vm = view.Vm as LocateFolderViewModel;
            if (vm == null) throw new Exception(); // will not happen
            var fileLocated = vm.ShowDialog(TargetFilesDirectory.TargetFilesDirectoryName) ?? view.Vm.ClosingResult;
            if (fileLocated == true) {
                TargetFilesDirectory.TargetFilesDirectoryName = vm.LocatedFolderName;
            }
        }
        #endregion LocateTargetFolder

        #region LocateCSharpFiles
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
        #endregion LocateCSharpFiles

        #region GetFileIx
        public TargetFilesDirectory.TargetPath GetFileIx(string fileLabel) {
            TargetFilesDirectory.TargetPath fileIx = TargetFilesDirectory.TargetPath.unkown;
            if (fileLabel == StateMachineBaseFileLbl) { fileIx = TargetFilesDirectory.TargetPath.StateMachineBaseFilePath; }
            if (fileLabel == StateMachineDerivedFileLbl) { fileIx = TargetFilesDirectory.TargetPath.StateMachineDerivedFilePath; }
            if (fileLabel == MainModelBaseFileLbl) { fileIx = TargetFilesDirectory.TargetPath.MainModelBaseFilePath; }
            if (fileLabel == MainModelDerivedFileLbl) { fileIx = TargetFilesDirectory.TargetPath.MainModelDerivedFilePath; }

            return fileIx;
        }
        #endregion GetFileIx

        #region GenerateCode properties

        #region CanGenerateCode
        public bool CanGenerateCode {
            get => TargetFilesDirectory.EaXmlFileInfo?.Exists == true && TargetFilesDirectory.TargetFilesDirectoryInfo != null;
        }
        #endregion CanGenerateCode

        #region GenerateCodeTooltip
        public string GenerateCodeTooltip {
            get {
                if (CanGenerateCode) {
                    //return "Generate code in the 'Target Folder for generated files'";
                    return "Generate State Machine code in the 'Target Folder ...'";
                }
                return "Must select 'EA Exported xml file' and 'Solution (.sln) file' to enable button";
            }
        }
        #endregion GenerateCodeTooltip

        #region GenerateCode
        public async void GenerateCode() {
            CursorHandler.Instance.AddBusyMember();
            var codeGenerator = new TemplatesGenerator(TargetFilesDirectory.EaXmlFileInfo);
            var filesGenerated = await codeGenerator.GenerateFiles(TargetFilesDirectory.GetMetadataTargetPaths());
            if (filesGenerated) {
                var key = TargetFilesDirectory.EaXmlFileInfo.Name;

                // Update PreviousInputFiles
                if (PreviousInputFiles.Contains(key)) { InputFilesDirectory[key] = TargetFilesDirectory.Clone; }
                else { PreviousInputFiles.Add(key);}
                // Refresh derived files cache
                TargetFilesDirectory.TargetFilesDirectoryName = 
                    TargetFilesDirectory.TargetFilesDirectoryName; 
                // update log messages panel
                LogMessage("State machine files generated for " + key);
                await Task.Delay(300); // give enough time to observe the busy indicator
            }
            RaisePropertyChanged(nameof(PreviousInputFilesVisibility));
            CursorHandler.Instance.RemoveBusyMember();
        }
        #endregion GenerateCode

        #endregion GenerateCode properties

        #endregion commands

    }
}
