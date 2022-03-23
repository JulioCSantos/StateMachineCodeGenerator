//StartManualScanEvent
//InitEvent
//AbortEvent
//AbortEvent
//DoorClosedEvent
//EStopOrInterLockEvent
//FiducialNotDetectedEvent
//AllAxisHomedEvent
//StartEvent
//UnloadWaferEvent
//MotionFaultEvent
//InitCompleteEvent
//ClearFaultsEvent
//MoveCompleteEvent
//CameraFaultEvent
//MoveCompleteEvent
//CalWaferCompleteEvent
//AbortCompleteEvent
//CalWaferPosEvent
//MoveCompleteEvent
//StartAutoScanEvent
//FiducialAcquiredEvent
//AbortEvent
//LoadWaferEvent
//MotionFaultEvent
//BypassSwitchOnEvent
//StartEvent
//DoorClosedEvent
//FiducialAcquiredEvent
//MoveCompleteEvent
//MoveCompleteEvent
//ScanCompleteEvent
//AbortEvent
//Abort Event

//AcquireFid3PosState: AcquireFid3Pos
//MoveToFiducial3: StartXYMoveToFid3Async
//AcquireFid2PosState: AcquireFid2Pos
//MoveToFiducial2: StartXYMoveToFid2Async
//AcquireFid1PosState: AcquireFid1Pos
//MoveToFiducial1: StartXYMoveToFid1Async
//MoveZState: MoveZAxisIfNewPosAsync
//CalibrateWaferPosState: PerformWaferCalibrationState, IsAutoSeq_, AcquireFid3PosState, MoveToFiducial3, AcquireFid2PosState, MoveToFiducial2, AcquireFid1PosState, MoveToFiducial1, MoveZState, CalibrateWaferPosStateInitial
//WaitForWaferLoadState: EnableStartBut___true
//MoveToLoadPosState: MoveToLoadPos
//HomeAllAxisState: HomeAllAxis
//LoadWaferState: WaitForWaferLoadState, WaitForDoorClosedState, IsDoorClosed_, MoveToLoadPosState, LoadWaferStateInitial
//UnloadWaferState: UnloadWaferStateInitial, WaitForWaferUnloadState, WaitForDoorClosedState, IsDoorClosed_, MoveToLoadPosState
//ProcessingState: ProcessingStateInitial, ScanState
//Idle_State: IsWaferLoaded_, WaferUnLoadedState, Idle_StateInitial, WaferLoadedState
//ReadyState: StopAllMotion, ProcessingState, ReadyStateInitial, Idle_State
//SystemPoweredState: IsWarningOrCriticalAlarm, ReadyState, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: WaferLoadedTOIsWarningOrCriticalAlarm_Transition From:WaferLoadedState To:IsWarningOrCriticalAlarm_ Via:StartManualScanEvent Action:SetManualScanParams Guard:none
//External Transition: IsWarningOrCriticalAlarmTOFaultedTransition From:IsWarningOrCriticalAlarm To:FaultedState Via: Action:none Guard:Yes
//External Transition: SystemPoweredTOInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:AbortEvent Action:none Guard:none
//External Transition: LoadWaferTOWaferUnLoadedTransition From:LoadWaferState To:WaferUnLoadedState Via:AbortEvent Action:none Guard:none
//External Transition: UnloadWaferInitialTOIsDoorClosed_Transition From:UnloadWaferStateInitial To:IsDoorClosed_ Via: Action:none Guard:none
//External Transition: WaitForDoorClosedTOMoveToLoadPosTransition From:WaitForDoorClosedState To:MoveToLoadPosState Via:DoorClosedEvent Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:EStopOrInterLockEvent Action:NotifyInterlockOrEStopAlarm Guard:none
//External Transition: CalibrateWaferPosTOFaultedTransition From:CalibrateWaferPosState To:FaultedState Via:FiducialNotDetectedEvent Action:none Guard:none
//External Transition: LoadWaferInitialTOIsDoorClosed_Transition From:LoadWaferStateInitial To:IsDoorClosed_ Via: Action:none Guard:none
//External Transition: HomeAllAxisTOIsWarningOrCriticalAlarmTransition From:HomeAllAxisState To:IsWarningOrCriticalAlarm Via:AllAxisHomedEvent Action:none Guard:none
//External Transition: WaitForWaferUnloadTOWaferUnLoadedTransition From:WaitForWaferUnloadState To:WaferUnLoadedState Via:StartEvent Action:none Guard:none
//External Transition: WaferLoadedTOUnloadWaferTransition From:WaferLoadedState To:UnloadWaferState Via:UnloadWaferEvent Action:none Guard:none
//External Transition: IsDoorClosed_TOWaitForDoorClosedTransition From:IsDoorClosed_ To:WaitForDoorClosedState Via: Action:none Guard:No
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:MotionFaultEvent Action:NotifyMotionAlarm Guard:none
//External Transition: IsDoorClosed_TOMoveToLoadPosTransition From:IsDoorClosed_ To:MoveToLoadPosState Via: Action:none Guard:Yes
//External Transition: InitAfterSettingsTOIsWarningOrCriticalAlarmTransition From:InitAfterSettingsState To:IsWarningOrCriticalAlarm Via: Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarmTOIsHomed_Transition From:IsWarningOrCriticalAlarm To:IsHomed_ Via: Action:none Guard:No
//External Transition: InitTOInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: SystemPoweredInitialTOInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: InitialTOSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: IsDoorClosed_TOMoveToLoadPosTransition From:IsDoorClosed_ To:MoveToLoadPosState Via: Action:none Guard:Yes
//External Transition: FaultedTOIsWarningOrCriticalAlarmTransition From:FaultedState To:IsWarningOrCriticalAlarm Via:ClearFaultsEvent Action:none Guard:none
//External Transition: ProcessingInitialTOCalibrateWaferPosTransition From:ProcessingStateInitial To:CalibrateWaferPosState Via: Action:none Guard:none
//External Transition: MoveToLoadPosTOWaitForWaferUnloadTransition From:MoveToLoadPosState To:WaitForWaferUnloadState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IsWaferLoaded_TOWaferLoadedTransition From:IsWaferLoaded_ To:WaferLoadedState Via: Action:none Guard:Yes
//External Transition: IsWaferLoaded_TOWaferUnLoadedTransition From:IsWaferLoaded_ To:WaferUnLoadedState Via: Action:none Guard:No
//External Transition: IsHomed_TOReadyTransition From:IsHomed_ To:ReadyState Via: Action:none Guard:Yes
//External Transition: ReadyInitialTOIdle_Transition From:ReadyStateInitial To:Idle_State Via: Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:CameraFaultEvent Action:NotifyCameraAlarm Guard:none
//External Transition: Idle_InitialTOIsWaferLoaded_Transition From:Idle_StateInitial To:IsWaferLoaded_ Via: Action:none Guard:none
//External Transition: MoveToFiducial2TOAcquireFid2PosTransition From:MoveToFiducial2 To:AcquireFid2PosState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: PerformWaferCalibrationTOIsAutoSeq_Transition From:PerformWaferCalibrationState To:IsAutoSeq_ Via:CalWaferCompleteEvent Action:none Guard:none
//External Transition: CalibrateWaferPosInitialTOPerformWaferCalibrationTransition From:CalibrateWaferPosStateInitial To:PerformWaferCalibrationState Via: Action:none Guard:none
//External Transition: AbortingScanTOIdle_Transition From:AbortingScanState To:Idle_State Via:AbortCompleteEvent Action:none Guard:none
//External Transition: WaferLoadedTOProcessingTransition From:WaferLoadedState To:ProcessingState Via:CalWaferPosEvent Action:IsAutoSeq___false Guard:none
//External Transition: IsAutoSeq_TOIdle_Transition From:IsAutoSeq_ To:Idle_State Via: Action:none Guard:No
//External Transition: MoveToFiducial1TOAcquireFid1PosTransition From:MoveToFiducial1 To:AcquireFid1PosState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: MoveZTOMoveToFiducial1Transition From:MoveZState To:MoveToFiducial1 Via: Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarm_TOFaultedTransition From:IsWarningOrCriticalAlarm_ To:FaultedState Via: Action:none Guard:Yes
//External Transition: IsWarningOrCriticalAlarm_TOProcessingTransition From:IsWarningOrCriticalAlarm_ To:ProcessingState Via: Action:none Guard:No
//External Transition: WaferLoadedTOIsWarningOrCriticalAlarm_Transition From:WaferLoadedState To:IsWarningOrCriticalAlarm_ Via:StartAutoScanEvent Action:SetAutoScanParams Guard:none
//External Transition: IsAutoSeq_TOScanTransition From:IsAutoSeq_ To:ScanState Via: Action:none Guard:Yes
//External Transition: AcquireFid2PosTOMoveToFiducial3Transition From:AcquireFid2PosState To:MoveToFiducial3 Via:FiducialAcquiredEvent Action:none Guard:none
//External Transition: UnloadWaferTOIdle_Transition From:UnloadWaferState To:Idle_State Via:AbortEvent Action:none Guard:none
//External Transition: WaferUnLoadedTOLoadWaferTransition From:WaferUnLoadedState To:LoadWaferState Via:LoadWaferEvent Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:MotionFaultEvent Action:NotifyMotionAlarm Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:BypassSwitchOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: IsHomed_TOHomeAllAxisTransition From:IsHomed_ To:HomeAllAxisState Via: Action:none Guard:none
//External Transition: WaitForWaferLoadTOWaferLoadedTransition From:WaitForWaferLoadState To:WaferLoadedState Via:StartEvent Action:none Guard:none
//External Transition: WaitForDoorClosedTOMoveToLoadPosTransition From:WaitForDoorClosedState To:MoveToLoadPosState Via:DoorClosedEvent Action:none Guard:none
//External Transition: AcquireFid1PosTOMoveToFiducial2Transition From:AcquireFid1PosState To:MoveToFiducial2 Via:FiducialAcquiredEvent Action:none Guard:none
//External Transition: MoveToLoadPosTOWaitForWaferLoadTransition From:MoveToLoadPosState To:WaitForWaferLoadState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: MoveToFiducial3TOAcquireFid3PosTransition From:MoveToFiducial3 To:AcquireFid3PosState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarm_TOFaultedTransition From:IsWarningOrCriticalAlarm_ To:FaultedState Via: Action:none Guard:Yes
//External Transition: IsWarningOrCriticalAlarm_TOScanTransition From:IsWarningOrCriticalAlarm_ To:ScanState Via: Action:none Guard:No
//External Transition: ScanTOIdle_Transition From:ScanState To:Idle_State Via:ScanCompleteEvent Action:none Guard:none
//External Transition: ScanTOAbortingScanTransition From:ScanState To:AbortingScanState Via:AbortEvent Action:none Guard:none
//External Transition: PerformWaferCalibrationTOIdle_Transition From:PerformWaferCalibrationState To:Idle_State Via:Abort Event Action:none Guard:none
//External Transition: IsDoorClosed_TOWaitForDoorClosedTransition From:IsDoorClosed_ To:WaitForDoorClosedState Via: Action:none Guard:No
//Internal Transition: CalibrateWaferPosStateOnmissingeventTransition. OwnedBy CalibrateWaferPosState. Action   Guard:none
//Internal Transition: WaitForWaferUnloadStateOnEStopOrInterlockEventTransition. OwnedBy WaitForWaferUnloadState. Action   Guard:none
//Internal Transition: WaitForWaferLoadStateOnEStopOrInterlockEventTransition. OwnedBy WaitForWaferLoadState. Action   Guard:none
//Internal Transition: SystemPoweredStateOnmissingeventTransition. OwnedBy SystemPoweredState. Action   Guard:none
//Internal Transition: ProcessingStateOnScanProgressEventTransition. OwnedBy ProcessingState. Action UpdateScanProgressOnGui  Guard:none
