using StateMachineMetadata.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StateMachineMetadata.Model
{
    public abstract class StateBase : EntityBase
    {
        public StateBase(string id) : base(id) { }

        public string OwnerId { get; set; }
        public StateBase Owner { get; set; }
        public Coordinates Coord { get; set; }

        public string name;
        public override string Name
        {
            get { return string.IsNullOrEmpty(name) ? base.Name : name; }
            set { name = value; }
        }

        public List<string> EntryActions { get; } = new List<string>();
        public List<string> ExitActions { get; } = new List<string>();

        public List<StateBase> Children { get; } = new List<StateBase>();

        public override void Map(XElement elem, Dictionary<string, EntityBase> elementsDictionary)
        {
            base.Map(elem);
            OwnerId = elem.GetOwnerId();
            //Name = elem.Attribute("name")?.Value;
            // Point to the Onwer Element. It is possible that the Owner was not registered in the 'elementsDictionary' yet.
            if (OwnerId != null && elementsDictionary.ContainsKey(OwnerId))
                Owner = elementsDictionary[OwnerId] as StateBase;
        }

        public int Level
        {
            get
            {
                if (Owner == null) return 0;
                else return Owner.Level + 1;
            }
        }
    }
}
