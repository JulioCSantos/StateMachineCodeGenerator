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
        public MainView() {
            InitializeComponent();
            this.Loaded += MainView_Loaded;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CursorHandler.IsBusy)) {
                Cursor previousCursor = this.Cursor;
                this.Cursor = CursorHandler.Instance.IsBusy ? Cursors.AppStarting : Cursors.Arrow;
                this.Loaded += MainView_Loaded;
            }
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e) {
            var locateFileViewModel = new LocateFileViewModel();
            var parentWindow = this.FindParent<Window>();
            if (parentWindow == null) { return; }

            if (DialogServices.Instance.Dialogs.ContainsKey(nameof(LocateFileViewModel)) == false) {
                var popupView1 = new LocateFileView(locateFileViewModel);
                DialogServices.Instance.Dialogs.Add(nameof(LocateFileViewModel), popupView1);
            }
        }
    }
}
