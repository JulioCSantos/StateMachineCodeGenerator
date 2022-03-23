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

//ReadyState: ScanningState, ReadyToStartScanState, MoreResultsToSend_, SendingResultsState, SubscribedIdleState, UnSucscribedIdleState, ScanIdleState, ReadyStateInitial, ReadyStateInitial
//PoweredOnState: NotifyFaultedState, ReadyState, ReadyForInitState, PoweredOnStateInitial

//External Transition: SubscribedIdleTOSendingResultsTransition From:SubscribedIdleState To:SendingResultsState Via:NewFeatureVectorsCreated Action:none Guard:none
//External Transition: InitEvent From:PoweredOnState To:ReadyForInitState Via: Action:none Guard:none
//External Transition: ReadyInitialTOScanIdleTransition From:ReadyStateInitial To:ScanIdleState Via: Action:none Guard:none
//External Transition: ReadyToStartScanTOScanIdleTransition From:ReadyToStartScanState To:ScanIdleState Via:DisableTriggeredScanEvent Action:none Guard:none
//External Transition: ScanIdleTOReadyToStartScanTransition From:ScanIdleState To:ReadyToStartScanState Via:EnableTriggeredScanEvent Action:none Guard:none
//External Transition: SendingResultsTOUnSucscribedIdleTransition From:SendingResultsState To:UnSucscribedIdleState Via:UnsubscribeFeatureVectorUpdatesEvent Action:none Guard:none
//External Transition: MoreResultsToSend_TOSubscribedIdleTransition From:MoreResultsToSend_ To:SubscribedIdleState Via: Action:none Guard:No
//External Transition: MoreResultsToSend_TOSendingResultsTransition From:MoreResultsToSend_ To:SendingResultsState Via: Action:none Guard:Yes
//External Transition: ScanningTOScanningTransition From:ScanningState To:ScanningState Via:StartOfLaserLineEvent Action:PulseStartOfLaserLineDO Guard:none
//External Transition: SendingResultsTOSendingResultsTransition From:SendingResultsState To:SendingResultsState Via:NewFeatureVectorsCreated Action:QueueNotification Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:EStopEvent Action:none Guard:none
//External Transition: UnSucscribedIdleTOUnSucscribedIdleTransition From:UnSucscribedIdleState To:UnSucscribedIdleState Via:GetFeatureVectorsEvent Action:none Guard:none
//External Transition: UnSucscribedIdleTOSubscribedIdleTransition From:UnSucscribedIdleState To:SubscribedIdleState Via:SubscribeFeatureVectorUpdatesEvent Action:none Guard:none
//External Transition: ReadyInitialTOUnSucscribedIdleTransition From:ReadyStateInitial To:UnSucscribedIdleState Via: Action:none Guard:none
//External Transition: ReadyForInitTOIsWarningOrCriticalAlarmTransition From:ReadyForInitState To:IsWarningOrCriticalAlarm Via:InitCompleteEvent Action:none Guard:none
//External Transition: PoweredOnInitialTOReadyForInitTransition From:PoweredOnStateInitial To:ReadyForInitState Via: Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarmTOFaultedTransition From:IsWarningOrCriticalAlarm To:FaultedState Via: Action:none Guard:Yes
//External Transition: ScanningTOScanIdleTransition From:ScanningState To:ScanIdleState Via:AbortScan Action:none Guard:none
//External Transition: SendingResultsTOMoreResultsToSend_Transition From:SendingResultsState To:MoreResultsToSend_ Via:SendFeatureVectorsEvent Action:none Guard:none
//External Transition: SubscribedIdleTOUnSucscribedIdleTransition From:SubscribedIdleState To:UnSucscribedIdleState Via:UnsubscribeFeatureVectorUpdatesEvent Action:none Guard:none
//External Transition: ScanIdleTOScanIdleTransition From:ScanIdleState To:ScanIdleState Via:EnableCmdTestMode / SetCmdTestCase Action:none Guard:none
//External Transition: ScanIdleTOScanIdleTransition From:ScanIdleState To:ScanIdleState Via:SubscribeXXX / UnsubscribeXXX Action:none Guard:none
//External Transition: ScanIdleTOScanIdleTransition From:ScanIdleState To:ScanIdleState Via:LoadRecipe / GetFeatureVectors / EnableXXX Action:none Guard:none
//External Transition: PoweredOnTOPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:ClearFaults Action:none Guard:none
//External Transition: PoweredOnTOPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:GetFaults Action:none Guard:none
//External Transition: ScanningTOScanningTransition From:ScanningState To:ScanningState Via:ScanProgressEventTimeOut Action:ScanProgressEvent Guard:none
//External Transition: PoweredOnTOPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:GetSWVer Action:none Guard:none
//External Transition: IsWarningOrCriticalAlarmTOReadyTransition From:IsWarningOrCriticalAlarm To:ReadyState Via: Action:none Guard:No
//External Transition: PoweredOnTOPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:GetModel Action:none Guard:none
//External Transition: SendingResultsTOSendingResultsTransition From:SendingResultsState To:SendingResultsState Via:GetFeatureVectorsEvent Action:none Guard:none
//External Transition: InitialTOPoweredOnTransition From:Initial To:PoweredOnState Via: Action:none Guard:none
//External Transition: PoweredOnTOPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:GetState Action:none Guard:none
//External Transition: SendingResultsTOSendingResultsTransition From:SendingResultsState To:SendingResultsState Via:ScanCompleteEvent Action:QueueNotification Guard:none
//External Transition: SubscribedIdleTOSubscribedIdleTransition From:SubscribedIdleState To:SubscribedIdleState Via:ScanCompleteEvent Action:SendNotification Guard:none
//External Transition: ScanningTOScanIdleTransition From:ScanningState To:ScanIdleState Via:Last Requested Line Scanned Action:Send_ScanCompleteEvent Guard:none
//External Transition: ReadyToStartScanTOScanningTransition From:ReadyToStartScanState To:ScanningState Via:StartScanDIPulsedEvent Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:CriticalFaultEvent Action:none Guard:none
//External Transition: PoweredOnTOPoweredOnTransition From:PoweredOnState To:PoweredOnState Via:GetHWVer Action:none Guard:none
