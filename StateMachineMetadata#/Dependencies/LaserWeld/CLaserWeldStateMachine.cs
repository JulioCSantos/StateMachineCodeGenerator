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
namespace LaserWeld.LaserWeldSystem
{
    public partial class CLaserWeldStateMachine
    {
        #region Fields
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CLaserWeldStateMachine");
        //private Semaphore m_semaphoreStateChange = new Semaphore();
        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);
        private object m_objLock = new object();
        private string m_strLastOperatorPrompt = "";

        private EState? m_estatePrevious;
        private EState? m_estateCurrent;
        private string m_strSystemState;
        private ILaserWeldSystem m_iLaserWeldSystem;

        private bool m_bSimulationMode = false;
        private Dictionary<LaserWeldSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<Proj3SystemEventsEnum, NSFEvent>();
        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        private bool m_bInitComplete = false;
        private bool m_bInitAfterSettingsComplete = false;

        #region State Machine Fields


        // ***********************************
        // State Machine NSF Event Definitions
        // ***********************************

        private static NSFEvent oStartManualScanEvent;
        private static NSFEvent oInitEvent;
        private static NSFEvent oAbortEvent;
        private static NSFEvent oDoorClosedEvent;
        private static NSFEvent oEStopOrInterLockEvent;
        private static NSFEvent oFiducialNotDetectedEvent;
        private static NSFEvent oAllAxisHomedEvent;
        private static NSFEvent oStartEvent;
        private static NSFEvent oUnloadWaferEvent;
        private static NSFEvent oMotionFaultEvent;
        private static NSFEvent oInitCompleteEvent;
        private static NSFEvent oClearFaultsEvent;
        private static NSFEvent oMoveCompleteEvent;
        private static NSFEvent oCameraFaultEvent;
        private static NSFEvent oCalWaferCompleteEvent;
        private static NSFEvent oAbortCompleteEvent;
        private static NSFEvent oCalWaferPosEvent;
        private static NSFEvent oStartAutoScanEvent;
        private static NSFEvent oFiducialAcquiredEvent;
        private static NSFEvent oLoadWaferEvent;
        private static NSFEvent oBypassSwitchOnEvent;
        private static NSFEvent oScanCompleteEvent;
        private static NSFEvent oAbort_Event;

        // ***********************************
        // End of State Machine NSF Event Definitions
        // ***********************************


        // ***********************************
        // State Machine NSF State Definitions
        // ***********************************

        // State Machine Upper Level Composite State Definitions
        private NSFCompositeState oAbortingScanState;
        private NSFCompositeState oCalibrateWaferPosState;
        private NSFCompositeState oHomeAllAxisState;
        private NSFCompositeState oLoadWaferState;
        private NSFCompositeState oUnloadWaferState;
        private NSFCompositeState oSystemPoweredState;

        // State Machine CalibrateWaferPosState State Definitions
        private NSFCompositeState       oPerformWaferCalibrationState;
        private NSFChoiceState          oIsAutoSeq_;
        private NSFCompositeState       oAcquireFid3PosState;
        private NSFCompositeState       oMoveToFiducial3;
        private NSFCompositeState       oAcquireFid2PosState;
        private NSFCompositeState       oMoveToFiducial2;
        private NSFCompositeState       oAcquireFid1PosState;
        private NSFCompositeState       oMoveToFiducial1;
        private NSFCompositeState       oMoveZState;
        private NSFInitialState         oCalibrateWaferPosStateInitial;

        // State Machine UnloadWaferState State Definitions
        private NSFInitialState         oUnloadWaferStateInitial;
        private NSFCompositeState       oWaitForWaferUnloadState;
        private NSFCompositeState       oWaitForDoorClosedState;
        private NSFChoiceState          oIsDoorClosed_;
        private NSFCompositeState       oMoveToLoadPosState;

        // State Machine LoadWaferState State Definitions
        private NSFCompositeState       oWaitForWaferLoadState;
        private NSFCompositeState       oWaitForDoorClosedState;
        private NSFChoiceState          oIsDoorClosed_;
        private NSFCompositeState       oMoveToLoadPosState;
        private NSFInitialState         oLoadWaferStateInitial;

        // State Machine Idle_State State Definitions
        private NSFChoiceState          oIsWaferLoaded_;
        private NSFCompositeState       oWaferUnLoadedState;
        private NSFInitialState         oIdle_StateInitial;
        private NSFCompositeState       oWaferLoadedState;

        // State Machine ProcessingState State Definitions
        private NSFInitialState         oProcessingStateInitial;
        private NSFCompositeState       oScanState;

        // State Machine ReadyState State Definitions
        private NSFCompositeState       oProcessingState;
        private NSFInitialState         oReadyStateInitial;
        private NSFCompositeState       oIdle_State;

        // State Machine SystemPoweredState State Definitions
        private NSFChoiceState          oIsWarningOrCriticalAlarm;
        private NSFCompositeState       oReadyState;
        private NSFCompositeState       oFaultedState;
        private NSFCompositeState       oInitAfterSettingsState;
        private NSFInitialState         oSystemPoweredStateInitial;
        private NSFCompositeState       oInitState;


        // ***********************************
        // End of State State Machine NSF State Definitions
        // ***********************************


        #endregion State Machine Fields

        #endregion Fields
    } //end CLaserWeldStateMachine
} //end LaserWeld.LaserWeldSystem
