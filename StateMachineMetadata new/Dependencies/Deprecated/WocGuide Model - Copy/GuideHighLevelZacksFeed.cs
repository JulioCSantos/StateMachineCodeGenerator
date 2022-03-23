//GlassRemovedAtEntranceEvent
//UnloadGlassEvent
//LoadStripeImageEvent
//DoorCloseEvent
//CartUnDockedEvent
//GlassNotDetectedEvent
//AbortEvent
//ReviewCompleteEvent
//GlassNotDetectedEvent
//ClearFaultsEvent
//ClearFaultsEvent
//ClearFaultsEvent
//Events (see Note for list)
//InitCompleteEvent
//EStopEvent
//AbortEvent
//EStopEvent
//GlassDetectedAtEntranceEvent
//UnLoadEvent
//LoadStripeImageEvent
//ClearFaultsEvent
//MotionFaultEvent
//PanelOpenEvent
//VacuumOffEvent
//CabinetOverTempEvent
//AirOffEvent
//VacuumOffEvent
//ScanSensorFaultEvent
//ScanSensorFaultEvent
//BypassSwitchOnEvent
//CabinetOverTempEvent
//MoveCompleteEvent
//RetractTrailingEdgePinsTimeOutEvent
//NotifyScanSensorAlarm
//PanelOpenEvent
//MoveCompleteEvent
//AbortEvent
//GlassNotDetectedEvent
//DoorNotClosed
//DoorNotClosed
//MoveTimeOutEvent
//CartUnDockedEvent
//AbortEvent
//WindowNotClosedEvent
//GlassDetectedAtDamperEvent
//LightOnEvent
//MoveCompleteEvent
//DoorOpenEvent
//GlassRemovedAtEntrance
//InitEvent
//MotionFaultEvent
//BypassSwitchOnEvent
//GlassDetectedAtDamperEvent

