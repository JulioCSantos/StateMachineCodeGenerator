//ProcessingCompleteEvent
//LocateCompleteEvent
//CalCompleteEvent
//ProcessingCompleteEvent
//MoveCompleteEvent
//LoadEvent
//StartEvent
//CalEvent
//InitCompleteEvent
//InitEvent
//EStopEvent
//ClearFaultsEvent
//AllAxisHomedEvent
//AbortEvent
//AbortEvent
//CommitEvent
//InitAfterSettingsCompleteEvent
//UnLoadEvent
//DoorOpenEvent
//AbortEvent
//AbortCompleteEvent
//FindFocusEvent
//FindFocusCompleteEvent
//AbortEvent
//MoveCompleteEvent
//CompleteEvent
//LocateDeviceEvent
//AbortEvent
//MoveCompleteEvent
//MotionFaultEvent
//StartEvent
//LaserFaultEvent
//SaveResultsCompleteEvent
//MoveCompleteEvent
//CompleteEvent
//MoveCompleteEvent
//DoorOpenEvent

//LocatingState: LocateDeviceAsync
//RepositionState: DisableCompleteButton, EnableCompleteButton
//MoveToRePositionState: MoveToLoadPos
//FindFocusState: FindFocusAsync
//AbortingState: MoveToRunPosAsync
//MoveToRunPosState: DisableCompleteButton, MoveToLoadPos
//MoveToLoadPosState: DisableCompleteButton, MoveToLoadPos
//AutoCalState: DisableCalButton, EnableCalButton
//AbortProcessingState: DisableAbortButton
//MoveToNextLocState: MoveToNextLoc
//ProcessingCompleteState: EnableCommitButton, DisableCommitButton
//LaserProcessingState: LaserProcessingAsync
//LocateDeviceState: MoveToRunPos2State, LocateDevice_InitialState, LocatingState, RepositionState, MoveToRePositionState, IsLocated4
//ProcessingState: MoreLocations, Processing_InitialState, SaveResultsState, AbortProcessingState, MoveToNextLocState, ProcessingCompleteState, LaserProcessingState
//ActiveState: IsLocated3, FindFocusState, AbortingState, AutoCalState, LocateDeviceState, DisableAbortButton, EnableAbortButton, ProcessingState, IsProcessingRequested
//LocatedState: EnableStartButton, DisableStartButton
//LoadedState: Loaded_InitialState, NotLocatedState, EnableLoadedStateGuiButons, DisableLoadedStateGuiButtons, LocatedState
//UnLoadedState: EnableUnLoadedStateGuiButons, DisableUnLoadedStateGuiButtons
//IdleState: Idle_InitialState, LoadedState, UnLoadedState
//LoadDeviceState: DisableCompleteButton, EnableCompleteButton
//HomeAllAxisState: HomeAllAxis
//PrepareState: IsLoaded2, MoveToRunPosState, MoveToLoadPosState, IsLoaded, IsLocated, Prepare_InitialState, IdleState, LoadDeviceState
//SystemPoweredState: IsCriticalAlarm, FaultedState, InitAfterSettingsState, SystemPowered_InitialState, InitState

