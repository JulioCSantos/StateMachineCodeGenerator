using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachineMetadata.Model
{
    public class NSFClauses
    {
        public MainModel Model { get; set; }
        public NSFClauses(MainModel model)
        {
            Model = model;
        }

        #region Triggers queries
        public IEnumerable<string> GetTriggerNames()
        {
            var result = Model.Triggers.Where(t => !string.IsNullOrEmpty(t.Name)).Select(t => t.Name).Distinct().OrderBy(t => t);
            return result;
        }
        #endregion Triggers queries

        #region Triggers queries
        public IEnumerable<string> GetFaultTriggerNames() {
            var result = Model.Triggers.Where(t => !string.IsNullOrEmpty(t.Name) && t.IsFaultTrigger).Select(t => t.FaultTriggerName).Distinct().OrderBy(t => t);
            return result;
        }
        #endregion Triggers queries

        #region States queries
        public IEnumerable<StateBase> GetNamedStates()
        {
            var result = Model.States.Where(t => !string.IsNullOrEmpty(t.Name));
            return result;
        }

        public IEnumerable<IGrouping<StateBase, StateBase>> GetNamedStatesGroupedByOwner()
        {
            var result = GetNamedStates().GroupBy(s => s.Owner).OrderBy(g => g.Key?.Level ?? -1);
            return result;
        }
        #endregion States queries

        #region Transitions queries
        public IEnumerable<IGrouping<StateBase, Transition>> GetGroupedTransitions()
        {
            var result = Model.Transitions.GroupBy(t => t.SourceState)
            .OrderBy(g => g.Key.Level);
            return result.Where(g => g.Any());
        }

        #endregion Transitions queries
    }
}
