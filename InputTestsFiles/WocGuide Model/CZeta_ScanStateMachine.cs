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
namespace Zeta_Scan.Zeta_ScanSystem
{
    public partial class CZeta_ScanStateMachine
    {
        #region Fields
        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CZeta_ScanStateMachine");
        //private Semaphore m_semaphoreStateChange = new Semaphore();
        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);
        private object m_objLock = new object();
        private string m_strLastOperatorPrompt = "";

        private EState? m_estatePrevious;
        private EState? m_estateCurrent;
        private string m_strSystemState;
        private IZeta_ScanSystem m_iZeta_ScanSystem;

        private bool m_bSimulationMode = false;
        private Dictionary<Zeta_ScanSystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<Proj3SystemEventsEnum, NSFEvent>();
        private Dictionary<NSFState, string> m_odictOperatorPromptForState;

        private bool m_bInitComplete = false;
        private bool m_bInitAfterSettingsComplete = false;

        #region State Machine Fields


        // ***********************************
        // State Machine NSF Event Definitions
        // ***********************************

        private static NSFEvent oNewFeatureVectorsCreated;
        private static NSFEvent oDisableTriggeredScanEvent;
        private static NSFEvent oEnableTriggeredScanEvent;
        private static NSFEvent oUnsubscribeFeatureVectorUpdatesEvent;
        private static NSFEvent oStartOfLaserLineEvent;
        private static NSFEvent oEStopEvent;
        private static NSFEvent oGetFeatureVectorsEvent;
        private static NSFEvent oSubscribeFeatureVectorUpdatesEvent;
        private static NSFEvent oInitCompleteEvent;
        private static NSFEvent oAbortScan;
        private static NSFEvent oSendFeatureVectorsEvent;
        private static NSFEvent oEnableCmdTestMode___SetCmdTestCase;
        private static NSFEvent oSubscribeXXX___UnsubscribeXXX;
        private static NSFEvent oLoadRecipe___GetFeatureVectors___EnableXXX;
        private static NSFEvent oClearFaults;
        private static NSFEvent oGetFaults;
        private static NSFEvent oScanProgressEventTimeOut;
        private static NSFEvent oGetSWVer;
        private static NSFEvent oGetModel;
        private static NSFEvent oGetState;
        private static NSFEvent oScanCompleteEvent;
        private static NSFEvent oLast_Requested_Line_Scanned;
        private static NSFEvent oStartScanDIPulsedEvent;
        private static NSFEvent oCriticalFaultEvent;
        private static NSFEvent oGetHWVer;

        // ***********************************
        // End of State Machine NSF Event Definitions
        // ***********************************


        // ***********************************
        // State Machine NSF State Definitions
        // ***********************************

        // State Machine Upper Level Composite State Definitions
        private NSFCompositeState oPoweredOnState;

        // State Machine ReadyState State Definitions
        private NSFCompositeState       oScanningState;
        private NSFCompositeState       oReadyToStartScanState;
        private NSFChoiceState          oMoreResultsToSend_;
        private NSFCompositeState       oSendingResultsState;
        private NSFCompositeState       oSubscribedIdleState;
        private NSFCompositeState       oUnSucscribedIdleState;
        private NSFCompositeState       oScanIdleState;
        private NSFInitialState         oReadyStateInitial;
        private NSFInitialState         oReadyStateInitial;

        // State Machine PoweredOnState State Definitions
        private NSFCompositeState       oReadyState;
        private NSFCompositeState       oReadyForInitState;
        private NSFInitialState         oPoweredOnStateInitial;


        // ***********************************
        // End of State State Machine NSF State Definitions
        // ***********************************


        #endregion State Machine Fields

        #endregion Fields
    } //end CZeta_ScanStateMachine
} //end Zeta_Scan.Zeta_ScanSystem
