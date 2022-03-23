///////////////////////////////////////////////////////////
//  Copyright © Corning Incorporated  2017
//  CStateMachineEventData.cs
//  Project CaliforniaSystem
//  Implementation of the Class CStateMachineEventData
//  Created on:      January 14, 2017 5:14:54 AM
///////////////////////////////////////////////////////////

using System.Threading;
using Corning.GenSys.Logger;
using NorthStateSoftware.NorthStateFramework;
using System;
using System.Collections.Generic;
using California.Interfaces;
using CaliforniaLensDisplay;

// Quick Start Guide for those using the GenSys Easy for the first time:
// First time users will primarily be interested in three states: Ready, Idle, and Running. The Idle and Prepare states are sub-states of the Ready State.
// The Ready state is entered after initialization and then the State Machine proceeds to the Idle State.
// The Idle state is intended to be a "waiting" state with nothing significant happening.
// The Prepare state is intended to be the state where your task is accomplished.

// Here are the rules for the Ready, Idle and Prepare States
// The method "ReadyStateEntryActions"   is called after initializtion has completed.
// The method "PrepareStateExitActions"    is called after if a Fault occurs.
// The method "IdleStateEntryActions"    is called after either "ReadyStateEntryActions" or "PrepareStateExitActions" is called.
// The method "IdleStateExitActions"     is called when the Start button is pressed.
// The method "PrepareStateEntryActions" is called when the Start button is pressed but after "IdleStateExitActions" is called.
// The method "PrepareStateExitActions"  is called when the Abort button is pressed.

// To start developing an Application utilizing a Start and Abort button:
// Place Initialization Code in "ReadyStateEntryActions". A call into the sytem may be needed.
// Place code to call into system to perform your task in "PrepareStateEntryActions".
// If a System thread was kicked off to start a task, then place a call in "PrepareStateExitActions" to call into the system to stop the thread.

// To start developing an application which doesn't require user interaction and just performs a task, place code to call into the System in "IdleStateEntryActions" and
// uncomment "m_iCaliforniaSystem.EnableStartButton = false;"  in "IdleStateEntryActions".

// This default template has code to show how to use the Start and Abort button in conjunction with calling into the System, which uses the ThreadHandler.
// When Start is pressed a message will be display every second until one of two conditions are met:
// 20 Messages have been displayed or
// The Abort button is pressed.
// 
// It is expected that this and the System code will be heavily modified.
// Note that the State Machine uses System to do all of the real work.
// The desired pattern is the State Machine not implementing any Application code.
//
// The method CreateOperatorPrompts() creates GUI prompts based on the state of the State Machine. 
// As the State Machine state changes, so does the GUI prompt.
// The following two statement are included in the code as part of the Quick Start Guide and may be tailored for your application.
// m_odictOperatorPromptForState.Add(oIdleState, "Hit Start to Start Running");
// m_odictOperatorPromptForState.Add(oPrepareState, "Hit Abort to Stop Running");
//
//


namespace California.CaliforniaSystem
{
    public class CStateMachineEventData : INSFNamedObject
    {
        private object m_oEventData;
        private string m_strName;

        public object EventData { get { return m_oEventData; } set { m_oEventData = value; } }
        public string Name { get { return m_strName; } set { m_strName = value; } }

        public CStateMachineEventData(object oObject)
        {
            m_oEventData = oObject;
        }
    }

    public class CCaliforniaStateMachine : NSFStateMachine, ICaliforniaStateMachine
    {
        #region Fields
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CCaliforniaStateMachine");
        //private Semaphore m_semaphoreStateChange = new Semaphore();
        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);
        private object m_objLock = new object();
        private string m_strLastOperatorPrompt = "";

        private EState? m_estatePrevious;
        private EState? m_estateCurrent;
        private string m_strSystemState;
        private ICaliforniaSystem m_iCaliforniaSystem;

        private bool m_bSimulationMode = false;
        private Dictionary<CaliforniaSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<CaliforniaSystemEventsEnum, NSFEvent>();
        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        private bool m_bInitComplete = false;
        private bool m_bInitAfterSettingsComplete = false;


        #region State Machine Fields

        // ***********************************
        // State Machine NSF Event Definitions
        // ***********************************


        private static NSFEvent oInitEvent;
        private static NSFEvent oInitCompleteEvent;
        private static NSFEvent oInitAfterSettingsCompleteEvent;
        private static NSFEvent oClearFaultsEvent;
        private static NSFEvent oAllAxisHomedEvent;
        private static NSFEvent oEStopEvent;
        private static NSFEvent oBypassKeyEnabledEvent;
        private static NSFEvent oAbortEvent;
        private static NSFEvent oMotionFaultEvent;
        private static NSFEvent oSensorFaultEvent;
        private static NSFEvent oMoveCompleteEvent;
        private static NSFEvent oAbortCompleteEvent;
        private static NSFEvent oCommitEvent;
        private static NSFEvent oLoadEvent;
        private static NSFEvent oDoorClosedEvent;
        private static NSFEvent oDoorOpenedEvent;
        private static NSFEvent oCreateRefFileEvent;
        private static NSFEvent oRefFileCompleteEvent;
        private static NSFEvent oStartEvent;
        private static NSFEvent oTestCompleteEvent;
        private static NSFEvent oWriteCompleteEvent;


        // *******************************************
        // End of State Machine NSF Event Definitions
        // *******************************************


        // ***********************************
        // State Machine NSF State Definitions
        // ***********************************

        // State Machine Upper Level SubState Definitions
        private NSFInitialState   oCaliforniaStateMachineInitialState;
        private NSFCompositeState oSystemPoweredState;

        // SystemPoweredState SubState Definitions
        private NSFInitialState   oSystemPoweredInitialState;
        private NSFCompositeState oInitState;
        private NSFCompositeState oInitAfterSettingsState;
        private NSFCompositeState oHomeAllAxisState;
        private NSFChoiceState    oIsHomedChoiceState;
        private NSFChoiceState    oIsCriticalAlarmChoiceState;
        private NSFCompositeState oFaultedState;
        private NSFCompositeState oPrepareState;
        private NSFCompositeState oInspectingState;

        // Prepare State SubState Definitions
        private NSFInitialState   oPrepareInitialState;
        private NSFCompositeState oIdleState;
        private NSFCompositeState oLoadOrUnLoadState;
        private NSFCompositeState oCreateReferenceFileState;
        private NSFCompositeState oWaitForDoorClose2State;
        private NSFCompositeState oCouponNotLoadedErrorState;
        private NSFChoiceState oIsDoorClosed4ChoiceState;
        private NSFChoiceState oIsChipLoadedChoiceState;

        // Inspecting State SubState Definitions
        private NSFInitialState oInspectingInitialState;
        private NSFCompositeState oMoveToLLState;
        private NSFCompositeState oTestLLState;
        private NSFCompositeState oReviewState;
        private NSFCompositeState oAbortInspectingState;
        private NSFCompositeState oWriteReportState;
        private NSFChoiceState oIsMoreLLToTestChoiceState;

        // Idle State SubState Definitions
        private NSFInitialState oIdleInitialState;
        private NSFCompositeState oRefFrameNeededState;
        private NSFCompositeState oRefFrameUpdatedState;
        private NSFChoiceState oIsRefFrameUpdatedTodayChoiceState;

        // LoadOrUnLoad State SubState Definitions
        private NSFInitialState oLoadOrUnLoadInitialState;
        private NSFCompositeState oWaitForDoorCloseState;
        private NSFCompositeState oMoveToLoadPosState;
        private NSFCompositeState oLoadCouponState;
        private NSFCompositeState oWaitForDoorClose3State;
        private NSFChoiceState oIsDoorClosedChoiceState;
        private NSFChoiceState oIsDoorClosed3ChoiceState;

