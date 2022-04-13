///////////////////////////////////////////////////////////
//  Copyright © Corning Incorporated  2016
//  CStateMachineEventData.cs
//  Project StateMachineCodeGeneratorSystem
//  Implementation of the Class CStateMachineEventData
//  Created on:      November 23, 2016 5:36:03 AM
///////////////////////////////////////////////////////////

using System.Threading;
using Corning.GenSys.Logger;
using NorthStateSoftware.NorthStateFramework;
using System;
using System.Collections.Generic;
using StateMachineCodeGenerator.Interfaces;


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
// uncomment "m_iStateMachineCodeGeneratorSystem.EnableStartButton = false;"  in "IdleStateEntryActions".

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
// m_odictOperatorPromptForState.Add(oRunningState, "Hit Abort to Stop Running");
//
//


namespace StateMachineCodeGenerator.StateMachineCodeGeneratorSystem
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

    public class CStateMachineCodeGeneratorStateMachine : NSFStateMachine, IStateMachineCodeGeneratorStateMachine
    {
        #region Fields
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CStateMachineCodeGeneratorStateMachine");
        //private Semaphore m_semaphoreStateChange = new Semaphore();
        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);
        private object m_objLock = new object();
        private string m_strLastOperatorPrompt = "";

        private EState? m_estatePrevious;
        private EState? m_estateCurrent;
        private string m_strSystemState;
        private IStateMachineCodeGeneratorSystem m_iStateMachineCodeGeneratorSystem;

        private bool m_bSimulationMode = false;
        private Dictionary<StateMachineCodeGeneratorSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<StateMachineCodeGeneratorSystemEventsEnum, NSFEvent>();
        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        private bool m_bInitComplete = false;
        private bool m_bInitAfterSettingsComplete = false;


        #region State Machine Fields

        // ***********************************
        // State Machine NSF Event Definitions
        // ***********************************
        private static NSFEvent oInitCompleteEvent;
        private static NSFEvent oInitAfterSettingsCompleteEvent;
        private static NSFEvent oClearFaultsEvent;
        private static NSFEvent oEStopEvent;
        private static NSFEvent oStartScanEvent;
        private static NSFEvent oScanCompleteEvent;  // OK to Remove?
        private static NSFEvent oStartScanReadyEvent;  // OK to Remove?
        private static NSFEvent oAbortEvent;
        private static NSFEvent oStopEvent;
        private static NSFEvent oProceedToFaultEvent;   // OK to Remove?
        private static NSFEvent oMoveCompleteEvent;
        private static NSFEvent oInitEvent;
        private static NSFEvent oScanSensorFaultEvent;
        private static NSFEvent oMotionFaultEvent;
        private static NSFEvent oScanProgressEvent;
        private static NSFEvent oScanSensorStateChangeEvent;
        private static NSFEvent oAllAxisHomedEvent;
        private static NSFEvent oSample1DInTrueEvent;  // OK to Remove?
        private static NSFEvent oAbortCompleteEvent;
        private static NSFEvent oCommitEvent;
        private static NSFEvent oLoadOfflineFileEvent;

        // *******************************************
        // End of State Machine NSF Event Definitions
        // *******************************************


        // ***********************************
        // State Machine NSF State Definitions
        // ***********************************

        // State Machine Upper Level SubState Definitions
        private NSFInitialState   oStateMachineCodeGeneratorStateMachineInitialState;
        private NSFCompositeState oSystemPoweredState;

        // SystemPoweredState SubState Definitions
        private NSFInitialState   oSystemPoweredInitialState;
        private NSFCompositeState oInitState;
        private NSFCompositeState oInitAfterSettingsState;
        private NSFCompositeState oHomeAllAxisState;
        private NSFChoiceState    oIsHomedChoiceState;
        private NSFChoiceState    oIsCriticalAlarmChoiceState;
        private NSFCompositeState oFaultedState;
        private NSFCompositeState oReadyState;

        // Ready State SubState Definitions
        private NSFInitialState   oReadyInitialState;
        private NSFCompositeState oIdleState;
        private NSFCompositeState oRunningState;

        // ******************************************
        // End of State Machine NSF State Definitions
        // ******************************************


        // ******************************
        // State Machine NFS Transitions
        // ******************************

        // State Machine Upper Level Transitions
        private NSFExternalTransition StateMachineCodeGeneratorStateMachineInitialToSystemPoweredTransition;

        // SystemPoweredState Transitions
        private NSFExternalTransition SystemPoweredInitialToInitTransition;
        private NSFExternalTransition SystemPoweredToInitTransition;

        // InitState Transitions
        private NSFExternalTransition InitToInitAfterSettingsTransition;

        // InitAfterSettingsState Transitions
        private NSFExternalTransition InitAfterSettingsToIsCriticalAlarmChoiceTransition;

        // IsWarningOrCriticalAlarmChoiceState Transitions
        private NSFExternalTransition IsCriticalAlarmChoiceToFaultedTransition;
        private NSFExternalTransition IsCriticalAlarmChoiceToIsHomedChoiceTransition;

        // IsHomedChoiceState Transitions
        private NSFExternalTransition IsHomedChoiceToReadyTransition;
        private NSFExternalTransition IsHomedChoiceToHomeAllAxisTransition;

        // HomeAllAxisState Transitions
        private NSFExternalTransition HomeAllAxisToIsCriticalAlarmTransition;

        // ReadyState Transitions
        private NSFExternalTransition ReadyToFaultedTransition;
        private NSFExternalTransition ReadyInitialToIdleTransition;

        // IdleState Transitions
        private NSFExternalTransition IdleToRunningTransition;

        // RunningState Transitions
        private NSFExternalTransition RunningToIdleTransition;

        // Faulted State Transitions
        private NSFExternalTransition FaultedToIsCriticalAlarmChoiceTransition;

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
        public CStateMachineCodeGeneratorStateMachine(string strName, IStateMachineCodeGeneratorSystem iStateMachineCodeGeneratorSystem)
            : base(strName, new NSFEventThread(strName))
        {
            // Capture reference to the parent system for calling back to perform system functions
            m_iStateMachineCodeGeneratorSystem = iStateMachineCodeGeneratorSystem;

            // Init State Machine
            CreateStateMachine();

            // Init Operator Prompts
            CreateOperatorPrompts();
        }

        ~CStateMachineCodeGeneratorStateMachine()
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
            NSFTraceLog.PrimaryTraceLog.saveLog("StateMachineCodeGeneratorStateMachineLog.xml");

            NSFEnvironment.terminate();
            return true;
        }

        public bool HandleEvent(StateMachineCodeGeneratorSystemEventsEnum eStateMachineCodeGeneratorEvent, object oEventData)
        {
            bool bEventHandled = false;

            if (!m_dictEventByEnum.ContainsKey(eStateMachineCodeGeneratorEvent))
            {
                ms_iLogger.Log(ELogLevel.Error, "Invalid Event passed to HandleEvent!");
                return false;
            }

            try
            {
                // Get event from enum passed in
                NSFEvent oEvent = m_dictEventByEnum[eStateMachineCodeGeneratorEvent];

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
                    string strMsg = string.Format(" Received Event {0}!", eStateMachineCodeGeneratorEvent.ToString());
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
                string strMsg = String.Format("Error!  Unable to handle handle event {0}!", eStateMachineCodeGeneratorEvent.ToString());
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
                oInitCompleteEvent = new NSFEvent("InitComplete", this, this);
                oInitAfterSettingsCompleteEvent = new NSFEvent("InitAfterSettingsComplete", this, this);
                oClearFaultsEvent = new NSFEvent("ClearFaultsEvent", this, this);
                oEStopEvent = new NSFEvent("EStopEvent", this, this);
                oStartScanEvent = new NSFEvent("StartScanEvent", this, this);
                oStopEvent = new NSFEvent("StopEvent", this, this);
                oScanCompleteEvent = new NSFEvent("ScanCompleteEvent", this, this); 
                oStartScanReadyEvent = new NSFEvent("StartScanReadyEvent", this, this);
                oAbortEvent = new NSFEvent("AbortEvent", this, this);
                oProceedToFaultEvent = new NSFEvent("ProceedToFaultEvent", this, this);
                oMoveCompleteEvent = new NSFEvent("MoveCompleteEvent", this, this);
                oInitEvent = new NSFEvent("InitEvent", this, this);
                oScanSensorFaultEvent = new NSFEvent("ScanSensorFaultEvent", this, this);
                oMotionFaultEvent = new NSFEvent("MotionFaultEvent", this, this);
                oScanProgressEvent = new NSFEvent("ScanProgressEvent", this, this);
                oScanSensorStateChangeEvent = new NSFEvent("ScanSensorStateChangeEvent", this, this);
                oSample1DInTrueEvent = new NSFEvent("Sample1DInTrueEvent", this, this);
                oAllAxisHomedEvent = new NSFEvent("AllAxisHomedEvent", this, this);
                oAbortCompleteEvent = new NSFEvent("AbortCompleteEvent", this, this);
                oCommitEvent = new NSFEvent("CommitEvent", this, this);
                oLoadOfflineFileEvent = new NSFEvent("LoadOfflineFileEvent", this, this);

                // Create dictionary mapping event enum to actual event
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.InitCompleteEvent, oInitCompleteEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.InitAfterSettingsCompleteEvent, oInitAfterSettingsCompleteEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.ClearFaultsEvent, oClearFaultsEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.EStopEvent, oEStopEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.StartScanEvent, oStartScanEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.StopEvent, oStopEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.ScanCompleteEvent, oScanCompleteEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.AbortEvent, oAbortEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.ProceedToFaultEvent, oProceedToFaultEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.MoveCompleteEvent, oMoveCompleteEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.InitEvent, oInitEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.ScanSensorFaultEvent, oScanSensorFaultEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.MotionFaultEvent, oMotionFaultEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.ScanProgressEvent, oScanProgressEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.ScanSensorStateChangeEvent, oScanSensorStateChangeEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.AllAxisHomedEvent, oAllAxisHomedEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.AbortCompleteEvent, oAbortCompleteEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.CommitEvent, oCommitEvent);
                m_dictEventByEnum.Add(StateMachineCodeGeneratorSystemEventsEnum.LoadOfflineFileEvent, oLoadOfflineFileEvent);

 
                // ****************************************
                // Create the states for the State Machine
                // ****************************************

                // State Machine Upper Level SubState Instantiations. Create all SubStates of the Upper Level.
                oStateMachineCodeGeneratorStateMachineInitialState = new NSFInitialState(EState.StateMachineCodeGeneratorStateMachineInitial.ToString(), this);
                oSystemPoweredState        = new NSFCompositeState(EState.SystemPowered.ToString(), this, null, null);

                // SystemPoweredState SubState Instantiations. Create all SubStates of SystemPoweredState
                oSystemPoweredInitialState = new NSFInitialState(EState.SystemPoweredInitial.ToString(), oSystemPoweredState);
                oInitState                 = new NSFCompositeState(EState.Init.ToString(), oSystemPoweredState, null, null);
                oInitAfterSettingsState    = new NSFCompositeState(EState.InitAfterSettings.ToString(), oSystemPoweredState, null, InitAfterSettingsStateExitActions);
                oHomeAllAxisState          = new NSFCompositeState(EState.HomeAllAxis.ToString(), oSystemPoweredState, HomeAllAxis, null);
                oIsHomedChoiceState        = new NSFChoiceState(EState.IsHomedChoice.ToString(), oSystemPoweredState);
                oIsCriticalAlarmChoiceState = new NSFChoiceState(EState.IsCriticalAlarm.ToString(), oSystemPoweredState);
                oFaultedState              = new NSFCompositeState(EState.Faulted.ToString(), oSystemPoweredState, FaultedStateEntryActions, null);
                oReadyState                = new NSFCompositeState(EState.Ready.ToString(),   oSystemPoweredState, ReadyStateEntryActions, ReadyStateExitActions);


                // InitState SubState Instantiations. Create all SubStates of InitState.
                // None

                // InitAfterSettingsState SubState Instantiations. Create all SubStates of InitAfterSettingsState
                // None

                // HomeAllAxisState SubState Instantiations. Create all SubStates of HomeAllAxisState.
                // None

                // FaultedState SubState Instantiations. Create all SubStates of FaultedState.
                // None


                // ReadyState SubState Instantiations. Create all SubStates of ReadyState.
                oReadyInitialState = new NSFInitialState("ReadyInitial", oReadyState);
                oIdleState                 = new NSFCompositeState(EState.Idle.ToString(),    oReadyState, IdleStateEntryActions, IdleStateExitActions);
                oRunningState              = new NSFCompositeState(EState.Running.ToString(), oReadyState, RunningStateEntryActions, RunningStateExitActions);

                // IdleState SubState Instantiations. Create all SubStates of IdleState.
                // None

                // RunningState SubState Instantiations. Create all SubStates of RunningState.
                // None


                // *************************************************
                // End of Creating the states for the State Machine
                // *************************************************


                // *********************************************
                // Create the Transitions for the State Machine
                // *********************************************

                // Top Level Transitions
                StateMachineCodeGeneratorStateMachineInitialToSystemPoweredTransition = new NSFExternalTransition("StateMachineCodeGeneratorStateMachineInitialToSystemPowered", oStateMachineCodeGeneratorStateMachineInitialState, oSystemPoweredState, null, null, null);

                // SystemPoweredState Transitions
                SystemPoweredInitialToInitTransition = new NSFExternalTransition("SystemPoweredInitialToInit", oSystemPoweredInitialState, oInitState, null, null, null);
                SystemPoweredToInitTransition = new NSFExternalTransition("SystemPoweredToInit", oSystemPoweredState, oInitState, oInitEvent, null, null);

                // InitState Transitions
                InitToInitAfterSettingsTransition = new NSFExternalTransition("InitToInitAfterSettings", oInitState, oInitAfterSettingsState, oInitCompleteEvent, null, ProcessInitAfterSettings);

                // InitAfterSettingsState Transitions
                InitAfterSettingsToIsCriticalAlarmChoiceTransition = new NSFExternalTransition("InitAfterSettingsToIsCriticalAlarmChoice", oInitAfterSettingsState, oIsCriticalAlarmChoiceState, oInitAfterSettingsCompleteEvent, null, null);

                // IsCriticalAlarmChoiceState Transitions
                IsCriticalAlarmChoiceToFaultedTransition = new NSFExternalTransition("IsFaultedChoiceToFaulted", oIsCriticalAlarmChoiceState, oFaultedState, null, IsCriticalAlarms, null);

                // Note: The Guard condition oIsHomedChoiceState always returns True in the template. It may be changed by modifying the "bool IsHomed" property in System
                IsCriticalAlarmChoiceToIsHomedChoiceTransition = new NSFExternalTransition("IsCriticalAlarmChoiceToIsHomedChoice", oIsCriticalAlarmChoiceState, oIsHomedChoiceState, null, Else, null);

                // IsHomedChoiceState Transitions
                IsHomedChoiceToReadyTransition = new NSFExternalTransition("IsHomedChoiceToReady", oIsHomedChoiceState, oReadyState, null, IsHomed, null);
                IsHomedChoiceToHomeAllAxisTransition = new NSFExternalTransition("IsHomedChoiceToHomeAllAxis", oIsHomedChoiceState, oHomeAllAxisState, null, Else, null);

                // HomeAllAxisState Transitions                              
                HomeAllAxisToIsCriticalAlarmTransition = new NSFExternalTransition("HomeAllAxisToIsCriticalAlarm", oHomeAllAxisState, oIsCriticalAlarmChoiceState, oAllAxisHomedEvent, null, null);


                // IdleState Transitions
                IdleToRunningTransition = new NSFExternalTransition("IdleToRunning",           oIdleState,         oRunningState, oStartScanEvent, null, null);

                // ReadyState Transitions
                ReadyInitialToIdleTransition = new NSFExternalTransition("ReadyInitialToIdleState", oReadyInitialState, oIdleState, null, null, null);
                ReadyToFaultedTransition = new NSFExternalTransition("ReadyToFaulted",          oReadyState,        oFaultedState, oEStopEvent,     null, null);

                // RunningState Transitions
                RunningToIdleTransition = new NSFExternalTransition("RunningToIdleViaAbort",           oRunningState,      oIdleState,    oAbortEvent,     null, null);
                RunningToIdleTransition = new NSFExternalTransition("RunningToIdle",           oRunningState,      oIdleState,    oScanCompleteEvent,     null, null);

                // FaultedState Transitions
                FaultedToIsCriticalAlarmChoiceTransition = new NSFExternalTransition("FaultedToIsCriticalAlarmChoice", oFaultedState, oIsCriticalAlarmChoiceState, oClearFaultsEvent, null, null);


                // ******************************************************
                // End of creating the Transitions for the State Machine
                // ******************************************************

                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Unable to initiate the StateMachineCodeGenerator State Machine!", ex);
            }

            return false;
        }

        private void CreateOperatorPrompts()
        {
            m_odictOperatorPromptForState = new Dictionary<NSFState, string>();

            // TODO:  Fix this prompt and add the appropriate operator prompt messages for your system that get displayed when the associated state is entered
            m_odictOperatorPromptForState.Add(oFaultedState, "Clear the Faulted Condition and then hit ClearFaults on the Alarm View!");
            m_odictOperatorPromptForState.Add(oIdleState, "Hit Start to Start Running");
            m_odictOperatorPromptForState.Add(oRunningState, "Hit Abort to Stop Running");
        }

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
                m_estateCurrent = Enum.TryParse(oContext.EnteringState.Name, out estateNew) ? (EState?) estateNew : null;
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

        // Guard Functions
        #region Guard Functions

        private bool IsSensorReady(NSFStateMachineContext oContext)
        {
            bool bSensorReady = m_iStateMachineCodeGeneratorSystem.IsSensorReady;
            if (bSensorReady == false)
            {
                ms_iLogger.Log(ELogLevel.Error, "ScanSensor is in an invalid state for Scanning!");
            }
            return bSensorReady;
        }

        private bool IsCriticalAlarms(NSFStateMachineContext oContext)
        {
            return m_iStateMachineCodeGeneratorSystem.IsCriticalAlarms;
        }

        private bool IsWarningOrCriticalAlarms(NSFStateMachineContext oContext)
        {
            return m_iStateMachineCodeGeneratorSystem.IsWarningOrCriticalAlarms;
        }

        private bool IsNoCriticalAlarms(NSFStateMachineContext oContext)
        {
            return !IsCriticalAlarms(oContext);
        }

        private bool IsHomed(NSFStateMachineContext oContext)
        {
            return m_iStateMachineCodeGeneratorSystem.IsHomed;
        }

        private bool IsNotAdminPrivilege(NSFStateMachineContext oContext)
        {
            return IsAdminPrivilege(oContext);
        }

        private bool IsAdminPrivilege(NSFStateMachineContext oContext)
        {
            return m_iStateMachineCodeGeneratorSystem.IsAdminPrivilege;
        }

        #endregion Guard Functions

        #region Transition Actions

        private void ProcessInitAfterSettings(NSFStateMachineContext oContext)
        {
            m_iStateMachineCodeGeneratorSystem.InitAfterSettings();
        }



        //private void EnableStartAndUnLoadButtonOnGui(NSFStateMachineContext oContext)
        //{
        //    m_iStateMachineCodeGeneratorSystem.EnableStartButton = true;
        //    m_iStateMachineCodeGeneratorSystem.UnLoadGlassOnGuiEnable = true;
        //}

        //private void EnableUnLoadButtonOnGui(NSFStateMachineContext oContext)
        //{
        //    m_iStateMachineCodeGeneratorSystem.UnLoadGlassOnGuiEnable = true;
        //}

        //private void DisableUnLoadButtonOnGui(NSFStateMachineContext oContext)
        //{
        //    m_iStateMachineCodeGeneratorSystem.UnLoadGlassOnGuiEnable = false;
        //}

        //private void DisableStartAndUnLoadButtonOnGui(NSFStateMachineContext oContext)
        //{
        //    m_iStateMachineCodeGeneratorSystem.EnableStartButton = false;
        //    m_iStateMachineCodeGeneratorSystem.UnLoadGlassOnGuiEnable = false;
        //}

        //private void EnableLoadGlassAndStartOnGui(NSFStateMachineContext oContext)
        //{
        //    m_iStateMachineCodeGeneratorSystem.EnableStartAndAbortButton = true;
        //}

        //private void EnableAbortButtonOnGui(NSFStateMachineContext oContext)
        //{
        //    m_iStateMachineCodeGeneratorSystem.EnableAbortButton = true;
        //}

        //private void DisableAbortButtonOnGui(NSFStateMachineContext oContext)
        //{
        //    m_iStateMachineCodeGeneratorSystem.EnableAbortButton = false;
        //}

        #endregion Transition Actions


        #region State Entry And Exit Actions

        /// <summary>
        /// This method is called when the InitAfterSettings State is Exited.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void InitAfterSettingsStateExitActions(NSFStateMachineContext oContext)
        {
            ms_iLogger.Log(ELogLevel.Info, "Exited Init After Settings State.");
            m_iStateMachineCodeGeneratorSystem.InitAfterSettingsComplete();
        }

        /// <summary>
        /// This method is called when the Ready State is Entered.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void ReadyStateEntryActions(NSFStateMachineContext oContext)
        {
            ms_iLogger.Log(ELogLevel.Info, "Entered Ready State.");
        }

        /// <summary>
        /// This method is called when the Ready State is Exited.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void ReadyStateExitActions(NSFStateMachineContext oContext)
        {
            ms_iLogger.Log(ELogLevel.Info, "Exited Ready State.");
        }


        /// <summary>
        /// This method is called when the Idle State is Entered.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void IdleStateEntryActions(NSFStateMachineContext oContext)
        {
            //m_iStateMachineCodeGeneratorSystem.EnableStartButton = false;  // Uncomment this line to prevent the Start button from being enabled.
            //ms_iLogger.Log(ELogLevel.Info, "Entered Idle State.");
        }

        /// <summary>
        /// This method is called when the Idle State is Exited.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void IdleStateExitActions(NSFStateMachineContext oContext)
        {
            //ms_iLogger.Log(ELogLevel.Info, "Exited Idle.");
        }

        /// <summary>
        /// This method is called when the Running State is Entered.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private async void RunningStateEntryActions(NSFStateMachineContext oContext)
        {
            ms_iLogger.Log(ELogLevel.Info, "Started Running.");
            m_iStateMachineCodeGeneratorSystem.EnableAbortButton = true;
            m_iStateMachineCodeGeneratorSystem.EnableStartButton = false;

            // Optional Code to demonstrate the ThreadHandler.
            await m_iStateMachineCodeGeneratorSystem.StartRunningTheApplication();  // Code in System will start a thread to display a message 20 times, then quit.

        }

        /// <summary>
        /// This method is called when the Running State is Exited.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void RunningStateExitActions(NSFStateMachineContext oContext)
        {
            ms_iLogger.Log(ELogLevel.Info, "Stopped Running.");
            m_iStateMachineCodeGeneratorSystem.EnableAbortButton = false;
            m_iStateMachineCodeGeneratorSystem.EnableStartButton = true;

            // Optional Code to demonstrate the ThreadHandler.
            m_iStateMachineCodeGeneratorSystem.StopRunningTheApplication(); // Call into the System to stop the thread in system.
        }



        /// <summary>
        /// This method is called when the Faulted State is Entered.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void FaultedStateEntryActions(NSFStateMachineContext oContext)
        {

        }

        /// <summary>
        /// Calls into the System to have all Axes homed.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void HomeAllAxis(NSFStateMachineContext oContext)
        {
            m_iStateMachineCodeGeneratorSystem.HomeAllAxis();
        }

        #endregion State Entry And Exit Actions





        /// <summary>
        /// Calls into the System to Clear all Faults.
        /// </summary>
        /// <param name="oContext">Information about the states before and after the transition as well as the transition and trigger.</param> 
        /// <returns>Nothing</returns>
        private void ClearFaults(NSFStateMachineContext oContext)
        {
            m_iStateMachineCodeGeneratorSystem.ClearFaults();
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
    }//end CStateMachineCodeGeneratorStateMachine
}//end namespace StateMachineCodeGeneratorSystem
