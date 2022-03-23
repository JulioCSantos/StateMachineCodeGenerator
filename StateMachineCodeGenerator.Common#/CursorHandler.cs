using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Common
{
    // Cursor value should be made disposable
    public class CursorHandler : SetPropertyBase
    {
        #region singleton
        public static CursorHandler Instance { get; } = new CursorHandler();

        public static CursorHandler GetInstance() { return Instance; }

        private CursorHandler() { }
        #endregion singleton

        #region ActiveMembersQueue
        private ConcurrentQueue<string> _activeMembersQueue;
        protected ConcurrentQueue<string> ActiveMembersQueue
        {
            get => _activeMembersQueue ?? (_activeMembersQueue = new ConcurrentQueue<string>());
            set => SetProperty(ref _activeMembersQueue, value);
        }
        #endregion ActiveMembersQueue

        public void AddBusyMember([CallerMemberName] string memberName = default) {
            ActiveMembersQueue.Enqueue(memberName);
            //System.Diagnostics.Debug.WriteLine($"added; {memberName} ({ActiveMembersQueue.Count})) ");
            AssessIsBusy();
        }

        private void AssessIsBusy() {
            Console.WriteLine(ActiveMembersQueue.Count);
            IsBusy = ActiveMembersQueue.Any();
        }

        public bool RemoveBusyMember([CallerMemberName] string memberName = default)
        {
           var result = ActiveMembersQueue.TryDequeue(out memberName);
           AssessIsBusy();
           //System.Diagnostics.Debug.WriteLine($"removed: {memberName} ({ActiveMembersQueue.Count})) ");

            return result;
        }

        #region IsBusy
        private bool _isBusy;
        public bool IsBusy {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
        #endregion IsBusy
    
    }
}