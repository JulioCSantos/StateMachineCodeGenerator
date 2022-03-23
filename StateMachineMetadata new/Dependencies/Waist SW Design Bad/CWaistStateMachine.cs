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
namespace Waist.WaistSystem
{
    public partial class CWaistStateMachine
    {
        #region Fields
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CWaistStateMachine");
        //private Semaphore m_semaphoreStateChange = new Semaphore();
        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);
        private object m_objLock = new object();
        private string m_strLastOperatorPrompt = "";

        private EState? m_estatePrevious;
        private EState? m_estateCurrent;
        private string m_strSystemState;
        private IWaistSystem m_iWaistSystem;

        private bool m_bSimulationMode = false;
        private Dictionary<WaistSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<Proj3SystemEventsEnum, NSFEvent>();
        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        private bool m_bInitComplete = false;
        private bool m_bInitAfterSettingsComplete = false;

        #region State Machine Fields


        // ***********************************
        // State Machine NSF Event Definitions
        // ***********************************

        private static NSFEvent oClearFaultsEvent;
        private static NSFEvent oStartZHeightScanEvent;
        private static NSFEvent oReMatchingCompleteEvent;
        private static NSFEvent oReadCadFileCompleteEvent;
        private static NSFEvent oReMatchCadFileEvent;
        private static NSFEvent oInvalidCadFileEvent;
        private static NSFEvent oInitEvent;
        private static NSFEvent oScanCompleteEvent;
        private static NSFEvent oStartEvent;
        private static NSFEvent oEStopEvent;
        private static NSFEvent oAllAxisHomedEvent;
        private static NSFEvent oInitCompleteEvent;
        private static NSFEvent oInvalidResultFileEvent;
        private static NSFEvent oProcessingCompleteEvent;
        private static NSFEvent oAbortEvent;
        private static NSFEvent oReadResultFileCompleteEvent;
        private static NSFEvent oLoadResultsFileEvent;
        private static NSFEvent oMoveToParkCompleteEvent;
        private static NSFEvent oLoadCompleteEvent;
        private static NSFEvent oLoadEvent;
        private static NSFEvent oCommitEvent;
        private static NSFEvent oInitAfterSettingsCompleteEvent;
        private static NSFEvent oReportCompleteEvent;

        // ***********************************
        // End of State Machine NSF Event Definitions
        // ***********************************


        // ***********************************
        // State Machine NSF State Definitions
        // ***********************************

        // State Machine Upper Level Composite State Definitions
        private NSFCompositeState oLoadGlassState;
        private NSFCompositeState oHomeAllAxisState;
        private NSFCompositeState oInspectingState;
        private NSFCompositeState oIdleState;
        private NSFCompositeState oSystemPoweredState;

        // State Machine InspectingState State Definitions
        private NSFCompositeState       oReMatchingState;
        private NSFCompositeState       oReadCadFileState;
        private NSFCompositeState       oReadResultsFileState;
        private NSFCompositeState       oWriteReportState;
        private NSFCompositeState       oAbortScanState;
        private NSFCompositeState       oProcessingState;
        private NSFCompositeState       oScanningZHeightState;
        private NSFCompositeState       oDefectReviewState;
        private NSFCompositeState       oScanningWaistState;

        // State Machine IdleState State Definitions
        private NSFInitialState         oIdleStateInitial;
        private NSFCompositeState       oReadyToScanOrLoadGlassState;
        private NSFCompositeState       oMovingToParkState;

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
    } //end CWaistStateMachine
} //end Waist.WaistSystem
