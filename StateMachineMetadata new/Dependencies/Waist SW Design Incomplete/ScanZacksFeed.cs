//AbortEvent
//AcqCompleteTimeOutEvent
//AcqCompleteEvent
//SettleTimeCompleteEvent
//MoveCompleteEvent
//StartScanEvent

//WaitForAcqCompleteState: StopAcqCompleteTimer, _StartAcqCompleteTimer
//ScanState: IsMoreToScan_, WaitForAcqCompleteState, WaitSettleTimeState, IsLineScanType_, MoveToStartPosState, ScanStateInitial
//SystemPoweredState: ScanState, SystemPoweredStateInitial, IdleState

//External Transition: ScanTOIdleTransition From:ScanState To:IdleState Via:AbortEvent Action:none Guard:none
//External Transition: WaitForAcqCompleteTOIdleTransition From:WaitForAcqCompleteState To:IdleState Via:AcqCompleteTimeOutEvent Action:WaistStateMachine_HandleEvent__AcqCompleteTimeOutEvent__ Guard:none
//External Transition: IsMoreToScan_TOIdleTransition From:IsMoreToScan_ To:IdleState Via: Action:WaistStateMachine_HandleEvent__ScanCompleteEvent__ Guard:No
//External Transition: IsMoreToScan_TOMoveToStartPosTransition From:IsMoreToScan_ To:MoveToStartPosState Via: Action:none Guard:Yes
//External Transition: WaitForAcqCompleteTOIsMoreToScan_Transition From:WaitForAcqCompleteState To:IsMoreToScan_ Via:AcqCompleteEvent Action:none Guard:none
//External Transition: IsLineScanType_TOWaitForAcqCompleteTransition From:IsLineScanType_ To:WaitForAcqCompleteState Via: Action:EnableAcqTrigger Guard:Yes
//External Transition: IsLineScanType_TOWaitForAcqCompleteTransition From:IsLineScanType_ To:WaitForAcqCompleteState Via: Action:AcquireImageAsynch Guard:No
//External Transition: WaitSettleTimeTOIsLineScanType_Transition From:WaitSettleTimeState To:IsLineScanType_ Via:SettleTimeCompleteEvent Action:none Guard:none
//External Transition: MoveToStartPosTOWaitSettleTimeTransition From:MoveToStartPosState To:WaitSettleTimeState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: ScanInitialTOMoveToStartPosTransition From:ScanStateInitial To:MoveToStartPosState Via: Action:none Guard:none
//External Transition: IdleTOScanTransition From:IdleState To:ScanState Via:StartScanEvent Action:none Guard:none
//External Transition: SystemPoweredInitialTOIdleTransition From:SystemPoweredStateInitial To:IdleState Via: Action:none Guard:none
//External Transition: InitialTOSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
