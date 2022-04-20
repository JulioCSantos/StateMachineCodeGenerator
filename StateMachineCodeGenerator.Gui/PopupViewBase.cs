using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.ViewModels;
using System;
using System.Windows;

namespace StateMachineCodeGenerator.Gui
{
    //https://stackoverflow.com/questions/7174315/understanding-wpf-deriving-window-class/7174427#7174427
    public class PopupViewBase : Window, IPopupView
    {
        #region Vm
        public IPopupViewModelBase Vm {
            get => DataContext as PopupViewModelBase;
            set => DataContext = value;
        }

        #endregion Vm

        #region constructor
        public PopupViewBase() {
            this.DataContextChanged += PopupWindow_DataContextChanged;
            this.Closing += PopupWindow_Closing;
        }
        #endregion constructor


        #region methods
        private void PopupWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (e.OldValue is PopupViewModelBase oldVm) {
                oldVm.PropertyChanged -= Vm_PropertyChanged;
            }

            if (e.NewValue is PopupViewModelBase newVm) {
                //Vm = newVm;
                MapToViewModel();
                newVm.PropertyChanged += Vm_PropertyChanged;

            }
            else {
                throw new Exception("Only \"PopupViewModelBase\" ViewModels base types can be set as DataContext");
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(Vm.ClosingResult):
                    DialogResult = Vm.ClosingResult;
                    //Vm._closingResult = null;
                    break;
                case nameof(Vm.DataContext):
                    DataContext = Vm.DataContext;
                    break;
            }
        }

        public void MapToViewModel() {
            //Vm.SetContentWrapper(typeof(Window).GetProperty(nameof(Window.Content)), this);
            //Vm.Content = new PopupContent();
            Vm.ShowDialogAsyncFunc = () => Dispatcher.InvokeAsync(ShowDialog).Task;
            Vm.ShowDialogFunc = this.ShowDialog;
            Vm.DataContext = this.DataContext as PopupViewModelBase;
            //Vm.PropertyChanged += Instance_PropertyChanged;
        }

        private void PopupWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;
            this.Hide();
        }
        #endregion methods

    }
}
