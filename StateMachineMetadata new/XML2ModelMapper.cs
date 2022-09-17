using StateMachineCodeGenerator.Common;
using StateMachineMetadata.Extensions;
using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using ErrorSeverity = StateMachineMetadata.Model.ErrorSeverity;

namespace StateMachineMetadata
{
    public class XML2ModelMapper
    {
        private ParsedXmlFile xml;
        private MainModel model;
        public Dictionary<string, EntityBase> ElementsDictionary = new Dictionary<string, EntityBase>();

        public readonly ErrorLog XML2ModelMapperErr1 = new ErrorLog(nameof(XML2ModelMapperErr1),
            "Parent/owner not found for state {0}.", StateMachineCodeGenerator.Common.ErrorSeverity.Warning);

        public void Map(ParsedXmlFile parsedXmlFile, MainModel mainModel)
        {
            xml = parsedXmlFile;
            model = mainModel;

            MapStates();
            MapTransitions();
            MapComments();

            //// TROUBLESHOOTING SPOT
            //var trans = model.Transitions.FirstOrDefault(t => t.Id == "EAID_714A9E4C_4FC9_48c9_BDDE_704ED9E16BF6");
            //var transitionWithMultipleAction = model.Transitions.Where(t => t.Actions != null && t.Actions.Count() > 1);
        }

        #region MapCommentsToInternalTransitions

        private void MapComments()
        {
            foreach (var xComm in xml.ActiveCommentsElems)
            {
                var documentation = xComm.Descendants().Where(d => d.Attribute("tag")?.Value == "documentation").FirstOrDefault()?.Attribute("value").Value;
                if (documentation == null) continue;
                //if (documentation.Contains("BypassKeyEnabledEvent [IsNotOperatorMode]")) System.Diagnostics.Debugger.Break();
                var tags = xComm.Descendants().Where(x => x.Attribute("tag")?.Value == "ea_stype");
                var values = xComm.Descendants().Where(x => x.Attribute("value")?.Value == "Note");
                var documentationHasCommentsOnly = xComm.Descendants().Where(x => x.Attribute("tag")?.Value == "ea_stype" && x.Attribute("value")?.Value == "Note").Any();
                if (documentationHasCommentsOnly) continue;
                var transt = new InternalTransition(xComm.GetId(), documentation, documentationHasCommentsOnly);
                transt.Map(xComm);
                ConnectInternalTransitionsToOwnerState(transt);
                if (transt.Trigger == null || transt.OwnerState == null) {
                    throw new Exception(nameof(transt.Trigger) + " or " + nameof(transt.OwnerState) + " is null.");
                }
                ElementsDictionary.Add(transt.Id, transt);
                model.Transitions.Add(transt);
                if (transt.Trigger != null) model.Triggers.Add(transt.Trigger);
            }
            //ConnectInternalTransitionsToOwnerState();
        }

        private void ConnectInternalTransitionsToOwnerState(InternalTransition intTrans)
        {
            //foreach (var intTrans in model.InternalTransitions)
            //{
                if (intTrans.OwnerId != null) intTrans.OwnerState = model.States.OfType<State>().Where(s => s.Id == intTrans.OwnerId).FirstOrDefault();
                else ConnectInternalTransitionToOuterState(intTrans);
            //}
        }

        private void ConnectInternalTransitionToOuterState(InternalTransition intTrans)
        {
            var intTransDrawnElem = xml.DrawnElems.Where(de => de.GetSubject() == intTrans.Id).FirstOrDefault();
            if (intTransDrawnElem == null)
                intTransDrawnElem = xml.ContentElem.Descendants().Where(de => de.GetSubject() == intTrans.Id).FirstOrDefault();
            if (intTransDrawnElem == null) { throw new Exception("DiagramElement (geometry) not found"); } //DiagramElement (geometry) not found anywhere
            var intTransCoord = (Coordinates)intTransDrawnElem.ToCoordinates();

            System.Xml.Linq.XElement container = null;
            foreach (var drawn in xml.DrawnElems)
            {
                if (drawn.GetSubject() == intTrans.Id) continue; // Container geometry must be different than InternalTransition (Comment XML Element). 
                var drawnCoord = drawn.ToCoordinates();
                if (drawnCoord == null) continue;
                if (((Coordinates)drawnCoord).Contains(intTransCoord))
                {
                    // if the next 'drawn' element has smaller area use it as a container instead
                    if (container == null || ((Coordinates)container.ToCoordinates()).Area > ((Coordinates)drawnCoord).Area)
                        container = drawn;
                }
                if (container == null) continue;
                intTrans.OwnerState = model.States.FirstOrDefault(s => s.Id == container.GetSubject() && s.GetType() == typeof(State)) as State;
            }

            if (intTrans.OwnerState == null) {
                throw new Exception("Owner State not found for " + intTrans.Name);
            } //Owner State not found for this Comment/InternalTransition
        }

