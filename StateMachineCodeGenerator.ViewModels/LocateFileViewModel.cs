using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class LocateFileViewModel : PopupViewModelBase
    {
        #region LocatedFileName
        private string _locatedFileName;
        public string LocatedFileName {
            get => _locatedFileName;
            set => SetProperty(ref _locatedFileName, value);
        }
        #endregion LocatedFileName
    }
}
