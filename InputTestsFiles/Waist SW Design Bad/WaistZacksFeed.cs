//ClearFaultsEvent
//StartZHeightScanEvent
//ReMatchingCompleteEvent
//ReadCadFileCompleteEvent
//ReMatchCadFileEvent
//InvalidCadFileEvent
//InitEvent
//ScanCompleteEvent
//StartEvent
//EStopEvent
//AllAxisHomedEvent
//InitCompleteEvent
//InvalidResultFileEvent
//ProcessingCompleteEvent
//AbortEvent
//ReadResultFileCompleteEvent
//LoadResultsFileEvent
//AbortEvent
//MoveToParkCompleteEvent
//AbortEvent
//ScanCompleteEvent
//ProcessingCompleteEvent
//LoadCompleteEvent
//LoadEvent
//CommitEvent
//InitAfterSettingsCompleteEvent
//AbortEvent
//ReportCompleteEvent

//ReadyToScanOrLoadGlassState: EnableManualMotion, DisableManualMotion
//MovingToParkState: MoveToParkPositionAsync
//AbortScanState: _DisableAbortButton
//ProcessingState: DisableAbortButton
//LoadGlassState: DisableLoadCompleteButton, EnableLoadCompleteButton
//DefectReviewState: EnableCommitButton, DisableCommitButton
//ScanningWaistState: EnableTextBoxesInSetupScreen
//HomeAllAxisState: HomeAllAxis
//InspectingState: ReMatchingState, ReadCadFileState, ReadResultsFileState, WriteReportState, AbortScanState, ProcessingState, ScanningZHeightState, DefectReviewState, ScanningWaistState, DisableAbortButton, EnableAbortButton
//IdleState: IdleStateInitial, ReadyToScanOrLoadGlassState, MovingToParkState
//SystemPoweredState: IsCriticalAlarm_, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: InitialTOSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: FaultedTOIsCriticalAlarm_Transition From:FaultedState To:IsCriticalAlarm_ Via:ClearFaultsEvent Action:none Guard:none
//External Transition: ReadyToScanOrLoadGlassTOScanningZHeightTransition From:ReadyToScanOrLoadGlassState To:ScanningZHeightState Via:StartZHeightScanEvent Action:none Guard:none
//External Transition: ReMatchingTODefectReviewTransition From:ReMatchingState To:DefectReviewState Via:ReMatchingCompleteEvent Action:none Guard:none
//External Transition: ReadCadFileTOReMatchingTransition From:ReadCadFileState To:ReMatchingState Via:ReadCadFileCompleteEvent Action:none Guard:none
//External Transition: DefectReviewTOReadCadFileTransition From:DefectReviewState To:ReadCadFileState Via:ReMatchCadFileEvent Action:none Guard:none
//External Transition: ReadCadFileTODefectReviewTransition From:ReadCadFileState To:DefectReviewState Via:InvalidCadFileEvent Action:none Guard:none
//External Transition: IsHomed_TOHomeAllAxisTransition From:IsHomed_ To:HomeAllAxisState Via: Action:none Guard:No
//External Transition: SystemPoweredTOInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: ScanningZHeightTOReadyToScanOrLoadGlassTransition From:ScanningZHeightState To:ReadyToScanOrLoadGlassState Via:ScanCompleteEvent Action:WriteTileHeightFileAsync Guard:none
//External Transition: SystemPoweredInitialTOInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: ReadyToScanOrLoadGlassTOScanningWaistTransition From:ReadyToScanOrLoadGlassState To:ScanningWaistState Via:StartEvent Action:none Guard:none
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:EStopEvent Action:none Guard:none
//External Transition: InitialTOIdleTransition From:Initial To:IdleState Via: Action:none Guard:none
//External Transition: HomeAllAxisTOIsCriticalAlarm_Transition From:HomeAllAxisState To:IsCriticalAlarm_ Via:AllAxisHomedEvent Action:none Guard:none
//External Transition: InitTOInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: ReadResultsFileTOReadyToScanOrLoadGlassTransition From:ReadResultsFileState To:ReadyToScanOrLoadGlassState Via:InvalidResultFileEvent Action:none Guard:none
//External Transition: ProcessingTODefectReviewTransition From:ProcessingState To:DefectReviewState Via:ProcessingCompleteEvent Action:none Guard:none
//External Transition: ReadResultsFileTOReadyToScanOrLoadGlassTransition From:ReadResultsFileState To:ReadyToScanOrLoadGlassState Via:AbortEvent Action:none Guard:none
//External Transition: ReadResultsFileTOProcessingTransition From:ReadResultsFileState To:ProcessingState Via:ReadResultFileCompleteEvent Action:none Guard:none
//External Transition: ReadyToScanOrLoadGlassTOReadResultsFileTransition From:ReadyToScanOrLoadGlassState To:ReadResultsFileState Via:LoadResultsFileEvent Action:none Guard:none
//External Transition: ScanningWaistTOAbortScanTransition From:ScanningWaistState To:AbortScanState Via:AbortEvent Action:AbortScanningAsync Guard:none
//External Transition: MovingToParkTOReadyToScanOrLoadGlassTransition From:MovingToParkState To:ReadyToScanOrLoadGlassState Via:MoveToParkCompleteEvent Action:none Guard:none
//External Transition: IdleInitialTOMovingToParkTransition From:IdleStateInitial To:MovingToParkState Via: Action:none Guard:none
//External Transition: IsCriticalAlarm_TOIsHomed_Transition From:IsCriticalAlarm_ To:IsHomed_ Via: Action:none Guard:none
//External Transition: DefectReviewTOWriteReportTransition From:DefectReviewState To:WriteReportState Via:AbortEvent Action:WriteMeasResultsReportNoDbAsync Guard:none
//External Transition: ScanningWaistTOProcessingTransition From:ScanningWaistState To:ProcessingState Via:ScanCompleteEvent Action:EndProcessingAsync Guard:none
//External Transition: AbortScanTOWriteReportTransition From:AbortScanState To:WriteReportState Via:ProcessingCompleteEvent Action:WriteMeasResultsReportNoDbAsync Guard:none
//External Transition: LoadGlassTOReadyToScanOrLoadGlassTransition From:LoadGlassState To:ReadyToScanOrLoadGlassState Via:LoadCompleteEvent Action:none Guard:none
//External Transition: ReadyToScanOrLoadGlassTOLoadGlassTransition From:ReadyToScanOrLoadGlassState To:LoadGlassState Via:LoadEvent Action:none Guard:none
//External Transition: DefectReviewTOWriteReportTransition From:DefectReviewState To:WriteReportState Via:CommitEvent Action:WriteMeasResultsReportAsync Guard:none
//External Transition: IsCriticalAlarm_TOFaultedTransition From:IsCriticalAlarm_ To:FaultedState Via: Action:none Guard:Yes
//External Transition: InitAfterSettingsTOIsCriticalAlarm_Transition From:InitAfterSettingsState To:IsCriticalAlarm_ Via:InitAfterSettingsCompleteEvent Action:none Guard:none
//External Transition: ReadCadFileTODefectReviewTransition From:ReadCadFileState To:DefectReviewState Via:AbortEvent Action:none Guard:none
//External Transition: WriteReportTOIdleTransition From:WriteReportState To:IdleState Via:ReportCompleteEvent Action:none Guard:none
//Internal Transition: SystemPoweredStateOnmissingeventTransition. OwnedBy SystemPoweredState. Action   Guard:none
//Internal Transition: SystemPoweredStateOnmissingeventTransition. OwnedBy SystemPoweredState. Action   Guard:none
