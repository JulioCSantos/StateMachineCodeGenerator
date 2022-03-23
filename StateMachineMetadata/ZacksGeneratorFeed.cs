using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata
{
    public class ZacksGeneratorFeed : IToCSharp
    {
        public IEnumerable<string> ToCSharp(MainModel model)
        {
            var results = model.ExternalTransitions.Where(t => t.Trigger?.Name != null).Select(t => "//" + t.Trigger.Name).ToList();
            results.Add("");

            foreach (var composedState in model.States.Where(s => s.Children.Any()))
            {
                var stateLine = $"//{composedState.Name}: {string.Join(", ", composedState.Children.Select(c => c.Name).ToList())}";
                results.Add(stateLine);
            }
            results.Add("");

            foreach (var tt in model.ExternalTransitions)
            {
                var transtLine = $"//External Transition: {tt.Name} From:{tt.Source.Name} To:{tt.Target.Name} Via:{tt.Trigger.Name} Action:{tt.ActionName ?? "none"} Guard:{tt.GuardExpressionValue ?? "none"}";
                results.Add(transtLine);
            }

            var invalidInternalTransitions = model.InternalTransitions.Where(t => t.OwnerState == null);
            if (invalidInternalTransitions.Any() && Debugger.IsAttached) Debugger.Break();
            foreach (var tt in model.InternalTransitions.Where(t => t.OwnerState != null))
            {
                var transtLine = $"//Internal Transition: {tt.Name}. OwnedBy {tt.OwnerState.Name}. Action {tt.ActionName}  Guard:{tt.GuardExpressionValue ?? "none"}";
                results.Add(transtLine);
            }

            return results;
        }
    }
}
