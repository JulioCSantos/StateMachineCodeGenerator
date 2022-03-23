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
//OperatorManualLoadState: PromptManualLoad
//WaitForDoorOpenToLoadState: PromptToOpenDoor
//GlassLoadingFromCartState: StopCartLoadTimer, StartCartLoadTimer
//UnCompressTrailingEdgePreLoadsState: UnCompressTrailingEdgePreloads
//RetractTrailingEdgePinsState: RetractTrailingEdgePins
//OpenDoorState: PromptToOpenDoor
//WaitForGlassToEnterState: PromptToStartCartLoad
//GlassLoadingState: IsSafetyCircuitReset_, CloseDoorToAlignState, OperatorManualLoadState, IsDoorOpened_, WaitForDoorOpenToLoadState, GlassLoadingFromCartState, GlassLoadingStateInitial, AlignGlassState, WaitForGlassToEnterState
//GlassUnLoadingState: CloseDoorState, RemoveGlassState, PushGlassToDoorState, UnCompressSidePreLoadState, UnCompressTrailingEdgePreLoadsState, RetractTrailingEdgePinsState, GlassUnLoadingStateInitial
//InspectingState: InspectingStateInitial, DefectReviewState, MacroScanState
//Idle_State: IsGlassLoaded_, GlassUnLoadedState, Idle_StateInitial, GlassLoadedState
//ReadyState: StopAllMotion, InspectingState, ReadyStateInitial, Idle_State
//SystemPoweredState: IsWarningOrCriticalAlarm, ReadyState, FaultedState, InitAfterSettingsState, SystemPoweredStateInitial, InitState