//External Transition: AbortProcessingTOSaveResultsTransition From:AbortProcessingState To:SaveResultsState Via:ProcessingCompleteEvent Action:SaveResultsNoDbAsync Guard:none
//External Transition: LocatingTOIsLocated4Transition From:LocatingState To:IsLocated4 Via:LocateCompleteEvent Action:none Guard:none
//External Transition: IsProcessingRequestedTOLocatedTransition From:IsProcessingRequested To:LocatedState Via: Action:none Guard:No
//External Transition: IsProcessingRequestedTOProcessingTransition From:IsProcessingRequested To:ProcessingState Via: Action:none Guard:Yes
//External Transition: AutoCalTOIdleTransition From:AutoCalState To:IdleState Via:CalCompleteEvent Action:none Guard:none
//External Transition: MoreLocationsTOMoveToNextLocTransition From:MoreLocations To:MoveToNextLocState Via: Action:none Guard:Yes
//External Transition: LaserProcessingTOMoreLocationsTransition From:LaserProcessingState To:MoreLocations Via:ProcessingCompleteEvent Action:none Guard:none
//External Transition: Processing_InitialTOMoveToNextLocTransition From:Processing_InitialState To:MoveToNextLocState Via: Action:none Guard:none
//External Transition: MoveToNextLocTOLaserProcessingTransition From:MoveToNextLocState To:LaserProcessingState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IdleTOMoveToLoadPosTransition From:IdleState To:MoveToLoadPosState Via:LoadEvent Action:SetLoadingFlag Guard:none
//External Transition: LocatedTOLocateDeviceTransition From:LocatedState To:LocateDeviceState Via:StartEvent Action:bProcessingRequested___true Guard:IsLocated == false
//External Transition: IdleTOAutoCalTransition From:IdleState To:AutoCalState Via:CalEvent Action:none Guard:none
//External Transition: IsHomedTOHomeAllAxisTransition From:IsHomed To:HomeAllAxisState Via: Action:none Guard:No
//External Transition: InitTOInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: MoreLocationsTOProcessingCompleteTransition From:MoreLocations To:ProcessingCompleteState Via: Action:none Guard:No
//External Transition: SystemPowered_InitialTOInitTransition From:SystemPowered_InitialState To:InitState Via: Action:none Guard:none
//External Transition: InitialTOSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: SystemPoweredTOInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:EStopEvent Action:none Guard:none
//External Transition: FaultedTOIsCriticalAlarmTransition From:FaultedState To:IsCriticalAlarm Via:ClearFaultsEvent Action:none Guard:none
//External Transition: HomeAllAxisTOIsCriticalAlarmTransition From:HomeAllAxisState To:IsCriticalAlarm Via:AllAxisHomedEvent Action:none Guard:none
//External Transition: IsCriticalAlarmTOFaultedTransition From:IsCriticalAlarm To:FaultedState Via: Action:none Guard:Yes
//External Transition: ProcessingTOAbortProcessingTransition From:ProcessingState To:AbortProcessingState Via:AbortEvent Action:AbortProcessingAsync Guard:none
//External Transition: IsCriticalAlarmTOIsHomedTransition From:IsCriticalAlarm To:IsHomed Via: Action:none Guard:none
//External Transition: IsHomedTOPrepareTransition From:IsHomed To:PrepareState Via: Action:none Guard:Yes
//External Transition: Prepare_InitialTOIsLocatedTransition From:Prepare_InitialState To:IsLocated Via: Action:none Guard:none
//External Transition: ProcessingCompleteTOSaveResultsTransition From:ProcessingCompleteState To:SaveResultsState Via:AbortEvent Action:WriteMeasResultsReportNoDbAsync Guard:none
//External Transition: ProcessingCompleteTOSaveResultsTransition From:ProcessingCompleteState To:SaveResultsState Via:CommitEvent Action:WriteMeasResultsReportAsync Guard:none
//External Transition: IsLocatedTOLocatedTransition From:IsLocated To:LocatedState Via: Action:none Guard:Yes
//External Transition: InitAfterSettingsTOIsCriticalAlarmTransition From:InitAfterSettingsState To:IsCriticalAlarm Via:InitAfterSettingsCompleteEvent Action:none Guard:none
//External Transition: Loaded_InitialTONotLocatedTransition From:Loaded_InitialState To:NotLocatedState Via: Action:none Guard:none
//External Transition: IdleTOMoveToLoadPosTransition From:IdleState To:MoveToLoadPosState Via:UnLoadEvent Action:SetUnLoadingFlag Guard:none
//External Transition: HomeAllAxisTOFaultedTransition From:HomeAllAxisState To:FaultedState Via:DoorOpenEvent Action:CreateDoorOpenAlarm Guard:ByPassKeyDisabled
//External Transition: LocateDeviceTOAbortingTransition From:LocateDeviceState To:AbortingState Via:AbortEvent Action:none Guard:none
//External Transition: AbortingTOPrepareTransition From:AbortingState To:PrepareState Via:AbortCompleteEvent Action:none Guard:none
//External Transition: IdleTOFindFocusTransition From:IdleState To:FindFocusState Via:FindFocusEvent Action:none Guard:none
//External Transition: IsLoaded2TOLoadedTransition From:IsLoaded2 To:LoadedState Via: Action:none Guard:Yes
//External Transition: FindFocusTOPrepareTransition From:FindFocusState To:PrepareState Via:FindFocusCompleteEvent Action:none Guard:none
//External Transition: AutoCalTOAbortingTransition From:AutoCalState To:AbortingState Via:AbortEvent Action:none Guard:none
//External Transition: MoveToRePositionTORepositionTransition From:MoveToRePositionState To:RepositionState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: RepositionTOMoveToRunPos2Transition From:RepositionState To:MoveToRunPos2State Via:CompleteEvent Action:none Guard:none
//External Transition: IdleTOLocateDeviceTransition From:IdleState To:LocateDeviceState Via:LocateDeviceEvent Action:bProcessingRequested___FALSE Guard:none
//External Transition: LocateDevice_InitialTOLocatingTransition From:LocateDevice_InitialState To:LocatingState Via: Action:none Guard:none
//External Transition: IsLocated3TOProcessingTransition From:IsLocated3 To:ProcessingState Via: Action:none Guard:Yes
//External Transition: IsLocated3TOLocateDeviceTransition From:IsLocated3 To:LocateDeviceState Via: Action:none Guard:No
//External Transition: FindFocusTOAbortingTransition From:FindFocusState To:AbortingState Via:AbortEvent Action:none Guard:none
//External Transition: IsLocated4TOIsProcessingRequestedTransition From:IsLocated4 To:IsProcessingRequested Via: Action:none Guard:Yes
//External Transition: MoveToRunPos2TOLocatingTransition From:MoveToRunPos2State To:LocatingState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IsLocatedTOIsLoadedTransition From:IsLocated To:IsLoaded Via: Action:none Guard:No
//External Transition: IsLoadedTOIdleTransition From:IsLoaded To:IdleState Via: Action:none Guard:No
//External Transition: IsLoadedTOLoadedTransition From:IsLoaded To:LoadedState Via: Action:none Guard:Yes
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:MotionFaultEvent Action:CreateMotionFaultAlarm Guard:none
//External Transition: LocatedTOIsLocated3Transition From:LocatedState To:IsLocated3 Via:StartEvent Action:none Guard:none
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:LaserFaultEvent Action:none Guard:ByPassKeyDisabled
//External Transition: SaveResultsTOLocatedTransition From:SaveResultsState To:LocatedState Via:SaveResultsCompleteEvent Action:none Guard:none
//External Transition: IsLocated4TOMoveToRePositionTransition From:IsLocated4 To:MoveToRePositionState Via: Action:none Guard:No
//External Transition: Idle_InitialTOUnLoadedTransition From:Idle_InitialState To:UnLoadedState Via: Action:none Guard:none
//External Transition: MoveToLoadPosTOLoadDeviceTransition From:MoveToLoadPosState To:LoadDeviceState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: LoadDeviceTOMoveToRunPosTransition From:LoadDeviceState To:MoveToRunPosState Via:CompleteEvent Action:none Guard:none
//External Transition: MoveToRunPosTOIsLoaded2Transition From:MoveToRunPosState To:IsLoaded2 Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IsLoaded2TOUnLoadedTransition From:IsLoaded2 To:UnLoadedState Via: Action:none Guard:No
//External Transition: ActiveTOFaultedTransition From:ActiveState To:FaultedState Via:DoorOpenEvent Action:CreateDoorOpenAlarm Guard:ByPassKeyDisabled
//Internal Transition: IdleStateOnManualLaserCmdEventTransition. OwnedBy IdleState. Action SetLaserValueAsync  Guard:none
//Internal Transition: PrepareStateOnmissingeventTransition. OwnedBy PrepareState. Action   Guard:none
//Internal Transition: ProcessingStateOnmissingeventTransition. OwnedBy ProcessingState. Action   Guard:none
//Internal Transition: LoadDeviceStateOnDoorOpenEventTransition. OwnedBy LoadDeviceState. Action   Guard:none
//Internal Transition: UnLoadedStateOnLocateDeviceEventTransition. OwnedBy UnLoadedState. Action   Guard:none
