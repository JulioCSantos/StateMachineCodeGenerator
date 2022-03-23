using System;

namespace StateMachineCodeGenerator.Common
{
    // ReSharper disable once InconsistentNaming
    public interface IUIDispatcher
    {
        Func<bool> CheckAccessFunc { get; set; }

        Action<Action> InvokeFunc { get; set; }

        bool CheckAccess { get; }

        void Invoke(Action action);
    }
}
