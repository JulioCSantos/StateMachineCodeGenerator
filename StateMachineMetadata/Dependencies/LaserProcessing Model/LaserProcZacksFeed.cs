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
//AbortProcessingState: _DisableAbortButton
//MoveToNextLocState: MoveToNextLoc
//ProcessingCompleteState: EnableCommitButton, DisableCommitButton
//LaserProcessingState: LaserProcessingAsync
//LocateDeviceState: MoveToRunPos2State, LocateDeviceStateInitial, LocatingState, RepositionState, MoveToRePositionState, IsLocated4_
//ProcessingState: MoreLocations_, ProcessingStateInitial, SaveResultsState, AbortProcessingState, MoveToNextLocState, ProcessingCompleteState, LaserProcessingState
//ActiveState: IsLocated3_, FindFocusState, AbortingState, AutoCalState, LocateDeviceState, DisableAbortButton, EnableAbortButton, ProcessingState, IsProcessingRequested_
//LocatedState: EnableStartButton, DisableStartButton
//LoadedState: LoadedStateInitial, NotLocatedState, EnableLoadedStateGuiButons, DisableLoadedStateGuiButtons, LocatedState
//UnLoadedState: EnableUnLoadedStateGuiButons, DisableUnLoadedStateGuiButtons
//IdleState: IdleStateInitial, LoadedState, UnLoadedState
//LoadDeviceState: DisableCompleteButton, EnableCompleteButton
//HomeAllAxisState: HomeAllAxis
//PrepareState: IsLoaded2_, MoveToRunPosState, MoveToLoadPosState, IsLoaded_, IsLocated_, PrepareStateInitial, IdleState, LoadDeviceState
//SystemPoweredState: IsCriticalAlarm_, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: AbortProcessingTOSaveResultsTransition From:AbortProcessingState To:SaveResultsState Via:ProcessingCompleteEvent Action:SaveResultsNoDbAsync Guard:none
//External Transition: LocatingTOIsLocated4_Transition From:LocatingState To:IsLocated4_ Via:LocateCompleteEvent Action:none Guard:none
//External Transition: IsProcessingRequested_TOLocatedTransition From:IsProcessingRequested_ To:LocatedState Via: Action:none Guard:No
//External Transition: IsProcessingRequested_TOProcessingTransition From:IsProcessingRequested_ To:ProcessingState Via: Action:none Guard:Yes
//External Transition: AutoCalTOIdleTransition From:AutoCalState To:IdleState Via:CalCompleteEvent Action:none Guard:none
//External Transition: MoreLocations_TOMoveToNextLocTransition From:MoreLocations_ To:MoveToNextLocState Via: Action:none Guard:Yes
//External Transition: LaserProcessingTOMoreLocations_Transition From:LaserProcessingState To:MoreLocations_ Via:ProcessingCompleteEvent Action:none Guard:none
//External Transition: ProcessingInitialTOMoveToNextLocTransition From:ProcessingStateInitial To:MoveToNextLocState Via: Action:none Guard:none
//External Transition: MoveToNextLocTOLaserProcessingTransition From:MoveToNextLocState To:LaserProcessingState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IdleTOMoveToLoadPosTransition From:IdleState To:MoveToLoadPosState Via:LoadEvent Action:_SetLoadingFlag Guard:none
//External Transition: LocatedTOLocateDeviceTransition From:LocatedState To:LocateDeviceState Via:StartEvent Action:bProcessingRequested___true Guard:IsLocated == false
//External Transition: IdleTOAutoCalTransition From:IdleState To:AutoCalState Via:CalEvent Action:none Guard:none
//External Transition: IsHomed_TOHomeAllAxisTransition From:IsHomed_ To:HomeAllAxisState Via: Action:none Guard:No
//External Transition: InitTOInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: MoreLocations_TOProcessingCompleteTransition From:MoreLocations_ To:ProcessingCompleteState Via: Action:none Guard:No
//External Transition: SystemPoweredInitialTOInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: InitialTOSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: SystemPoweredTOInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:EStopEvent Action:none Guard:none
//External Transition: FaultedTOIsCriticalAlarm_Transition From:FaultedState To:IsCriticalAlarm_ Via:ClearFaultsEvent Action:none Guard:none
//External Transition: HomeAllAxisTOIsCriticalAlarm_Transition From:HomeAllAxisState To:IsCriticalAlarm_ Via:AllAxisHomedEvent Action:none Guard:none
//External Transition: IsCriticalAlarm_TOFaultedTransition From:IsCriticalAlarm_ To:FaultedState Via: Action:none Guard:Yes
//External Transition: ProcessingTOAbortProcessingTransition From:ProcessingState To:AbortProcessingState Via:AbortEvent Action:AbortProcessingAsync Guard:none
//External Transition: IsCriticalAlarm_TOIsHomed_Transition From:IsCriticalAlarm_ To:IsHomed_ Via: Action:none Guard:none
//External Transition: IsHomed_TOPrepareTransition From:IsHomed_ To:PrepareState Via: Action:none Guard:Yes
//External Transition: PrepareInitialTOIsLocated_Transition From:PrepareStateInitial To:IsLocated_ Via: Action:none Guard:none
//External Transition: ProcessingCompleteTOSaveResultsTransition From:ProcessingCompleteState To:SaveResultsState Via:AbortEvent Action:WriteMeasResultsReportNoDbAsync Guard:none
//External Transition: ProcessingCompleteTOSaveResultsTransition From:ProcessingCompleteState To:SaveResultsState Via:CommitEvent Action:WriteMeasResultsReportAsync Guard:none
//External Transition: IsLocated_TOLocatedTransition From:IsLocated_ To:LocatedState Via: Action:none Guard:Yes
//External Transition: InitAfterSettingsTOIsCriticalAlarm_Transition From:InitAfterSettingsState To:IsCriticalAlarm_ Via:InitAfterSettingsCompleteEvent Action:none Guard:none
//External Transition: LoadedInitialTONotLocatedTransition From:LoadedStateInitial To:NotLocatedState Via: Action:none Guard:none
//External Transition: IdleTOMoveToLoadPosTransition From:IdleState To:MoveToLoadPosState Via:UnLoadEvent Action:SetUnLoadingFlag Guard:none
//External Transition: HomeAllAxisTOFaultedTransition From:HomeAllAxisState To:FaultedState Via:DoorOpenEvent Action:CreateDoorOpenAlarm Guard:ByPassKeyDisabled
//External Transition: LocateDeviceTOAbortingTransition From:LocateDeviceState To:AbortingState Via:AbortEvent Action:none Guard:none
//External Transition: AbortingTOPrepareTransition From:AbortingState To:PrepareState Via:AbortCompleteEvent Action:none Guard:none
//External Transition: IdleTOFindFocusTransition From:IdleState To:FindFocusState Via:FindFocusEvent Action:none Guard:none
//External Transition: IsLoaded2_TOLoadedTransition From:IsLoaded2_ To:LoadedState Via: Action:none Guard:Yes
//External Transition: FindFocusTOPrepareTransition From:FindFocusState To:PrepareState Via:FindFocusCompleteEvent Action:none Guard:none
//External Transition: AutoCalTOAbortingTransition From:AutoCalState To:AbortingState Via:AbortEvent Action:none Guard:none
//External Transition: MoveToRePositionTORepositionTransition From:MoveToRePositionState To:RepositionState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: RepositionTOMoveToRunPos2Transition From:RepositionState To:MoveToRunPos2State Via:CompleteEvent Action:none Guard:none
//External Transition: IdleTOLocateDeviceTransition From:IdleState To:LocateDeviceState Via:LocateDeviceEvent Action:bProcessingRequested___FALSE Guard:none
//External Transition: LocateDeviceInitialTOLocatingTransition From:LocateDeviceStateInitial To:LocatingState Via: Action:none Guard:none
//External Transition: IsLocated3_TOProcessingTransition From:IsLocated3_ To:ProcessingState Via: Action:none Guard:Yes
//External Transition: IsLocated3_TOLocateDeviceTransition From:IsLocated3_ To:LocateDeviceState Via: Action:none Guard:No
//External Transition: FindFocusTOAbortingTransition From:FindFocusState To:AbortingState Via:AbortEvent Action:none Guard:none
//External Transition: IsLocated4_TOIsProcessingRequested_Transition From:IsLocated4_ To:IsProcessingRequested_ Via: Action:none Guard:Yes
//External Transition: MoveToRunPos2TOLocatingTransition From:MoveToRunPos2State To:LocatingState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IsLocated_TOIsLoaded_Transition From:IsLocated_ To:IsLoaded_ Via: Action:none Guard:No
//External Transition: IsLoaded_TOIdleTransition From:IsLoaded_ To:IdleState Via: Action:none Guard:No
//External Transition: IsLoaded_TOLoadedTransition From:IsLoaded_ To:LoadedState Via: Action:none Guard:Yes
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:MotionFaultEvent Action:CreateMotionFaultAlarm Guard:none
//External Transition: LocatedTOIsLocated3_Transition From:LocatedState To:IsLocated3_ Via:StartEvent Action:none Guard:none
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:LaserFaultEvent Action:none Guard:ByPassKeyDisabled
//External Transition: SaveResultsTOLocatedTransition From:SaveResultsState To:LocatedState Via:SaveResultsCompleteEvent Action:none Guard:none
//External Transition: IsLocated4_TOMoveToRePositionTransition From:IsLocated4_ To:MoveToRePositionState Via: Action:none Guard:No
//External Transition: IdleInitialTOUnLoadedTransition From:IdleStateInitial To:UnLoadedState Via: Action:none Guard:none
//External Transition: MoveToLoadPosTOLoadDeviceTransition From:MoveToLoadPosState To:LoadDeviceState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: LoadDeviceTOMoveToRunPosTransition From:LoadDeviceState To:MoveToRunPosState Via:CompleteEvent Action:none Guard:none
//External Transition: MoveToRunPosTOIsLoaded2_Transition From:MoveToRunPosState To:IsLoaded2_ Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IsLoaded2_TOUnLoadedTransition From:IsLoaded2_ To:UnLoadedState Via: Action:none Guard:No
//External Transition: ActiveTOFaultedTransition From:ActiveState To:FaultedState Via:DoorOpenEvent Action:CreateDoorOpenAlarm Guard:ByPassKeyDisabled
//Internal Transition: IdleStateOnManualLaserCmdEventTransition. OwnedBy IdleState. Action SetLaserValueAsync  Guard:none
//Internal Transition: PrepareStateOnmissingeventTransition. OwnedBy PrepareState. Action   Guard:none
//Internal Transition: ProcessingStateOnmissingeventTransition. OwnedBy ProcessingState. Action   Guard:none
//Internal Transition: LoadDeviceStateOnDoorOpenEventTransition. OwnedBy LoadDeviceState. Action   Guard:none
//Internal Transition: UnLoadedStateOnLocateDeviceEventTransition. OwnedBy UnLoadedState. Action   Guard:none
