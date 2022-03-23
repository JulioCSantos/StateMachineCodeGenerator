using StateMachineMetadata.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace StateMachineMetadata.Model
{
    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    public class InternalTransition : Transition
    {
        public InternalTransition(string id, string documentation, bool documentationIsComment) : base(id)
        {
            if (string.IsNullOrEmpty(documentation)) throw new ArgumentException(nameof(documentation));
            Documentation = documentation;
            DocumentationIsComment = documentationIsComment;
        }

        public string Documentation { get; set; }
        public bool DocumentationIsComment { get; set; }

        public State OwnerState { get; set; }
        public override StateBase SourceState { get { return OwnerState; } }

        internal string OwnerId { get; set; }
        public override string Name
        {
            get { return base.Name ?? $"{OwnerState?.ValidCSharpName}On{Trigger?.Name ?? "missingevent"}Transition"; }
            set { base.Name = value; }
        }

        public int Level { get { return OwnerState.Level; } }

        public override string ToNSFType()
        {
            return "NSFInternalTransition";
        }
        public override void Map(XElement elem)
        {
            OwnerId = elem.Descendants().Where(d => d.Attribute("tag")?.Value == "owner").FirstOrDefault()?.Attribute("value").Value;

            // Don't map Documentation to Trigger and Action if it just a comment
            if (DocumentationIsComment) return;
            base.Map(elem);
            var documentation = Documentation;
            var regex = new Regex(@"(\[.*\])");
            var guardGroupMatch = regex.Match(Documentation);
            if (guardGroupMatch.Success)
            {
                this.GuardExpressionValue = guardGroupMatch.Groups[1].Value;
                documentation = regex.Replace(documentation, "");
                this.GuardName = this.GuardExpressionValue.ToValidCSharpName();
                //System.Diagnostics.Debugger.Break();
            }
            var docNames = documentation.Replace(@"\", @" ").Replace(@"/", @" ").Replace(
                "\n", @" ")
                .Split(new char[]{ ' '}, StringSplitOptions.RemoveEmptyEntries).Select(n => n.Trim().ToValidCSharpName()).ToArray();
            if (docNames.Length > 0) Trigger = new Trigger(docNames[0]) { TransitionOwner = this };
            if (docNames.Length > 1)
            {
                this.ActionName = docNames[1];
                Actions = new List<string>();
                for (int i = 1; i < docNames.Length; i++) { Actions.Add(docNames[i]); }
            }
        }
    }
}
