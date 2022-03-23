///////////////////////////////////////////////////////////
//  Copyright © Corning Incorporated  2014
//  CGuideStateMachine.cs
//  Implementation of the Class CGuideStateMachine
//  Created on:      30-Jun-2014 9:21:12 PM
//  Original author: webbda
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Guide.GuideInterfaces;

using Corning.Common.Logger;

using NorthStateSoftware.NorthStateFramework;

namespace Guide.GuideSystem
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

    public class CGuideStateMachine : NSFStateMachine, IGuideStateMachine
    {
        #region MemberVariables
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CGuideStateMachine");
        private string m_strSystemState;
        public event StateChangeEventHandler eventStateChange;
        private IGuideSystem m_iGuideSystem;
        private bool m_bSimulationMode = false;
        private bool m_bScanComplete = false;

        // NSFEvents
        private static NSFEvent oInitCompleteEvent;
        private static NSFEvent oInitAfterSettingsCompleteEvent;
        private static NSFEvent oCartDockedEvent;
        private static NSFEvent oCartUnDockedEvent;
        private static NSFEvent oGlassNotDetectedEvent;
        private static NSFEvent oDoorCloseEvent;
        private static NSFEvent oDoorNotClosedEvent;
        private static NSFEvent oDoorOpenedEvent;
        private static NSFEvent oDoorNotOpenEvent;
        private static NSFEvent oClearFaultsEvent;
        private static NSFEvent oEStopEvent;
        private static NSFEvent oInterlocksFaultEvent;
        private static NSFEvent oStartScanEvent;
        private static NSFEvent oLoadStripeImageEvent;
        private static NSFEvent oLoadGlassEvent;
        private static NSFEvent oUnLoadGlassEvent;
        private static NSFEvent oReviewCompleteEvent;
        private static NSFEvent oSaveImageEvent;
        private static NSFEvent oGlassDetectedAtEntranceEvent;
        private static NSFEvent oScanCompleteEvent;
        private static NSFEvent oLaserAndPmtOnEvent;
        private static NSFEvent oStartScanReadyEvent;
        private static NSFEvent oAbortEvent;
        private static NSFEvent oCartLoadTimeoutEvent;
        private static NSFEvent oAutoLoadCompleteEvent;
        private static NSFEvent oAutoLoadFailedEvent;
        private static NSFEvent oAutoLoadTimeOutEvent;
        private static NSFEvent oDelayForSpringTimeoutEvent;
        private static NSFEvent oWaitForPreloadsUncompressedTimeoutEvent;
        private static NSFEvent oGlassDetectedAtDamperEvent;
        private static NSFEvent oSizeCorrectedEvent;
        private static NSFEvent oProceedToFaultEvent;
        private static NSFEvent oMoveCompleteEvent;
        private static NSFEvent oGlassRemovedAtEntranceEvent;
        private static NSFEvent oInitEvent;
        private static NSFEvent oScanSensorFaultEvent;
        private static NSFEvent oMotionFaultEvent;
        private static NSFEvent oStripeFeatureVectorCompleteEvent;
        private static NSFEvent oScanProgressEvent;
        private static NSFEvent oScanSensorStateChangeEvent;
        private static NSFEvent oFanOffEvent;
        private static NSFEvent oPanelOpenEvent;
        private static NSFEvent oCabinetOverTempEvent;
        private static NSFEvent oAirOffEvent;
        private static NSFEvent oVacuumOffEvent;
        private static NSFEvent oWindowNotClosedEvent;
        private static NSFEvent oWindowClosedEvent;
        private static NSFEvent oLightOnEvent;
        private static NSFEvent oTrailingEdgePinsRetractedEvent;
        private static NSFEvent oTrailingEdgePinsExtendedEvent;
        private static NSFEvent oRetractTrailingEdgePinsTimeOutEvent;
        private static NSFEvent oAllAxisHomedEvent;
        private static NSFEvent oLoadPosMoveCompleteEvent;
        private static NSFEvent oByPassSwitchOnEvent;
        private static NSFEvent oSafetyCircuitResetEvent;
        private static NSFEvent oSheetJudgementCompleteEvent;
        private static NSFEvent oAbortCompleteEvent;
        private static NSFEvent oAllPreloadsUnCompressedEvent;
        private static NSFEvent oCommitEvent;
        private static NSFEvent oLoadOfflineFileEvent;
        private static NSFEvent oImagesSavedEvent;
        private static NSFEvent oNgImagesSavedEvent;

        private Dictionary<GuideSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<GuideSystemEventsEnum, NSFEvent>();


        // NSFStates
        private NSFInitialState oGuideStateMachineInitialState;
        private NSFInitialState oSystemPoweredInitialState;
        private NSFCompositeState oInitState;
        private NSFCompositeState oSystemPoweredState;
        private NSFCompositeState oInitAfterSettingsState;
        private NSFCompositeState oHomeAllAxisState;
        private NSFChoiceState oIsHomedChoiceState;
        private NSFChoiceState oIsWarningOrCriticalAlarmChoiceState;
        private NSFCompositeState oFaultedState;
        private NSFInitialState oFaultedInitialState;
        private NSFChoiceState oIsSheetDetected2ChoiceState;
        private NSFCompositeState oFaultWithNoGlassState;
        private NSFCompositeState oFaultAndOperatorClearsGlassState;
        private NSFCompositeState oFaultWithGlassLostState;
        private NSFCompositeState oFaultWithGlassHeldState;
        private NSFCompositeState oReadyState;
        private NSFInitialState oReadyInitialState;
        private NSFCompositeState oIdleState;
        private NSFInitialState oIdleInitialState;
        private NSFCompositeState oGlassUnLoadedState;
        private NSFCompositeState oGlassLoadedState;
        private NSFCompositeState oViewOfflineState;
        private NSFChoiceState oIsGlassLoadedChoiceState;
        private NSFChoiceState oIsCartDockedChoiceState;
        private NSFInitialState oGlassUnLoadedInitialState;
        private NSFCompositeState oCartDockedState;
        private NSFCompositeState oCartUnDockedState;
        private NSFChoiceState oIsGlassLostDetectedChoiceState;
        private NSFChoiceState oIsSheetDetectedChoiceState;

        // GlassLoadingState
        private NSFCompositeState oGlassLoadingState;
        private NSFInitialState oGlassLoadingInitialState;
        private NSFCompositeState oMoveToLoadPosState;
        private NSFChoiceState oIsDoorOpenedChoiceState;
        private NSFCompositeState oWaitForDoorOpenToLoadState;
        private NSFChoiceState oIsAutoLoadChoiceState;
        private NSFCompositeState oOperatorManualLoadState;
        private NSFCompositeState oWaitForGlassToEnterState;
        private NSFCompositeState oGlassLoadingFromCartState;
        private NSFCompositeState oAutoLoadSeqState;
        private NSFChoiceState oIsSafetyCircuitResetChoiceState;
        private NSFCompositeState oResetState;
        private NSFChoiceState oIsDoorOpened2ChoiceState;
        private NSFCompositeState oCloseDoorToAlignState;

        // AlignGlassState
        private NSFCompositeState oAlignGlassState;
        private NSFInitialState oAlignGlassInitialState;
        private NSFChoiceState oIsValidGlassSizeChoiceState;
        private NSFCompositeState oCorrectGlassSizeState;
        private NSFCompositeState oDeployTrailingEdgePinsState;
        private NSFCompositeState oAutoAlignGlassState;
        private NSFInitialState oAutoAlignGlassInitialState;
        private NSFChoiceState oIsNTimesCompleteChoiceState;
        private NSFCompositeState oStepInSideState;
        private NSFCompositeState oStepInleadingEdgeState;
        private NSFChoiceState oIsAnyPreLoadCompressedChoiceState;
        private NSFCompositeState oDelayForSpringState;
        private NSFCompositeState oClampSheetState;
        private NSFChoiceState oIsWarningOrCriticalAlarm2ChoiceState;

        // Inspecting State
        private NSFCompositeState oInspectingState;
        private NSFInitialState oInspectingInitialState;
        private NSFCompositeState oMacroScanState;
        private NSFCompositeState oAbortingState;
        private NSFInitialState oMacroScanInitialState;
        private NSFCompositeState oDefectReviewState;
        private NSFInitialState oDefectReviewInitialState;
        private NSFCompositeState oRevisitState;
        private NSFCompositeState oMoveToStripeState;
        private NSFCompositeState oLaserAndPmtOnState;
        private NSFCompositeState oScanningState;
        private NSFChoiceState oIsMoreStripesChoiceState;
        private NSFChoiceState oIsWarningOrCriticalAlarm3ChoiceState;
        private NSFChoiceState oIsScanSensorReadyChoiceState;
        private NSFChoiceState oIsWindowClosedChoiceState;
        private NSFCompositeState oWaitForWindowClosedState;
        private NSFCompositeState oProcessingState;
        private NSFCompositeState oImagesSaveState;
        private NSFChoiceState oSaveNGImagesChoiceState;

        // GlassUnloadingState
        private NSFCompositeState oGlassUnLoadingState;
        private NSFInitialState oGlassUnLoadingInitialState;
        private NSFCompositeState oPrepForUnloadState;
        private NSFInitialState oPrepForUnloadInitialState;
        private NSFChoiceState oIsCartDocked2ChoiceState;
        private NSFCompositeState oPromptToDockCartState;
        private NSFChoiceState oIsDoorOpened3ChoiceState;
        private NSFCompositeState oOpenDoorState;
        private NSFCompositeState oMoveXToUnloadPosState;
        private NSFCompositeState oUnCompressSidePreLoadState;
        private NSFCompositeState oUnCompressTrailingEdgePreloadsState;
        private NSFCompositeState oWaitForPreloadsUnCompressedState;
        private NSFCompositeState oRetractTrailingEdgePinsState;
        private NSFCompositeState oWalkGlassOutState;
        private NSFCompositeState oMoveXToWalkGlassOutStartPosState;
        private NSFCompositeState oMAStageExtendState;
        private NSFCompositeState oMAStageRetractState;
        private NSFCompositeState oRemoveGlassState;


        // NFS Transitions
        private NSFExternalTransition GuideStateMachineInitialToSystemPoweredTransition;
        private NSFExternalTransition SystemPoweredInitialToInitTransition;
        private NSFExternalTransition InitToInitAfterSettingsTransition;
        private NSFExternalTransition InitAfterSettingsToIsIsWarningOrCriticalAlarmChoiceTransition;
        private NSFExternalTransition IsWarningOrCriticalAlarmChoiceToFaultedTransition;
        private NSFExternalTransition IsWarningOrCriticalAlarmChoiceToIsGlassDetectedChoiceTransition;
        private NSFExternalTransition IsGlassLostDetectedChoiceToIsHomedChoiceTransition;
        private NSFExternalTransition IsGlassLostDetectedChoiceToFaultedTransition;
        private NSFExternalTransition IsHomedChoiceToReadyTransition;
        private NSFExternalTransition IsHomedChoiceToIsSheetDetectedTransition;
        private NSFExternalTransition IsSheetDetectedToHomeAllAxisTransition;
        private NSFExternalTransition IsSheetDetectedToFaultedStateTransition;
        private NSFExternalTransition HomeAllAxisToIsWarningOrCriticalAlarmTransition;
        private NSFExternalTransition IdleInitialToIsGlassLoadedChoiceTransition;
        private NSFExternalTransition IsGlassLoadedToGlassLoadedTransition;
        private NSFExternalTransition IsGlassLoadedToIsCartDockedTransition;
        private NSFExternalTransition IsCartDockedToCartDockedTransition;
        private NSFExternalTransition IsCartDockedToGlassUnloadedTransition;
        private NSFExternalTransition GlassUnLoadedInitialToCartUnDockedTransition;
        private NSFExternalTransition ReadyInitialToIdleTransition;
        private NSFExternalTransition CartUnDockedToCartDockedTransition;
        private NSFExternalTransition CartUnDockedToCartDockedTransition2;
        private NSFExternalTransition CartDockedToCartUnDockedTransition;
        private NSFInternalTransition DoorOpenInternalTransition;
        private NSFInternalTransition DoorCloseInternalTransition;
        private NSFExternalTransition CartDockedToGlassLoadingTransition;
        private NSFExternalTransition GlassLoadedToGlassUnLoadingTransition;
        private NSFInternalTransition GlassUnloadedGlassNotDetectedInternalTransition;
        private NSFExternalTransition MacroScanInitialToLaserAndPmtOnTransition;
        private NSFExternalTransition LaserAndPmtOnToIsMoreStripesTransition;
        private NSFExternalTransition MacroScanToAbortingTransition;
        private NSFExternalTransition AbortingToIdleTransition;
        private NSFExternalTransition AbortingToFaultWithGlassHeldTransition;
        private NSFExternalTransition AbortingToFaultWithGlassHeld2Transition;
        private NSFExternalTransition IsMoreStripesToMoveToStripeTransition;
        private NSFExternalTransition IsMoreStripesToProcessingTransition;
        private NSFExternalTransition ProcessingToDefectReviewTransition;
        private NSFExternalTransition ProcessingToGlassLoadedTransition;
        private NSFExternalTransition MoveToStripeToScanningTransition;
        private NSFExternalTransition ScanningToIsMoreStripesTransition;
        private NSFExternalTransition DefectReviewInitialToRevisitTransition;
        private NSFExternalTransition RevisitStateToGlassUnLoadingTransition;
        private NSFExternalTransition FaultedInitialToIsSheetDetected2ChoiceTransition;
        private NSFExternalTransition IsSheetDetectedChoiceToFaultWithGlassLostTransition;
        private NSFExternalTransition IsSheetDetectedChoiceToFaultWithNoGlassTransition;
        private NSFExternalTransition FaultWithNoGlassToIsWarningOrCriticalAlarmChoiceTransition;
        private NSFExternalTransition FaultAndOperatorClearsGlassToIsWarningOrCriticalAlarmChoiceTransition;
        private NSFExternalTransition FaultWithGlassLostToFaultAndOperatorClearsGlassTransition;
        private NSFExternalTransition FaultWithGlassHeldToFaultWithGlassLostTransition;
        private NSFExternalTransition FaultWithGlassHeldToGlassLoadedTransition;
        private NSFExternalTransition MacroScanToFaultWithGlassHeldTransition;
        private NSFExternalTransition InspectingInitialToMacroScan;
        private NSFExternalTransition GlassLoadingInitialToMoveToLoadPosTransition;
        private NSFExternalTransition IsDoorOpenedChoiceToIsAutoLoadChoiceTransition;
        private NSFExternalTransition IsDoorOpenedChoiceToWaitForDoorOpenToLoadTransition;
        private NSFExternalTransition WaitForDoorOpenToLoadToIdleTransition;
        private NSFExternalTransition WaitForDoorOpenToLoadToIdleTransition2;
        private NSFExternalTransition WaitForDoorOpenToLoadToIsAutoLoadChoiceTransition;
        private NSFExternalTransition IsAutoLoadChoiceToWaitForGlassToEnterTransition;
        private NSFExternalTransition IsAutoLoadChoiceToOperatorManualLoadTransition;
        private NSFExternalTransition WaitForGlassToEnterToIdleTransition;
        private NSFExternalTransition WaitForGlassToEnterToIdleTransition2;
        private NSFExternalTransition WaitForGlassToEnterToGlassLoadingFromCartTransition;
        private NSFInternalTransition GlassLoadingGlassNotDetectedInternalTransition;

        private NSFExternalTransition GlassLoadingFromCartToAutoLoadSeqTransition;
        private NSFExternalTransition AutoLoadSeqToOperatorManualLoadTransition;
        private NSFExternalTransition AutoLoadSeqToIsSafetyCircuitResetTransition;
        private NSFExternalTransition AutoLoadSeqToFaultWithGlassLostTransition;
        private NSFInternalTransition AutoLoadSeqGlassNotDetectedInternalTransition;

        private NSFExternalTransition MoveToLoadPosToIsDoorOpenedChoiceTransition;
        private NSFExternalTransition MoveToLoadPosToIdleTransition;
        private NSFExternalTransition GlassLoadingFromCartToIsSafetyCircuitResetChoiceTransition;
        private NSFExternalTransition IsDoorOpened2ChoiceToCloseDoorToAlignTransition;
        private NSFExternalTransition IsDoorOpened2ChoiceToAlignGlassTransition;
        private NSFExternalTransition CloseDoorToAlignToAlignGlassTransition;
        private NSFExternalTransition AlignGlassInitialToIsValidGlassSizeTransition;

        private NSFExternalTransition IsValidGlassSizeToCorrectGlassSizeTransition;
        private NSFExternalTransition IsValidGlassSizeToDeployTrailingEdgePinsTransition;
        private NSFExternalTransition CorrectGlassSizeToIsValidGlassSizeTransition;
        private NSFExternalTransition CorrectGlassSizeToFaultWithGlassLostTransition;
        private NSFExternalTransition DeployTrailingEdgePinsToAutoAlignGlassTransition;
        private NSFExternalTransition ClampSheetToIsWarningOrCriticalAlarmTransition;

        private NSFExternalTransition AlignGlassToFaultWithGlassLostTransition;
        private NSFExternalTransition AutoAlignGlassInitialToIsNTimesCompleteTransition;
        private NSFExternalTransition IsNTimesCompleteChoiceToClampSheetTransition;
        private NSFExternalTransition IsNTimesCompleteChoiceToStepInSideTransition;
        private NSFExternalTransition StepinSideToStepInLeadingEdgeTransition;
        private NSFExternalTransition StepInLeadingEdgeToDelayForSpringTransition;
        private NSFExternalTransition DelayForSpringStateToIsAnyPreLoadCompressedChoiceTransition;
        private NSFExternalTransition IsAnyPreLoadCompressedToIsNTimesCompleteChoiceTransition;
        private NSFExternalTransition IsAnyPreLoadCompressedToFaultWithGlassLostTransition;

        // Unload
        private NSFExternalTransition GlassUnLoadingInitialToPrepForUnloadTransition;
        private NSFExternalTransition PrepForUnloadInitialToIsCartDocked2Transition;
        private NSFExternalTransition PrepForUnloadToIdleTransition;
        private NSFExternalTransition PrepForUnloadToFaultWithGlassHeldTransition;
        private NSFInternalTransition PrepForUnloadDoorNotOpenInternalTransition;
        private NSFExternalTransition MoveXToUnloadPosToUnCompressSidePreLoadTransition;
        private NSFExternalTransition UnCompressSidePreLoadToUnCompressTrailingEdgePreLoadsTransition;
        private NSFExternalTransition UnCompressTrailingEdgePreLoadsToWaitForPreloadsUnCompressedTransition;
        private NSFExternalTransition WaitForPreloadsUnCompressedToRetractTrailingEdgePinsTransition;
        private NSFExternalTransition WaitForPreloadsUnCompressedToFaultWithGlassLostTransition;
        private NSFExternalTransition RetractTrailingEdgePinsToFaultWithGlassLostTransition;
        private NSFExternalTransition RetractTrailingEdgePinsToMoveToWalkGlassOutStartPosTransition;
        private NSFExternalTransition MoveToWalkGlassOutStartPosToMAStageExtendTransition;
        private NSFExternalTransition MAStageExtendToWalkGlassOutTransition;
        private NSFExternalTransition IsCartDocked2ChoiceToIsDoorOpened3ChoiceTransition;
        private NSFExternalTransition IsCartDocked2ChoiceToPromptToDockTransition;
        private NSFExternalTransition IsDoorOpened3ChoiceToOpenDoorTransition;
        private NSFExternalTransition PromptToDockCartToIsDoorOpen3Transition;
        private NSFExternalTransition IsDoorOpened3ChoiceToMoveXToUnloadPosTransition;
        private NSFExternalTransition OpenDoorToMoveXToUnloadPosTransition;
        private NSFExternalTransition WalkGlassOutToMAStageRetractTransition;
        private NSFExternalTransition MAStageRetractToRemoveGlassTransition;

        private NSFExternalTransition RemoveGlassToIdleTransition;
        private NSFExternalTransition OperatorManualLoadToIsSafetyCircuitResetTransition;
        private NSFExternalTransition SystemPoweredToInitTransition;
        private NSFExternalTransition ReadyToFaultWithGlassLostTransition5;
        private NSFExternalTransition ReadyToFaultWithGlassLostTransition6;
        private NSFExternalTransition ReadyToFaultWithGlassLostTransition7;
        private NSFExternalTransition ReadyToFaultWithGlassLostTransition8;
        private NSFExternalTransition ReadyToFaultWithGlassLostTransition9;
        private NSFExternalTransition ReadyToFaultWithGlassLostTransition10;
        private NSFExternalTransition GlassLoadedToFaultWithGlassHeldTransition;
        private NSFExternalTransition GlassLoadedToFaultWithGlassHeldTransition2;
        private NSFExternalTransition GlassLoadedToFaultWithGlassHeldTransition3;
        private NSFExternalTransition GlassLoadedToFaultWithGlassHeldTransition4;
        private NSFExternalTransition GlassLoadedToFaultWithGlassHeldTransition5;
        private NSFExternalTransition GlassLoadedToFaultWithGlassHeldTransition6;
        private NSFExternalTransition GlassLoadedToFaultWithGlassHeldTransition7;
        private NSFExternalTransition GlassLoadedToFaultWithGlassHeldTransition8;
        private NSFExternalTransition GlassLoadedToFaultWithGlassHeldTransition9;
        private NSFExternalTransition GlassLoadedToIsWarningOrCriticalAlarm3ChoiceTransition;
        private NSFExternalTransition IsWarningOrCriticalAlarm3ChoiceToIsScanSensorReadyChoiceTransition;
        private NSFExternalTransition IsScanSensorReadyChoiceToFaultWithGlassHeldTransition;
        private NSFExternalTransition IsScanSensorReadyChoiceToIsWindowClosedChoiceTransition;
        private NSFExternalTransition IsWindowClosedChoiceToInspectingTransition;
        private NSFExternalTransition IsWindowClosedChoiceToWaitForWindowClosedTransition;
        private NSFExternalTransition WaitForWindowClosedToInspectingTransition;

        private NSFExternalTransition RevisitToSaveNGImagesChoiceTransition;
        private NSFExternalTransition SaveNGImagesChoiceToImagesSaveTransition;
        private NSFExternalTransition SaveNGImagesChoiceToGlassLoadedTransition;
        private NSFExternalTransition ImagesSaveToGlassLoadedTransition;
        private NSFExternalTransition ImagesSaveToGlassLoadedTransition2;
        private NSFExternalTransition RevisitToIsWarningOrCriticalAlarm3ChoiceTransition;
        private NSFExternalTransition IdleToViewOfflineTransition;
        private NSFExternalTransition ViewOfflineToIsGlassLoadedTransition;
        private NSFInternalTransition RevisitInternalNgImageSavedTransition;
        private NSFInternalTransition RevisitInternalAbortTransition;

        private NSFExternalTransition IsWarningOrCriticalAlarm3ChoiceToFaultWithGlassHeldTransition;
        private NSFExternalTransition InspectingToFaultWithGlassHeldTransition;
        private NSFExternalTransition InspectingToFaultWithGlassHeldTransition2;
        private NSFExternalTransition InspectingToFaultWithGlassHeldTransition3;
        private NSFExternalTransition InspectingToFaultWithGlassHeldTransition4;
        private NSFExternalTransition InspectingToFaultWithGlassHeldTransition5;
        private NSFExternalTransition InspectingToFaultWithGlassHeldTransition6;
        private NSFExternalTransition InspectingToFaultWithGlassHeldTransition7;
        private NSFExternalTransition InspectingToFaultWithGlassHeldTransition8;
        private NSFExternalTransition GlassLoadingToFaultWithGlassLostTransition;
        private NSFExternalTransition GlassUnLoadingToFaultWithGlassLostTransition;
        private NSFExternalTransition GlassUnLoadingToFaultWithGlassLostTransition2;
        private NSFInternalTransition GlassUnloadingGlassNotDetectedInternalTransition;

        private NSFExternalTransition GlassLoadedToFaultWithGlassLostTransition;
        private NSFExternalTransition ReadyToFaultTransition;
        private NSFExternalTransition ReadyToFaultTransition2;
        private NSFExternalTransition ReadyToFaultTransition3;
        private NSFExternalTransition ReadyToFaultTransition4;
        private NSFExternalTransition IsSafetyCircuitResetToResetTransition;
        private NSFExternalTransition ResetToIsDoorOpened2Transition;
        private NSFExternalTransition IsSafetyCircuitResetToIsDoorOpened2Transition;
        private NSFExternalTransition IsWarningOrCriticalAlarm2ChoiceToFaultWithGlassHeldTransition;
        private NSFExternalTransition IsWarningOrCriticalAlarm2ChoiceToGlassLoadedTransition;

        private NSFInternalTransition ScanProgessEventToReadyStateTransition;
        private NSFInternalTransition ZetaStateChangeEventToReadyStateTransition;
        private NSFInternalTransition StripeFeatureVectorCompleteInternalTransition;

        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        #endregion //MemberVariables

        #region Properties

        public string SystemState { get { return m_strSystemState; } }
        bool SimulationModeEnabled
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

        #endregion //Properties

        #region Constructors

        public CGuideStateMachine(string strName, IGuideSystem iGuideSystem)
            : base(strName, new NSFEventThread(strName))
        {
            // Capture reference to the parent system for calling back to perform system functions
            m_iGuideSystem = iGuideSystem;

            // Init State Machine
            CreateStateMachine();

            // Init Operator Prompts
            CreateOperatorPrompts();
        }

        ~CGuideStateMachine()
        {

        }
        #endregion //Constructor

        #region Methods

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

                // Events
                oInitCompleteEvent = new NSFEvent("InitComplete", this, this);
                oInitAfterSettingsCompleteEvent = new NSFEvent("InitAfterSettingsComplete", this, this);
                oLoadGlassEvent = new NSFEvent("LoadGlassEvent", this, this);
                oCartDockedEvent = new NSFEvent("CartDockedEvent", this, this);
                oCartUnDockedEvent = new NSFEvent("CartUnDockedEvent", this, this);
                oGlassNotDetectedEvent = new NSFEvent("GlassNotDetectedEvent", this, this);
                oDoorCloseEvent = new NSFEvent("DoorClosedEvent", this, this);
                oDoorNotClosedEvent = new NSFEvent("DoorNotClosedEvent", this, this);
                oDoorOpenedEvent = new NSFEvent("DoorOpenedEvent", this, this);
                oDoorNotOpenEvent = new NSFEvent("DoorNotOpenEvent", this, this);
                oClearFaultsEvent = new NSFEvent("ClearFaultsEvent", this, this);
                oEStopEvent = new NSFEvent("EStopEvent", this, this);
                oInterlocksFaultEvent = new NSFEvent("InterlocksFaultEvent", this, this);
                oStartScanEvent = new NSFEvent("StartScanEvent", this, this);
                oLoadStripeImageEvent = new NSFEvent("LoadStripeImageEvent", this, this);
                oUnLoadGlassEvent = new NSFEvent("UnLoadGlassEvent", this, this);
                oReviewCompleteEvent = new NSFEvent("ReviewCompleteEvent", this, this);
                oSaveImageEvent = new NSFEvent("SaveImageEvent", this, this);
                oGlassDetectedAtEntranceEvent = new NSFEvent("GlassDetectedAtEntranceEvent", this, this);
                oScanCompleteEvent = new NSFEvent("ScanCompleteEvent", this, this);
                oStartScanReadyEvent = new NSFEvent("StartScanReadyEvent", this, this);
                oLaserAndPmtOnEvent = new NSFEvent("LaserAndPmtOnEvent", this, this);
                oAbortEvent = new NSFEvent("AbortEvent", this, this);
                oCartLoadTimeoutEvent = new NSFEvent("CartLoadTimeoutEvent", this, this);
                oAutoLoadCompleteEvent = new NSFEvent("AutoLoadCompleteEvent", this, this);
                oAutoLoadFailedEvent = new NSFEvent("AutoLoadFailedEvent", this, this);
                oAutoLoadTimeOutEvent = new NSFEvent("AutoLoadTimeOutEvent", this, this);
                oDelayForSpringTimeoutEvent = new NSFEvent("DelayForSpringTimeoutEvent", this, this);
                oWaitForPreloadsUncompressedTimeoutEvent = new NSFEvent("WaitForPreloadsUncompressedTimeoutEvent", this, this);
                oGlassDetectedAtDamperEvent = new NSFEvent("GlassDetectedAtCatcherEvent", this, this);
                oSizeCorrectedEvent = new NSFEvent("SizeCorrectedEvent", this, this);
                oProceedToFaultEvent = new NSFEvent("ProceedToFaultEvent", this, this);
                oMoveCompleteEvent = new NSFEvent("MoveCompleteEvent", this, this);
                oGlassRemovedAtEntranceEvent = new NSFEvent("GlassRemovedAtEntranceEvent", this, this);
                oInitEvent = new NSFEvent("InitEvent", this, this);
                oScanSensorFaultEvent = new NSFEvent("ScanSensorFaultEvent", this, this);
                oMotionFaultEvent = new NSFEvent("MotionFaultEvent", this, this);
                oStripeFeatureVectorCompleteEvent = new NSFEvent("StripeFeatureVectorCompleteEvent", this, this);
                oScanProgressEvent = new NSFEvent("ScanProgressEvent", this, this);
                oScanSensorStateChangeEvent = new NSFEvent("ScanSensorStateChangeEvent", this, this);
                oFanOffEvent = new NSFEvent("FanOffEvent", this, this);
                oPanelOpenEvent = new NSFEvent("PanelOpenEvent", this, this);
                oCabinetOverTempEvent = new NSFEvent("CabinetOverTempEvent", this, this);
                oAirOffEvent = new NSFEvent("AirOffEvent", this, this);
                oVacuumOffEvent = new NSFEvent("VacuumOffEvent", this, this);
                oWindowNotClosedEvent = new NSFEvent("WindowNotClosedEvent", this, this);
                oLightOnEvent = new NSFEvent("LightOnEvent", this, this);
                oTrailingEdgePinsRetractedEvent = new NSFEvent("TrailingEdgePinsRetractedEvent", this, this);
                oTrailingEdgePinsExtendedEvent = new NSFEvent("TrailingEdgePinsExtendedEvent", this, this);
                oLoadPosMoveCompleteEvent = new NSFEvent("LoadPosMoveCompleteEvent", this, this);
                oByPassSwitchOnEvent = new NSFEvent("ByPassSwitchOnEvent", this, this);
                oSafetyCircuitResetEvent = new NSFEvent("SafetyCircuitResetEvent", this, this);
                oSheetJudgementCompleteEvent = new NSFEvent("SheetJudgementCompleteEvent", this, this);
                oRetractTrailingEdgePinsTimeOutEvent = new NSFEvent("RetractTrailingEdgePinsTimeOutEvent", this, this);
                oAllAxisHomedEvent = new NSFEvent("AllAxisHomedEvent", this, this);
                oAbortCompleteEvent = new NSFEvent("AbortCompleteEvent", this, this);
                oAllPreloadsUnCompressedEvent = new NSFEvent("AllPreloadsNotCompressedEvent", this, this);
                oWindowClosedEvent = new NSFEvent("WindowClosedEvent", this, this);
                oCommitEvent = new NSFEvent("CommitEvent", this, this);
                oLoadOfflineFileEvent = new NSFEvent("LoadOfflineFileEvent", this, this);
                oImagesSavedEvent = new NSFEvent("ImagesSavedEvent", this, this);
                oNgImagesSavedEvent = new NSFEvent("NgImagesSavedEvent", this, this);

                // Create dictionary mapping event enum to actual event
                m_dictEventByEnum.Add(GuideSystemEventsEnum.InitCompleteEvent, oInitCompleteEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.InitAfterSettingsCompleteEvent, oInitAfterSettingsCompleteEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.CartDockedEvent, oCartDockedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.CartUnDockedEvent, oCartUnDockedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.GlassNotDetectedEvent, oGlassNotDetectedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.DoorClosedEvent, oDoorCloseEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.DoorNotClosedEvent, oDoorNotClosedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.DoorOpenedEvent, oDoorOpenedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.DoorNotOpenEvent, oDoorNotOpenEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.ClearFaultsEvent, oClearFaultsEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.EStopEvent, oEStopEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.InterlocksFaultEvent, oInterlocksFaultEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.StartScanEvent, oStartScanEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.LoadStripeImageEvent, oLoadStripeImageEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.LoadGlassEvent, oLoadGlassEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.UnLoadGlassEvent, oUnLoadGlassEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.ReviewCompleteEvent, oReviewCompleteEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.SaveImageEvent, oSaveImageEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.GlassDetectedAtEntranceEvent, oGlassDetectedAtEntranceEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.StripeScanCompleteEvent, oScanCompleteEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.LaserAndPmtOnEvent, oLaserAndPmtOnEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.StripeScanReadyEvent, oStartScanReadyEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.AbortEvent, oAbortEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.GlassDetectedAtDamperEvent, oGlassDetectedAtDamperEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.SizeCorrectedEvent, oSizeCorrectedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.ProceedToFaultEvent, oProceedToFaultEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.MoveCompleteEvent, oMoveCompleteEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.GlassRemovedAtEntranceEvent, oGlassRemovedAtEntranceEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.InitEvent, oInitEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.ScanSensorFaultEvent, oScanSensorFaultEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.MotionFaultEvent, oMotionFaultEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.StripeFeatureVectorCompleteEvent, oStripeFeatureVectorCompleteEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.ScanProgressEvent, oScanProgressEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.ScanSensorStateChangeEvent, oScanSensorStateChangeEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.FanOffEvent, oFanOffEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.PanelOpenEvent, oPanelOpenEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.CabinetOverTempEvent, oCabinetOverTempEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.AirOffEvent, oAirOffEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.VacuumOffEvent, oVacuumOffEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.WindowNotClosedEvent, oWindowNotClosedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.LightOnEvent, oLightOnEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.LoadPosMoveCompleteEvent, oLoadPosMoveCompleteEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.AutoLoadCompleteEvent, oAutoLoadCompleteEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.AutoLoadFailedEvent, oAutoLoadFailedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.AutoLoadTimeOutEvent, oAutoLoadTimeOutEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.TrailingEdgePinsRetractedEvent, oTrailingEdgePinsRetractedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.TrailingEdgePinsExtendedEvent, oTrailingEdgePinsExtendedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.ByPassSwitchOnEvent, oByPassSwitchOnEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.SafetyCircuitResetEvent, oSafetyCircuitResetEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.SheetJudgementCompleteEvent, oSheetJudgementCompleteEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.AllAxisHomedEvent, oAllAxisHomedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.AbortCompleteEvent, oAbortCompleteEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.AllPreloadsUnCompressedEvent, oAllPreloadsUnCompressedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.WindowClosedEvent, oWindowClosedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.CommitEvent, oCommitEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.LoadOfflineFileEvent, oLoadOfflineFileEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.ImagesSavedEvent, oImagesSavedEvent);
                m_dictEventByEnum.Add(GuideSystemEventsEnum.NgImagesSavedEvent, oNgImagesSavedEvent);

                // Regions and States for SystemPoweredState
                oGuideStateMachineInitialState = new NSFInitialState("GuideStateMachineInitial", this);
                oSystemPoweredState = new NSFCompositeState("SystemPowered", this, null, null);
                oSystemPoweredInitialState = new NSFInitialState("SystemPoweredInitial", oSystemPoweredState);
                oInitState = new NSFCompositeState("Init", oSystemPoweredState, null, null);
                oInitAfterSettingsState = new NSFCompositeState("InitAfterSettings", oSystemPoweredState, null, null);
                oHomeAllAxisState = new NSFCompositeState("HomeAllAxis", oSystemPoweredState, HomeAllAxis, null);
                oIsHomedChoiceState = new NSFChoiceState("IsHomedChoice", oSystemPoweredState);
                oIsWarningOrCriticalAlarmChoiceState = new NSFChoiceState("IsWarningOrCriticalAlarm", oSystemPoweredState);
                oFaultedState = new NSFCompositeState("Faulted", oSystemPoweredState, FaultedStateEntryActions, DisableManualMotion);
                oReadyState = new NSFCompositeState("Ready", oSystemPoweredState, DisableLoadGlassAndStartOnGui, StopAllMotion);
                oIsGlassLostDetectedChoiceState = new NSFChoiceState("IsGlassLostDetected", oSystemPoweredState);
                oIsSheetDetectedChoiceState = new NSFChoiceState("IsSheetDetected", oSystemPoweredState);

                // Regions and States for FaultedState
                oFaultedInitialState = new NSFInitialState("FaultedInitial", oFaultedState);
                oIsSheetDetected2ChoiceState = new NSFChoiceState("IsSheetDetected2", oFaultedState);
                oFaultWithNoGlassState = new NSFCompositeState("FaultWithNoGlass", oFaultedState, null, null);
                oFaultAndOperatorClearsGlassState = new NSFCompositeState("FaultAndOperatorClearsGlass", oFaultedState, null, ClearFaults);
                oFaultWithGlassLostState = new NSFCompositeState("FaultWithGlassLost", oFaultedState, null, null);
                oFaultWithGlassHeldState = new NSFCompositeState("FaultWithGlassHeld", oFaultedState, null, null);

                // Regions and States for ReadyState
                oReadyInitialState = new NSFInitialState("ReadyInitial", oReadyState);
                oIdleState = new NSFCompositeState("Idle", oReadyState, IdleEntryActions, IdleExitActions);
                oGlassLoadingState = new NSFCompositeState("GlassLoading", oReadyState, GlassLoadingEntryActions, GlassLoadingExitActions);
                oInspectingState = new NSFCompositeState("Inspecting", oReadyState, InspectingStateEntryActions, InspectingStateExitActions);
                oGlassUnLoadingState = new NSFCompositeState("GlassUnLoading", oReadyState, GlassUnLoadingEntryAction, GlassUnloadingExitAction);

                // Regions and States for IdleState
                oIdleInitialState = new NSFInitialState("IdleInitial", oIdleState);
                oGlassUnLoadedState = new NSFCompositeState("GlassUnLoaded", oIdleState, null, null);
                oGlassLoadedState = new NSFCompositeState("GlassLoaded", oIdleState, GlassLoadedEntryActions, GlassLoadedExitActions);
                oViewOfflineState = new NSFCompositeState("ViewOffline", oIdleState, ViewOfflineStateEntryActions, ViewOfflineStateExitActions);
                oIsGlassLoadedChoiceState = new NSFChoiceState("IsGlassLoaded", oIdleState);
                oIsCartDockedChoiceState = new NSFChoiceState("IsCartDocked", oIdleState);
                oIsWindowClosedChoiceState = new NSFChoiceState("IsWindowClosed", oIdleState);
                oWaitForWindowClosedState = new NSFCompositeState("WaitForWindowClosed", oIdleState, null, null);

                // Regions and States for GlassUnLoadedState
                oGlassUnLoadedInitialState = new NSFInitialState("GlassUnLoadedInitial", oGlassUnLoadedState);
                oCartDockedState = new NSFCompositeState("CartDocked", oGlassUnLoadedState, EnableLoadGlassOnGuiIfDoorOpened, DisableLoadGlassOnGui);
                oCartUnDockedState = new NSFCompositeState("CartUnDocked", oGlassUnLoadedState, null, null);

                // Regions and States for InspectingState
                oInspectingInitialState = new NSFInitialState("InspectingInitial", oInspectingState);
                oMacroScanState = new NSFCompositeState("MacroScan", oInspectingState, MacroScanStateEntryActions, MacroScanStateExitActions);
                oAbortingState = new NSFCompositeState("Aborting", oInspectingState, AbortScan, null);
                oDefectReviewState = new NSFCompositeState("DefectReview", oInspectingState, DefectReviewStateEntryActions, DefectReviewStateExitActions);
                oProcessingState = new NSFCompositeState("Processing", oInspectingState, null, null);
                oImagesSaveState = new NSFCompositeState("ImagesSave", oInspectingState, NotifySaveComplete, ImagesSaveExitActions);
                oSaveNGImagesChoiceState = new NSFChoiceState("SaveNGImagesChoice", oInspectingState);

                // Regions and States for MacroScanState
                oMacroScanInitialState = new NSFInitialState("MacroScanInitial", oMacroScanState);
                oMoveToStripeState = new NSFCompositeState("MoveToStripe", oMacroScanState, PrepareStripeScanAsync, null);
                oLaserAndPmtOnState = new NSFCompositeState("LaserAndPmtOnState", oMacroScanState, LaserAndPmtOn, null);
                oScanningState = new NSFCompositeState("Scanning", oMacroScanState, StripeScanAsync, null);
                oIsMoreStripesChoiceState = new NSFChoiceState("IsMoreStripesChoice", oMacroScanState);

                // Regions and States for DefectReviewState
                oDefectReviewInitialState = new NSFInitialState("DefectReviewInitial", oDefectReviewState);
                oRevisitState = new NSFCompositeState("Revisit", oDefectReviewState, RevisitStateEntryActions, DisableManualMotion);

                // Regions and States for GlassLoadingState
                oGlassLoadingInitialState = new NSFInitialState("GlassLoadingInitial", oGlassLoadingState);
                oMoveToLoadPosState = new NSFCompositeState("MoveToLoadPos", oGlassLoadingState, MoveAllToLoadPosAndEnableGlassSizeEntry, null);
                oIsDoorOpenedChoiceState = new NSFChoiceState("IsDoorOpened", oGlassLoadingState);
                oWaitForDoorOpenToLoadState = new NSFCompositeState("WaitForDoorOpenToLoad", oGlassLoadingState, null, null);
                oIsAutoLoadChoiceState = new NSFChoiceState("IsDoorOpened", oGlassLoadingState);
                oOperatorManualLoadState = new NSFCompositeState("OperatorManualLoad", oGlassLoadingState, DisableCartAndMailSlotAlarm, null);
                oWaitForGlassToEnterState = new NSFCompositeState("WaitForGlassToEnter", oGlassLoadingState, null, null);
                oGlassLoadingFromCartState = new NSFCompositeState("GlassLoadingFromCart", oGlassLoadingState, StartCartLoadTimer, StopCartLoadTimer);
                oCloseDoorToAlignState = new NSFCompositeState("CloseDoorToAlign", oGlassLoadingState, DisableMailSlotAlarms, null);
                oAlignGlassState = new NSFCompositeState("AlignGlass", oGlassLoadingState, AlignGlassEntryActions, null);
                oAutoLoadSeqState = new NSFCompositeState("AutoLoadSeq", oGlassLoadingState, AutoLoadGlass, null);
                oIsSafetyCircuitResetChoiceState = new NSFChoiceState("oIsSafetyCircuitReset", oGlassLoadingState);
                oResetState = new NSFCompositeState("Reset", oGlassLoadingState, null, null);
                oIsDoorOpened2ChoiceState = new NSFChoiceState("IsDoorOpened2", oGlassLoadingState);

                // Regions and States for AlignGlassState
                oAlignGlassInitialState = new NSFInitialState("AlignGlassInitial", oAlignGlassState);
                oIsValidGlassSizeChoiceState = new NSFChoiceState("IsValidGlassSize", oAlignGlassState);
                oCorrectGlassSizeState = new NSFCompositeState("CorrectGlassSize", oAlignGlassState, null, null);
                oDeployTrailingEdgePinsState = new NSFCompositeState("DeployTrailingEdgePins", oAlignGlassState, DeployTrailingEdgePinsAndDamper, null);
                oAutoAlignGlassState = new NSFCompositeState("AutoAlignGlass", oAlignGlassState, ResetNTimesCounter, null);
                oClampSheetState = new NSFCompositeState("ClampSheet", oAlignGlassState, SlowSqeezeWidthAndLength, null);
                oIsWarningOrCriticalAlarm2ChoiceState = new NSFChoiceState("IsWarningorCriticalAlarm2", oAlignGlassState);


                // Regions and States for AutoAlignGlassState
                oAutoAlignGlassInitialState = new NSFInitialState("AutoAlignGlassInitial", oAutoAlignGlassState);
                oIsNTimesCompleteChoiceState = new NSFChoiceState("IsNTimesComplete", oAutoAlignGlassState);
                oIsAnyPreLoadCompressedChoiceState = new NSFChoiceState("IsAnyPreLoadCompressed", oAutoAlignGlassState);
                oStepInleadingEdgeState = new NSFCompositeState("StepInLeadingEdge", oAutoAlignGlassState, MoveEdgeIn, null);
                oStepInSideState = new NSFCompositeState("StepInSide", oAutoAlignGlassState, MoveSideIn, null);
                oDelayForSpringState = new NSFCompositeState("DelayForSpring", oAutoAlignGlassState, StartDelayForSpringTimer, null);
                oIsWarningOrCriticalAlarm3ChoiceState = new NSFChoiceState("oIsWarningOrCriticalAlarm3", oIdleState);
                oIsScanSensorReadyChoiceState = new NSFChoiceState("oIsScanSensorReady", oIdleState);

                // Regions and States for GlassUnloadingState
                oGlassUnLoadingInitialState = new NSFInitialState("GlassUnLoadingInitial", oGlassUnLoadingState);
                oPrepForUnloadState = new NSFCompositeState("PrepForUnload", oGlassUnLoadingState, null, null);
                oPrepForUnloadInitialState = new NSFInitialState("PrepForUnloadInitial", oPrepForUnloadState);
                oIsCartDocked2ChoiceState = new NSFChoiceState("IsCartDocked2Choice", oPrepForUnloadState);
                oPromptToDockCartState = new NSFCompositeState("PromptToDockCart", oPrepForUnloadState, null, EnableCartUnDockedAlarm);
                oIsDoorOpened3ChoiceState = new NSFChoiceState("IsDoorOpened3Choice", oPrepForUnloadState);
                oOpenDoorState = new NSFCompositeState("OpenDoor", oPrepForUnloadState, DisableMailSlotAlarms, EnableMailSlotClosedAlarm);
                oMoveXToUnloadPosState = new NSFCompositeState("MoveXToUnloadPos", oPrepForUnloadState, MoveXToUnloadPos, null);
                oMoveXToWalkGlassOutStartPosState = new NSFCompositeState("MoveXToWalkGlassOutStartPos", oGlassUnLoadingState, MoveXToWalkGlassOutStartPos, null);
                oUnCompressTrailingEdgePreloadsState = new NSFCompositeState("UnCompressTrailingEdgePreloads", oGlassUnLoadingState, UnCompressTrailingEdgePreLoads, null);
                oUnCompressSidePreLoadState = new NSFCompositeState("UnCompressSidePreLoads", oGlassUnLoadingState, UnCompressSidePreLoads, null);
                oWaitForPreloadsUnCompressedState = new NSFCompositeState("WaitForPreloadsUnCompressed", oGlassUnLoadingState, StartWaitForPreloadsUnCompressedTimer, StopWaitForPreloadsUnCompressedTimer);
                oRetractTrailingEdgePinsState = new NSFCompositeState("RetractTrailingEdgePins", oGlassUnLoadingState, RetractTrailingEdgePins, null);
                oWalkGlassOutState = new NSFCompositeState("WalkGlassOut", oGlassUnLoadingState, MoveXToUnloadPos, null);
                oRemoveGlassState = new NSFCompositeState("RemoveGlass", oGlassUnLoadingState, null, null);
                oMAStageExtendState = new NSFCompositeState("MAStageExtend", oGlassUnLoadingState, MAStageExtend, null);
                oMAStageRetractState = new NSFCompositeState("MAStageRetract", oGlassUnLoadingState, MAStageRetract, null);

                // Transitions for SystemPoweredState   
                GuideStateMachineInitialToSystemPoweredTransition = new NSFExternalTransition("GuideStateMachineInitialToSystemPowered", oGuideStateMachineInitialState, oSystemPoweredState, null, null, null);
                SystemPoweredInitialToInitTransition = new NSFExternalTransition("SystemPoweredInitialToInit", oSystemPoweredInitialState, oInitState, null, null, null);
                SystemPoweredToInitTransition = new NSFExternalTransition("SystemPoweredToInit", oSystemPoweredState, oInitState, oInitEvent, null, null);
                InitToInitAfterSettingsTransition = new NSFExternalTransition("InitToInitAfterSettings", oInitState, oInitAfterSettingsState, oInitCompleteEvent, null, ProcessInitAfterSettings);
                InitAfterSettingsToIsIsWarningOrCriticalAlarmChoiceTransition = new NSFExternalTransition("InitAfterSettingsToIsIsWarningOrCriticalAlarmChoice", oInitAfterSettingsState, oIsWarningOrCriticalAlarmChoiceState, oInitAfterSettingsCompleteEvent, null, null);
                IsWarningOrCriticalAlarmChoiceToFaultedTransition = new NSFExternalTransition("IsFaultedChoiceToFaulted", oIsWarningOrCriticalAlarmChoiceState, oFaultedState, null, IsCriticalAlarms, null);
                IsWarningOrCriticalAlarmChoiceToIsGlassDetectedChoiceTransition = new NSFExternalTransition("IsWarningOrCriticalAlarmChoiceToIsGlassDetectedChoice", oIsWarningOrCriticalAlarmChoiceState, oIsGlassLostDetectedChoiceState, null, Else, null);
                IsGlassLostDetectedChoiceToFaultedTransition = new NSFExternalTransition("IsGlassLostDetectedChoiceToFaulted", oIsGlassLostDetectedChoiceState, oFaultedState, null, IsGlassLostDetected, null);
                IsGlassLostDetectedChoiceToIsHomedChoiceTransition = new NSFExternalTransition("IsGlassLostDetectedChoiceToIsHomedChoice", oIsGlassLostDetectedChoiceState, oIsHomedChoiceState, null, Else, null);
                IsHomedChoiceToReadyTransition = new NSFExternalTransition("IsHomedChoiceToIsReady", oIsHomedChoiceState, oReadyState, null, IsHomed, null);
                IsHomedChoiceToIsSheetDetectedTransition = new NSFExternalTransition("IsHomedChoiceToIsSheetDetected", oIsHomedChoiceState, oIsSheetDetectedChoiceState, null, Else, null);
                IsSheetDetectedToFaultedStateTransition = new NSFExternalTransition("IsSheetDetectedToFaultedState", oIsSheetDetectedChoiceState, oFaultedState, null, IsGlassDetected, null);
                IsSheetDetectedToHomeAllAxisTransition = new NSFExternalTransition("IsGlassLoaded2ToHomeAllAxis", oIsSheetDetectedChoiceState, oHomeAllAxisState, null, Else, null);
                HomeAllAxisToIsWarningOrCriticalAlarmTransition = new NSFExternalTransition("HomeAllAxisToIsWarningOrCriticalAlarm", oHomeAllAxisState, oIsWarningOrCriticalAlarmChoiceState, oAllAxisHomedEvent, null, null);
                ReadyToFaultWithGlassLostTransition5 = new NSFExternalTransition("ReadyToFaultWithGlassLost5", oReadyState, oFaultWithGlassLostState, oEStopEvent, null, null);
                ReadyToFaultWithGlassLostTransition6 = new NSFExternalTransition("ReadyToFaultWithGlassLost6", oReadyState, oFaultWithGlassLostState, oMotionFaultEvent, null, NotifyMotionAlarm);
                ReadyToFaultWithGlassLostTransition7 = new NSFExternalTransition("ReadyToFaultWithGlassLost7", oReadyState, oFaultWithGlassLostState, oScanSensorFaultEvent, null, NotifyScanSensorAlarm);
                ReadyToFaultWithGlassLostTransition8 = new NSFExternalTransition("ReadyToFaultWithGlassLost8", oReadyState, oFaultWithGlassLostState, oGlassNotDetectedEvent, null, null);// TODO:  Make notify alarm for this when time permits
                ReadyToFaultWithGlassLostTransition9 = new NSFExternalTransition("ReadyToFaultWithGlassLost9", oReadyState, oFaultWithGlassLostState, oByPassSwitchOnEvent, IsNotAdminPrivilege, null);
                ReadyToFaultWithGlassLostTransition10 = new NSFExternalTransition("ReadyToFaultWithGlassLost10", oReadyState, oFaultWithGlassLostState, oInterlocksFaultEvent, null, null);
                GlassUnloadedGlassNotDetectedInternalTransition = new NSFInternalTransition("GlassUnloadedGlassNotDetectedInternalTransition", oGlassUnLoadedState, oGlassNotDetectedEvent, null, null);

                // Transitions for ReadyState 
                ReadyInitialToIdleTransition = new NSFExternalTransition("ReadyInitialToIdleState", oReadyInitialState, oIdleState, null, null, null);
                IdleInitialToIsGlassLoadedChoiceTransition = new NSFExternalTransition("SystemPoweredInitialToInit", oIdleInitialState, oIsGlassLoadedChoiceState, null, null, null);
                ReadyToFaultTransition = new NSFExternalTransition("ReadyToFault", oReadyState, oFaultedState, oPanelOpenEvent, null, null);
                ReadyToFaultTransition2 = new NSFExternalTransition("ReadyToFault2", oReadyState, oFaultedState, oCabinetOverTempEvent, null, null);
                ReadyToFaultTransition4 = new NSFExternalTransition("ReadyToFault4", oReadyState, oFaultedState, oAirOffEvent, null, null);

                // Transitions for IdleState 
                IsGlassLoadedToGlassLoadedTransition = new NSFExternalTransition("IsGlassLoadedToGlassLoadedChoice", oIsGlassLoadedChoiceState, oGlassLoadedState, null, IsGlassLoaded, EnableStartAndUnLoadButtonOnGui);
                IsGlassLoadedToIsCartDockedTransition = new NSFExternalTransition("IsGlassLoadedToIsCartDocked", oIsGlassLoadedChoiceState, oIsCartDockedChoiceState, null, Else, null);
                IsCartDockedToCartDockedTransition = new NSFExternalTransition("IsCartDockedToCartDocked", oIsCartDockedChoiceState, oCartDockedState, null, IsCartDocked, null);
                IsCartDockedToGlassUnloadedTransition = new NSFExternalTransition("IsCartDockedToGlassUnloaded", oIsCartDockedChoiceState, oGlassUnLoadedState, null, Else, null);
                GlassLoadedToGlassUnLoadingTransition = new NSFExternalTransition("GlassLoadedToGlassUnLoading", oGlassLoadedState, oGlassUnLoadingState, oUnLoadGlassEvent, null, null);
                GlassLoadedToFaultWithGlassHeldTransition = new NSFExternalTransition("GlassLoadedToFaultWithGlassHeld", oGlassLoadedState, oFaultWithGlassHeldState, oEStopEvent, null, null);
                GlassLoadedToFaultWithGlassHeldTransition2 = new NSFExternalTransition("GlassLoadedToFaultWithGlassHeld2", oGlassLoadedState, oFaultWithGlassHeldState, oFanOffEvent, null, null);
                GlassLoadedToFaultWithGlassHeldTransition3 = new NSFExternalTransition("GlassLoadedToFaultWithGlassHeld3", oGlassLoadedState, oFaultWithGlassHeldState, oCabinetOverTempEvent, null, null);
                GlassLoadedToFaultWithGlassHeldTransition4 = new NSFExternalTransition("GlassLoadedToFaultWithGlassHeld4", oGlassLoadedState, oFaultWithGlassHeldState, oPanelOpenEvent, null, null);
                GlassLoadedToFaultWithGlassHeldTransition5 = new NSFExternalTransition("GlassLoadedToFaultWithGlassHeld5", oGlassLoadedState, oFaultWithGlassHeldState, oVacuumOffEvent, null, null);
                GlassLoadedToFaultWithGlassHeldTransition6 = new NSFExternalTransition("GlassLoadedToFaultWithGlassHeld6", oGlassLoadedState, oFaultWithGlassHeldState, oMotionFaultEvent, null, null);
                GlassLoadedToFaultWithGlassHeldTransition7 = new NSFExternalTransition("GlassLoadedToFaultWithGlassHeld7", oGlassLoadedState, oFaultWithGlassHeldState, oByPassSwitchOnEvent, IsNotAdminPrivilege, null);
                GlassLoadedToFaultWithGlassHeldTransition8 = new NSFExternalTransition("GlassLoadedToFaultWithGlassHeld8", oGlassLoadedState, oFaultWithGlassHeldState, oScanSensorFaultEvent, null, NotifyScanSensorAlarm);
                GlassLoadedToFaultWithGlassHeldTransition9 = new NSFExternalTransition("GlassLoadedToFaultWithGlassHeld9", oGlassLoadedState, oFaultWithGlassHeldState, oInterlocksFaultEvent, null, null);
                GlassLoadedToFaultWithGlassLostTransition = new NSFExternalTransition("GlassLoadedToFaultWithGlassLost", oGlassLoadedState, oFaultWithGlassLostState, oGlassNotDetectedEvent, null, null);

                // Transitions for InspectingState 
                GlassLoadedToIsWarningOrCriticalAlarm3ChoiceTransition = new NSFExternalTransition("GlassLoadedToIsWarningOrCriticalAlarm3Choice", oGlassLoadedState, oIsWarningOrCriticalAlarm3ChoiceState, oStartScanEvent, null, null);
                IsWarningOrCriticalAlarm3ChoiceToFaultWithGlassHeldTransition = new NSFExternalTransition("IsWarningOrCriticalAlarm3ChoiceToFaultWithGlassHeld", oIsWarningOrCriticalAlarm3ChoiceState, oFaultWithGlassHeldState, null, IsCriticalAlarms, null);
                IsWarningOrCriticalAlarm3ChoiceToIsScanSensorReadyChoiceTransition = new NSFExternalTransition("IsWarningOrCriticalAlarm3ChoiceToIsScanSensorReadyChoice", oIsWarningOrCriticalAlarm3ChoiceState, oIsScanSensorReadyChoiceState, null, Else, null);
                IsScanSensorReadyChoiceToIsWindowClosedChoiceTransition = new NSFExternalTransition("IsScanSensorReadyChoiceToIsWindowClosedChoice", oIsScanSensorReadyChoiceState, oIsWindowClosedChoiceState, null, IsSensorReady, null);
                IsWindowClosedChoiceToInspectingTransition = new NSFExternalTransition("IsWindowClosedChoiceToInspecting", oIsWindowClosedChoiceState, oInspectingState, null, IsWindowClosed, null);
                IsWindowClosedChoiceToWaitForWindowClosedTransition = new NSFExternalTransition("IsWindowClosedChoiceToWaitForWindowClosed", oIsWindowClosedChoiceState, oWaitForWindowClosedState, null, Else, null);
                WaitForWindowClosedToInspectingTransition = new NSFExternalTransition("WaitForWindowClosedToInspecting", oWaitForWindowClosedState, oInspectingState, oWindowClosedEvent, null, null);
                IsScanSensorReadyChoiceToFaultWithGlassHeldTransition = new NSFExternalTransition("IsScanSensorReadyChoiceToFaultWithGlassHeld", oIsScanSensorReadyChoiceState, oFaultWithGlassHeldState, null, Else, null);
                InspectingInitialToMacroScan = new NSFExternalTransition("InspectingInitialToMacroScan", oInspectingInitialState, oMacroScanState, null, null, null);
                StripeFeatureVectorCompleteInternalTransition = new NSFInternalTransition("ScanningToReadyViaScanProgressEvent", oInspectingState, oStripeFeatureVectorCompleteEvent, null, HandleStripeFeatureVectorComplete);
                ProcessingToDefectReviewTransition = new NSFExternalTransition("ProcessingToDefectReview", oProcessingState, oDefectReviewState, oSheetJudgementCompleteEvent, null, StopSystemQueueCountLoggingTimer);
                ProcessingToGlassLoadedTransition = new NSFExternalTransition("ProcessingToGlassLoaded", oProcessingState, oGlassLoadedState, oAbortEvent, null, StopSystemQueueCountLoggingTimer);
                InspectingToFaultWithGlassHeldTransition = new NSFExternalTransition("InspectingToFaultWithGlassHeld", oInspectingState, oFaultWithGlassHeldState, oEStopEvent, null, null);
                InspectingToFaultWithGlassHeldTransition2 = new NSFExternalTransition("InspectingToFaultWithGlassHeld2", oInspectingState, oFaultWithGlassHeldState, oScanSensorFaultEvent, null, NotifyScanSensorAlarm);
                InspectingToFaultWithGlassHeldTransition3 = new NSFExternalTransition("InspectingToFaultWithGlassHeld3", oInspectingState, oFaultWithGlassHeldState, oMotionFaultEvent, null, NotifyMotionAlarm);
                InspectingToFaultWithGlassHeldTransition4 = new NSFExternalTransition("InspectingToFaultWithGlassHeld4", oInspectingState, oFaultWithGlassHeldState, oCabinetOverTempEvent, null, null);
                InspectingToFaultWithGlassHeldTransition5 = new NSFExternalTransition("InspectingToFaultWithGlassHeld5", oInspectingState, oFaultWithGlassHeldState, oVacuumOffEvent, null, null);
                InspectingToFaultWithGlassHeldTransition6 = new NSFExternalTransition("InspectingToFaultWithGlassHeld6", oInspectingState, oFaultWithGlassHeldState, oPanelOpenEvent, null, null);
                InspectingToFaultWithGlassHeldTransition7 = new NSFExternalTransition("InspectingToFaultWithGlassHeld7", oInspectingState, oFaultWithGlassHeldState, oByPassSwitchOnEvent, IsNotAdminPrivilege, null);
                InspectingToFaultWithGlassHeldTransition8 = new NSFExternalTransition("InspectingToFaultWithGlassHeld8", oInspectingState, oFaultWithGlassHeldState, oInterlocksFaultEvent, null, null);
                MacroScanToAbortingTransition = new NSFExternalTransition("MacroScanToAborting", oMacroScanState, oAbortingState, oAbortEvent, null, StopSystemQueueCountLoggingTimer);
                AbortingToIdleTransition = new NSFExternalTransition("AbortingToIdle", oAbortingState, oIdleState, oAbortCompleteEvent, null, null);
                AbortingToFaultWithGlassHeldTransition = new NSFExternalTransition("AbortingToFaultWithGlassHeld", oAbortingState, oFaultWithGlassHeldState, oScanSensorFaultEvent, null, null);
                AbortingToFaultWithGlassHeld2Transition = new NSFExternalTransition("AbortingToFaultWithGlassHeld2", oAbortingState, oFaultWithGlassHeldState, oAbortEvent, null, null);
                ImagesSaveToGlassLoadedTransition2 = new NSFExternalTransition("ImagesSaveToGlassLoaded", oImagesSaveState, oGlassLoadedState, oAbortEvent, null, StopImageSave);

                // Transitions for MacroScanState 
                MacroScanInitialToLaserAndPmtOnTransition = new NSFExternalTransition("MacroScanInitialToIsMoreStripes", oMacroScanInitialState, oLaserAndPmtOnState, null, null, null);
                LaserAndPmtOnToIsMoreStripesTransition = new NSFExternalTransition("LaserAndPmtOnToIsMoreStripes", oLaserAndPmtOnState, oIsMoreStripesChoiceState, oLaserAndPmtOnEvent, null, null);
                IsMoreStripesToMoveToStripeTransition = new NSFExternalTransition("IsMoreStripesToMoveToStripe", oIsMoreStripesChoiceState, oMoveToStripeState, null, IsMoreStripes, null);
                IsMoreStripesToProcessingTransition = new NSFExternalTransition("IsMoreStripesToProcessing", oIsMoreStripesChoiceState, oProcessingState, null, Else, null);
                MoveToStripeToScanningTransition = new NSFExternalTransition("MoveToStripeToScanning", oMoveToStripeState, oScanningState, oStartScanReadyEvent, null, null);
                ScanningToIsMoreStripesTransition = new NSFExternalTransition("ScanningToIsMoreStripes", oScanningState, oIsMoreStripesChoiceState, oScanCompleteEvent, null, null);
                ScanProgessEventToReadyStateTransition = new NSFInternalTransition("ScanningToReadyViaScanProgressEvent", oReadyState, oScanProgressEvent, null, HandleScanProgessEventToReadyState);
                ZetaStateChangeEventToReadyStateTransition = new NSFInternalTransition("ScanningToReadyViaZetaStateChangeEvent", oReadyState, oScanSensorStateChangeEvent, null, HandleZetaStateChangeEventToReadyState);
                MacroScanToFaultWithGlassHeldTransition = new NSFExternalTransition("MacroScanToFaultWithGlassHeld", oMacroScanState, oFaultWithGlassHeldState, oWindowNotClosedEvent, IsNotAdminPrivilege, null);
                MacroScanToFaultWithGlassHeldTransition = new NSFExternalTransition("MacroScanToFaultWithGlassHeld", oMacroScanState, oFaultWithGlassHeldState, oDoorNotClosedEvent, null, null);
                MacroScanToFaultWithGlassHeldTransition = new NSFExternalTransition("MacroScanToFaultWithGlassHeld", oMacroScanState, oFaultWithGlassHeldState, oLightOnEvent, IsNotAdminPrivilege, null);

                // Transitions for DefectReviewState 
                RevisitToSaveNGImagesChoiceTransition = new NSFExternalTransition("RevisitToGlassLoaded", oRevisitState, oSaveNGImagesChoiceState, oCommitEvent, null, null);
                SaveNGImagesChoiceToImagesSaveTransition = new NSFExternalTransition("SaveNGImagesChoiceToImagesSave", oSaveNGImagesChoiceState, oImagesSaveState, null, IsSaveNGImages, null);
                SaveNGImagesChoiceToGlassLoadedTransition = new NSFExternalTransition("SaveNGImagesChoiceToGlassLoaded", oSaveNGImagesChoiceState, oGlassLoadedState, null, Else, EnableStartAndUnLoadButtonOnGui);
                ImagesSaveToGlassLoadedTransition = new NSFExternalTransition("ImagesSaveToGlassLoaded", oImagesSaveState, oGlassLoadedState, oImagesSavedEvent, null, EnableStartAndUnLoadButtonOnGui);
                DefectReviewInitialToRevisitTransition = new NSFExternalTransition("DefectReviewInitialToRevisit", oDefectReviewInitialState, oRevisitState, null, null, null);
                RevisitStateToGlassUnLoadingTransition = new NSFExternalTransition("RevisitStateToGlassUnLoading", oRevisitState, oGlassUnLoadingState, oUnLoadGlassEvent, null, null);
                RevisitToIsWarningOrCriticalAlarm3ChoiceTransition = new NSFExternalTransition("RevisitToIsWarningOrCriticalAlarm3", oRevisitState, oIsWarningOrCriticalAlarm3ChoiceState, oStartScanEvent, null, null);
                IdleToViewOfflineTransition = new NSFExternalTransition("IdleToViewOffline", oIdleState, oViewOfflineState, oLoadOfflineFileEvent, null, LoadOfflineFile);
                ViewOfflineToIsGlassLoadedTransition = new NSFExternalTransition("ViewOfflineToIsGlassLoaded", oViewOfflineState, oIsGlassLoadedChoiceState, oCommitEvent, null, null);
                RevisitInternalNgImageSavedTransition = new NSFInternalTransition("RevisitInternalImage", oRevisitState, oNgImagesSavedEvent, null, DisableAbortButtonOnGui);
                RevisitInternalAbortTransition = new NSFInternalTransition("RevisitInternalAbort", oRevisitState, oAbortEvent, null, StopImageSave);

                // Transitions for FaultedState 
                FaultedInitialToIsSheetDetected2ChoiceTransition = new NSFExternalTransition("FaultedInitialToIsSheetDetected2Choice", oFaultedInitialState, oIsSheetDetected2ChoiceState, null, null, null);
                IsSheetDetectedChoiceToFaultWithGlassLostTransition = new NSFExternalTransition("IsSheetDetectedChoiceToFaultWithGlassLost", oIsSheetDetected2ChoiceState, oFaultWithGlassLostState, null, IsGlassDetected, null);
                IsSheetDetectedChoiceToFaultWithNoGlassTransition = new NSFExternalTransition("IsSheetDetectedChoiceToFaultWithNoGlass", oIsSheetDetected2ChoiceState, oFaultWithNoGlassState, null, Else, null);
                FaultWithNoGlassToIsWarningOrCriticalAlarmChoiceTransition = new NSFExternalTransition("FaultWithNoGlassToIsWarningOrCriticalAlarmChoice", oFaultWithNoGlassState, oIsWarningOrCriticalAlarmChoiceState, oClearFaultsEvent, IsNoCriticalAlarms, null);
                FaultAndOperatorClearsGlassToIsWarningOrCriticalAlarmChoiceTransition = new NSFExternalTransition("FaultAndOperatorClearsGlassToIsWarningOrCriticalAlarmChoice", oFaultAndOperatorClearsGlassState, oIsWarningOrCriticalAlarmChoiceState, oClearFaultsEvent, IsNoCriticalAlarms, null);
                FaultWithGlassLostToFaultAndOperatorClearsGlassTransition = new NSFExternalTransition("FaultWithGlassLostToIsFaultedChoice", oFaultWithGlassLostState, oFaultAndOperatorClearsGlassState, oClearFaultsEvent, null, DisableGlassNotDetectedAlarm);
                FaultWithGlassHeldToFaultWithGlassLostTransition = new NSFExternalTransition("FaultWithGlassHeldToFaultWithGlassLost", oFaultWithGlassHeldState, oFaultWithGlassLostState, oGlassNotDetectedEvent, null, null);
                FaultWithGlassHeldToGlassLoadedTransition = new NSFExternalTransition("FaultWithGlassHeldToGlassLoaded", oFaultWithGlassHeldState, oGlassLoadedState, oClearFaultsEvent, IsNoCriticalAlarms, EnableUnLoadButtonOnGui);

                // Transitions for GlassLoadingState
                GlassLoadingInitialToMoveToLoadPosTransition = new NSFExternalTransition("GlassLoadingInitialToMoveToLoadPos", oGlassLoadingInitialState, oMoveToLoadPosState, null, null, null);
                MoveToLoadPosToIsDoorOpenedChoiceTransition = new NSFExternalTransition("MoveToLoadPosToIsDoorOpenedChoice", oMoveToLoadPosState, oIsDoorOpenedChoiceState, oLoadPosMoveCompleteEvent, null, null);
                MoveToLoadPosToIdleTransition = new NSFExternalTransition("MoveToLoadPosToIdle", oMoveToLoadPosState, oIdleState, oCartUnDockedEvent, null, null);
                IsDoorOpenedChoiceToWaitForDoorOpenToLoadTransition = new NSFExternalTransition("IsDoorOpenedChoiceToWaitForDoorOpenToLoad", oIsDoorOpenedChoiceState, oWaitForDoorOpenToLoadState, null, Else, null);
                IsDoorOpenedChoiceToIsAutoLoadChoiceTransition = new NSFExternalTransition("IsDoorOpenedChoiceToIsAutoLoadChoice", oIsDoorOpenedChoiceState, oIsAutoLoadChoiceState, null, IsDoorOpen, null);
                WaitForDoorOpenToLoadToIdleTransition = new NSFExternalTransition("WaitForDoorOpenToLoadToIdle", oWaitForDoorOpenToLoadState, oIdleState, oCartUnDockedEvent, null, null);
                WaitForDoorOpenToLoadToIdleTransition2 = new NSFExternalTransition("WaitForDoorOpenToLoadToIdle", oWaitForDoorOpenToLoadState, oIdleState, oAbortEvent, null, null);
                WaitForDoorOpenToLoadToIsAutoLoadChoiceTransition = new NSFExternalTransition("WaitForDoorOpenToLoadToIsAutoLoadChoice", oWaitForDoorOpenToLoadState, oIsAutoLoadChoiceState, oDoorOpenedEvent, null, null);
                IsAutoLoadChoiceToWaitForGlassToEnterTransition = new NSFExternalTransition("IsAutoLoadChoiceToWaitForGlassToEnter", oIsAutoLoadChoiceState, oWaitForGlassToEnterState, null, IsAutoLoad, null);
                IsAutoLoadChoiceToOperatorManualLoadTransition = new NSFExternalTransition("IsAutoLoadChoiceToOperatorManualLoad", oIsAutoLoadChoiceState, oOperatorManualLoadState, null, Else, null);
                WaitForGlassToEnterToIdleTransition = new NSFExternalTransition("WaitForGlassToEnterToIdle", oWaitForGlassToEnterState, oIdleState, oCartUnDockedEvent, null, null);
                WaitForGlassToEnterToIdleTransition2 = new NSFExternalTransition("WaitForGlassToEnterToIdle", oWaitForGlassToEnterState, oIdleState, oAbortEvent, null, null);
                WaitForGlassToEnterToGlassLoadingFromCartTransition = new NSFExternalTransition("WaitForGlassToEnterToGlassLoadingFromCart", oWaitForGlassToEnterState, oGlassLoadingFromCartState, oGlassDetectedAtEntranceEvent, null, EnableCartUnDockedAlarm);
                GlassLoadingFromCartToAutoLoadSeqTransition = new NSFExternalTransition("GlassLoadingFromCartToAutoLoadSeq", oGlassLoadingFromCartState, oAutoLoadSeqState, oCartLoadTimeoutEvent, null, null);
                AutoLoadSeqGlassNotDetectedInternalTransition = new NSFInternalTransition("AutoLoadSeqGlassNotDetectedInternalTransition", oAutoLoadSeqState, oGlassNotDetectedEvent, null, null);
                AutoLoadSeqToOperatorManualLoadTransition = new NSFExternalTransition("AutoLoadSeqToOperatorManualLoad", oAutoLoadSeqState, oOperatorManualLoadState, oAutoLoadFailedEvent, null, null);
                AutoLoadSeqToIsSafetyCircuitResetTransition = new NSFExternalTransition("AutoLoadSeqToIsSafetyCircuitReset", oAutoLoadSeqState, oIsSafetyCircuitResetChoiceState, oAutoLoadCompleteEvent, null, null);
                AutoLoadSeqToFaultWithGlassLostTransition = new NSFExternalTransition("AutoLoadSeqToFaultWithGlassLost", oAutoLoadSeqState, oFaultWithGlassLostState, oAutoLoadTimeOutEvent, null, null);
                OperatorManualLoadToIsSafetyCircuitResetTransition = new NSFExternalTransition("OperatorManualLoadToIsSafetyCircuitReset", oOperatorManualLoadState, oIsSafetyCircuitResetChoiceState, oGlassDetectedAtDamperEvent, null, null);
                GlassLoadingFromCartToIsSafetyCircuitResetChoiceTransition = new NSFExternalTransition("GlassLoadingFromCartToIsSafetyCircuitResetChoice", oGlassLoadingFromCartState, oIsSafetyCircuitResetChoiceState, oGlassDetectedAtDamperEvent, null, null);
                IsSafetyCircuitResetToIsDoorOpened2Transition = new NSFExternalTransition("IsSafetyCircuitResetToIsDoorOpened2", oIsSafetyCircuitResetChoiceState, oIsDoorOpened2ChoiceState, null, IsSafetyCircuitReset, null);
                IsSafetyCircuitResetToResetTransition = new NSFExternalTransition("IsSafetyCircuitResetToReset", oIsSafetyCircuitResetChoiceState, oResetState, null, Else, null);
                ResetToIsDoorOpened2Transition = new NSFExternalTransition("ResetToIsDoorOpened2", oResetState, oIsDoorOpened2ChoiceState, oSafetyCircuitResetEvent, null, null);
                IsDoorOpened2ChoiceToAlignGlassTransition = new NSFExternalTransition("IsDoorOpened2ChoiceToAlignGlass", oIsDoorOpened2ChoiceState, oAlignGlassState, null, IsDoorClosed, DisableCartUnDockedAlarm);
                IsDoorOpened2ChoiceToCloseDoorToAlignTransition = new NSFExternalTransition("IsDoorOpened2ChoiceToCloseDoorToAlign", oIsDoorOpened2ChoiceState, oCloseDoorToAlignState, null, Else, null);
                CloseDoorToAlignToAlignGlassTransition = new NSFExternalTransition("CloseDoorToAlignToAlignGlass", oCloseDoorToAlignState, oAlignGlassState, oDoorCloseEvent, null, DisableCartUnDockedAlarm);
                GlassLoadingToFaultWithGlassLostTransition = new NSFExternalTransition("GlassLoadingToFaultWithGlassLost", oGlassLoadingState, oFaultWithGlassLostState, oAbortEvent, null, NotifytLoadGlassAbortAlarm);
                GlassLoadingGlassNotDetectedInternalTransition = new NSFInternalTransition("GlassLoadingGlassNotDetectedInternalTransition", oGlassLoadingState, oGlassNotDetectedEvent, null, null);

                // Transitions for AlignGlassState
                AlignGlassInitialToIsValidGlassSizeTransition = new NSFExternalTransition("AlignGlassInitialToIsValidGlassSize", oAlignGlassInitialState, oIsValidGlassSizeChoiceState, null, null, null);
                IsValidGlassSizeToDeployTrailingEdgePinsTransition = new NSFExternalTransition("IsValidGlassSizeToDeployTrailingEdgePins", oIsValidGlassSizeChoiceState, oDeployTrailingEdgePinsState, null, IsValidGlassSize, null);
                IsValidGlassSizeToCorrectGlassSizeTransition = new NSFExternalTransition("IsValidGlassSizeToCorrectGlassSize", oIsValidGlassSizeChoiceState, oCorrectGlassSizeState, null, Else, null);
                CorrectGlassSizeToIsValidGlassSizeTransition = new NSFExternalTransition("CorrectGlassSizeToIsValidGlassSize", oCorrectGlassSizeState, oIsValidGlassSizeChoiceState, oSizeCorrectedEvent, null, null);
                CorrectGlassSizeToFaultWithGlassLostTransition = new NSFExternalTransition("CorrectGlassSizeToFaultWithGlassLost", oCorrectGlassSizeState, oFaultWithGlassLostState, oAbortEvent, null, NotifyInvalidGlassSizeAlarm);
                DeployTrailingEdgePinsToAutoAlignGlassTransition = new NSFExternalTransition("DeployTrailingEdgePinsToAutoAlignGlass", oDeployTrailingEdgePinsState, oAutoAlignGlassState, oTrailingEdgePinsExtendedEvent, null, null);
                AutoAlignGlassInitialToIsNTimesCompleteTransition = new NSFExternalTransition("AutoAlignGlassInitialToIsNTimesComplete", oAutoAlignGlassInitialState, oIsNTimesCompleteChoiceState, null, null, null);
                IsNTimesCompleteChoiceToClampSheetTransition = new NSFExternalTransition("IsNTimesCompleteChoiceToClampSheet", oIsNTimesCompleteChoiceState, oClampSheetState, null, IsNTimesComplete, null);
                IsNTimesCompleteChoiceToStepInSideTransition = new NSFExternalTransition("IsNTimesCompleteChoiceToStepInSide", oIsNTimesCompleteChoiceState, oStepInSideState, null, Else, null);
                StepinSideToStepInLeadingEdgeTransition = new NSFExternalTransition("StepinSideToStepInLeadingEdge", oStepInSideState, oStepInleadingEdgeState, oMoveCompleteEvent, null, null);
                StepInLeadingEdgeToDelayForSpringTransition = new NSFExternalTransition("StepInLeadingEdgeToDelayForSpring", oStepInleadingEdgeState, oDelayForSpringState, oMoveCompleteEvent, null, null);
                DelayForSpringStateToIsAnyPreLoadCompressedChoiceTransition = new NSFExternalTransition("DelayForSpringStateToIsAnyPreLoadCompressed", oDelayForSpringState, oIsAnyPreLoadCompressedChoiceState, oDelayForSpringTimeoutEvent, null, null);
                IsAnyPreLoadCompressedToFaultWithGlassLostTransition = new NSFExternalTransition("IsAnyPreLoadCompressedToFaultWithGlassLost", oIsAnyPreLoadCompressedChoiceState, oFaultWithGlassLostState, null, IsAnyPreLoadCompressed, null);
                IsAnyPreLoadCompressedToIsNTimesCompleteChoiceTransition = new NSFExternalTransition("IsAnyPreLoadCompressedToIsNTimesCompleteChoice", oIsAnyPreLoadCompressedChoiceState, oIsNTimesCompleteChoiceState, null, Else, null);
                IsWarningOrCriticalAlarm2ChoiceToFaultWithGlassHeldTransition = new NSFExternalTransition("IsWarningOrCriticalAlarm2ChoiceToFaultWithGlassHeld", oIsWarningOrCriticalAlarm2ChoiceState, oFaultWithGlassHeldState, null, IsCriticalAlarms, null);
                IsWarningOrCriticalAlarm2ChoiceToGlassLoadedTransition = new NSFExternalTransition("IsWarningOrCriticalAlarm2ChoiceToGlassLoaded", oIsWarningOrCriticalAlarm2ChoiceState, oGlassLoadedState, null, Else, null);
                AlignGlassToFaultWithGlassLostTransition = new NSFExternalTransition("AlignGlassStateToFaultWithGlassLost", oAlignGlassState, oFaultWithGlassLostState, oDoorNotClosedEvent, null, null);
                ClampSheetToIsWarningOrCriticalAlarmTransition = new NSFExternalTransition("ClampSheetToIsWarningOrCriticalAlarm", oClampSheetState, oIsWarningOrCriticalAlarm2ChoiceState, oMoveCompleteEvent, null, null);

                // Transitions for GlassUnloadingState
                GlassUnLoadingInitialToPrepForUnloadTransition = new NSFExternalTransition("GlassUnLoadingInitialToPrepForUnload", oGlassUnLoadingInitialState, oPrepForUnloadState, null, null, null);
                PrepForUnloadInitialToIsCartDocked2Transition = new NSFExternalTransition("PrepForUnloadInitialToIsCartDocked2", oPrepForUnloadInitialState, oIsCartDocked2ChoiceState, null, null, null);
                PrepForUnloadToIdleTransition = new NSFExternalTransition("PrepForUnloadToIdle", oPrepForUnloadState, oIdleState, oAbortEvent, null, null);
                PrepForUnloadToFaultWithGlassHeldTransition = new NSFExternalTransition("PrepForUnloadToFaultWithGlassHeld", oPrepForUnloadState, oFaultWithGlassHeldState, oMotionFaultEvent, null, null);
                PrepForUnloadDoorNotOpenInternalTransition = new NSFInternalTransition("PrepForUnloadDoorNotOpenInternal", oPrepForUnloadState, oDoorNotOpenEvent, null, null);
                IsCartDocked2ChoiceToIsDoorOpened3ChoiceTransition = new NSFExternalTransition("oIsCartDocked2ChoiceToOpenDoor", oIsCartDocked2ChoiceState, oIsDoorOpened3ChoiceState, null, IsCartDocked, null);
                IsCartDocked2ChoiceToPromptToDockTransition = new NSFExternalTransition("IsCartDocked2ChoiceToPromptToDock", oIsCartDocked2ChoiceState, oPromptToDockCartState, null, Else, null);
                PromptToDockCartToIsDoorOpen3Transition = new NSFExternalTransition("PromptToDockCartToIsDoorOpen3", oPromptToDockCartState, oIsDoorOpened3ChoiceState, oCartDockedEvent, null, null);
                IsDoorOpened3ChoiceToMoveXToUnloadPosTransition = new NSFExternalTransition("IsDoorOpened3ChoiceToMoveXToUnloadPos", oIsDoorOpened3ChoiceState, oMoveXToUnloadPosState, null, IsDoorOpen, null);
                IsDoorOpened3ChoiceToOpenDoorTransition = new NSFExternalTransition("IsDoorOpened3ChoiceToOpenDoor", oIsDoorOpened3ChoiceState, oOpenDoorState, null, Else, null);
                OpenDoorToMoveXToUnloadPosTransition = new NSFExternalTransition("OpenDoorToMoveXToUnloadPos", oOpenDoorState, oMoveXToUnloadPosState, oDoorOpenedEvent, null, null);
                MoveXToUnloadPosToUnCompressSidePreLoadTransition = new NSFExternalTransition("MoveXToUnloadPosToUnCompressSidePreLoad", oMoveXToUnloadPosState, oUnCompressSidePreLoadState, oMoveCompleteEvent, null, null);
                UnCompressSidePreLoadToUnCompressTrailingEdgePreLoadsTransition = new NSFExternalTransition("UnCompressSidePreLoadToUnCompressTrailingEdgePreLoads", oUnCompressSidePreLoadState, oUnCompressTrailingEdgePreloadsState, oMoveCompleteEvent, null, null);
                UnCompressTrailingEdgePreLoadsToWaitForPreloadsUnCompressedTransition = new NSFExternalTransition("UnCompressTrailingEdgePreLoadsToWaitForPreloadsUnCompressed", oUnCompressTrailingEdgePreloadsState, oWaitForPreloadsUnCompressedState, oMoveCompleteEvent, null, null);
                WaitForPreloadsUnCompressedToRetractTrailingEdgePinsTransition = new NSFExternalTransition("WaitForPreloadsUnCompressedToRetractTrailingEdgePins", oWaitForPreloadsUnCompressedState, oRetractTrailingEdgePinsState, oAllPreloadsUnCompressedEvent, null, null);
                WaitForPreloadsUnCompressedToFaultWithGlassLostTransition = new NSFExternalTransition("WaitForPreloadsUnCompressedToFaultWithGlassLost", oWaitForPreloadsUnCompressedState, oFaultWithGlassLostState, oWaitForPreloadsUncompressedTimeoutEvent, null, null);
                RetractTrailingEdgePinsToFaultWithGlassLostTransition = new NSFExternalTransition("RetractTrailingEdgePinsToFaultWithGlassLost", oRetractTrailingEdgePinsState, oFaultWithGlassLostState, oRetractTrailingEdgePinsTimeOutEvent, null, NotifyUnableToRetractTEPinsAlarm);
                RetractTrailingEdgePinsToMoveToWalkGlassOutStartPosTransition = new NSFExternalTransition("RetractTrailingEdgePinsToMoveToWalkGlassOutStartPos", oRetractTrailingEdgePinsState, oMoveXToWalkGlassOutStartPosState, oTrailingEdgePinsRetractedEvent, null, StopRetractTrailingEdgePinsTimer);
                MoveToWalkGlassOutStartPosToMAStageExtendTransition = new NSFExternalTransition("MoveToWalkGlassOutStartPosToMAStageExtend", oMoveXToWalkGlassOutStartPosState, oMAStageExtendState, oMoveCompleteEvent, null, null);
                MAStageExtendToWalkGlassOutTransition = new NSFExternalTransition("MAStageExtendToWalkGlassOut", oMAStageExtendState, oWalkGlassOutState, oMoveCompleteEvent, null, null);
                WalkGlassOutToMAStageRetractTransition = new NSFExternalTransition("WalkGlassOutToMAStageRetract", oWalkGlassOutState, oMAStageRetractState, oMoveCompleteEvent, null, null);
                MAStageRetractToRemoveGlassTransition = new NSFExternalTransition("MAStageRetract2ToRemoveGlass", oMAStageRetractState, oRemoveGlassState, oMoveCompleteEvent, null, null);
                RemoveGlassToIdleTransition = new NSFExternalTransition("RemoveGlassToIdle", oRemoveGlassState, oIdleState, oGlassRemovedAtEntranceEvent, null, MoveAllToLoadPosAndEnableGlassSizeEntry);
                GlassUnLoadingToFaultWithGlassLostTransition = new NSFExternalTransition("GlassUnLoadingToFaultWithGlassLost", oGlassUnLoadingState, oFaultWithGlassLostState, oAbortEvent, null, NotifyUnLoadGlassAbortAlarm);
                GlassUnLoadingToFaultWithGlassLostTransition2 = new NSFExternalTransition("GlassUnloadingToFaultWithGlassLost2", oGlassUnLoadingState, oFaultWithGlassLostState, oDoorNotOpenEvent, null, null);
                GlassUnloadingGlassNotDetectedInternalTransition = new NSFInternalTransition("GlassUnloadingGlassNotDetectedInternal", oGlassUnLoadingState, oGlassNotDetectedEvent, null, null);

                // Transitions for GlassUnLoadedState 
                GlassUnLoadedInitialToCartUnDockedTransition = new NSFExternalTransition("GlassUnLoadedInitialToCartUnDocked", oGlassUnLoadedInitialState, oCartUnDockedState, null, null, null);
                CartUnDockedToCartDockedTransition = new NSFExternalTransition("CartUnDockedToCartDocked", oCartUnDockedState, oCartDockedState, oCartDockedEvent, IsDoorOpen, EnableLoadGlassOnGui);
                CartUnDockedToCartDockedTransition2 = new NSFExternalTransition("CartUnDockedToCartDocked2", oCartUnDockedState, oCartDockedState, oCartDockedEvent, IsDoorNotOpen, null);
                CartDockedToCartUnDockedTransition = new NSFExternalTransition("CartDockedToCartUnDocked", oCartDockedState, oCartUnDockedState, oCartUnDockedEvent, null, null);
                CartDockedToGlassLoadingTransition = new NSFExternalTransition("CartDockedToGlassLoading", oCartDockedState, oGlassLoadingState, oLoadGlassEvent, null, null);
                DoorOpenInternalTransition = new NSFInternalTransition("DoorOpenInternalTransition", oCartDockedState, oDoorOpenedEvent, null, EnableLoadGlassOnGui);
                DoorCloseInternalTransition = new NSFInternalTransition("DoorCloseInternalTransition", oCartDockedState, oDoorCloseEvent, null, DisableLoadGlassOnGui);

                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Unable to initiate the Guide State Machine!", ex);
            }

            return false;
        }

        private void CreateOperatorPrompts()
        {
            m_odictOperatorPromptForState = new Dictionary<NSFState, string>();

            m_odictOperatorPromptForState.Add(oWaitForDoorOpenToLoadState, "Open Mail Slot Door to Proceed");
            m_odictOperatorPromptForState.Add(oWaitForGlassToEnterState, "Start the Glass Loading Into Mail Slot to Proceed");
            m_odictOperatorPromptForState.Add(oOperatorManualLoadState, "Manually Load the Glass Into the Mail Slot");
            m_odictOperatorPromptForState.Add(oCloseDoorToAlignState, "Close the Mail Slot Door to Proceed");
            m_odictOperatorPromptForState.Add(oCorrectGlassSizeState, "Correct the Glass Size on the Setup Screen or Hit Abort Button to Abort Glass Load.");
            m_odictOperatorPromptForState.Add(oWaitForWindowClosedState, "Close the Viewing Window to Proceed");
            m_odictOperatorPromptForState.Add(oOpenDoorState, "Open the Mail Slot Door to Proceed");
            m_odictOperatorPromptForState.Add(oRemoveGlassState, "Remove the Glass and then Close Door to Proceed");
            m_odictOperatorPromptForState.Add(oFaultWithGlassLostState, "Clear Glass From System and then Hit ClearFaults to Proceed");
            m_odictOperatorPromptForState.Add(oFaultAndOperatorClearsGlassState, "System may Move Motion Axis to Reset.  Hit ClearFaults to Proceed.");
            m_odictOperatorPromptForState.Add(oFaultWithNoGlassState, "Use the Alarm Screen to Understand and Clear the Alarm.  Then Hit ClearFaults to Proceed.");
            m_odictOperatorPromptForState.Add(oFaultWithGlassHeldState, "Use the Alarm Screen to Understand and Clear the Alarm.  Then Hit ClearFaults to Proceed.");
            m_odictOperatorPromptForState.Add(oCartUnDockedState, "Dock the Cart to Proceed.");
            m_odictOperatorPromptForState.Add(oCartDockedState, "Make Sure the Mail Slot is Open and then Hit the Load Glass Button on the Toolbar to Proceed.");
            m_odictOperatorPromptForState.Add(oGlassLoadedState, "Hit the Start Button to Start the Inspection or the UnLoad Button to UnLoad the Sheet.  The Start Button will enable when the sensor is ready.");
            m_odictOperatorPromptForState.Add(oResetState, "Press the Green System Reset Button to Proceed.");
            m_odictOperatorPromptForState.Add(oPromptToDockCartState, "Dock the Cart to Proceed.");
            m_odictOperatorPromptForState.Add(oAbortingState, "Wait for the Sensor to Complete the Abort Process.");
            m_odictOperatorPromptForState.Add(oProcessingState, "Wait for the Feature Vector Processing to Complete.");
            m_odictOperatorPromptForState.Add(oRevisitState, "Hit the Commit button to Complete the Revisit and Save Results.");
            m_odictOperatorPromptForState.Add(oImagesSaveState, "Wait for the NG Images to be Saved to Disk.  Hit Abort to Stop Saving NG Images and Proceed.");
            m_odictOperatorPromptForState.Add(oViewOfflineState, "Hit the Commit button to Complete Offline Viewing.");
        }

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
            NSFTraceLog.PrimaryTraceLog.saveLog("GuideStateMachineLog.xml");

            NSFEnvironment.terminate();
            return true;
        }

        // Called by the NSF whenever there is a state change
        private void handleStateChange(NSFStateMachineContext oContext)
        {
            // Log state change event
            string strMsg;
            string strNewState = oContext.EnteringState.Name;
            if (strNewState.StartsWith("Is"))
            {
                strMsg = string.Format(" Evaluating {0} check!", strNewState);
            }
            else
            {
                strMsg = string.Format(" Changing from the {0} state to the {1} state!", m_strSystemState, strNewState);
                // Capture new state
                m_strSystemState = strNewState;
            }
            ms_iLogger.Log(ELogLevel.Info, strMsg);

            // Notify anyone subscribed to the state change event
            if (eventStateChange != null)
            {
                eventStateChange.Invoke(m_strSystemState);
            }

            // Has the GUI been constructed yet?
            if (m_iGuideSystem.GuideGui != null)
            {
                // Clear any previous Operator Prompt
                if (m_iGuideSystem.GuideGui != null)
                {
                    m_iGuideSystem.GuideGui.ClearUserPrompt("");
                }
            }

            // Is there an operator prompt to display for this new state?
            string strOperatorPrompt;
            if (m_odictOperatorPromptForState.TryGetValue(oContext.EnteringState, out strOperatorPrompt))
            {
                m_iGuideSystem.GuideGui.PromptUser(strOperatorPrompt);
            }
        }

        public bool HandleEvent(GuideSystemEventsEnum eGuideEvent, object oEventData)
        {
            bool bEventHandled = false;

            if (!m_dictEventByEnum.ContainsKey(eGuideEvent))
            {
                ms_iLogger.Log(ELogLevel.Error, "Invalid Event passed to HandleEvent!");
                return false;
            }

            try
            {
                // Get event from enum passed in
                NSFEvent oEvent = m_dictEventByEnum[eGuideEvent];

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
                    string strMsg = string.Format(" Received Event {0}!", eGuideEvent.ToString());
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
                string strMsg = String.Format("Error!  Unable to handle handle event {0}!", eGuideEvent.ToString());
                ms_iLogger.Log(ELogLevel.Error, strMsg);
            }

            return false;
        }

        // Guard Functions

        private bool IsSensorReady(NSFStateMachineContext oContext)
        {
            bool bSensorReady = m_iGuideSystem.IsSensorReady;
            if (bSensorReady == false)
            {
                ms_iLogger.Log(ELogLevel.Error, "ScanSensor is in an invalid state for Scanning!");
            }
            return bSensorReady;
        }

        public bool AbortingScan { get; private set; }
        public bool ScanInProgress { get; private set; }

        private bool IsWindowClosed(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsWindowClosed;
        }

        private bool IsCriticalAlarms(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsCriticalAlarms;
        }

        private bool IsNoCriticalAlarms(NSFStateMachineContext oContext)
        {
            return !IsCriticalAlarms(oContext);
        }

        private bool IsHomed(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsHomed;
        }

        //All preloads compressed
        private bool IsGlassLoaded(NSFStateMachineContext context)
        {
            return m_iGuideSystem.IsGlassLoaded;
        }

        private bool IsGlassDetected(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsGlassDetected;
        }

        //Any preloads compressed or glass present at entrance but Not AllPreloadsCompressed
        private bool IsGlassLostDetected(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsGlassDetected && !m_iGuideSystem.IsGlassLoaded;
        }

        //One or more preloads comnpressed
        private bool IsGlassPresentAtEntrance(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsGlassPresentAtEntrance;
        }

        private bool GlassNotPresentAtEntrance(NSFStateMachineContext oContext)
        {
            return !IsGlassPresentAtEntrance(oContext);
        }

        private bool IsCartDocked(NSFStateMachineContext context)
        {
            return m_iGuideSystem.IsCartDocked;
        }

        private bool IsSafetyCircuitReset(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsSafetyCircuitReset;
        }

        private bool IsNotAdminPrivilege(NSFStateMachineContext oContext)
        {
            return IsAdminPrivilege(oContext);
        }

        private bool IsAdminPrivilege(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsAdminPrivilege;
        }

        private bool IsMoreStripes(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsMoreStripes;
        }

        private bool IsDoorOpen(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsDoorOpen;
        }

        private bool IsAutoLoad(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsAutoLoad;
        }

        private bool IsDoorClosed(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsDoorClosed;
        }

        private bool IsSaveNGImages(NSFStateMachineContext oContext)
        {
            // Todo Vincent:  Add check here for property in System that utilizes the new SaveNGImages point
            return m_iGuideSystem.IsSaveNGImages;
        }

        private bool IsDoorNotOpen(NSFStateMachineContext oContext)
        {
            return !m_iGuideSystem.IsDoorOpen;
        }

        private bool IsValidGlassSize(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsValidGlassSize;
        }

        private bool IsNTimesComplete(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsNTimesComplete;
        }

        private bool IsAnyPreLoadCompressed(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsAnyPreLoadCompressed;
        }

        private bool IsTrailingEdgePreLoadCompressed(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsTrailingEdgePreLoadCompressed;
        }

        private bool IsTrailingEdgePreLoadNotCompressed(NSFStateMachineContext oContext)
        {
            return !IsTrailingEdgePreLoadCompressed(oContext);
        }

        private bool IsMAPreLoadCompressed(NSFStateMachineContext oContext)
        {
            return m_iGuideSystem.IsMAPreLoadCompressed;
        }

        private bool IsMAPreLoadNotCompressed(NSFStateMachineContext oContext)
        {
            return !IsMAPreLoadCompressed(oContext);
        }

        // Action Functions
        private void InitSystem(NSFStateMachineContext oContext)
        {
            //m_iGuideSystem.Init();
        }

        private void EnableGlassNotDetectedAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableGlassNotDetectedAlarm(true);
        }

        private void DisableGlassNotDetectedAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableGlassNotDetectedAlarm(false);
        }

        private void DisableCartAndMailSlotAlarm(NSFStateMachineContext oContext)
        {
            StopMotionAndRetractMAStage(oContext);
            m_iGuideSystem.EnableMailSlotOpenedAlarm(false);
            m_iGuideSystem.EnableMailSlotClosedAlarm(false);
            m_iGuideSystem.EnableCartUnDockedAlarm(false);
        }

        private void StopMotionAndRetractMAStage(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.StopAllMotionAsync();
            m_iGuideSystem.MAStageRetract();
        }

        private void EnableMailSlotOpenedAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableMailSlotClosedAlarm(false);
            m_iGuideSystem.EnableMailSlotOpenedAlarm(true);
        }

        private void AlignGlassEntryActions(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.VacuumOn = true;
            DisableCartUnDockedAlarm(oContext);
            EnableMailSlotOpenedAlarm(oContext);
        }

        private void EnableMailSlotClosedAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableMailSlotOpenedAlarm(false);
            m_iGuideSystem.EnableMailSlotClosedAlarm(true);
        }

        private void DisableMailSlotAlarms(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableMailSlotOpenedAlarm(false);
            m_iGuideSystem.EnableMailSlotClosedAlarm(false);
        }

        private void EnableCartUnDockedAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableCartUnDockedAlarm(true);
        }

        private void StopImageSave(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.CancelImageWritting();
        }

        private void GlassLoadingExitActions(NSFStateMachineContext oContext)
        {
            DisableCartUnDockedAlarm(oContext);
            DisableMailSlotAlarms(oContext);
            DisableAbortButtonOnGui(oContext);
        }

        private void DisableCartUnDockedAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableCartUnDockedAlarm(false);
        }

        private void ProcessInitAfterSettings(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.InitAfterSettings();
        }

        private void ResetNTimesCounter(NSFStateMachineContext oContext)
        {
            // Reset the AutoAlign sequence
            m_iGuideSystem.ResetNTimesCounter();
        }

        private void MoveEdgeIn(NSFStateMachineContext oContext)
        {
            try
            {
                m_iGuideSystem.MoveEdgeInAsynch();
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Unable to MoveEdgeIn!", ex);
            }
        }

        private void MoveSideIn(NSFStateMachineContext oContext)
        {
            try
            {
                m_iGuideSystem.MoveSideInAsynch();
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Unable to MoveSideIn!", ex);
            }
        }

        private void HomeAllAxis(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.HomeAllAxis();
        }

        private void GlassUnLoadingEntryAction(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.TurnOnLightsAndGlassPosSensor(true);
            DisableGlassNotDetectedAlarm(oContext);
            DisableMailSlotAlarms(oContext);
            EnableAbortButtonOnGui(oContext);
            DisableMailSlotAlarms(oContext);
            DisableUnLoadButtonOnGui(oContext);

        }

        private void GlassUnloadingExitAction(NSFStateMachineContext oContext)
        {
            DisableAbortButtonOnGui(oContext);
            DisableMailSlotAlarms(oContext);
            DisableCartUnDockedAlarm(oContext);
        }

        private void IdleEntryActions(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.ManualMotionEnable = true;
            m_iGuideSystem.EnableLoadOfflineFileButton = true;
        }

        private void IdleExitActions(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.ManualMotionEnable = false;
            m_iGuideSystem.EnableLoadOfflineFileButton = false;
        }

        private void EnableManualMotion(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.ManualMotionEnable = true;
        }

        private void StopAllMotion(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.StopAllMotionAsync();
        }

        private void DisableManualMotion(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.ManualMotionEnable = false;
        }

        private void SaveAllInspectionResults(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.SaveAllInspectionResultsAsync();
        }

        private void GlassLoadedEntryActions(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableGlassSizeEntry = false;
            m_iGuideSystem.StartScanSensorCheck();
            EnableUnLoadButtonOnGui(null);
        }

        private void ViewOfflineStateEntryActions(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableCommitButton = true;
            m_iGuideSystem.EnableLoadOfflineFileButton = false;
            m_iGuideSystem.EnableStartButton = false;
            m_iGuideSystem.LoadGlassOnGuiEnable = false;
        }

        private void ViewOfflineStateExitActions(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableCommitButton = false;
            m_iGuideSystem.EnableLoadOfflineFileButton = true;
            m_iGuideSystem.LoadGlassOnGuiEnable = true;
            SaveAllInspectionResults(oContext);
        }

        private void GlassLoadedExitActions(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.StopScanSensorCheck();
        }

        private void EnableStartAndUnLoadButtonOnGui(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableStartButton = true;
            m_iGuideSystem.UnLoadGlassOnGuiEnable = true;
        }

        private void EnableUnLoadButtonOnGui(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.UnLoadGlassOnGuiEnable = true;
        }

        private void DisableUnLoadButtonOnGui(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.UnLoadGlassOnGuiEnable = false;
        }

        private void LoadOfflineFile(NSFStateMachineContext oContext)
        {
            // TODO Vincent  Here is where the file should be selected for offline viewer
            m_iGuideSystem.LoadOfflineDataFileAsync((string)((CStateMachineEventData)oContext.Trigger.Source).EventData);
        }

        private void DisableStartAndUnLoadButtonOnGui(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableStartButton = false;
            m_iGuideSystem.UnLoadGlassOnGuiEnable = false;
        }

        private void EnableLoadGlassOnGuiIfDoorOpened(NSFStateMachineContext oContext)
        {
            if (m_iGuideSystem.IsDoorOpen)
            {
                m_iGuideSystem.LoadGlassOnGuiEnable = true;
            }
        }

        private void EnableLoadGlassAndStartOnGui(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableStartAndAbortButton = true;
            m_iGuideSystem.LoadGlassOnGuiEnable = true;
        }

        private void DisableLoadGlassAndStartOnGui(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.LoadGlassOnGuiEnable = false;
            m_iGuideSystem.EnableStartButton = false;
        }

        private void EnableLoadGlassOnGui(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.LoadGlassOnGuiEnable = true;
        }

        private void GlassLoadingEntryActions(NSFStateMachineContext oContext)
        {
            DisableMailSlotAlarms(oContext);
            EnableAbortButtonOnGui(oContext);
        }

        private void EnableAbortButtonOnGui(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableAbortButton = true;
        }

        private void DisableAbortButtonOnGui(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableAbortButton = false;
        }

        private void FaultedStateEntryActions(NSFStateMachineContext oContext)
        {
            DisableLoadGlassAndStartOnGui(oContext);
            EnableManualMotion(oContext);
        }

        private void ClearFaults(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.ClearFaults();
        }

        private void DisableLoadGlassOnGui(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.LoadGlassOnGuiEnable = false;
        }

        private void NotifyInvalidGlassSizeAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.NotifyInvalidGlassSizeAlarm();
        }

        private void NotifytLoadGlassAbortAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.NotifytLoadGlassAbortAlarm();
        }

        private void NotifyUnLoadGlassAbortAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.NotifyUnLoadGlassAbortAlarm();
        }

        private void NotifyScanSensorAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.NotifyScanSensorFaultAlarm();
        }

        private void NotifyMotionAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.NotifyMotionFaultAlarm();
        }

        private void NotifyUnableToUnCompressPreLoadsAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.NotifyUnableToUnCompressPreLoadsAlarm();
        }
        private void NotifyUnableToRetractTEPinsAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.NotifyUnableToRetractTEPinsAlarm();
        }
        private void NotifyGlassLostAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.NotifyGlassLostAlarm();
        }
        private void NotifyUnableToExtendMAStageAlarm(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.NotifyUnableToExtendMAStageAlarm();
        }

        private void MAStageRetract(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.MAStageRetract();
        }

        private void MAStageExtend(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.MAStageExtend();
        }

        private void AutoLoadGlass(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.AutoLoadGlass();
        }

        private void SlowSqeezeWidthAndLength(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.ClampSheet();
        }

        private void DeployTrailingEdgePinsAndDamper(NSFContext oContext)
        {
            m_iGuideSystem.ExtendTrailingEdgePins(); // Deploy trailing edge pins
            m_iGuideSystem.ExtendDamperPistons(); // Extend Damper
            m_iGuideSystem.EnableGlassSizeEntry = false;    // Disable Glass Size Entry changes
        }

        private void MoveXToUnloadPos(NSFContext oContext)
        {
            m_iGuideSystem.MoveXToUnloadPosAsync();
        }

        private void MoveXToWalkGlassOutStartPos(NSFContext oContext)
        {
            m_iGuideSystem.MoveXToWalkGlassOutStartPos();
        }

        private void UnCompressTrailingEdgePreLoads(NSFContext oContext)
        {
            m_iGuideSystem.UnCompressTrailingEdgePreLoads();
        }

        private void RetractTrailingEdgePins(NSFContext oContext)
        {
            // Start timer in case the TrailingEdgePins do not retract
            oRetractTrailingEdgePinsTimeOutEvent.schedule(m_iGuideSystem.RetractTrailingEdgePinsTimeMax_ms);

            // Retract trailing edge pins
            m_iGuideSystem.RetractTrailingEdgePins();
        }

        private void StopRetractTrailingEdgePinsTimer(NSFContext oContext)
        {
            // Stop timer
            oRetractTrailingEdgePinsTimeOutEvent.unschedule();
        }

        private void UnCompressSidePreLoads(NSFContext oContext)
        {
            m_iGuideSystem.UnCompressSidePreLoads();
        }

        private void StartCartLoadTimer(NSFStateMachineContext oContext)
        {
            oCartLoadTimeoutEvent.schedule(m_iGuideSystem.CartLoadTimeMax_ms);
        }

        private void StopCartLoadTimer(NSFStateMachineContext oContext)
        {
            oCartLoadTimeoutEvent.unschedule();
        }

        private void StartDelayForSpringTimer(NSFStateMachineContext oContext)
        {
            oDelayForSpringTimeoutEvent.schedule(m_iGuideSystem.LoadSeqDelayForSpringTime_ms);
        }

        private void StartWaitForPreloadsUnCompressedTimer(NSFStateMachineContext oContext)
        {
            if (m_iGuideSystem.IsAnyPreLoadCompressed == false)
                this.HandleEvent(GuideSystemEventsEnum.AllPreloadsUnCompressedEvent, null);
            else
                oWaitForPreloadsUncompressedTimeoutEvent.schedule(m_iGuideSystem.UnloadSeqDelayForSpringTimeout_ms);
        }

        private void StopWaitForPreloadsUnCompressedTimer(NSFStateMachineContext oContext)
        {
            oWaitForPreloadsUncompressedTimeoutEvent.unschedule();
        }

        private void MoveAllToLoadPosAndEnableGlassSizeEntry(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.MoveAllToLoadPosAsync();
            m_iGuideSystem.EnableGlassSizeEntry = true;
        }

        private void PrepareStripeScanAsync(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.PrepareStripeScanAsync();
        }

        private void LaserAndPmtOn(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.LaserAndPmtOn = true;
        }

        private void StripeScanAsync(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.StripeScanAsync();
        }

        private void AbortScan(NSFStateMachineContext oContext)
        {
            AbortingScan = true;
            m_iGuideSystem.ScanAbort();
            m_iGuideSystem.StopAllMotionAsync();
        }

        private void DefectReviewStateEntryActions(NSFStateMachineContext oContext)
        {
            DisableAbortButtonOnGui(oContext);
        }

        private void StartNgImageSave(NSFStateMachineContext oContext)
        {
            // Todo:  Vincent  Please use this as the trigger that starts the NG Image Save (so we guarantee we won't miss the event when the images are complete).  Do not send ImagesSavedEvent back to state machine yet even if you complete early.
            m_iGuideSystem.SaveRawImagesAsynch();
            // This needs to be called last because it raises the NgImageSaved event which moves to next state.
            m_iGuideSystem.SaveNgImagesAsynch();
        }

        private void NotifySaveComplete(NSFStateMachineContext oContext)
        {
            // Todo:  Vincent  Please use this as the trigger that starts the NG Image Save (so we guarantee we won't miss the event when the images are complete)
            // Please call HandleEvent(GuideSystemEventsEnum.ImagesSavedEvent) if and when the image save is complete.  Note, this still needs to happen even if it wasn't successful.  This event must be guaranteed to come back no matter what.
            if (m_iGuideSystem.OperatorNgImagesWritten)
            {
                HandleEvent(GuideSystemEventsEnum.ImagesSavedEvent, null);
                HandleEvent(GuideSystemEventsEnum.NgImagesSavedEvent, null);
            }
            else
            {
                m_iGuideSystem.EnableAbortButton = true;
            }
        }

        private void ImagesSaveExitActions(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.EnableAbortButton = false;
        }

        private void DefectReviewStateExitActions(NSFStateMachineContext oContext)
        {
            DisableStartAndUnLoadButtonOnGui(oContext);
            SaveAllInspectionResults(oContext);
        }

        private void MacroScanStateEntryActions(NSFStateMachineContext oContext)
        {
            m_bScanComplete = false;
            m_iGuideSystem.GuideSetupRecipe.InspectionTime = DateTime.Now;
            m_iGuideSystem.CalculateScanSequence();
            ScanInProgress = true;
            m_iGuideSystem.EnableAbortButton = true;
            m_iGuideSystem.EnableScanSettingEntry = false;  // DisableScanSettings entry
            StartProcessingThreads(oContext);
            m_iGuideSystem.VacuumOn = true;
            EnableMailSlotOpenedAlarm(oContext);
            m_iGuideSystem.EnableInspectionAlarms(true);
            m_iGuideSystem.UnLoadGlassOnGuiEnable = false;
            ms_iLogger.Log(ELogLevel.Info, String.Format("Scan Started... Scan Type: {0}; CarrierID: {1}; CarrierThickness: {2}; WillowID: {3}; WillowThickness: {4};", m_iGuideSystem.GuideSetupRecipe.ScanType, 
                                                            m_iGuideSystem.GuideSetupRecipe.CarrierID, m_iGuideSystem.GuideSetupRecipe.CarrierThickness, m_iGuideSystem.GuideSetupRecipe.WillowID, m_iGuideSystem.GuideSetupRecipe.WillowThickness));
        }

        private void MacroScanStateExitActions(NSFStateMachineContext oContext)
        {
            ScanInProgress = false;
            m_iGuideSystem.EnableInspectionAlarms(false);
            DisableMailSlotAlarms(oContext);
            m_iGuideSystem.NotifyMacroScanComplete();
            m_iGuideSystem.EnableScanSettingEntry = true;  // EnableScanSettings entry
            m_iGuideSystem.TurnOnLightsAndGlassPosSensor(true);
            m_iGuideSystem.LaserAndPmtOn = false;
        }

        private void RevisitStateEntryActions(NSFStateMachineContext oContext)
        {
            m_bScanComplete = true;
            m_iGuideSystem.EnableSaveButtons = true;
            EnableManualMotion(oContext);
            m_iGuideSystem.EnableAbortButton = false;
            StartNgImageSave(oContext);
        }

        private void HandleStripeFeatureVectorComplete(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.LastFeatureVectorProvidedForStripe();
        }

        private void InspectingStateEntryActions(NSFStateMachineContext oContext)
        {
            EnableAbortButtonOnGui(oContext);
            m_iGuideSystem.EnableSaveButtons = false; 
            m_iGuideSystem.TurnOnLightsAndGlassPosSensor(false);
            m_iGuideSystem.StartQueueCountLoggingTimer();
        }

        private void InspectingStateExitActions(NSFStateMachineContext oContext)
        {
            AbortingScan = false;
            StopProcessingThreads(oContext);
            DisableAbortButtonOnGui(oContext);
            m_iGuideSystem.LaserAndPmtOn = false;
            m_iGuideSystem.RestartZetaScanAppAsync();
        }

        private void StartProcessingThreads(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.StartProcessingThreads();
        }

        private void StopProcessingThreads(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.StopProcessingThreads();
        }

        private void HandleScanProgessEventToReadyState(NSFStateMachineContext oContext)
        {
            Tuple<int, int> oProgress = new Tuple<int, int>(-1, -1);


            if ((oContext != null) && (oContext.Trigger != null) && (oContext.Trigger.Source != null))
            {
                oProgress = (Tuple<int, int>)((CStateMachineEventData)oContext.Trigger.Source).EventData;

            }

            ms_iLogger.Log(ELogLevel.Info, String.Format("Saw Scan Progress Event: Num Pixel Rows Scanned {0}, Total Pixels to Scan {1}", oProgress.Item1, oProgress.Item2));
        }

        private void HandleZetaStateChangeEventToReadyState(NSFStateMachineContext oContext)
        {
            string strNewState = "";

            if ((oContext != null) && (oContext.Trigger != null) && (oContext.Trigger.Source != null))
            {
                strNewState = (string)((CStateMachineEventData)oContext.Trigger.Source).EventData;
            }

            ms_iLogger.Log(ELogLevel.Info, String.Format("Saw State Change Event to {0}", strNewState));
        }

        private void StopSystemQueueCountLoggingTimer(NSFStateMachineContext oContext)
        {
            m_iGuideSystem.StopQueueCountLoggingTimer();
        }


        //        List<string> lstFaults;


        #endregion //Methods

        #region InnerClasses
        #endregion //InnerClasses

    }//end CGuideStateMachine

}//end namespace GuideSystem
