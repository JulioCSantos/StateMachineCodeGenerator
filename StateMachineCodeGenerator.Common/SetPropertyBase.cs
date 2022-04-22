using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace StateMachineCodeGenerator.Common
{
    public abstract class SetPropertyBase : INotifyPropertyChanged
    {

        //public static IUIDispatcher UIDispatcher { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [DebuggerStepThrough]
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            //if (UIDispatcher?.CheckAccessFunc != null && UIDispatcher.CheckAccessFunc())
            //{
            //    UIDispatcher.InvokeFunc.Invoke(() => PropertyChanged?
            //        .Invoke(this, new PropertyChangedEventArgs(propertyName)));
            //}
            //else PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [DebuggerStepThrough]
        public virtual bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, backingField)) { return false; }

            backingField = value;
            RaisePropertyChanged(propertyName);
            
            return true;
        }
        #endregion INotifyPropertyChanged
    }
}
