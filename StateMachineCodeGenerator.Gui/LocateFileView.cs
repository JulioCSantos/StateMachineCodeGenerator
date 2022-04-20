using Microsoft.Win32;
using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Gui
{
    public class LocateFileView : IPopupView
    {

        #region Vm
        public IPopupViewModelBase Vm { get; set; }
        public LocateFileViewModel LocateFileViewModel => Vm as LocateFileViewModel;
        #endregion Vm

        public LocateFileView(LocateFileViewModel vm) {
            Vm = vm;
            LocateFileViewModel.ShowDialogFunc = ShowDialog;
        }

        public bool? ShowDialog() {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                LocateFileViewModel.LocatedFileName = openFileDialog.FileName;
                return true;
            }

            return false;
        }

    }
}
