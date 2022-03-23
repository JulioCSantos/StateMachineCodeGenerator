//ScanCompleteEvent
//InitCompleteEvent
//AllAxisHomedEvent
//EStopEvent
//StartEvent
//InitEvent
//MotionFaultEvent
//StartZHeightScanEvent
//ReportCompleteEvent
//ScanCompleteEvent
//DistSensorMeasFailEvent
//AbortEvent
//InitAfterSettingsCompleteEvent
//CommitEvent
//LoadEvent
//InterLockEvent
//LoadCompleteEvent
//ProcessingCompleteEvent
//ProcessingCompleteEvent
//AbortEvent
//ClearFaultsEvent

//AbortingState: AbortScan
//LoadGlassState: EnableLoadCompleteButton
//InspectingState: WriteReportState, AbortingState, ProcessingState, ScanningZHeightState, DefectReviewState, ScanningWaistState, DisableAbortButton, EnableAbortButton
//IdleState: EnableManualMotion, DisableManualMotion
//ReadyState: ReadyStateInitial
//SystemPoweredState: IsCriticalAlarm_, ReadyState, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: ScanningWaistTOProcessingTransition From:ScanningWaistState To:ProcessingState Via:ScanCompleteEvent Action:none Guard:none
//External Transition: IsHomed_TOHomeAllAxisTransition From:IsHomed_ To:HomeAllAxisState Via: Action:none Guard:No
//External Transition: InitTOInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: HomeAllAxisTOIsCriticalAlarm_Transition From:HomeAllAxisState To:IsCriticalAlarm_ Via:AllAxisHomedEvent Action:none Guard:none
//External Transition: ReadyInitialTOIdleTransition From:ReadyStateInitial To:IdleState Via: Action:none Guard:none
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:EStopEvent Action:none Guard:none
//External Transition: IdleTOScanningWaistTransition From:IdleState To:ScanningWaistState Via:StartEvent Action:none Guard:none
//External Transition: SystemPoweredInitialTOInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: InitialTOSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: SystemPoweredTOInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:MotionFaultEvent Action:NotifyMotionFaultAlarm Guard:none
//External Transition: IdleTOScanningZHeightTransition From:IdleState To:ScanningZHeightState Via:StartZHeightScanEvent Action:none Guard:none
//External Transition: IsHomed_TOReadyTransition From:IsHomed_ To:ReadyState Via: Action:none Guard:Yes
//External Transition: IsCriticalAlarm_TOIsHomed_Transition From:IsCriticalAlarm_ To:IsHomed_ Via: Action:none Guard:none
//External Transition: WriteReportTOIdleTransition From:WriteReportState To:IdleState Via:ReportCompleteEvent Action:none Guard:none
//External Transition: ScanningZHeightTOIdleTransition From:ScanningZHeightState To:IdleState Via:ScanCompleteEvent Action:WriteTileHeightFileAsync Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:DistSensorMeasFailEvent Action:NotifyDistSensorMeasFailAlarm Guard:none
//External Transition: InspectingTOAbortingTransition From:InspectingState To:AbortingState Via:AbortEvent Action:none Guard:none
//External Transition: InitAfterSettingsTOIsCriticalAlarm_Transition From:InitAfterSettingsState To:IsCriticalAlarm_ Via:InitAfterSettingsCompleteEvent Action:none Guard:none
//External Transition: IsCriticalAlarm_TOFaultedTransition From:IsCriticalAlarm_ To:FaultedState Via: Action:none Guard:Yes
//External Transition: DefectReviewTOWriteReportTransition From:DefectReviewState To:WriteReportState Via:CommitEvent Action:WriteMeasResultsReportAsync Guard:none
//External Transition: IdleTOLoadGlassTransition From:IdleState To:LoadGlassState Via:LoadEvent Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:InterLockEvent Action:NotifyInterlockFaultEvent Guard:none
//External Transition: LoadGlassTOIdleTransition From:LoadGlassState To:IdleState Via:LoadCompleteEvent Action:none Guard:none
//External Transition: AbortingTOIdleTransition From:AbortingState To:IdleState Via:ProcessingCompleteEvent Action:none Guard:none
//External Transition: ProcessingTODefectReviewTransition From:ProcessingState To:DefectReviewState Via:ProcessingCompleteEvent Action:none Guard:none
//External Transition: DefectReviewTOIdleTransition From:DefectReviewState To:IdleState Via:AbortEvent Action:none Guard:none
//External Transition: FaultedTOIsCriticalAlarm_Transition From:FaultedState To:IsCriticalAlarm_ Via:ClearFaultsEvent Action:none Guard:none
//Internal Transition: ReadyStateOnInterLockEventTransition. OwnedBy ReadyState. Action   Guard:none
//Internal Transition: ReadyStateOnmissingeventTransition. OwnedBy ReadyState. Action   Guard:none
