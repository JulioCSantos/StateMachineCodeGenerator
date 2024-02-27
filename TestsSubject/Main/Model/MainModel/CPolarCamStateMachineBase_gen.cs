 
 
 
// Created by t4 template 'StateMachineBaseTemplate'
///////////////////////////////////////////////////////////
// Copyright © Corning Incorporated 2017
// File CPolarCamStateMachineBase_gen.cs
// Project PolarCam
// Implementation of the Class CPolarCamStateMachineBase
// Created on 2/27/2024 1:48:23 PM
///////////////////////////////////////////////////////////

// Quick Start Guide for those using the GenSys Easy for the first time:
// First time users will primarily be interested in three states: Ready, Idle, and Running. The Idle and Running states are sub-states of the Ready State.
// The Ready state is entered after initialization and then the State Machine proceeds to the Idle State.
// The Idle state is intended to be a "waiting" state with nothing significant happening.
// The Running state is intended to be the state where your task is accomplished.


// Here are the rules for the Ready, Idle and Running States
// The method "ReadyStateEntryActions"   is called after initializtion has completed.
// The method "ReadyStateExitActions"    is called after if a Fault occurs.
// The method "IdleStateEntryActions"    is called after either "ReadyStateEntryActions" or "RunningStateExitActions" is called.
// The method "IdleStateExitActions"     is called when the Start button is pressed.
// The method "RunningStateEntryActions" is called when the Start button is pressed but after "IdleStateExitActions" is called.
// The method "RunningStateExitActions"  is called when the Abort button is pressed.


// To start developing an Application utilizing a Start and Abort button:
// Place Initialization Code in "ReadyStateEntryActions". A call into the sytem may be needed.
// Place code to call into system to perform your task in "RunningStateEntryActions".
// If a System thread was kicked off to start a task, then place a call in "RunningStateExitActions" to call into the system to stop the thread.

// To start developing an application which doesn't require user interaction and just performs a task, place code to call into the System in "IdleStateEntryActions" and
// uncomment "m_iShield.EnableStartButton = false; "  in "IdleStateEntryActions".
// This default template has code to show how to use the Start and Abort button in conjunction with calling into the System, which uses the ThreadHandler.
// When Start is pressed a message will be display every second until one of two conditions are met:
// 20 Messages have been displayed or
// The Abort button is pressed.

// It is expected that this and the System code will be heavily modified.
// Note that the State Machine uses System to do all of the real work.
// The desired pattern is the State Machine not implementing any Application code.

// The method CreateOperatorPrompts() creates GUI prompts based on the state of the State Machine.
// As the State Machine state changes, so does the GUI prompt.
// The following two statement are included in the code as part of the Quick Start Guide and may be tailored for your application.
// m_odictOperatorPromptForState.Add(oIdleState, "Hit Start to Start Running");
// m_odictOperatorPromptForState.Add(oRunningState, "Hit Abort to Stop Running");

using System;
using System.Windows;
using System.Drawing;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using Corning.GenSys.Logger;
using NorthStateSoftware.NorthStateFramework;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Corning.GenSys.Scanning;
using GenSysCommon;
using GenSysCommon.Interfaces;
using NSFEventName = System.String;

// ReSharper disable InconsistentNaming
namespace PolarCam.Model
{
    public partial class CPolarCamStateMachineBase : NSFStateMachine 
    {
        #region Fields and Properties
        internal static ILogger ms_iLogger = CLoggerFactory.CreateLog("CPolarCamStateMachine");
        public const string StateMachineName = "PolarCam";
        //private object m_objLock = new object();

        public Dictionary<NSFEventName, NSFEvent> Triggers { get; } = new Dictionary<NSFEventName, NSFEvent>();
        public string SystemState {get; private set;}
        public EState? CurrentState { get; private set; }
        public EState? PreviousState { get; private set; }
        public NSFStateMachineContext CurrentContext { get; private set; }
        public NSFStateMachineContext PreviousContext { get; private set; }

        //private bool m_bSimulationMode = false;

        #region OperatorPromptForStateDict
        private Dictionary<EState, (string OperatorPrompt, bool HighlightPrompt)> m_odictOperatorPromptForState;
        public Dictionary<EState, (string OperatorPrompt, bool HighlightPrompt)> OperatorPromptForStateDict
            { get { return m_odictOperatorPromptForState ?? (m_odictOperatorPromptForState = new Dictionary<EState, (string OperatorPrompt, bool HighlightPrompt)>() ); } }
        #endregion OperatorPromptForStateDict
       
        public string LastOperatorPrompt { get; private set; }
		public virtual string UndefinedFaultEvent { get; } = TriggerName.UndefinedFaultEvent;
        public CPolarCamModel MainModel {get; }
        
        private StateChangesEventArgs _stateChangesEventArgs;
        public StateChangesEventArgs StateChangesEventArgs
        {
            get { return _stateChangesEventArgs ?? (_stateChangesEventArgs = new StateChangesEventArgs( /*CurrentContext, MetaData*/ )); }
            //set { _stateChangesEventArgs = value; }
        }
        #endregion Fields and Properties

        #region Constructors
        protected internal CPolarCamStateMachineBase(string strName, CPolarCamModel mainModel) 
			: base(strName, new NSFEventThread(strName)) 
        { 
            MainModel = mainModel; 
            CreateStateMachine();
            //MetaData = GetMetadata();
        }

        //private StateMachineMetaData _metaData;
        //public StateMachineMetaData MetaData
        //{
            //get { return _metaData ?? (_metaData = new StateMachineMetaData()); }
            //set { _metaData = value; }
        //}


        private StateMachineMetaData GetMetadata()
        {
            var metaData = new StateMachineMetaData();
            var triggersInfo = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(pi => pi.FieldType == typeof(NSFEvent)).ToList();

            foreach (var triggerInfo in triggersInfo)
            {
                var nEvent = triggerInfo.GetValue(this) as NSFEvent;
                var trigger = new TriggerEvent(nEvent);
                metaData.TriggerEvents.Add(trigger.Name, trigger);
            }

            var statesInfo = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(pi => pi.FieldType == typeof(NSFInitialState) || pi.FieldType == typeof(NSFCompositeState) || pi.FieldType == typeof(NSFChoiceState)).ToList();


            foreach (var stateInfo in statesInfo)
            {
                var nState = stateInfo.GetValue(this) as NSFState;
                StateBase state = null;
                if (stateInfo.FieldType == typeof(NSFCompositeState)) state = new CompositeState(nState as NSFCompositeState);
                if (stateInfo.FieldType == typeof(NSFInitialState)) state = new InitialState(nState as NSFInitialState);
                if (stateInfo.FieldType == typeof(NSFChoiceState)) state = new ChoiceState(nState as NSFChoiceState);
                if (state != null) metaData.States.Add(state.Name, state);
                else System.Diagnostics.Debugger.Break();
            }

            var transitionsInfo = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(pi => pi.PropertyType == typeof(NSFExternalTransition) || pi.PropertyType == typeof(NSFInternalTransition)).ToList();

            foreach (var transInfo in transitionsInfo)
            {
                var nTrans = transInfo.GetValue(this) as NSFTransition;
                TransitionBase trans = null;
                if (transInfo.PropertyType == typeof(NSFExternalTransition))
                {
                    trans = new ExternalTransition(nTrans as NSFExternalTransition);
                    var extTrans = trans as ExternalTransition;
                    extTrans.SourceState = metaData.States[nTrans.Source.Name] as CompositeState;
                    extTrans.TargetState = metaData.States[nTrans.Target.Name] as CompositeState;
                    //if (extTrans.SourceState == null || extTrans.TargetState == null) System.Diagnostics.Debugger.Break();
                }

                if (transInfo.PropertyType == typeof(NSFInternalTransition))
                {
                    trans = new InternalTransition(nTrans as NSFInternalTransition);
                    var intTrans = trans as InternalTransition;
                    if (intTrans.State != null)
                        intTrans.State = metaData.States[intTrans.State.Name] as CompositeState;
                    //else System.Diagnostics.Debugger.Break();
                }

                if (trans == null) System.Diagnostics.Debugger.Break();
                else {
                    if (nTrans.Triggers.Any())
                        trans.TriggerEvent = metaData.TriggerEvents[nTrans.Triggers.First().Name];
                    //else System.Diagnostics.Debugger.Break();
                    metaData.Transitions.Add(trans.Name, trans);
                }
            }

            return metaData;
        }
        #endregion Constructors

