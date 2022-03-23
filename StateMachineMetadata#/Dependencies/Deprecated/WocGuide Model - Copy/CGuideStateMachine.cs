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
namespace Guide.GuideSystem
{
    public partial class CGuideStateMachine
    {
        #region Fields
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CGuideStateMachine");
        //private Semaphore m_semaphoreStateChange = new Semaphore();
        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);
        private object m_objLock = new object();
        private string m_strLastOperatorPrompt = "";

        private EState? m_estatePrevious;
        private EState? m_estateCurrent;
        private string m_strSystemState;
        private IGuideSystem m_iGuideSystem;

        private bool m_bSimulationMode = false;
        private Dictionary<GuideSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<Proj3SystemEventsEnum, NSFEvent>();
        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        private bool m_bInitComplete = false;
        private bool m_bInitAfterSettingsComplete = false;

        #region State Machine Fields


        // ***********************************
        // State Machine NSF Event Definitions
        // ***********************************

        private static NSFEvent oAllAxisHomedEvent;
        private static NSFEvent oClearFaultsEvent;
        private static NSFEvent oGlassNotDetectedEvent;
        private static NSFEvent oCartDockedEvent;
        private static NSFEvent oImagesSavedEvent;
        private static NSFEvent oAbortCompleteEvent;
        private static NSFEvent oGlassRemovedAtEntranceEvent;
        private static NSFEvent oMoveCompleteEvent;
        private static NSFEvent oTrailingEdgePreLoadsExtendedEvent;
        private static NSFEvent oGlassDetectedAtDamperEvent;
        private static NSFEvent oCartLoadTimeoutEvent;
        private static NSFEvent oAbortEvent;
        private static NSFEvent oCartUnDockedEvent;
        private static NSFEvent oTimeOutEvent;
        private static NSFEvent oMoveTimeOutEvent;
        private static NSFEvent oSafetyCircuitResetEvent;
        private static NSFEvent oDoorNotClosed;
        private static NSFEvent oDoorOpenEvent;
        private static NSFEvent oStripeScanReadyEvent;
        private static NSFEvent oUnLoadEvent;
        private static NSFEvent oEvents (see Note for list);
        private static NSFEvent oEStopEvent;
        private static NSFEvent oStripeScanCompleteEvent;
        private static NSFEvent oInitCompleteEvent;
        private static NSFEvent oNo;
        private static NSFEvent oDoorCloseEvent;
        private static NSFEvent oUnloadGlassEvent;
        private static NSFEvent oGlassDetectedAtEntranceEvent;
        private static NSFEvent oStartScanEvent;
        private static NSFEvent oLoadGlassEvent;
        private static NSFEvent oAutoLoadTimeOutEvent;
        private static NSFEvent oTrailingEdgePinsRetractedEvent;
        private static NSFEvent oRetractTrailingEdgePinsTimeOutEvent;
        private static NSFEvent oLaserAndPmtOnEvent;
        private static NSFEvent oTimoutWaitingForPreLoadUnCompressEvent;
        private static NSFEvent oPanelOpenEvent;
        private static NSFEvent oCommitEvent;
        private static NSFEvent oLoadOfflineFileEvent;
        private static NSFEvent oScanSensorFault;
        private static NSFEvent oProcessingCompleteEvent;
        private static NSFEvent oNotifyScanSensorAlarm;
        private static NSFEvent oAllPreLoadsUnCompressedEvent;
        private static NSFEvent oWindowClosedEvent;
        private static NSFEvent oWindowNotClosedEvent;
        private static NSFEvent oLightOnEvent;
        private static NSFEvent oScanSensorFaultEvent;
        private static NSFEvent oMotionFaultEvent;
        private static NSFEvent oSizeCorrectedEvent;
        private static NSFEvent oCabinetOverTempEvent;
        private static NSFEvent oInitEvent;
        private static NSFEvent oBypassSwitchOnEvent;
        private static NSFEvent oVacuumOffEvent;
        private static NSFEvent oAutoLoadCompleteEvent;
        private static NSFEvent oAutoLoadFailedEvent;
        private static NSFEvent oAirOffEvent;
        private static NSFEvent oLoadPosMoveCompleteEvent;

        // ***********************************
        // End of State Machine NSF Event Definitions
        // ***********************************


        // ***********************************
        // State Machine NSF State Definitions
        // ***********************************

