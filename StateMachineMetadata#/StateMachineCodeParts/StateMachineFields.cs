using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.StateMachineCodeParts
{
    public class StateMachineFields : IToCSharp
    {
        private MainModel Model { get; set; }
        public IEnumerable<string> ToCSharp(MainModel model)
        {
            Model = model;
            var results =  new List<string>() {
@"",
@"        // ***********************************",
@"        // State Machine NSF Event Definitions",
@"        // ***********************************",
@"",
            };
            results.AddRange(GetEventDefitions());
            results.AddRange(new List<string>()
            {
@"",
@"        // ***********************************",
@"        // End of State Machine NSF Event Definitions",
@"        // ***********************************",
@"",
@"",
@"        // ***********************************",
@"        // State Machine NSF State Definitions",
@"        // ***********************************",
@"",
            });
            results.AddRange(GetStateDefitions());
            results.AddRange(new List<string>()
            {
@"",
@"        // ***********************************",
@"        // End of State State Machine NSF State Definitions",
@"        // ***********************************",
@"",
            });

            return results;
        }

        #region Event Definitions
        public IEnumerable<string> GetEventDefitions()
        {
            var results = new List<string>();
            Model.EventNames.ForEach(en => results.Add($"        private static NSFEvent {en};"));
            return results;
        }
        #endregion Event Definitions

        #region State Definitions

        private List<State> setStates = new List<State>();
        public IEnumerable<string> GetStateDefitions()
        {
            var results = new List<string>();

            // define top level States
            results.Add("        // State Machine Upper Level Composite State Definitions");
            Model.TopLevelStates.ForEach(s => { results.Add($"        private NSFCompositeState o{s.ValidCSharpName};"); setStates.Add(s); });
            results.Add("");

            var groupedStates = Model.States.Where(s => s.Owner != null).GroupBy(s => s.Owner.Name);
            foreach (var group in groupedStates)
            {
                results.Add($"        // State Machine {group.Key} State Definitions");
                foreach (var state in group)
                {
                    results.Add($"        private {state.ToNSFType()}".PadRight(40) + $"o{state.Name};");
                }
                results.Add("");
            }
            return results;
        }

        #endregion State Definitions

        #region Transition Definitions
        #endregion Transition Definitions
    }
}
