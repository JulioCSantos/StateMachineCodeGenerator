using System;

namespace StateMachineCodeGenerator.Common
{
    // ReSharper disable once InconsistentNaming
    public class UIDispatcher : IUIDispatcher
    {
        #region IUIDispatcher
        public Func<bool> CheckAccessFunc { get; set; }
        public Action<Action> InvokeFunc { get; set; }

        public bool CheckAccess => CheckAccessFunc();

        public void Invoke(Action action)
        {
            InvokeFunc(action);
        }
        #endregion IUIDispatcher

        #region singleton

        private static readonly UIDispatcher instance = new UIDispatcher();

        // Explicit static constructor to tell C# compiler  
        // not to mark type as beforefieldinit  
        public static UIDispatcher Instance => instance;
        public static UIDispatcher GetInstance() { return instance; }


        #endregion singleton

    }
}
