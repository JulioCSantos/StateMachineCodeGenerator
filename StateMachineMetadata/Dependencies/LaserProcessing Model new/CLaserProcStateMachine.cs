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
namespace LaserProc.LaserProcSystem
{
    public partial class CLaserProcStateMachine
    {
        #region Fields
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CLaserProcStateMachine");
        //private Semaphore m_semaphoreStateChange = new Semaphore();
        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);
        private object m_objLock = new object();
        private string m_strLastOperatorPrompt = "";

        private EState? m_estatePrevious;
        private EState? m_estateCurrent;
        private string m_strSystemState;
        private ILaserProcSystem m_iLaserProcSystem;

        private bool m_bSimulationMode = false;
        private Dictionary<LaserProcSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<Proj3SystemEventsEnum, NSFEvent>();
        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        private bool m_bInitComplete = false;
        private bool m_bInitAfterSettingsComplete = false;

        #region State Machine Fields


        // ***********************************
        // State Machine NSF Event Definitions
        // ***********************************

        private static NSFEvent oProcessingCompleteEvent;
        private static NSFEvent oLocateCompleteEvent;
        private static NSFEvent oCalCompleteEvent;
        private static NSFEvent oMoveCompleteEvent;
        private static NSFEvent oLoadEvent;
        private static NSFEvent oStartEvent;
        private static NSFEvent oCalEvent;
        private static NSFEvent oInitCompleteEvent;
        private static NSFEvent oInitEvent;
        private static NSFEvent oEStopEvent;
        private static NSFEvent oClearFaultsEvent;
        private static NSFEvent oAllAxisHomedEvent;
        private static NSFEvent oAbortEvent;
        private static NSFEvent oCommitEvent;
        private static NSFEvent oInitAfterSettingsCompleteEvent;
        private static NSFEvent oUnLoadEvent;
        private static NSFEvent oDoorOpenEvent;
        private static NSFEvent oAbortCompleteEvent;
        private static NSFEvent oFindFocusEvent;
        private static NSFEvent oFindFocusCompleteEvent;
        private static NSFEvent oCompleteEvent;
        private static NSFEvent oLocateDeviceEvent;
        private static NSFEvent oMotionFaultEvent;
        private static NSFEvent oLaserFaultEvent;
        private static NSFEvent oSaveResultsCompleteEvent;

        // ***********************************
        // End of State Machine NSF Event Definitions
        // ***********************************


        // ***********************************
        // State Machine NSF State Definitions
        // ***********************************

        // State Machine Upper Level Composite State Definitions
        private NSFCompositeState oActiveState;
        private NSFCompositeState oHomeAllAxisState;
        private NSFCompositeState oPrepareState;
        private NSFCompositeState oSystemPoweredState;

        // State Machine LocateDeviceState State Definitions
        private NSFCompositeState       oMoveToRunPos2State;
        private NSFInitialState         oLocateDeviceStateInitial;
        private NSFCompositeState       oLocatingState;
        private NSFCompositeState       oRepositionState;
        private NSFCompositeState       oMoveToRePositionState;
        private NSFChoiceState          oIsLocated4;

        // State Machine ActiveState State Definitions
        private NSFChoiceState          oIsLocated3;
        private NSFCompositeState       oFindFocusState;
        private NSFCompositeState       oAbortingState;
        private NSFCompositeState       oAutoCalState;
        private NSFCompositeState       oLocateDeviceState;
        private NSFCompositeState       oProcessingState;
        private NSFChoiceState          oIsProcessingRequested;

        // State Machine LoadedState State Definitions
        private NSFInitialState         oLoadedStateInitial;
        private NSFCompositeState       oNotLocatedState;
        private NSFCompositeState       oLocatedState;

        // State Machine PrepareState State Definitions
        private NSFChoiceState          oIsLoaded2;
        private NSFCompositeState       oMoveToRunPosState;
        private NSFCompositeState       oMoveToLoadPosState;
        private NSFChoiceState          oIsLoaded;
        private NSFChoiceState          oIsLocated;
        private NSFInitialState         oPrepareStateInitial;
        private NSFCompositeState       oIdleState;
        private NSFCompositeState       oLoadDeviceState;

        // State Machine IdleState State Definitions
        private NSFInitialState         oIdleStateInitial;
        private NSFCompositeState       oLoadedState;
        private NSFCompositeState       oUnLoadedState;

        // State Machine ProcessingState State Definitions
        private NSFChoiceState          oMoreLocations;
        private NSFInitialState         oProcessingStateInitial;
        private NSFCompositeState       oSaveResultsState;
        private NSFCompositeState       oAbortProcessingState;
        private NSFCompositeState       oMoveToNextLocState;
        private NSFCompositeState       oProcessingCompleteState;
        private NSFCompositeState       oLaserProcessingState;

        // State Machine SystemPoweredState State Definitions
        private NSFChoiceState          oIsCriticalAlarm;
        private NSFCompositeState       oFaultedState;
        private NSFCompositeState       oInitAfterSettingsState;
        private NSFInitialState         oSystemPoweredStateInitial;
        private NSFCompositeState       oInitState;


        // ***********************************
        // End of State State Machine NSF State Definitions
        // ***********************************


        #endregion State Machine Fields

        #endregion Fields
    } //end CLaserProcStateMachine
} //end LaserProc.LaserProcSystem
