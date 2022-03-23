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
namespace GuideHighLevel.GuideHighLevelSystem
{
    public partial class CGuideHighLevelStateMachine
    {
        #region Fields
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CGuideHighLevelStateMachine");
        //private Semaphore m_semaphoreStateChange = new Semaphore();
        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);
        private object m_objLock = new object();
        private string m_strLastOperatorPrompt = "";

        private EState? m_estatePrevious;
        private EState? m_estateCurrent;
        private string m_strSystemState;
        private IGuideHighLevelSystem m_iGuideHighLevelSystem;

        private bool m_bSimulationMode = false;
        private Dictionary<GuideHighLevelSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<Proj3SystemEventsEnum, NSFEvent>();
        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        private bool m_bInitComplete = false;
        private bool m_bInitAfterSettingsComplete = false;

        #region State Machine Fields


        // ***********************************
        // State Machine NSF Event Definitions
        // ***********************************

        private static NSFEvent oGlassRemovedAtEntranceEvent;
        private static NSFEvent oUnloadGlassEvent;
        private static NSFEvent oLoadStripeImageEvent;
        private static NSFEvent oDoorCloseEvent;
        private static NSFEvent oCartUnDockedEvent;
        private static NSFEvent oGlassNotDetectedEvent;
        private static NSFEvent oAbortEvent;
        private static NSFEvent oReviewCompleteEvent;
        private static NSFEvent oClearFaultsEvent;
        private static NSFEvent oEvents (see Note for list);
        private static NSFEvent oInitCompleteEvent;
        private static NSFEvent oEStopEvent;
        private static NSFEvent oGlassDetectedAtEntranceEvent;
        private static NSFEvent oUnLoadEvent;
        private static NSFEvent oMotionFaultEvent;
        private static NSFEvent oPanelOpenEvent;
        private static NSFEvent oVacuumOffEvent;
        private static NSFEvent oCabinetOverTempEvent;
        private static NSFEvent oAirOffEvent;
        private static NSFEvent oScanSensorFaultEvent;
        private static NSFEvent oBypassSwitchOnEvent;
        private static NSFEvent oMoveCompleteEvent;
        private static NSFEvent oRetractTrailingEdgePinsTimeOutEvent;
        private static NSFEvent oNotifyScanSensorAlarm;
        private static NSFEvent oDoorNotClosed;
        private static NSFEvent oMoveTimeOutEvent;
        private static NSFEvent oWindowNotClosedEvent;
        private static NSFEvent oGlassDetectedAtDamperEvent;
        private static NSFEvent oLightOnEvent;
        private static NSFEvent oDoorOpenEvent;
        private static NSFEvent oGlassRemovedAtEntrance;
        private static NSFEvent oInitEvent;

        // ***********************************
        // End of State Machine NSF Event Definitions
        // ***********************************


        // ***********************************
        // State Machine NSF State Definitions
        // ***********************************

        // State Machine Upper Level Composite State Definitions
        private NSFCompositeState oFaultAndOperatorClearsGlassState;
        private NSFCompositeState oFaultWithNoGlassState;
        private NSFCompositeState oFaultWithGlassLostState;
        private NSFCompositeState oFaultWithGlassHeldState;
        private NSFCompositeState oGlassLoadingState;
        private NSFCompositeState oGlassUnLoadingState;
        private NSFCompositeState oManualReviewStripeDefectsState;
        private NSFCompositeState oSystemPoweredState;

        // State Machine GlassUnLoadingState State Definitions
        private NSFCompositeState       oCloseDoorState;
        private NSFCompositeState       oRemoveGlassState;
        private NSFCompositeState       oPushGlassToDoorState;
        private NSFCompositeState       oUnCompressSidePreLoadState;
        private NSFCompositeState       oUnCompressTrailingEdgePreLoadsState;
        private NSFCompositeState       oRetractTrailingEdgePinsState;
        private NSFInitialState         oGlassUnLoadingStateInitial;

        // State Machine GlassLoadingState State Definitions
        private NSFChoiceState          oIsSafetyCircuitReset;
        private NSFCompositeState       oCloseDoorToAlignState;
        private NSFCompositeState       oOperatorManualLoadState;
        private NSFChoiceState          oIsDoorOpened;
        private NSFCompositeState       oWaitForDoorOpenToLoadState;
        private NSFCompositeState       oGlassLoadingFromCartState;
        private NSFInitialState         oGlassLoadingStateInitial;
        private NSFCompositeState       oAlignGlassState;
        private NSFCompositeState       oWaitForGlassToEnterState;

        // State Machine Idle State State Definitions
        private NSFChoiceState          oIsGlassLoaded;
        private NSFCompositeState       oGlassUnLoadedState;
        private NSFInitialState         oIdleStateInitial;
        private NSFCompositeState       oGlassLoadedState;

        // State Machine InspectingState State Definitions
        private NSFInitialState         oInspectingStateInitial;
        private NSFCompositeState       oDefectReviewState;
        private NSFCompositeState       oMacroScanState;

        // State Machine ReadyState State Definitions
        private NSFCompositeState       oInspectingState;
        private NSFInitialState         oReadyStateInitial;
        private NSFCompositeState       oIdleState;

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
    } //end CGuideHighLevelStateMachine
} //end GuideHighLevel.GuideHighLevelSystem
