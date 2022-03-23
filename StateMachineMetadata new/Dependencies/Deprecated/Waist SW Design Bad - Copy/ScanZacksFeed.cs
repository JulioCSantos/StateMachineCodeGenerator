//AbortEvent
//AcqCompleteTimeOutEvent
//AcqCompleteEvent
//SettleTimeCompleteEvent
//MoveCompleteEvent
//StartScanEvent

//WaitForAcqCompleteState: StopAcqCompleteTimer,  StartAcqCompleteTimer
//ScanState: IsMoreToScan?, WaitForAcqCompleteState, WaitSettleTimeState, IsLineScanType?, MoveToStartPosState, ScanStateInitial
//SystemPoweredState: ScanState, SystemPoweredStateInitial, IdleState

//External Transition: ScanToIdleTransition From:ScanState To:IdleState Via:AbortEvent Action:none Guard:none
//External Transition: WaitForAcqCompleteToIdleTransition From:WaitForAcqCompleteState To:IdleState Via:AcqCompleteTimeOutEvent Action:WaistStateMachine.HandleEvent("AcqCompleteTimeOutEvent") Guard:none
//External Transition: IsMoreToScanToIdleTransition From:IsMoreToScan? To:IdleState Via: Action:WaistStateMachine.HandleEvent("ScanCompleteEvent") Guard:No
//External Transition: IsMoreToScanToMoveToStartPosTransition From:IsMoreToScan? To:MoveToStartPosState Via: Action:none Guard:Yes
//External Transition: WaitForAcqCompleteToIsMoreToScanTransition From:WaitForAcqCompleteState To:IsMoreToScan? Via:AcqCompleteEvent Action:none Guard:none
//External Transition: IsLineScanTypeToWaitForAcqCompleteTransition From:IsLineScanType? To:WaitForAcqCompleteState Via: Action:EnableAcqTrigger; StartScanMotion Guard:Yes
//External Transition: IsLineScanTypeToWaitForAcqCompleteTransition From:IsLineScanType? To:WaitForAcqCompleteState Via: Action:AcquireImageAsynch Guard:No
//External Transition: WaitSettleTimeToIsLineScanTypeTransition From:WaitSettleTimeState To:IsLineScanType? Via:SettleTimeCompleteEvent Action:none Guard:none
//External Transition: MoveToStartPosToWaitSettleTimeTransition From:MoveToStartPosState To:WaitSettleTimeState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: ScanInitialToMoveToStartPosTransition From:ScanStateInitial To:MoveToStartPosState Via: Action:none Guard:none
//External Transition: IdleToScanTransition From:IdleState To:ScanState Via:StartScanEvent Action:none Guard:none
//External Transition: SystemPoweredInitialToIdleTransition From:SystemPoweredStateInitial To:IdleState Via: Action:none Guard:none
//External Transition: InitialToSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
