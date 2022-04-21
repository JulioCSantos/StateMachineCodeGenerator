using StateMachineCodeGenerator.Common;
using StateMachineCodeGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StateMachineCodeGenerator.Gui
{
    public class LocateFolderView : IPopupView
    {
        #region Vm
        public IPopupViewModelBase Vm { get; set; }
        public LocateFolderViewModel LocateFolderViewModel => Vm as LocateFolderViewModel;
        #endregion Vm

        public LocateFolderView(LocateFolderViewModel vm) {
            Vm = vm;
            LocateFolderViewModel.ShowDialogFunc = ShowDialog;
            LocateFolderViewModel.ShowDialogFuncWithArgs = ShowDialog;
        }

        public bool? ShowDialog() {
            var folderDialog = new FolderBrowserDialog();
            var dialogResult = folderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK) {
                LocateFolderViewModel.LocatedFolderName = folderDialog.SelectedPath;
                return true;
            }
            return false;
        }

        public bool? ShowDialog(string folder) {
            if (string.IsNullOrEmpty(folder)) { return ShowDialog(); }
            var folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = folder;
            var dialogResult = folderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK) {
                LocateFolderViewModel.LocatedFolderName = folderDialog.SelectedPath;
                return true;
            }
            return false;
        }
    }
}