//CloseDoorState: PromptCloseDoor
//RemoveGlassState: PromptRemoveGlass
//PushGlassToDoorState: PushGlassToDoor
//UnCompressSidePreLoadState: UnCompressSidePreload
//CloseDoorToAlignState: DisableMailSlotAlarms
//OperatorManualLoadState: PromptManualLoad; DisableCartAndMailSlotAlarm
//WaitForDoorOpenToLoadState: PromptToOpenDoor
//GlassLoadingFromCartState: StopCartLoadTimer, StartCartLoadTimer
//UnCompressTrailingEdgePreLoadsState: UnCompressTrailingEdgePreloads
//RetractTrailingEdgePinsState: RetractTrailingEdgePins; StartTimer
//OpenDoorState: PromptToOpenDoor; DisableMailSlotAlarms
//WaitForGlassToEnterState: PromptToStartCartLoad
//GlassLoadingState: IsSafetyCircuitReset?, CloseDoorToAlignState, OperatorManualLoadState, IsDoorOpened?, WaitForDoorOpenToLoadState, GlassLoadingFromCartState, GlassLoadingStateInitial, AlignGlassState, WaitForGlassToEnterState
//GlassUnLoadingState: CloseDoorState, RemoveGlassState, PushGlassToDoorState, UnCompressSidePreLoadState, UnCompressTrailingEdgePreLoadsState, RetractTrailingEdgePinsState, GlassUnLoadingStateInitial
//InspectingState: InspectingStateInitial, DefectReviewState, MacroScanState
//Idle State: IsGlassLoaded?, GlassUnLoadedState, Idle StateInitial, GlassLoadedState
//ReadyState: StopAllMotion, InspectingState, ReadyStateInitial, Idle State
//SystemPoweredState: IsWarningOrCriticalAlarm, ReadyState, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: RemoveGlassToIdleTransition From:RemoveGlassState To:Idle State Via:GlassRemovedAtEntranceEvent Action:MoveAllToLoadPos; PromptCloseDoor; EnableGlassSizeEntry Guard:none
//External Transition: GlassLoadedToGlassUnLoadingTransition From:GlassLoadedState To:GlassUnLoadingState Via:UnloadGlassEvent Action:none Guard:CartDocked
//External Transition: IsDoorOpenedToWaitForDoorOpenToLoadTransition From:IsDoorOpened? To:WaitForDoorOpenToLoadState Via: Action:none Guard:No
//External Transition: GlassUnLoadedToManualReviewStripeDefectsTransition From:GlassUnLoadedState To:ManualReviewStripeDefectsState Via:LoadStripeImageEvent Action:none Guard:none
//External Transition: CloseDoorToAlignToAlignGlassTransition From:CloseDoorToAlignState To:AlignGlassState Via:DoorCloseEvent Action:none Guard:none
//External Transition: WaitForGlassToEnterToIdleTransition From:WaitForGlassToEnterState To:Idle State Via:CartUnDockedEvent Action:none Guard:none
//External Transition: IsFaulted2ToGlassUnLoadingTransition From:IsFaulted2 To:GlassUnLoadingState Via: Action:none Guard:No
//External Transition: ReadyToFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: WaitForGlassToEnterToIdleTransition From:WaitForGlassToEnterState To:Idle State Via:AbortEvent Action:none Guard:none
//External Transition: ManualReviewStripeDefectsToIsGlassLoadedTransition From:ManualReviewStripeDefectsState To:IsGlassLoaded? Via:ReviewCompleteEvent Action:none Guard:none
//External Transition: FaultWithGlassHeldToFaultWithGlassLostTransition From:FaultWithGlassHeldState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: FaultAndOperatorClearsGlassToIsWarningOrCriticalAlarmTransition From:FaultAndOperatorClearsGlassState To:IsWarningOrCriticalAlarm Via:ClearFaultsEvent Action:MoveAllToLoadPos Guard:IsNoWarningOrCriticalAlarms
//External Transition: FaultWithGlassLostToFaultAndOperatorClearsGlassTransition From:FaultWithGlassLostState To:FaultAndOperatorClearsGlassState Via:ClearFaultsEvent Action:PromptToClearGlassAndClearFaultsAgain; DisableGlassNotDetectedAlarm Guard:none
//External Transition: FaultWithGlassHeldToGlassLoadedTransition From:FaultWithGlassHeldState To:GlassLoadedState Via:ClearFaultsEvent Action:none Guard:IsNoWarningOrCriticalAlarms
//External Transition: IsWarningOrCriticalAlarmToFaultedTransition From:IsWarningOrCriticalAlarm To:FaultedState Via: Action:none Guard:Yes
//External Transition: IsCartDockedToGlassUnLoadedTransition From:IsCartDocked? To:GlassUnLoadedState Via: Action:none Guard:No
//External Transition: GlassLoadedToFaultWithGlassHeldTransition From:GlassLoadedState To:FaultWithGlassHeldState Via:Events (see Note for list) Action:none Guard:none
//External Transition: InitAfterSettingsToIsWarningOrCriticalAlarmTransition From:InitAfterSettingsState To:IsWarningOrCriticalAlarm Via: Action:none Guard:none
//External Transition: InitToInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: SystemPoweredInitialToInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: InitialToSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: IdleInitialToIsGlassLoadedTransition From:Idle StateInitial To:IsGlassLoaded? Via: Action:none Guard:none
//External Transition: ReadyToFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:EStopEvent Action:none Guard:none
//External Transition: GlassLoadingToFaultWithGlassLostTransition From:GlassLoadingState To:FaultWithGlassLostState Via:AbortEvent Action:NotifyLoadGlassAbortAlarm Guard:none
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:EStopEvent Action:none Guard:none
//External Transition: WaitForGlassToEnterToGlassLoadingFromCartTransition From:WaitForGlassToEnterState To:GlassLoadingFromCartState Via:GlassDetectedAtEntranceEvent Action:EnableCartUnDockedAlarm Guard:none
//External Transition: DefectReviewToGlassUnLoadingTransition From:DefectReviewState To:GlassUnLoadingState Via:UnLoadEvent Action:none Guard:none
//External Transition: IsGlassLoadedToGlassLoadedTransition From:IsGlassLoaded? To:GlassLoadedState Via: Action:none Guard:Yes
//External Transition: IsGlassLoadedToIsCartDockedTransition From:IsGlassLoaded? To:IsCartDocked? Via: Action:none Guard:No
//External Transition: ReadyInitialToIdleTransition From:ReadyStateInitial To:Idle State Via: Action:none Guard:none
//External Transition: InspectingInitialToMacroScanTransition From:InspectingStateInitial To:MacroScanState Via: Action:none Guard:none
//External Transition: GlassLoadedToManualReviewStripeDefectsTransition From:GlassLoadedState To:ManualReviewStripeDefectsState Via:LoadStripeImageEvent Action:none Guard:none
//External Transition: FaultWithNoGlassToIsWarningOrCriticalAlarmTransition From:FaultWithNoGlassState To:IsWarningOrCriticalAlarm Via:ClearFaultsEvent Action:none Guard:none
//External Transition: ReadyToFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:MotionFaultEvent Action:NotifyMotionAlarm Guard:none
//External Transition: ReadyToFaultedTransition From:ReadyState To:FaultedState Via:PanelOpenEvent Action:none Guard:IsNotBypassed
//External Transition: MacroScanToFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:VacuumOffEvent Action:none Guard:none
//External Transition: ReadyToFaultedTransition From:ReadyState To:FaultedState Via:CabinetOverTempEvent Action:none Guard:none
//External Transition: ReadyToFaultedTransition From:ReadyState To:FaultedState Via:AirOffEvent Action:none Guard:none
//External Transition: ReadyToFaultedTransition From:ReadyState To:FaultedState Via:VacuumOffEvent Action:none Guard:none
//External Transition: IsFaulted2ToFaultWithGlassHeldTransition From:IsFaulted2 To:FaultWithGlassHeldState Via: Action:none Guard:Yes
//External Transition: ReadyToFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:ScanSensorFaultEvent Action:NotifyScanSensorAlarm Guard:none
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:ScanSensorFaultEvent Action:NotifyScanSensorAlarm Guard:none
//External Transition: ReadyToFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:BypassSwitchOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:CabinetOverTempEvent Action:none Guard:none
//External Transition: PushGlassToDoorToFaultWithGlassLostTransition From:PushGlassToDoorState To:FaultWithGlassLostState Via:MoveCompleteEvent Action:none Guard:GlassNotDetectedAtEntrance
//External Transition: RetractTrailingEdgePinsToFaultWithGlassLostTransition From:RetractTrailingEdgePinsState To:FaultWithGlassLostState Via:RetractTrailingEdgePinsTimeOutEvent Action:none Guard:none
//External Transition: IdleToFaultWithGlassHeldTransition From:Idle State To:FaultWithGlassHeldState Via:NotifyScanSensorAlarm Action:none Guard:ScanSensorFaultEvent
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:PanelOpenEvent Action:none Guard:none
//External Transition: UnCompressSidePreLoadToUnCompressTrailingEdgePreLoadsTransition From:UnCompressSidePreLoadState To:UnCompressTrailingEdgePreLoadsState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: GlassUnLoadingToFaultWithGlassLostTransition From:GlassUnLoadingState To:FaultWithGlassLostState Via:AbortEvent Action:NotifyUnLoadGlassAbortAlarm Guard:none
//External Transition: GlassLoadedToFaultWithGlassLostTransition From:GlassLoadedState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: AlignGlassToFaultWithGlassLostTransition From:AlignGlassState To:FaultWithGlassLostState Via:DoorNotClosed Action:none Guard:none
//External Transition: MacroScanToFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:DoorNotClosed Action:none Guard:none
//External Transition: GlassUnLoadingToFaultWithGlassLostTransition From:GlassUnLoadingState To:FaultWithGlassLostState Via:MoveTimeOutEvent Action:none Guard:none
//External Transition: WaitForDoorOpenToLoadToIdleTransition From:WaitForDoorOpenToLoadState To:Idle State Via:CartUnDockedEvent Action:none Guard:none
//External Transition: WaitForDoorOpenToLoadToIdleTransition From:WaitForDoorOpenToLoadState To:Idle State Via:AbortEvent Action:none Guard:none
//External Transition: MacroScanToFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:WindowNotClosedEvent Action:none Guard:IsNotAdminLogin
//External Transition: OperatorManualLoadToIsSafetyCircuitResetTransition From:OperatorManualLoadState To:IsSafetyCircuitReset? Via:GlassDetectedAtDamperEvent Action:none Guard:none
//External Transition: MacroScanToFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:LightOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: RetractTrailingEdgePinsToPushGlassToDoorTransition From:RetractTrailingEdgePinsState To:PushGlassToDoorState Via:MoveCompleteEvent Action:none Guard:IsMAPreLoadNotCompressed
//External Transition: OpenDoorToUnCompressSidePreLoadTransition From:OpenDoorState To:UnCompressSidePreLoadState Via:DoorOpenEvent Action:none Guard:none
//External Transition: RemoveGlassToCloseDoorTransition From:RemoveGlassState To:CloseDoorState Via:GlassRemovedAtEntrance Action:none Guard:none
//External Transition: SystemPoweredToInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:MotionFaultEvent Action:NotifyMotionAlarm Guard:none
//External Transition: InspectingToFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:BypassSwitchOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: GlassLoadingFromCartToIsSafetyCircuitResetTransition From:GlassLoadingFromCartState To:IsSafetyCircuitReset? Via:GlassDetectedAtDamperEvent Action:none Guard:none
//Internal Transition: ReadyStateOnGUITransition. OwnedBy ReadyState. Action enforces
