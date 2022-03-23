using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.Model
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}")]
    public class Trigger 
    {
        public readonly String name;
        public String Name { get { return name; } }
        public Transition TransitionOwner { get; set; }
        public Trigger(string name) 
        {
            this.name = name;
        }
    }
}