		#region Events
		public NSFEvent oAbortEvent;
		public NSFEvent oCameraFaultEvent;
		public NSFEvent oCameraViewEvent;
		public NSFEvent oClearFaultsEvent;
		public NSFEvent oEStopEvent;
		public NSFEvent oHaltAcquisitionEvent;
		public NSFEvent oImagesAcquiredEvent;
		public NSFEvent oInitAfterSettingsCompleteEvent;
		public NSFEvent oInitCompleteEvent;
		public NSFEvent oInitEvent;
		public NSFEvent oLeaveCameraViewEvent;
		public NSFEvent oLoadBatchFileEvent;
		public NSFEvent oLoadLayerTiffEvent;
		public NSFEvent oLoadSingleFileEvent;
		public NSFEvent oManualProcCompleteEvent;
		public NSFEvent oMotionCompleteEvent;
		public NSFEvent oOpenSensorCompleteEvent;
		public NSFEvent oReadLayerTiffCompleteEvent;
		public NSFEvent oReprocBatchEvent;
		public NSFEvent oSetEnvCompleteEvent;
		public NSFEvent oSettingsFaultEvent;
		public NSFEvent oStartAcquisitionEvent;
		public NSFEvent oStartEvent;
		public NSFEvent oUndefinedFaultEvent;
		#endregion Events


		#region States
        // root state definition
        public NSFChoiceState       oIsBatchModeOnChoiceState;
        public NSFCompositeState    oSystemPoweredState;
        // SystemPoweredState's substate definitions
        public NSFCompositeState    oProcessingState;
        public NSFCompositeState    oCameraViewState;
        public NSFCompositeState    oReadyState;
        public NSFChoiceState       oIsCriticalAlarmChoiceState;
        public NSFCompositeState    oFaultParentState;
        public NSFCompositeState    oInitAfterSettingsState;
        public NSFInitialState      oSystemPowered_InitialState;
        public NSFCompositeState    oInitState;
        // ReadyState's substate definitions
        public NSFCompositeState    oReviewProcState;
        public NSFCompositeState    oManualProcessImageState;
        public NSFCompositeState    oReadTiffLayerState;
        public NSFInitialState      oReady_InitialState;
        public NSFCompositeState    oIdleState;
        // ProcessingState's substate definitions
        public NSFCompositeState    oOpenSensorState;
        public NSFCompositeState    oAbortingState;
        public NSFChoiceState       oIsMotionChoiceState;
        public NSFChoiceState       oIsEnvSettingChoiceState;
        public NSFCompositeState    oSetEnvironmentState;
        public NSFCompositeState    oMotionState;
        public NSFChoiceState       oIsOperatorRequestChoiceState;
        public NSFCompositeState    oOperatorRequestState;
        public NSFChoiceState       oIsMoreImagesChoiceState;
        public NSFCompositeState    oAcquisitionState;
        public NSFInitialState      oProcessing_InitialState;
        // CameraViewState's substate definitions
        public NSFCompositeState    oCameraViewIdleState;
        public NSFInitialState      oCameraView_InitialState;
        public NSFCompositeState    oCameraViewActiveState;
        // FaultParentState's substate definitions
        public NSFInitialState      oFaultParent_InitialState;
        public NSFCompositeState    oFaultedState;
        
        #endregion States

        #region StateLocations

        private Dictionary<string, Rectangle> dictStateLocation = new Dictionary<string, Rectangle>{

        // root state definition
         {"IsBatchModeOnChoiceState", new Rectangle(1873,732,26,34)}, 
         {"SystemPoweredState", new Rectangle(172,25,2569,1836)}, 
         // SystemPoweredState's substates
         {"ProcessingState", new Rectangle(911,946,1354,866)}, 
         {"CameraViewState", new Rectangle(1664,80,752,348)}, 
         {"ReadyState", new Rectangle(917,485,1393,422)}, 
         {"IsCriticalAlarmChoiceState", new Rectangle(563,1043,26,34)}, 
         {"FaultParentState", new Rectangle(498,1154,288,345)}, 
         {"InitAfterSettingsState", new Rectangle(478,736,291,110)}, 
         {"SystemPowered_InitialState", new Rectangle(249,632,20,20)}, 
         {"InitState", new Rectangle(528,605,223,75)}, 
         // ReadyState's substates
         {"ReviewProcState", new Rectangle(1976,661,242,196)}, 
         {"ManualProcessImageState", new Rectangle(1490,732,314,124)}, 
         {"ReadTiffLayerState", new Rectangle(1485,546,314,115)}, 
         {"Ready_InitialState", new Rectangle(952,514,20,20)}, 
         {"IdleState", new Rectangle(997,559,314,115)}, 
         // ProcessingState's substates
         {"OpenSensorState", new Rectangle(1178,959,286,106)}, 
         {"AbortingState", new Rectangle(1873,1257,361,106)}, 
         {"IsMotionChoiceState", new Rectangle(1099,1294,26,34)}, 
         {"IsEnvSettingChoiceState", new Rectangle(1099,1466,26,34)}, 
         {"SetEnvironmentState", new Rectangle(1171,1430,348,105)}, 
         {"MotionState", new Rectangle(1170,1257,353,106)}, 
         {"IsOperatorRequestChoiceState", new Rectangle(1098,1123,26,34)}, 
         {"OperatorRequestState", new Rectangle(1178,1093,345,107)}, 
         {"IsMoreImagesChoiceState", new Rectangle(950,1656,26,34)}, 
         {"AcquisitionState", new Rectangle(1178,1585,319,105)}, 
         {"Processing_InitialState", new Rectangle(1021,974,20,20)}, 
         // CameraViewState's substates
         {"CameraViewIdleState", new Rectangle(1803,270,491,98)}, 
         {"CameraView_InitialState", new Rectangle(1684,109,20,20)}, 
         {"CameraViewActiveState", new Rectangle(1794,139,551,94)}, 
         // FaultParentState's substates
         {"FaultParent_InitialState", new Rectangle(515,1198,20,20)}, 
         {"FaultedState", new Rectangle(554,1226,216,121)}, 
        {"LastState", new Rectangle(1, 2, 3, 4)}};

        public Dictionary<string, Rectangle> DictStateLocation
        {
            get => dictStateLocation;
            set => dictStateLocation = value;
        }
		#endregion StateLocations

		#region Transitions
        // From IsBatchModeOnChoiceState Transitions' definitions
        public NSFExternalTransition IsBatchModeOnChoiceTOReviewProcTransition {get; private set;}

        // From SystemPoweredState Transitions' definitions
        public NSFExternalTransition SystemPoweredTOFaultParentTransitionBYUndefinedFaultEvent {get; private set;}
        public NSFExternalTransition SystemPoweredTOFaultParentTransitionBYEStopEvent {get; private set;}
        public NSFExternalTransition SystemPoweredTOFaultParentTransitionBYSettingsFaultEvent {get; private set;}
        public NSFExternalTransition SystemPoweredTOFaultParentTransitionBYCameraFaultEvent {get; private set;}
        public NSFExternalTransition SystemPoweredTOInitTransitionBYInitEvent {get; private set;}

        // From IsCriticalAlarmChoiceState Transitions' definitions
        public NSFExternalTransition IsCriticalAlarmChoiceTOReadyTransition {get; private set;}
        public NSFExternalTransition IsCriticalAlarmChoiceTOFaultParentTransition {get; private set;}

        // From InitState Transitions' definitions
        public NSFExternalTransition InitTOInitAfterSettingsTransitionBYInitCompleteEvent {get; private set;}

        // From ReadyState Transitions' definitions
        public NSFExternalTransition ReadyTOIdleTransitionBYAbortEvent {get; private set;}
        public NSFExternalTransition ReadyTOCameraViewTransitionBYCameraViewEvent {get; private set;}

        // From SystemPowered_InitialState Transitions' definitions
        public NSFExternalTransition SystemPowered_InitialTOInitTransition {get; private set;}

        // From ProcessingState Transitions' definitions
        public NSFExternalTransition ProcessingTOReadyTransitionBYAbortEvent {get; private set;}

        // From FaultParentState Transitions' definitions
        public NSFExternalTransition FaultParentTOIsCriticalAlarmChoiceTransitionBYClearFaultsEvent {get; private set;}
        public NSFInternalTransition FaultParentStateOnAbortEventTransition {get; private set;}

        // From InitAfterSettingsState Transitions' definitions
        public NSFExternalTransition InitAfterSettingsTOIsCriticalAlarmChoiceTransitionBYInitAfterSettingsCompleteEvent {get; private set;}

        // From CameraViewState Transitions' definitions
        public NSFExternalTransition CameraViewTOReadyTransitionBYLeaveCameraViewEvent {get; private set;}

        // From ReviewProcState Transitions' definitions
        public NSFExternalTransition ReviewProcTOProcessingTransitionBYStartEvent {get; private set;}
        public NSFExternalTransition ReviewProcTOReadTiffLayerTransitionBYLoadLayerTiffEvent {get; private set;}
        public NSFExternalTransition ReviewProcTOManualProcessImageTransitionBYLoadSingleFileEvent {get; private set;}
        public NSFLocalTransition ReviewProcTOReviewProcTransitionBYReprocBatchEvent {get; private set;}
        public NSFExternalTransition ReviewProcTOManualProcessImageTransitionBYLoadBatchFileEvent {get; private set;}

