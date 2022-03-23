using StateMachineMetadata.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StateMachineMetadata.Model
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}")]
    public abstract class EntityBase
    {
        private readonly string id;
        public string Id { get { return id; } }
        private string name { get; set; }
        public virtual string Name { get { return name ?? OrigName?.ToValidCSharpName(); } set { name = value; } }
        public string OrigName { get; set; }
        public virtual XElement XmlELement { get; set; }

        public string stype { get; set; }
        public int ntype = int.MinValue;

        public bool IsManufactured { get; set; }

        public EntityBase(string id)
        {
            this.id = id;
        }

        public virtual void Map(XElement elem)
        {
            XmlELement = elem;
            OrigName = elem.GetName();
            stype = elem.Descendants().FirstOrDefault(d => d.Attribute("tag")?.Value == "ea_stype")?.Attribute("value").Value;
            int.TryParse(elem.Descendants().FirstOrDefault(d => d.Attribute("tag")?.Value == "ea_ntype")?.Attribute("value").Value, out ntype);
        }

        public virtual void Map(XElement elem, Dictionary<string, EntityBase> elementsDictionary)
        {
            Map(elem);
        }

        public abstract string ToNSFType();

        public string ValidCSharpName => Name?.ToValidCSharpName();

    }
}