        // CreateReferenceFileState SubState Definitions
        private NSFInitialState oCreateReferenceFileInitialState;
        private NSFCompositeState oWaitForDoorClose4State;
        private NSFCompositeState oMoveToLens1PosState;
        private NSFCompositeState oRemoveCouponFirstErrorState;
        private NSFCompositeState oCreateRefFileState;
        private NSFChoiceState oIsDoorClosed2ChoiceState;
        private NSFChoiceState oIsCouponLoadedChoiceState;
        private NSFChoiceState oIsCouponLoaded2ChoiceState;

        // ******************************************
        // End of State Machine NSF State Definitions
        // ******************************************


        // ******************************
        // State Machine NSF Transitions
        // ******************************

        // State Machine Upper Level Transitions
        private NSFExternalTransition CaliforniaStateMachineInitialToSystemPoweredTransition;

        // SystemPoweredState Transitions
        private NSFExternalTransition SystemPoweredInitialToInitTransition;
        private NSFExternalTransition SystemPoweredToInitTransition;
        private NSFExternalTransition SystemPoweredToFaultTransition;
        private NSFExternalTransition SystemPoweredToFaultTransition2;
        private NSFExternalTransition SystemPoweredToFaultTransition3;
        private NSFExternalTransition SystemPoweredToFaultTransition4;

        // InitState Transitions
        private NSFExternalTransition InitToInitAfterSettingsTransition;

        // InitAfterSettingsState Transitions
        private NSFExternalTransition InitAfterSettingsToIsCriticalAlarmChoiceTransition;

        // IsCriticalAlarmChoiceState Transitions
        private NSFExternalTransition IsCriticalAlarmChoiceToFaultedTransition;
        private NSFExternalTransition IsCriticalAlarmChoiceToIsHomedChoiceTransition;

        // IsHomedChoiceState Transitions
        private NSFExternalTransition IsHomedChoiceToPrepareTransition;
        private NSFExternalTransition IsHomedChoiceToHomeAllAxisTransition;

        // HomeAllAxisState Transitions
        private NSFExternalTransition HomeAllAxisToIsCriticalAlarmChoiceTransition;

        // FaultedState Transitions
        private NSFExternalTransition FaultedToIsCriticalAlarmChoiceTransition;

        // PrepareState Transitions
        private NSFExternalTransition PrepareInitialToIdleTransition;

        // IdleState Transitions
        private NSFExternalTransition IdleInitialToIsRefFrameUpdatedTodayChoiceTransition;
        private NSFExternalTransition IdleToLoadOrUnLoadTransition;
        private NSFExternalTransition IdleToIsDoorClosed4ChoiceTransition;
        private NSFExternalTransition IdleToCreateReferenceFileTransition;

        // IsRefFrameUpdatedTodayChoice Transitions
        private NSFExternalTransition IsRefFrameUpdatedTodayToRefFrameUpdatedTransition;
        private NSFExternalTransition IsRefFrameUpdatedTodayToRefFrameNeededTransition;

        // IsDoorClosed4 Transitions
        private NSFExternalTransition IsDoorClosed4ToWaitForDoorClose2Transition;
        private NSFExternalTransition IsDoorClosed4ToIsCouponLoadedTransition;

        // IsCouponLoaded Transitions
        private NSFExternalTransition IsCouponLoadedChoiceToCouponNotLoadedErrorTransition;
        private NSFExternalTransition IsCouponLoadedChoiceToInspectingTransition;

        // WaitForDoorClose2 Transitions
        private NSFExternalTransition WaitForDoorClose2ToIsCouponLoadedChoiceTransition;

        // CouponNotLoadedError Transitions
        private NSFExternalTransition CouponNotLoadedErrorToIdleTransition;

        // LoadOrUnLoadInitial Transitions
        private NSFExternalTransition LoadOrUnLoadInitialToIsDoorClosedChoiceTransition;

        // IsDoorClosedChoice Transitions
        private NSFExternalTransition IsDoorClosedChoiceToWaitForDoorCloseTransition;
        private NSFExternalTransition IsDoorClosedChoiceToMoveToLoadPosTransition;

        // WaitForDoorClose Transitions
        private NSFExternalTransition WaitForDoorCloseToMoveToLoadPosTransition;

        // MoveToLoadPos Transitions
        private NSFExternalTransition MoveToLoadPosToLoadCouponTransition;

        // LoadCoupon Transitions
        private NSFExternalTransition LoadCouponToIsDoorClosed3Transition;

        // IsDoorClosed3Choice Transitions
        private NSFExternalTransition IsDoorClosed3ChoiceToWaitForDoorClose3Transition;
        private NSFExternalTransition IsDoorClosed3ChoiceToIdleTransition;

        // WaitForDoorClose3 Transitions
        private NSFExternalTransition WaitForDoorClose3ToIdleTransition;

        // CreateReferenceFile Transitions
        private NSFExternalTransition CreateReferenceFileInitialToIsCouponLoadedChoiceTransition;

        // CreateRefFileTransitions
        private NSFExternalTransition CreateRefFileToIdleTransition;

        // IsCouponLoadedChoice Transitions
        private NSFExternalTransition IsCouponLoaded2ChoiceToIsDoorClosed2ChoiceTransition;
        private NSFExternalTransition IsCouponLoaded2ChoiceToRemoveCouponFirstErrorTransition;

        // IsDoorClosed2Choice Transitions
        private NSFExternalTransition IsDoorClosed2ChoiceToMoveToLens1PosTransition;
        private NSFExternalTransition IsDoorClosed2ChoiceToWaitForDoorClose4Transition;

        // WaitForDoorClose4State Transitions
        private NSFExternalTransition WaitForDoorClose4ToMoveToLens1PosTransition;

        // MoveToLens1Pos Transitions
        private NSFExternalTransition MoveToLens1PosToMoveToCreateRefFileTransition;

        // RemoveCouponFirstError Transitions
        private NSFExternalTransition RemoveCouponFirstErrorToIdleTransition;

        // Inspecting Transitions
        private NSFExternalTransition InspectingInitialToMoveToLLStateTransition;
        private NSFExternalTransition InspectingToFaultedTransition;
        private NSFExternalTransition InspectingToAbortInspectingTransition;

        // MoveToLL Transitions
        private NSFExternalTransition MoveToLLToTestLLTransition;

        // TestLL Transitions
        private NSFExternalTransition TestLLToIsMoreLLToTestChoiceTransition;

        // IsMoreLLToTestChoice Transitions
        private NSFExternalTransition IsMoreLLToTestChoiceToMoveToLLTransition;
        private NSFExternalTransition IsMoreLLToTestChoiceToReviewTransition;

        // Review Transitions
        private NSFExternalTransition ReviewToWriteReportTransition;
        private NSFExternalTransition ReviewToWriteReportTransition2;

        // WriteReport Transitions
        private NSFExternalTransition WriteReportToPrepareTransition;

        // AbortInspecting Transitions
        private NSFExternalTransition AbortInspectingToWriteReportTransition;


        // ************************************
        // End of State Machine NFS Transitions
        // ************************************
        #endregion State Machine Fields

        #endregion Fields

        #region Properties
        public string SystemState { get { return m_strSystemState; } }

        public bool AbortingScan { get; private set; }

        public bool ScanInProgress { get; private set; }

        private bool SimulationModeEnabled
        {
            get
            {
                return m_bSimulationMode;
            }
            set
            {
                m_bSimulationMode = value;
            }
        }

        public bool InitAfterSettingsComplete
        {
            get { return m_bInitAfterSettingsComplete; }
        }

        public bool InitComplete
        {
            get { return m_bInitComplete; }
        }

        public string LastOperatorPrompt
        {
            get { return m_strLastOperatorPrompt; }
        }

        #endregion Properties

        #region Events
        public event StateChangeEventHandler eventStateChange;
        #endregion Events

        #region Constructors
        public CCaliforniaStateMachine(string strName, ICaliforniaSystem iCaliforniaSystem)
            : base(strName, new NSFEventThread(strName))
        {
            // Capture reference to the parent system for calling back to perform system functions
            m_iCaliforniaSystem = iCaliforniaSystem;

            // Init State Machine
            CreateStateMachine();

            // Init Operator Prompts
            CreateOperatorPrompts();
        }

