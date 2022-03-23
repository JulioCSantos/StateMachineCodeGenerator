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
namespace Scan.ScanSystem
{
    public partial class CScanStateMachine
    {
        #region Fields
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CScanStateMachine");
        //private Semaphore m_semaphoreStateChange = new Semaphore();
        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);
        private object m_objLock = new object();
        private string m_strLastOperatorPrompt = "";

        private EState? m_estatePrevious;
        private EState? m_estateCurrent;
        private string m_strSystemState;
        private IScanSystem m_iScanSystem;

        private bool m_bSimulationMode = false;
        private Dictionary<ScanSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<Proj3SystemEventsEnum, NSFEvent>();
        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        private bool m_bInitComplete = false;
        private bool m_bInitAfterSettingsComplete = false;

        #region State Machine Fields


        // ***********************************
        // State Machine NSF Event Definitions
        // ***********************************

        private static NSFEvent oAbortEvent;
        private static NSFEvent oAcqCompleteTimeOutEvent;
        private static NSFEvent oAcqCompleteEvent;
        private static NSFEvent oSettleTimeCompleteEvent;
        private static NSFEvent oMoveCompleteEvent;
        private static NSFEvent oStartScanEvent;

        // ***********************************
        // End of State Machine NSF Event Definitions
        // ***********************************


        // ***********************************
        // State Machine NSF State Definitions
        // ***********************************

        // State Machine Upper Level Composite State Definitions
        private NSFCompositeState oSystemPoweredState;

        // State Machine ScanState State Definitions
        private NSFChoiceState          oIsMoreToScan_;
        private NSFCompositeState       oWaitForAcqCompleteState;
        private NSFCompositeState       oWaitSettleTimeState;
        private NSFChoiceState          oIsLineScanType_;
        private NSFCompositeState       oMoveToStartPosState;
        private NSFInitialState         oScanStateInitial;

        // State Machine SystemPoweredState State Definitions
        private NSFCompositeState       oScanState;
        private NSFInitialState         oSystemPoweredStateInitial;
        private NSFCompositeState       oIdleState;


        // ***********************************
        // End of State State Machine NSF State Definitions
        // ***********************************


        #endregion State Machine Fields

        #endregion Fields
    } //end CScanStateMachine
} //end Scan.ScanSystem
