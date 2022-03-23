using StateMachineMetadata.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StateMachineMetadata.Model
{
    public abstract class Transition : EntityBase
    {
        public Transition(string id) : base(id) { }

        public virtual Trigger Trigger { get; set; }
        public List<string> Actions { get; set; }

        private string _actionName;
        public string ActionName
        {
            get { return _actionName; }
            set { _actionName = value; }
        }


        public string GuardExpressionValue { get; set; }

        public virtual string GuardName { get; set; } = "NONE";
        public virtual StateBase SourceState { get; set; }
        public override void Map(XElement elem)
        {
            base.Map(elem);
            if (elem.Descendants().FirstOrDefault(d => d.Name.LocalName == "UninterpretedAction") != null)
            {
                Actions = elem.Descendants().Where(d => d.Name.LocalName == "UninterpretedAction")?.Select(a => a.Attribute("name")?.Value).First()
                    .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Replace("\n","").Trim()).ToList();
                if (Actions.Any()) ActionName = Actions.First().ToValidCSharpName();
                //ActionName = elem.Descendants().Where(d => d.Name.LocalName == "UninterpretedAction")?.FirstOrDefault()?.Attribute("name").Value.ToValidCSharpName();
                if (!string.IsNullOrEmpty(ActionName) && ActionName.Length < 4) System.Diagnostics.Debugger.Break();
            }
            if (elem.Descendants().FirstOrDefault(d => d.Name.LocalName == "BooleanExpression") != null)
                GuardExpressionValue = elem.Descendants().Where(d => d.Name.LocalName == "BooleanExpression")?.FirstOrDefault()?.Attribute("body")?.Value;
        }

        public override void Map(XElement elem, Dictionary<string, EntityBase> elementsDictionary)
        {
            this.Map(elem);
            base.Map(elem, elementsDictionary);
        }

    }
}