        ~CCaliforniaStateMachine()
        {
        }
        #endregion Constructors

        #region Methods
        public bool StartStateMachine()
        {
            // Support StateMachine specific log
            NSFTraceLog.PrimaryTraceLog.Enabled = true;

            StateChangeActions += handleStateChange;

            // Start State machine
            startStateMachine();

            return true;
        }

        public bool StopStateMachine()
        {
            stopStateMachine();

            // Save trace log
            NSFTraceLog.PrimaryTraceLog.saveLog("CaliforniaStateMachineLog.xml");

            NSFEnvironment.terminate();
            return true;
        }

        public bool HandleEvent(CaliforniaSystemEventsEnum eCaliforniaEvent, object oEventData)
        {
            bool bEventHandled = false;

            if (!m_dictEventByEnum.ContainsKey(eCaliforniaEvent))
            {
                ms_iLogger.Log(ELogLevel.Error, "Invalid Event passed to HandleEvent!");
                return false;
            }

            try
            {
                // Get event from enum passed in
                NSFEvent oEvent = m_dictEventByEnum[eCaliforniaEvent];

                // Handle event data
                if (oEventData == null)
                {
                    // queue the event to the StateMachine
                    queueEvent(oEvent);
                    bEventHandled = true;
                }
                else
                {
                    // Queue the event and EventData to the state machine
                    CStateMachineEventData oStateMachineEventData = new CStateMachineEventData(oEventData);
                    queueEvent(oEvent, oStateMachineEventData);
                    bEventHandled = true;
                }

                // Log Event
                if (bEventHandled)
                {
                    string strMsg = string.Format(" Received Event {0}!", eCaliforniaEvent.ToString());
                    ms_iLogger.Log(ELogLevel.Info, strMsg);
                }
                else
                {
                    ms_iLogger.Log(ELogLevel.Error, "Invalid event type passed to HandleEvent!");
                }

                return true;
            }
            catch (Exception ex)
            {
                string strMsg = String.Format("Error!  Unable to handle handle event {0}!", eCaliforniaEvent.ToString());
                ms_iLogger.Log(ELogLevel.Error, strMsg);
            }

            return false;
        }