        #endregion MapCommentsToInternalTransitions


        #region Map States from XML to Model

        private Coordinates? GetCoordinatesForState(string strId)
        {
            // Match the ID in the DrawnElem collection 
            var stateDrawnElem = xml.DrawnElems.Where(de => de.GetSubject() == strId).FirstOrDefault();
            
            // Return the coordinates if the state was found
            Coordinates? oCoord = stateDrawnElem != null ? stateDrawnElem.ToCoordinates() : null;
            return oCoord;
        }

        private void MapStates()
        {
            foreach (var xState in xml.ActiveStatesElems)
            {
                StateBase state = null;
                switch (xState.Name.LocalName)
                {
                    case "PseudoState":
                        if (xState.Attribute("kind")?.Value == "initial") state = new InitialState(xState.GetId());
                        else state = new ChoiceState(xState.GetId());
                        break;
                    case "SimpleState":
                    case "CompositeState":
                        state = new State(xState.GetId());
                        break;
                    default:
                        throw new Exception("Not valid State type");
                }
                // Capture the Coordinate if it exists
                Coordinates? oCoord = GetCoordinatesForState(xState.GetId());
                if (oCoord != null)
                    state.Coord = (Coordinates)oCoord;

                if (ElementsDictionary.ContainsKey(state.Id)) throw new Exception("Duplicated state Id");
                state.Map(xState, ElementsDictionary);
                ElementsDictionary.Add(state.Id, state);
                // Reject Entry/Exit State Actions from the model. They will be connected as properties to the Owner state
                if (state.stype == "StateNode" && (state.ntype == 13 || state.ntype == 14)) continue;
                else model.States.Add(state);
            }

            ConnectToOwnerStates();
            ConnectStateActionsToOwnerState();
            QualifyDuplicatedNames();
        }

        private void QualifyDuplicatedNames()
        {
            var duplicatedNames = model.States.GroupBy(s => s.Name).Where(g => g.Count() > 1);
            foreach (var dupState in duplicatedNames.SelectMany(g => g))
            {
                if (dupState.Owner == null) continue;
                dupState.Name = dupState.Owner.Name.ToValidCSharpName().Replace("State", "") + "_" + dupState.Name;
                if (!dupState.Name.EndsWith("State")) dupState.Name += "State";
            }
        }

        private void ConnectToOwnerStates() {
            var children = ElementsDictionary.Values.Cast<StateBase>().Where(s => !string.IsNullOrEmpty(s.OwnerId));
            foreach (var stateWithParent in children)
            {
                //if (stateWithParent.OwnerId == null) {System.Diagnostics.Debugger.Break();}
                //if (stateWithParent.Name == "SystemPoweredState") { System.Diagnostics.Debugger.Break(); }
                if (ElementsDictionary.ContainsKey(stateWithParent.OwnerId))
                {
                    stateWithParent.Owner = ElementsDictionary[stateWithParent.OwnerId] as State;
                    if (!stateWithParent.Owner.Children.Contains(stateWithParent))
                        stateWithParent.Owner.Children.Add(stateWithParent);
                }

                //if (stateWithParent.Owner == null)
                //{

                //    var e1 = xml.PrimaryElems.Where(pe => pe.GetId() == stateWithParent.OwnerId);
                //    var e2 = xml.DrawnElems.Where(ae => ae.Attribute("subject")?.Value == stateWithParent.OwnerId);

                //    if (e1.Any() && !e2.Any())
                //    {
                //        var errorMsg = $"Parent/Owner not found. State:'{stateWithParent.Name}/{stateWithParent.Id}', Owner:'{e1.First().Attribute("name").Value}/{e1.First().GetId()}' is missing in Diagram '{model.DiagramName}'";
                //        //if (Debugger.IsAttached) Debugger.Break();
                //    }
                //}
                //else if (stateWithParent.Owner == null)
                //{
                //    throw new Exception("Parent/Owner not found");
                //}
            }
            var orphans = ElementsDictionary.Values.Cast<StateBase>().Where(s => string.IsNullOrEmpty(s.OwnerId));
            foreach (StateBase orphanState in orphans) {
                var orphanStateName = $"'{orphanState.Name}/{orphanState.Id}'";
                var err = ErrorLog.GetEditedErrorLog(XML2ModelMapperErr1.Id, new object[] { orphanStateName });
                XPLogger.Instance.AddError(err);
                //var errorMsg = $"Parent(Owner) not found for State: '{orphanState.Name}/{orphanState.Id}'";

                //throw new Exception("Parent/Owner not found");
            }
        }