        // From ManualProcessImageState Transitions' definitions
        public NSFExternalTransition ManualProcessImageTOIsBatchModeOnChoiceTransitionBYAbortEvent {get; private set;}
        public NSFExternalTransition ManualProcessImageTOIsBatchModeOnChoiceTransitionBYManualProcCompleteEvent {get; private set;}

        // From FaultParent_InitialState Transitions' definitions
        public NSFExternalTransition FaultParent_InitialTOFaultedTransition {get; private set;}

        // From Ready_InitialState Transitions' definitions
        public NSFExternalTransition Ready_InitialTOIdleTransition {get; private set;}

        // From IdleState Transitions' definitions
        public NSFExternalTransition IdleTOManualProcessImageTransitionBYLoadSingleFileEvent {get; private set;}
        public NSFExternalTransition IdleTOManualProcessImageTransitionBYLoadBatchFileEvent {get; private set;}
        public NSFExternalTransition IdleTOReadTiffLayerTransitionBYLoadLayerTiffEvent {get; private set;}
        public NSFExternalTransition IdleTOProcessingTransitionBYStartEvent {get; private set;}
        public NSFInternalTransition IdleStateOnAbortEventTransition {get; private set;}

        // From ReadTiffLayerState Transitions' definitions
        public NSFExternalTransition ReadTiffLayerTOManualProcessImageTransitionBYReadLayerTiffCompleteEvent {get; private set;}

        // From IsMoreImagesChoiceState Transitions' definitions
        public NSFExternalTransition IsMoreImagesChoiceTOReadyTransition {get; private set;}
        public NSFExternalTransition IsMoreImagesChoiceTOIsOperatorRequestChoiceTransition {get; private set;}

        // From OpenSensorState Transitions' definitions
        public NSFExternalTransition OpenSensorTOIsOperatorRequestChoiceTransitionBYOpenSensorCompleteEvent {get; private set;}

        // From IsOperatorRequestChoiceState Transitions' definitions
        public NSFExternalTransition IsOperatorRequestChoiceTOIsMotionChoiceTransition {get; private set;}
        public NSFExternalTransition IsOperatorRequestChoiceTOOperatorRequestTransition {get; private set;}

        // From AbortingState Transitions' definitions
        public NSFExternalTransition AbortingTOReadyTransitionBYMotionCompleteEvent {get; private set;}
        public NSFInternalTransition AbortingStateOnAbortEventTransition {get; private set;}

        // From MotionState Transitions' definitions
        public NSFExternalTransition MotionTOAbortingTransitionBYAbortEvent {get; private set;}
        public NSFExternalTransition MotionTOIsEnvSettingChoiceTransitionBYMotionCompleteEvent {get; private set;}

        // From IsMotionChoiceState Transitions' definitions
        public NSFExternalTransition IsMotionChoiceTOMotionTransition {get; private set;}
        public NSFExternalTransition IsMotionChoiceTOIsEnvSettingChoiceTransition {get; private set;}

        // From OperatorRequestState Transitions' definitions
        public NSFExternalTransition OperatorRequestTOIsMotionChoiceTransitionBYStartEvent {get; private set;}

        // From Processing_InitialState Transitions' definitions
        public NSFExternalTransition Processing_InitialTOOpenSensorTransition {get; private set;}

        // From AcquisitionState Transitions' definitions
        public NSFExternalTransition AcquisitionTOIsMoreImagesChoiceTransitionBYImagesAcquiredEvent {get; private set;}

        // From SetEnvironmentState Transitions' definitions
        public NSFExternalTransition SetEnvironmentTOAcquisitionTransitionBYSetEnvCompleteEvent {get; private set;}

        // From IsEnvSettingChoiceState Transitions' definitions
        public NSFExternalTransition IsEnvSettingChoiceTOSetEnvironmentTransition {get; private set;}
        public NSFExternalTransition IsEnvSettingChoiceTOAcquisitionTransition {get; private set;}

        // From CameraViewActiveState Transitions' definitions
        public NSFExternalTransition CameraViewActiveTOCameraViewIdleTransitionBYCameraFaultEvent {get; private set;}
        public NSFExternalTransition CameraViewActiveTOCameraViewIdleTransitionBYHaltAcquisitionEvent {get; private set;}

        // From CameraViewIdleState Transitions' definitions
        public NSFExternalTransition CameraViewIdleTOCameraViewActiveTransitionBYStartAcquisitionEvent {get; private set;}

        // From CameraView_InitialState Transitions' definitions
        public NSFExternalTransition CameraView_InitialTOCameraViewActiveTransition {get; private set;}

		#endregion Transitions

