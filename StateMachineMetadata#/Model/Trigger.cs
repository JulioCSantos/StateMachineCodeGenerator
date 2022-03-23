using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.Model
{
    [System.Diagnostics.DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    public class Trigger 
    {
        public string Name { get; }
        public Transition TransitionOwner { get; set; }
        public Trigger(string name) 
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            this.Name = name;
        }

        public const string FaultEventStr = "FaultEvent";
        public bool IsFaultTrigger { get { return Name.EndsWith(FaultEventStr); } }
        public string FaultTriggerName { get { return IsFaultTrigger ? Name.Replace(FaultEventStr, "Fault") : null; } }
        //public static IEnumerable<string> GetTriggerNamesList ()
        //{
        //    return Application.ActiveModel.Triggers.Where(t => !string.IsNullOrEmpty(t.Name)).Select(t => t.Name).Distinct();
        //}
    }
}