        private void ConnectStateActionsToOwnerState()
        {
            var EntryExitActionStates = ElementsDictionary.Values.Cast<StateBase>().Where(s => !string.IsNullOrEmpty(s.OwnerId) && s.stype == "StateNode" && (s.ntype == 13 || s.ntype == 14)).ToList();
            foreach (var state in EntryExitActionStates)
            {
                //if (state.OrigName.Contains("SetMeasuring Annunciator")) System.Diagnostics.Debugger.Break();
                if (state.ntype == 13) state.Owner.EntryActions.AddRange(state.OrigName
                        .Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToValidCSharpName())); 
                if (state.ntype == 14) state.Owner.ExitActions.AddRange(state.OrigName
                        .Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToValidCSharpName()));
            }
        }
        #endregion Map States from XML to Model


        #region Map Transitions from XML to Model
        private void MapTransitions()
        {
            foreach (var xEvent in xml.ActiveTransitionsElems)
            {
                var transId = xEvent.GetId();
                //// TROUBLESHOOTING SPOT
                //if (transId == "EAID_714A9E4C_4FC9_48c9_BDDE_704ED9E16BF6") System.Diagnostics.Debugger.Break();
                var transt = new ExternalTransition(transId);
                transt.Map(xEvent, ElementsDictionary);
                ElementsDictionary.Add(transt.Id, transt);
                model.Transitions.Add(transt);
                if (transt.Trigger != null) model.Triggers.Add(transt.Trigger);
            }
            ConnectTransitions();
            ManufactureMissingActions();
            SetActionNames();
            if (model.Transitions.OfType<ExternalTransition>().Any(t => t.Source == null || t.Target == null)) {
                throw new Exception("MapTransitions failed"); };
        }


        private void ConnectTransitions()
        {
            var parsedTransitions = ElementsDictionary.Where(e => e.Value.GetType() == typeof(ExternalTransition)).Select(e => e.Value as ExternalTransition);
            foreach (var outT in parsedTransitions)
            {
                //if (outT.Name.StartsWith("WaitForManualLoad")) System.Diagnostics.Debugger.Break();
                var parents = parsedTransitions.Where(inT => outT.Source == inT.Target);
                outT.ParentTransitions.AddRange(parents);

                var children = parsedTransitions.Where(inT => outT.Target == inT.Source);
                outT.ChildrenTransitions.AddRange(children);
            }
        }

        private void ManufactureMissingActions()
        {
            var initToInitAfterSettingsTranstion = model.ExternalTransitions.FirstOrDefault(t => t.Source.Name == "InitState" && t.Target.Name == "InitAfterSettingsState");
            if (initToInitAfterSettingsTranstion == null)
                model.ErrorMessages.Add(new ErrorMessage("'InitToInitAfterSettings' transition is missing") { Severity = ErrorSeverity.Warning });
            else
            {
                if (string.IsNullOrEmpty(initToInitAfterSettingsTranstion.ActionName))
                    initToInitAfterSettingsTranstion.ActionName = "InitAfterInitSettingsAction";
            }
        }

        private void SetActionNames()
        {
            var transitionWithMultipleAction = model.Transitions.Where(t => t.Actions != null && t.Actions.Count() > 1);
            foreach (var transition in transitionWithMultipleAction)
            {
                transition.ActionName = transition.Name.Replace("Transition", "") + "Action";
            }
        }
        #endregion Map Transitions from XML to Model

    }
}
