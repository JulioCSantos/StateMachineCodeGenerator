using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Common
{
    public interface IPopupViewModelBase
    {
        Func<bool?> ShowDialogFunc { get; set; }
        Func<Task<bool?>> ShowDialogAsyncFunc { get; set; }
        IPopupViewModelBase DataContext { get; set; }
        bool? ClosingResult { get; }
        RelayCommand OKCommand { get; }
        RelayCommand CancelCommand { get; }
        bool? ShowDialog();
        Task<bool?> ShowDialogAsync(IPopupViewModelBase viewModel);
        Task<bool?> ShowDialogAsync();
        event PropertyChangedEventHandler PropertyChanged;
        bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null);
    }
}