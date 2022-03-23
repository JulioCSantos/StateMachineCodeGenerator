///////////////////////////////////////////////////////////
//  Copyright © Corning Incorporated  2017
//  CStateMachineEventData.cs
//  Project CaliforniaSystem
//  Implementation of the Class CStateMachineEventData
//  Created on:      January 14, 2017 5:14:54 AM
///////////////////////////////////////////////////////////

using System.Threading;
using Corning.GenSys.Logger;
using NorthStateSoftware.NorthStateFramework;
using System;
using System.Collections.Generic;
using California.Interfaces;
using CaliforniaLensDisplay;
namespace CLEO.CLEOSystem
{
    public partial class CCLEOStateMachine
    {
        #region Fields
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CCLEOStateMachine");
        //private Semaphore m_semaphoreStateChange = new Semaphore();
        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);
        private object m_objLock = new object();
        private string m_strLastOperatorPrompt = "";

        private EState? m_estatePrevious;
        private EState? m_estateCurrent;
        private string m_strSystemState;
        private ICLEOSystem m_iCLEOSystem;

        private bool m_bSimulationMode = false;
        private Dictionary<CLEOSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<Proj3SystemEventsEnum, NSFEvent>();
        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        private bool m_bInitComplete = false;
        private bool m_bInitAfterSettingsComplete = false;

        #region State Machine Fields


        // ***********************************
        // State Machine NSF Event Definitions
        // ***********************************

        private static NSFEvent oWriteCompleteEvent;
        private static NSFEvent oTestCompleteEvent;
        private static NSFEvent oMoveCompleteEvent;
        private static NSFEvent oLoadEvent;
        private static NSFEvent oStartEvent;
        private static NSFEvent oCreateRefFileEvent;
        private static NSFEvent oAllAxisHomedEvent;
        private static NSFEvent oAbortEvent;
        private static NSFEvent oInitCompleteEvent;
        private static NSFEvent oCommitEvent;
        private static NSFEvent oInitAfterSettingsCompleteEvent;
        private static NSFEvent oClearFaultsEvent;
        private static NSFEvent oEStopEvent;
        private static NSFEvent oInitEvent;
        private static NSFEvent oAbortCompleteEvent;
        private static NSFEvent oMvoeCompleteEvent;
        private static NSFEvent oDoorClosedEvent;
        private static NSFEvent oSensorFaultEvent;
        private static NSFEvent oRefFileCompleteEvent;
        private static NSFEvent oBypassKeyEnabledEvent;
        private static NSFEvent oDoorOpenedEvent;
        private static NSFEvent oMotionFaultEvent;

        // ***********************************
        // End of State Machine NSF Event Definitions
        // ***********************************


        // ***********************************
        // State Machine NSF State Definitions
        // ***********************************

        // State Machine Upper Level Composite State Definitions
        private NSFCompositeState oHomeAllAxisState;
        private NSFCompositeState oInspectingState;
        private NSFCompositeState oPrepareState;
        private NSFCompositeState oSystemPoweredState;

        // State Machine CreateReferenceFileState State Definitions
        private NSFCompositeState       oMoveToLens1PosState;
        private NSFCompositeState       oCreateRefFileState;
        private NSFCompositeState       oRemoveCouponFirstErrorState;
        private NSFChoiceState          oIsCouponLoaded2_;
        private NSFChoiceState          oIsDoorClosed2_;
        private NSFCompositeState       oWaitForDoorClose4State;
        private NSFInitialState         oCreateReferenceFileStateInitial;

        // State Machine PrepareState State Definitions
        private NSFCompositeState       oCouponNotLoadedErrorState;
        private NSFChoiceState          oIsCouponLoaded_OR_BypassEnabled_;
        private NSFCompositeState       oWaitForDoorClose2State;
        private NSFChoiceState          oIsDoorClosed4_Or_IsBypassEnabled_;
        private NSFCompositeState       oCreateReferenceFileState;
        private NSFInitialState         oPrepareStateInitial;
        private NSFCompositeState       oIdleState;
        private NSFCompositeState       oLoadOrUnLoadState;

        // State Machine LoadOrUnLoadState State Definitions
        private NSFChoiceState          oIsDoorClosed3__OR___BypassKeyEnabled_AND__Login____Operator___;
        private NSFCompositeState       oWaitForDoorClose3State;
        private NSFCompositeState       oLoadCouponState;
        private NSFCompositeState       oMoveToLoadPosState;
        private NSFInitialState         oLoadOrUnLoadStateInitial;
        private NSFChoiceState          oIsDoorClosed_OR___BypassKeyEnabled_AND__Login____Operator___;
        private NSFCompositeState       oWaitForDoorCloseState;

        // State Machine IdleState State Definitions
        private NSFChoiceState          oIsRefFrameUpdatedToday_;
        private NSFInitialState         oIdleStateInitial;
        private NSFCompositeState       oRefFrameNeededState;
        private NSFCompositeState       oRefFrameUpdatedState;

        // State Machine InspectingState State Definitions
        private NSFChoiceState          oIsMoreLLToTest_;
        private NSFInitialState         oInspectingStateInitial;
        private NSFCompositeState       oWriteReportState;
        private NSFCompositeState       oAbortInspectingState;
        private NSFCompositeState       oMoveToLLState;
        private NSFCompositeState       oReviewState;
        private NSFCompositeState       oTestLLState;

        // State Machine SystemPoweredState State Definitions
        private NSFChoiceState          oIsCriticalAlarm_;
        private NSFCompositeState       oFaultedState;
        private NSFCompositeState       oInitAfterSettingsState;
        private NSFInitialState         oSystemPoweredStateInitial;
        private NSFCompositeState       oInitState;


        // ***********************************
        // End of State State Machine NSF State Definitions
        // ***********************************


        #endregion State Machine Fields

        #endregion Fields
    } //end CCLEOStateMachine
} //end CLEO.CLEOSystem
