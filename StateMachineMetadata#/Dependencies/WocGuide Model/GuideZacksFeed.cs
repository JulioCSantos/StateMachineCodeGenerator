//AllAxisHomedEvent
//ClearFaultsEvent
//ClearFaultsEvent
//ClearFaultsEvent
//GlassNotDetectedEvent
//CartDockedEvent
//ImagesSavedEvent
//AbortCompleteEvent
//GlassRemovedAtEntranceEvent
//GlassNotDetectedEvent
//MoveCompleteEvent
//TrailingEdgePreLoadsExtendedEvent
//GlassDetectedAtDamperEvent
//CartLoadTimeoutEvent
//GlassDetectedAtDamperEvent
//AbortEvent
//CartUnDockedEvent
//MoveCompleteEvent
//TimeOutEvent
//MoveTimeOutEvent
//SafetyCircuitResetEvent
//CartDockedEvent
//DoorNotClosed
//DoorNotClosed
//GlassNotDetectedEvent
//DoorOpenEvent
//StripeScanReadyEvent
//UnLoadEvent
//Events (see Note for list)
//EStopEvent
//ClearFaultsEvent
//EStopEvent
//StripeScanCompleteEvent
//InitCompleteEvent
//CartDockedEvent
//No
//DoorCloseEvent
//UnloadGlassEvent
//CartUnDockedEvent
//GlassDetectedAtEntranceEvent
//AbortEvent
//StartScanEvent
//LoadGlassEvent
//CartUnDockedEvent
//AbortEvent
//AutoLoadTimeOutEvent
//TrailingEdgePinsRetractedEvent
//MoveCompleteEvent
//RetractTrailingEdgePinsTimeOutEvent
//AbortEvent
//LaserAndPmtOnEvent
//TimoutWaitingForPreLoadUnCompressEvent
//PanelOpenEvent
//AbortEvent
//AbortEvent
//CommitEvent
//LoadOfflineFileEvent
//ScanSensorFault
//AbortEvent
//ProcessingCompleteEvent
//NotifyScanSensorAlarm
//AllPreLoadsUnCompressedEvent
//WindowClosedEvent
//CommitEvent
//WindowNotClosedEvent
//LightOnEvent
//ScanSensorFaultEvent
//MotionFaultEvent
//SizeCorrectedEvent
//CabinetOverTempEvent
//InitEvent
//DoorOpenEvent
//AbortEvent
//MoveCompleteEvent
//BypassSwitchOnEvent
//MoveCompleteEvent
//MoveCompleteEvent
//CartUnDockedEvent
//MoveCompleteEvent
//CartUnDockedEvent
//CabinetOverTempEvent
//BypassSwitchOnEvent
//MotionFaultEvent
//PanelOpenEvent
//ScanSensorFaultEvent
//VacuumOffEvent
//MoveCompleteEvent
//MoveCompleteEvent
//AutoLoadCompleteEvent
//MoveCompleteEvent
//AutoLoadFailedEvent
//VacuumOffEvent
//AirOffEvent
//AbortEvent
//LoadPosMoveCompleteEvent

