using StateMachineMetadata.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.Model
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}")]
    public class ChoiceState : StateBase
    {
        public ChoiceState(string id) : base(id) { }

        public override string ToNSFType()
        {
            return "NSFChoiceState";
        }

        public override string Name
        {
            get {
                if (!string.IsNullOrEmpty(base.name)) return base.name;
                var validName = base.OrigName.Split(';')[0].ToValidCSharpName();
                if (string.IsNullOrEmpty(validName)) System.Diagnostics.Debugger.Break();
                if (validName.EndsWith("Choice")) validName += "State";
                if (!validName.EndsWith("ChoiceState")) validName += "ChoiceState";
                return validName;
            }
            set { base.name = value; }
        }
    }
}
