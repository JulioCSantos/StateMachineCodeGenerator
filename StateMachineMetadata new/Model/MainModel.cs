using StateMachineMetadata.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.Model
{
    public class MainModel : EntityBase
    {

        public MainModel(string name, string id) : base(id)
        {
            this.Name = name;
        }

        private string _name;

        public override string Name
        {
            get {
                var name = _name.Replace("Diagram","").Replace("State","").Replace(" ","");
                if (string.IsNullOrEmpty(name)) {name = "UnNamed";}
                return name;
            }
            set => _name = value;
        }

        private List<StateBase> states;
        public List<StateBase> States { get { return states ?? (states = new List<StateBase>()); } }


        private List<Trigger> triggers;
        public List<Trigger> Triggers { get { return triggers ?? (triggers = new List<Trigger>()); } }


        private List<Transition> transitions;
        public List<Transition> Transitions { get { return transitions ?? (transitions = new List<Transition>()); } }

        public List<ErrorMessage> ErrorMessages { get; } = new List<ErrorMessage>();

        #region Methods
        public void AddManufacturedEntities()
        {
            var errMsgs = ErrorMessages;
            // Add StartScan Trigger (event)
            var idleState = States.Where(s => s.Name == "IdleState").FirstOrDefault();
            if (idleState == null) errMsgs.Add(new ErrorMessage("IdleState is missing from Diagram"));
            // Add StartScan Trigger (event)
            var runningState = States.Where(s => s.Name == "RunningState").FirstOrDefault();
            if (runningState == null) errMsgs.Add(new ErrorMessage("RunningState is missing from Diagram"));
            // Add AddStartScan Trigger (event)
            var startScan = States.Where(s => s.Name == "StartScan").FirstOrDefault();
            if (startScan == null) errMsgs.Add(new ErrorMessage("StartScanState is missing from Diagram") { Severity = ErrorSeverity.Warning });

            if (startScan == null && runningState != null) AddStartScanEvent(idleState, runningState);

            // Add missing StopEvent
            if (!Triggers.Where(t => t.Name == "StopEvent").Any())
            {
                var readyState = States.Where(s => s.Name == "ReadyState").FirstOrDefault();
                var faultedState = States.Where(s => s.Name == "FaultedState").FirstOrDefault();
                //if (readyState == null) errMsgs.Add(new ErrorMessage("ReadyState is missing from Diagram") { Severity = ErrorSeverity.Warning });
                if (faultedState == null) errMsgs.Add(new ErrorMessage("FaultedState is missing from Diagram") { Severity = ErrorSeverity.Warning });
                AddStopEvent(readyState, faultedState);
            }
        }

        private void AddStartScanEvent(StateBase idleState, StateBase runningState)
        {
            if (idleState == null || runningState == null) return;
            var startScanEvent = new Trigger("StartScan");
            Triggers.Add(startScanEvent);
            var idleToRunning = new ExternalTransition(Guid.NewGuid().ToString())
            { Source = idleState, Target = runningState, Trigger = startScanEvent };
            Transitions.Add(idleToRunning);
            startScanEvent.TransitionOwner = idleToRunning;
            ErrorMessages.Add(new ErrorMessage("StartScan was manufactured.") { Severity = ErrorSeverity.Warning });
        }

        private void AddStopEvent(StateBase readyState, StateBase faultedState)
        {
            ErrorMessages.Add(new ErrorMessage("StopEvent is missing.") { Severity = ErrorSeverity.Warning });
            var stopEvent = new Trigger("StopEvent");
            Triggers.Add(stopEvent);

            if (readyState == null || faultedState == null) return;
            var readyToFaulted = new ExternalTransition(Guid.NewGuid().ToString())
            { Source = readyState, Target = faultedState, Trigger = stopEvent };
            Transitions.Add(readyToFaulted);
            stopEvent.TransitionOwner = readyToFaulted;
            ErrorMessages.Add(new ErrorMessage("StopEvent was manufactured.") { Severity = ErrorSeverity.Warning });
        }
        #endregion Methods

        #region Parts
        public List<string> EventNames => ExternalTransitions.Where(t => t.Trigger?.Name != null).Select(t => "o" + t.Trigger.Name.ToValidCSharpName()).Distinct().ToList();
        public List<State> TopLevelStates
        {
            get { return States.Where(s => s.OwnerId == null && s.GetType() == typeof(State)).Cast<State>().Distinct().ToList(); }
        }

        public IEnumerable<ExternalTransition> ExternalTransitions => Transitions.OfType<ExternalTransition>();
        public IEnumerable<InternalTransition> InternalTransitions => Transitions.OfType<InternalTransition>();
        public override string ToNSFType()
        {
            return "NSFStateMachine";
        }

        #region Members
        public string DiagramName => Name;
        public string ProjectName => DiagramName.GetCoreName().ToValidCSharpName();
        public string SystemTypeName => ProjectName + ".System";
        //public string SystemNamespace => ProjectName + "." + SystemTypeName;
        public string SystemNamespace => SystemTypeName;
        public string StateMachineTypeName => "C" + ProjectName + "StateMachine";
        public string StateMachineBaseTypeName => StateMachineTypeName + "Base";

        public NSFClauses NSF => new NSFClauses(this);
        #endregion Members

        #endregion Parts

    }
}