//ImagesSaveState: NotifySaveComplete
//AbortingState: SensorAbort
//LaserAndPmtOnState: LaserAndPmtOn
//WaitForPreloadsUnCompressedState: StopTimer2, StartTimer2
//MoveXToUnLoadPosState: MoveXToUnLoadPos
//PromptToDockCart: EnableCartUnDockedAlarm, PromptToDockCart
//OpenDoorState: EnableMailSlotClosedAlarm, PromptToOpenDoor
//PrepForUnLoadState: PrepForUnLoadStateInitial, MoveXToUnLoadPosState, IsCartDocked2_, IsDoorOpened3_, PromptToDockCart, OpenDoorState
//ResetState: PromptToReset
//MoveToLoadPosState: MoveAllToLoadPos
//DelayForSpring: StartTimer
//HomeAllAxisState: HomeAllAxis
//MAStageRetractState: MAStageRetract
//WalkGlassOutState: MoveXToUnLoadPos
//MAStageExtendState: MAStageExtend
//MoveXToWalkGlassOutStartPosState: MoveXToWalkGlassOutStartPos
//WalkGlassOutState: MAStageRetractState, WalkGlassOutState, MAStageExtendState, MoveXToWalkGlassOutStartPosState, WalkGlassOutStateInitial
//CorrectGlassSizeState: PromptSizeCorrection
//RemoveGlassState: PromptRemoveGlass
//UnCompressSidePreLoadState: UnCompressSidePreload
//ClampSheetState: SlowSqueezeWidthAndLength
//CloseDoorToAlignState: DisableMailSlotAlarms
//OperatorManualLoadState: PromptManualLoad
//WaitForDoorOpenToLoadState: PromptToOpenDoor
//GlassLoadingFromCartState: StopCartLoadTimer, StartCartLoadTimer
//DeployTrailingEdgePins: DeployTrailingEdgePins
//UnCompressTrailingEdgePreLoadsState: UnCompressTrailingEdgePreloads
//RetractTrailingEdgePinsState: RetractTrailingEdgePins
//StepInSideState: MoveSideIn
//StepInLeadingEdgeState: MoveEdgeIn
//AutoAlignGlassState: IsAnyPreLoadCompressed_, DelayForSpring, ResetNTimesCounter, IsNTimes_Complete_, StepInSideState, StepInLeadingEdgeState, AutoAlignGlassStateInitial
//AlignGlassState: DisableCartUnDockedAlarm, IsWarningOrCriticalAlarm2_, CorrectGlassSizeState, ClampSheetState, IsValidGlassSize_, AlignGlassStateInitial, DeployTrailingEdgePins, AutoAlignGlassState
//WaitForGlassToEnterState: PromptToStartCartLoad
//GlassLoadingState: IsAutoLoad_, EnableAbortButton, ResetState, IsDoorOpened2_, MoveToLoadPosState, AutoLoadSeqState, DisableCartAndMailSlotAlarm, IsSafetyCircuitReset_, CloseDoorToAlignState, OperatorManualLoadState, IsDoorOpened_, WaitForDoorOpenToLoadState, GlassLoadingFromCartState, GlassLoadingStateInitial, AlignGlassState, WaitForGlassToEnterState
//GlassUnLoadingState: WaitForPreloadsUnCompressedState, PrepForUnLoadState, DisableAbortButton, WalkGlassOutState, DisableGlassNotDetectedAlarm, RemoveGlassState, UnCompressSidePreLoadState, UnCompressTrailingEdgePreLoadsState, RetractTrailingEdgePinsState, GlassUnLoadingStateInitial
//CartDockedState: EnableLoadGlassOnGUI__IsDoorOpened_, DisableLoadGlassOnGui
//GlassUnLoadedState: GlassUnLoadedStateInitial, CartUnDockedState
//DefectReviewState: RevisitState, DefectReviewStateInitial
//MacroScanState: IsMoreStripes, MacroScanStateInitial, ScanningState, MoveToStripeState
//InspectingState: InspectingStateInitial, DefectReviewState, MacroScanState
//Idle_State: IsGlassLoaded_, GlassUnLoadedState, Idle_StateInitial, GlassLoadedState
//ReadyState: StopAllMotion, InspectingState, ReadyStateInitial, Idle_State
//SystemPoweredState: IsWarningOrCriticalAlarm, ReadyState, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: HomeAllAxisTOIsWarningOrCriticalAlarmTransition From:HomeAllAxisState To:IsWarningOrCriticalAlarm Via:AllAxisHomedEvent Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarmTOFaultedTransition From:IsWarningOrCriticalAlarm To:FaultedState Via: Action:none Guard:Yes
//External Transition: FaultWithGlassHeldTOGlassLoadedTransition From:FaultWithGlassHeldState To:GlassLoadedState Via:ClearFaultsEvent Action:none Guard:IsNoWarningOrCriticalAlarms
//External Transition: FaultWithGlassLostTOFaultAndOperatorClearsGlassTransition From:FaultWithGlassLostState To:FaultAndOperatorClearsGlassState Via:ClearFaultsEvent Action:PromptToClearGlassAndClearFaultsAgain Guard:none
//External Transition: FaultAndOperatorClearsGlassTOIsWarningOrCriticalAlarmTransition From:FaultAndOperatorClearsGlassState To:IsWarningOrCriticalAlarm Via:ClearFaultsEvent Action:MoveAllToLoadPos Guard:IsNoWarningOrCriticalAlarms
//External Transition: FaultWithGlassHeldTOFaultWithGlassLostTransition From:FaultWithGlassHeldState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: InitialTOIsSheetDetected2_Transition From:Initial To:IsSheetDetected2_ Via: Action:none Guard:none
//External Transition: AutoAlignGlassInitialTOIsNTimes_Complete_Transition From:AutoAlignGlassStateInitial To:IsNTimes_Complete_ Via: Action:none Guard:none
//External Transition: PromptToDockCartTOIsDoorOpened3_Transition From:PromptToDockCart To:IsDoorOpened3_ Via:CartDockedEvent Action:none Guard:none
//External Transition: IsNTimes_Complete_TOClampSheetTransition From:IsNTimes_Complete_ To:ClampSheetState Via: Action:none Guard:Yes
//External Transition: ImagesSaveTOGlassLoadedTransition From:ImagesSaveState To:GlassLoadedState Via:ImagesSavedEvent Action:EnableStartAndUnLoadButtonOnGui Guard:none
//External Transition: IsHomed_TOIsSheetDetected_Transition From:IsHomed_ To:IsSheetDetected_ Via: Action:none Guard:No
//External Transition: IsDoorOpened_TOIsAutoLoad_Transition From:IsDoorOpened_ To:IsAutoLoad_ Via: Action:none Guard:Yes
//External Transition: AbortingTOIdle_Transition From:AbortingState To:Idle_State Via:AbortCompleteEvent Action:NotifyScanAbortAlarm Guard:none
//External Transition: IsAutoLoad_TOWaitForGlassToEnterTransition From:IsAutoLoad_ To:WaitForGlassToEnterState Via: Action:none Guard:Yes
//External Transition: RemoveGlassTOIdle_Transition From:RemoveGlassState To:Idle_State Via:GlassRemovedAtEntranceEvent Action:MoveAllToLoadPos Guard:none
//External Transition: ReadyTOFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: IsCartDocked_TOGlassUnLoadedTransition From:IsCartDocked_ To:GlassUnLoadedState Via: Action:none Guard:No
//External Transition: StepInSideTOStepInLeadingEdgeTransition From:StepInSideState To:StepInLeadingEdgeState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: DeployTrailingEdgePinsTOAutoAlignGlassTransition From:DeployTrailingEdgePins To:AutoAlignGlassState Via:TrailingEdgePreLoadsExtendedEvent Action:none Guard:none
//External Transition: IsDoorOpened2_TOAlignGlassTransition From:IsDoorOpened2_ To:AlignGlassState Via: Action:none Guard:No
//External Transition: IsDoorOpened2_TOCloseDoorToAlignTransition From:IsDoorOpened2_ To:CloseDoorToAlignState Via: Action:none Guard:Yes
//External Transition: OperatorManualLoadTOIsSafetyCircuitReset_Transition From:OperatorManualLoadState To:IsSafetyCircuitReset_ Via:GlassDetectedAtDamperEvent Action:none Guard:none
//External Transition: GlassLoadingFromCartTOAutoLoadSeqTransition From:GlassLoadingFromCartState To:AutoLoadSeqState Via:CartLoadTimeoutEvent Action:none Guard:none
//External Transition: GlassLoadingFromCartTOIsSafetyCircuitReset_Transition From:GlassLoadingFromCartState To:IsSafetyCircuitReset_ Via:GlassDetectedAtDamperEvent Action:none Guard:none
//External Transition: WaitForDoorOpenToLoadTOIdle_Transition From:WaitForDoorOpenToLoadState To:Idle_State Via:AbortEvent Action:none Guard:none
//External Transition: WaitForDoorOpenToLoadTOIdle_Transition From:WaitForDoorOpenToLoadState To:Idle_State Via:CartUnDockedEvent Action:none Guard:none
//External Transition: GlassLoadingInitialTOMoveToLoadPosTransition From:GlassLoadingStateInitial To:MoveToLoadPosState Via: Action:none Guard:none
//External Transition: ClampSheetTOIsWarningOrCriticalAlarm2_Transition From:ClampSheetState To:IsWarningOrCriticalAlarm2_ Via:MoveCompleteEvent Action:EnableGlassNotDetectedAlarm Guard:none
//External Transition: DelayForSpringTOIsAnyPreLoadCompressed_Transition From:DelayForSpring To:IsAnyPreLoadCompressed_ Via:TimeOutEvent Action:none Guard:none
//External Transition: GlassUnLoadingTOFaultWithGlassLostTransition From:GlassUnLoadingState To:FaultWithGlassLostState Via:MoveTimeOutEvent Action:none Guard:none
//External Transition: ResetTOIsDoorOpened2_Transition From:ResetState To:IsDoorOpened2_ Via:SafetyCircuitResetEvent Action:none Guard:none
//External Transition: IsSafetyCircuitReset_TOResetTransition From:IsSafetyCircuitReset_ To:ResetState Via: Action:none Guard:No
//External Transition: CartUnDockedTOCartDockedTransition From:CartUnDockedState To:CartDockedState Via:CartDockedEvent Action:none Guard:!IsDoorOpen
//External Transition: MacroScanTOFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:DoorNotClosed Action:none Guard:none
//External Transition: AlignGlassTOFaultWithGlassLostTransition From:AlignGlassState To:FaultWithGlassLostState Via:DoorNotClosed Action:none Guard:none
//External Transition: GlassLoadedTOFaultWithGlassLostTransition From:GlassLoadedState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: WaitForDoorOpenToLoadTOIsAutoLoad_Transition From:WaitForDoorOpenToLoadState To:IsAutoLoad_ Via:DoorOpenEvent Action:none Guard:none
//External Transition: MoveToStripeTOScanningTransition From:MoveToStripeState To:ScanningState Via:StripeScanReadyEvent Action:none Guard:none
//External Transition: DefectReviewTOGlassUnLoadingTransition From:DefectReviewState To:GlassUnLoadingState Via:UnLoadEvent Action:none Guard:none
//External Transition: DefectReviewInitialTORevisitTransition From:DefectReviewStateInitial To:RevisitState Via: Action:none Guard:none
//External Transition: GlassLoadedTOFaultWithGlassHeldTransition From:GlassLoadedState To:FaultWithGlassHeldState Via:Events (see Note for list) Action:none Guard:none
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:EStopEvent Action:none Guard:none
//External Transition: FaultWithNoGlassTOIsWarningOrCriticalAlarmTransition From:FaultWithNoGlassState To:IsWarningOrCriticalAlarm Via:ClearFaultsEvent Action:none Guard:none
//External Transition: ReadyTOFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:EStopEvent Action:none Guard:none
//External Transition: Idle_InitialTOIsGlassLoaded_Transition From:Idle_StateInitial To:IsGlassLoaded_ Via: Action:none Guard:none
//External Transition: IsCartDocked_TOCartDockedTransition From:IsCartDocked_ To:CartDockedState Via: Action:none Guard:Yes
//External Transition: ScanningTOIsMoreStripesTransition From:ScanningState To:IsMoreStripes Via:StripeScanCompleteEvent Action:none Guard:none
//External Transition: GlassUnLoadedInitialTOCartUnDockedTransition From:GlassUnLoadedStateInitial To:CartUnDockedState Via: Action:none Guard:none
//External Transition: IsMoreStripesTOMoveToStripeTransition From:IsMoreStripes To:MoveToStripeState Via: Action:none Guard:Yes
//External Transition: MacroScanInitialTOLaserAndPmtOnTransition From:MacroScanStateInitial To:LaserAndPmtOnState Via: Action:none Guard:none
//External Transition: InitialTOSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: SystemPoweredInitialTOInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: InitTOInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarmTOIsGlassLostDetected_Transition From:IsWarningOrCriticalAlarm To:IsGlassLostDetected_ Via: Action:none Guard:No
//External Transition: InitAfterSettingsTOIsWarningOrCriticalAlarmTransition From:InitAfterSettingsState To:IsWarningOrCriticalAlarm Via: Action:none Guard:none
//External Transition: IsMoreStripesTOProcessingTransition From:IsMoreStripes To:ProcessingState Via: Action:none Guard:No
//External Transition: CartUnDockedTOCartDockedTransition From:CartUnDockedState To:CartDockedState Via:CartDockedEvent Action:EnableLoadGlassOnGui Guard:IsDoorOpen
//External Transition: MoveXToUnLoadPosTOIsCartDocked2_Transition From:MoveXToUnLoadPosState To:IsCartDocked2_ Via: Action:none Guard:none
//External Transition: IsNTimes_Complete_TOStepInSideTransition From:IsNTimes_Complete_ To:StepInSideState Via:No Action:none Guard:none
//External Transition: IsSafetyCircuitReset_TOIsDoorOpened2_Transition From:IsSafetyCircuitReset_ To:IsDoorOpened2_ Via: Action:none Guard:Yes
//External Transition: CloseDoorToAlignTOAlignGlassTransition From:CloseDoorToAlignState To:AlignGlassState Via:DoorCloseEvent Action:none Guard:none
//External Transition: IsValidGlassSize_TODeployTrailingEdgePinsTransition From:IsValidGlassSize_ To:DeployTrailingEdgePins Via: Action:none Guard:Yes
//External Transition: IsDoorOpened_TOWaitForDoorOpenToLoadTransition From:IsDoorOpened_ To:WaitForDoorOpenToLoadState Via: Action:none Guard:No
//External Transition: GlassLoadedTOGlassUnLoadingTransition From:GlassLoadedState To:GlassUnLoadingState Via:UnloadGlassEvent Action:none Guard:CartDocked
//External Transition: IsGlassLoaded_TOGlassLoadedTransition From:IsGlassLoaded_ To:GlassLoadedState Via: Action:none Guard:Yes
//External Transition: CartDockedTOCartUnDockedTransition From:CartDockedState To:CartUnDockedState Via:CartUnDockedEvent Action:none Guard:none
//External Transition: IsGlassLoaded_TOIsCartDocked_Transition From:IsGlassLoaded_ To:IsCartDocked_ Via: Action:none Guard:No
//External Transition: WaitForGlassToEnterTOGlassLoadingFromCartTransition From:WaitForGlassToEnterState To:GlassLoadingFromCartState Via:GlassDetectedAtEntranceEvent Action:EnableCartUnDockedAlarm Guard:none
//External Transition: WaitForGlassToEnterTOIdle_Transition From:WaitForGlassToEnterState To:Idle_State Via:AbortEvent Action:none Guard:none
//External Transition: InspectingInitialTOMacroScanTransition From:InspectingStateInitial To:MacroScanState Via: Action:none Guard:none
//External Transition: GlassLoadedTOIsWarningOrCriticalAlarm3_Transition From:GlassLoadedState To:IsWarningOrCriticalAlarm3_ Via:StartScanEvent Action:none Guard:none
//External Transition: ReadyInitialTOIdle_Transition From:ReadyStateInitial To:Idle_State Via: Action:none Guard:none
//External Transition: IsHomed_TOReadyTransition From:IsHomed_ To:ReadyState Via: Action:none Guard:Yes
//External Transition: CartDockedTOGlassLoadingTransition From:CartDockedState To:GlassLoadingState Via:LoadGlassEvent Action:MoveAllToLoadPos Guard:none
//External Transition: WaitForGlassToEnterTOIdle_Transition From:WaitForGlassToEnterState To:Idle_State Via:CartUnDockedEvent Action:none Guard:none
//External Transition: GlassLoadingTOFaultWithGlassLostTransition From:GlassLoadingState To:FaultWithGlassLostState Via:AbortEvent Action:NotifyLoadGlassAbortAlarm Guard:none
//External Transition: IsCartDocked2_TOIsDoorOpened3_Transition From:IsCartDocked2_ To:IsDoorOpened3_ Via: Action:none Guard:Yes
//External Transition: AutoLoadSeqTOFaultWithGlassLostTransition From:AutoLoadSeqState To:FaultWithGlassLostState Via:AutoLoadTimeOutEvent Action:none Guard:none
//External Transition: RetractTrailingEdgePinsTOWalkGlassOutTransition From:RetractTrailingEdgePinsState To:WalkGlassOutState Via:TrailingEdgePinsRetractedEvent Action:StopRetractTrailingEdgePinsTimer Guard:none
//External Transition: MAStageRetractTORemoveGlassTransition From:MAStageRetractState To:RemoveGlassState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: WalkGlassOutInitialTOMoveXToWalkGlassOutStartPosTransition From:WalkGlassOutStateInitial To:MoveXToWalkGlassOutStartPosState Via: Action:none Guard:none
//External Transition: IsDoorOpened3_TOOpenDoorTransition From:IsDoorOpened3_ To:OpenDoorState Via: Action:none Guard:No
//External Transition: IsAutoLoad_TOOperatorManualLoadTransition From:IsAutoLoad_ To:OperatorManualLoadState Via: Action:none Guard:No
//External Transition: RetractTrailingEdgePinsTOFaultWithGlassLostTransition From:RetractTrailingEdgePinsState To:FaultWithGlassLostState Via:RetractTrailingEdgePinsTimeOutEvent Action:none Guard:none
//External Transition: IsAnyPreLoadCompressed_TOFaultWithGlassLostTransition From:IsAnyPreLoadCompressed_ To:FaultWithGlassLostState Via: Action:CreatePreLoadCompressedDuringAlignAlarm Guard:Yes
//External Transition: IsScanSensorReady_TOIsWindowClosed_Transition From:IsScanSensorReady_ To:IsWindowClosed_ Via: Action:none Guard:Yes
//External Transition: IsWarningOrCriticalAlarm3_TOIsScanSensorReady_Transition From:IsWarningOrCriticalAlarm3_ To:IsScanSensorReady_ Via: Action:none Guard:No
//External Transition: MacroScanTOAbortingTransition From:MacroScanState To:AbortingState Via:AbortEvent Action:none Guard:none
//External Transition: LaserAndPmtOnTOIsMoreStripesTransition From:LaserAndPmtOnState To:IsMoreStripes Via:LaserAndPmtOnEvent Action:none Guard:none
//External Transition: WaitForPreloadsUnCompressedTOFaultWithGlassLostTransition From:WaitForPreloadsUnCompressedState To:FaultWithGlassLostState Via:TimoutWaitingForPreLoadUnCompressEvent Action:none Guard:none
//External Transition: IsCartDocked2_TOPromptToDockCartTransition From:IsCartDocked2_ To:PromptToDockCart Via: Action:none Guard:No
//External Transition: IsWarningOrCriticalAlarm2_TOGlassLoadedTransition From:IsWarningOrCriticalAlarm2_ To:GlassLoadedState Via: Action:none Guard:No
//External Transition: IsWarningOrCriticalAlarm2_TOFaultWithGlassHeldTransition From:IsWarningOrCriticalAlarm2_ To:FaultWithGlassHeldState Via: Action:none Guard:Yes
//External Transition: AlignGlassInitialTOIsValidGlassSize_Transition From:AlignGlassStateInitial To:IsValidGlassSize_ Via: Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarm3_TOFaultWithGlassHeldTransition From:IsWarningOrCriticalAlarm3_ To:FaultWithGlassHeldState Via: Action:none Guard:Yes
//External Transition: IsSheetDetected_TOFaultedTransition From:IsSheetDetected_ To:FaultedState Via: Action:none Guard:none
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:PanelOpenEvent Action:none Guard:none
//External Transition: ProcessingTOGlassLoadedTransition From:ProcessingState To:GlassLoadedState Via:AbortEvent Action:none Guard:none
//External Transition: ImagesSaveTOGlassLoadedTransition From:ImagesSaveState To:GlassLoadedState Via:AbortEvent Action:StopImageSave Guard:none
//External Transition: ViewOfflineTOIsGlassLoaded_Transition From:ViewOfflineState To:IsGlassLoaded_ Via:CommitEvent Action:none Guard:none
//External Transition: Idle_TOViewOfflineTransition From:Idle_State To:ViewOfflineState Via:LoadOfflineFileEvent Action:none Guard:none
//External Transition: IsSaveNGImages_TOGlassLoadedTransition From:IsSaveNGImages_ To:GlassLoadedState Via: Action:EnableStartAndUnLoadButtonOnGui Guard:No
//External Transition: IsSaveNGImages_TOImagesSaveTransition From:IsSaveNGImages_ To:ImagesSaveState Via: Action:none Guard:Yes
//External Transition: AbortingTOFaultWithGlassHeldTransition From:AbortingState To:FaultWithGlassHeldState Via:ScanSensorFault Action:none Guard:none
//External Transition: IsSheetDetected_TOHomeAllAxisTransition From:IsSheetDetected_ To:HomeAllAxisState Via: Action:none Guard:none
//External Transition: AbortingTOFaultWithGlassHeldTransition From:AbortingState To:FaultWithGlassHeldState Via:AbortEvent Action:none Guard:none
//External Transition: ProcessingTODefectReviewTransition From:ProcessingState To:DefectReviewState Via:ProcessingCompleteEvent Action:none Guard:none
//External Transition: Idle_TOFaultWithGlassHeldTransition From:Idle_State To:FaultWithGlassHeldState Via:NotifyScanSensorAlarm Action:none Guard:ScanSensorFaultEvent
//External Transition: WaitForPreloadsUnCompressedTORetractTrailingEdgePinsTransition From:WaitForPreloadsUnCompressedState To:RetractTrailingEdgePinsState Via:AllPreLoadsUnCompressedEvent Action:none Guard:none
//External Transition: WaitForWindowClosedTOInspectingTransition From:WaitForWindowClosedState To:InspectingState Via:WindowClosedEvent Action:none Guard:none
//External Transition: IsScanSensorReady_TOFaultWithGlassHeldTransition From:IsScanSensorReady_ To:FaultWithGlassHeldState Via: Action:none Guard:No
//External Transition: IsWindowClosed_TOInspectingTransition From:IsWindowClosed_ To:InspectingState Via: Action:none Guard:Yes
//External Transition: IsWindowClosed_TOWaitForWindowClosedTransition From:IsWindowClosed_ To:WaitForWindowClosedState Via: Action:none Guard:No
//External Transition: IsAnyPreLoadCompressed_TOIsNTimes_Complete_Transition From:IsAnyPreLoadCompressed_ To:IsNTimes_Complete_ Via: Action:none Guard:No
//External Transition: RevisitTOIsSaveNGImages_Transition From:RevisitState To:IsSaveNGImages_ Via:CommitEvent Action:none Guard:none
//External Transition: IsValidGlassSize_TOCorrectGlassSizeTransition From:IsValidGlassSize_ To:CorrectGlassSizeState Via: Action:none Guard:No
//External Transition: MacroScanTOFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:WindowNotClosedEvent Action:none Guard:IsNotAdminLogin
//External Transition: MacroScanTOFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:LightOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:ScanSensorFaultEvent Action:NotifyScanSensorAlarm Guard:none
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:MotionFaultEvent Action:NotifyMotionAlarm Guard:none
//External Transition: IsGlassLostDetected_TOFaultedTransition From:IsGlassLostDetected_ To:FaultedState Via: Action:none Guard:Yes
//External Transition: IsGlassLostDetected_TOIsHomed_Transition From:IsGlassLostDetected_ To:IsHomed_ Via: Action:none Guard:No
//External Transition: IsSheetDetected2_TOFaultWithGlassLostTransition From:IsSheetDetected2_ To:FaultWithGlassLostState Via: Action:none Guard:Yes
//External Transition: IsDoorOpened3_TOUnCompressSidePreLoadTransition From:IsDoorOpened3_ To:UnCompressSidePreLoadState Via: Action:none Guard:Yes
//External Transition: CorrectGlassSizeTOIsValidGlassSize_Transition From:CorrectGlassSizeState To:IsValidGlassSize_ Via:SizeCorrectedEvent Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:CabinetOverTempEvent Action:none Guard:none
//External Transition: SystemPoweredTOInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: OpenDoorTOUnCompressSidePreLoadTransition From:OpenDoorState To:UnCompressSidePreLoadState Via:DoorOpenEvent Action:none Guard:none
//External Transition: PrepForUnLoadInitialTOMoveXToUnLoadPosTransition From:PrepForUnLoadStateInitial To:MoveXToUnLoadPosState Via: Action:none Guard:none
//External Transition: GlassUnLoadingInitialTOPrepForUnLoadTransition From:GlassUnLoadingStateInitial To:PrepForUnLoadState Via: Action:none Guard:none
//External Transition: GlassUnLoadingTOFaultWithGlassLostTransition From:GlassUnLoadingState To:FaultWithGlassLostState Via:AbortEvent Action:NotifyUnLoadGlassAbortAlarm Guard:none
//External Transition: UnCompressTrailingEdgePreLoadsTOWaitForPreloadsUnCompressedTransition From:UnCompressTrailingEdgePreLoadsState To:WaitForPreloadsUnCompressedState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:BypassSwitchOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: IsSheetDetected2_TOFaultWithNoGlassTransition From:IsSheetDetected2_ To:FaultWithNoGlassState Via: Action:none Guard:No
//External Transition: WalkGlassOutTOMAStageRetractTransition From:WalkGlassOutState To:MAStageRetractState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: StepInLeadingEdgeTODelayForSpringTransition From:StepInLeadingEdgeState To:DelayForSpring Via:MoveCompleteEvent Action:none Guard:none
//External Transition: MoveToLoadPosTOIdle_Transition From:MoveToLoadPosState To:Idle_State Via:CartUnDockedEvent Action:none Guard:none
//External Transition: MAStageExtendTOFaultWithGlassLostTransition From:MAStageExtendState To:FaultWithGlassLostState Via:MoveCompleteEvent Action:none Guard:IsMAPreloadNotCompressed
//External Transition: WalkGlassOutTOFaultWithGlassLostTransition From:WalkGlassOutState To:FaultWithGlassLostState Via:CartUnDockedEvent Action:none Guard:none
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:CabinetOverTempEvent Action:none Guard:none
//External Transition: ReadyTOFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:BypassSwitchOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: ReadyTOFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:MotionFaultEvent Action:NotifyMotionAlarm Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:PanelOpenEvent Action:none Guard:IsNotBypassed
//External Transition: ReadyTOFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:ScanSensorFaultEvent Action:NotifyScanSensorAlarm Guard:none
//External Transition: MacroScanTOFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:VacuumOffEvent Action:none Guard:none
//External Transition: MAStageExtendTOWalkGlassOutTransition From:MAStageExtendState To:WalkGlassOutState Via:MoveCompleteEvent Action:none Guard:IsMAPreloadCompressed
//External Transition: MoveXToWalkGlassOutStartPosTOMAStageExtendTransition From:MoveXToWalkGlassOutStartPosState To:MAStageExtendState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: AutoLoadSeqTOIsSafetyCircuitReset_Transition From:AutoLoadSeqState To:IsSafetyCircuitReset_ Via:AutoLoadCompleteEvent Action:none Guard:none
//External Transition: UnCompressSidePreLoadTOUnCompressTrailingEdgePreLoadsTransition From:UnCompressSidePreLoadState To:UnCompressTrailingEdgePreLoadsState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: AutoLoadSeqTOOperatorManualLoadTransition From:AutoLoadSeqState To:OperatorManualLoadState Via:AutoLoadFailedEvent Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:VacuumOffEvent Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:AirOffEvent Action:none Guard:none
//External Transition: CorrectGlassSizeTOFaultWithGlassLostTransition From:CorrectGlassSizeState To:FaultWithGlassLostState Via:AbortEvent Action:NotifyInvalidGlassSizeAlarm Guard:none
//External Transition: MoveToLoadPosTOIsDoorOpened_Transition From:MoveToLoadPosState To:IsDoorOpened_ Via:LoadPosMoveCompleteEvent Action:none Guard:none
//Internal Transition: GlassLoadedStateOnSensorCheckTOTransition. OwnedBy GlassLoadedState. Action CheckSensor  Guard:none
//Internal Transition: GlassUnLoadingStateOnGlassNotDetectedEventTransition. OwnedBy GlassUnLoadingState. Action   Guard:none
//Internal Transition: ReadyStateOnmissingeventTransition. OwnedBy ReadyState. Action   Guard:none
//Internal Transition: ReadyStateOnGlassNotDetectedEventTransition. OwnedBy ReadyState. Action   Guard:none
//Internal Transition: Idle_StateOnGlassNotDetectedEventTransition. OwnedBy Idle_State. Action   Guard:none
//Internal Transition: InspectingStateOnSheetJudgementCompleteEventTransition. OwnedBy InspectingState. Action EnableStartAndUnLoadButtons  Guard:none
//Internal Transition: Idle_StateOnLoadStripeImageEventTransition. OwnedBy Idle_State. Action m_oGuideSystem_LoadStripeImage  Guard:none
//Internal Transition: CartDockedStateOnDoorOpenEventTransition. OwnedBy CartDockedState. Action EnableLoadGlasssOnGui  Guard:none
//Internal Transition: InspectingStateOnStripeFeatureVectorCompleteEventTransition. OwnedBy InspectingState. Action m_oGuideSystem_LastFeatureVectorProvidedForStripe  Guard:none
//Internal Transition: ReadyStateOnScanSensorStateChangeEventTransition. OwnedBy ReadyState. Action LogScanSensorStateChange  Guard:none
//Internal Transition: ReadyStateOnScanProgressEventTransition. OwnedBy ReadyState. Action UpdateScanProgressOnGui  Guard:none
