//NewFeatureVectorsCreated
//DisableTriggeredScanEvent
//EnableTriggeredScanEvent
//UnsubscribeFeatureVectorUpdatesEvent
//StartOfLaserLineEvent
//NewFeatureVectorsCreated
//EStopEvent
//GetFeatureVectorsEvent
//SubscribeFeatureVectorUpdatesEvent
//InitCompleteEvent
//AbortScan
//SendFeatureVectorsEvent
//UnsubscribeFeatureVectorUpdatesEvent
//EnableCmdTestMode / SetCmdTestCase
//SubscribeXXX / UnsubscribeXXX
//LoadRecipe / GetFeatureVectors / EnableXXX
//ClearFaults
//GetFaults
//ScanProgressEventTimeOut
//GetSWVer
//GetModel
//GetFeatureVectorsEvent
//GetState
//ScanCompleteEvent
//ScanCompleteEvent
//Last Requested Line Scanned
//StartScanDIPulsedEvent
//CriticalFaultEvent
//GetHWVer

//ReadyState: ScanningState, ReadyToStartScanState, MoreResultsToSend?, SendingResultsState, SubscribedIdleState, UnSucscribedIdleState, ScanIdleState, ReadyStateInitial, ReadyStateInitial
//PoweredOnState: NotifyFaultedState, ReadyState, ReadyForInitState, PoweredOnStateInitial

//External Transition: SubscribedIdleToSendingResultsTransition From:SubscribedIdleState To:SendingResultsState Via:NewFeatureVectorsCreated Action:none Guard:none
//External Transition: InitEvent From:PoweredOnState To:ReadyForInitState Via: Action:none Guard:none
//External Transition: ReadyInitialToScanIdleTransition From:ReadyStateInitial To:ScanIdleState Via: Action:none Guard:none
//External Transition: ReadyToStartScanToScanIdleTransition From:ReadyToStartScanState To:ScanIdleState Via:DisableTriggeredScanEvent Action:none Guard:none
//External Transition: ScanIdleToReadyToStartScanTransition From:ScanIdleState To:ReadyToStartScanState Via:EnableTriggeredScanEvent Action:none Guard:none
//External Transition: SendingResultsToUnSucscribedIdleTransition From:SendingResultsState To:UnSucscribedIdleState Via:UnsubscribeFeatureVectorUpdatesEvent Action:none Guard:none
//External Transition: MoreResultsToSendToSubscribedIdleTransition From:MoreResultsToSend? To:SubscribedIdleState Via: Action:none Guard:No
//External Transition: MoreResultsToSendToSendingResultsTransition From:MoreResultsToSend? To:SendingResultsState Via: Action:none Guard:Yes
//External Transition: ScanningToScanningTransition From:ScanningState To:ScanningState Via:StartOfLaserLineEvent Action:PulseStartOfLaserLineDO Guard:none
//External Transition: SendingResultsToSendingResultsTransition From:SendingResultsState To:SendingResultsState Via:NewFeatureVectorsCreated Action:QueueNotification Guard:none
//External Transition: ReadyToFaultedTransition From:ReadyState To:FaultedState Via:EStopEvent Action:none Guard:none
//External Transition: UnSucscribedIdleToUnSucscribedIdleTransition From:UnSucscribedIdleState To:UnSucscribedIdleState Via:GetFeatureVectorsEvent Action:none Guard:none
//External Transition: UnSucscribedIdleToSubscribedIdleTransition From:UnSucscribedIdleState To:SubscribedIdleState Via:SubscribeFeatureVectorUpdatesEvent Action:none Guard:none
//External Transition: ReadyInitialToUnSucscribedIdleTransition From:ReadyStateInitial To:UnSucscribedIdleState Via: Action:none Guard:none
//External Transition: ReadyForInitToIsWarningOrCriticalAlarmTransition From:ReadyForInitState To:IsWarningOrCriticalAlarm Via:InitCompleteEvent Action:none Guard:none
//External Transition: PoweredOnInitialToReadyForInitTransition From:PoweredOnStateInitial To:ReadyForInitState Via: Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarmToFaultedTransition From:IsWarningOrCriticalAlarm To:FaultedState Via: Action:none Guard:Yes
//External Transition: ScanningToScanIdleTransition From:ScanningState To:ScanIdleState Via:AbortScan Action:none Guard:none
//External Transition: SendingResultsToMoreResultsToSendTransition From:SendingResultsState To:MoreResultsToSend? Via:SendFeatureVectorsEvent Action:none Guard:none
//External Transition: SubscribedIdleToUnSucscribedIdleTransition From:SubscribedIdleState To:UnSucscribedIdleState Via:UnsubscribeFeatureVectorUpdatesEvent Action:none Guard:none
//External Transition: ScanIdleToScanIdleTransition From:ScanIdleState To:ScanIdleState Via:EnableCmdTestMode / SetCmdTestCase Action:none Guard:none
//External Transition: ScanIdleToScanIdleTransition From:ScanIdleState To:ScanIdleState Via:SubscribeXXX / UnsubscribeXXX Action:none Guard:none
//External Transition: ScanIdleToScanIdleTransition From:ScanIdleState To:ScanIdleState Via:LoadRecipe / GetFeatureVectors / EnableXXX Action:none Guard:none
//External Transition: PoweredOnToPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:ClearFaults Action:none Guard:none
//External Transition: PoweredOnToPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:GetFaults Action:none Guard:none
//External Transition: ScanningToScanningTransition From:ScanningState To:ScanningState Via:ScanProgressEventTimeOut Action:ScanProgressEvent Guard:none
//External Transition: PoweredOnToPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:GetSWVer Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarmToReadyTransition From:IsWarningOrCriticalAlarm To:ReadyState Via: Action:none Guard:No
//External Transition: PoweredOnToPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:GetModel Action:none Guard:none
//External Transition: SendingResultsToSendingResultsTransition From:SendingResultsState To:SendingResultsState Via:GetFeatureVectorsEvent Action:none Guard:none
//External Transition: InitialToPoweredOnTransition From:Initial To:PoweredOnState Via: Action:none Guard:none
//External Transition: PoweredOnToPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:GetState Action:none Guard:none
//External Transition: SendingResultsToSendingResultsTransition From:SendingResultsState To:SendingResultsState Via:ScanCompleteEvent Action:QueueNotification Guard:none
//External Transition: SubscribedIdleToSubscribedIdleTransition From:SubscribedIdleState To:SubscribedIdleState Via:ScanCompleteEvent Action:SendNotification Guard:none
//External Transition: ScanningToScanIdleTransition From:ScanningState To:ScanIdleState Via:Last Requested Line Scanned Action:Send ScanCompleteEvent Guard:none
//External Transition: ReadyToStartScanToScanningTransition From:ReadyToStartScanState To:ScanningState Via:StartScanDIPulsedEvent Action:none Guard:none
//External Transition: ReadyToFaultedTransition From:ReadyState To:FaultedState Via:CriticalFaultEvent Action:none Guard:none
//External Transition: PoweredOnToPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:GetHWVer Action:none Guard:none
