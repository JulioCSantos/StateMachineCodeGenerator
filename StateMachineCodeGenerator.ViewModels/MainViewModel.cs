using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.Common.Extensions;
using StateMachineCodeGeneratorSystem.Templates;
using StateMachineMetadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class MainViewModel : SetPropertyBase
    {
        #region properties

        #region Logger

        private XPLogger _logger;
        public XPLogger Logger {
            get {
                if (_logger != null) { return _logger; }
                _logger = XPLogger.Instance;
                _logger.ActiveErrors.CollectionChanged += (s, e) =>
                    RaisePropertyChanged(nameof(MessagesHeight));

                return _logger;
            }
        }

        private void ActiveErrors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            
        }

        #endregion Logger

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
            string key = null;
            if (args.Action == NotifyCollectionChangedAction.Add) {
                key = (string)args.NewItems?[0];
                if (key == null) { return; } // defensive
                if (InputFilesDirectory.ContainsKey(key)) { InputFilesDirectory[key] = TargetFilesDirectory.Clone; }
                else { InputFilesDirectory.Add(key, TargetFilesDirectory.Clone); }
            }

            if (args.Action == NotifyCollectionChangedAction.Remove) {
                key = (string)args.OldItems?[0];
                if (key == null) { return; } // defensive

                var targetFilesDirectory = InputFilesDirectory[key];
                targetFilesDirectory.PropertyChanged -= TargetFilesDirectory_PropertyChanged;
                if (InputFilesDirectory.ContainsKey(key)) { InputFilesDirectory.Remove(key); }

                PersistPreviousInputFiles(key);
                key = null;
            }

        }

        private void PersistPreviousInputFiles(string key)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true, IgnoreReadOnlyFields = true, WriteIndented = true
            };
            var serializedInputFiles = JsonSerializer.Serialize(InputFilesDirectory, options);
            File.WriteAllText(SerializedInputFilesPath, serializedInputFiles);
            if (key == null)
            {
                TargetFilesDirectory = null;
            }

            _selectedInputFileKey = key;
            RaisePropertyChanged(nameof(SelectedInputFileKey));
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
                if (value == null) { SetProperty(ref _selectedInputFileKey, null); }
                else {
                    TargetFilesDirectory = InputFilesDirectory[value];
                    SetProperty(ref _selectedInputFileKey, value);
                    TargetFilesDirectory.EaXmlParsed = true;
                    RaisePropertyChanged(nameof(CanGenerateCode));
                }
                RaisePropertyChanged(nameof(CanDeleteInputFile));
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
            get => _targetFilesDirectory ??= new TargetFilesDirectory();
            set {
                if (_targetFilesDirectory != null) { 
                    _targetFilesDirectory.PropertyChanged -= TargetFilesDirectory_PropertyChanged; }
                SetProperty(ref _targetFilesDirectory, value);
                if (_targetFilesDirectory != null) {
                    _targetFilesDirectory.PropertyChanged += TargetFilesDirectory_PropertyChanged;
                }
            }
        }

        private void TargetFilesDirectory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(TargetFilesDirectory.EaXmlParsed):
                    if (TargetFilesDirectory.EaXmlParsed == false) {
                        SelectedInputFileKey = null;
                        //TargetFilesDirectory.CleanUpTargetFilesDirectory();
                    }
                    goto case (nameof(TargetFilesDirectory.TargetFilesDirectoryInfo));
                case nameof(TargetFilesDirectory.EaXmlFileInfo):
                case nameof(TargetFilesDirectory.TargetFilesDirectoryInfo):
                    RaisePropertyChanged(nameof(CanGenerateCode));
                    RaisePropertyChanged(nameof(GenerateCodeTooltip));
                    break;

            }
        }

        #endregion TargetFilesDirectory

        public RelayCommand ClearErrorsCommand => new((o) => Logger.ClearErrors());

        #region WindowTitle
        private string _windowTitle;
        public string WindowTitle
        {
            get {
                if (_windowTitle != null) return _windowTitle;
                StackFrame[] frames = new StackTrace().GetFrames();
                var initialAssemblyName = (from f in frames
                        select f.GetMethod()?.ReflectedType?.AssemblyQualifiedName
                    ).Distinct().Last() ?? Assembly.GetEntryAssembly()?.GetName().Name;
                return _windowTitle ??= "GenSys State Machine Generator V" + Assembly.GetEntryAssembly()?.GetName().Version;
            }
        }
        #endregion WindowTitle

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
        private void MessagesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            RaisePropertyChanged(nameof(MessagesHeight));
        }
        #endregion MessagesOnCollectionChanged

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

        #region AddMessage
        public void AddMessage(string message) {
            var messageWithPrefix = MsgNbr + message;
            if (Messages.Any() && Messages[0].Contains(message)) { Messages[0] = messageWithPrefix; }
            else {
                Messages.Insert(0, messageWithPrefix);
            }
        }
        #endregion AddMessage

        #region MessagesHeight
        private double _messagesHeight;
        public double MessagesHeight {
            get {
                //MessagesHeight = Math.Min(Messages.Count * 22, 100);
                if (Logger.ActiveErrors.Any() == false) { MessagesHeight = 0; }
                else { MessagesHeight = Math.Min(60 + Logger.ActiveErrors.Count * 22, 300);}
                return _messagesHeight;
            }
            set {
                if ( Equals(value, _messagesHeight) == false) {
                    _messagesHeight = value;
                    RaisePropertyChanged(nameof(MessagesHeight));
                    RaisePropertyChanged(nameof(ClearErrorsVisibility));
                }
            }
        }

        #endregion MessagesHeight

        #region ClearErrorsVisibility
        public string ClearErrorsVisibility {
            get {
                var visibility = "Visible";
                if (MessagesHeight == 0) { visibility = "Collapsed"; }

                return visibility;
            }
        }

        #endregion ClearErrorsVisibility

        #region WindowHeight
        private double _windowHeight = 500;
        public double WindowHeight {
            get => _windowHeight + MessagesHeight;
            set => SetProperty(ref _windowHeight, value);
        }

        #endregion WindowHeight

        #region MessagesOpacity
        private double _messagesOpacity;
        public double MessagesOpacity {
            get => _messagesOpacity;
            set => SetProperty(ref _messagesOpacity, value);
        }
        #endregion MessagesOpacity

        #endregion Messages animation

        public ErrorLog MainViewModelErr1 => new ErrorLog(new ErrorId(nameof(MainViewModelErr1)), "{0] file could not be read.", ErrorSeverity.Error);

        #endregion properties

        #region constructor
        public MainViewModel() {
            try {
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
                    if (Logger.ActiveErrors.Any(e => e.Severity.Value == ErrorSeverity.Error.Value) == false) {
                        Logger.ClearErrors();
                        Logger.Seq = 0;
                    }
                }
            }
            catch (Exception e) {
                var err = ErrorLog.GetEditedErrorLog(MainViewModelErr1.Id, new object[] { SerializedInputFilesPath });
                err.Message += " - " + e;
                XPLogger.Instance.AddError(err);
            }


            this.TargetFilesDirectory = new TargetFilesDirectory();

            this.PropertyChanged += MainViewModel_PropertyChanged;

        }

        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(MessagesHeight):
                    RaisePropertyChanged(nameof(WindowHeight));
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
        public RelayCommand DeleteInputFilesItemCommand => new RelayCommand(DeleteInputFilesItem);

        #region LocateEaXmlFile
        public const string EnterpriseArchitectFilterLiteral = "EA files (*.xml)|*.xml|All files (*.*)|*.*";
        public void LocateEaXmlFile(object path) {
            TargetFilesDirectory = new TargetFilesDirectory();
            IPopupView view = DialogServices.Instance.Dialogs[nameof(LocateFileViewModel)];
            var vm = view.Vm as LocateFileViewModel;
            if (vm == null) throw new Exception(); // will not happen
            var eaXmlFileName = string.IsNullOrEmpty(TargetFilesDirectory.EaXmlFileName) ? 
                TargetFilesDirectory.EaXmlFileNameLiteral : TargetFilesDirectory.EaXmlFileName;
            var fileLocated = vm.ShowDialog(eaXmlFileName) ?? view.Vm.ClosingResult;
            if (fileLocated == true) {
                try {
                    TargetFilesDirectory.EaXmlFileName = vm.LocatedFileName;
                }
                catch (Exception e) {
                    var filename = (new FileInfo(TargetFilesDirectory.EaXmlFileName)).Name;
                    var err = ErrorLog.GetEditedErrorLog(MainViewModelErr1.Id, new object[] { filename });
                    err.Message += " - " + e.Message;
                    XPLogger.Instance.AddError(err);
                    AddMessage($"{filename}: {e.Message}");
                }
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
            get {
                var currCanGenerateCode = TargetFilesDirectory.EaXmlFileInfo?.Exists == true
                       && TargetFilesDirectory.TargetFilesDirectoryInfo != null
                       && TargetFilesDirectory.EaXmlParsed;

                return currCanGenerateCode;
            }
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
                AddMessage("State machine files generated for " + key);
                PersistPreviousInputFiles(key);
                //await Task.Delay(300); // give enough time to observe the busy indicator
            }
            RaisePropertyChanged(nameof(PreviousInputFilesVisibility));
            CursorHandler.Instance.RemoveBusyMember();
        }
        #endregion GenerateCode

        #endregion GenerateCode properties

        #region Delete Input Files item
        public void DeleteInputFilesItem(object obj) {
            if (CanDeleteInputFile == false) { return; }

            var asdf = SelectedInputFileKey;
            TargetFilesDirectory.EaXmlFileName = null;
            TargetFilesDirectory.SolutionFileName = null;
            TargetFilesDirectory.CleanUpTargetFilesDirectory();
            PreviousInputFiles.Remove(asdf);
        }


        #endregion Delete Input Files item

        #region CanDeleteImputFile
        public bool CanDeleteInputFile {
            get => string.IsNullOrEmpty(SelectedInputFileKey) == false;
        }
        #endregion CanDeleteImputFile

        #endregion commands

    }
}