        private bool CreateStateMachine()
        {
            try
            {
                // State Machine Components
                // Define and initialize in the order:
                //   1) Events
                //   2) Regions and states, from outer to inner
                //   3) Transitions, ordered internal, local, external
                //   4) Group states and transitions within a region together.

                // Instantiate All State Machine Events.
                oInitEvent = new NSFEvent("InitEvent", this, this);
                oInitCompleteEvent = new NSFEvent("InitComplete", this, this);
                oInitAfterSettingsCompleteEvent = new NSFEvent("InitAfterSettingsComplete", this, this);
                oClearFaultsEvent = new NSFEvent("ClearFaultsEvent", this, this);
                oAllAxisHomedEvent = new NSFEvent("AllAxisHomedEvent", this, this);
                oEStopEvent = new NSFEvent("EStopEvent", this, this);
                oAbortEvent = new NSFEvent("AbortEvent", this, this);
                oBypassKeyEnabledEvent = new NSFEvent("ByPassKeyEnabledEvent", this, this);
                oMotionFaultEvent = new NSFEvent("MotionFaultEvent", this, this);
                oSensorFaultEvent = new NSFEvent("SensorFaultEvent", this, this);
                oMoveCompleteEvent = new NSFEvent("MoveCompleteEvent", this, this);
                oAbortCompleteEvent = new NSFEvent("AbortCompleteEvent", this, this);
                oCommitEvent = new NSFEvent("CommitEvent", this, this);
                oLoadEvent = new NSFEvent("LoadEvent", this, this);
                oDoorClosedEvent = new NSFEvent("DoorClosedEvent", this, this);
                oDoorOpenedEvent = new NSFEvent("DoorOpenedEvent", this, this);
                oCreateRefFileEvent = new NSFEvent("CreateRefFileEvent", this, this);
                oRefFileCompleteEvent = new NSFEvent("RefFileCompleteEvent", this, this);
                oStartEvent = new NSFEvent("StartEvent", this, this);
                oTestCompleteEvent = new NSFEvent("TestCompleteEvent", this, this);
                oWriteCompleteEvent = new NSFEvent("WriteCompleteEvent", this, this);

                // Create dictionary mapping event enum to actual event
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.InitCompleteEvent, oInitCompleteEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.InitAfterSettingsCompleteEvent, oInitAfterSettingsCompleteEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.ClearFaultsEvent, oClearFaultsEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.AllAxisHomedEvent, oAllAxisHomedEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.EStopEvent, oEStopEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.AbortEvent, oAbortEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.BypassKeyEnabledEvent, oBypassKeyEnabledEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.MotionFaultEvent, oMotionFaultEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.SensorFaultEvent, oSensorFaultEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.MoveCompleteEvent, oMoveCompleteEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.AbortCompleteEvent, oAbortCompleteEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.CommitEvent, oCommitEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.LoadEvent, oLoadEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.DoorClosedEvent, oDoorClosedEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.DoorOpenedEvent, oDoorOpenedEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.CreateRefFileEvent, oCreateRefFileEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.RefFileCompleteEvent, oRefFileCompleteEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.StartEvent, oStartEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.TestCompleteEvent, oTestCompleteEvent);
                m_dictEventByEnum.Add(CaliforniaSystemEventsEnum.WriteCompleteEvent, oWriteCompleteEvent);
 
                // ****************************************
                // Create the states for the State Machine
                // ****************************************


                // State Machine Upper Level SubState Instantiations. Create all SubStates of the Upper Level.
                oCaliforniaStateMachineInitialState = new NSFInitialState(EState.CaliforniaStateMachineInitial.ToString(), this);
                oSystemPoweredState        = new NSFCompositeState(EState.SystemPowered.ToString(), this, null, null);

                // SystemPoweredState SubState Instantiations. Create all SubStates of SystemPoweredState
                oSystemPoweredInitialState = new NSFInitialState(EState.SystemPoweredInitial.ToString(), oSystemPoweredState);
                oInitState = new NSFCompositeState(EState.Init.ToString(), oSystemPoweredState, InitStateEntryActions, InitStateExitActions);
                oInitAfterSettingsState    = new NSFCompositeState(EState.InitAfterSettings.ToString(), oSystemPoweredState, null, InitAfterSettingsStateExitActions);
                oHomeAllAxisState          = new NSFCompositeState(EState.HomeAllAxis.ToString(), oSystemPoweredState, HomeAllAxis, null);
                oIsHomedChoiceState        = new NSFChoiceState(EState.IsHomedChoice.ToString(), oSystemPoweredState);
                oIsCriticalAlarmChoiceState = new NSFChoiceState(EState.IsCriticalAlarmChoice.ToString(), oSystemPoweredState);
                oFaultedState              = new NSFCompositeState(EState.Faulted.ToString(), oSystemPoweredState, FaultedStateEntryActions, null);
                oPrepareState                = new NSFCompositeState(EState.Prepare.ToString(),   oSystemPoweredState, PrepareStateEntryActions, PrepareStateExitActions);
                oInspectingState = new NSFCompositeState(EState.Inspecting.ToString(), oSystemPoweredState, InspectingStateEntryActions, InspectingStateExitActions);

                // InitState SubState Instantiations. Create all SubStates of InitState.
                // None

                // InitAfterSettingsState SubState Instantiations. Create all SubStates of InitAfterSettingsState
                // None

                // HomeAllAxisState SubState Instantiations. Create all SubStates of HomeAllAxisState.
                // None

                // FaultedState SubState Instantiations. Create all SubStates of FaultedState.
                // None

                // PrepareState SubState Instantiations. Create all SubStates 
                oPrepareInitialState = new NSFInitialState(EState.PrepareInitial.ToString(), oPrepareState);
                oIdleState = new NSFCompositeState(EState.Idle.ToString(), oPrepareState, IdleStateEntryActions, IdleStateExitActions);
                oLoadOrUnLoadState = new NSFCompositeState(EState.LoadOrUnLoad.ToString(), oPrepareState, LoadOrUnLoadStateEntryActions, LoadOrUnLoadStateExitActions);
                oCreateReferenceFileState = new NSFCompositeState(EState.CreateReferenceFile.ToString(), oPrepareState, CreateReferenceFileStateEntryActions, CreateReferenceFileStateExitActions);
                oWaitForDoorClose2State = new NSFCompositeState(EState.WaitForDoorClose2.ToString(), oPrepareState, WaitForDoorClose2StateEntryActions, WaitForDoorClose2StateExitActions);
                oCouponNotLoadedErrorState = new NSFCompositeState(EState.CouponNotLoadedError.ToString(), oPrepareState, CouponNotLoadedErrorStateEntryActions, CouponNotLoadedErrorStateExitActions);
                oIsDoorClosed4ChoiceState = new NSFChoiceState(EState.IsDoorClosed4Choice.ToString(), oPrepareState);
                oIsChipLoadedChoiceState = new NSFChoiceState(EState.IsChipLoadedChoice.ToString(), oPrepareState);
                oIsCouponLoadedChoiceState = new NSFChoiceState(EState.IsCouponLoadedChoice.ToString(), oPrepareState);

                // Inspecting State SubState Instantiations. Create all SubStates 
                oInspectingInitialState = new NSFInitialState(EState.InspectingInitial.ToString(), oInspectingState);
                oMoveToLLState = new NSFCompositeState(EState.MoveToLL.ToString(), oInspectingState, MoveToLLStateEntryActions, MoveToLLStateExitActions);
                oTestLLState = new NSFCompositeState(EState.TestLL.ToString(), oInspectingState, TestLLStateEntryActions, TestLLStateExitActions);
                oReviewState = new NSFCompositeState(EState.Review.ToString(), oInspectingState, ReviewStateEntryActions, ReviewStateExitActions);
                oAbortInspectingState = new NSFCompositeState(EState.AbortInspecting.ToString(), oInspectingState, AbortInspectingStateEntryActions, AbortInspectingStateExitActions);
                oWriteReportState = new NSFCompositeState(EState.WriteReport.ToString(), oInspectingState, WriteReportStateEntryActions, WriteReportStateExitActions);
                oIsMoreLLToTestChoiceState = new NSFChoiceState(EState.IsMoreLLToTestChoice.ToString(), oInspectingState);

                // Idle State SubState Instantiations. Create all SubStates 
                oIdleInitialState = new NSFInitialState(EState.IdleInitial.ToString(), oIdleState);
                oRefFrameNeededState = new NSFCompositeState(EState.RefFrameNeeded.ToString(), oIdleState, RefFrameNeededStateEntryActions, RefFrameNeededStateExitActions);
                oRefFrameUpdatedState = new NSFCompositeState(EState.RefFrameUpdated.ToString(), oIdleState, RefFrameUpdatedStateEntryActions, RefFrameUpdatedStateExitActions);
                oIsRefFrameUpdatedTodayChoiceState = new NSFChoiceState(EState.IsRefFrameUpdatedTodayChoice.ToString(), oIdleState);

                // LoadOrUnLoad State SubState Instantiations. Create all SubStates 
                oLoadOrUnLoadInitialState = new NSFInitialState(EState.LoadOrUnLoadInitial.ToString(), oLoadOrUnLoadState);
                oWaitForDoorCloseState = new NSFCompositeState(EState.WaitForDoorClose.ToString(), oLoadOrUnLoadState, WaitForDoorCloseStateEntryActions, WaitForDoorCloseStateExitActions);
                oMoveToLoadPosState = new NSFCompositeState(EState.MoveToLoadPos.ToString(), oLoadOrUnLoadState, MoveToLoadPosStateEntryActions, MoveToLoadPosStateExitActions);
                oLoadCouponState = new NSFCompositeState(EState.LoadCoupon.ToString(), oLoadOrUnLoadState, LoadCouponStateEntryActions, LoadCouponStateExitActions);
                oWaitForDoorClose3State = new NSFCompositeState(EState.WaitForDoorClose3.ToString(), oLoadOrUnLoadState, WaitForDoorClose3StateEntryActions, WaitForDoorClose3StateExitActions);
                oIsDoorClosedChoiceState = new NSFChoiceState(EState.IsDoorClosedChoice.ToString(), oLoadOrUnLoadState);
                oIsDoorClosed3ChoiceState = new NSFChoiceState(EState.IsDoorClosed3Choice.ToString(), oLoadOrUnLoadState);

                // CreateReferenceFile State SubState Instantiations. Create all SubStates 
                oCreateReferenceFileInitialState = new NSFInitialState(EState.CreateReferenceFileInitial.ToString(), oCreateReferenceFileState);
                oWaitForDoorClose4State = new NSFCompositeState(EState.WaitForDoorClose4.ToString(), oCreateReferenceFileState, WaitForDoorClose4StateEntryActions, WaitForDoorClose4StateExitActions);
                oMoveToLens1PosState = new NSFCompositeState(EState.MoveToLens1Pos.ToString(), oCreateReferenceFileState, MoveToLens1PosStateEntryActions, MoveToLens1PosStateExitActions);
                oRemoveCouponFirstErrorState = new NSFCompositeState(EState.RemoveCouponFirstError.ToString(), oCreateReferenceFileState, RemoveCouponFirstErrorStateEntryActions, RemoveCouponFirstErrorStateExitActions);
                oCreateRefFileState = new NSFCompositeState(EState.CreateRefFile.ToString(), oCreateReferenceFileState, CreateRefFileStateEntryActions, CreateRefFileStateExitActions);
                oIsDoorClosed2ChoiceState = new NSFChoiceState(EState.IsDoorClosed2Choice.ToString(), oCreateReferenceFileState);
                oIsCouponLoaded2ChoiceState = new NSFChoiceState(EState.IsCouponLoadedChoice.ToString(), oCreateReferenceFileState);

                // *************************************************
                // End of Creating the states for the State Machine
                // *************************************************


                // *********************************************
                // Create the Transitions for the State Machine
                // *********************************************

                // Top Level Transitions
                CaliforniaStateMachineInitialToSystemPoweredTransition = new NSFExternalTransition("CaliforniaStateMachineInitialToSystemPowered", oCaliforniaStateMachineInitialState, oSystemPoweredState, null, null, null);

                // SystemPoweredState Transitions
                SystemPoweredInitialToInitTransition = new NSFExternalTransition("SystemPoweredInitialToInit", oSystemPoweredInitialState, oInitState, null, null, null);
                SystemPoweredToInitTransition = new NSFExternalTransition("SystemPoweredToInit", oSystemPoweredState, oInitState, oInitEvent, null, null);
                SystemPoweredToFaultTransition = new NSFExternalTransition("SystemPoweredToFault", oSystemPoweredState, oFaultedState, oBypassKeyEnabledEvent, IsOperatorLogin, RaiseBypassKeyOnMomentaryAlarm);
                SystemPoweredToFaultTransition2 = new NSFExternalTransition("SystemPoweredToFault2", oSystemPoweredState, oFaultedState, oEStopEvent, null, RaiseEStopMomentaryAlarm);
                SystemPoweredToFaultTransition3 = new NSFExternalTransition("SystemPoweredToFault3", oSystemPoweredState, oFaultedState, oSensorFaultEvent, null, RaiseSensorFaultMomentaryAlarm);
                SystemPoweredToFaultTransition4 = new NSFExternalTransition("SystemPoweredToFault4", oSystemPoweredState, oFaultedState, oMotionFaultEvent, null, RaiseMotionFaultMomentaryAlarm);

                // InitState Transitions
                InitToInitAfterSettingsTransition = new NSFExternalTransition("InitToInitAfterSettings", oInitState, oInitAfterSettingsState, oInitCompleteEvent, null, ProcessInitAfterSettings);

                // InitAfterSettingsState Transitions
                InitAfterSettingsToIsCriticalAlarmChoiceTransition = new NSFExternalTransition("InitAfterSettingsToIsCriticalAlarmChoice", oInitAfterSettingsState, oIsCriticalAlarmChoiceState, oInitAfterSettingsCompleteEvent, null, null);

                // IsCriticalAlarmChoiceState Transitions
                IsCriticalAlarmChoiceToFaultedTransition = new NSFExternalTransition("IsFaultedChoiceToFaulted", oIsCriticalAlarmChoiceState, oFaultedState, null, IsCriticalAlarms, null);
                IsCriticalAlarmChoiceToIsHomedChoiceTransition = new NSFExternalTransition("IsCriticalAlarmChoiceToIsHomedChoice", oIsCriticalAlarmChoiceState, oIsHomedChoiceState, null, Else, null);

                // IsHomedChoiceState Transitions
                IsHomedChoiceToPrepareTransition = new NSFExternalTransition("IsHomedChoiceToPrepare", oIsHomedChoiceState, oPrepareState, null, IsHomed, null);
                IsHomedChoiceToHomeAllAxisTransition = new NSFExternalTransition("IsHomedChoiceToHomeAllAxis", oIsHomedChoiceState, oHomeAllAxisState, null, Else, null);

                // HomeAllAxisState Transitions                              
                HomeAllAxisToIsCriticalAlarmChoiceTransition = new NSFExternalTransition("HomeAllAxisToIsCriticalAlarm", oHomeAllAxisState, oIsCriticalAlarmChoiceState, oAllAxisHomedEvent, null, null);

                // FaultedState Transitions
                FaultedToIsCriticalAlarmChoiceTransition = new NSFExternalTransition("FaultedToIsCriticalAlarmChoice", oFaultedState, oIsCriticalAlarmChoiceState, oClearFaultsEvent, null, null);

                // PrepareState Transitions
                PrepareInitialToIdleTransition = new NSFExternalTransition("PrepareInitialToIdle", oPrepareInitialState, oIdleState, null, null, null);

                // IdleState Transitions
                IdleInitialToIsRefFrameUpdatedTodayChoiceTransition = new NSFExternalTransition("IdleInitialToIsRefFrameUpdatedTodayChoice", oIdleInitialState, oIsRefFrameUpdatedTodayChoiceState, null, null, null);
                IdleToLoadOrUnLoadTransition = new NSFExternalTransition("IdleToLoadOrUnLoad", oIdleState, oLoadOrUnLoadState, oLoadEvent, null, null);
                IdleToIsDoorClosed4ChoiceTransition = new NSFExternalTransition("IdleToIsDoorClosed4Choice", oIdleState, oIsDoorClosed4ChoiceState, oStartEvent, IsBypassKeyOk, null);
                IdleToCreateReferenceFileTransition = new NSFExternalTransition("IdleToCreateReferenceFile", oIdleState, oCreateReferenceFileState, oCreateRefFileEvent, null, null);

                //oIsRefFrameUpdatedTodayChoice Transitions
                IsRefFrameUpdatedTodayToRefFrameUpdatedTransition = new NSFExternalTransition("IsRefFrameUpdatedTodayToRefFrameUpdated", oIsRefFrameUpdatedTodayChoiceState, oRefFrameUpdatedState, null, IsRefFrameUpdatedToday, null);
                IsRefFrameUpdatedTodayToRefFrameNeededTransition = new NSFExternalTransition("IsRefFrameUpdatedTodayToRefFrameNeeded", oIsRefFrameUpdatedTodayChoiceState, oRefFrameNeededState, null, Else, null);

                //IsDoorClosed4 Transitions
                IsDoorClosed4ToIsCouponLoadedTransition = new NSFExternalTransition("IsDoorClosed4ToIsCouponLoadedChoice", oIsDoorClosed4ChoiceState, oIsCouponLoadedChoiceState, null, IsDoorClosed, null);
                IsDoorClosed4ToWaitForDoorClose2Transition = new NSFExternalTransition("IsDoorClosed4ToWaitForDoorClose2", oIsDoorClosed4ChoiceState, oWaitForDoorClose2State, null, Else, null);

                //IsCouponLoaded Transitions
                IsCouponLoadedChoiceToInspectingTransition = new NSFExternalTransition("IsCouponLoadedChoiceToInspecting", oIsCouponLoadedChoiceState, oInspectingState, null, IsCouponLoaded, null);
                IsCouponLoadedChoiceToCouponNotLoadedErrorTransition = new NSFExternalTransition("IsCouponLoadedChoiceToCouponNotLoadedError", oIsCouponLoadedChoiceState, oCouponNotLoadedErrorState, null, Else, null);

                //WaitForDoorClose2 Transitions
                WaitForDoorClose2ToIsCouponLoadedChoiceTransition = new NSFExternalTransition("WaitForDoorClose2ToIsCouponLoadedChoice", oWaitForDoorClose2State, oIsCouponLoadedChoiceState, oDoorClosedEvent, null, null);

                //CouponNotLoadedError Transitions
                CouponNotLoadedErrorToIdleTransition = new NSFExternalTransition("CouponNotLoadedErrorToIdle", oCouponNotLoadedErrorState, oIdleState, oAbortEvent, null, null);

                //LoadOrUnLoadInitial Transitions
                LoadOrUnLoadInitialToIsDoorClosedChoiceTransition = new NSFExternalTransition("LoadOrUnLoadInitialToIsDoorClosedChoice", oLoadOrUnLoadInitialState, oIsDoorClosedChoiceState, null, null, null);

                //IsDoorClosedChoice Transitions
                IsDoorClosedChoiceToMoveToLoadPosTransition = new NSFExternalTransition("IsDoorClosedChoiceToMoveToLoadPos", oIsDoorClosedChoiceState, oMoveToLoadPosState, null, IsDoorClosedOrBypassKeyOnAndValid, null);
                IsDoorClosedChoiceToWaitForDoorCloseTransition = new NSFExternalTransition("IsDoorClosedChoiceToWaitForDoorClose", oIsDoorClosedChoiceState, oWaitForDoorCloseState, null, Else, null);

                //WaitForDoorClose Transitions
                WaitForDoorCloseToMoveToLoadPosTransition = new NSFExternalTransition("WaitForDoorCloseToMoveToLoadPos", oWaitForDoorCloseState, oMoveToLoadPosState, oDoorClosedEvent, null, null);

                //MoveToLoadPos Transitions
                MoveToLoadPosToLoadCouponTransition = new NSFExternalTransition("MoveToLoadPosToLoadCoupon", oMoveToLoadPosState, oLoadCouponState, oMoveCompleteEvent, null, null);

                //LoadCoupon Transitions
                LoadCouponToIsDoorClosed3Transition = new NSFExternalTransition("LoadCouponToIsDoorClosed3", oLoadCouponState, oIsDoorClosed3ChoiceState, oCommitEvent, null, null);

                //IsDoorClosed3Choice Transitions
                IsDoorClosed3ChoiceToIdleTransition = new NSFExternalTransition("IsDoorClosed3ChoiceToIdle", oIsDoorClosed3ChoiceState, oIdleState, null, IsDoorClosedOrBypassKeyOnAndValid, null);
                IsDoorClosed3ChoiceToWaitForDoorClose3Transition = new NSFExternalTransition("IsDoorClosed3ChoiceToWaitForDoorClose3", oIsDoorClosed3ChoiceState, oWaitForDoorClose3State, null, Else, null);

                //WaitForDoorClose3 Transitions
                WaitForDoorClose3ToIdleTransition = new NSFExternalTransition("WaitForDoorClose3ToIdle", oWaitForDoorClose3State, oIdleState, oDoorClosedEvent, null, null);

                // CreateReferenceFile Transitions
                CreateReferenceFileInitialToIsCouponLoadedChoiceTransition = new NSFExternalTransition("CreateReferenceFileInitialToIsCouponLoadedChoice", oCreateReferenceFileInitialState, oIsCouponLoaded2ChoiceState, null, null, null);

                // CreateRefFileTransitions
                CreateRefFileToIdleTransition = new NSFExternalTransition("CreateRefFileToIdle", oCreateRefFileState, oIdleState, oRefFileCompleteEvent, null, null);

                // IsCouponLoadedChoice Transitions
                IsCouponLoaded2ChoiceToRemoveCouponFirstErrorTransition = new NSFExternalTransition("IsCouponLoaded2ChoiceToRemoveCouponFirstError", oIsCouponLoaded2ChoiceState, oRemoveCouponFirstErrorState, null, IsCouponLoaded, null);
                IsCouponLoaded2ChoiceToIsDoorClosed2ChoiceTransition = new NSFExternalTransition("IsCouponLoaded2ChoiceToIsDoorClosed2Choice", oIsCouponLoaded2ChoiceState, oIsDoorClosed2ChoiceState, null, Else, null);

                // IsDoorClosed2Choice Transitions
                IsDoorClosed2ChoiceToMoveToLens1PosTransition = new NSFExternalTransition("IsDoorClosed2ChoiceToMoveToLens1Pos", oIsDoorClosed2ChoiceState, oMoveToLens1PosState, null, IsDoorClosed, null);
                IsDoorClosed2ChoiceToWaitForDoorClose4Transition = new NSFExternalTransition("IsDoorClosed2ChoiceToWaitForDoorClose4", oIsDoorClosed2ChoiceState, oWaitForDoorClose4State, null, Else, null);

                // WaitForDoorClose4State Transitions
                WaitForDoorClose4ToMoveToLens1PosTransition = new NSFExternalTransition("WaitForDoorClose4ToMoveToLens1Pos", oWaitForDoorClose4State, oMoveToLens1PosState, oDoorClosedEvent, null, null);

                // MoveToLens1Pos Transitions
                MoveToLens1PosToMoveToCreateRefFileTransition = new NSFExternalTransition("MoveToLens1PosToMoveToCreateRefFile", oMoveToLens1PosState, oCreateRefFileState, oMoveCompleteEvent, null, null);

                // RemoveCouponFirstError Transitions
                RemoveCouponFirstErrorToIdleTransition = new NSFExternalTransition("RemoveCouponFirstErrorToIdle", oRemoveCouponFirstErrorState, oIdleState, oAbortEvent, null, null);

                // RemoveCouponFirstError Transitions
                InspectingInitialToMoveToLLStateTransition = new NSFExternalTransition("InspectingInitialToMoveToLLState", oInspectingInitialState, oMoveToLLState, null, null, null);
                InspectingToFaultedTransition = new NSFExternalTransition("InspectingToFaulted", oInspectingState, oFaultedState, oDoorOpenedEvent, IsBypassKeyOk, RaiseDoorOpenedMomentaryAlarm);
                InspectingToAbortInspectingTransition = new NSFExternalTransition("InspectingToAbortInspecting", oInspectingState, oAbortInspectingState, oAbortEvent, null, AbortInspectingAsync);

                // RemoveCouponFirstError Transitions
                MoveToLLToTestLLTransition = new NSFExternalTransition("MoveToLLToTestLL", oMoveToLLState, oTestLLState, oMoveCompleteEvent, null, null);

                // RemoveCouponFirstError Transitions
                TestLLToIsMoreLLToTestChoiceTransition = new NSFExternalTransition("TestLLToIsMoreLLToTestChoice", oTestLLState, oIsMoreLLToTestChoiceState, oTestCompleteEvent, null, null);

                //IsMoreLLToTestChoice Transitions
                IsMoreLLToTestChoiceToMoveToLLTransition = new NSFExternalTransition("IsMoreLLToTestChoiceToMoveToLL", oIsMoreLLToTestChoiceState, oMoveToLLState, null, IsMoreLLToTest, null);
                IsMoreLLToTestChoiceToReviewTransition = new NSFExternalTransition("IsMoreLLToTestChoiceToReview", oIsMoreLLToTestChoiceState, oReviewState, null, Else, null);

                // RemoveCouponFirstError Transitions
                ReviewToWriteReportTransition = new NSFExternalTransition("ReviewToWriteReport", oReviewState, oWriteReportState, oCommitEvent, null, WriteMeasResultsMfgDbAsync);
                ReviewToWriteReportTransition2 = new NSFExternalTransition("ReviewToWriteReport2", oReviewState, oWriteReportState, oAbortEvent, null, WriteMeasResultsLocalAsync);

                // WriteReport Transitions
                WriteReportToPrepareTransition = new NSFExternalTransition("WriteReportToPrepare", oWriteReportState, oPrepareState, oWriteCompleteEvent, null, null);

                // AbortInspecting Transitions
                AbortInspectingToWriteReportTransition = new NSFExternalTransition("AbortInspectingToWriteReport", oAbortInspectingState, oWriteReportState, oAbortCompleteEvent, null, WriteMeasResultsLocalAsync);

                // ******************************************************
                // End of creating the Transitions for the State Machine
                // ******************************************************

                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Unable to initiate the CLEO State Machine!", ex);
            }

