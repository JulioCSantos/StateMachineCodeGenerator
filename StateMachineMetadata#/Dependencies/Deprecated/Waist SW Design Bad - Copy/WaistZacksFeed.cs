//InitEvent
//ClearFaultsEvent
//StartZHeightScanEvent
//ReMatchingCompleteEvent
//ReadCadFileCompleteEvent
//ReMatchCadFileEvent
//ScanCompleteEvent
//InvalidResultFileEvent
//ScanCompleteEvent
//StartEvent
//EStopEvent
//AllAxisHomedEvent
//InitCompleteEvent
//InvalidCadFileEvent
//AbortEvent
//AbortEvent
//AbortEvent
//ReadResultFileCompleteEvent
//LoadResultsFileEvent
//AbortEvent
//MoveToParkCompleteEvent
//ReportCompleteEvent
//ProcessingCompleteEvent
//ProcessingCompleteEvent
//LoadCompleteEvent
//LoadEvent
//CommitEvent
//InitAfterSettingsCompleteEvent

//ReadyToScanOrLoadGlassState: EnableManualMotion; EnableLoadButton; EnableStartButton; EnableZHeightScanButton; EnableLoadFileButton;DisableTextBoxesInSetupScreen, DisableManualMotion; DisableLoadGlassButton; DisableStartButton; DisableZHeightScanButton;DisableLoadFileButton
//MovingToParkState: MoveToParkPositionAsync
//AbortScanState:  DisableAbortButton;
//ProcessingState: DisableAbortButton;
//LoadGlassState: DisableLoadCompleteButton, EnableLoadCompleteButton
//DefectReviewState: EnableCommitButton; EnableAbortButton;EnableReMatchButton, DisableCommitButton; DisableAbortButton;DisableReMatchButton
//ScanningWaistState: EnableTextBoxesInSetupScreen; StartScanAsynch
//HomeAllAxisState: HomeAllAxis
//InspectingState: ReMatchingState, ReadCadFileState, ReadResultsFileState, WriteReportState, AbortScanState, ProcessingState, ScanningZHeightState, DefectReviewState, ScanningWaistState, DisableAbortButton; , EnableAbortButton; DisableStartButton
//IdleState: IdleStateInitial, ReadyToScanOrLoadGlassState, MovingToParkState
//SystemPoweredState: IsCriticalAlarm?, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: SystemPoweredToInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: IsHomedToHomeAllAxisTransition From:IsHomed? To:HomeAllAxisState Via: Action:none Guard:No
//External Transition: FaultedToIsCriticalAlarmTransition From:FaultedState To:IsCriticalAlarm? Via:ClearFaultsEvent Action:none Guard:none
//External Transition: ReadyToScanOrLoadGlassToScanningZHeightTransition From:ReadyToScanOrLoadGlassState To:ScanningZHeightState Via:StartZHeightScanEvent Action:none Guard:none
//External Transition: ReMatchingToDefectReviewTransition From:ReMatchingState To:DefectReviewState Via:ReMatchingCompleteEvent Action:none Guard:none
//External Transition: ReadCadFileToReMatchingTransition From:ReadCadFileState To:ReMatchingState Via:ReadCadFileCompleteEvent Action:none Guard:none
//External Transition: DefectReviewToReadCadFileTransition From:DefectReviewState To:ReadCadFileState Via:ReMatchCadFileEvent Action:none Guard:none
//External Transition: ScanningWaistToProcessingTransition From:ScanningWaistState To:ProcessingState Via:ScanCompleteEvent Action:EndProcessingAsync Guard:none
//External Transition: ReadResultsFileToReadyToScanOrLoadGlassTransition From:ReadResultsFileState To:ReadyToScanOrLoadGlassState Via:InvalidResultFileEvent Action:none Guard:none
//External Transition: ScanningZHeightToReadyToScanOrLoadGlassTransition From:ScanningZHeightState To:ReadyToScanOrLoadGlassState Via:ScanCompleteEvent Action:WriteTileHeightFileAsync Guard:none
//External Transition: InitialToSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: SystemPoweredInitialToInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: ReadyToScanOrLoadGlassToScanningWaistTransition From:ReadyToScanOrLoadGlassState To:ScanningWaistState Via:StartEvent Action:none Guard:none
//External Transition: SystemPoweredToFaultedTransition From:SystemPoweredState To:FaultedState Via:EStopEvent Action:none Guard:none
//External Transition: InitialToIdleTransition From:Initial To:IdleState Via: Action:none Guard:none
//External Transition: HomeAllAxisToIsCriticalAlarmTransition From:HomeAllAxisState To:IsCriticalAlarm? Via:AllAxisHomedEvent Action:none Guard:none
//External Transition: InitToInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: ReadCadFileToDefectReviewTransition From:ReadCadFileState To:DefectReviewState Via:InvalidCadFileEvent Action:none Guard:none
//External Transition: DefectReviewToWriteReportTransition From:DefectReviewState To:WriteReportState Via:AbortEvent Action:WriteMeasResultsReportNoDbAsync Guard:none
//External Transition: ReadCadFileToDefectReviewTransition From:ReadCadFileState To:DefectReviewState Via:AbortEvent Action:none Guard:none
//External Transition: ReadResultsFileToReadyToScanOrLoadGlassTransition From:ReadResultsFileState To:ReadyToScanOrLoadGlassState Via:AbortEvent Action:none Guard:none
//External Transition: ReadResultsFileToProcessingTransition From:ReadResultsFileState To:ProcessingState Via:ReadResultFileCompleteEvent Action:none Guard:none
//External Transition: ReadyToScanOrLoadGlassToReadResultsFileTransition From:ReadyToScanOrLoadGlassState To:ReadResultsFileState Via:LoadResultsFileEvent Action:none Guard:none
//External Transition: ScanningWaistToAbortScanTransition From:ScanningWaistState To:AbortScanState Via:AbortEvent Action:AbortScanningAsync Guard:none
//External Transition: MovingToParkToReadyToScanOrLoadGlassTransition From:MovingToParkState To:ReadyToScanOrLoadGlassState Via:MoveToParkCompleteEvent Action:none Guard:none
//External Transition: IsCriticalAlarmToIsHomedTransition From:IsCriticalAlarm? To:IsHomed? Via: Action:none Guard:none
//External Transition: WriteReportToIdleTransition From:WriteReportState To:IdleState Via:ReportCompleteEvent Action:none Guard:none
//External Transition: InitialToIdleTransition From:Initial To:IdleState Via: Action:none Guard:none
//External Transition: ProcessingToDefectReviewTransition From:ProcessingState To:DefectReviewState Via:ProcessingCompleteEvent Action:none Guard:none
//External Transition: AbortScanToWriteReportTransition From:AbortScanState To:WriteReportState Via:ProcessingCompleteEvent Action:WriteMeasResultsReportNoDbAsync Guard:none
//External Transition: LoadGlassToReadyToScanOrLoadGlassTransition From:LoadGlassState To:ReadyToScanOrLoadGlassState Via:LoadCompleteEvent Action:none Guard:none
//External Transition: ReadyToScanOrLoadGlassToLoadGlassTransition From:ReadyToScanOrLoadGlassState To:LoadGlassState Via:LoadEvent Action:none Guard:none
//External Transition: DefectReviewToWriteReportTransition From:DefectReviewState To:WriteReportState Via:CommitEvent Action:WriteMeasResultsReportAsync Guard:none
//External Transition: IsCriticalAlarmToFaultedTransition From:IsCriticalAlarm? To:FaultedState Via: Action:none Guard:Yes
//External Transition: InitAfterSettingsToIsCriticalAlarmTransition From:InitAfterSettingsState To:IsCriticalAlarm? Via:InitAfterSettingsCompleteEvent Action:none Guard:none
//External Transition: IdleInitialToMovingToParkTransition From:IdleStateInitial To:MovingToParkState Via: Action:none Guard:none
//Internal Transition: SystemPoweredStateOnmissingeventTransition. OwnedBy SystemPoweredState. Action 
//Internal Transition: SystemPoweredStateOnWhatTransition. OwnedBy SystemPoweredState. Action type
