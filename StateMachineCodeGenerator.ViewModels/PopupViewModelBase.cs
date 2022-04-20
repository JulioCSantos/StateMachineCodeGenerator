using StateMachineCodeGenerator.Common;
using System;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class PopupViewModelBase : SetPropertyBase, IPopupViewModelBase
    {

        #region properties

        #region ShowDialogFunc
        public Func<bool?> ShowDialogFunc { get; set; }
        #endregion ShowDialogFunc

        #region ShowDialogAsyncFunc
        public Func<Task<bool?>> ShowDialogAsyncFunc { get; set; }
        #endregion ShowDialogAsyncFunc

        #region DataContext
        private IPopupViewModelBase _dataContext;
        public IPopupViewModelBase DataContext {
            get => _dataContext;
            set => SetProperty(ref _dataContext, value);
        }
        #endregion DataContext

        #region ClosingResult
        private bool? _closingResult;
        // True if OK was selected, False if Cancel was selected, Null if window was just closed
        // Once this value is set it returns to null (to support the case when the Window is just closed).
        public bool? ClosingResult {
            get => _closingResult;
            private set
            {
                _closingResult = value;
                RaisePropertyChanged(); //Force PropertyChanged event even if the new and old values are the equal
            }
        }

        #endregion ClosingResult

        #region OKCommand
        public RelayCommand OKCommand => new RelayCommand((o) => { ClosingResult = true; });
        #endregion OKCommand

        #region CancelCommand
        public RelayCommand CancelCommand => new RelayCommand((o) => { ClosingResult = false; });
        #endregion CancelCommand

        #endregion properties

        #region methods

        #region ShowDialog
        public bool? ShowDialog() { return ShowDialogFunc(); }
        #endregion ShowDialog

        #region ShowDialogAsync
        public async Task<bool?> ShowDialogAsync(IPopupViewModelBase viewModel) {
            DataContext = viewModel;
            return await ShowDialogAsyncFunc();
        }

        public async Task<bool?> ShowDialogAsync() {
            await ShowDialogAsyncFunc();
            var result = DataContext.ClosingResult;
            _closingResult = null;
            return result;
        }
        #endregion ShowDialogAsync

        #endregion methods

    }
}
