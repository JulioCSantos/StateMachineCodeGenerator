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
            LocateFileViewModel.ShowDialogFuncWithArgs = ShowDialog;
        }

        public bool? ShowDialog() {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                LocateFileViewModel.LocatedFileName = openFileDialog.FileName;
                return true;
            }

            return false;
        }

        public bool? ShowDialog(string fullFilename, string filter = null, int filterIndex = 1) {
            var openFileDialog = new OpenFileDialog();
            if (string.IsNullOrEmpty(filter) == false) {
                openFileDialog.Filter = filter;
                openFileDialog.FilterIndex = filterIndex;
            }

            if (string.IsNullOrEmpty(fullFilename) == false) {
                var filename = Path.GetFileName(fullFilename);
                openFileDialog.FileName = fullFilename;
                var initialDirectory = Path.GetDirectoryName(fullFilename);
                if (initialDirectory != null) {openFileDialog.InitialDirectory = initialDirectory;}
            }

            if (openFileDialog.ShowDialog() == true) {
                LocateFileViewModel.LocatedFileName = openFileDialog.FileName;
                return true;
            }

            return false;
        }
    }
}