        private void CreateStateMachine()
        {

            BeforeCreateStateMachine();

		    #region Events  instantiations
		    oAbortEvent = new NSFEvent(nameof(oAbortEvent), this, this);
		    oCameraFaultEvent = new NSFEvent(nameof(oCameraFaultEvent), this, this);
		    oCameraViewEvent = new NSFEvent(nameof(oCameraViewEvent), this, this);
		    oClearFaultsEvent = new NSFEvent(nameof(oClearFaultsEvent), this, this);
		    oEStopEvent = new NSFEvent(nameof(oEStopEvent), this, this);
		    oHaltAcquisitionEvent = new NSFEvent(nameof(oHaltAcquisitionEvent), this, this);
		    oImagesAcquiredEvent = new NSFEvent(nameof(oImagesAcquiredEvent), this, this);
		    oInitAfterSettingsCompleteEvent = new NSFEvent(nameof(oInitAfterSettingsCompleteEvent), this, this);
		    oInitCompleteEvent = new NSFEvent(nameof(oInitCompleteEvent), this, this);
		    oInitEvent = new NSFEvent(nameof(oInitEvent), this, this);
		    oLeaveCameraViewEvent = new NSFEvent(nameof(oLeaveCameraViewEvent), this, this);
		    oLoadBatchFileEvent = new NSFEvent(nameof(oLoadBatchFileEvent), this, this);
		    oLoadLayerTiffEvent = new NSFEvent(nameof(oLoadLayerTiffEvent), this, this);
		    oLoadSingleFileEvent = new NSFEvent(nameof(oLoadSingleFileEvent), this, this);
		    oManualProcCompleteEvent = new NSFEvent(nameof(oManualProcCompleteEvent), this, this);
		    oMotionCompleteEvent = new NSFEvent(nameof(oMotionCompleteEvent), this, this);
		    oOpenSensorCompleteEvent = new NSFEvent(nameof(oOpenSensorCompleteEvent), this, this);
		    oReadLayerTiffCompleteEvent = new NSFEvent(nameof(oReadLayerTiffCompleteEvent), this, this);
		    oReprocBatchEvent = new NSFEvent(nameof(oReprocBatchEvent), this, this);
		    oSetEnvCompleteEvent = new NSFEvent(nameof(oSetEnvCompleteEvent), this, this);
		    oSettingsFaultEvent = new NSFEvent(nameof(oSettingsFaultEvent), this, this);
		    oStartAcquisitionEvent = new NSFEvent(nameof(oStartAcquisitionEvent), this, this);
		    oStartEvent = new NSFEvent(nameof(oStartEvent), this, this);
		    oUndefinedFaultEvent = new NSFEvent(nameof(oUndefinedFaultEvent), this, this);

            Triggers.Add(TriggerName.AbortEvent, oAbortEvent);
            Triggers.Add(TriggerName.CameraFaultEvent, oCameraFaultEvent);
            Triggers.Add(TriggerName.CameraViewEvent, oCameraViewEvent);
            Triggers.Add(TriggerName.ClearFaultsEvent, oClearFaultsEvent);
            Triggers.Add(TriggerName.EStopEvent, oEStopEvent);
            Triggers.Add(TriggerName.HaltAcquisitionEvent, oHaltAcquisitionEvent);
            Triggers.Add(TriggerName.ImagesAcquiredEvent, oImagesAcquiredEvent);
            Triggers.Add(TriggerName.InitAfterSettingsCompleteEvent, oInitAfterSettingsCompleteEvent);
            Triggers.Add(TriggerName.InitCompleteEvent, oInitCompleteEvent);
            Triggers.Add(TriggerName.InitEvent, oInitEvent);
            Triggers.Add(TriggerName.LeaveCameraViewEvent, oLeaveCameraViewEvent);
            Triggers.Add(TriggerName.LoadBatchFileEvent, oLoadBatchFileEvent);
            Triggers.Add(TriggerName.LoadLayerTiffEvent, oLoadLayerTiffEvent);
            Triggers.Add(TriggerName.LoadSingleFileEvent, oLoadSingleFileEvent);
            Triggers.Add(TriggerName.ManualProcCompleteEvent, oManualProcCompleteEvent);
            Triggers.Add(TriggerName.MotionCompleteEvent, oMotionCompleteEvent);
            Triggers.Add(TriggerName.OpenSensorCompleteEvent, oOpenSensorCompleteEvent);
            Triggers.Add(TriggerName.ReadLayerTiffCompleteEvent, oReadLayerTiffCompleteEvent);
            Triggers.Add(TriggerName.ReprocBatchEvent, oReprocBatchEvent);
            Triggers.Add(TriggerName.SetEnvCompleteEvent, oSetEnvCompleteEvent);
            Triggers.Add(TriggerName.SettingsFaultEvent, oSettingsFaultEvent);
            Triggers.Add(TriggerName.StartAcquisitionEvent, oStartAcquisitionEvent);
            Triggers.Add(TriggerName.StartEvent, oStartEvent);
            Triggers.Add(TriggerName.UndefinedFaultEvent, oUndefinedFaultEvent);
		    #endregion Events

		    #region States instantiations
            // root state instantiation
            oIsBatchModeOnChoiceState = new NSFChoiceState(nameof(oIsBatchModeOnChoiceState), this);
            oSystemPoweredState = new NSFCompositeState(nameof(oSystemPoweredState), this, (c) => RaiseStateEnterEvent(c, oSystemPoweredState), (c) => RaiseStateExitEvent(c, oSystemPoweredState));

            // SystemPoweredState's substate instantiation
            oProcessingState = new NSFCompositeState(nameof(oProcessingState), oSystemPoweredState, (c) => RaiseStateEnterEvent(c, oProcessingState), (c) => ProcessingStateExitedAsync(c, oProcessingState));
            oCameraViewState = new NSFCompositeState(nameof(oCameraViewState), oSystemPoweredState, (c) => CameraViewStateEnteredAsync(c, oCameraViewState), (c) => CameraViewStateExitedAsync(c, oCameraViewState));
            oReadyState = new NSFCompositeState(nameof(oReadyState), oSystemPoweredState, (c) => RaiseStateEnterEvent(c, oReadyState), (c) => RaiseStateExitEvent(c, oReadyState));
            oIsCriticalAlarmChoiceState = new NSFChoiceState(nameof(oIsCriticalAlarmChoiceState), oSystemPoweredState);
            oFaultParentState = new NSFCompositeState(nameof(oFaultParentState), oSystemPoweredState, (c) => RaiseStateEnterEvent(c, oFaultParentState), (c) => RaiseStateExitEvent(c, oFaultParentState));
            oInitAfterSettingsState = new NSFCompositeState(nameof(oInitAfterSettingsState), oSystemPoweredState, (c) => InitAfterSettingsStateEnteredAsync(c, oInitAfterSettingsState), (c) => RaiseStateExitEvent(c, oInitAfterSettingsState));
            oSystemPowered_InitialState = new NSFInitialState(nameof(oSystemPowered_InitialState), oSystemPoweredState);
            oInitState = new NSFCompositeState(nameof(oInitState), oSystemPoweredState, (c) => InitStateEnteredAsync(c, oInitState), (c) => RaiseStateExitEvent(c, oInitState));

            // ReadyState's substate instantiation
            oReviewProcState = new NSFCompositeState(nameof(oReviewProcState), oReadyState, (c) => ReviewProcStateEnteredAsync(c, oReviewProcState), (c) => RaiseStateExitEvent(c, oReviewProcState));
            oManualProcessImageState = new NSFCompositeState(nameof(oManualProcessImageState), oReadyState, (c) => ManualProcessImageStateEnteredAsync(c, oManualProcessImageState), (c) => RaiseStateExitEvent(c, oManualProcessImageState));
            oReadTiffLayerState = new NSFCompositeState(nameof(oReadTiffLayerState), oReadyState, (c) => ReadTiffLayerStateEnteredAsync(c, oReadTiffLayerState), (c) => RaiseStateExitEvent(c, oReadTiffLayerState));
            oReady_InitialState = new NSFInitialState(nameof(oReady_InitialState), oReadyState);
            oIdleState = new NSFCompositeState(nameof(oIdleState), oReadyState, (c) => RaiseStateEnterEvent(c, oIdleState), (c) => RaiseStateExitEvent(c, oIdleState));

            // ProcessingState's substate instantiation
            oOpenSensorState = new NSFCompositeState(nameof(oOpenSensorState), oProcessingState, (c) => OpenSensorStateEnteredAsync(c, oOpenSensorState), (c) => RaiseStateExitEvent(c, oOpenSensorState));
            oAbortingState = new NSFCompositeState(nameof(oAbortingState), oProcessingState, (c) => AbortingStateEnteredAsync(c, oAbortingState), (c) => RaiseStateExitEvent(c, oAbortingState));
            oIsMotionChoiceState = new NSFChoiceState(nameof(oIsMotionChoiceState), oProcessingState);
            oIsEnvSettingChoiceState = new NSFChoiceState(nameof(oIsEnvSettingChoiceState), oProcessingState);
            oSetEnvironmentState = new NSFCompositeState(nameof(oSetEnvironmentState), oProcessingState, (c) => SetEnvironmentStateEnteredAsync(c, oSetEnvironmentState), (c) => RaiseStateExitEvent(c, oSetEnvironmentState));
            oMotionState = new NSFCompositeState(nameof(oMotionState), oProcessingState, (c) => MotionStateEnteredAsync(c, oMotionState), (c) => RaiseStateExitEvent(c, oMotionState));
            oIsOperatorRequestChoiceState = new NSFChoiceState(nameof(oIsOperatorRequestChoiceState), oProcessingState);
            oOperatorRequestState = new NSFCompositeState(nameof(oOperatorRequestState), oProcessingState, (c) => OperatorRequestStateEnteredAsync(c, oOperatorRequestState), (c) => RaiseStateExitEvent(c, oOperatorRequestState));
            oIsMoreImagesChoiceState = new NSFChoiceState(nameof(oIsMoreImagesChoiceState), oProcessingState);
            oAcquisitionState = new NSFCompositeState(nameof(oAcquisitionState), oProcessingState, (c) => AcquisitionStateEnteredAsync(c, oAcquisitionState), (c) => RaiseStateExitEvent(c, oAcquisitionState));
            oProcessing_InitialState = new NSFInitialState(nameof(oProcessing_InitialState), oProcessingState);

            // CameraViewState's substate instantiation
            oCameraViewIdleState = new NSFCompositeState(nameof(oCameraViewIdleState), oCameraViewState, (c) => CameraViewIdleStateEnteredAsync(c, oCameraViewIdleState), (c) => CameraViewIdleStateExitedAsync(c, oCameraViewIdleState));
            oCameraView_InitialState = new NSFInitialState(nameof(oCameraView_InitialState), oCameraViewState);
            oCameraViewActiveState = new NSFCompositeState(nameof(oCameraViewActiveState), oCameraViewState, (c) => CameraViewActiveStateEnteredAsync(c, oCameraViewActiveState), (c) => CameraViewActiveStateExitedAsync(c, oCameraViewActiveState));

            // FaultParentState's substate instantiation
            oFaultParent_InitialState = new NSFInitialState(nameof(oFaultParent_InitialState), oFaultParentState);
            oFaultedState = new NSFCompositeState(nameof(oFaultedState), oFaultParentState, (c) => RaiseStateEnterEvent(c, oFaultedState), (c) => RaiseStateExitEvent(c, oFaultedState));

		    #endregion States  instantiations

	        #region Transitions instantiations
            // From IsBatchModeOnChoiceState Transitions' definitions
            IsBatchModeOnChoiceTOReviewProcTransition = new NSFExternalTransition(nameof(IsBatchModeOnChoiceTOReviewProcTransition), oIsBatchModeOnChoiceState, oReviewProcState, null, IsBatchModeOn, async (c) => await DisplayBatchOn(c));

            // From SystemPoweredState Transitions' definitions
            SystemPoweredTOFaultParentTransitionBYUndefinedFaultEvent = new NSFExternalTransition(nameof(SystemPoweredTOFaultParentTransitionBYUndefinedFaultEvent), oSystemPoweredState, oFaultParentState, oUndefinedFaultEvent, null, null);
            SystemPoweredTOFaultParentTransitionBYEStopEvent = new NSFExternalTransition(nameof(SystemPoweredTOFaultParentTransitionBYEStopEvent), oSystemPoweredState, oFaultParentState, oEStopEvent, null, null);
            SystemPoweredTOFaultParentTransitionBYSettingsFaultEvent = new NSFExternalTransition(nameof(SystemPoweredTOFaultParentTransitionBYSettingsFaultEvent), oSystemPoweredState, oFaultParentState, oSettingsFaultEvent, null, async (c) => await LatchSettingsFaultAlarm(c));
            SystemPoweredTOFaultParentTransitionBYCameraFaultEvent = new NSFExternalTransition(nameof(SystemPoweredTOFaultParentTransitionBYCameraFaultEvent), oSystemPoweredState, oFaultParentState, oCameraFaultEvent, null, async (c) => await LatchCameraFaultAlarm(c));
            SystemPoweredTOInitTransitionBYInitEvent = new NSFExternalTransition(nameof(SystemPoweredTOInitTransitionBYInitEvent), oSystemPoweredState, oInitState, oInitEvent, SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard, null);

            // From IsCriticalAlarmChoiceState Transitions' definitions
            IsCriticalAlarmChoiceTOReadyTransition = new NSFExternalTransition(nameof(IsCriticalAlarmChoiceTOReadyTransition), oIsCriticalAlarmChoiceState, oReadyState, null, Else, null);
            IsCriticalAlarmChoiceTOFaultParentTransition = new NSFExternalTransition(nameof(IsCriticalAlarmChoiceTOFaultParentTransition), oIsCriticalAlarmChoiceState, oFaultParentState, null, IsCriticalAlarm, null);

            // From InitState Transitions' definitions
            InitTOInitAfterSettingsTransitionBYInitCompleteEvent = new NSFExternalTransition(nameof(InitTOInitAfterSettingsTransitionBYInitCompleteEvent), oInitState, oInitAfterSettingsState, oInitCompleteEvent, null, async (c) => await InitAfterInitSettingsAction(c));

            // From ReadyState Transitions' definitions
            ReadyTOIdleTransitionBYAbortEvent = new NSFExternalTransition(nameof(ReadyTOIdleTransitionBYAbortEvent), oReadyState, oIdleState, oAbortEvent, null, null);
            ReadyTOCameraViewTransitionBYCameraViewEvent = new NSFExternalTransition(nameof(ReadyTOCameraViewTransitionBYCameraViewEvent), oReadyState, oCameraViewState, oCameraViewEvent, null, null);

            // From SystemPowered_InitialState Transitions' definitions
            SystemPowered_InitialTOInitTransition = new NSFExternalTransition(nameof(SystemPowered_InitialTOInitTransition), oSystemPowered_InitialState, oInitState, null, null, null);

            // From ProcessingState Transitions' definitions
            ProcessingTOReadyTransitionBYAbortEvent = new NSFExternalTransition(nameof(ProcessingTOReadyTransitionBYAbortEvent), oProcessingState, oReadyState, oAbortEvent, null, null);

            // From FaultParentState Transitions' definitions
            FaultParentTOIsCriticalAlarmChoiceTransitionBYClearFaultsEvent = new NSFExternalTransition(nameof(FaultParentTOIsCriticalAlarmChoiceTransitionBYClearFaultsEvent), oFaultParentState, oIsCriticalAlarmChoiceState, oClearFaultsEvent, null, null);
            FaultParentStateOnAbortEventTransition = new NSFInternalTransition(nameof(FaultParentStateOnAbortEventTransition), oFaultParentState, oAbortEvent, null, null);

            // From InitAfterSettingsState Transitions' definitions
            InitAfterSettingsTOIsCriticalAlarmChoiceTransitionBYInitAfterSettingsCompleteEvent = new NSFExternalTransition(nameof(InitAfterSettingsTOIsCriticalAlarmChoiceTransitionBYInitAfterSettingsCompleteEvent), oInitAfterSettingsState, oIsCriticalAlarmChoiceState, oInitAfterSettingsCompleteEvent, null, null);

            // From CameraViewState Transitions' definitions
            CameraViewTOReadyTransitionBYLeaveCameraViewEvent = new NSFExternalTransition(nameof(CameraViewTOReadyTransitionBYLeaveCameraViewEvent), oCameraViewState, oReadyState, oLeaveCameraViewEvent, null, null);

            // From ReviewProcState Transitions' definitions
            ReviewProcTOProcessingTransitionBYStartEvent = new NSFExternalTransition(nameof(ReviewProcTOProcessingTransitionBYStartEvent), oReviewProcState, oProcessingState, oStartEvent, null, null);
            ReviewProcTOReadTiffLayerTransitionBYLoadLayerTiffEvent = new NSFExternalTransition(nameof(ReviewProcTOReadTiffLayerTransitionBYLoadLayerTiffEvent), oReviewProcState, oReadTiffLayerState, oLoadLayerTiffEvent, null, async (c) => await ClearReviewedData(c));
            ReviewProcTOManualProcessImageTransitionBYLoadSingleFileEvent = new NSFExternalTransition(nameof(ReviewProcTOManualProcessImageTransitionBYLoadSingleFileEvent), oReviewProcState, oManualProcessImageState, oLoadSingleFileEvent, null, async (c) => await ClearReviewedData(c));
            ReviewProcTOReviewProcTransitionBYReprocBatchEvent = new NSFLocalTransition(nameof(ReviewProcTOReviewProcTransitionBYReprocBatchEvent), oReviewProcState, oReviewProcState, oReprocBatchEvent, null, async (c) => await UpdateProcDataCacheAsync(c));
            ReviewProcTOManualProcessImageTransitionBYLoadBatchFileEvent = new NSFExternalTransition(nameof(ReviewProcTOManualProcessImageTransitionBYLoadBatchFileEvent), oReviewProcState, oManualProcessImageState, oLoadBatchFileEvent, null, async (c) => await ClearReviewedData(c));

            // From ManualProcessImageState Transitions' definitions
            ManualProcessImageTOIsBatchModeOnChoiceTransitionBYAbortEvent = new NSFExternalTransition(nameof(ManualProcessImageTOIsBatchModeOnChoiceTransitionBYAbortEvent), oManualProcessImageState, oIsBatchModeOnChoiceState, oAbortEvent, null, null);
            ManualProcessImageTOIsBatchModeOnChoiceTransitionBYManualProcCompleteEvent = new NSFExternalTransition(nameof(ManualProcessImageTOIsBatchModeOnChoiceTransitionBYManualProcCompleteEvent), oManualProcessImageState, oIsBatchModeOnChoiceState, oManualProcCompleteEvent, null, null);

            // From FaultParent_InitialState Transitions' definitions
            FaultParent_InitialTOFaultedTransition = new NSFExternalTransition(nameof(FaultParent_InitialTOFaultedTransition), oFaultParent_InitialState, oFaultedState, null, null, null);

            // From Ready_InitialState Transitions' definitions
            Ready_InitialTOIdleTransition = new NSFExternalTransition(nameof(Ready_InitialTOIdleTransition), oReady_InitialState, oIdleState, null, null, null);

            // From IdleState Transitions' definitions
            IdleTOManualProcessImageTransitionBYLoadSingleFileEvent = new NSFExternalTransition(nameof(IdleTOManualProcessImageTransitionBYLoadSingleFileEvent), oIdleState, oManualProcessImageState, oLoadSingleFileEvent, null, async (c) => await BatchModeOff(c));
            IdleTOManualProcessImageTransitionBYLoadBatchFileEvent = new NSFExternalTransition(nameof(IdleTOManualProcessImageTransitionBYLoadBatchFileEvent), oIdleState, oManualProcessImageState, oLoadBatchFileEvent, null, async (c) => await BatchModeOn(c));
            IdleTOReadTiffLayerTransitionBYLoadLayerTiffEvent = new NSFExternalTransition(nameof(IdleTOReadTiffLayerTransitionBYLoadLayerTiffEvent), oIdleState, oReadTiffLayerState, oLoadLayerTiffEvent, null, async (c) => await BatchModeOn(c));
            IdleTOProcessingTransitionBYStartEvent = new NSFExternalTransition(nameof(IdleTOProcessingTransitionBYStartEvent), oIdleState, oProcessingState, oStartEvent, null, null);
            IdleStateOnAbortEventTransition = new NSFInternalTransition(nameof(IdleStateOnAbortEventTransition), oIdleState, oAbortEvent, null, null);

            // From ReadTiffLayerState Transitions' definitions
            ReadTiffLayerTOManualProcessImageTransitionBYReadLayerTiffCompleteEvent = new NSFExternalTransition(nameof(ReadTiffLayerTOManualProcessImageTransitionBYReadLayerTiffCompleteEvent), oReadTiffLayerState, oManualProcessImageState, oReadLayerTiffCompleteEvent, null, null);

            // From IsMoreImagesChoiceState Transitions' definitions
            IsMoreImagesChoiceTOReadyTransition = new NSFExternalTransition(nameof(IsMoreImagesChoiceTOReadyTransition), oIsMoreImagesChoiceState, oReadyState, null, Else, null);
            IsMoreImagesChoiceTOIsOperatorRequestChoiceTransition = new NSFExternalTransition(nameof(IsMoreImagesChoiceTOIsOperatorRequestChoiceTransition), oIsMoreImagesChoiceState, oIsOperatorRequestChoiceState, null, IsMoreImages, null);

            // From OpenSensorState Transitions' definitions
            OpenSensorTOIsOperatorRequestChoiceTransitionBYOpenSensorCompleteEvent = new NSFExternalTransition(nameof(OpenSensorTOIsOperatorRequestChoiceTransitionBYOpenSensorCompleteEvent), oOpenSensorState, oIsOperatorRequestChoiceState, oOpenSensorCompleteEvent, null, null);

            // From IsOperatorRequestChoiceState Transitions' definitions
            IsOperatorRequestChoiceTOIsMotionChoiceTransition = new NSFExternalTransition(nameof(IsOperatorRequestChoiceTOIsMotionChoiceTransition), oIsOperatorRequestChoiceState, oIsMotionChoiceState, null, Else, null);
            IsOperatorRequestChoiceTOOperatorRequestTransition = new NSFExternalTransition(nameof(IsOperatorRequestChoiceTOOperatorRequestTransition), oIsOperatorRequestChoiceState, oOperatorRequestState, null, IsOperatorRequest, null);

            // From AbortingState Transitions' definitions
            AbortingTOReadyTransitionBYMotionCompleteEvent = new NSFExternalTransition(nameof(AbortingTOReadyTransitionBYMotionCompleteEvent), oAbortingState, oReadyState, oMotionCompleteEvent, null, null);
            AbortingStateOnAbortEventTransition = new NSFInternalTransition(nameof(AbortingStateOnAbortEventTransition), oAbortingState, oAbortEvent, null, null);

            // From MotionState Transitions' definitions
            MotionTOAbortingTransitionBYAbortEvent = new NSFExternalTransition(nameof(MotionTOAbortingTransitionBYAbortEvent), oMotionState, oAbortingState, oAbortEvent, null, null);
            MotionTOIsEnvSettingChoiceTransitionBYMotionCompleteEvent = new NSFExternalTransition(nameof(MotionTOIsEnvSettingChoiceTransitionBYMotionCompleteEvent), oMotionState, oIsEnvSettingChoiceState, oMotionCompleteEvent, null, null);

            // From IsMotionChoiceState Transitions' definitions
            IsMotionChoiceTOMotionTransition = new NSFExternalTransition(nameof(IsMotionChoiceTOMotionTransition), oIsMotionChoiceState, oMotionState, null, IsMotion, null);
            IsMotionChoiceTOIsEnvSettingChoiceTransition = new NSFExternalTransition(nameof(IsMotionChoiceTOIsEnvSettingChoiceTransition), oIsMotionChoiceState, oIsEnvSettingChoiceState, null, Else, null);

            // From OperatorRequestState Transitions' definitions
            OperatorRequestTOIsMotionChoiceTransitionBYStartEvent = new NSFExternalTransition(nameof(OperatorRequestTOIsMotionChoiceTransitionBYStartEvent), oOperatorRequestState, oIsMotionChoiceState, oStartEvent, null, null);

            // From Processing_InitialState Transitions' definitions
            Processing_InitialTOOpenSensorTransition = new NSFExternalTransition(nameof(Processing_InitialTOOpenSensorTransition), oProcessing_InitialState, oOpenSensorState, null, null, null);

            // From AcquisitionState Transitions' definitions
            AcquisitionTOIsMoreImagesChoiceTransitionBYImagesAcquiredEvent = new NSFExternalTransition(nameof(AcquisitionTOIsMoreImagesChoiceTransitionBYImagesAcquiredEvent), oAcquisitionState, oIsMoreImagesChoiceState, oImagesAcquiredEvent, null, null);

            // From SetEnvironmentState Transitions' definitions
            SetEnvironmentTOAcquisitionTransitionBYSetEnvCompleteEvent = new NSFExternalTransition(nameof(SetEnvironmentTOAcquisitionTransitionBYSetEnvCompleteEvent), oSetEnvironmentState, oAcquisitionState, oSetEnvCompleteEvent, null, null);

            // From IsEnvSettingChoiceState Transitions' definitions
            IsEnvSettingChoiceTOSetEnvironmentTransition = new NSFExternalTransition(nameof(IsEnvSettingChoiceTOSetEnvironmentTransition), oIsEnvSettingChoiceState, oSetEnvironmentState, null, IsEnvSetting, null);
            IsEnvSettingChoiceTOAcquisitionTransition = new NSFExternalTransition(nameof(IsEnvSettingChoiceTOAcquisitionTransition), oIsEnvSettingChoiceState, oAcquisitionState, null, Else, null);

            // From CameraViewActiveState Transitions' definitions
            CameraViewActiveTOCameraViewIdleTransitionBYCameraFaultEvent = new NSFExternalTransition(nameof(CameraViewActiveTOCameraViewIdleTransitionBYCameraFaultEvent), oCameraViewActiveState, oCameraViewIdleState, oCameraFaultEvent, null, null);
            CameraViewActiveTOCameraViewIdleTransitionBYHaltAcquisitionEvent = new NSFExternalTransition(nameof(CameraViewActiveTOCameraViewIdleTransitionBYHaltAcquisitionEvent), oCameraViewActiveState, oCameraViewIdleState, oHaltAcquisitionEvent, null, null);

            // From CameraViewIdleState Transitions' definitions
            CameraViewIdleTOCameraViewActiveTransitionBYStartAcquisitionEvent = new NSFExternalTransition(nameof(CameraViewIdleTOCameraViewActiveTransitionBYStartAcquisitionEvent), oCameraViewIdleState, oCameraViewActiveState, oStartAcquisitionEvent, null, null);

            // From CameraView_InitialState Transitions' definitions
            CameraView_InitialTOCameraViewActiveTransition = new NSFExternalTransition(nameof(CameraView_InitialTOCameraViewActiveTransition), oCameraView_InitialState, oCameraViewActiveState, null, null, null);

		    #endregion Transitions instantiations

            AfterCreateStateMachine();

        }