        // State Machine Upper Level Composite State Definitions
        private NSFCompositeState oViewOfflineState;
        private NSFCompositeState oImagesSaveState;
        private NSFCompositeState oProcessingState;
        private NSFCompositeState oWaitForWindowClosedState;
        private NSFCompositeState oAbortingState;
        private NSFCompositeState oLaserAndPmtOnState;
        private NSFCompositeState oHomeAllAxisState;
        private NSFCompositeState oFaultAndOperatorClearsGlassState;
        private NSFCompositeState oFaultWithNoGlassState;
        private NSFCompositeState oFaultWithGlassLostState;
        private NSFCompositeState oFaultWithGlassHeldState;
        private NSFCompositeState oGlassLoadingState;
        private NSFCompositeState oGlassUnLoadingState;
        private NSFCompositeState oCartDockedState;
        private NSFCompositeState oSystemPoweredState;

        // State Machine GlassUnLoadingState State Definitions
        private NSFCompositeState       oWaitForPreloadsUnCompressedState;
        private NSFCompositeState       oPrepForUnLoadState;
        private NSFCompositeState       oWalkGlassOutState;
        private NSFCompositeState       oRemoveGlassState;
        private NSFCompositeState       oUnCompressSidePreLoadState;
        private NSFCompositeState       oUnCompressTrailingEdgePreLoadsState;
        private NSFCompositeState       oRetractTrailingEdgePinsState;
        private NSFInitialState         oGlassUnLoadingStateInitial;

        // State Machine PrepForUnLoadState State Definitions
        private NSFInitialState         oPrepForUnLoadStateInitial;
        private NSFCompositeState       oMoveXToUnLoadPosState;
        private NSFChoiceState          oIsCartDocked2;
        private NSFChoiceState          oIsDoorOpened3;
        private NSFCompositeState       oPromptToDockCart;
        private NSFCompositeState       oOpenDoorState;

        // State Machine GlassLoadingState State Definitions
        private NSFChoiceState          oIsAutoLoad;
        private NSFCompositeState       oResetState;
        private NSFChoiceState          oIsDoorOpened2;
        private NSFCompositeState       oMoveToLoadPosState;
        private NSFCompositeState       oAutoLoadSeqState;
        private NSFChoiceState          oIsSafetyCircuitReset;
        private NSFCompositeState       oCloseDoorToAlignState;
        private NSFCompositeState       oOperatorManualLoadState;
        private NSFChoiceState          oIsDoorOpened;
        private NSFCompositeState       oWaitForDoorOpenToLoadState;
        private NSFCompositeState       oGlassLoadingFromCartState;
        private NSFInitialState         oGlassLoadingStateInitial;
        private NSFCompositeState       oAlignGlassState;
        private NSFCompositeState       oWaitForGlassToEnterState;

        // State Machine AutoAlignGlassState State Definitions
        private NSFChoiceState          oIsAnyPreLoadCompressed;
        private NSFCompositeState       oDelayForSpring;
        private NSFChoiceState          oIsNTimesComplete;
        private NSFCompositeState       oStepInSideState;
        private NSFCompositeState       oStepInLeadingEdgeState;
        private NSFInitialState         oAutoAlignGlassStateInitial;

        // State Machine AlignGlassState State Definitions
        private NSFChoiceState          oIsWarningOrCriticalAlarm2;
        private NSFCompositeState       oCorrectGlassSizeState;
        private NSFCompositeState       oClampSheetState;
        private NSFChoiceState          oIsValidGlassSize;
        private NSFInitialState         oAlignGlassStateInitial;
        private NSFCompositeState       oDeployTrailingEdgePins;
        private NSFCompositeState       oAutoAlignGlassState;

        // State Machine WalkGlassOutState State Definitions
        private NSFCompositeState       oMAStageRetractState;
        private NSFCompositeState       oWalkGlassOutState;
        private NSFCompositeState       oMAStageExtendState;
        private NSFCompositeState       oMoveXToWalkGlassOutStartPosState;
        private NSFInitialState         oWalkGlassOutStateInitial;

        // State Machine GlassUnLoadedState State Definitions
        private NSFInitialState         oGlassUnLoadedStateInitial;
        private NSFCompositeState       oCartUnDockedState;

        // State Machine Idle State State Definitions
        private NSFChoiceState          oIsGlassLoaded;
        private NSFCompositeState       oGlassUnLoadedState;
        private NSFInitialState         oIdleStateInitial;
        private NSFCompositeState       oGlassLoadedState;

        // State Machine DefectReviewState State Definitions
        private NSFCompositeState       oRevisitState;
        private NSFInitialState         oDefectReviewStateInitial;

        // State Machine MacroScanState State Definitions
        private NSFChoiceState          oIsMoreStripes;
        private NSFInitialState         oMacroScanStateInitial;
        private NSFCompositeState       oScanningState;
        private NSFCompositeState       oMoveToStripeState;

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
    } //end CGuideStateMachine
} //end Guide.GuideSystem
