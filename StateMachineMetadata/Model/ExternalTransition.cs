using StateMachineMetadata.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StateMachineMetadata.Model
{
    [DebuggerDisplay("Name = {Name}")]
    public class ExternalTransition : Transition
    {
        public virtual StateBase Source { get; set; }
        public override StateBase SourceState { get {return Source; } }
        public virtual StateBase Target { get; set; }


        public override string Name
        {
            get
            {
                var name = $"{Source.ValidCSharpName.Replace("State", "")}TO{Target.ValidCSharpName.Replace("State","")}Transition";
                var fullName = Trigger?.Name == null ? name : name + "BY" + Trigger.Name;
                return fullName;
            }
            set { base.Name = value; }
        }

        private string guardName;
        public override string GuardName
        {
            get
            {
                if (!string.IsNullOrEmpty(guardName)) return guardName;
                if (Source.GetType() == typeof(ChoiceState))
                {
                    if (!string.IsNullOrEmpty(GuardExpressionValue))
                    {
                        if (string.Compare(GuardExpressionValue.Trim(), "No", ignoreCase: true) == 0)
                            guardName = "Else";
                        else
                            guardName = Source.Name.Replace("ChoiceState","");
                    }
                    else guardName = "Else";
                }
                else
                {
                    if (string.IsNullOrEmpty(GuardExpressionValue))
                        guardName = string.IsNullOrEmpty(GuardExpressionValue) ? "None" : this.Name + "Guard";
                    else
                        guardName = this.Name + GuardExpressionValue.ToValidCSharpName() + "Guard";
                }
                return guardName;
            }
            set { guardName = value; }
        }

        public int Level { get { return Source.Level; } }

        public List<ExternalTransition> ParentTransitions { get; } = new List<ExternalTransition>();
        public List<ExternalTransition> ChildrenTransitions { get; } = new List<ExternalTransition>();

        public ExternalTransition(string id) : base(id) { }
        public override void Map(XElement elem, Dictionary<string, EntityBase> elementsDictionary)
        {
            base.Map(elem, elementsDictionary);
            var sourceId = XmlELement.Attribute("source").Value;
            Source = elementsDictionary[sourceId] as StateBase;
            var targetId = XmlELement.Attribute("target").Value;
            Target = elementsDictionary[targetId] as StateBase;
            var eventName = elem.Descendants().Where(d => d.Name.LocalName == "Event").FirstOrDefault()?.Attribute("name").Value;
            if (string.IsNullOrEmpty(eventName) == false)
                Trigger = new Trigger(eventName) { TransitionOwner = this };


            var xref_property = elem.Descendants().Where(d => d.Attribute("tag")?.Value == "$ea_xref_property").FirstOrDefault()?.Attribute("value").Value;
            var isLocal = xref_property?.Contains("@VALU=local");
            if (isLocal == true) System.Diagnostics.Debugger.Break();
        }

        public bool IsSelfTransition { get { return Source == Target; } }

        public override string ToNSFType()
        {
            if (IsSelfTransition) return "NSFLocalTransition";
            else return "NSFExternalTransition";
        }
    }
}
