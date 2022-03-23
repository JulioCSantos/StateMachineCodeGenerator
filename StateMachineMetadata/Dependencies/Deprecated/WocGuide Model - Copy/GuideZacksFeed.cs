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
//OpenDoorState: EnableMailSlotClosedAlarm, PromptToOpenDoor; DisableMailSlotAlarms
//PrepForUnLoadState: PrepForUnLoadStateInitial, MoveXToUnLoadPosState, IsCartDocked2?, IsDoorOpened3?, PromptToDockCart, OpenDoorState
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
//OperatorManualLoadState: PromptManualLoad; DisableCartAndMailSlotAlarm
//WaitForDoorOpenToLoadState: PromptToOpenDoor
//GlassLoadingFromCartState: StopCartLoadTimer, StartCartLoadTimer
//DeployTrailingEdgePins: DeployTrailingEdgePins; ExtendDamper; DisableGlassSizeEntry
//UnCompressTrailingEdgePreLoadsState: UnCompressTrailingEdgePreloads
//RetractTrailingEdgePinsState: RetractTrailingEdgePins; StartTimer
//StepInSideState: MoveSideIn
//StepInLeadingEdgeState: MoveEdgeIn
//AutoAlignGlassState: IsAnyPreLoadCompressed?, DelayForSpring, ResetNTimesCounter, IsNTimes Complete?, StepInSideState, StepInLeadingEdgeState, AutoAlignGlassStateInitial
//AlignGlassState: DisableCartUnDockedAlarm; EnableMailSlotOpenedAlarm; VacuumOn, IsWarningOrCriticalAlarm2?, CorrectGlassSizeState, ClampSheetState, IsValidGlassSize?, AlignGlassStateInitial, DeployTrailingEdgePins, AutoAlignGlassState
//WaitForGlassToEnterState: PromptToStartCartLoad
//GlassLoadingState: IsAutoLoad?, EnableAbortButton, ResetState, IsDoorOpened2?, MoveToLoadPosState, AutoLoadSeqState, DisableCartAndMailSlotAlarm, IsSafetyCircuitReset?, CloseDoorToAlignState, OperatorManualLoadState, IsDoorOpened?, WaitForDoorOpenToLoadState, GlassLoadingFromCartState, GlassLoadingStateInitial, AlignGlassState, WaitForGlassToEnterState
//GlassUnLoadingState: WaitForPreloadsUnCompressedState, PrepForUnLoadState, DisableAbortButton; DisableMailSlotAlarm, WalkGlassOutState, DisableGlassNotDetectedAlarm; EnableAbortButton, RemoveGlassState, UnCompressSidePreLoadState, UnCompressTrailingEdgePreLoadsState, RetractTrailingEdgePinsState, GlassUnLoadingStateInitial
//CartDockedState: EnableLoadGlassOnGUI [IsDoorOpened], DisableLoadGlassOnGui
//GlassUnLoadedState: GlassUnLoadedStateInitial, CartUnDockedState
//DefectReviewState: RevisitState, DefectReviewStateInitial
//MacroScanState: IsMoreStripes, MacroScanStateInitial, ScanningState, MoveToStripeState
//InspectingState: InspectingStateInitial, DefectReviewState, MacroScanState
//Idle State: IsGlassLoaded?, GlassUnLoadedState, Idle StateInitial, GlassLoadedState
//ReadyState: StopAllMotion, InspectingState, ReadyStateInitial, Idle State
//SystemPoweredState: IsWarningOrCriticalAlarm, ReadyState, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: HomeAllAxisToIsWarningOrCriticalAlarmTransition From:HomeAllAxisState To:IsWarningOrCriticalAlarm Via:AllAxisHomedEvent Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarmToFaultedTransition From:IsWarningOrCriticalAlarm To:FaultedState Via: Action:none Guard:Yes
//External Transition: FaultWithGlassHeldToGlassLoadedTransition From:FaultWithGlassHeldState To:GlassLoadedState Via:ClearFaultsEvent Action:none Guard:IsNoWarningOrCriticalAlarms
//External Transition: FaultWithGlassLostToFaultAndOperatorClearsGlassTransition From:FaultWithGlassLostState To:FaultAndOperatorClearsGlassState Via:ClearFaultsEvent Action:PromptToClearGlassAndClearFaultsAgain; DisableGlassNotDetectedAlarm Guard:none
//External Transition: FaultAndOperatorClearsGlassToIsWarningOrCriticalAlarmTransition From:FaultAndOperatorClearsGlassState To:IsWarningOrCriticalAlarm Via:ClearFaultsEvent Action:MoveAllToLoadPos Guard:IsNoWarningOrCriticalAlarms
//External Transition: FaultWithGlassHeldToFaultWithGlassLostTransition From:FaultWithGlassHeldState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: InitialToIsSheetDetected2Transition From:Initial To:IsSheetDetected2? Via: Action:none Guard:none
//External Transition: AutoAlignGlassInitialToIsNTimesCompleteTransition From:AutoAlignGlassStateInitial To:IsNTimes Complete? Via: Action:none Guard:none
//External Transition: PromptToDockCartToIsDoorOpened3Transition From:PromptToDockCart To:IsDoorOpened3? Via:CartDockedEvent Action:none Guard:none
//External Transition: IsNTimesCompleteToClampSheetTransition From:IsNTimes Complete? To:ClampSheetState Via: Action:none Guard:Yes
//External Transition: ImagesSaveToGlassLoadedTransition From:ImagesSaveState To:GlassLoadedState Via:ImagesSavedEvent Action:EnableStartAndUnLoadButtonOnGui Guard:none
//External Transition: IsHomedToIsSheetDetectedTransition From:IsHomed? To:IsSheetDetected? Via: Action:none Guard:No
//External Transition: IsDoorOpenedToIsAutoLoadTransition From:IsDoorOpened? To:IsAutoLoad? Via: Action:none Guard:Yes
//External Transition: AbortingToIdleTransition From:AbortingState To:Idle State Via:AbortCompleteEvent Action:NotifyScanAbortAlarm Guard:none
//External Transition: IsAutoLoadToWaitForGlassToEnterTransition From:IsAutoLoad? To:WaitForGlassToEnterState Via: Action:none Guard:Yes
//External Transition: RemoveGlassToIdleTransition From:RemoveGlassState To:Idle State Via:GlassRemovedAtEntranceEvent Action:MoveAllToLoadPos; PromptCloseDoor; EnableGlassSizeEntry Guard:none
//External Transition: ReadyToFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: IsCartDockedToGlassUnLoadedTransition From:IsCartDocked? To:GlassUnLoadedState Via: Action:none Guard:No
//External Transition: StepInSideToStepInLeadingEdgeTransition From:StepInSideState To:StepInLeadingEdgeState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: DeployTrailingEdgePinsToAutoAlignGlassTransition From:DeployTrailingEdgePins To:AutoAlignGlassState Via:TrailingEdgePreLoadsExtendedEvent Action:none Guard:none
//External Transition: IsDoorOpened2ToAlignGlassTransition From:IsDoorOpened2? To:AlignGlassState Via: Action:none Guard:No
//External Transition: IsDoorOpened2ToCloseDoorToAlignTransition From:IsDoorOpened2? To:CloseDoorToAlignState Via: Action:none Guard:Yes
//External Transition: OperatorManualLoadToIsSafetyCircuitResetTransition From:OperatorManualLoadState To:IsSafetyCircuitReset? Via:GlassDetectedAtDamperEvent Action:none Guard:none
//External Transition: GlassLoadingFromCartToAutoLoadSeqTransition From:GlassLoadingFromCartState To:AutoLoadSeqState Via:CartLoadTimeoutEvent Action:none Guard:none
//External Transition: GlassLoadingFromCartToIsSafetyCircuitResetTransition From:GlassLoadingFromCartState To:IsSafetyCircuitReset? Via:GlassDetectedAtDamperEvent Action:none Guard:none
//External Transition: WaitForDoorOpenToLoadToIdleTransition From:WaitForDoorOpenToLoadState To:Idle State Via:AbortEvent Action:none Guard:none
//External Transition: WaitForDoorOpenToLoadToIdleTransition From:WaitForDoorOpenToLoadState To:Idle State Via:CartUnDockedEvent Action:none Guard:none
//External Transition: GlassLoadingInitialToMoveToLoadPosTransition From:GlassLoadingStateInitial To:MoveToLoadPosState Via: Action:none Guard:none
//External Transition: ClampSheetToIsWarningOrCriticalAlarm2Transition From:ClampSheetState To:IsWarningOrCriticalAlarm2? Via:MoveCompleteEvent Action:EnableGlassNotDetectedAlarm Guard:none
//External Transition: DelayForSpringToIsAnyPreLoadCompressedTransition From:DelayForSpring To:IsAnyPreLoadCompressed? Via:TimeOutEvent Action:none Guard:none
//External Transition: GlassUnLoadingToFaultWithGlassLostTransition From:GlassUnLoadingState To:FaultWithGlassLostState Via:MoveTimeOutEvent Action:none Guard:none
//External Transition: ResetToIsDoorOpened2Transition From:ResetState To:IsDoorOpened2? Via:SafetyCircuitResetEvent Action:none Guard:none
//External Transition: IsSafetyCircuitResetToResetTransition From:IsSafetyCircuitReset? To:ResetState Via: Action:none Guard:No
//External Transition: CartUnDockedToCartDockedTransition From:CartUnDockedState To:CartDockedState Via:CartDockedEvent Action:none Guard:!IsDoorOpen
//External Transition: MacroScanToFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:DoorNotClosed Action:none Guard:none
//External Transition: AlignGlassToFaultWithGlassLostTransition From:AlignGlassState To:FaultWithGlassLostState Via:DoorNotClosed Action:none Guard:none
//External Transition: GlassLoadedToFaultWithGlassLostTransition From:GlassLoadedState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: WaitForDoorOpenToLoadToIsAutoLoadTransition From:WaitForDoorOpenToLoadState To:IsAutoLoad? Via:DoorOpenEvent Action:none Guard:none
//External Transition: MoveToStripeToScanningTransition From:MoveToStripeState To:ScanningState Via:StripeScanReadyEvent Action:none Guard:none
//External Transition: DefectReviewToGlassUnLoadingTransition From:DefectReviewState To:GlassUnLoadingState Via:UnLoadEvent Action:none Guard:none
//External Transition: DefectReviewInitialToRevisitTransition From:DefectReviewStateInitial To:RevisitState Via: Action:none Guard:none
//External Transition: GlassLoadedToFaultWithGlassHeldTransition From:GlassLoadedState To:FaultWithGlassHeldState Via:Events (see Note for list) Action:none Guard:none
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:EStopEvent Action:none Guard:none
//External Transition: FaultWithNoGlassToIsWarningOrCriticalAlarmTransition From:FaultWithNoGlassState To:IsWarningOrCriticalAlarm Via:ClearFaultsEvent Action:none Guard:none
//External Transition: ReadyToFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:EStopEvent Action:none Guard:none
//External Transition: IdleInitialToIsGlassLoadedTransition From:Idle StateInitial To:IsGlassLoaded? Via: Action:none Guard:none
//External Transition: IsCartDockedToCartDockedTransition From:IsCartDocked? To:CartDockedState Via: Action:none Guard:Yes
//External Transition: ScanningToIsMoreStripesTransition From:ScanningState To:IsMoreStripes Via:StripeScanCompleteEvent Action:none Guard:none
//External Transition: GlassUnLoadedInitialToCartUnDockedTransition From:GlassUnLoadedStateInitial To:CartUnDockedState Via: Action:none Guard:none
//External Transition: IsMoreStripesToMoveToStripeTransition From:IsMoreStripes To:MoveToStripeState Via: Action:none Guard:Yes
//External Transition: MacroScanInitialToLaserAndPmtOnTransition From:MacroScanStateInitial To:LaserAndPmtOnState Via: Action:none Guard:none
//External Transition: InitialToSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: SystemPoweredInitialToInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: InitToInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarmToIsGlassLostDetectedTransition From:IsWarningOrCriticalAlarm To:IsGlassLostDetected? Via: Action:none Guard:No
//External Transition: InitAfterSettingsToIsWarningOrCriticalAlarmTransition From:InitAfterSettingsState To:IsWarningOrCriticalAlarm Via: Action:none Guard:none
//External Transition: IsMoreStripesToProcessingTransition From:IsMoreStripes To:ProcessingState Via: Action:none Guard:No
//External Transition: CartUnDockedToCartDockedTransition From:CartUnDockedState To:CartDockedState Via:CartDockedEvent Action:EnableLoadGlassOnGui Guard:IsDoorOpen
//External Transition: MoveXToUnLoadPosToIsCartDocked2Transition From:MoveXToUnLoadPosState To:IsCartDocked2? Via: Action:none Guard:none
//External Transition: IsNTimesCompleteToStepInSideTransition From:IsNTimes Complete? To:StepInSideState Via:No Action:none Guard:none
//External Transition: IsSafetyCircuitResetToIsDoorOpened2Transition From:IsSafetyCircuitReset? To:IsDoorOpened2? Via: Action:none Guard:Yes
//External Transition: CloseDoorToAlignToAlignGlassTransition From:CloseDoorToAlignState To:AlignGlassState Via:DoorCloseEvent Action:none Guard:none
//External Transition: IsValidGlassSizeToDeployTrailingEdgePinsTransition From:IsValidGlassSize? To:DeployTrailingEdgePins Via: Action:none Guard:Yes
//External Transition: IsDoorOpenedToWaitForDoorOpenToLoadTransition From:IsDoorOpened? To:WaitForDoorOpenToLoadState Via: Action:none Guard:No
//External Transition: GlassLoadedToGlassUnLoadingTransition From:GlassLoadedState To:GlassUnLoadingState Via:UnloadGlassEvent Action:none Guard:CartDocked
//External Transition: IsGlassLoadedToGlassLoadedTransition From:IsGlassLoaded? To:GlassLoadedState Via: Action:none Guard:Yes
//External Transition: CartDockedToCartUnDockedTransition From:CartDockedState To:CartUnDockedState Via:CartUnDockedEvent Action:none Guard:none
//External Transition: IsGlassLoadedToIsCartDockedTransition From:IsGlassLoaded? To:IsCartDocked? Via: Action:none Guard:No
//External Transition: WaitForGlassToEnterToGlassLoadingFromCartTransition From:WaitForGlassToEnterState To:GlassLoadingFromCartState Via:GlassDetectedAtEntranceEvent Action:EnableCartUnDockedAlarm Guard:none
//External Transition: WaitForGlassToEnterToIdleTransition From:WaitForGlassToEnterState To:Idle State Via:AbortEvent Action:none Guard:none
//External Transition: InspectingInitialToMacroScanTransition From:InspectingStateInitial To:MacroScanState Via: Action:none Guard:none
//External Transition: GlassLoadedToIsWarningOrCriticalAlarm3Transition From:GlassLoadedState To:IsWarningOrCriticalAlarm3? Via:StartScanEvent Action:none Guard:none
//External Transition: ReadyInitialToIdleTransition From:ReadyStateInitial To:Idle State Via: Action:none Guard:none
//External Transition: IsHomedToReadyTransition From:IsHomed? To:ReadyState Via: Action:none Guard:Yes
//External Transition: CartDockedToGlassLoadingTransition From:CartDockedState To:GlassLoadingState Via:LoadGlassEvent Action:MoveAllToLoadPos Guard:none
//External Transition: WaitForGlassToEnterToIdleTransition From:WaitForGlassToEnterState To:Idle State Via:CartUnDockedEvent Action:none Guard:none
//External Transition: GlassLoadingToFaultWithGlassLostTransition From:GlassLoadingState To:FaultWithGlassLostState Via:AbortEvent Action:NotifyLoadGlassAbortAlarm Guard:none
//External Transition: IsCartDocked2ToIsDoorOpened3Transition From:IsCartDocked2? To:IsDoorOpened3? Via: Action:none Guard:Yes
//External Transition: AutoLoadSeqToFaultWithGlassLostTransition From:AutoLoadSeqState To:FaultWithGlassLostState Via:AutoLoadTimeOutEvent Action:none Guard:none
//External Transition: RetractTrailingEdgePinsToWalkGlassOutTransition From:RetractTrailingEdgePinsState To:WalkGlassOutState Via:TrailingEdgePinsRetractedEvent Action:StopRetractTrailingEdgePinsTimer Guard:none
//External Transition: MAStageRetractToRemoveGlassTransition From:MAStageRetractState To:RemoveGlassState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: WalkGlassOutInitialToMoveXToWalkGlassOutStartPosTransition From:WalkGlassOutStateInitial To:MoveXToWalkGlassOutStartPosState Via: Action:none Guard:none
//External Transition: IsDoorOpened3ToOpenDoorTransition From:IsDoorOpened3? To:OpenDoorState Via: Action:none Guard:No
//External Transition: IsAutoLoadToOperatorManualLoadTransition From:IsAutoLoad? To:OperatorManualLoadState Via: Action:none Guard:No
//External Transition: RetractTrailingEdgePinsToFaultWithGlassLostTransition From:RetractTrailingEdgePinsState To:FaultWithGlassLostState Via:RetractTrailingEdgePinsTimeOutEvent Action:none Guard:none
//External Transition: IsAnyPreLoadCompressedToFaultWithGlassLostTransition From:IsAnyPreLoadCompressed? To:FaultWithGlassLostState Via: Action:CreatePreLoadCompressedDuringAlignAlarm Guard:Yes
//External Transition: IsScanSensorReadyToIsWindowClosedTransition From:IsScanSensorReady? To:IsWindowClosed? Via: Action:none Guard:Yes
//External Transition: IsWarningOrCriticalAlarm3ToIsScanSensorReadyTransition From:IsWarningOrCriticalAlarm3? To:IsScanSensorReady? Via: Action:none Guard:No
//External Transition: MacroScanToAbortingTransition From:MacroScanState To:AbortingState Via:AbortEvent Action:none Guard:none
//External Transition: LaserAndPmtOnToIsMoreStripesTransition From:LaserAndPmtOnState To:IsMoreStripes Via:LaserAndPmtOnEvent Action:none Guard:none
//External Transition: WaitForPreloadsUnCompressedToFaultWithGlassLostTransition From:WaitForPreloadsUnCompressedState To:FaultWithGlassLostState Via:TimoutWaitingForPreLoadUnCompressEvent Action:none Guard:none
//External Transition: IsCartDocked2ToPromptToDockCartTransition From:IsCartDocked2? To:PromptToDockCart Via: Action:none Guard:No
//External Transition: IsWarningOrCriticalAlarm2ToGlassLoadedTransition From:IsWarningOrCriticalAlarm2? To:GlassLoadedState Via: Action:none Guard:No
//External Transition: IsWarningOrCriticalAlarm2ToFaultWithGlassHeldTransition From:IsWarningOrCriticalAlarm2? To:FaultWithGlassHeldState Via: Action:none Guard:Yes
//External Transition: AlignGlassInitialToIsValidGlassSizeTransition From:AlignGlassStateInitial To:IsValidGlassSize? Via: Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarm3ToFaultWithGlassHeldTransition From:IsWarningOrCriticalAlarm3? To:FaultWithGlassHeldState Via: Action:none Guard:Yes
//External Transition: IsSheetDetectedToFaultedTransition From:IsSheetDetected? To:FaultedState Via: Action:none Guard:none
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:PanelOpenEvent Action:none Guard:none
//External Transition: ProcessingToGlassLoadedTransition From:ProcessingState To:GlassLoadedState Via:AbortEvent Action:none Guard:none
//External Transition: ImagesSaveToGlassLoadedTransition From:ImagesSaveState To:GlassLoadedState Via:AbortEvent Action:StopImageSave; EnableStartAndUnLoadButtonOnGui; Guard:none
//External Transition: ViewOfflineToIsGlassLoadedTransition From:ViewOfflineState To:IsGlassLoaded? Via:CommitEvent Action:none Guard:none
//External Transition: IdleToViewOfflineTransition From:Idle State To:ViewOfflineState Via:LoadOfflineFileEvent Action:none Guard:none
//External Transition: IsSaveNGImagesToGlassLoadedTransition From:IsSaveNGImages? To:GlassLoadedState Via: Action:EnableStartAndUnLoadButtonOnGui Guard:No
//External Transition: IsSaveNGImagesToImagesSaveTransition From:IsSaveNGImages? To:ImagesSaveState Via: Action:none Guard:Yes
//External Transition: AbortingToFaultWithGlassHeldTransition From:AbortingState To:FaultWithGlassHeldState Via:ScanSensorFault Action:none Guard:none
//External Transition: IsSheetDetectedToHomeAllAxisTransition From:IsSheetDetected? To:HomeAllAxisState Via: Action:none Guard:none
//External Transition: AbortingToFaultWithGlassHeldTransition From:AbortingState To:FaultWithGlassHeldState Via:AbortEvent Action:none Guard:none
//External Transition: ProcessingToDefectReviewTransition From:ProcessingState To:DefectReviewState Via:ProcessingCompleteEvent Action:none Guard:none
//External Transition: IdleToFaultWithGlassHeldTransition From:Idle State To:FaultWithGlassHeldState Via:NotifyScanSensorAlarm Action:none Guard:ScanSensorFaultEvent
//External Transition: WaitForPreloadsUnCompressedToRetractTrailingEdgePinsTransition From:WaitForPreloadsUnCompressedState To:RetractTrailingEdgePinsState Via:AllPreLoadsUnCompressedEvent Action:none Guard:none
//External Transition: WaitForWindowClosedToInspectingTransition From:WaitForWindowClosedState To:InspectingState Via:WindowClosedEvent Action:none Guard:none
//External Transition: IsScanSensorReadyToFaultWithGlassHeldTransition From:IsScanSensorReady? To:FaultWithGlassHeldState Via: Action:none Guard:No
//External Transition: IsWindowClosedToInspectingTransition From:IsWindowClosed? To:InspectingState Via: Action:none Guard:Yes
//External Transition: IsWindowClosedToWaitForWindowClosedTransition From:IsWindowClosed? To:WaitForWindowClosedState Via: Action:none Guard:No
//External Transition: IsAnyPreLoadCompressedToIsNTimesCompleteTransition From:IsAnyPreLoadCompressed? To:IsNTimes Complete? Via: Action:none Guard:No
//External Transition: RevisitToIsSaveNGImagesTransition From:RevisitState To:IsSaveNGImages? Via:CommitEvent Action:none Guard:none
//External Transition: IsValidGlassSizeToCorrectGlassSizeTransition From:IsValidGlassSize? To:CorrectGlassSizeState Via: Action:none Guard:No
//External Transition: MacroScanToFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:WindowNotClosedEvent Action:none Guard:IsNotAdminLogin
//External Transition: MacroScanToFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:LightOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:ScanSensorFaultEvent Action:NotifyScanSensorAlarm Guard:none
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:MotionFaultEvent Action:NotifyMotionAlarm Guard:none
//External Transition: IsGlassLostDetectedToFaultedTransition From:IsGlassLostDetected? To:FaultedState Via: Action:none Guard:Yes
//External Transition: IsGlassLostDetectedToIsHomedTransition From:IsGlassLostDetected? To:IsHomed? Via: Action:none Guard:No
//External Transition: IsSheetDetected2ToFaultWithGlassLostTransition From:IsSheetDetected2? To:FaultWithGlassLostState Via: Action:none Guard:Yes
//External Transition: IsDoorOpened3ToUnCompressSidePreLoadTransition From:IsDoorOpened3? To:UnCompressSidePreLoadState Via: Action:none Guard:Yes
//External Transition: CorrectGlassSizeToIsValidGlassSizeTransition From:CorrectGlassSizeState To:IsValidGlassSize? Via:SizeCorrectedEvent Action:none Guard:none
//External Transition: ReadyToFaultedTransition From:ReadyState To:FaultedState Via:CabinetOverTempEvent Action:none Guard:none
//External Transition: SystemPoweredToInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: OpenDoorToUnCompressSidePreLoadTransition From:OpenDoorState To:UnCompressSidePreLoadState Via:DoorOpenEvent Action:none Guard:none
//External Transition: PrepForUnLoadInitialToMoveXToUnLoadPosTransition From:PrepForUnLoadStateInitial To:MoveXToUnLoadPosState Via: Action:none Guard:none
//External Transition: GlassUnLoadingInitialToPrepForUnLoadTransition From:GlassUnLoadingStateInitial To:PrepForUnLoadState Via: Action:none Guard:none
//External Transition: GlassUnLoadingToFaultWithGlassLostTransition From:GlassUnLoadingState To:FaultWithGlassLostState Via:AbortEvent Action:NotifyUnLoadGlassAbortAlarm Guard:none
//External Transition: UnCompressTrailingEdgePreLoadsToWaitForPreloadsUnCompressedTransition From:UnCompressTrailingEdgePreLoadsState To:WaitForPreloadsUnCompressedState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:BypassSwitchOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: IsSheetDetected2ToFaultWithNoGlassTransition From:IsSheetDetected2? To:FaultWithNoGlassState Via: Action:none Guard:No
//External Transition: WalkGlassOutToMAStageRetractTransition From:WalkGlassOutState To:MAStageRetractState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: StepInLeadingEdgeToDelayForSpringTransition From:StepInLeadingEdgeState To:DelayForSpring Via:MoveCompleteEvent Action:none Guard:none
//External Transition: MoveToLoadPosToIdleTransition From:MoveToLoadPosState To:Idle State Via:CartUnDockedEvent Action:none Guard:none
//External Transition: MAStageExtendToFaultWithGlassLostTransition From:MAStageExtendState To:FaultWithGlassLostState Via:MoveCompleteEvent Action:none Guard:IsMAPreloadNotCompressed
//External Transition: WalkGlassOutToFaultWithGlassLostTransition From:WalkGlassOutState To:FaultWithGlassLostState Via:CartUnDockedEvent Action:none Guard:none
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:CabinetOverTempEvent Action:none Guard:none
//External Transition: ReadyToFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:BypassSwitchOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: ReadyToFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:MotionFaultEvent Action:NotifyMotionAlarm Guard:none
//External Transition: ReadyToFaultedTransition From:ReadyState To:FaultedState Via:PanelOpenEvent Action:none Guard:IsNotBypassed
//External Transition: ReadyToFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:ScanSensorFaultEvent Action:NotifyScanSensorAlarm Guard:none
//External Transition: MacroScanToFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:VacuumOffEvent Action:none Guard:none
//External Transition: MAStageExtendToWalkGlassOutTransition From:MAStageExtendState To:WalkGlassOutState Via:MoveCompleteEvent Action:none Guard:IsMAPreloadCompressed
//External Transition: MoveXToWalkGlassOutStartPosToMAStageExtendTransition From:MoveXToWalkGlassOutStartPosState To:MAStageExtendState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: AutoLoadSeqToIsSafetyCircuitResetTransition From:AutoLoadSeqState To:IsSafetyCircuitReset? Via:AutoLoadCompleteEvent Action:none Guard:none
//External Transition: UnCompressSidePreLoadToUnCompressTrailingEdgePreLoadsTransition From:UnCompressSidePreLoadState To:UnCompressTrailingEdgePreLoadsState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: AutoLoadSeqToOperatorManualLoadTransition From:AutoLoadSeqState To:OperatorManualLoadState Via:AutoLoadFailedEvent Action:none Guard:none
//External Transition: ReadyToFaultedTransition From:ReadyState To:FaultedState Via:VacuumOffEvent Action:none Guard:none
//External Transition: ReadyToFaultedTransition From:ReadyState To:FaultedState Via:AirOffEvent Action:none Guard:none
//External Transition: CorrectGlassSizeToFaultWithGlassLostTransition From:CorrectGlassSizeState To:FaultWithGlassLostState Via:AbortEvent Action:NotifyInvalidGlassSizeAlarm Guard:none
//External Transition: MoveToLoadPosToIsDoorOpenedTransition From:MoveToLoadPosState To:IsDoorOpened? Via:LoadPosMoveCompleteEvent Action:none Guard:none
//Internal Transition: GlassLoadedStateOnSensorCheckTOTransition. OwnedBy GlassLoadedState. Action CheckSensor
//Internal Transition: GlassUnLoadingStateOnGlassNotDetectedEventTransition. OwnedBy GlassUnLoadingState. Action 
//Internal Transition: ReadyStateOnFanOffEvent;Transition. OwnedBy ReadyState. Action CabinetOverTempEvent;
//Internal Transition: ReadyStateOnGlassNotDetectedEventTransition. OwnedBy ReadyState. Action 
//Internal Transition: IdleStateOnGlassNotDetectedEventTransition. OwnedBy Idle State. Action 
//Internal Transition: InspectingStateOnSheetJudgementCompleteEventTransition. OwnedBy InspectingState. Action EnableStartAndUnLoadButtons
//Internal Transition: IdleStateOnLoadStripeImageEventTransition. OwnedBy Idle State. Action m_oGuideSystem.LoadStripeImage
//Internal Transition: CartDockedStateOnDoorOpenEventTransition. OwnedBy CartDockedState. Action EnableLoadGlasssOnGui
//Internal Transition: InspectingStateOnStripeFeatureVectorCompleteEventTransition. OwnedBy InspectingState. Action m_oGuideSystem.LastFeatureVectorProvidedForStripe
//Internal Transition: ReadyStateOnScanSensorStateChangeEventTransition. OwnedBy ReadyState. Action LogScanSensorStateChange
//Internal Transition: ReadyStateOnScanProgressEventTransition. OwnedBy ReadyState. Action UpdateScanProgressOnGui
