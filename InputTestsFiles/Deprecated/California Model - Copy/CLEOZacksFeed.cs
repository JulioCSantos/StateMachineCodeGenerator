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

//CouponNotLoadedErrorState: DisableAbortButton, EnableAbortButton; LockDoor
//CreateRefFileState: LockDoor; CreateRefFile
//RemoveCouponFirstErrorState: DisableAbortButton, EnableAbortButton
//LoadCouponState: DisableCommitButton, EnableCommitButton
//WaitForDoorClose2State: LockDoor
//MoveToLoadPosState: EnableLoadButton; UnLockDoor, LockDoor; MoveToLoadPos
//RefFrameUpdatedState: EnableStartButton
//CreateReferenceFileState: MoveToLens1PosState, CreateRefFileState, RemoveCouponFirstErrorState, IsCouponLoaded2?, IsDoorClosed2?, WaitForDoorClose4State, CreateReferenceFileStateInitial
//IdleState: IsRefFrameUpdatedToday?, IdleStateInitial, RefFrameNeededState, RefFrameUpdatedState, EnableManualMotion;  EnableLoadButton;  EnableCreateRefButton; EnableSetupViewFields, DisableManualMotion; DisableLoadButton; DisableCreateRefButton; DisableSetupViewFields, DisableStartButton
//AbortInspectingState:  DisableAbortButton; StopAllMotion
//LoadOrUnLoadState: IsDoorClosed3  OR  (BypassKeyEnabled AND (Login != Operator)) , WaitForDoorClose3State, LoadCouponState, MoveToLoadPosState, LoadOrUnLoadStateInitial, IsDoorClosed OR  (BypassKeyEnabled AND (Login != Operator)) , WaitForDoorCloseState, LockDoor [BypassKeyEnabled AND (Login != Operator)], DisableLoadButton; 
//MoveToLLState: MoveToNextLens
//ReviewState: EnableCommitButton, DisableCommitButton; DisableAbortButton;
//TestLLState: StartLLTestAsync
//HomeAllAxisState: HomeAllAxis
//InspectingState: CloseShutter; StopAllMotion;, IsMoreLLToTest?, InspectingStateInitial, WriteReportState, AbortInspectingState, MoveToLLState, ReviewState, TestLLState, EnableAbortButton
//PrepareState: CouponNotLoadedErrorState, IsCouponLoaded OR BypassEnabled?, DisableManualMotion; DisableLoadButton; DisableCreateRefButton; DisableSetupViewFields, WaitForDoorClose2State, IsDoorClosed4 Or IsBypassEnabled?, CreateReferenceFileState, PrepareStateInitial, IdleState, LoadOrUnLoadState
//SystemPoweredState: IsCriticalAlarm?, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: WriteReportToPrepareTransition From:WriteReportState To:PrepareState Via:WriteCompleteEvent Action:none Guard:none
//External Transition: TestLLToIsMoreLLToTestTransition From:TestLLState To:IsMoreLLToTest? Via:TestCompleteEvent Action:none Guard:none
//External Transition: IsDoorClosed4OrIsBypassEnabledToIsCouponLoadedORBypassEnabledTransition From:IsDoorClosed4 Or IsBypassEnabled? To:IsCouponLoaded OR BypassEnabled? Via: Action:none Guard:Yes
//External Transition: MoveToLLToTestLLTransition From:MoveToLLState To:TestLLState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IsRefFrameUpdatedTodayToRefFrameUpdatedTransition From:IsRefFrameUpdatedToday? To:RefFrameUpdatedState Via: Action:none Guard:Yes
//External Transition: IdleInitialToIsRefFrameUpdatedTodayTransition From:IdleStateInitial To:IsRefFrameUpdatedToday? Via: Action:none Guard:none
//External Transition: MoveToLoadPosToLoadCouponTransition From:MoveToLoadPosState To:LoadCouponState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: IdleToLoadOrUnLoadTransition From:IdleState To:LoadOrUnLoadState Via:LoadEvent Action:none Guard:none
//External Transition: IdleToIsDoorClosed4OrIsBypassEnabledTransition From:IdleState To:IsDoorClosed4 Or IsBypassEnabled? Via:StartEvent Action:none Guard:BypassKeyDisabled OR (Login != Operator)
//External Transition: IdleToCreateReferenceFileTransition From:IdleState To:CreateReferenceFileState Via:CreateRefFileEvent Action:none Guard:none
//External Transition: IsHomedToHomeAllAxisTransition From:IsHomed? To:HomeAllAxisState Via: Action:none Guard:No
//External Transition: HomeAllAxisToIsCriticalAlarmTransition From:HomeAllAxisState To:IsCriticalAlarm? Via:AllAxisHomedEvent Action:none Guard:none
//External Transition: InspectingToAbortInspectingTransition From:InspectingState To:AbortInspectingState Via:AbortEvent Action:AbortInspectingAsync Guard:none
//External Transition: InitToInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: ReviewToWriteReportTransition From:ReviewState To:WriteReportState Via:CommitEvent Action:WriteMeasResultsMfgDbAsync Guard:none
//External Transition: IsRefFrameUpdatedTodayToRefFrameNeededTransition From:IsRefFrameUpdatedToday? To:RefFrameNeededState Via: Action:none Guard:No
//External Transition: ReviewToWriteReportTransition From:ReviewState To:WriteReportState Via:AbortEvent Action:WriteMeasResultsLocalAsync Guard:none
//External Transition: PrepareInitialToIdleTransition From:PrepareStateInitial To:IdleState Via: Action:none Guard:none
//External Transition: IsHomedToPrepareTransition From:IsHomed? To:PrepareState Via: Action:none Guard:Yes
//External Transition: IsCriticalAlarmToIsHomedTransition From:IsCriticalAlarm? To:IsHomed? Via: Action:none Guard:none
//External Transition: IsMoreLLToTestToReviewTransition From:IsMoreLLToTest? To:ReviewState Via: Action:none Guard:No
//External Transition: IsCriticalAlarmToFaultedTransition From:IsCriticalAlarm? To:FaultedState Via: Action:none Guard:Yes
//External Transition: InitAfterSettingsToIsCriticalAlarmTransition From:InitAfterSettingsState To:IsCriticalAlarm? Via:InitAfterSettingsCompleteEvent Action:none Guard:none
//External Transition: FaultedToIsCriticalAlarmTransition From:FaultedState To:IsCriticalAlarm? Via:ClearFaultsEvent Action:none Guard:none
//External Transition: SystemPoweredToFaultedTransition From:SystemPoweredState To:FaultedState Via:EStopEvent Action:RaiseEStopMomentaryAlarm Guard:none
//External Transition: SystemPoweredToInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: InitialToSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: SystemPoweredInitialToInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: AbortInspectingToWriteReportTransition From:AbortInspectingState To:WriteReportState Via:AbortCompleteEvent Action:WriteMeasResultsLocalAsync Guard:none
//External Transition: IsCouponLoaded2ToRemoveCouponFirstErrorTransition From:IsCouponLoaded2? To:RemoveCouponFirstErrorState Via: Action:none Guard:Yes
//External Transition: MoveToLens1PosToCreateRefFileTransition From:MoveToLens1PosState To:CreateRefFileState Via:MvoeCompleteEvent Action:none Guard:none
//External Transition: CouponNotLoadedErrorToIdleTransition From:CouponNotLoadedErrorState To:IdleState Via:AbortEvent Action:none Guard:none
//External Transition: IsCouponLoadedORBypassEnabledToInspectingTransition From:IsCouponLoaded OR BypassEnabled? To:InspectingState Via: Action:none Guard:Yes
//External Transition: IsCouponLoadedORBypassEnabledToCouponNotLoadedErrorTransition From:IsCouponLoaded OR BypassEnabled? To:CouponNotLoadedErrorState Via: Action:none Guard:No
//External Transition: WaitForDoorClose3ToIdleTransition From:WaitForDoorClose3State To:IdleState Via:DoorClosedEvent Action:none Guard:none
//External Transition: IsDoorClosed3ORBypassKeyEnabledANDLoginOperatorToWaitForDoorClose3Transition From:IsDoorClosed3  OR  (BypassKeyEnabled AND (Login != Operator))  To:WaitForDoorClose3State Via: Action:none Guard:No
//External Transition: IsDoorClosed3ORBypassKeyEnabledANDLoginOperatorToIdleTransition From:IsDoorClosed3  OR  (BypassKeyEnabled AND (Login != Operator))  To:IdleState Via: Action:none Guard:Yes
//External Transition: SystemPoweredToFaultedTransition From:SystemPoweredState To:FaultedState Via:SensorFaultEvent Action:RaiseSensorFaultMomentaryAlarm Guard:none
//External Transition: WaitForDoorClose4ToMoveToLens1PosTransition From:WaitForDoorClose4State To:MoveToLens1PosState Via:DoorClosedEvent Action:none Guard:none
//External Transition: RemoveCouponFirstErrorToIdleTransition From:RemoveCouponFirstErrorState To:IdleState Via:AbortEvent Action:none Guard:none
//External Transition: IsDoorClosed2ToMoveToLens1PosTransition From:IsDoorClosed2? To:MoveToLens1PosState Via: Action:none Guard:Yes
//External Transition: IsMoreLLToTestToMoveToLLTransition From:IsMoreLLToTest? To:MoveToLLState Via: Action:none Guard:Yes
//External Transition: CreateReferenceFileInitialToIsCouponLoaded2Transition From:CreateReferenceFileStateInitial To:IsCouponLoaded2? Via: Action:none Guard:none
//External Transition: CreateRefFileToIdleTransition From:CreateRefFileState To:IdleState Via:RefFileCompleteEvent Action:none Guard:none
//External Transition: IsCouponLoaded2ToIsDoorClosed2Transition From:IsCouponLoaded2? To:IsDoorClosed2? Via: Action:none Guard:No
//External Transition: LoadCouponToIsDoorClosed3ORBypassKeyEnabledANDLoginOperatorTransition From:LoadCouponState To:IsDoorClosed3  OR  (BypassKeyEnabled AND (Login != Operator))  Via:CommitEvent Action:none Guard:none
//External Transition: WaitForDoorClose2ToIsCouponLoadedORBypassEnabledTransition From:WaitForDoorClose2State To:IsCouponLoaded OR BypassEnabled? Via:DoorClosedEvent Action:none Guard:none
//External Transition: InspectingInitialToMoveToLLTransition From:InspectingStateInitial To:MoveToLLState Via: Action:none Guard:none
//External Transition: IsDoorClosed4OrIsBypassEnabledToWaitForDoorClose2Transition From:IsDoorClosed4 Or IsBypassEnabled? To:WaitForDoorClose2State Via: Action:none Guard:No
//External Transition: IsDoorClosedORBypassKeyEnabledANDLoginOperatorToMoveToLoadPosTransition From:IsDoorClosed OR  (BypassKeyEnabled AND (Login != Operator))  To:MoveToLoadPosState Via: Action:none Guard:Yes
//External Transition: LoadOrUnLoadInitialToIsDoorClosedORBypassKeyEnabledANDLoginOperatorTransition From:LoadOrUnLoadStateInitial To:IsDoorClosed OR  (BypassKeyEnabled AND (Login != Operator))  Via: Action:none Guard:none
//External Transition: WaitForDoorCloseToMoveToLoadPosTransition From:WaitForDoorCloseState To:MoveToLoadPosState Via:DoorClosedEvent Action:none Guard:none
//External Transition: IsDoorClosedORBypassKeyEnabledANDLoginOperatorToWaitForDoorCloseTransition From:IsDoorClosed OR  (BypassKeyEnabled AND (Login != Operator))  To:WaitForDoorCloseState Via: Action:none Guard:No
//External Transition: SystemPoweredToFaultedTransition From:SystemPoweredState To:FaultedState Via:BypassKeyEnabledEvent Action:RaiseBypassKeyOnMomentaryAlarm Guard:Login == Operator
//External Transition: InspectingToFaultedTransition From:InspectingState To:FaultedState Via:DoorOpenedEvent Action:RaiseDoorOpenedMomentaryAlarm Guard:BypassKeyEnabled == FALSE
//External Transition: SystemPoweredToFaultedTransition From:SystemPoweredState To:FaultedState Via:MotionFaultEvent Action:RaiseMotionFaultMomentaryAlarm Guard:none
//External Transition: IsDoorClosed2ToWaitForDoorClose4Transition From:IsDoorClosed2? To:WaitForDoorClose4State Via: Action:none Guard:No
//Internal Transition: PrepareStateOnLockDoorTransition. OwnedBy PrepareState. Action validates
//Internal Transition: SystemPoweredStateOnCriticalTransition. OwnedBy SystemPoweredState. Action Alarms