//External Transition: RemoveGlassTOIdle_Transition From:RemoveGlassState To:Idle_State Via:GlassRemovedAtEntranceEvent Action:MoveAllToLoadPos Guard:none
//External Transition: GlassLoadedTOGlassUnLoadingTransition From:GlassLoadedState To:GlassUnLoadingState Via:UnloadGlassEvent Action:none Guard:CartDocked
//External Transition: IsDoorOpened_TOWaitForDoorOpenToLoadTransition From:IsDoorOpened_ To:WaitForDoorOpenToLoadState Via: Action:none Guard:No
//External Transition: GlassUnLoadedTOManualReviewStripeDefectsTransition From:GlassUnLoadedState To:ManualReviewStripeDefectsState Via:LoadStripeImageEvent Action:none Guard:none
//External Transition: CloseDoorToAlignTOAlignGlassTransition From:CloseDoorToAlignState To:AlignGlassState Via:DoorCloseEvent Action:none Guard:none
//External Transition: WaitForGlassToEnterTOIdle_Transition From:WaitForGlassToEnterState To:Idle_State Via:CartUnDockedEvent Action:none Guard:none
//External Transition: IsFaulted2TOGlassUnLoadingTransition From:IsFaulted2 To:GlassUnLoadingState Via: Action:none Guard:No
//External Transition: ReadyTOFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: WaitForGlassToEnterTOIdle_Transition From:WaitForGlassToEnterState To:Idle_State Via:AbortEvent Action:none Guard:none
//External Transition: ManualReviewStripeDefectsTOIsGlassLoaded_Transition From:ManualReviewStripeDefectsState To:IsGlassLoaded_ Via:ReviewCompleteEvent Action:none Guard:none
//External Transition: FaultWithGlassHeldTOFaultWithGlassLostTransition From:FaultWithGlassHeldState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: FaultAndOperatorClearsGlassTOIsWarningOrCriticalAlarmTransition From:FaultAndOperatorClearsGlassState To:IsWarningOrCriticalAlarm Via:ClearFaultsEvent Action:MoveAllToLoadPos Guard:IsNoWarningOrCriticalAlarms
//External Transition: FaultWithGlassLostTOFaultAndOperatorClearsGlassTransition From:FaultWithGlassLostState To:FaultAndOperatorClearsGlassState Via:ClearFaultsEvent Action:PromptToClearGlassAndClearFaultsAgain Guard:none
//External Transition: FaultWithGlassHeldTOGlassLoadedTransition From:FaultWithGlassHeldState To:GlassLoadedState Via:ClearFaultsEvent Action:none Guard:IsNoWarningOrCriticalAlarms
//External Transition: IsWarningOrCriticalAlarmTOFaultedTransition From:IsWarningOrCriticalAlarm To:FaultedState Via: Action:none Guard:Yes
//External Transition: IsCartDocked_TOGlassUnLoadedTransition From:IsCartDocked_ To:GlassUnLoadedState Via: Action:none Guard:No
//External Transition: GlassLoadedTOFaultWithGlassHeldTransition From:GlassLoadedState To:FaultWithGlassHeldState Via:Events (see Note for list) Action:none Guard:none
//External Transition: InitAfterSettingsTOIsWarningOrCriticalAlarmTransition From:InitAfterSettingsState To:IsWarningOrCriticalAlarm Via: Action:none Guard:none
//External Transition: InitTOInitAfterSettingsTransition From:InitState To:InitAfterSettingsState Via:InitCompleteEvent Action:none Guard:none
//External Transition: SystemPoweredInitialTOInitTransition From:SystemPoweredStateInitial To:InitState Via: Action:none Guard:none
//External Transition: InitialTOSystemPoweredTransition From:Initial To:SystemPoweredState Via: Action:none Guard:none
//External Transition: Idle_InitialTOIsGlassLoaded_Transition From:Idle_StateInitial To:IsGlassLoaded_ Via: Action:none Guard:none
//External Transition: ReadyTOFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:EStopEvent Action:none Guard:none
//External Transition: GlassLoadingTOFaultWithGlassLostTransition From:GlassLoadingState To:FaultWithGlassLostState Via:AbortEvent Action:NotifyLoadGlassAbortAlarm Guard:none
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:EStopEvent Action:none Guard:none
//External Transition: WaitForGlassToEnterTOGlassLoadingFromCartTransition From:WaitForGlassToEnterState To:GlassLoadingFromCartState Via:GlassDetectedAtEntranceEvent Action:EnableCartUnDockedAlarm Guard:none
//External Transition: DefectReviewTOGlassUnLoadingTransition From:DefectReviewState To:GlassUnLoadingState Via:UnLoadEvent Action:none Guard:none
//External Transition: IsGlassLoaded_TOGlassLoadedTransition From:IsGlassLoaded_ To:GlassLoadedState Via: Action:none Guard:Yes
//External Transition: IsGlassLoaded_TOIsCartDocked_Transition From:IsGlassLoaded_ To:IsCartDocked_ Via: Action:none Guard:No
//External Transition: ReadyInitialTOIdle_Transition From:ReadyStateInitial To:Idle_State Via: Action:none Guard:none
//External Transition: InspectingInitialTOMacroScanTransition From:InspectingStateInitial To:MacroScanState Via: Action:none Guard:none
//External Transition: GlassLoadedTOManualReviewStripeDefectsTransition From:GlassLoadedState To:ManualReviewStripeDefectsState Via:LoadStripeImageEvent Action:none Guard:none
//External Transition: FaultWithNoGlassTOIsWarningOrCriticalAlarmTransition From:FaultWithNoGlassState To:IsWarningOrCriticalAlarm Via:ClearFaultsEvent Action:none Guard:none
//External Transition: ReadyTOFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:MotionFaultEvent Action:NotifyMotionAlarm Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:PanelOpenEvent Action:none Guard:IsNotBypassed
//External Transition: MacroScanTOFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:VacuumOffEvent Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:CabinetOverTempEvent Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:AirOffEvent Action:none Guard:none
//External Transition: ReadyTOFaultedTransition From:ReadyState To:FaultedState Via:VacuumOffEvent Action:none Guard:none
//External Transition: IsFaulted2TOFaultWithGlassHeldTransition From:IsFaulted2 To:FaultWithGlassHeldState Via: Action:none Guard:Yes
//External Transition: ReadyTOFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:ScanSensorFaultEvent Action:NotifyScanSensorAlarm Guard:none
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:ScanSensorFaultEvent Action:NotifyScanSensorAlarm Guard:none
//External Transition: ReadyTOFaultWithGlassLostTransition From:ReadyState To:FaultWithGlassLostState Via:BypassSwitchOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:CabinetOverTempEvent Action:none Guard:none
//External Transition: PushGlassToDoorTOFaultWithGlassLostTransition From:PushGlassToDoorState To:FaultWithGlassLostState Via:MoveCompleteEvent Action:none Guard:GlassNotDetectedAtEntrance
//External Transition: RetractTrailingEdgePinsTOFaultWithGlassLostTransition From:RetractTrailingEdgePinsState To:FaultWithGlassLostState Via:RetractTrailingEdgePinsTimeOutEvent Action:none Guard:none
//External Transition: Idle_TOFaultWithGlassHeldTransition From:Idle_State To:FaultWithGlassHeldState Via:NotifyScanSensorAlarm Action:none Guard:ScanSensorFaultEvent
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:PanelOpenEvent Action:none Guard:none
//External Transition: UnCompressSidePreLoadTOUnCompressTrailingEdgePreLoadsTransition From:UnCompressSidePreLoadState To:UnCompressTrailingEdgePreLoadsState Via:MoveCompleteEvent Action:none Guard:none
//External Transition: GlassUnLoadingTOFaultWithGlassLostTransition From:GlassUnLoadingState To:FaultWithGlassLostState Via:AbortEvent Action:NotifyUnLoadGlassAbortAlarm Guard:none
//External Transition: GlassLoadedTOFaultWithGlassLostTransition From:GlassLoadedState To:FaultWithGlassLostState Via:GlassNotDetectedEvent Action:none Guard:none
//External Transition: AlignGlassTOFaultWithGlassLostTransition From:AlignGlassState To:FaultWithGlassLostState Via:DoorNotClosed Action:none Guard:none
//External Transition: MacroScanTOFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:DoorNotClosed Action:none Guard:none
//External Transition: GlassUnLoadingTOFaultWithGlassLostTransition From:GlassUnLoadingState To:FaultWithGlassLostState Via:MoveTimeOutEvent Action:none Guard:none
//External Transition: WaitForDoorOpenToLoadTOIdle_Transition From:WaitForDoorOpenToLoadState To:Idle_State Via:CartUnDockedEvent Action:none Guard:none
//External Transition: WaitForDoorOpenToLoadTOIdle_Transition From:WaitForDoorOpenToLoadState To:Idle_State Via:AbortEvent Action:none Guard:none
//External Transition: MacroScanTOFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:WindowNotClosedEvent Action:none Guard:IsNotAdminLogin
//External Transition: OperatorManualLoadTOIsSafetyCircuitReset_Transition From:OperatorManualLoadState To:IsSafetyCircuitReset_ Via:GlassDetectedAtDamperEvent Action:none Guard:none
//External Transition: MacroScanTOFaultWithGlassHeldTransition From:MacroScanState To:FaultWithGlassHeldState Via:LightOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: RetractTrailingEdgePinsTOPushGlassToDoorTransition From:RetractTrailingEdgePinsState To:PushGlassToDoorState Via:MoveCompleteEvent Action:none Guard:IsMAPreLoadNotCompressed
//External Transition: OpenDoorTOUnCompressSidePreLoadTransition From:OpenDoorState To:UnCompressSidePreLoadState Via:DoorOpenEvent Action:none Guard:none
//External Transition: RemoveGlassTOCloseDoorTransition From:RemoveGlassState To:CloseDoorState Via:GlassRemovedAtEntrance Action:none Guard:none
//External Transition: SystemPoweredTOInitTransition From:SystemPoweredState To:InitState Via:InitEvent Action:none Guard:IsAdminLogin
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:MotionFaultEvent Action:NotifyMotionAlarm Guard:none
//External Transition: InspectingTOFaultWithGlassHeldTransition From:InspectingState To:FaultWithGlassHeldState Via:BypassSwitchOnEvent Action:none Guard:IsNotAdminLogin
//External Transition: GlassLoadingFromCartTOIsSafetyCircuitReset_Transition From:GlassLoadingFromCartState To:IsSafetyCircuitReset_ Via:GlassDetectedAtDamperEvent Action:none Guard:none
//Internal Transition: ReadyStateOnmissingeventTransition. OwnedBy ReadyState. Action   Guard:none
