using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StateMachineCodeGenerator.Gui
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainViewModel Vm { get; protected set; }
        public MainView() {
            InitializeComponent();
            Vm = (MainViewModel) DataContext;
            this.Loaded += MainView_Loaded;
            CursorHandler.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CursorHandler.IsBusy)) {
                Cursor previousCursor = this.Cursor;
                this.Cursor = CursorHandler.Instance.IsBusy ? Cursors.AppStarting : Cursors.Arrow;
            }
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e) {
            var parentWindow = this.FindParent<Window>();
            if (parentWindow == null) { return; }

            // LocateFile service
            var locateFileViewModel = new LocateFileViewModel();
            if (DialogServices.Instance.Dialogs.ContainsKey(nameof(LocateFileViewModel)) == false) {
                var locateFileView = new LocateFileView(locateFileViewModel);
                DialogServices.Instance.Dialogs.Add(nameof(LocateFileViewModel), locateFileView);
            }

            //LocateFolder service
            var locateFolderViewModel = new LocateFolderViewModel();
            if (DialogServices.Instance.Dialogs.ContainsKey(nameof(LocateFolderViewModel)) == false) {
                var locateFolderView = new LocateFolderView(locateFolderViewModel);
                DialogServices.Instance.Dialogs.Add(nameof(LocateFolderViewModel), locateFolderView);
            }
        }
    }
}
