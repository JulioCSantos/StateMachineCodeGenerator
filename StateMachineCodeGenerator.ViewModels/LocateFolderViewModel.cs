using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.ViewModels
{
    public class LocateFolderViewModel : PopupViewModelBase
    {
        #region LocatedFolderName
        private string _locatedFolderName;
        public string LocatedFolderName {
            get => _locatedFolderName;
            set => SetProperty(ref _locatedFolderName, value);
        }
        #endregion LocatedFolderName

        public Func<string, bool?> ShowDialogFuncWithArgs { get; set; }

        public bool? ShowDialog(string folder) {
            return ShowDialogFuncWithArgs(folder);
        }
    }
}
