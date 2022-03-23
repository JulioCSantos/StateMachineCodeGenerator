using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.Model
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}")]
    public class InitialState : StateBase
    {
        public InitialState(string id) : base(id) { }

        public override string Name
        {
            get {
                if (string.IsNullOrEmpty(base.Name)) return Owner?.Name == null ? base.Name : Owner?.Name + base.Name;
                else return base.Name;
            }
            set { base.Name = value; }
        }

        public override string ToNSFType()
        {
            return "NSFInitialState";
        }
    }
}
