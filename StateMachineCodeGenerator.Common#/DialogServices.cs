using System.Collections.Generic;

namespace StateMachineCodeGenerator.Common
{
    public class DialogServices
    {
        #region Dialogs
        private Dictionary<string, IPopupView> _dialogs;
        public Dictionary<string, IPopupView> Dialogs => _dialogs ?? (_dialogs = new Dictionary<string, IPopupView>());
        #endregion Dialogs

        #region singleton
        public static DialogServices Instance { get; } = new DialogServices();

        public static DialogServices GetInstance() { return Instance; }

        private DialogServices() { }
        #endregion singleton

    }
}
