//WriteCompleteEvent
//TestCompleteEvent
//MoveCompleteEvent
//MoveCompleteEvent
//LoadEvent
//StartEvent
//CreateRefFileEvent
//AllAxisHomedEvent
//AbortEvent
//InitCompleteEvent
//CommitEvent
//AbortEvent
//InitAfterSettingsCompleteEvent
//ClearFaultsEvent
//EStopEvent
//InitEvent
//AbortCompleteEvent
//MvoeCompleteEvent
//AbortEvent
//DoorClosedEvent
//SensorFaultEvent
//DoorClosedEvent
//AbortEvent
//RefFileCompleteEvent
//CommitEvent
//DoorClosedEvent
//DoorClosedEvent
//BypassKeyEnabledEvent
//DoorOpenedEvent
//MotionFaultEvent

//CouponNotLoadedErrorState: DisableAbortButton, EnableAbortButton
//CreateRefFileState: LockDoor
//RemoveCouponFirstErrorState: DisableAbortButton, EnableAbortButton
//LoadCouponState: DisableCommitButton, EnableCommitButton
//WaitForDoorClose2State: LockDoor
//MoveToLoadPosState: EnableLoadButton, LockDoor
//RefFrameUpdatedState: EnableStartButton
//CreateReferenceFileState: MoveToLens1PosState, CreateRefFileState, RemoveCouponFirstErrorState, IsCouponLoaded2_, IsDoorClosed2_, WaitForDoorClose4State, CreateReferenceFileStateInitial
//IdleState: IsRefFrameUpdatedToday_, IdleStateInitial, RefFrameNeededState, RefFrameUpdatedState, EnableManualMotion, DisableManualMotion
//AbortInspectingState: _DisableAbortButton
//LoadOrUnLoadState: IsDoorClosed3__OR___BypassKeyEnabled_AND__Login____Operator___, WaitForDoorClose3State, LoadCouponState, MoveToLoadPosState, LoadOrUnLoadStateInitial, IsDoorClosed_OR___BypassKeyEnabled_AND__Login____Operator___, WaitForDoorCloseState, LockDoor__BypassKeyEnabled_AND__Login____Operator__, DisableLoadButton
//MoveToLLState: MoveToNextLens
//ReviewState: EnableCommitButton, DisableCommitButton
//TestLLState: StartLLTestAsync
//HomeAllAxisState: HomeAllAxis
//InspectingState: CloseShutter, IsMoreLLToTest_, InspectingStateInitial, WriteReportState, AbortInspectingState, MoveToLLState, ReviewState, TestLLState, EnableAbortButton
//PrepareState: CouponNotLoadedErrorState, IsCouponLoaded_OR_BypassEnabled_, DisableManualMotion, WaitForDoorClose2State, IsDoorClosed4_Or_IsBypassEnabled_, CreateReferenceFileState, PrepareStateInitial, IdleState, LoadOrUnLoadState
//SystemPoweredState: IsCriticalAlarm_, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: WriteReportTOPrepareTransition From:WriteReportState To:PrepareState Via:WriteCompleteEvent Action:none Guard:none
//External Transition: TestLLTOIsMoreLLToTest_Transition From:TestLLState To:IsMoreLLToTest_ Via:TestCompleteEvent Action:none Guard:none
//External Transition: IsDoorClosed4_Or_IsBypassEnabled_TOIsCouponLoaded_OR_BypassEnabled_Transition From:IsDoorClosed4_Or_IsBypassEnabled_ To:IsCouponLoaded_OR_BypassEnabled_ Via: Action:none Guard:Yes
//External Transition: MoveToLLTOTestLLTransition From:MoveToLLState To:TestLLState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IsRefFrameUpdatedToday_TORefFrameUpdatedTransition From:IsRefFrameUpdatedToday_ To:RefFrameUpdatedState Via: Action:none Guard:Yes
//External Transition: IdleInitialTOIsRefFrameUpdatedToday_Transition From:IdleStateInitial To:IsRefFrameUpdatedToday_ Via: Action:none Guard:none
//External Transition: MoveToLoadPosTOLoadCouponTransition From:MoveToLoadPosState To:LoadCouponState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IdleTOLoadOrUnLoadTransition From:IdleState To:LoadOrUnLoadState Via:LoadEvent Action:none Guard:none
//External Transition: IdleTOIsDoorClosed4_Or_IsBypassEnabled_Transition From:IdleState To:IsDoorClosed4_Or_IsBypassEnabled_ Via:StartEvent Action:none Guard:BypassKeyDisabled OR (Login != Operator)
//External Transition: IdleTOCreateReferenceFileTransition From:IdleState To:CreateReferenceFileState Via:CreateRefFileEvent Action:none Guard:none
//External Transition: IsHomed_TOHomeAllAxisTransition From:IsHomed_ To:HomeAllAxisState Via: Action:none Guard:No
//External Transition: HomeAllAxisTOIsCriticalAlarm_Transition From:HomeAllAxisState To:IsCriticalAlarm_ Via:AllAxisHomedEvent Action:none Guard:none
//External Transition: InspectingTOAbortInspectingTransition From:InspectingState To:AbortInspectingState Via:AbortEvent Action:AbortInspectingAsync Guard:none
//External Transition: InitTOInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: ReviewTOWriteReportTransition From:ReviewState To:WriteReportState Via:CommitEvent Action:WriteMeasResultsMfgDbAsync Guard:none
//External Transition: IsRefFrameUpdatedToday_TORefFrameNeededTransition From:IsRefFrameUpdatedToday_ To:RefFrameNeededState Via: Action:none Guard:No
//External Transition: ReviewTOWriteReportTransition From:ReviewState To:WriteReportState Via:AbortEvent Action:WriteMeasResultsLocalAsync Guard:none
//External Transition: PrepareInitialTOIdleTransition From:PrepareStateInitial To:IdleState Via: Action:none Guard:none
//External Transition: IsHomed_TOPrepareTransition From:IsHomed_ To:PrepareState Via: Action:none Guard:Yes
//External Transition: IsCriticalAlarm_TOIsHomed_Transition From:IsCriticalAlarm_ To:IsHomed_ Via: Action:none Guard:none
//External Transition: IsMoreLLToTest_TOReviewTransition From:IsMoreLLToTest_ To:ReviewState Via: Action:none Guard:No
//External Transition: IsCriticalAlarm_TOFaultedTransition From:IsCriticalAlarm_ To:FaultedState Via: Action:none Guard:Yes
//External Transition: InitAfterSettingsTOIsCriticalAlarm_Transition From:InitAfterSettingsState To:IsCriticalAlarm_ Via:InitAfterSettingsCompleteEvent Action:none Guard:none
//External Transition: FaultedTOIsCriticalAlarm_Transition From:FaultedState To:IsCriticalAlarm_ Via:ClearFaultsEvent Action:none Guard:none
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:EStopEvent Action:RaiseEStopMomentaryAlarm Guard:none
//External Transition: SystemPoweredTOInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: InitialTOSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: SystemPoweredInitialTOInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: AbortInspectingTOWriteReportTransition From:AbortInspectingState To:WriteReportState Via:AbortCompleteEvent Action:WriteMeasResultsLocalAsync Guard:none
//External Transition: IsCouponLoaded2_TORemoveCouponFirstErrorTransition From:IsCouponLoaded2_ To:RemoveCouponFirstErrorState Via: Action:none Guard:Yes
//External Transition: MoveToLens1PosTOCreateRefFileTransition From:MoveToLens1PosState To:CreateRefFileState Via:MvoeCompleteEvent Action:none Guard:none
//External Transition: CouponNotLoadedErrorTOIdleTransition From:CouponNotLoadedErrorState To:IdleState Via:AbortEvent Action:none Guard:none
//External Transition: IsCouponLoaded_OR_BypassEnabled_TOInspectingTransition From:IsCouponLoaded_OR_BypassEnabled_ To:InspectingState Via: Action:none Guard:Yes
//External Transition: IsCouponLoaded_OR_BypassEnabled_TOCouponNotLoadedErrorTransition From:IsCouponLoaded_OR_BypassEnabled_ To:CouponNotLoadedErrorState Via: Action:none Guard:No
//External Transition: WaitForDoorClose3TOIdleTransition From:WaitForDoorClose3State To:IdleState Via:DoorClosedEvent Action:none Guard:none
//External Transition: IsDoorClosed3__OR___BypassKeyEnabled_AND__Login____Operator___TOWaitForDoorClose3Transition From:IsDoorClosed3__OR___BypassKeyEnabled_AND__Login____Operator___ To:WaitForDoorClose3State Via: Action:none Guard:No
//External Transition: IsDoorClosed3__OR___BypassKeyEnabled_AND__Login____Operator___TOIdleTransition From:IsDoorClosed3__OR___BypassKeyEnabled_AND__Login____Operator___ To:IdleState Via: Action:none Guard:Yes
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:SensorFaultEvent Action:RaiseSensorFaultMomentaryAlarm Guard:none
//External Transition: WaitForDoorClose4TOMoveToLens1PosTransition From:WaitForDoorClose4State To:MoveToLens1PosState Via:DoorClosedEvent Action:none Guard:none
//External Transition: RemoveCouponFirstErrorTOIdleTransition From:RemoveCouponFirstErrorState To:IdleState Via:AbortEvent Action:none Guard:none
//External Transition: IsDoorClosed2_TOMoveToLens1PosTransition From:IsDoorClosed2_ To:MoveToLens1PosState Via: Action:none Guard:Yes
//External Transition: IsMoreLLToTest_TOMoveToLLTransition From:IsMoreLLToTest_ To:MoveToLLState Via: Action:none Guard:Yes
//External Transition: CreateReferenceFileInitialTOIsCouponLoaded2_Transition From:CreateReferenceFileStateInitial To:IsCouponLoaded2_ Via: Action:none Guard:none
//External Transition: CreateRefFileTOIdleTransition From:CreateRefFileState To:IdleState Via:RefFileCompleteEvent Action:none Guard:none
//External Transition: IsCouponLoaded2_TOIsDoorClosed2_Transition From:IsCouponLoaded2_ To:IsDoorClosed2_ Via: Action:none Guard:No
//External Transition: LoadCouponTOIsDoorClosed3__OR___BypassKeyEnabled_AND__Login____Operator___Transition From:LoadCouponState To:IsDoorClosed3__OR___BypassKeyEnabled_AND__Login____Operator___ Via:CommitEvent Action:none Guard:none
//External Transition: WaitForDoorClose2TOIsCouponLoaded_OR_BypassEnabled_Transition From:WaitForDoorClose2State To:IsCouponLoaded_OR_BypassEnabled_ Via:DoorClosedEvent Action:none Guard:none
//External Transition: InspectingInitialTOMoveToLLTransition From:InspectingStateInitial To:MoveToLLState Via: Action:none Guard:none
//External Transition: IsDoorClosed4_Or_IsBypassEnabled_TOWaitForDoorClose2Transition From:IsDoorClosed4_Or_IsBypassEnabled_ To:WaitForDoorClose2State Via: Action:none Guard:No
//External Transition: IsDoorClosed_OR___BypassKeyEnabled_AND__Login____Operator___TOMoveToLoadPosTransition From:IsDoorClosed_OR___BypassKeyEnabled_AND__Login____Operator___ To:MoveToLoadPosState Via: Action:none Guard:Yes
//External Transition: LoadOrUnLoadInitialTOIsDoorClosed_OR___BypassKeyEnabled_AND__Login____Operator___Transition From:LoadOrUnLoadStateInitial To:IsDoorClosed_OR___BypassKeyEnabled_AND__Login____Operator___ Via: Action:none Guard:none
//External Transition: WaitForDoorCloseTOMoveToLoadPosTransition From:WaitForDoorCloseState To:MoveToLoadPosState Via:DoorClosedEvent Action:none Guard:none
//External Transition: IsDoorClosed_OR___BypassKeyEnabled_AND__Login____Operator___TOWaitForDoorCloseTransition From:IsDoorClosed_OR___BypassKeyEnabled_AND__Login____Operator___ To:WaitForDoorCloseState Via: Action:none Guard:No
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:BypassKeyEnabledEvent Action:RaiseBypassKeyOnMomentaryAlarm Guard:Login == Operator
//External Transition: InspectingTOFaultedTransition From:InspectingState To:FaultedState Via:DoorOpenedEvent Action:RaiseDoorOpenedMomentaryAlarm Guard:BypassKeyEnabled == FALSE
//External Transition: SystemPoweredTOFaultedTransition From:SystemPoweredState To:FaultedState Via:MotionFaultEvent Action:RaiseMotionFaultMomentaryAlarm Guard:none
//External Transition: IsDoorClosed2_TOWaitForDoorClose4Transition From:IsDoorClosed2_ To:WaitForDoorClose4State Via: Action:none Guard:No
//Internal Transition: PrepareStateOnmissingeventTransition. OwnedBy PrepareState. Action   Guard:none
//Internal Transition: SystemPoweredStateOnmissingeventTransition. OwnedBy SystemPoweredState. Action   Guard:none
