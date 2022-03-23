using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.Model
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}")]
    public class State : StateBase
    {
        public State(string id) : base(id) { }
        public override string ToNSFType()
        {
            return "NSFCompositeState";
        }
    }
}