            return false;
        }

        private void CreateOperatorPrompts()
        {
            m_odictOperatorPromptForState = new Dictionary<NSFState, string>();

            // TODO:  Fix this prompt and add the appropriate operator prompt messages for your system that get displayed when the associated state is entered
            m_odictOperatorPromptForState.Add(oFaultedState, "Clear the Faulted Condition and then hit ClearFaults on the Alarm View!");
            m_odictOperatorPromptForState.Add(oHomeAllAxisState, "Please Wait for the Motion Axis to Home");
            m_odictOperatorPromptForState.Add(oRefFrameNeededState, "Make sure Part is Unloaded and then Hit Create Ref File");
            m_odictOperatorPromptForState.Add(oRefFrameUpdatedState, "Use the Load button to Load / Unload a Part and then Hit Start to Begin Testing");
            m_odictOperatorPromptForState.Add(oWaitForDoorCloseState, "Please Close Door to Proceed");
            m_odictOperatorPromptForState.Add(oWaitForDoorClose4State, "Please Close Door to Proceed");
            m_odictOperatorPromptForState.Add(oWaitForDoorClose3State, "Please Close Door to Proceed");
            m_odictOperatorPromptForState.Add(oWaitForDoorClose2State, "Please Close Door to Proceed");
            m_odictOperatorPromptForState.Add(oLoadCouponState, "Please Load / Unload Part and Hit Commit to Proceed");
            m_odictOperatorPromptForState.Add(oRemoveCouponFirstErrorState, "Please Hit Abort and Remove the Part Before Attempting to Create Ref File");
            m_odictOperatorPromptForState.Add(oCouponNotLoadedErrorState, "Please Hit Abort and then Load Part Before Attempting to Test");
            m_odictOperatorPromptForState.Add(oReviewState, "Testing Complete!  Please Hit Commit to Save Test Results or Abort to Discard Test Results");
            m_odictOperatorPromptForState.Add(oWriteReportState, "Please Wait for the Test Results to be Written");
            m_odictOperatorPromptForState.Add(oAbortInspectingState, "Please Wait for the Abort to Complete");
        }

        // Guard Functions
        #region Guard Functions

        private bool IsRefFrameUpdatedToday(NSFStateMachineContext oContext)
        {
            // Zack todo
            return true;
        }

        private bool IsMoreLLToTest(NSFStateMachineContext oContext)
        {
            return m_iCaliforniaSystem.IsMoreLLToTest("Unit1");
        }

        private bool IsBypassKeyOk(NSFStateMachineContext oContext)
        {
            //  Zack ToDo   Need check for BypassKey Disabled || LoginLevel != Operator
            return true;
        }
        private bool IsBypassKeyOnAndValid(NSFStateMachineContext oContext)
        {
            //  Zack ToDo   Need check for BypassKey Enabled && LoginLevel != Operator
            return true;
        }

        private bool IsDoorClosedOrBypassKeyOnAndValid(NSFStateMachineContext oContext)
        {
            return (IsDoorClosed(oContext) || IsBypassKeyOnAndValid(oContext));
        }

        private bool IsDoorClosed(NSFStateMachineContext oContext)
        {
            //  Zack ToDo   return True if Closed
            return true;
        }
        private bool IsCouponLoaded(NSFStateMachineContext oContext)
        {
            //  Zack ToDo   return True if Loaded
            return true;
        }

        
        private bool IsSensorReady(NSFStateMachineContext oContext)
        {
            bool bSensorReady = m_iCaliforniaSystem.IsSensorReady;
            if (bSensorReady == false)
            {
                ms_iLogger.Log(ELogLevel.Error, "ScanSensor is in an invalid state for Scanning!");
            }
            return bSensorReady;
        }

        private bool IsCriticalAlarms(NSFStateMachineContext oContext)
        {
            return m_iCaliforniaSystem.IsCriticalAlarms;
        }

        private bool IsWarningOrCriticalAlarms(NSFStateMachineContext oContext)
        {
            return m_iCaliforniaSystem.IsWarningOrCriticalAlarms;
        }

        private bool IsNoCriticalAlarms(NSFStateMachineContext oContext)
        {
            return !IsCriticalAlarms(oContext);
        }

        private bool IsHomed(NSFStateMachineContext oContext)
        {
            return m_iCaliforniaSystem.IsHomed;
        }

        private bool IsNotAdminPrivilege(NSFStateMachineContext oContext)
        {
            return IsAdminPrivilege(oContext);
        }

        private bool IsAdminPrivilege(NSFStateMachineContext oContext)
        {
            return m_iCaliforniaSystem.IsAdminPrivilege;
        }

        private bool IsOperatorLogin(NSFStateMachineContext oContext)
        {
            return (m_iCaliforniaSystem.LoginLevel == ELoginLevel.OPERATOR);
        }

        #endregion Guard Functions


        #region State Entry And Exit Actions

        private void InitStateEntryActions(NSFStateMachineContext oContext)
        {
            EnableStartButton(false);
            EnableAbortButton(false);
            EnableLoadButton(false);
            EnableManualMotion(false);
            EnableCommitButton(false);
            EnableCreateRefButton(false);
        }
        private void InitStateExitActions(NSFStateMachineContext oContext)
        {

        }
        
        /// <summary>
        /// This method is called when the InitAfterSettings State is Exited.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void InitAfterSettingsStateExitActions(NSFStateMachineContext oContext)
        {
            ms_iLogger.Log(ELogLevel.Info, "Exited Init After Settings State.");
            m_iCaliforniaSystem.InitAfterSettingsComplete();
        }

        private void PrepareStateEntryActions(NSFStateMachineContext oContext)
        {
        }

        private void PrepareStateExitActions(NSFStateMachineContext oContext)
        {
            EnableManualMotion(false);
            EnableLoadButton(false);
            EnableCreateRefButton(false);
            EnableSetupViewFields(false);
        }

        private void InspectingStateEntryActions(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.EnableAbortButton = true;
        }

        private void InspectingStateExitActions(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.StopAllMotion();
            m_iCaliforniaSystem.SetShutterStateOpen(false);
        }

        private void LoadOrUnLoadStateEntryActions(NSFStateMachineContext oContext)
        {
            EnableLoadButton(false);
        }
        private void LoadOrUnLoadStateExitActions(NSFStateMachineContext oContext)
        {
            if (IsBypassKeyOk(null))    // If bypass key is validly not enabled
            {
                LockDoor(true);
            }
        }


        private void CreateReferenceFileStateEntryActions(NSFStateMachineContext oContext)
        {
        }
        private void CreateReferenceFileStateExitActions(NSFStateMachineContext oContext)
        {
        }


        private void WaitForDoorClose2StateEntryActions(NSFStateMachineContext oContext)
        {
        }
        private void WaitForDoorClose2StateExitActions(NSFStateMachineContext oContext)
        {
            LockDoor(true);
        }


        private void CouponNotLoadedErrorStateEntryActions(NSFStateMachineContext oContext)
        {
            EnableAbortButton(true);
            LockDoor(true);
        }
        private void CouponNotLoadedErrorStateExitActions(NSFStateMachineContext oContext)
        {
            EnableAbortButton(false);
        }


        private void MoveToLLStateEntryActions(NSFStateMachineContext oContext)
        {
            MoveToNextLens();
        }
        private void MoveToLLStateExitActions(NSFStateMachineContext oContext)
        {
        }


        private void TestLLStateEntryActions(NSFStateMachineContext oContext)
        {
            StartLLTestAsync();
        }
        private void TestLLStateExitActions(NSFStateMachineContext oContext)
        {
        }


        private void ReviewStateEntryActions(NSFStateMachineContext oContext)
        {
            EnableCommitButton(true);
        }
        private void ReviewStateExitActions(NSFStateMachineContext oContext)
        {
            EnableCommitButton(false);
            EnableAbortButton(false);
        }


        private void AbortInspectingStateEntryActions(NSFStateMachineContext oContext)
        {
            EnableAbortButton(true);
        }
        private void AbortInspectingStateExitActions(NSFStateMachineContext oContext)
        {
            OpenShutter(false);
            StopAllMotion();
        }


        private void WriteReportStateEntryActions(NSFStateMachineContext oContext)
        {
        }
        private void WriteReportStateExitActions(NSFStateMachineContext oContext)
        {
        }


        private void RefFrameNeededStateEntryActions(NSFStateMachineContext oContext)
        {
        }
        private void RefFrameNeededStateExitActions(NSFStateMachineContext oContext)
        {
        }


        private void RefFrameUpdatedStateEntryActions(NSFStateMachineContext oContext)
        {
            EnableStartButton(true);
        }
        private void RefFrameUpdatedStateExitActions(NSFStateMachineContext oContext)
        {
        }


        private void WaitForDoorCloseStateEntryActions(NSFStateMachineContext oContext)
        {
        }
        private void WaitForDoorCloseStateExitActions(NSFStateMachineContext oContext)
        {
        }


        private void MoveToLoadPosStateEntryActions(NSFStateMachineContext oContext)
        {
            LockDoor(true);
            MoveToLoadPos();
        }
        private void MoveToLoadPosStateExitActions(NSFStateMachineContext oContext)
        {
            LockDoor(false);
            EnableLoadButton(true);
        }


        private void LoadCouponStateEntryActions(NSFStateMachineContext oContext)
        {
            EnableCommitButton(true);
        }
        private void LoadCouponStateExitActions(NSFStateMachineContext oContext)
        {
            EnableCommitButton(false);
        }


        private void WaitForDoorClose3StateEntryActions(NSFStateMachineContext oContext)
        {
        }
        private void WaitForDoorClose3StateExitActions(NSFStateMachineContext oContext)
        {
        }


        private void WaitForDoorClose4StateEntryActions(NSFStateMachineContext oContext)
        {
        }
        private void WaitForDoorClose4StateExitActions(NSFStateMachineContext oContext)
        {
        }


        private void MoveToLens1PosStateEntryActions(NSFStateMachineContext oContext)
        {
        }
        private void MoveToLens1PosStateExitActions(NSFStateMachineContext oContext)
        {
        }


        private void RemoveCouponFirstErrorStateEntryActions(NSFStateMachineContext oContext)
        {
            EnableAbortButton(true);
        }
        private void RemoveCouponFirstErrorStateExitActions(NSFStateMachineContext oContext)
        {
            EnableAbortButton(false);
        }

        private void CreateRefFileStateEntryActions(NSFStateMachineContext oContext)
        {
            LockDoor(true);
            CreateRefFile();
        }
        private void CreateRefFileStateExitActions(NSFStateMachineContext oContext)
        {
        }


        /// <summary>
        /// This method is called when the Idle State is Entered.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void IdleStateEntryActions(NSFStateMachineContext oContext)
        {
            EnableManualMotion(true);
            EnableLoadButton(true);
            EnableCreateRefButton(true);
            EnableSetupViewFields(true);

        }

        /// <summary>
        /// This method is called when the Idle State is Exited.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void IdleStateExitActions(NSFStateMachineContext oContext)
        {
            EnableManualMotion(false);
            EnableLoadButton(false);
            EnableCreateRefButton(false);
            EnableSetupViewFields(false);
            EnableStartButton(false);
        }

        /// <summary>
        /// This method is called when the Faulted State is Entered.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void FaultedStateEntryActions(NSFStateMachineContext oContext)
        {
            OpenShutter(false); // Close Shutter
            StopAllMotion();
        }

        /// <summary>
        /// Calls into the System to have all Axes homed.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void HomeAllAxis(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.HomeAllAxis();
        }

        #endregion State Entry And Exit Actions


        #region Custom System Interface Functions

        private void ProcessInitAfterSettings(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.InitAfterSettings();
        }

        private void RaiseDoorOpenedMomentaryAlarm(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.RaiseDoorOpenedMomentaryAlarm(oContext);
        }

        private void RaiseMotionFaultMomentaryAlarm(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.RaiseMotionFaultMomentaryAlarm(oContext);
        }

        private void RaiseSensorFaultMomentaryAlarm(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.RaiseSensorFaultMomentaryAlarm(oContext);
        }

        private void RaiseEStopMomentaryAlarm(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.RaiseEStopMomentaryAlarm(oContext);
        }

        private void RaiseBypassKeyOnMomentaryAlarm(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.RaiseBypassKeyOnMomentaryAlarm(oContext);
        }


        private void AbortInspectingAsync(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.AbortInspectingAsync(oContext);
        }

        private void WriteMeasResultsMfgDbAsync(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.WriteMeasResultsMfgDbAsync(oContext); // Write results to the location where the DB loader will get them
        }

        private void WriteMeasResultsLocalAsync(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.WriteMeasResultsLocalAsync(oContext); //   Write results to a local directory but NOT to where the DB will get them
        }


        private void LockDoor(bool bLock)
        {
            m_iCaliforniaSystem.SetDoorStateLocked(bLock);
        }

        private void MoveToLoadPos()
        {
            m_iCaliforniaSystem.MoveToLoadPos();
        }

        private void OpenShutter(bool bOpen)
        {
            m_iCaliforniaSystem.SetShutterStateOpen(bOpen);
        }

        private void StopAllMotion()
        {
            m_iCaliforniaSystem.StopAllMotion();
        }

        private void EnableStartButton(bool bEnable)
        {
            m_iCaliforniaSystem.EnableStartButton = bEnable;
        }


        private void EnableLoadButton(bool bEnable)
        {
            if (bEnable)
            {
                // Zack todo
            }
            else
            {
                // Zack todo
            }
        }
        private void EnableAbortButton(bool bEnable)
        {
            m_iCaliforniaSystem.EnableAbortButton = bEnable;

        }
        

        private void EnableManualMotion(bool bEnable)
        {
            if (bEnable)
            {
                // Zack todo
            }
            else
            {
                // Zack todo
            }
        }


        private void EnableCommitButton(bool bEnable)
        {
            m_iCaliforniaSystem.EnableCommitButton  = bEnable;
        }


        private void EnableSetupViewFields(bool bEnable)
        {
            if (bEnable)
            {
                // Zack todo
            }
            else
            {
                // Zack todo
            }
        }

        private void EnableCreateRefButton(bool bEnable)
        {
            if (bEnable)
            {
                // Zack todo
            }
            else
            {
                // Zack todo
            }
        }

        private void MoveToNextLens()
        {
            m_iCaliforniaSystem.MoveToNextLens();
        }
        private void CreateRefFile()
        {

            m_iCaliforniaSystem.CreateRefFile();
        }

        private void StartLLTestAsync()
        {
            Tuple<string, string, string, string, string, bool, List<UCLensBox>> tupParameters;
            ms_iLogger.Log(ELogLevel.Info, "Started Running.");
            //m_iCaliforniaSystem.EnableAbortButton = true;
            //m_iCaliforniaSystem.EnableStartButton = false;


            tupParameters = (Tuple<string, string, string, string, string, bool, List<UCLensBox>>)((CStateMachineEventData)oStartEvent.Source).EventData;


            m_iCaliforniaSystem.StartLLTestAsync(tupParameters);
        }

        #endregion

        /// <summary>
        /// Called by the NSF whenever there is a state change
        /// </summary>
        /// <param name="oContext"></param>
        private void handleStateChange(NSFStateMachineContext oContext)
        {
            // Log state change event
            EState estateNew;

            if (oContext.EnteringState is NSFChoiceState) //(strNewState.StartsWith("Is"))
            {
                string strMsg = string.Format(" Evaluating {0} check!", oContext.EnteringState.Name);
                ms_iLogger.Log(ELogLevel.Info, strMsg);
                return;
            }
            else
            {
                string strMsg = string.Format(" Changing from the {0} state to the {1} state!", m_strSystemState, oContext.EnteringState.Name);
                ms_iLogger.Log(ELogLevel.Info, strMsg);
                // Capture new state
                m_strSystemState = oContext.EnteringState.Name;
                m_estatePrevious = m_estateCurrent;
                m_estateCurrent = Enum.TryParse(oContext.EnteringState.Name, out estateNew) ? (EState?)estateNew : null;
            }
            if (m_estatePrevious != null && m_estatePrevious == EState.Init)
                m_bInitComplete = true;
            if (m_estatePrevious != null && m_estatePrevious == EState.InitAfterSettings)
                m_bInitAfterSettingsComplete = true;

            m_autoreseteventStateChange.Set();

            //// Is there an operator prompt to display for this new state?
            string strOperatorPrompt = "";
            m_odictOperatorPromptForState.TryGetValue(oContext.EnteringState, out strOperatorPrompt);
            m_strLastOperatorPrompt = strOperatorPrompt;

            // Notify anyone subscribed to the state change event
            if (eventStateChange != null)
            {
                eventStateChange.Invoke(m_estatePrevious, m_estateCurrent, strOperatorPrompt);
            }
        }

        /// <summary>
        /// Calls into the System to Clear all Faults.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void ClearFaults(NSFStateMachineContext oContext)
        {
            m_iCaliforniaSystem.ClearFaults();
        }

        /// <summary>
        /// Waits for the specified state.
        /// </summary>
        /// <param name="eState">The State to wait for.</param> 
        /// <param name="bIsNewState">.</param> 
        /// <returns>Nothing</returns>
        private void WaitForState(EState eState, bool bIsNewState)
        {
            while ((bIsNewState&&m_estateCurrent!=eState)||(!bIsNewState&&m_estatePrevious!=eState))
            {
                m_autoreseteventStateChange.WaitOne();
                //m_semaphoreStateChange.WaitOne();
            } 
        }

        /// <summary>
        /// Waits for.
        /// </summary>
        /// <returns>Nothing</returns>
        public void WaitForInitComplete()
        {
            lock (m_objLock)
            {
                if (m_bInitComplete)
                    return;
            }
            WaitForState(EState.Init, false);
        }

        /// <summary>
        /// Waits for.
        /// </summary>
        /// <returns>Nothing</returns>
        public void WaitForInitAfterSettingsComplete()
        {
            lock (m_objLock)
            {
                if (m_bInitAfterSettingsComplete)
                    return;
            }
            WaitForState(EState.InitAfterSettings, false);
        }
        #endregion Methods

        #region InnerClasses
        #endregion InnerClasses
    }//end CCaliforniaStateMachine
}//end namespace CaliforniaSystem