        public virtual void BeforeCreateStateMachine() { }
        public virtual void AfterCreateStateMachine() { }

        #region C# Events
        public delegate void StateChangesEventHandler(object sender, StateChangesEventArgs stateChangesEventArgs);
        public event StateChangesEventHandler StateChangesEvent;
        public event StateChangesEventHandler StateChangesAtStateEvent;
        //public delegate Task StateChangedEventHandler(PropertyChangedEventArgs e);
        //public delegate Task StateChangingEventHandler(PropertyChangedEventArgs e);
        //public event StateChangingEventHandler StateEnteredAsync;
        //public event StateChangedEventHandler StateExitedAsync;
        #endregion C# Events

        #region State Machine Methods

        #region States Actions

        public virtual async Task AbortingStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.HaltAllMotionAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task AcquisitionStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.AcquireImagesAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task CameraViewActiveStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.EnableCamViewActiveFeatures().ConfigureAwait(continueOnCapturedContext:false);
                await MainModel.StartAcquisition().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task CameraViewActiveStateExitedAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                await MainModel.DisableCamViewActiveFeatures().ConfigureAwait(continueOnCapturedContext:false);;
                await MainModel.StopAcquisition().ConfigureAwait(continueOnCapturedContext:false);;
                this.RaiseStateExitEvent(oContext, state);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task CameraViewIdleStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.EnableCamViewIdleFeatures().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task CameraViewIdleStateExitedAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                await MainModel.DisableCamViewIdleFeatures().ConfigureAwait(continueOnCapturedContext:false);;
                this.RaiseStateExitEvent(oContext, state);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task CameraViewStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.EnableCameraViewFeatures().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task CameraViewStateExitedAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                await MainModel.DisableCameraViewFeatures().ConfigureAwait(continueOnCapturedContext:false);;
                this.RaiseStateExitEvent(oContext, state);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task InitAfterSettingsStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.InitAfterSettingsAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task InitStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.InitAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task ManualProcessImageStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.ManualProcessImagesAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task MotionStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.StartNextMotionAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task OpenSensorStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.OpenAcqSensorAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task OperatorRequestStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.ShowNextOperatorPromptAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task ProcessingStateExitedAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                await MainModel.CloseAcqSensorAsync().ConfigureAwait(continueOnCapturedContext:false);;
                this.RaiseStateExitEvent(oContext, state);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task ReadTiffLayerStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.ReadLayeredTiffFileAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task ReviewProcStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.ReviewProcessedDataAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }
        public virtual async Task SetEnvironmentStateEnteredAsync(NSFStateMachineContext oContext, NSFState state)
        {
            try {
                this.RaiseStateEnterEvent(oContext, state);
                await MainModel.StartNextMotionAsync().ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }

        #endregion States Actions

	    #region Transitions Actions
        /// <summary>
        /// This method is called when the 'IsBatchModeOnChoiceState' transitions to 'ReviewProcState'
        /// triggered by ''trigger is missing'' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task DisplayBatchOn(NSFStateMachineContext oContext)
        {
            try {
                await MainModel.DisplayBatchOn(oContext).ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e) {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }

        /// <summary>
        /// This method is called when the 'SystemPoweredState' transitions to 'FaultParentState'
        /// triggered by 'SettingsFaultEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task LatchSettingsFaultAlarm(NSFStateMachineContext oContext)
        {
            try {
                await MainModel.LatchSettingsFaultAlarm(oContext).ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e) {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }

        /// <summary>
        /// This method is called when the 'SystemPoweredState' transitions to 'FaultParentState'
        /// triggered by 'CameraFaultEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task LatchCameraFaultAlarm(NSFStateMachineContext oContext)
        {
            try {
                await MainModel.LatchCameraFaultAlarm(oContext).ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e) {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }

        /// <summary>
        /// This method is called when the 'InitState' transitions to 'InitAfterSettingsState'
        /// triggered by 'InitCompleteEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task InitAfterInitSettingsAction(NSFStateMachineContext oContext)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// This method is called when the 'ReviewProcState' transitions to 'ReadTiffLayerState'
        /// triggered by 'LoadLayerTiffEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task ClearReviewedData(NSFStateMachineContext oContext)
        {
            try {
                await MainModel.ClearReviewedData(oContext).ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e) {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }

        /// <summary>
        /// This method is called when the 'ReviewProcState' transitions to 'ReviewProcState'
        /// triggered by 'ReprocBatchEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task UpdateProcDataCacheAsync(NSFStateMachineContext oContext)
        {
            try {
                await MainModel.UpdateProcDataCacheAsync(oContext).ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e) {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }

        /// <summary>
        /// This method is called when the 'IdleState' transitions to 'ManualProcessImageState'
        /// triggered by 'LoadSingleFileEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task BatchModeOff(NSFStateMachineContext oContext)
        {
            try {
                await MainModel.BatchModeOff(oContext).ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e) {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }

        /// <summary>
        /// This method is called when the 'IdleState' transitions to 'ManualProcessImageState'
        /// triggered by 'LoadBatchFileEvent' 
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>Nothing</returns>
        public virtual async Task BatchModeOn(NSFStateMachineContext oContext)
        {
            try {
                await MainModel.BatchModeOn(oContext).ConfigureAwait(continueOnCapturedContext:false);
            }
            catch (Exception e) {
                ms_iLogger.LogException(ELogLevel.Error, e.Source, e);
                this.SendStateEvent(UndefinedFaultEvent);
            }
        }

		#endregion Transitions Actions

	    #region Transitions Choices and Guards
        #region IsBatchModeOn

        public bool IsBatchModeOn_InSimulationMode { get; set; }
        public bool IsBatchModeOn_SimulatedValue { get; set; }

        /// <summary>
        /// A Guard function returning the State of 'IsBatchModeOn'.
        /// Defined at Transition IsBatchModeOnChoiceTOReviewProcTransition
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>guard result</returns>
        public virtual bool IsBatchModeOn(NSFStateMachineContext oContext)
        {
            if (IsBatchModeOn_InSimulationMode == true) 
                return IsBatchModeOn_SimulatedValue;
            else 
                return MainModel.IsBatchModeOn;
        }
        #endregion IsBatchModeOn

        #region SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard

        public bool SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard_InSimulationMode { get; set; }
        public bool SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard_SimulatedValue { get; set; }

        /// <summary>
        /// A Guard function returning the State of 'SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard'.
        /// Defined at Transition SystemPoweredTOInitTransitionBYInitEvent
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>guard result</returns>
        public virtual bool SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard(NSFStateMachineContext oContext)
        {
            if (SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard_InSimulationMode == true) 
                return SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard_SimulatedValue;
            else 
                return MainModel.SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard;
        }
        #endregion SystemPoweredTOInitTransitionBYInitEventIsAdminLoginGuard

        #region IsCriticalAlarm

        public bool IsCriticalAlarm_InSimulationMode { get; set; }
        public bool IsCriticalAlarm_SimulatedValue { get; set; }

        /// <summary>
        /// A Guard function returning the State of 'IsCriticalAlarm'.
        /// Defined at Transition IsCriticalAlarmChoiceTOFaultParentTransition
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>guard result</returns>
        public virtual bool IsCriticalAlarm(NSFStateMachineContext oContext)
        {
            if (IsCriticalAlarm_InSimulationMode == true) 
                return IsCriticalAlarm_SimulatedValue;
            else 
                return MainModel.IsCriticalAlarm;
        }
        #endregion IsCriticalAlarm

        #region IsMoreImages

        public bool IsMoreImages_InSimulationMode { get; set; }
        public bool IsMoreImages_SimulatedValue { get; set; }

        /// <summary>
        /// A Guard function returning the State of 'IsMoreImages'.
        /// Defined at Transition IsMoreImagesChoiceTOIsOperatorRequestChoiceTransition
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>guard result</returns>
        public virtual bool IsMoreImages(NSFStateMachineContext oContext)
        {
            if (IsMoreImages_InSimulationMode == true) 
                return IsMoreImages_SimulatedValue;
            else 
                return MainModel.IsMoreImages;
        }
        #endregion IsMoreImages

        #region IsOperatorRequest

        public bool IsOperatorRequest_InSimulationMode { get; set; }
        public bool IsOperatorRequest_SimulatedValue { get; set; }

        /// <summary>
        /// A Guard function returning the State of 'IsOperatorRequest'.
        /// Defined at Transition IsOperatorRequestChoiceTOOperatorRequestTransition
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>guard result</returns>
        public virtual bool IsOperatorRequest(NSFStateMachineContext oContext)
        {
            if (IsOperatorRequest_InSimulationMode == true) 
                return IsOperatorRequest_SimulatedValue;
            else 
                return MainModel.IsOperatorRequest;
        }
        #endregion IsOperatorRequest

        #region IsMotion

        public bool IsMotion_InSimulationMode { get; set; }
        public bool IsMotion_SimulatedValue { get; set; }

        /// <summary>
        /// A Guard function returning the State of 'IsMotion'.
        /// Defined at Transition IsMotionChoiceTOMotionTransition
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>guard result</returns>
        public virtual bool IsMotion(NSFStateMachineContext oContext)
        {
            if (IsMotion_InSimulationMode == true) 
                return IsMotion_SimulatedValue;
            else 
                return MainModel.IsMotion;
        }
        #endregion IsMotion

        #region IsEnvSetting

        public bool IsEnvSetting_InSimulationMode { get; set; }
        public bool IsEnvSetting_SimulatedValue { get; set; }

        /// <summary>
        /// A Guard function returning the State of 'IsEnvSetting'.
        /// Defined at Transition IsEnvSettingChoiceTOSetEnvironmentTransition
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param>
        /// <returns>guard result</returns>
        public virtual bool IsEnvSetting(NSFStateMachineContext oContext)
        {
            if (IsEnvSetting_InSimulationMode == true) 
                return IsEnvSetting_SimulatedValue;
            else 
                return MainModel.IsEnvSetting;
        }
        #endregion IsEnvSetting

		#endregion Transitions Choices and Guards

	    #region Other methods

        public bool StartStateMachine()
        {
            // Support StateMachine specific log
        
            NSFTraceLog.PrimaryTraceLog.Enabled = true;
            StateChangeActions += HandleStateChange;
        
            // Start State machine
            startStateMachine();
        
            return true;
        }

        public bool StopStateMachine()
        {
            stopStateMachine();
            // Save trace log
            NSFTraceLog.PrimaryTraceLog.saveLog("PolarCamStateMachineLog.xml");
            NSFEnvironment.terminate();
        
            return true;
        }

        //public bool InvokeEvent(NSFEvent oEvent, object oEventData)
        //{
        //    bool bEventHandled = false;
        //    //TODO: log event received
        //    try
        //    {
        //        if (oEventData == null) queueEvent(oEvent);
        //        else queueEvent(oEvent, new CStateMachineEventData(oEventData));
        //        bEventHandled = true;
        //        //TODO: log event completed
        //    }
        //    catch (Exception)
        //    {
        //        bEventHandled = false;
        //        //TODO: log event failed
        //    }

        //    return bEventHandled;
        //}

        #region Indicators Dictionary
        private Dictionary<string, PropertyInfo> indicatorsDictionary;
        public Dictionary<string, PropertyInfo> IndicatorsDictionary
        {
            get
            {
                if (indicatorsDictionary == null)
                {
                    indicatorsDictionary = new Dictionary<string, PropertyInfo>();
                    var indicatorProperties = MainModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.Name.StartsWith("IsIn") && p.PropertyType == typeof(bool));
                    indicatorProperties.ToList().ForEach(p => indicatorsDictionary.Add(p.Name, p));
                }
                return indicatorsDictionary;
            }
        }
        #endregion Indicators Dictionary

        /// <summary>
        /// This method is called when a State is Entered.
        /// Must be synchronous, non-blocking, no-locks, return on when-all-completed only and run all subscribers concurrently
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <param name="state">State that is changing.</param> 
        /// <returns>Nothing</returns>
        protected void RaiseStateEnterEvent(NSFStateMachineContext oContext, NSFState state)
        {
            //TODO: queue new state
            var propName = "IsIn" + state.Name.Substring(1);
            if (state.GetType() == typeof(NSFCompositeState) && IndicatorsDictionary.ContainsKey(propName))
                IndicatorsDictionary[propName].SetValue(MainModel, true);

            //if (StateEnteredAsync != null)
            //{
            //    var asyncHandlers = StateEnteredAsync.GetInvocationList().Cast<StateChangingEventHandler>();
            //    var asyncHandlerTasks = new List<Task>();
            //    asyncHandlers.ToList().ForEach(delgt => asyncHandlerTasks.Add(new Task(() => delgt(new PropertyChangedEventArgs(state.Name)))));
            //    await Task.WhenAll(asyncHandlerTasks.ToArray());
            //}
        }

        /// <summary>
        /// This method is called when a State is exiting.
        /// Must be synchronous, non-blocking, no-locks, return on when-all-completed only and run all subscribers concurrently
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <param name="state">State that is changing.</param> 
        /// <returns>Nothing</returns>
        protected void RaiseStateExitEvent(NSFStateMachineContext oContext, NSFState state)
        {
            //TODO: queue new state
            var propName = "IsIn" + state.Name.Substring(1);
            if (state.GetType() == typeof(NSFCompositeState) && IndicatorsDictionary.ContainsKey(propName))
                IndicatorsDictionary[propName].SetValue(MainModel, false);

            //if (StateExitedAsync != null)
            //{
            //    var asyncHandlers = StateExitedAsync.GetInvocationList().Cast<StateChangedEventHandler>();
            //    var asyncHandlerTasks = new List<Task>();
            //    asyncHandlers.ToList().ForEach(delgt =>
            //        asyncHandlerTasks.Add(new Task(() => delgt(new PropertyChangedEventArgs(state.Name)))));
            //    await Task.WhenAll(asyncHandlerTasks.ToArray());
            //}
        }

        /// <summary>}
        /// Called by the NSF whenever there is a state change}
        /// </summary>}
        /// <param name="oContext"></param>}
        private void HandleStateChange(NSFStateMachineContext oContext)
        {
            PreviousState = CurrentState;
            CurrentState = GetState(oContext.EnteringState.Name);
            //PreviousStateName = CurrentStateName;
            //if (CurrentStateName == null) CurrentStateName = "none";
            //else CurrentStateName = ((EState)CurrentState).ToString();
            PreviousContext = CurrentContext;
            CurrentContext = oContext;

            if (oContext.EnteringState is NSFChoiceState) //(strNewState.StartsWith("Is"))
            {
                string strMsg = $@"Evaluating {CurrentState} state after {PreviousState} state.";
                ms_iLogger.XLog(ELogLevel.Info, strMsg, new SCodeContext(CurrentState));
                return;
            }
            else
            {
                string strMsg;
                if (PreviousState == null)
                    strMsg = $"Starting at the {CurrentState} state!";
                else
                    strMsg = $"Leaving {PreviousState} state to {CurrentState} state!";

                ms_iLogger.XLog(ELogLevel.Info, strMsg, new SCodeContext(CurrentState));
                SystemState = oContext.EnteringState.Name;
            }

            // Is there an operator prompt to display for this new state?
            (string OperatorPrompt, bool HighlightOperator) oPromptDetails = ("", false);
            if (CurrentState != null)
                OperatorPromptForStateDict.TryGetValue(((EState) CurrentState), out oPromptDetails);
            
            StateChangesEventArgs.OperatorPrompt = LastOperatorPrompt = oPromptDetails.OperatorPrompt;
            StateChangesEventArgs.HandledStateName = CurrentState.ToString();
            StateChangesEventArgs.HighlightPrompt = oPromptDetails.HighlightOperator;

            // Notify anyone subscribed to the State Changed event
            try
            {
                StateChangesEvent?.Invoke(this, StateChangesEventArgs);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Fatal, e.Message, e);
            }

            try
            {
                StateChangesAtStateEvent?.Invoke(this, StateChangesEventArgs);
            }
            catch (Exception e)
            {
                ms_iLogger.LogException(ELogLevel.Fatal, e.Message, e);
            }
        }

        public bool SendStateEvent(NSFEventName eventName, object oEventData = null, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            if (!Triggers.ContainsKey(eventName))
            {
                ms_iLogger.XLog(ELogLevel.Error, "Invalid Event passed to SendStateEvent!", new SCodeContext(CurrentState));
                return false;
            }

            try
            {

                // Get event from enum passed in
                NSFEvent oEvent = Triggers[eventName];

                // Handle event data
                if (oEventData == null)
                {
                    // queue the event to the StateMachine
                    queueEvent(oEvent);
                }
                else
                {
                    // Queue the event and EventData to the state machine
                    CStateMachineEventData oStateMachineEventData = new CStateMachineEventData(oEventData);
                    queueEvent(oEvent, oStateMachineEventData);
                }

                // Log Event
                string strMsg = $@" Received Event ""{eventName}"" from {caller} @ {lineNumber}";
                ms_iLogger.XLog(ELogLevel.Info, strMsg, new SCodeContext(CurrentState));

                return true;
            }
            catch (Exception ex)
            {
                string strMsg = String.Format(@"Error!  Unable to handle handle event CPolarCamStateMachineBase!", eventName);
                ms_iLogger.LogException(ELogLevel.Error, strMsg, ex);
            }

            return false;
        }


        public string GetState(EState eState) { 
            var name = "o" + eState + "State"; 
            if (GetState(name) == null) name = "o" + eState;
            else if (GetState(name) == null) name = eState.ToString();
            return name;
        }

        public EState? GetState(string strState)
        {
            var strippedName = strState.Substring(1); //remove prefix "o"
            if (strippedName.EndsWith("State"))
                strippedName = strippedName.Substring(0, strippedName.Length - "State".Length); //Remove suffix State
            EState eStateNew;
            var nullableState = Enum.TryParse(strippedName, out eStateNew) ? (EState?)eStateNew : null;

            return nullableState;
        }

	    #endregion Other methods

        #endregion State Machine Methods
	}

    #region Enums
        #region Trigger's enum
        public partial class TriggerName
        {
		    public const NSFEventName AbortEvent = nameof(AbortEvent);
		    public const NSFEventName CameraFaultEvent = nameof(CameraFaultEvent);
		    public const NSFEventName CameraViewEvent = nameof(CameraViewEvent);
		    public const NSFEventName ClearFaultsEvent = nameof(ClearFaultsEvent);
		    public const NSFEventName EStopEvent = nameof(EStopEvent);
		    public const NSFEventName HaltAcquisitionEvent = nameof(HaltAcquisitionEvent);
		    public const NSFEventName ImagesAcquiredEvent = nameof(ImagesAcquiredEvent);
		    public const NSFEventName InitAfterSettingsCompleteEvent = nameof(InitAfterSettingsCompleteEvent);
		    public const NSFEventName InitCompleteEvent = nameof(InitCompleteEvent);
		    public const NSFEventName InitEvent = nameof(InitEvent);
		    public const NSFEventName LeaveCameraViewEvent = nameof(LeaveCameraViewEvent);
		    public const NSFEventName LoadBatchFileEvent = nameof(LoadBatchFileEvent);
		    public const NSFEventName LoadLayerTiffEvent = nameof(LoadLayerTiffEvent);
		    public const NSFEventName LoadSingleFileEvent = nameof(LoadSingleFileEvent);
		    public const NSFEventName ManualProcCompleteEvent = nameof(ManualProcCompleteEvent);
		    public const NSFEventName MotionCompleteEvent = nameof(MotionCompleteEvent);
		    public const NSFEventName OpenSensorCompleteEvent = nameof(OpenSensorCompleteEvent);
		    public const NSFEventName ReadLayerTiffCompleteEvent = nameof(ReadLayerTiffCompleteEvent);
		    public const NSFEventName ReprocBatchEvent = nameof(ReprocBatchEvent);
		    public const NSFEventName SetEnvCompleteEvent = nameof(SetEnvCompleteEvent);
		    public const NSFEventName SettingsFaultEvent = nameof(SettingsFaultEvent);
		    public const NSFEventName StartAcquisitionEvent = nameof(StartAcquisitionEvent);
		    public const NSFEventName StartEvent = nameof(StartEvent);
		    public const NSFEventName UndefinedFaultEvent = nameof(UndefinedFaultEvent);
        }
        #endregion Trigger's enum
 
    public enum EState
    {
		    none,
		    Aborting,
		    Acquisition,
		    CameraView_Initial,
		    CameraViewActive,
		    CameraViewIdle,
		    CameraView,
		    Faulted,
		    FaultParent_Initial,
		    FaultParent,
		    Idle,
		    InitAfterSettings,
		    Init,
		    IsBatchModeOnChoice,
		    IsCriticalAlarmChoice,
		    IsEnvSettingChoice,
		    IsMoreImagesChoice,
		    IsMotionChoice,
		    IsOperatorRequestChoice,
		    ManualProcessImage,
		    Motion,
		    OpenSensor,
		    OperatorRequest,
		    Processing_Initial,
		    Processing,
		    ReadTiffLayer,
		    Ready_Initial,
		    Ready,
		    ReviewProc,
		    SetEnvironment,
		    SystemPowered_Initial,
		    SystemPowered,
    }
    #endregion Enums
}
