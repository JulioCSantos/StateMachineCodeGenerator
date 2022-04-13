///////////////////////////////////////////////////////////
//  Copyright © Corning Incorporated  2016
//  CStateMachineCodeGeneratorSystem.cs
//  Project StateMachineCodeGeneratorSystem
//  Implementation of the Class CStateMachineCodeGeneratorSystem
//  Created on:      November 23, 2016 5:36:03 AM
///////////////////////////////////////////////////////////

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Corning.GenSys.DirCleaner;
using Corning.GenSys.Initialization;
using Corning.GenSys.Language;
using Corning.GenSys.Logger;
using Corning.GenSys.Motion;
using Corning.GenSys.Point;
using Corning.GenSys.SettingAttribute;
using Corning.GenSys.SettingHelperPoint;
using Corning.GenSys.FileArchiver;
using Corning.GenSys.SystemSettings;
using Corning.GenSys.Threading;

using StateMachineCodeGenerator.Components.Io;
using StateMachineCodeGenerator.Components.Motion;
using StateMachineCodeGenerator.Components.Points;
using StateMachineCodeGenerator.Interfaces;
using StateMachineCodeGenerator.Components.RuleEngine;

using IStateMachineCodeGenerator;
using StateMachineCodeGeneratorSystem;
using StateMachineCodeGeneratorSystem.Templates;

namespace StateMachineCodeGenerator.StateMachineCodeGeneratorSystem
{
    public class CStateMachineCodeGeneratorSystem : IStateMachineCodeGeneratorSystem
    {
        #region Fields

        //private string m_strRuleEngineSettingsDir;
        public const string m_strLogDirPathXMLName = "LogDirPath";

        public string m_strLogDirPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Corning Incorporated\StateMachineCodeGenerator\logs";
        private const string m_strMotionFaultAlarmName = "System.Alarms.MotionFaultAlarm";
        private const string m_strSampleAlarmName = "System.Alarms.SampleAlarm";
        private const string m_strScanAbortAlarmName = "System.Alarms.ScanAbortAlarm";
        private const string m_strScanSensorFaultAlarmName = "System.Alarms.ScanSensorFaultAlarm";

        private const string m_strDefaultStateMachineCodeGeneratorDataDir = "c:\\StateMachineCodeGeneratorData\\Data";

        // Make this string accessible outside of this class via the const (i.e. static) designation
        private static string m_strCorningProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Corning Incorporated\StateMachineCodeGenerator";

        private static ILogger ms_iLogger = CLoggerFactory.CreateLog("CStateMachineCodeGeneratorSystem");
        private static string ms_strRuleEngineSettingsDirOriginal = @"RuleEngineXml";
        //private readonly IPointsServerLocal m_iPointServerLocal;

        // Processing Tasks
        private CThreadHandler<bool, int>[] m_aoSampleTask;
        private CThreadHandler m_oApplicationTaskThread = null; // This was included as part of a Quick Start Guide any may be removed.

        private bool m_bDataModifiedByOperator;
        private bool m_bHomeInProgress;
        private BlockingCollection<IStateMachineCodeGeneratorData> m_blocoliStateMachineCodeGeneratorData;
        private BlockingCollection<IStateMachineCodeGeneratorData> m_blocoliStateMachineCodeGeneratorDataProcessed;
        private bool m_bManualMotionEnable = true;
        private bool m_bSimulationMode = false;
        private bool m_bSimulationOverrideActualAlarms = false;
        private bool m_bStateSimulationIsAdminLogin = true;
        private bool m_bStateSimulationIsDoorClosed = false;
        private bool m_bStateSimulationIsDoorOpened = false;
        private bool m_bStateSimulationIsHomed = true;
        private bool m_bStateSimulationIsCriticalAlarm = false;
        private bool m_bStateSimulationIsWarningOrCriticalAlarm = false;
        //private DateTime m_datetimeStartOfScan;
        private ELoginLevel m_eLoginLevel = ELoginLevel.OPERATOR;
        private bool m_bGuiConnectedYet = false;
        private bool m_bInitAfterSettingsAlreadyCompleted = false;
        private bool m_bInitAfterSettingsEventSent = false;
        private bool m_bIsSystemShutdown = false;
        private int m_nTaskIterationCount = 1;

        //private CLanguage m_iLanguage;
        private CLanguageDummy m_iLanguage;
        private List<IInitialization> m_lstiInitialization = new List<IInitialization>(); // A List of IInitializations so Init and InitAfterSettings can be called.
        private IPAddress m_ipaddressLocal = IPAddress.Parse("127.0.0.1");
        private IPoint m_iPointMotionFaultAlarm;
        private IPointProxyServer m_iPointProxyServer;
        private IPoint m_iPointSampleAlarm;
        private IPoint m_iPointScanAbortAlarm;
        private IPoint m_iPointScanSensorFaultAlarm;
        private ISystemSettings m_iSettingsManager;
        private IStateMachineCodeGeneratorIo m_iStateMachineCodeGeneratorIo;
        private IStateMachineCodeGeneratorMotion m_iStateMachineCodeGeneratorMotion;
        private IStateMachineCodeGeneratorRuleEngine m_iStateMachineCodeGeneratorRuleEngineSample;
        private CAlarmManager m_oAlarmManager;
        private CCoordSystem m_oCCoordSystemCoordSystem;
        //  TODO:  Put this back in when data output is setup for GenSysEasy-->  private CDirCleaner m_oDataOutputDirCleaner;
        private CDirCleaner m_oLogDirectoryCleaner;
        //private CPointProxyServer m_oPointProxyServer;
        private CPointsForIo m_oPointsForIo;
        private CPointsForMotion m_oPointsForMotion;
        private CPointsServer m_oPointsServer;
        private CThreadHandler m_oSystemShutdownTask;
        //private CDataOutputManager m_oDataOutputManager;
        //private IStateMachineCodeGeneratorGui m_iStateMachineCodeGeneratorGui;
        private CStateMachineCodeGeneratorCalcs m_oStateMachineCodeGeneratorCalcs;
        private IFileUpdater m_iOwnerSourceCodeFileUpdater;
        private IFileUpdater m_iOwnerInterfaceFileUpdater;
        private IFileUpdater m_iStateMachineSourceCodeFileUpdater;
        private IFileUpdater m_iStateMachineInterfaceFileUpdater;

        private string m_strStateMachineName = "";
        private string m_strStateMachineNamespaceName = "";
        private string m_strStateMachineInterfaceNamespaceName
        {
            get
            {
                var tokens = m_strStateMachineNamespaceName.Split('.');
                if (tokens.Length == 2) return $"{tokens[0]}.Interfaces";
                else return m_strStateMachineNamespaceName; // I don't know what to do here. 
            }
        }

        /// <summary>
        ///
        /// </summary>
        private string m_strStateMachineInterfaceFilePath = "";      // The Filename and Path of the State Machine's Interface
        private string m_strStateMachineSourceFilePath = "";         // The Filename and Path of the State Machine's Source Code
        private string m_strStateMachineOwnerInterfaceFilePath = ""; // The Filename and Path of the State Machine's Owner's Interface
        private string m_strStateMachineOwnerSourceFilePath = "";    // The Filename and Path of the State Machine's Owner's Source Code



        private List<string> m_lstStates = new List<string>();
        private List<string> m_lstStateType = new List<string>();
        private List<string> m_lstParentStates = new List<string>();
        private List<string> m_lstTransitionTypes = new List<string>();
        private List<string> m_lstTransitionFromStates = new List<string>();
        private List<string> m_lstTransitionToStates = new List<string>();
        private List<string> m_lstTransitionEvents = new List<string>();
        private List<string> m_lstTransitionGuards = new List<string>();
        private List<string> m_lstTransitionActions = new List<string>();
        //private List<string> m_lstStateNames = new List<string>();
        private List<string> m_lstEventNames = new List<string>();
        private List<string> m_lstEventNamesWithoutDataType = new List<string>();
        private List<string> m_lstOperatorPromptStates = new List<string>();
        private List<string> m_lstOperatorPromptText = new List<string>();
        private Dictionary<string, string> m_dictDataEventNameAndDataType = new Dictionary<string, string>();


        private CStateMachineCodeGeneratorStateMachine m_oStateMachineCodeGeneratorStateMachine;
        private string m_strOfflineViewFilePath = "";
        private string m_strStateMachineCodeGeneratorSettingsFileName = "StateMachineCodeGeneratorSettings.xml";
        private string m_strStateMachineCodeGeneratorSettingsPathAndFileName;
        private string m_strStateMachineSettingsPathAndFileName;
        private System.Timers.Timer m_timerStackLight = new System.Timers.Timer();

        // This member variable purposely placed with the properties to that they are serialzied together in the same group
        [ASetting(Name = "Logging.LogDirectoryCleaningInterval", Description = "Log Directory Cleaning Interval (units defined in another setting)", Units = "", MinVal = 1.0, MaxVal = 100000.0, ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private double m_dLogDirectoryCleaningInterval = 1.0;

        [ASetting(Name = "Logging.LogDirRetentionTime", Description = "Log Dir Retention Time (units defined in another setting)", Units = "", MinVal = 1.0, MaxVal = 100000.0, ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private double m_oLogDirRetentionTime = 999.0;

        // This member variable purposely placed with the properties to that they are serialzied together in the same group
        [ASetting(Name = "Logging.LogDirCleaningIntervalUnits_MinutesHoursDays", Description = "Log Dir Cleaning Interval Units MinutesHoursDays", Units = "MinutesHoursDays", ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private string m_strLogDirectoryCleaningIntervalUnits_MinutesHoursDays = "DAYS";

        [ASetting(Name = "Logging.LogDirRetentionUnits_MinutesHoursDays", Description = "Log Dir Retention Unit", Units = "MinutesHoursDays", ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private string m_strLogDirRetentionUnits_MinutesHoursDays = "Days";

        // This member variable purposely placed with the properties to that they are serialzied together in the same group
        [ASetting(Name = "Logging.LogDirMaxFilesAllowed", Description = "NLog Dir Max Files Allowed", Units = "files", ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private uint m_uLogDirMaxFilesAllowed = 99999999;

        // This member variable purposely placed with the properties to that they are serialzied together in the same group
        [ASetting(Name = "Logging.LogDirMaxSizeAllowed", Description = "NLog Dir Max Size Allowed", Units = "bytes", ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private ulong m_uLogDirMaxSizeAllowed = 99999999;

        // This member variable purposely placed with the properties to that they are serialzied together in the same group
        [ASetting(Name = "DataOutput.DirCleaningInterval", Description = "Data Output Directory Cleaning Interval (units defined in another setting)", Units = "", MinVal = 1.0, MaxVal = 100000.0, ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private double m_dDataOutputDirectoryCleaningInterval = 1.0;

        [ASetting(Name = "DataOutput.DirRetentionTime", Description = "Data Output Dir Retention Time (units defined in another setting)", Units = "", MinVal = 1.0, MaxVal = 100000.0, ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private double m_oDataOutputDirRetentionTime = 999.0;

        // This member variable purposely placed with the properties to that they are serialzied together in the same group
        [ASetting(Name = "DataOutput.DirCleaningIntervalUnits_MinutesHoursDays", Description = "Data Output Dir Cleaning Interval Units MinutesHoursDays", Units = "MinutesHoursDays", ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private string m_strDataOutputDirectoryCleaningIntervalUnits_MinutesHoursDays = "DAYS";

        [ASetting(Name = "DataOutput.DirRetentionUnits_MinutesHoursDays", Description = "Data Output Dir Retention Unit", Units = "MinutesHoursDays", ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private string m_strDataOutputDirRetentionUnits_MinutesHoursDays = "Days";

        // This member variable purposely placed with the properties to that they are serialzied together in the same group
        [ASetting(Name = "DataOutput.DirMaxFilesAllowed", Description = "Data Output Dir Max Files Allowed", Units = "files", ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private uint m_uDataOutputDirMaxFilesAllowed = 99999999;

        // This member variable purposely placed with the properties to that they are serialzied together in the same group
        [ASetting(Name = "DataOutput.DirMaxSizeAllowed", Description = "Data Output Dir Max Size Allowed", Units = "bytes", ReadAccess = EAccess.Admin | EAccess.Engineer)]
        private ulong m_uDataOutputDirMaxSizeAllowed = 250000000;

        // Task related settings
        [ASetting(Name = "Processing.NumSampleProcessingThreads", Description = "Number Of sample processing Threads", Units = "", MinVal = 1, MaxVal = 50, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private int m_nNumSingleChanCalcThreads = 8;

        [ASetting(Name = "Processing.RuleEngineSettingDir", Description = "Directory where the rule engine settings are stored", ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private string m_strRuleEngineSettingsDir = Path.Combine(m_strCorningProgramDataPath, @"Scripts");

        private string m_dataOutputStatus="";

        private double m_dblGlassOriginX;

        private double m_dblGlassOriginY;

        [ASetting(Name = "Motion.Defaults.HomingTimeout", Description = "The time in seconds to wait for HomeComplete before a timeout occurs.", Units = "s", IncVal = 1, MinVal = 1, MaxVal = 999, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionHomingTimeout = 300.0;

        [ASetting(Name = "Motion.Defaults.MoveTimeout", Description = "The time in seconds to wait for MoveComplete before a timeout occurs.", Units = "s", IncVal = 1, MinVal = 1, MaxVal = 999, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionMoveTimeout = 60.0;

        [ASetting(Name = "Motion.Defaults.XAxisAcceleration", Description = "X Axis Default Acceleration", Units = "mm/s�", IncVal = 0.1, MinVal = 0.1, MaxVal = 1000, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionXAxisAcceleration = 1000.0;

        [ASetting(Name = "Motion.Limits.XAxisLowerLimit", Description = "X Axis Lower Limit", Units = "mm", IncVal = 0.1, MinVal = -9999, MaxVal = 9999, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionXAxisLowerLimit = -1335.1;

        // Limits
        [ASetting(Name = "Motion.Limits.XAxisUpperLimit", Description = "X Axis Upper Limit", Units = "mm", IncVal = 0.1, MinVal = -9999, MaxVal = 9999, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionXAxisUpperLimit = 0.1;

        [ASetting(Name = "Motion.Defaults.XAxisVelocity", Description = "X Axis Default Velocity", Units = "mm/s�", IncVal = 0.1, MinVal = 0.1, MaxVal = 300, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionXAxisVelocity = 300.0;

        [ASetting(Name = "Motion.Defaults.YAxisAcceleration", Description = "Y Axis Default Acceleration", Units = "mm/s�", IncVal = 0.1, MinVal = 0.1, MaxVal = 1000, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionYAxisAcceleration = 1000.0;

        [ASetting(Name = "Motion.Limits.YAxisLowerLimit", Description = "Y Axis Lower Limit", Units = "mm", IncVal = 0.1, MinVal = -9999, MaxVal = 9999, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionYAxisLowerLimit = -1200.1;

        [ASetting(Name = "Motion.Limits.YAxisUpperLimit", Description = "Y Axis Upper Limit", Units = "mm", IncVal = 0.1, MinVal = -9999, MaxVal = 9999, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionYAxisUpperLimit = 0.1;

        [ASetting(Name = "Motion.Defaults.YAxisVelocity", Description = "Y Axis Default Velocity", Units = "mm/s�", IncVal = 0.1, MinVal = 0.1, MaxVal = 300, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionYAxisVelocity = 200.0;

        [ASetting(Name = "Motion.Defaults.ZAxisAcceleration", Description = "Z Axis Default Acceleration", Units = "mm/s�", IncVal = 0.1, MinVal = 0.1, MaxVal = 1000, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionZAxisAcceleration = 500.0;

        [ASetting(Name = "Motion.Limits.ZAxisLowerLimit", Description = "Z Axis Lower Limit", Units = "mm", IncVal = 0.1, MinVal = -9999, MaxVal = 9999, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionZAxisLowerLimit = -21.5;

        [ASetting(Name = "Motion.Limits.ZAxisUpperLimit", Description = "Z Axis Upper Limit", Units = "mm", IncVal = 0.1, MinVal = -9999, MaxVal = 9999, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionZAxisUpperLimit = 4.5;

        [ASetting(Name = "Motion.Defaults.ZAxisVelocity", Description = "Z Axis Default Velocity", Units = "mm/s�", IncVal = 0.1, MinVal = 0.1, MaxVal = 100, ReadAccess = EAccess.Admin | EAccess.Engineer, WriteAccess = EAccess.Admin)]
        private double m_dblMotionZAxisVelocity = 5.0;

        private string m_StateMachineCodeGeneratorDataDataDir;

        private bool m_isInOfflineViewerMode;

        private bool m_isSensorReady;

        private bool m_simulateSample1;

        // Default Velocity/Acceleration
        [ASetting(Name = "Motion.Defaults.IoTaskFilePath", Description = @"The FilePath for the A3200 task responsible for updating the StateMachineCodeGenerator Io.", Units = "", ReadAccess = EAccess.Admin, WriteAccess = EAccess.Admin)]
        private string m_strMotionIoTaskFilePath = @"C:\ProgramData\Corning Incorporated\StateMachineCodeGenerator\A3200\StateMachineCodeGenerator_IO.PGM";

        private bool m_unLoadGlassOnGuiEnable;

        public event LoginLevelChangeEventHandler eventLoginLevelChange;

        #region Log Settings
        #endregion Log Settings

        #region DataOutputSettings
        #endregion DataOutputSettings

        #region Processing Settings
        #endregion Processing Settings

        #region Motion Settings
        #endregion Motion Settings

        #endregion Fields

        #region Properties
        //[ASetting(Name = "DataOutput.StateMachineCodeGeneratorDataRootDir", Description = "Root directory where all StateMachineCodeGenerator data as well as matching config files are stored")]
        //public string StateMachineCodeGeneratorDataRootDir
        //{
        //    get { return m_oDataOutputManager == null ? m_strDefaultStateMachineCodeGeneratorDataDir : m_oDataOutputManager.StateMachineCodeGeneratorDataRootDir; }
        //    set
        //    {
        //        if (m_oDataOutputManager != null) m_oDataOutputManager.StateMachineCodeGeneratorDataRootDir = value;
        //    }
        //}

        //public string StateMachineCodeGeneratorDataDataDir
        //{
        //    get { return m_oDataOutputManager == null ? Path.Combine(m_strDefaultStateMachineCodeGeneratorDataDir, "Data") : m_oDataOutputManager.StateMachineCodeGeneratorDataDataDir; }
        //}

        //public string StateMachineCodeGeneratorDataConfigDir
        //{
        //    get { return m_oDataOutputManager == null ? Path.Combine(m_strDefaultStateMachineCodeGeneratorDataDir, "Config") : m_oDataOutputManager.StateMachineCodeGeneratorDataConfigDir; }
        //}
        public static string VersionMajorS
        {
            get
            {
                Version oVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("v.{0:000}.{1:000}", oVersion.Major, oVersion.Minor); //"v1.042";
            }
        }

        public static string VersionS
        {
            get
            {
                Version oVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("v.{0:000}.{1:000}.{2}.{3}", oVersion.Major, oVersion.Minor, oVersion.Build, oVersion.Revision);
            }
        }



        public string StateMachineName
        {
            get
            {
                return m_strStateMachineName;
            }

            set
            {
                m_strStateMachineName = value;
            }
        }


        public string StateMachineNamespaceName
        {
            get { return m_strStateMachineNamespaceName; }
            set { m_strStateMachineNamespaceName = value; }
        }





        //m_oStateMachineNamespaceName_textBox










        /// <summary>
        /// The Filename and Path of the State Machine's Interface
        /// </summary>
        public string StateMachineInterfaceFilePath
        {
            get { return m_strStateMachineInterfaceFilePath; }
            set { m_strStateMachineInterfaceFilePath = value; }
        }



        /// <summary>
        /// The Filename and Path of the State Machine's Source Code
        /// </summary>
        public string StateMachineSourceFilePath
        {
            get { return m_strStateMachineSourceFilePath; }
            set { m_strStateMachineSourceFilePath = value; }
        }


        /// <summary>
        /// The Filename and Path of the State Machine's Owner's Interface
        /// </summary>
        public string StateMachineOwnerInterfaceFilePath
        {
            get { return m_strStateMachineOwnerInterfaceFilePath; }
            set { m_strStateMachineOwnerInterfaceFilePath = value; }
        }


        /// <summary>
        /// he Filename and Path of the State Machine's Owner's Source Code
        /// </summary>
        public string StateMachineOwnerSourceFilePath
        {
            get { return m_strStateMachineOwnerSourceFilePath; }
            set { m_strStateMachineOwnerSourceFilePath = value; }
        }

        /// <summary>
        /// The Filename and Path of the State Machine's Interface
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private string m_strEAXMLFilePath;
        public string EAXMLFilePath
        {
            get { return m_strEAXMLFilePath; }
            set { m_strEAXMLFilePath = value; }
        }

        //public List<string> StateNames
        //{
        //    get
        //    {
        //        return m_lstStateNames;
        //    }
        //    set
        //    {
        //        m_lstStateNames = value;
        //    }
        //}


        public List<string> EventNames
        {
            get
            {
                return m_lstEventNames;
            }
            set
            {
                m_lstEventNames = value;
            }
        }

        public List<string> EventNamesWithoutDataType
        {
            get
            {
                return m_lstEventNamesWithoutDataType;
            }
            set
            {
                m_lstEventNamesWithoutDataType = value;
            }
        }



        public List<string> States
        {
            get
            {
                return m_lstStates;
            }
            set
            {
                m_lstStates = value;
            }
        }


        public List<string> StateTypes
        {
            get
            {
                return m_lstStateType;
            }
            set
            {
                m_lstStateType = value;
            }
        }







        public List<string> ParentStates
        {
            get
            {
                return m_lstParentStates;
            }
            set
            {
                m_lstParentStates = value;
            }
        }

        //
        public List<string> TransitionTypes
        {
            get
            {
                return m_lstTransitionTypes;
            }
            set
            {
                m_lstTransitionTypes = value;
            }
        }


        public List<string> TransitionFromStates
        {
            get
            {
                return m_lstTransitionFromStates;
            }
            set
            {
                m_lstTransitionFromStates = value;
            }
        }


        public List<string> TransitionToStates
        {
            get
            {
                return m_lstTransitionToStates;
            }
            set
            {
                m_lstTransitionToStates = value;
            }
        }


        public List<string> TransitionEvents
        {
            get
            {
                return m_lstTransitionEvents;
            }
            set
            {
                m_lstTransitionEvents = value;
            }
        }


        public List<string> TransitionGuards
        {
            get
            {
                return m_lstTransitionGuards;
            }
            set
            {
                m_lstTransitionGuards = value;
            }
        }


        public List<string> TransitionActions
        {
            get
            {
                return m_lstTransitionActions;
            }
            set
            {
                m_lstTransitionActions = value;
            }
        }


        // = new List<string>();
        //private List<string> m_lstOperatorPromptText



        public List<string> OperatorPromptStates
        {
            get
            {
                return m_lstOperatorPromptStates;
            }
            set
            {
                m_lstOperatorPromptStates = value;
            }
        }

        public List<string> OperatorPromptText
        {
            get
            {
                return m_lstOperatorPromptText;
            }
            set
            {
                m_lstOperatorPromptText = value;
            }
        }

        public Dictionary<string, string> DictDataEventNameAndDataType
        {
            get { return m_dictDataEventNameAndDataType; }
            set { m_dictDataEventNameAndDataType = value; }
        }

        object GuiConnectLock;

        public bool GuiConnected
        {
            get
            {
                return m_bGuiConnectedYet;
            }
            set
            {
                lock (GuiConnectLock)
                {
                    if (!m_bGuiConnectedYet && (value == true))
                    {
                        m_bGuiConnectedYet = true;

                        // Signal the State Machine to proceed if the system has completed InitAfterSettings
                        if ((m_bInitAfterSettingsAlreadyCompleted) && (!m_bInitAfterSettingsEventSent))
                        {
                            m_bInitAfterSettingsEventSent = true;
                            m_oStateMachineCodeGeneratorStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.InitAfterSettingsCompleteEvent, 0);
                        }
                    }
                    else
                    {
                        ms_iLogger.Log(ELogLevel.Error, "Error!  GuiConnected should never be called more than once and should only be set to True!");
                    }
                }
            }
        }

        public IAlarmManager AlarmManager
        {
            get { return m_oAlarmManager; }
        }

        public string DataOutputStatus
        {
            get { return m_dataOutputStatus; }
        }

        public ILanguage Language
        {
            get { return m_iLanguage; }
        }

        public ELoginLevel LoginLevel
        {
            get
            {
                return m_eLoginLevel;
            }
            set
            {
                bool bChange = (m_eLoginLevel != value);
                m_eLoginLevel = value;

                if (bChange && (eventLoginLevelChange != null))
                {
                    // Notify anyone subscribed to the state change event
                    eventLoginLevelChange.Invoke(m_eLoginLevel);
                }
            }
        }

        public IPointProxyServer PointProxyServer
        {
            get { return m_iPointProxyServer; }
            set { m_iPointProxyServer = value; }
        }

        public IPointsServerLocal PointsServerLocal
        {
            get { return m_oPointsServer; }
        }

        public IPointsServerRemote PointsServerRemote
        {
            get { return m_oPointsServer; }
        }

        public string RuleEngineSettingsArchiveDir
        {
            get { return string.Format("{0}\\Archive\\", m_strRuleEngineSettingsDir); }
        }

        public string RuleEngineSettingsDir
        {
            get { return m_strRuleEngineSettingsDir; }
        }

        public ISystemSettings SettingsManager
        {
            get { return m_iSettingsManager; }
        }

        //public string DataOutputStatus
        //{
        //    get { return m_oDataOutputManager.DataOutputStatus; }
        //}
        public IStateMachineCodeGeneratorStateMachine SystemStateMachine
        {
            get { return (IStateMachineCodeGeneratorStateMachine)m_oStateMachineCodeGeneratorStateMachine; }
        }

        public string Version { get { return VersionS; } }

        public string VersionMajor { get { return VersionMajorS; } }

        public string CorningProgramDataPath { get { return m_strCorningProgramDataPath; } }

        public bool IsSystemShutdown { get { return m_bIsSystemShutdown; } }

        public bool IsInOfflineViewerMode
        {
            get { return m_isInOfflineViewerMode; }
        }

        public bool IsSensorReady
        {
            get { return m_isSensorReady; }

            set { m_isSensorReady = value; }
        }

        public bool SimulateSample1
        {
            get { return m_simulateSample1; }
        }

        public bool SimulationModeEnabled
        {
            get
            {
                return m_bSimulationMode;
            }
            set
            {
                m_bSimulationMode = value;
            }
        }

        public bool SimulationOverrideActualAlarms
        {
            get
            {
                return m_bSimulationOverrideActualAlarms;
            }
            set
            {
                m_bSimulationOverrideActualAlarms = value;
            }
        }

        public bool StateSimulationIsAdminLogin
        {
            get
            {
                return m_bStateSimulationIsAdminLogin;
            }
            set
            {
                m_bStateSimulationIsAdminLogin = value;
            }
        }

        public bool StateSimulationIsDoorOpened
        {
            get
            {
                return m_bStateSimulationIsDoorOpened;
            }
            set
            {
                m_bStateSimulationIsDoorOpened = value;
            }
        }

        public bool StateSimulationIsHomed
        {
            get
            {
                return m_bStateSimulationIsHomed;
            }
            set
            {
                m_bStateSimulationIsHomed = value;
            }
        }

        public bool StateSimulationIsCriticalAlarm
        {
            get
            {
                return m_bStateSimulationIsCriticalAlarm;
            }
            set
            {
                m_bStateSimulationIsCriticalAlarm = value;
            }
        }

        public bool StateSimulationIsWarningOrCriticalAlarm
        {
            get
            {
                return m_bStateSimulationIsWarningOrCriticalAlarm;
            }
            set
            {
                m_bStateSimulationIsWarningOrCriticalAlarm = value;
            }
        }

        public string StateMachineCodeGeneratorDataDataDir
        {
            get { return m_StateMachineCodeGeneratorDataDataDir; }
        }

        public bool IsAdminPrivilege
        {
            get
            {
                if (m_bSimulationMode)
                {
                    return m_bStateSimulationIsAdminLogin;
                }
                else
                {
                    return (m_eLoginLevel == ELoginLevel.ADMIN);
                }
            }
        }

        public bool IsCriticalAlarms
        {
            get
            {
                if ((m_bSimulationMode) && (m_bSimulationOverrideActualAlarms))
                {
                    return m_bStateSimulationIsCriticalAlarm;
                }


                // Are any critical alarms active?
                List<IAlarmRule> olstAlarmRules = m_oAlarmManager.GetAlarmRules(true, null, false, null);
                foreach (IAlarmRule iAlarmRule in olstAlarmRules)
                {
                    if (iAlarmRule.Severity == EAlarmSeverity.Critical)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsDoorClosed
        {
            get
            {
                if (m_bSimulationMode)
                    return m_bStateSimulationIsDoorClosed;
                else
                    return m_iStateMachineCodeGeneratorIo.Sample2DIn.Value;
            }
        }

        public bool IsDoorOpen
        {
            get
            {
                if (m_bSimulationMode)
                    return m_bStateSimulationIsDoorOpened;

                return true; //  TODO:  m_iStateMachineCodeGeneratorIo.Sample1DIn.Value;
            }
        }

        public bool IsHomed
        {
            get
            {
                if (m_bSimulationMode)
                    return m_bStateSimulationIsHomed;

                return true; // TODO:  Put this back in when motion axis is setup for GenSysEasy-->  m_iStateMachineCodeGeneratorMotion.XAxis.AxisStatus.Homed && m_iStateMachineCodeGeneratorMotion.YAxis.AxisStatus.Homed && m_iStateMachineCodeGeneratorMotion.ZAxis.AxisStatus.Homed;
            }
        }

        public bool IsWarningAlarms
        {
            get
            {
                // Are any critical alarms active?
                List<IAlarmRule> olstAlarmRules = m_oAlarmManager.GetAlarmRules(true, null, false, null);
                foreach (IAlarmRule iAlarmRule in olstAlarmRules)
                {
                    if (iAlarmRule.Severity == EAlarmSeverity.Warning)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsWarningOrCriticalAlarms
        {
            get
            {
                if ((m_bSimulationMode) && (m_bSimulationOverrideActualAlarms))
                {
                    return m_bStateSimulationIsWarningOrCriticalAlarm;
                }
                else
                {
                    // Are any critical alarms active?
                    return IsCriticalAlarms || IsWarningAlarms;
                }
            }
        }





        [ASetting(Name = "CurrentLanguage", Description = "Indicates whether to retrieve the Filtered image for Feature Vectors.", ReadAccess = EAccess.Admin | EAccess.Engineer)]
        public ELanguageType CurrentLanguage { get; set; }

        public bool EnableAbortButton
        {
            set
            {
                GuiActionRequestHelper(EGuiActionRequest.ENABLE_ABORT_BUTTON, value);
            }
        }

        public bool EnableStopButton
        {
            set
            {
                GuiActionRequestHelper(EGuiActionRequest.ENABLE_STOP_BUTTON, value);
            }
        }

        public bool EnableCommitButton
        {
            set
            {
                //m_iStateMachineCodeGeneratorGui.EnableCommitButton(value);
            }
        }

        public bool EnableLoadOfflineFileButton
        {
            set
            {
                //m_iStateMachineCodeGeneratorGui.EnableLoadOfflineFileButton(value);
            }
        }

        public bool EnableSaveButtons
        {
            set
            {
                //m_iStateMachineCodeGeneratorGui.EnableCommitButton(value);
                //m_iStateMachineCodeGeneratorGui.EnableSaveZetaImgButton(value);
            }
        }

        public bool EnableStartAndAbortButton
        {
            set
            {
                //m_iStateMachineCodeGeneratorGui.EnableStartAndAbortButton(value);
            }
        }

        public bool EnableStartButton
        {
            set
            {
                GuiActionRequestHelper(EGuiActionRequest.ENABLE_START_BUTTON, value);
            }
        }

        public bool ManualMotionEnable
        {
            get { return m_bManualMotionEnable; }
            set
            {
                m_bManualMotionEnable = value;

                try
                {
                    // set manual motion status on GUI
                    //m_iStateMachineCodeGeneratorGui.ManualMotionEnable = value;
                }
                catch (Exception ex)
                {
                    ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to ManualMotionEnable!", ex);
                }
            }
        }

        public bool UnLoadGlassOnGuiEnable
        {
            set { m_unLoadGlassOnGuiEnable = value; }
        }

        //public IStateMachineCodeGeneratorGui StateMachineCodeGeneratorGui
        //{
        //    get { return m_iStateMachineCodeGeneratorGui; }
        //}
        public IStateMachineCodeGeneratorIo StateMachineCodeGeneratorIo
        {
            get { return m_iStateMachineCodeGeneratorIo; }
        }

        public IStateMachineCodeGeneratorMotion StateMachineCodeGeneratorMotion
        {
            get { return m_iStateMachineCodeGeneratorMotion; }
        }

        #region StateMachineSimulation
        #endregion StateMachineSimulation

        #region StateMachineGuardProperties
        #endregion StateMachineGuardProperties

        #region StateMachineActionProperties
        #endregion StateMachineActionProperties

        #endregion Properties

        #region Events
        public event SystemClosingEventHandler OnSystemClosing;
        public event SystemShutdownEventHandler OnSystemShutdown;

        public void RaiseOnSystemClosing()
        {
            if(OnSystemClosing!=null)
              OnSystemClosing(this);
        }


        public void RaiseOnSystemShutdown()
        {
            if (OnSystemShutdown != null)
                OnSystemShutdown(this);
        }

        public event GuiActionRequestEventHandler eventGuiActionRequest;

        #endregion Events

        #region Constructors
        public CStateMachineCodeGeneratorSystem()
        {
            GuiConnectLock = new object();
            
            // Create and Start State Machine
            m_oStateMachineCodeGeneratorStateMachine = new CStateMachineCodeGeneratorStateMachine("StateMachineCodeGeneratorStateMachine", this);
            m_oStateMachineCodeGeneratorStateMachine.StartStateMachine();

            // Initialize the System
            Init();


            foreach (IInitialization iInitialization in m_lstiInitialization)
            {
                iInitialization.Init();
            }
        }

        ~CStateMachineCodeGeneratorSystem()
        {
        }

        public virtual void Dispose()
        {
        }

        //public void SystemClosing()
        //{
        //    // Stop State Machine
        //    m_oStateMachineCodeGeneratorStateMachine.StopStateMachine();

        //    RaiseOnSystemClosing();

        //    ms_iLogger.Log(ELogLevel.Info, "StateMachineCodeGenerator System is Shutting down.");

        //    // Set I/O to the correct states
        //    try
        //    {
        //        // Turn off the Stack Light Alarm
        //        if (m_iStateMachineCodeGeneratorIo.Sample1DOut != null) m_iStateMachineCodeGeneratorIo.Sample1DOut.Value = false;
        //        if (m_iStateMachineCodeGeneratorIo.Sample2DOut != null) m_iStateMachineCodeGeneratorIo.Sample2DOut.Value = false;
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    // Serialize the system settings
        //    try
        //    {
        //        this.SerializeSettings();
        //    }
        //    catch (Exception ex)
        //    {
        //        ms_iLogger.LogException(ELogLevel.Error, "Error Serializing Settings!", ex);
        //    }

        //    // Settings Manager needs to see if the settings files need to be archived
        //    //m_iSettingsManager.SaveAllSettings();
        //}




        #endregion Constructors

        #region Methods
        public void Init()
        {
            // Initialize the points server
            m_oPointsServer = new CPointsServer("StateMachineCodeGeneratorSystem");
            // Needs to be created before settings are read
            //m_oDataOutputManager = new CDataOutputManager(m_strCorningProgramDataPath, this);

            // Create System Points
            CreateSystemPoints();

            //m_iLanguage = new CLanguage(m_oPointsServer, ms_iLogger);
            m_iLanguage = new CLanguageDummy();

            // Initialize the Settings Manager
            bool bInitializedOK;
            CFileArchiver.SetLogger(ms_iLogger);
            m_iSettingsManager = new CFileArchiver(VersionMajor, m_strCorningProgramDataPath + @"\Settings", out bInitializedOK);

            // Create the Alarm Manager
            m_oAlarmManager = new CAlarmManager("StateMachineCodeGeneratorAlarmManager");
            m_oAlarmManager.AddPointServer(m_oPointsServer);
            m_oAlarmManager.OnAlarm += m_oAlarmManager_OnAlarm;
            m_oAlarmManager.OnAlarmAcknowledged += m_oAlarmManager_OnAlarmAcknowledged;

            m_oStateMachineCodeGeneratorCalcs = new CStateMachineCodeGeneratorCalcs(0.5f, 0.6f, this);

            // Add virtual points to PointsServer

            try
            {
                // Create "virtual points" for handling all of the serialization \ deserialization for this class
                List<Corning.GenSys.Point.IPoint> lstiPoints = CSettingHelperPoint.CreateSettingPoints("System", this);
                // Add points to the PointServer
                m_oPointsServer.AddPoints(lstiPoints, "", true);
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to create System points!", ex);
            }

            try
            {
                List<Corning.GenSys.Point.IPoint> lstiPoints = CSettingHelperPoint.CreateSettingPoints("System.Calc", m_oStateMachineCodeGeneratorCalcs);
                // Add points to the PointServer
                m_oPointsServer.AddPoints(lstiPoints, "", true);
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to create System.Calc points!", ex);
            }

            // Initialize objects for Serialization & Settings Management
            m_iSettingsManager.RegisterToSerializeAsFile(new[] { m_strStateMachineCodeGeneratorSettingsFileName }, m_oPointsServer);

            // Create the Settings Directory if necessary
            m_strStateMachineCodeGeneratorSettingsPathAndFileName = m_strCorningProgramDataPath + @"\Settings\" + m_strStateMachineCodeGeneratorSettingsFileName;
            try
            {
                if (!Directory.Exists(m_strCorningProgramDataPath + @"\Settings"))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(m_strCorningProgramDataPath + @"\Settings");
                    ms_iLogger.Log(ELogLevel.Info, string.Format("Created Settings folder in {0}.", m_strCorningProgramDataPath + @"\Settings"));
                }
            }
            catch (IOException oIOException)
            {
                ms_iLogger.Log(ELogLevel.Error, string.Format("Unable to create Settings folder in {0} Reason {1}.", m_strCorningProgramDataPath + @"\Settings", oIOException.ToString()));
            }


            // Create the Generated State Machine Settings Directory if necessary
            m_strStateMachineSettingsPathAndFileName = m_strCorningProgramDataPath + @"\StateMachines\" + m_strStateMachineCodeGeneratorSettingsFileName;
            try
            {
                if (!Directory.Exists(m_strCorningProgramDataPath + @"\StateMachines"))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(m_strCorningProgramDataPath + @"\StateMachines");
                    ms_iLogger.Log(ELogLevel.Info, string.Format("Created State Machine Settings folder in {0}.", m_strCorningProgramDataPath + @"\StateMachines"));
                }
            }
            catch (IOException oIOException)
            {
                ms_iLogger.Log(ELogLevel.Error, string.Format("Unable to create Settings folder in {0} Reason {1}.", m_strCorningProgramDataPath + @"\StateMachines", oIOException.ToString()));
            }




            try
            {
                // Init Motion
                m_oCCoordSystemCoordSystem = new CCoordSystem(0, 1); // X = 0, Y = 1
                m_oPointsForMotion = new CPointsForMotion(m_oPointsServer, m_oStateMachineCodeGeneratorStateMachine);
                m_iStateMachineCodeGeneratorMotion = new CStateMachineCodeGeneratorMotion(m_oCCoordSystemCoordSystem.ConvertDelegate, m_oPointsForMotion);
                m_iStateMachineCodeGeneratorMotion.OnMotionStarted += StateMachineCodeGeneratorMotion_OnMotionStarted;
                m_iStateMachineCodeGeneratorMotion.OnMotionComplete += StateMachineCodeGeneratorMotion_OnMotionComplete;
            }
            catch (Exception oException)
            {
                ms_iLogger.Log(ELogLevel.Error, string.Format("Error initializing motion. Reason {0}.", oException));
            }

            // Init I/O
            m_oPointsForIo = new CPointsForIo(m_oPointsServer, m_oStateMachineCodeGeneratorStateMachine);
            m_iStateMachineCodeGeneratorIo = new CStateMachineCodeGeneratorIo(m_oPointsForIo, this);

            // Deserialize the system settings
            try
            {
                this.DeserializeSettings();
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error Deserializing Settings!", ex);
            }

            //CopyRuleEngineSettings();

            string strError;
            // WARNING: All the points used by the Rule engines must be created and added to the point server before
            //          the creation of the Rule engines. This is true for the rule which are not written in C#
            //          and that are converted to C# by the rule engine code.
            //m_iStateMachineCodeGeneratorRuleEngineSample = CStateMachineCodeGeneratorRuleEngine.MakeRuleEngineFromXmlSettingFile(Path.Combine(m_strRuleEngineSettingsDir, "SampleRules.xml"), "",
            //                                                                                 m_oPointsServer, out strError);
            //if (m_iStateMachineCodeGeneratorRuleEngineSample == null || strError != null)
            //{
            //    ms_iLogger.Log(ELogLevel.Fatal, string.Format("Unable to create StateMachineCodeGeneratorRuleEngineSample Rule Engine{0}", strError != null ? string.Format("\nError:\n{0}", strError) : ""));
            //}
        }

        public void InitAfterSettings()
        {
            try
            {
                // Startup Dir Cleaners
                m_oLogDirectoryCleaner = new CDirCleaner();
                //TODO:  Put this back in when data output is setup for GenSysEasy-->  m_oDataOutputDirCleaner = new CDirCleaner();

                // Init Settings associated with cleaners
                InitDirCleanerSettings();

                // Kickoff a cleaning at startup
                m_oLogDirectoryCleaner.CleanDirsOnceAsync(); // Clean the directories on startup
                //TODO:  Put this back in when data output is setup for GenSysEasy-->  m_oDataOutputDirCleaner.CleanDirsOnceAsync(); // Clean the directories on startup

                // Create the configured number of threads for each of the processing activities
                CreateTasks("SingleChanCalcTask{0}", ref m_aoSampleTask, m_nNumSingleChanCalcThreads, SampleTaskSP, System.Threading.ThreadPriority.Normal, 0);

                try
                {
                    m_oCCoordSystemCoordSystem.GlassOriginX = m_dblGlassOriginX;
                    m_oCCoordSystemCoordSystem.GlassOriginY = m_dblGlassOriginY;
                    m_iStateMachineCodeGeneratorMotion.XAxis.SetPosLimits(m_dblMotionXAxisLowerLimit, m_dblMotionXAxisUpperLimit);
                    m_iStateMachineCodeGeneratorMotion.YAxis.SetPosLimits(m_dblMotionYAxisLowerLimit, m_dblMotionYAxisUpperLimit);
                    m_iStateMachineCodeGeneratorMotion.ZAxis.SetPosLimits(m_dblMotionZAxisLowerLimit, m_dblMotionZAxisUpperLimit);

                    m_iStateMachineCodeGeneratorMotion.XAxis.SetMotionProfile(m_dblMotionXAxisVelocity, m_dblMotionXAxisAcceleration, m_dblMotionXAxisAcceleration, null, null);
                    m_iStateMachineCodeGeneratorMotion.YAxis.SetMotionProfile(m_dblMotionYAxisVelocity, m_dblMotionYAxisAcceleration, m_dblMotionYAxisAcceleration, null, null);
                    m_iStateMachineCodeGeneratorMotion.ZAxis.SetMotionProfile(m_dblMotionZAxisVelocity, m_dblMotionZAxisAcceleration, m_dblMotionZAxisAcceleration, null, null);
                }
                catch (Exception ex)
                {
                    ms_iLogger.LogException(ELogLevel.Error, "Error while setting Motion Profile and Limits", ex);
                }

                // Create the Blocking Collections
                m_blocoliStateMachineCodeGeneratorData = new BlockingCollection<IStateMachineCodeGeneratorData>();
                m_blocoliStateMachineCodeGeneratorDataProcessed = new BlockingCollection<IStateMachineCodeGeneratorData>();

                // Setup Alarm Rules
                try
                {
                    m_oAlarmManager.AddAlarmRule("DIn Sample1 True", "Sample 1 digital input is true",
                                EAlarmSeverity.Critical, "[" + m_oPointsForIo.Sample1DIn.PointName + ", HIGHHIGH]");

                    m_oAlarmManager.AddAlarmRule("Sample Alarm Fault", "Use Error Code to determine cause of Motion fault",
                        EAlarmSeverity.Critical, "[" + m_iPointSampleAlarm.PointName + ", HIGHHIGH]");

                    m_oAlarmManager.AddAlarmRule("Scan sequence aborted", "Scan sequence has been aborted by the User",
                         EAlarmSeverity.Warning, "[" + m_iPointScanAbortAlarm.PointName + ", HIGHHIGH]");
                    m_oAlarmManager.AddAlarmRule("Scan Sensor Fault", "Use Error Code to determine cause of Scan Sensor fault",
                         EAlarmSeverity.Critical, "[" + m_iPointScanSensorFaultAlarm.PointName + ", HIGHHIGH]");
                    m_oAlarmManager.AddAlarmRule("Motion Fault", "Use Error Code to determine cause of Motion fault",
                        EAlarmSeverity.Critical, "[" + m_iPointMotionFaultAlarm.PointName + ", HIGHHIGH]");
                }
                catch (Exception ex)
                {
                    ms_iLogger.LogException(ELogLevel.Error, "Error!  Error setting up an AlarmRule in InitAfterSettings!", ex);
                }

                bool m_bSuccess = false;
                while (!m_bSuccess)
                {
                    try
                    {
                        //m_iStateMachineCodeGeneratorGui.InitAfterSettings();
                        m_bSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        ms_iLogger.Log(ELogLevel.Info, "Waiting for GUI to Complete Initialization before Continuing!");
                    }
                }

                if (m_oAlarmManager != null)
                    m_oAlarmManager.ForceAllRuleEvaluation();
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!  Error in System Init!", ex);
            }

            foreach (IInitialization iInitialization in m_lstiInitialization)
            {
                iInitialization.InitAfterSettings();
            }


            // Proceed to the next state  (keep this the last step in InitAfterSettings!)
            lock (GuiConnectLock)
            {
                m_bInitAfterSettingsAlreadyCompleted = true;

                if ((GuiConnected) && (!m_bInitAfterSettingsEventSent))
                {
                    m_bInitAfterSettingsEventSent = true;
                    m_oStateMachineCodeGeneratorStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.InitAfterSettingsCompleteEvent, 0);
                }
            }
        }

        public void InitAfterSettingsComplete()
        {
            ;
        }

        // This was included as part of a Quick Start Guid and may be removed
        // This starts a thread which calls "ApplicationTask" every 1000 mSec and is called by the State Machine.
        public Task StartRunningTheApplication()
        {

            m_nTaskIterationCount = 1;

            var taskGen = new Task(async () => await ApplicationTask());
            try
            {
                taskGen.Start();
                taskGen.Wait(); // this is a non-blocking (UI fully operational) command even though we won't proceed until 'ApplicationTask' finishes running.
                m_oStateMachineCodeGeneratorStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.ScanCompleteEvent, null);
            }
            catch (AggregateException ea)
            {
                m_oStateMachineCodeGeneratorStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.AbortCompleteEvent, ea.GetBaseException().Message);
            }

            return taskGen;
        }

        // This was included as part of a Quick Start Guide and may be removed
        // Stop the thread if it is running. This is called by the State Machine.
        public void StopRunningTheApplication()
        {
            if (m_oApplicationTaskThread == null) return;
            // Optional Code to demonstrate the ThreadHandler.
            if ((m_oApplicationTaskThread.ThreadState != ThreadStateEnum.Stopped) && (m_oApplicationTaskThread.ThreadState != ThreadStateEnum.Stopping))
            {
                m_oApplicationTaskThread.StopThreadAndWaitUntilStopped(); // Stop the thread and wait until it stops.
            }
        }


        #region CodeGenerator
        private TemplatesGenerator _codeGenerator;

        public TemplatesGenerator CodeGenerator
        {
            get { return _codeGenerator; }
            set { _codeGenerator = value; }
        }
        #endregion CodeGenerator


        // This task is called by the Thread Handler every 1000 mSec
        private async Task<bool> ApplicationTask()
        {
            //var xmlFile = new FileInfo(@"C:\GenSys\GenSys projects\Shield T4\Models\Shield State Machine.xml");
            var xmlFile = new FileInfo(EAXMLFilePath);

            CodeGenerator = new TemplatesGenerator(xmlFile);
            var filesGenerated = await CodeGenerator.GenerateFiles();

            return filesGenerated;

            bool bSubStatesFound = true;
            bool bEnterFields = false;
            bool bEnterProperties = false;
            bool bEnterMethods = false;
            bool bFieldsEntered = false;
            bool bPropertiesEntered = false;
            bool bMethodsEntered = false;
            bool bEndOfFileEntered = false;
            int nIndex;
            int nTransitionIndex;
            int nCharIndex;
            int nStateLevel;
            int nDuplicateTransitionSearcherIndex;
            int nNumberOfTransitionsWithSameFromAndToStates;


            int nStateMachineSourceCodeFileEndingLineNumber;

            int nStateMachineSourceCodeFileStateMachineFieldRegionStartingLineNumber;
            int nStateMachineSourceCodeFileStateMachineFieldRegionEndingLineNumber;
            int nStateMachineSourceCodeFileStateMachineMethodRegionStartingLineNumber;
            int nStateMachineSourceCodeFileStateMachineMethodRegionEndingLineNumber;
            int nStateMachineSourceCodeFileStateMachinePropertyRegionStartingLineNumber;
            int nStateMachineSourceCodeFileStateMachinePropertyRegionEndingLineNumber;


            int nOwnerSourceCodeFileEndingLineNumber;

            int nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber;
            int nOwnerSourceCodeFileStateMachineFieldRegionEndingLineNumber;
            int nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber;
            int nOwnerSourceCodeFileStateMachineMethodRegionEndingLineNumber;
            int nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber;
            int nOwnerSourceCodeFileStateMachinePropertyRegionEndingLineNumber;

            // Interface
            int nOwnerInterfaceFileEndingLineNumber;
            int nOwnerInterfaceFileStateMachineMethodRegionStartingLineNumber;
            int nOwnerInterfaceFileStateMachineMethodRegionEndingLineNumber;
            int nOwnerInterfaceFileStateMachinePropertyRegionStartingLineNumber;
            int nOwnerInterfaceFileStateMachinePropertyRegionEndingLineNumber;

            string strEnumStateValue;
            string strTransitionName;
            string strInstantiatedTransitionName;
            string strStateMachineTransitionAction;
            string strTransitionTrigger;
            string strTransitionGuard;
            string strTransitionAction;
            string strTransitionFromState;
            string strTransitionToState;
            string strMethodOrPropertyName;
            string strName;
            string strDataEventDataType;
            string[] astrTransitionActionParameters;
            string strTransitionActionWithNoParameters;
            string strFormattedTransitionActionParameters;
            string strStartOfRegionText;
            string strEndOfRegionText;
            List<Tuple<string, List<string>>> lstTupleOfStateNamesAndSubStates;
            List<Tuple<string, List<string>>> lstNextLevelTupleOfStateNamesAndSubStates;
            List<string> lstSubStates = new List<string>();
            Dictionary<int, List<Tuple<string, List<string>>>> dictStateLevelAndListOfStatesAndSubStatesOnThatLevel = new Dictionary<int, List<Tuple<string, List<string>>>>();
            List<string> lstInstantiatedGuards = new List<string>();
            List<string> lstInstantiatedActions = new List<string>();
            StringBuilder sbMethodOrPropertyWithDocumentation;
            StringBuilder sbMethodOrPropertyWithoutDocumentation;
            StringBuilder sbTempSourceCode;
            StringBuilder sbRegionsWhichNeedToBeAddedByOperator = new StringBuilder();
            EFileUpdaterStatus eFileUpdaterStatus;
            int nStateMachineSourceCodeCommentStartingIndex;
            int nStateMachineSourceCodeCommentEndingIndex;
            int nStateMachineSourceCodeFieldRegionStartingLineNumber;
            int nStateMachineSourceCodeFieldRegionEndingLineNumber;
            int nStateMachineSourceCodeMethodRegionStartingLineNumber;
            int nStateMachineSourceCodeMethodRegionEndingLineNumber;



            int nStateMachineInterfaceCommentStartingIndex;
            int nStateMachineInterfaceCommentEndingIndex;

            try
            {

                m_iOwnerSourceCodeFileUpdater = new CFileUpdater(StateMachineOwnerSourceFilePath);
                m_iOwnerSourceCodeFileUpdater.LoadFileIntoMemory();


                if (m_iOwnerSourceCodeFileUpdater.LocateMethodsAndPropertiesInLoadedFile(string.Format("StateMachine{0}Fields", m_strStateMachineName)) == EFileUpdaterStatus.RegionOrPropertyDoesntExist)
                {
                    strStartOfRegionText = string.Format(@"{0} #region StateMachine{1}Fields{0}", Environment.NewLine, m_strStateMachineName);
                    strEndOfRegionText = string.Format(@"{0} #endregion StateMachine{1}Fields{0}", Environment.NewLine, m_strStateMachineName);
                    var regionBlock = new String[] { strStartOfRegionText, string.Empty, strEndOfRegionText, string.Empty, string.Empty };

                    var ix = m_iOwnerSourceCodeFileUpdater.GetConstructorLocation(m_strStateMachineName) ; 
                    m_iOwnerSourceCodeFileUpdater.InsertLines(ix - 1, regionBlock); // -1 to step out of the Constructor region
                }


                if (m_iOwnerSourceCodeFileUpdater.LocateMethodsAndPropertiesInLoadedFile(string.Format("StateMachine{0}Methods", m_strStateMachineName)) == EFileUpdaterStatus.RegionOrPropertyDoesntExist)
                {
                    strStartOfRegionText = string.Format(@"{0} #region StateMachine{1}Methods{0}", Environment.NewLine, m_strStateMachineName);
                    strEndOfRegionText = string.Format(@"{0} #endregion StateMachine{1}Methods{0}", Environment.NewLine, m_strStateMachineName);
                    var regionBlock = new String[] { strStartOfRegionText, string.Empty, strEndOfRegionText, string.Empty, string.Empty };

                    var ix = m_iOwnerSourceCodeFileUpdater.GetConstructorLocation(m_strStateMachineName);
                    m_iOwnerSourceCodeFileUpdater.InsertLines(ix - 1, regionBlock); // -1 to step out of the Constructor region

                }

                if (m_iOwnerSourceCodeFileUpdater.LocateMethodsAndPropertiesInLoadedFile(string.Format("StateMachine{0}Properties", m_strStateMachineName)) == EFileUpdaterStatus.RegionOrPropertyDoesntExist)
                {
                    strStartOfRegionText = string.Format(@"{0} #region StateMachine{1}Properties{0}", Environment.NewLine, m_strStateMachineName);
                    strEndOfRegionText = string.Format(@"{0} #endregion StateMachine{1}Properties{0}", Environment.NewLine, m_strStateMachineName);
                    var regionBlock = new String[] { strStartOfRegionText, string.Empty, strEndOfRegionText, string.Empty, string.Empty };

                    var ix = m_iOwnerSourceCodeFileUpdater.GetConstructorLocation(m_strStateMachineName);
                    m_iOwnerSourceCodeFileUpdater.InsertLines(ix - 1, regionBlock); // -1 to step out of the Constructor region

                }

                m_iOwnerSourceCodeFileUpdater.FindRegionExtentsInLoadedFile(string.Format("StateMachine{0}Fields", m_strStateMachineName), out nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber, out nOwnerSourceCodeFileStateMachineFieldRegionEndingLineNumber);
                m_iOwnerSourceCodeFileUpdater.FindRegionExtentsInLoadedFile(string.Format("StateMachine{0}Properties", m_strStateMachineName), out nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber, out nOwnerSourceCodeFileStateMachinePropertyRegionEndingLineNumber);
                m_iOwnerSourceCodeFileUpdater.FindRegionExtentsInLoadedFile(string.Format("StateMachine{0}Methods", m_strStateMachineName), out nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber, out nOwnerSourceCodeFileStateMachineMethodRegionEndingLineNumber);


                m_iOwnerInterfaceFileUpdater = new CFileUpdater(StateMachineOwnerInterfaceFilePath);
                m_iOwnerInterfaceFileUpdater.LoadFileIntoMemory();



                if (m_iOwnerInterfaceFileUpdater.LocateMethodsAndPropertiesInLoadedFile(string.Format("StateMachine{0}Methods", m_strStateMachineName)) == EFileUpdaterStatus.RegionOrPropertyDoesntExist)
                {
                    strStartOfRegionText = string.Format(@"{0} #region StateMachine{1}Methods{0}", Environment.NewLine, m_strStateMachineName);
                    strEndOfRegionText = string.Format(@"{0} #endregion StateMachine{1}Methods{0}", Environment.NewLine, m_strStateMachineName);
                    var regionBlock = new String[] { strStartOfRegionText, string.Empty, strEndOfRegionText, string.Empty, string.Empty };

                    var ix = m_iOwnerInterfaceFileUpdater.GetInterfaceLocation(m_strStateMachineName);
                    m_iOwnerInterfaceFileUpdater.InsertLines(ix + 2, regionBlock); // +2 to step into the interface
                }


                if (m_iOwnerInterfaceFileUpdater.LocateMethodsAndPropertiesInLoadedFile(string.Format("StateMachine{0}Properties", m_strStateMachineName)) == EFileUpdaterStatus.RegionOrPropertyDoesntExist)
                {
                    strStartOfRegionText = string.Format(@"{0} #region StateMachine{1}Properties{0}", Environment.NewLine, m_strStateMachineName);
                    strEndOfRegionText = string.Format(@"{0} #endregion StateMachine{1}Properties{0}", Environment.NewLine, m_strStateMachineName);
                    var regionBlock = new String[] { strStartOfRegionText, string.Empty, strEndOfRegionText, string.Empty, string.Empty };

                    var ix = m_iOwnerInterfaceFileUpdater.GetInterfaceLocation(m_strStateMachineName);
                    m_iOwnerInterfaceFileUpdater.InsertLines(ix + 2, regionBlock); // +2 to step into the interface
                }

                m_iOwnerInterfaceFileUpdater.FindRegionExtentsInLoadedFile(string.Format("StateMachine{0}Properties", m_strStateMachineName), out nOwnerInterfaceFileStateMachinePropertyRegionStartingLineNumber, out nOwnerInterfaceFileStateMachinePropertyRegionEndingLineNumber);
                m_iOwnerInterfaceFileUpdater.FindRegionExtentsInLoadedFile(string.Format("StateMachine{0}Methods", m_strStateMachineName), out nOwnerInterfaceFileStateMachineMethodRegionStartingLineNumber, out nOwnerInterfaceFileStateMachineMethodRegionEndingLineNumber);





                m_iStateMachineSourceCodeFileUpdater = new CFileUpdater(StateMachineSourceFilePath);
                m_iStateMachineSourceCodeFileUpdater.LoadFileIntoMemory();
                m_iStateMachineSourceCodeFileUpdater.FindHeaderCommentExtentsInLoadedFile(out nStateMachineSourceCodeCommentStartingIndex, out nStateMachineSourceCodeCommentEndingIndex);
                m_iStateMachineSourceCodeFileUpdater.LocateMethodsAndPropertiesInLoadedFile(string.Format("Auto-Generated State Machine Methods"));
                m_iStateMachineSourceCodeFileUpdater.LocateMethodsAndPropertiesInLoadedFile(string.Format("State Machine Properties"));


                m_iStateMachineInterfaceFileUpdater = new CFileUpdater(StateMachineInterfaceFilePath);
                m_iStateMachineInterfaceFileUpdater.LoadFileIntoMemory();
                m_iStateMachineInterfaceFileUpdater.FindHeaderCommentExtentsInLoadedFile(out nStateMachineInterfaceCommentStartingIndex, out nStateMachineInterfaceCommentEndingIndex);


                StringBuilder oSbStateMachineSourceCode = new StringBuilder();
                StringBuilder oSbStateMachineInterface = new StringBuilder();
                StringBuilder sbOwnerSourceCode = new StringBuilder();
                StringBuilder sbOwnerInterface = new StringBuilder();



                StringBuilder oSbStateInstantiations = new StringBuilder();
                StringBuilder oSbEventInstantiations = new StringBuilder();
                StringBuilder oSbEntryAndExitActions = new StringBuilder();

                StringBuilder oSbStateMachineOwnerMethods = new StringBuilder();
                StringBuilder oSbStateMachineOwnerInterfaceProperties = new StringBuilder();
                StringBuilder oSbStateMachineOwnerInterfaceMethods = new StringBuilder();
                StringBuilder oSbStateMachineOwnerInterfaceTransitionGuardProperties = new StringBuilder();
                StringBuilder oSbStateMachineOwnerTransitionGuardProperties = new StringBuilder();

                StringBuilder oSbStateMachineOwnerTransitionActions = new StringBuilder();
                StringBuilder oSbStateMachineOperatorPrompts = new StringBuilder();
                StringBuilder oSbStateMachineOwnerFields = new StringBuilder();
                StringBuilder oSbStateMachineOwnerProperties = new StringBuilder();

                //StringBuilder oSbStateMachineInterface = new StringBuilder();
                StringBuilder oSbStateMachineTransitionGuards = new StringBuilder();
                StringBuilder oSbStateMachineTransitionActions = new StringBuilder();
                StringBuilder oSbStateMachineDictEventByEnum = new StringBuilder();


                ms_iLogger.Log(ELogLevel.Info, string.Format("Generating State Machine."));



                if (nStateMachineSourceCodeCommentStartingIndex != -1)
                {
                    m_iStateMachineSourceCodeFileUpdater.GetLinesOfTextFromFile(nStateMachineSourceCodeCommentStartingIndex, nStateMachineSourceCodeCommentEndingIndex, out sbTempSourceCode);
                    oSbStateMachineSourceCode.Append(sbTempSourceCode);
                }
                else // Generate Header of State Machine Source Code
                {
                    oSbStateMachineSourceCode.AppendLine(@"///////////////////////////////////////////////////////////");
                    oSbStateMachineSourceCode.AppendFormat(@"// Copyright © Corning Incorporated {0}", DateTime.Now.Year);
                    oSbStateMachineSourceCode.AppendFormat(@"{0}// C{1}.cs", Environment.NewLine, m_strStateMachineName);
                    oSbStateMachineSourceCode.AppendFormat(@"{0}// Project {1}", Environment.NewLine, m_strStateMachineName);
                    oSbStateMachineSourceCode.AppendFormat(@"{0}// Implementation of the Class C{1}", Environment.NewLine, m_strStateMachineName);
                    oSbStateMachineSourceCode.AppendFormat(@"{0}// Created on {1}", Environment.NewLine, DateTime.Now);
                    oSbStateMachineSourceCode.AppendFormat(@"{0}///////////////////////////////////////////////////////////", Environment.NewLine);
                    oSbStateMachineSourceCode.AppendFormat(@"{0}", Environment.NewLine);
                }

                if (m_iStateMachineSourceCodeFileUpdater.Status == EFileUpdaterStatus.FileDoesntExist)
                {
                    oSbStateMachineSourceCode.AppendFormat(@"{0}", Environment.NewLine);
                    oSbStateMachineSourceCode.AppendLine(@"using System;");
                    oSbStateMachineSourceCode.AppendLine(@"using System.Collections.Generic;");
                    oSbStateMachineSourceCode.AppendLine(@"using System.Threading;");
                    oSbStateMachineSourceCode.AppendLine(@"using Corning.GenSys.Logger;");
                    oSbStateMachineSourceCode.AppendLine(@"using NorthStateSoftware.NorthStateFramework;");
                    //oSbStateMachineSourceCode.AppendLine(string.Format(@"using {0}.Interfaces;", StateMachineNamespaceName));
                    oSbStateMachineSourceCode.AppendLine(string.Format(@"using {0};", m_strStateMachineInterfaceNamespaceName));
                    oSbStateMachineSourceCode.AppendLine(@"");
                    oSbStateMachineSourceCode.AppendLine(@"");
                    oSbStateMachineSourceCode.AppendLine(@"// Quick Start Guide for those using the GenSys Easy for the first time:");
                    oSbStateMachineSourceCode.AppendLine(@"// First time users will primarily be interested in three states: Ready, Idle, and Running. The Idle and Running states are sub-states of the Ready State.");
                    oSbStateMachineSourceCode.AppendLine(@"// The Ready state is entered after initialization and then the State Machine proceeds to the Idle State.");
                    oSbStateMachineSourceCode.AppendLine(@"// The Idle state is intended to be a ""waiting"" state with nothing significant happening.");
                    oSbStateMachineSourceCode.AppendLine(@"// The Running state is intended to be the state where your task is accomplished.");
                    oSbStateMachineSourceCode.AppendLine(@"");
                    oSbStateMachineSourceCode.AppendLine(@"");
                    oSbStateMachineSourceCode.AppendLine(@"// Here are the rules for the Ready, Idle and Running States");
                    oSbStateMachineSourceCode.AppendLine(@"// The method ""ReadyStateEntryActions""   is called after initializtion has completed.");
                    oSbStateMachineSourceCode.AppendLine(@"// The method ""ReadyStateExitActions""    is called after if a Fault occurs.");
                    oSbStateMachineSourceCode.AppendLine(@"// The method ""IdleStateEntryActions""    is called after either ""ReadyStateEntryActions"" or ""RunningStateExitActions"" is called.");
                    oSbStateMachineSourceCode.AppendLine(@"// The method ""IdleStateExitActions""     is called when the Start button is pressed.");
                    oSbStateMachineSourceCode.AppendLine(@"// The method ""RunningStateEntryActions"" is called when the Start button is pressed but after ""IdleStateExitActions"" is called.");
                    oSbStateMachineSourceCode.AppendLine(@"// The method ""RunningStateExitActions""  is called when the Abort button is pressed.");
                    oSbStateMachineSourceCode.AppendLine(@"");
                    oSbStateMachineSourceCode.AppendLine(@"");
                    oSbStateMachineSourceCode.AppendLine(@"// To start developing an Application utilizing a Start and Abort button:");
                    oSbStateMachineSourceCode.AppendLine(@"// Place Initialization Code in ""ReadyStateEntryActions"". A call into the sytem may be needed.");
                    oSbStateMachineSourceCode.AppendLine(@"// Place code to call into system to perform your task in ""RunningStateEntryActions"".");
                    oSbStateMachineSourceCode.AppendLine(@"// If a System thread was kicked off to start a task, then place a call in ""RunningStateExitActions"" to call into the system to stop the thread.");
                    oSbStateMachineSourceCode.AppendLine(@"");
                    oSbStateMachineSourceCode.AppendLine(@"// To start developing an application which doesn't require user interaction and just performs a task, place code to call into the System in ""IdleStateEntryActions"" and");
                    oSbStateMachineSourceCode.AppendLine(string.Format(@"// uncomment ""m_i{0}.EnableStartButton = false; ""  in ""IdleStateEntryActions"".", m_strStateMachineName));
                    oSbStateMachineSourceCode.AppendLine(@"// This default template has code to show how to use the Start and Abort button in conjunction with calling into the System, which uses the ThreadHandler.");
                    oSbStateMachineSourceCode.AppendLine(@"// When Start is pressed a message will be display every second until one of two conditions are met:");
                    oSbStateMachineSourceCode.AppendLine(@"// 20 Messages have been displayed or");
                    oSbStateMachineSourceCode.AppendLine(@"// The Abort button is pressed.");
                    oSbStateMachineSourceCode.AppendLine(@"");
                    oSbStateMachineSourceCode.AppendLine(@"// It is expected that this and the System code will be heavily modified.");
                    oSbStateMachineSourceCode.AppendLine(@"// Note that the State Machine uses System to do all of the real work.");
                    oSbStateMachineSourceCode.AppendLine(@"// The desired pattern is the State Machine not implementing any Application code.");
                    oSbStateMachineSourceCode.AppendLine(@"");
                    oSbStateMachineSourceCode.AppendLine(@"// The method CreateOperatorPrompts() creates GUI prompts based on the state of the State Machine.");
                    oSbStateMachineSourceCode.AppendLine(@"// As the State Machine state changes, so does the GUI prompt.");
                    oSbStateMachineSourceCode.AppendLine(@"// The following two statement are included in the code as part of the Quick Start Guide and may be tailored for your application.");
                    oSbStateMachineSourceCode.AppendLine(@"// m_odictOperatorPromptForState.Add(oIdleState, ""Hit Start to Start Running"");");
                    oSbStateMachineSourceCode.AppendLine(@"// m_odictOperatorPromptForState.Add(oRunningState, ""Hit Abort to Stop Running"");");
                    //oSbStateMachineSourceCode.AppendFormat(@"{0}namespace {1}.{2}", Environment.NewLine, m_strStateMachineNamespaceName, m_strStateMachineName);
                    oSbStateMachineSourceCode.AppendFormat(@"{0}namespace {1}", Environment.NewLine, m_strStateMachineNamespaceName);
                    oSbStateMachineSourceCode.AppendFormat(@"{0}{{", Environment.NewLine);
                    oSbStateMachineSourceCode.AppendFormat(@"{0}    public class CStateMachineEventData : INSFNamedObject", Environment.NewLine);
                    oSbStateMachineSourceCode.AppendFormat(@"{0}    {{{0}", Environment.NewLine);
                    oSbStateMachineSourceCode.AppendLine(@"        private object m_oEventData;");
                    oSbStateMachineSourceCode.AppendLine(@"        private string m_strName;");
                    oSbStateMachineSourceCode.AppendLine(@"        ");
                    oSbStateMachineSourceCode.AppendLine(@"        public object EventData { get { return m_oEventData; } set { m_oEventData = value; } }");
                    oSbStateMachineSourceCode.AppendLine(@"        public string Name { get { return m_strName; } set { m_strName = value; } }");
                    oSbStateMachineSourceCode.AppendLine(@"        ");
                    oSbStateMachineSourceCode.AppendLine(@"        public CStateMachineEventData(object oObject)");
                    oSbStateMachineSourceCode.AppendLine(@"        {");
                    oSbStateMachineSourceCode.AppendLine(@"            m_oEventData = oObject;");
                    oSbStateMachineSourceCode.AppendLine(@"        }");
                    oSbStateMachineSourceCode.AppendLine(@"    }");
                    oSbStateMachineSourceCode.AppendLine(@" ");
                    oSbStateMachineSourceCode.AppendLine(@" ");



                    oSbStateMachineSourceCode.AppendLine(string.Format(@"    public class C{0}StateMachine : NSFStateMachine, I{0}StateMachine", m_strStateMachineName));
                    oSbStateMachineSourceCode.AppendLine(@"    {");
                    oSbStateMachineSourceCode.AppendLine(@"        #region Fields");
                    oSbStateMachineSourceCode.AppendLine(string.Format(@"        private static ILogger ms_iLogger = CLoggerFactory.CreateLog(""C{0}StateMachine"");", m_strStateMachineName));
                    oSbStateMachineSourceCode.AppendLine(@"        private AutoResetEvent m_autoreseteventStateChange = new AutoResetEvent(false);");
                    oSbStateMachineSourceCode.AppendLine(@"        private object m_objLock = new object();");
                    oSbStateMachineSourceCode.AppendLine(@"        private string m_strLastOperatorPrompt = """";");
                    //oSbStateMachineSourceCode.AppendLine(string.Format(@"        private E{0}State? m_estatePrevious;", m_strStateMachineName));
                    oSbStateMachineSourceCode.AppendLine(string.Format(@"        private EState? m_estatePrevious;", m_strStateMachineName));
                    //oSbStateMachineSourceCode.AppendLine(string.Format(@"        private E{0}State? m_estateCurrent;", m_strStateMachineName));
                    oSbStateMachineSourceCode.AppendLine(string.Format(@"        private EState? m_estateCurrent;", m_strStateMachineName));
                    oSbStateMachineSourceCode.AppendLine(@"        private string m_strSystemState;");
                    oSbStateMachineSourceCode.AppendLine(string.Format(@"        private I{0}System m_i{0};", m_strStateMachineName));
                    oSbStateMachineSourceCode.AppendLine(@"        private bool m_bSimulationMode = false;");
                    oSbStateMachineSourceCode.AppendLine(string.Format(@"        private Dictionary<{0}SystemEventsEnum, NSFEvent> m_dictEventByEnum = new Dictionary<{0}SystemEventsEnum, NSFEvent>();", m_strStateMachineName));
                    oSbStateMachineSourceCode.AppendLine(@"        private Dictionary<NSFState, string> m_odictOperatorPromptForState;");
                    oSbStateMachineSourceCode.AppendLine(@"        private bool m_bInitComplete = false;");
                    oSbStateMachineSourceCode.AppendLine(@"        private bool m_bInitAfterSettingsComplete = false;");
                    oSbStateMachineSourceCode.AppendLine(@"        ");
                    oSbStateMachineSourceCode.AppendLine(@"");
                    oSbStateMachineSourceCode.AppendLine(@"        #region Auto-Generated State Machine Fields");
                    oSbStateMachineSourceCode.AppendLine("  // DO NOT ADD ANY FIELDS INSIDE THIS AUTO-GENERATED REGION (Below this line). TREAT IT AS A KEEP_OUT AREA AS USER ADDED CODE WILL BE ELIMINATED. Thanks!");
                    oSbStateMachineSourceCode.AppendLine(@"");
                }
                else // State Machine Source Code File Exists so copy from the End of the Header to the start of the Auto-Generated State Machine Fields region
                {
                    if (m_iStateMachineSourceCodeFileUpdater.FindRegionExtentsInLoadedFile(string.Format("Auto-Generated State Machine Fields", m_strStateMachineName), out nStateMachineSourceCodeFieldRegionStartingLineNumber, out nStateMachineSourceCodeFieldRegionEndingLineNumber) == EFileUpdaterStatus.OK)
                    {
                        m_iStateMachineSourceCodeFileUpdater.GetLinesOfTextFromFile(nStateMachineInterfaceCommentEndingIndex + 1, nStateMachineSourceCodeFieldRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                        oSbStateMachineSourceCode.Append(sbTempSourceCode); // Add in the Owner's Existing code
                        oSbStateMachineSourceCode.AppendLine("  // DO NOT ADD ANY FIELDS INSIDE THIS AUTO-GENERATED REGION (Below this line). TREAT IT AS A KEEP_OUT AREA AS USER ADDED CODE WILL BE ELIMINATED. Thanks!");
                    }
                }


                if (nStateMachineInterfaceCommentStartingIndex != -1)
                {
                    m_iStateMachineInterfaceFileUpdater.GetLinesOfTextFromFile(nStateMachineInterfaceCommentStartingIndex, nStateMachineInterfaceCommentEndingIndex, out sbTempSourceCode);
                    oSbStateMachineInterface.Append(sbTempSourceCode);
                }
                else
                {
                    oSbStateMachineInterface.AppendLine(@"///////////////////////////////////////////////////////////");
                    oSbStateMachineInterface.AppendFormat(@"// Copyright © Corning Incorporated {0}", DateTime.Now.Year);
                    oSbStateMachineInterface.AppendFormat(@"{0}// I{1}.cs", Environment.NewLine, m_strStateMachineName);
                    oSbStateMachineInterface.AppendFormat(@"{0}// Project {1}", Environment.NewLine, m_strStateMachineName);
                    oSbStateMachineInterface.AppendFormat(@"{0}// Implementation of the Interface I{1}", Environment.NewLine, m_strStateMachineName);
                    oSbStateMachineInterface.AppendFormat(@"{0}// Created on {1}", Environment.NewLine, DateTime.Now);
                    oSbStateMachineInterface.AppendFormat(@"{0}///////////////////////////////////////////////////////////", Environment.NewLine);
                    oSbStateMachineInterface.AppendFormat(@"{0}", Environment.NewLine);
                }


                oSbStateMachineInterface.AppendFormat(@"{0}namespace {1}", Environment.NewLine, m_strStateMachineInterfaceNamespaceName);
                oSbStateMachineInterface.AppendFormat(@"{0}{{", Environment.NewLine);

                oSbStateMachineInterface.AppendFormat(@"{0}{0}    #region Enums{0}", Environment.NewLine);

                //oSbStateMachineInterface.AppendFormat(@"{0}    public enum E{1}State", Environment.NewLine, m_strStateMachineName);
                oSbStateMachineInterface.AppendFormat(@"{0}    public enum EState", Environment.NewLine);
                oSbStateMachineInterface.AppendFormat(@"{0}    {{", Environment.NewLine);

                for (nIndex = 0; nIndex < States.Count; nIndex++)
                {
                    strName = States[nIndex];

                    // Strip off the trailing "State"
                    if (strName.EndsWith("State"))
                    {
                        strName = strName.Substring(0, strName.Length - 5);
                    }

                    oSbStateMachineInterface.AppendFormat(@"{0}        {1},", Environment.NewLine, strName);
                }


                oSbStateMachineInterface.AppendFormat(@"{0}    }}", Environment.NewLine); // End the Enum for
                oSbStateMachineInterface.AppendFormat(@"{0}", Environment.NewLine); //
                oSbStateMachineInterface.AppendFormat(@"{0}", Environment.NewLine); //


                ms_iLogger.Log(ELogLevel.Info, string.Format("Adding Events."));

                // Generate an Enum for Events
                //oSbStateMachineInterface.AppendFormat(@"{0}    public enum {1}EventsEnum", Environment.NewLine, m_strStateMachineName);
                oSbStateMachineInterface.AppendFormat(@"{0}    public enum {1}SystemEventsEnum", Environment.NewLine, m_strStateMachineName);
                oSbStateMachineInterface.AppendFormat(@"{0}    {{", Environment.NewLine);
                //oSbStateMachineInterface.AppendFormat(@"{0}        StartScanEvent,", Environment.NewLine);
                oSbStateMachineInterface.AppendFormat(@"{0}        LoadOfflineFileEvent,", Environment.NewLine); //TODO: Must be removed from Template
                //oSbStateMachineInterface.AppendFormat(@"{0}        StopEvent,", Environment.NewLine);
                oSbStateMachineInterface.AppendFormat(@"{0}        ScanInProgress,", Environment.NewLine);
                oSbStateMachineInterface.AppendFormat(@"{0}        AbortingScan,", Environment.NewLine); 
                oSbStateMachineInterface.AppendFormat(@"{0}        Sample1DInTrueEvent,", Environment.NewLine); 
                oSbStateMachineInterface.AppendFormat(@"{0}", Environment.NewLine);



                //foreach (string strEventName in m_lstEventNames)
                for (nIndex = 0; nIndex < m_lstEventNames.Count; nIndex++)
                {
                    string strEventName = m_lstEventNames[nIndex];
                    string strEventNameWithoutDataType = m_lstEventNamesWithoutDataType[nIndex];

                    if (m_dictDataEventNameAndDataType.TryGetValue(strEventNameWithoutDataType, out strDataEventDataType) == true) // Then it is a DataEvent and not a regular event
                    {
                        oSbStateMachineSourceCode.AppendFormat("{0}        private static NSFDataEvent o{1};", Environment.NewLine, strEventNameWithoutDataType);
                    }
                    else
                    {
                        oSbStateMachineSourceCode.AppendFormat("{0}        private static NSFEvent o{1};", Environment.NewLine, strEventNameWithoutDataType);
                    }

                    oSbStateMachineDictEventByEnum.AppendFormat(@"{0}                m_dictEventByEnum.Add({1}SystemEventsEnum.{2}, o{2});", Environment.NewLine, m_strStateMachineName, strEventNameWithoutDataType);
                    oSbStateMachineInterface.AppendFormat(@"{0}        {1},", Environment.NewLine, strEventNameWithoutDataType);  // Add the Events to the Enum in the State Machine Interface File

                    //strName = strEventName;

                    if (m_dictDataEventNameAndDataType.TryGetValue(strEventNameWithoutDataType, out strDataEventDataType) == true) // Then it is a DataEvent and not a regular event
                    {
                        oSbEventInstantiations.AppendFormat(@"{0}                o{1} = new NSFDataEvent<{2}>(""{3}"", this, this);", Environment.NewLine, strEventNameWithoutDataType, strDataEventDataType, strEventNameWithoutDataType);
                    }
                    else
                    {
                        oSbEventInstantiations.AppendFormat(@"{0}                o{1} = new NSFEvent(""{2}"", this, this);", Environment.NewLine, strEventNameWithoutDataType, strEventNameWithoutDataType);
                    }
                }

                oSbStateMachineInterface.AppendFormat(@"{0}    }}", Environment.NewLine); // End the Enum for Events
                oSbStateMachineInterface.AppendFormat(@"{0}{0}    #endregion Enums{0}{0}", Environment.NewLine);

                //oSbStateMachineInterface.AppendFormat(@"{0}{0}    public delegate void StateChangeEventHandler(E{1}State? estateOld, E{1}State? estateNew, string strOperatorPrompt);", Environment.NewLine, m_strStateMachineName);
                oSbStateMachineInterface.AppendFormat(@"{0}{0}    public delegate void StateChangeEventHandler(EState? estateOld, EState? estateNew, string strOperatorPrompt);", Environment.NewLine);

                oSbStateMachineInterface.AppendFormat(@"{0}{0}    public interface I{1}StateMachine", Environment.NewLine, m_strStateMachineName);
                oSbStateMachineInterface.AppendFormat(@"{0}    {{", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        #region Properties", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        string SystemState {{ get; }}", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        bool ScanInProgress {{ get; }}", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        bool AbortingScan {{ get; }}", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        bool InitAfterSettingsComplete {{ get; }}", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        bool InitComplete {{ get; }}", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        string LastOperatorPrompt {{ get; }}", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        #endregion Properties", Environment.NewLine); // 


                oSbStateMachineInterface.AppendFormat(@"{0}{0}        #region Events", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        event StateChangeEventHandler eventStateChange;", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        #endregion Events", Environment.NewLine); // 

                oSbStateMachineInterface.AppendFormat(@"{0}{0}        #region Methods", Environment.NewLine); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        bool HandleEvent({1}SystemEventsEnum eEvent, object oValue);", Environment.NewLine, m_strStateMachineName); // 
                oSbStateMachineInterface.AppendFormat(@"{0}        #endregion Methods", Environment.NewLine); // 

                oSbStateMachineInterface.AppendFormat(@"{0}    }} // End I{1}StateMachine", Environment.NewLine, m_strStateMachineName);
                oSbStateMachineInterface.AppendFormat(@"{0}{0}}} // End Namespace {1}", Environment.NewLine, m_strStateMachineNamespaceName);

                ms_iLogger.Log(ELogLevel.Info, string.Format("Adding States."));

                lstSubStates = new List<string>();

                // Handle the Upper Level
                lstNextLevelTupleOfStateNamesAndSubStates = new List<Tuple<string, List<string>>>();
                //lstSubStates.Add(States[0]);
                //lstNextLevelTupleOfStateNamesAndSubStates.Add(new Tuple<string, List<string>>(States[0], lstSubStates));

                lstSubStates.Add("None");
                lstNextLevelTupleOfStateNamesAndSubStates.Add(new Tuple<string, List<string>>(States[0], lstSubStates));
                dictStateLevelAndListOfStatesAndSubStatesOnThatLevel.Add(0, lstNextLevelTupleOfStateNamesAndSubStates);

                nStateLevel = 0;

                HashSet<string> systInrftgeneratedActions = new HashSet<string>(); // to prevent duplicated code generation
                HashSet<string> systGeneratedActions = new HashSet<string>(); // to prevent duplicated code generation
                HashSet<string> m_lstAddedSystemTransitionActions = new HashSet<string>(); // to prevent duplicated code generation


                while (bSubStatesFound == true)
                {
                    bSubStatesFound = false;

                    if (dictStateLevelAndListOfStatesAndSubStatesOnThatLevel.TryGetValue(nStateLevel, out lstTupleOfStateNamesAndSubStates) == true) // Get the List of States for this level
                    {
                        nStateLevel++; // These States go on the next level

                        lstNextLevelTupleOfStateNamesAndSubStates = new List<Tuple<string, List<string>>>();

                        foreach (Tuple<string, List<string>> tupleOfStateAndSubStateList in lstTupleOfStateNamesAndSubStates) // Loop through all of the States at this level and look for its SubStates
                        {
                            // Find Substates of strStateName
                            foreach (string strStateName in tupleOfStateAndSubStateList.Item2)
                            {
                                lstSubStates = new List<string>();
                                if (ParentStates.Where(p => p == strStateName).Any())
                                {
                                    oSbStateMachineSourceCode.AppendFormat("{0}{0}        // {1} SubState Definitions", Environment.NewLine, strStateName);
                                    oSbStateInstantiations.AppendFormat("{0}{0}        // {1} SubState Instantiations", Environment.NewLine, strStateName);
                                }

                                // Loop through all of the Parent State Names looking for all match with the State Name
                                for (nIndex = 0; nIndex < States.Count; nIndex++)
                                {
                                    if (ParentStates[nIndex] == strStateName)
                                    {
                                        lstSubStates.Add(States[nIndex]);
                                        oSbStateMachineSourceCode.AppendFormat("{0}        public NSF{1}State   o{2};", Environment.NewLine, StateTypes[nIndex], States[nIndex]);

                                        if (States[nIndex].EndsWith("State"))
                                        {
                                            strEnumStateValue = States[nIndex].Substring(0, States[nIndex].Length - 5);
                                        }
                                        else
                                        {
                                            strEnumStateValue = States[nIndex];
                                        }

                                        switch (StateTypes[nIndex])
                                        {
                                            case "Composite":
                                                //if (ParentStates[nIndex] == States[0]) // Removed 01/02/2017
                                                if (ParentStates[nIndex] == "None")
                                                {
                                                    //oSbStateInstantiations.AppendFormat(@"{0}                o{1} = new NSFCompositeState(E{2}State.{3}.ToString(), this, {1}EntryAction, {1}ExitAction);", Environment.NewLine, States[nIndex], m_strStateMachineName, strEnumStateValue);
                                                    oSbStateInstantiations.AppendFormat(@"{0}                o{1} = new NSFCompositeState(EState.{3}.ToString(), this, {1}EntryAction, {1}ExitAction);", Environment.NewLine, States[nIndex], m_strStateMachineName, strEnumStateValue);
                                                }
                                                else
                                                {
                                                    //oSbStateInstantiations.AppendFormat(@"{0}                o{1} = new NSFCompositeState(E{2}State.{3}.ToString(), o{4}, {1}EntryAction, {1}ExitAction);", Environment.NewLine, States[nIndex], m_strStateMachineName, strEnumStateValue, ParentStates[nIndex]);
                                                    oSbStateInstantiations.AppendFormat(@"{0}                o{1} = new NSFCompositeState(EState.{3}.ToString(), o{4}, {1}EntryAction, {1}ExitAction);", Environment.NewLine, States[nIndex], m_strStateMachineName, strEnumStateValue, ParentStates[nIndex]);
                                                }


                                                // Now Add the Entry and Exit Actions
                                                var entryActions = StateMachineMetadata.Main.ActiveModel.States
                                                    .Single(s => s.Name == States[nIndex]).EntryActions.Where(s => !string.IsNullOrEmpty(s)).Distinct();
                                                string entryActionForStMc = string.Format("{0}EntryAction", States[nIndex]);

                                                // Generate the Entry Action for the State Machine
                                                oSbEntryAndExitActions.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                oSbEntryAndExitActions.AppendFormat(@"{0}        /// This method is called when the {1} State is Entered.", Environment.NewLine, strEnumStateValue);
                                                oSbEntryAndExitActions.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                oSbEntryAndExitActions.AppendFormat(@"{0}        /// <param name=""oContext"">Information about the states before and after the transition as well as the transition and trigger.</param> ", Environment.NewLine);
                                                oSbEntryAndExitActions.AppendFormat(@"{0}        /// <returns>Nothing</returns> #9", Environment.NewLine);

                                                if (m_iStateMachineSourceCodeFileUpdater.DoesMethodOrPropertyAlreadyExistInRegionOfLoadedFile("Auto-Generated State Machine Methods", entryActionForStMc, out sbMethodOrPropertyWithDocumentation, out sbMethodOrPropertyWithoutDocumentation, "TREAT IT AS A KEEP_OUT AREA.") == true)
                                                {
                                                    oSbEntryAndExitActions.AppendFormat(@"{0}", Environment.NewLine);
                                                    oSbEntryAndExitActions.Append(sbMethodOrPropertyWithoutDocumentation);
                                                }
                                                else
                                                {
                                                    //oSbEntryAndExitActions.AppendFormat(@"{0}        private void {1}EntryAction(NSFStateMachineContext oContext)", Environment.NewLine, States[nIndex]);
                                                    oSbEntryAndExitActions.AppendFormat(@"{0}        private void {1}(NSFStateMachineContext oContext)", Environment.NewLine, entryActionForStMc);
                                                    oSbEntryAndExitActions.AppendFormat(@"{0}        {{", Environment.NewLine);
                                                    foreach (var action in entryActions)
                                                    {
                                                        var actionMethodName = string.Format("{0}", action);
                                                        //oSbEntryAndExitActions.AppendFormat($"{Environment.NewLine}            m_i{m_strStateMachineName}.{actionMethodName}();");
                                                        oSbEntryAndExitActions.AppendFormat($"{Environment.NewLine}            m_i{m_strStateMachineName}.{actionMethodName}();");
                                                    }
                                                    oSbEntryAndExitActions.AppendFormat(@"{0}        }} ", Environment.NewLine);
                                                    oSbEntryAndExitActions.AppendFormat(@"{0}", Environment.NewLine);
                                                }

                                                // Generate the System Interface Prototype for the Entry Action
                                                foreach (var action in entryActions)
                                                {
                                                    var actionMethodName = string.Format("{0}", action);
                                                    if (systInrftgeneratedActions.Contains(actionMethodName)) continue;
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// This method is called when the {1} State is Entered.", Environment.NewLine, strEnumStateValue);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// <returns>Nothing</returns> #A", Environment.NewLine);
                                                    //oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        void {1}EntryAction();", Environment.NewLine, States[nIndex]);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        void {1}();", Environment.NewLine, actionMethodName);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}", Environment.NewLine);
                                                    systInrftgeneratedActions.Add(actionMethodName);
                                                }

                                                //// Now generate the Entry Action Method for the State Machine Owner

                                                // Now generate the Entry Action METHOD for the State Machine's Owner (System file)
                                                foreach (var action in entryActions)
                                                {
                                                    var actionMethodName = string.Format("{0}", action);
                                                    if (systGeneratedActions.Contains(actionMethodName)) continue;
                                                    if (m_iOwnerSourceCodeFileUpdater.DoesMethodOrPropertyAlreadyExistInRegionOfLoadedFile(string.Format("StateMachine{0}Methods", m_strStateMachineName), actionMethodName, out sbMethodOrPropertyWithDocumentation, out sbMethodOrPropertyWithoutDocumentation, "TREAT IT AS A KEEP_OUT AREA.") == true)
                                                    {
                                                        oSbStateMachineOwnerMethods.Append(sbMethodOrPropertyWithDocumentation);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}", Environment.NewLine);
                                                    }
                                                    else
                                                    {
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        /// This method is called when the {1} State is Entered.", Environment.NewLine, strEnumStateValue);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        /// <returns>Nothing</returns> #B", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        public void {1}()", Environment.NewLine, actionMethodName);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        {{", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}               ", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        }} ", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}", Environment.NewLine);
                                                    }
                                                    systGeneratedActions.Add(actionMethodName);
                                                }

                                                var exitActions = StateMachineMetadata.Main.ActiveModel.States
                                                    .Single(s => s.Name == States[nIndex]).ExitActions.Where(s => !string.IsNullOrEmpty(s)).Distinct();
                                                string exitActionForStMc = string.Format("{0}ExitAction", States[nIndex]);

                                                // Generate the Exit Action for the State Machine
                                                oSbEntryAndExitActions.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                oSbEntryAndExitActions.AppendFormat(@"{0}        /// This method is called when the {1} State is Exited.", Environment.NewLine, strEnumStateValue);
                                                oSbEntryAndExitActions.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                oSbEntryAndExitActions.AppendFormat(@"{0}        /// <param name=""oContext"">Information about the states before and after the transition as well as the transition and trigger.</param>", Environment.NewLine);
                                                oSbEntryAndExitActions.AppendFormat(@"{0}        /// <returns>Nothing</returns> #C", Environment.NewLine);

                                                if (m_iStateMachineSourceCodeFileUpdater.DoesMethodOrPropertyAlreadyExistInRegionOfLoadedFile("Auto-Generated State Machine Methods", exitActionForStMc, out sbMethodOrPropertyWithDocumentation, out sbMethodOrPropertyWithoutDocumentation, "TREAT IT AS A KEEP_OUT AREA.") == true)
                                                {
                                                    oSbEntryAndExitActions.AppendFormat(@"{0}", Environment.NewLine);
                                                    oSbEntryAndExitActions.Append(sbMethodOrPropertyWithoutDocumentation);
                                                }
                                                else
                                                {
                                                    oSbEntryAndExitActions.AppendFormat(@"{0}        private void {1}(NSFStateMachineContext oContext)", Environment.NewLine, exitActionForStMc);
                                                    oSbEntryAndExitActions.AppendFormat(@"{0}        {{", Environment.NewLine);
                                                    foreach (var action in exitActions)
                                                    {
                                                        var actionMethodName = string.Format("{0}", action);
                                                        oSbEntryAndExitActions.AppendFormat($"{Environment.NewLine}            m_i{m_strStateMachineName}.{actionMethodName}();");
                                                    }
                                                    oSbEntryAndExitActions.AppendFormat(@"{0}        }}", Environment.NewLine);
                                                    oSbEntryAndExitActions.AppendFormat(@"{0}", Environment.NewLine);
                                                }

                                                // Generate the System Interface Prototype for the Exit Action
                                                foreach (var action in exitActions)
                                                {
                                                    var actionMethodName = string.Format("{0}", action);
                                                    if (systInrftgeneratedActions.Contains(actionMethodName)) continue;
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// This method is called when the {1} State is Exited.", Environment.NewLine, strEnumStateValue);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// <returns>Nothing</returns> #D", Environment.NewLine);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        void {1}();", Environment.NewLine, actionMethodName);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}", Environment.NewLine);
                                                    systInrftgeneratedActions.Add(actionMethodName);
                                                }

                                                // Now generate the Exit Action Method for the State Machine Owner
                                                foreach (var action in exitActions)
                                                {
                                                    var actionMethodName = string.Format("{0}", action);
                                                    if (systGeneratedActions.Contains(actionMethodName)) continue;
                                                    if (m_iStateMachineSourceCodeFileUpdater.DoesMethodOrPropertyAlreadyExistInRegionOfLoadedFile(string.Format("StateMachine{0}Methods", m_strStateMachineName), actionMethodName, out sbMethodOrPropertyWithDocumentation, out sbMethodOrPropertyWithoutDocumentation, "TREAT IT AS A KEEP_OUT AREA.") == true)
                                                    {
                                                        oSbStateMachineOwnerMethods.Append(sbMethodOrPropertyWithDocumentation);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}", Environment.NewLine);
                                                    }
                                                    else
                                                    {
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        /// This method is called when the {1} State is Exited.", Environment.NewLine, strEnumStateValue);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        /// <returns>Nothing</returns>", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        public void {1}()", Environment.NewLine, actionMethodName);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        {{", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}               ", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}        }} ", Environment.NewLine);
                                                        oSbStateMachineOwnerMethods.AppendFormat(@"{0}", Environment.NewLine);
                                                    }
                                                    systGeneratedActions.Add(actionMethodName);
                                                }

                                                break;

                                            case "Choice":
                                                oSbStateInstantiations.AppendFormat(@"{0}                o{1} = new NSFChoiceState(EState.{3}.ToString(), o{4});", Environment.NewLine, States[nIndex], m_strStateMachineName, strEnumStateValue, ParentStates[nIndex]);


                                                break;

                                            case "Initial":
                                                oSbStateInstantiations.AppendFormat(@"{0}                o{1} = new NSFInitialState(EState.{3}.ToString(), o{4});", Environment.NewLine, States[nIndex], m_strStateMachineName, strEnumStateValue, ParentStates[nIndex]);
                                                break;

                                            default:
                                                break;
                                        }



                                        bSubStatesFound = true;
                                    }
                                }

                                if (lstSubStates.Count > 0)
                                {
                                    lstNextLevelTupleOfStateNamesAndSubStates.Add(new Tuple<string, List<string>>(strStateName, lstSubStates));
                                }
                            }
                        }

                        if (lstNextLevelTupleOfStateNamesAndSubStates.Count > 0)
                        {
                            dictStateLevelAndListOfStatesAndSubStatesOnThatLevel.Add(nStateLevel, lstNextLevelTupleOfStateNamesAndSubStates);
                        }
                    }
                    else
                    {
                        bSubStatesFound = false;
                    }
                }

                ms_iLogger.Log(ELogLevel.Info, string.Format("Adding Transitions."));
                oSbStateMachineOwnerProperties.AppendFormat($@"{Environment.NewLine}        public C{m_strStateMachineName}StateMachine StateMachine  //1!");
                oSbStateMachineOwnerProperties.AppendFormat(@"{0}        {{", Environment.NewLine);
                oSbStateMachineOwnerProperties.AppendFormat(@"{0}            get {{ return m_o{1}StateMachine; }}", Environment.NewLine, m_strStateMachineName);
                oSbStateMachineOwnerProperties.AppendFormat(@"{0}        }}", Environment.NewLine);
                oSbStateMachineOwnerProperties.AppendFormat(@"{0}", Environment.NewLine);

                //oSbStateMachineOwnerInterfaceProperties.AppendFormat($@"{Environment.NewLine}        C{m_strStateMachineName} StateMachine {{ get; }} //1!");

                List<string> lstGuardSimulationNames = new List<string>();
                for (nStateLevel = 0; nStateLevel < dictStateLevelAndListOfStatesAndSubStatesOnThatLevel.Count; nStateLevel++)
                {
                    if (dictStateLevelAndListOfStatesAndSubStatesOnThatLevel.TryGetValue(nStateLevel, out lstTupleOfStateNamesAndSubStates) == true) // Get the List of States for this level
                    {
                        foreach (Tuple<string, List<string>> tupleOfStateAndSubStateList in lstTupleOfStateNamesAndSubStates) // Loop through all of the States at this level and look for its SubStates
                        {
                            // Find Substates of strStateName
                            foreach (string strStateName in tupleOfStateAndSubStateList.Item2)
                            {
                                lstSubStates = new List<string>();
                                oSbStateMachineSourceCode.AppendFormat("{0}{0}        // Transitions for the {1}", Environment.NewLine, strStateName);
                                oSbStateInstantiations.AppendFormat("{0}{0}             // Transition Instantiations for the {1}", Environment.NewLine, strStateName);
                                for (nTransitionIndex = 0; nTransitionIndex < m_lstTransitionFromStates.Count; nTransitionIndex++)
                                {
                                    if (m_lstTransitionFromStates[nTransitionIndex] == strStateName)
                                    {
                                        //switch (m_lstTransitionTypes[nTransitionIndex])
                                        //{
                                        //    case "External":

                                                // Remove
                                                strTransitionFromState = m_lstTransitionFromStates[nTransitionIndex];

                                                if (strTransitionFromState.EndsWith("State"))
                                                {
                                                    strTransitionFromState = strTransitionFromState.Substring(0, strTransitionFromState.Length - 5);
                                                }

                                                strTransitionToState = m_lstTransitionToStates[nTransitionIndex];

                                                if (strTransitionToState.EndsWith("State"))
                                                {
                                                    strTransitionToState = strTransitionToState.Substring(0, strTransitionToState.Length - 5);
                                                }

                                                nNumberOfTransitionsWithSameFromAndToStates = 0;
                                                for (nDuplicateTransitionSearcherIndex = 0; nDuplicateTransitionSearcherIndex < m_lstTransitionFromStates.Count; nDuplicateTransitionSearcherIndex++)
                                                {
                                                    if ((m_lstTransitionFromStates[nTransitionIndex] == m_lstTransitionFromStates[nDuplicateTransitionSearcherIndex]) && (m_lstTransitionToStates[nTransitionIndex] == m_lstTransitionToStates[nDuplicateTransitionSearcherIndex]))
                                                    {
                                                        nNumberOfTransitionsWithSameFromAndToStates++;
                                                    }
                                                }

                                                if (nNumberOfTransitionsWithSameFromAndToStates == 1)
                                                {
                                                    strInstantiatedTransitionName = string.Format("{0}To{1}Transition", strTransitionFromState, strTransitionToState);
                                                    strTransitionName = string.Format("{0}To{1}", strTransitionFromState, strTransitionToState);
                                                }
                                                else
                                                {
                                                    strInstantiatedTransitionName = string.Format("{0}To{1}TransitionBy{2}", strTransitionFromState, strTransitionToState, m_lstTransitionEvents[nTransitionIndex]);
                                                    strTransitionName = string.Format("{0}To{1}By{2}", strTransitionFromState, strTransitionToState, m_lstTransitionEvents[nTransitionIndex]);
                                                }

                                        
                                        // Create the Member definition for the transition
                                        switch (m_lstTransitionTypes[nTransitionIndex])
                                        {
                                            case "External":
                                                oSbStateMachineSourceCode.AppendFormat(@"{0}        private NSFExternalTransition {1};", Environment.NewLine, strInstantiatedTransitionName);
                                                break;

                                            case "Internal":
                                                oSbStateMachineSourceCode.AppendFormat(@"{0}        private NSFInternalTransition {1};", Environment.NewLine, strInstantiatedTransitionName);
                                                break;

                                            default:
                                                break;
                                        }

                                        // Compose the Transition Trigger Name
                                        if (m_lstTransitionEvents[nTransitionIndex] != "None")
                                                {
                                                    strTransitionTrigger = m_lstTransitionEvents[nTransitionIndex];
                                                }
                                                else
                                                {
                                                    strTransitionTrigger = "null";
                                                }


                                                var lstSimulatorsEnabledNames = new List<string>();
                                                // Compose the Transition Guard Name and its Method
                                                if ((m_lstTransitionGuards[nTransitionIndex] != "None") && (m_lstTransitionGuards[nTransitionIndex] != "Else"))
                                                {
                                                    strTransitionGuard = m_lstTransitionGuards[nTransitionIndex];
                                                    if (lstInstantiatedGuards.Contains(strTransitionGuard) == false)
                                                    {
                                                        lstInstantiatedGuards.Add(strTransitionGuard);


                                                        //oSbStateMachineTransitionGuards.AppendFormat(@"{0}", Environment.NewLine); // Removed because of double line spaces when using preexisting code
                                                        oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                        oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// A Guard function returning the State of {1}.", Environment.NewLine, strTransitionGuard);
                                                        oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                        oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// <param name=""oContext"">Information about the states before and after the transition as well as the transition and trigger.</param>", Environment.NewLine);
                                                        oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// <returns>The State of {1}</returns>", Environment.NewLine, strTransitionGuard);


                                                        if (m_iStateMachineSourceCodeFileUpdater.DoesMethodOrPropertyAlreadyExistInRegionOfLoadedFile("Auto-Generated State Machine Methods", strTransitionGuard, out sbMethodOrPropertyWithDocumentation, out sbMethodOrPropertyWithoutDocumentation, "TREAT IT AS A KEEP_OUT AREA.") == true)
                                                        {
                                                            oSbStateMachineTransitionGuards.AppendFormat(@"{0}", Environment.NewLine);
                                                            oSbStateMachineTransitionGuards.Append(sbMethodOrPropertyWithoutDocumentation);
                                                        }
                                                        else
                                                        {
                                                            oSbStateMachineTransitionGuards.AppendFormat(@"{0}        private bool {1}(NSFStateMachineContext oContext)", Environment.NewLine, strTransitionGuard);
                                                            oSbStateMachineTransitionGuards.AppendFormat(@"{0}        {{", Environment.NewLine);
                                                            //oSbStateMachineTransitionGuards.AppendFormat(string.Format(@"{0}            return true;", Environment.NewLine)); m_i{0};", m_strStateMachineName)
                                                            oSbStateMachineTransitionGuards.AppendFormat(string.Format(@"{0}            return m_i{1}.{2};", Environment.NewLine, m_strStateMachineName, strTransitionGuard));
                                                            oSbStateMachineTransitionGuards.AppendFormat(@"{0}        }}", Environment.NewLine);
                                                        }

                                   
                                                        // GuardSimulation value's duplicated handling
                                                        string strGuardSimulationName = $"{m_strStateMachineName}GuardSimulation{strTransitionGuard}";
                                                        if (!lstGuardSimulationNames.Contains(strGuardSimulationName))
                                                        {
                                                            oSbStateMachineOwnerProperties.AppendFormat($@"{Environment.NewLine}        public bool {strGuardSimulationName}");
                                                            //oSbStateMachineOwnerProperties.AppendFormat(@"{0}        public bool {1}GuardSimulation{2}", Environment.NewLine, m_strStateMachineName, strTransitionGuard);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}        {{", Environment.NewLine);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}            get {{ return m_b{1}GuardSimulation{2}; }}", Environment.NewLine, m_strStateMachineName, strTransitionGuard);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}            set {{ m_b{1}GuardSimulation{2} = value; }}", Environment.NewLine, m_strStateMachineName, strTransitionGuard);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}        }}", Environment.NewLine);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}", Environment.NewLine);

                                                            oSbStateMachineOwnerInterfaceProperties.AppendFormat(@"{0}        bool {1} {{ get; set; }}", Environment.NewLine, strGuardSimulationName);
                                                        }

                                                        // GuardSimulationEnabled value's duplicated handling
                                                        string strSimulationEnabledPropertyName = $"Is{m_strStateMachineName}GuardSimulationEnabledFor{strTransitionGuard}";
                                                        if (lstSimulatorsEnabledNames.Contains(strSimulationEnabledPropertyName))
                                                        {
                                                            lstSimulatorsEnabledNames.Add(strSimulationEnabledPropertyName);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}        public bool {1}", Environment.NewLine, strSimulationEnabledPropertyName);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}        {{", Environment.NewLine);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}            get {{ return m_bIs{1}GuardSimulationEnabledFor{2}; }}", Environment.NewLine, m_strStateMachineName, strTransitionGuard);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}            set {{ m_bIs{1}GuardSimulationEnabledFor{2} = value; }}", Environment.NewLine, m_strStateMachineName, strTransitionGuard);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}        }}", Environment.NewLine);
                                                            oSbStateMachineOwnerProperties.AppendFormat(@"{0}", Environment.NewLine);

                                                            oSbStateMachineOwnerInterfaceProperties.AppendFormat(@"{0}        bool Is{1}GuardSimulationEnabledFor{2} {{ get; set; }}", Environment.NewLine, m_strStateMachineName, strTransitionGuard);
                                                        }

                                                        // Now add the Transition Guard Simulation Fields for the Owner
                                                        oSbStateMachineOwnerFields.AppendFormat(@"{0}        public bool m_b{1}GuardSimulation{2} = false;", Environment.NewLine, m_strStateMachineName, strTransitionGuard);
                                                        oSbStateMachineOwnerFields.AppendFormat(@"{0}        public bool m_bIs{1}GuardSimulationEnabledFor{2} = false;", Environment.NewLine, m_strStateMachineName, strTransitionGuard);


                                                        // Now add the Transition Guard Properties for the Owner

                                                        if (m_iOwnerSourceCodeFileUpdater.DoesMethodOrPropertyAlreadyExistInRegionOfLoadedFile(string.Format("StateMachine{0}Properties", m_strStateMachineName), strTransitionGuard, out sbMethodOrPropertyWithDocumentation, out sbMethodOrPropertyWithoutDocumentation, "TREAT IT AS A KEEP_OUT AREA.") == true)
                                                        {
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.Append(sbMethodOrPropertyWithDocumentation);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}", Environment.NewLine);
                                                        }
                                                        else
                                                        {
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}        /// The Guard function returning the State or Simulation State of {1}.", Environment.NewLine, strTransitionGuard);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}        /// <returns>The State of {1}</returns>", Environment.NewLine, strTransitionGuard);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}        public bool {1}", Environment.NewLine, strTransitionGuard);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}        {{", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}            get", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}            {{", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}                if (m_bIs{1}GuardSimulationEnabledFor{2} == true) // Check to see if the Guard's Simulation Value should be used.", Environment.NewLine, m_strStateMachineName, strTransitionGuard);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}                {{", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}                    return m_b{1}GuardSimulation{2};  // Return the Guard's Simulation value and not the real one.", Environment.NewLine, m_strStateMachineName, strTransitionGuard);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}                }}", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}                else", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}                {{", Environment.NewLine);
                                                            if (strTransitionGuard.StartsWith("IsCriticalAlarm") || strTransitionGuard.EndsWith("_No"))
                                                                oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}                   return false; //1@ ToDo: Please add code which returns the Guard's current state.  ", Environment.NewLine);
                                                            else
                                                                oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}                   return true; //1@ ToDo: Please add code which returns the Guard's current state.  ", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}                }}", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}            }}", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0}        }}", Environment.NewLine);
                                                            oSbStateMachineOwnerTransitionGuardProperties.AppendFormat(@"{0} ", Environment.NewLine);
                                                        }


                                                        // Now add the Transition Guard Properties for the Owner's Interface
                                                        //
                                                        oSbStateMachineOwnerInterfaceTransitionGuardProperties.AppendFormat(@"{0}", Environment.NewLine);
                                                        oSbStateMachineOwnerInterfaceTransitionGuardProperties.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                        oSbStateMachineOwnerInterfaceTransitionGuardProperties.AppendFormat(@"{0}        /// 2 - The Guard function returning the State or Simulation State of {1}.", Environment.NewLine, strTransitionGuard);
                                                        oSbStateMachineOwnerInterfaceTransitionGuardProperties.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                        oSbStateMachineOwnerInterfaceTransitionGuardProperties.AppendFormat(@"{0}        /// <returns>The State of {1}</returns>", Environment.NewLine, strTransitionGuard);
                                                        oSbStateMachineOwnerInterfaceTransitionGuardProperties.AppendFormat(@"{0}        bool {1} {{ get; }}", Environment.NewLine, strTransitionGuard);
                                                        oSbStateMachineOwnerInterfaceTransitionGuardProperties.AppendFormat(@"{0} ", Environment.NewLine);
                                                    }
                                                }
                                                else if (m_lstTransitionGuards[nTransitionIndex] == "Else")
                                                {
                                                    strTransitionGuard = "Else"; // The Else for the Choice transitions
                                                }
                                                else
                                                {
                                                    strTransitionGuard = "null";
                                                }


                                                //--------------------Transition Action---------------------------
                                                // Now Compose the Transition Action, its Method and prototype for The System Interface.
                                                if (m_lstTransitionActions[nTransitionIndex] != "None")
                                                {
                                                    strTransitionAction = m_lstTransitionActions[nTransitionIndex];
                                            
                                                    if (strTransitionAction == "null") strStateMachineTransitionAction = "null";
                                                    else strStateMachineTransitionAction = strInstantiatedTransitionName.Replace("Transition", "") + "Action";

                                                    if (lstInstantiatedActions.Contains(strStateMachineTransitionAction) == false)
                                                    {
                                                        lstInstantiatedActions.Add(strStateMachineTransitionAction);

                                                        nCharIndex = strTransitionAction.IndexOf('(');

                                                        if (nCharIndex >= 0)
                                                        {
                                                            strTransitionActionWithNoParameters = strTransitionAction.Substring(0, strTransitionAction.IndexOf('('));
                                                            strFormattedTransitionActionParameters = strTransitionAction.Substring(strTransitionAction.IndexOf('(') + 1);
                                                            strFormattedTransitionActionParameters = strFormattedTransitionActionParameters.TrimEnd(new char[] { ')' });
                                                            astrTransitionActionParameters = strFormattedTransitionActionParameters.Split(new char[] { ',' });

                                                            // Needs more work

                                                        }

                                                //// Add the Transition Action for the State Machine
                                                //oSbStateMachineTransitionActions.AppendFormat(@"{0}", Environment.NewLine);
                                                //oSbStateMachineTransitionActions.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                ////oSbStateMachineTransitionActions.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nTransitionIndex], m_lstTransitionToStates[nTransitionIndex], m_lstTransitionEvents[nTransitionIndex]);

                                                //for (int nInstantiatedActionIndex = 0; nInstantiatedActionIndex < m_lstTransitionFromStates.Count; nInstantiatedActionIndex++)
                                                //{
                                                //    if (m_lstTransitionActions[nInstantiatedActionIndex] == strTransitionAction)
                                                //    {
                                                //        if (m_lstTransitionEvents[nInstantiatedActionIndex] != "")
                                                //        {
                                                //            if (m_dictDataEventNameAndDataType.TryGetValue(m_lstTransitionEvents[nInstantiatedActionIndex], out strDataEventDataType) == true) // Then it is a Data Event
                                                //            {
                                                //                oSbStateMachineTransitionActions.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3} and carries a {4} object.", Environment.NewLine, m_lstTransitionFromStates[nInstantiatedActionIndex], m_lstTransitionToStates[nInstantiatedActionIndex], m_lstTransitionEvents[nInstantiatedActionIndex], strDataEventDataType);
                                                //            }
                                                //            else
                                                //            {
                                                //                oSbStateMachineTransitionActions.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nInstantiatedActionIndex], m_lstTransitionToStates[nInstantiatedActionIndex], m_lstTransitionEvents[nInstantiatedActionIndex]);
                                                //            }
                                                //        }
                                                //        else
                                                //        {
                                                //            oSbStateMachineTransitionActions.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nInstantiatedActionIndex], m_lstTransitionToStates[nInstantiatedActionIndex], m_lstTransitionEvents[nInstantiatedActionIndex]);
                                                //        }


                                                //    }
                                                //}


                                                //oSbStateMachineTransitionActions.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                //oSbStateMachineTransitionActions.AppendFormat(@"{0}        /// <param name=""oContext"">Information about the states before and after the transition as well as the transition and trigger.</param> ", Environment.NewLine);
                                                //oSbStateMachineTransitionActions.AppendFormat(@"{0}        /// <returns>Nothing</returns> #8", Environment.NewLine);

                                                //if (m_iStateMachineSourceCodeFileUpdater.DoesMethodOrPropertyAlreadyExistInRegionOfLoadedFile("Auto-Generated State Machine Methods", strTransitionAction, out sbMethodOrPropertyWithDocumentation, out sbMethodOrPropertyWithoutDocumentation, "TREAT IT AS A KEEP_OUT AREA.") == true)
                                                //{
                                                //    oSbStateMachineTransitionActions.AppendFormat(@"{0}", Environment.NewLine);
                                                //    oSbStateMachineTransitionActions.Append(sbMethodOrPropertyWithoutDocumentation);
                                                //}
                                                //else
                                                //{

                                                //    oSbStateMachineTransitionActions.AppendFormat(@"{0}        private void {1}(NSFStateMachineContext oContext)", Environment.NewLine, strStateMachineTransitionAction);
                                                //    oSbStateMachineTransitionActions.AppendFormat(@"{0}        {{", Environment.NewLine);

                                                //    if (strTransitionAction == "InitAfterInitSettingsAction")
                                                //        oSbStateMachineTransitionActions.AppendFormat(@"{0}             m_i{1}.InitAfterSettings();", Environment.NewLine, m_strStateMachineName);

                                                //    // Find the Events associated with this Transition Action
                                                //    for (int nTransitionActionIndex = 0; nTransitionActionIndex < m_lstTransitionActions.Count; nTransitionActionIndex++)
                                                //    {
                                                //        string strEventAssociatedWithTransition = m_lstTransitionEvents[nTransitionActionIndex];

                                                //        if (strEventAssociatedWithTransition != "")
                                                //        {
                                                //            if (m_dictDataEventNameAndDataType.TryGetValue(strEventAssociatedWithTransition, out strDataEventDataType) == true) // Then it is a Data Event
                                                //            {
                                                //                oSbStateMachineTransitionActions.AppendFormat(@"{0}              // This code captures the Data passed with the Event named {1}   = ({2})((CStateMachineEventData)o{3}.Source).EventData;", Environment.NewLine, strEventAssociatedWithTransition, strDataEventDataType, strEventAssociatedWithTransition);
                                                //            }
                                                //        }
                                                //    }


                                                //    //oSbStateMachineTransitionActions.AppendFormat(@"{0}            m_i{1}.{2}();", Environment.NewLine, m_strStateMachineName, strTransitionAction);
                                                //    oSbStateMachineTransitionActions.AppendFormat(@"{0}        }}", Environment.NewLine);
                                                //}

                                                // Add the Transition Action Prototype to the Owner's Interface
                                                if (!systInrftgeneratedActions.Contains(strTransitionAction))
                                                {
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}", Environment.NewLine);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                    //oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nTransitionIndex], m_lstTransitionToStates[nTransitionIndex], m_lstTransitionEvents[nTransitionIndex]);
                                                    for (int nInstantiatedActionIndex = 0; nInstantiatedActionIndex < m_lstTransitionFromStates.Count; nInstantiatedActionIndex++)
                                                    {
                                                        if (m_lstTransitionActions[nInstantiatedActionIndex] == strTransitionAction)
                                                        {
                                                            oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nInstantiatedActionIndex], m_lstTransitionToStates[nInstantiatedActionIndex], m_lstTransitionEvents[nInstantiatedActionIndex]);
                                                        }
                                                    }

                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"            // #3");
                                                    oSbStateMachineOwnerInterfaceMethods.AppendFormat(@"{0}        void {1}();", Environment.NewLine, strTransitionAction);
                                                    systInrftgeneratedActions.Add(strTransitionAction);
                                                }

                                                //if (strTransitionAction == "null") strStateMachineTransitionAction = "null";
                                                //else strStateMachineTransitionAction = strInstantiatedTransitionName.Replace("Transition", "") + "Action";


                                                // Add the Transition Actions to the System
                                                if (!m_lstAddedSystemTransitionActions.Contains(strTransitionAction))
                                                {
                                                    if (m_iOwnerSourceCodeFileUpdater.DoesMethodOrPropertyAlreadyExistInRegionOfLoadedFile(string.Format("StateMachine{0}Methods", m_strStateMachineName), strTransitionAction, out sbMethodOrPropertyWithDocumentation, out sbMethodOrPropertyWithoutDocumentation, "TREAT IT AS A KEEP_OUT AREA.") == true)
                                                    {
                                                        if (strTransitionAction == "WriteMeasResultsReportAsync")
                                                        {

                                                        }

                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}", Environment.NewLine);
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);

                                                        for (int nInstantiatedActionIndex = 0; nInstantiatedActionIndex < m_lstTransitionFromStates.Count; nInstantiatedActionIndex++)
                                                        {
                                                            if (m_lstTransitionActions[nInstantiatedActionIndex] == strTransitionAction)
                                                            {
                                                                oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nInstantiatedActionIndex], m_lstTransitionToStates[nInstantiatedActionIndex], m_lstTransitionEvents[nInstantiatedActionIndex]);
                                                            }
                                                        }
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        /// <returns>Nothing</returns>", Environment.NewLine);
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"            // #4");

                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}", Environment.NewLine);
                                                        oSbStateMachineOwnerTransitionActions.Append(sbMethodOrPropertyWithoutDocumentation);
                                                        //oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}", Environment.NewLine); // Removed 12/27/2016
                                                    }
                                                    else
                                                    {
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}", Environment.NewLine);
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                        //oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nTransitionIndex], m_lstTransitionToStates[nTransitionIndex], m_lstTransitionEvents[nTransitionIndex]);
                                                        for (int nInstantiatedActionIndex = 0; nInstantiatedActionIndex < m_lstTransitionFromStates.Count; nInstantiatedActionIndex++)
                                                        {
                                                            if (m_lstTransitionActions[nInstantiatedActionIndex] == strTransitionAction)
                                                            {
                                                                oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nInstantiatedActionIndex], m_lstTransitionToStates[nInstantiatedActionIndex], m_lstTransitionEvents[nInstantiatedActionIndex]);
                                                            }
                                                        }
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        /// <returns>Nothing</returns> #1", Environment.NewLine);
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        public void {1}()", Environment.NewLine, strTransitionAction);
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        {{", Environment.NewLine);
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}            ", Environment.NewLine);
                                                        oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        }}", Environment.NewLine);
                                                    }
                                                    m_lstAddedSystemTransitionActions.Add(strTransitionAction);
                                                }


                                                // Add the Transition Actions to the State Machine
                                                if (m_iStateMachineSourceCodeFileUpdater.DoesMethodOrPropertyAlreadyExistInRegionOfLoadedFile(string.Format("StateMachine{0}Methods", m_strStateMachineName), strStateMachineTransitionAction, out sbMethodOrPropertyWithDocumentation, out sbMethodOrPropertyWithoutDocumentation, "TREAT IT AS A KEEP_OUT AREA.") == true)
                                                {

                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}", Environment.NewLine);
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);

                                                    for (int nInstantiatedActionIndex = 0; nInstantiatedActionIndex < m_lstTransitionFromStates.Count; nInstantiatedActionIndex++)
                                                    {
                                                        if (m_lstTransitionActions[nInstantiatedActionIndex] == strTransitionAction)
                                                        {
                                                            oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nInstantiatedActionIndex], m_lstTransitionToStates[nInstantiatedActionIndex], m_lstTransitionEvents[nInstantiatedActionIndex]);
                                                        }
                                                    }
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// <returns>Nothing</returns>", Environment.NewLine);
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"            // #5");

                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}", Environment.NewLine);
                                                    oSbStateMachineTransitionGuards.Append(sbMethodOrPropertyWithoutDocumentation);
                                                }
                                                else
                                                {
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}", Environment.NewLine);
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// <summary>", Environment.NewLine);
                                                    //oSbStateMachineOwnerTransitionActions.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nTransitionIndex], m_lstTransitionToStates[nTransitionIndex], m_lstTransitionEvents[nTransitionIndex]);
                                                    for (int nInstantiatedActionIndex = 0; nInstantiatedActionIndex < m_lstTransitionFromStates.Count; nInstantiatedActionIndex++)
                                                    {
                                                        if (m_lstTransitionActions[nInstantiatedActionIndex] == strTransitionAction)
                                                        {
                                                            oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// This method is called when the {1} transitions to {2} because of the {3}.", Environment.NewLine, m_lstTransitionFromStates[nInstantiatedActionIndex], m_lstTransitionToStates[nInstantiatedActionIndex], m_lstTransitionEvents[nInstantiatedActionIndex]);
                                                        }
                                                    }
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// </summary>", Environment.NewLine);
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}        /// <returns>Nothing</returns> #6", Environment.NewLine);
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}        public void {1}(NSFStateMachineContext oContext)", Environment.NewLine, strStateMachineTransitionAction);
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}        {{", Environment.NewLine);
                                                    oSbStateMachineTransitionGuards.AppendFormat($"{Environment.NewLine}            m_i{m_strStateMachineName}.{strTransitionAction}();");
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}        }}", Environment.NewLine);
                                                    oSbStateMachineTransitionGuards.AppendFormat(@"{0}", Environment.NewLine);
                                                }

                                            }
                                                }
                                                else
                                                {
                                                    strTransitionAction = "null";
                                                }


                                                
                                                
                                                
                                                // Add the Transition Instantiation to the State Machine
                                                if (strTransitionTrigger != "null")
                                                {
                                                    strTransitionTrigger = "o" + strTransitionTrigger; // 
                                                }
                                        oSbStateInstantiations.AppendFormat(@"            // #2");

                                        if (strTransitionAction == "null") strStateMachineTransitionAction = "null";
                                        else strStateMachineTransitionAction = strInstantiatedTransitionName.Replace("Transition", "") + "Action";

                                        if (m_lstTransitionTypes[nTransitionIndex] == "External")
                                            oSbStateInstantiations.AppendFormat(@"{0}                 {1} = new  NSFExternalTransition(""{2}"", o{3}, o{4}, {5}, {6}, {7});", Environment.NewLine, strInstantiatedTransitionName, strTransitionName, m_lstTransitionFromStates[nTransitionIndex], m_lstTransitionToStates[nTransitionIndex], strTransitionTrigger, strTransitionGuard, strStateMachineTransitionAction);
                                            //oSbStateInstantiations.AppendFormat(@"{0}                 {1} = new  NSFExternalTransition(""{2}"", o{3}, o{4}, {5}, {6}, {7});", Environment.NewLine, strInstantiatedTransitionName, strTransitionName, m_lstTransitionFromStates[nTransitionIndex], m_lstTransitionToStates[nTransitionIndex], strTransitionTrigger, strTransitionGuard, strTransitionAction);
                                        else
                                            oSbStateInstantiations.AppendFormat(@"{0}                 {1} = new  NSFInternalTransition(""{2}"", o{3}, {5}, {6}, {7});", Environment.NewLine, strInstantiatedTransitionName, strTransitionName, m_lstTransitionFromStates[nTransitionIndex], m_lstTransitionToStates[nTransitionIndex], strTransitionTrigger, strTransitionGuard, strStateMachineTransitionAction);
                                            //oSbStateInstantiations.AppendFormat(@"{0}                 {1} = new  NSFInternalTransition(""{2}"", o{3}, {5}, {6}, {7});", Environment.NewLine, strInstantiatedTransitionName, strTransitionName, m_lstTransitionFromStates[nTransitionIndex], m_lstTransitionToStates[nTransitionIndex], strTransitionTrigger, strTransitionGuard, strTransitionAction);


                                        //        break;

                                        //    case "Internal":

                                        //        break;



                                        //    default:
                                        //        break;
                                        //}
                                    }
                                }

                            }
                        }
                    } // End of Processing a State Level
                } // End of going through all of the State Levels

                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        // ************************************");
                oSbStateMachineSourceCode.AppendLine(@"        // End of State Machine NFS Transitions");
                oSbStateMachineSourceCode.AppendLine(@"        // ************************************");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine("  // DO NOT ADD ANY FIELDS INSIDE THIS AUTO-GENERATED REGION (Above this line). TREAT IT AS A KEEP_OUT AREA AS USER ADDED CODE WILL BE ELIMINATED. Thanks!");
                oSbStateMachineSourceCode.AppendLine(@"        #endregion Auto-Generated State Machine Fields");
                oSbStateMachineSourceCode.AppendLine(@"        #endregion Fields");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        #region Properties");
                oSbStateMachineSourceCode.AppendLine(@"        public string SystemState { get { return m_strSystemState; } }");
                oSbStateMachineSourceCode.AppendLine(@"        public bool AbortingScan { get; private set; }");
                oSbStateMachineSourceCode.AppendLine(@"        public bool ScanInProgress { get; private set; }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        private bool SimulationModeEnabled");
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            get");
                oSbStateMachineSourceCode.AppendLine(@"            {");
                oSbStateMachineSourceCode.AppendLine(@"                return m_bSimulationMode;");
                oSbStateMachineSourceCode.AppendLine(@"            }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"            set");
                oSbStateMachineSourceCode.AppendLine(@"            {");
                oSbStateMachineSourceCode.AppendLine(@"                m_bSimulationMode = value;");
                oSbStateMachineSourceCode.AppendLine(@"            }");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        public bool InitAfterSettingsComplete");
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            get { return m_bInitAfterSettingsComplete; }");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        public bool InitComplete");
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            get { return m_bInitComplete; }");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        public string LastOperatorPrompt");
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            get { return m_strLastOperatorPrompt; }");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        #endregion Properties");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        #region Events");
                oSbStateMachineSourceCode.AppendLine(@"        public event StateChangeEventHandler eventStateChange;");
                oSbStateMachineSourceCode.AppendLine(@"        #endregion Events");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        #region Constructors");
                oSbStateMachineSourceCode.AppendLine(string.Format(@"        public C{0}StateMachine(string strName, I{0}System i{0})", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"            : base(strName, new NSFEventThread(strName))");
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            // Capture reference to the parent system for calling back to perform system functions");
                oSbStateMachineSourceCode.AppendLine(string.Format(@"            m_i{0} = i{0};", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"            // Init State Machine");
                oSbStateMachineSourceCode.AppendLine(@"            CreateStateMachine();");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"            // Init Operator Prompts");
                oSbStateMachineSourceCode.AppendLine(@"            CreateOperatorPrompts();");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(string.Format(@"        ~C{0}StateMachine()", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            ");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        #endregion Constructors");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        #region Methods");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        public bool StartStateMachine()");
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            // Support StateMachine specific log");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"            NSFTraceLog.PrimaryTraceLog.Enabled = true;");
                oSbStateMachineSourceCode.AppendLine(@"            StateChangeActions += handleStateChange;");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"            // Start State machine");
                oSbStateMachineSourceCode.AppendLine(@"            startStateMachine();");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"            return true;");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        public bool StopStateMachine()");
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            stopStateMachine();");
                oSbStateMachineSourceCode.AppendLine(@"            // Save trace log");
                oSbStateMachineSourceCode.AppendLine(string.Format(@"            NSFTraceLog.PrimaryTraceLog.saveLog(""{0}StateMachineLog.xml"");", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"            NSFEnvironment.terminate();");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"            return true;");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                //oSbStateMachineSourceCode.AppendLine(string.Format(@"        public bool HandleEvent({0}EventsEnum e{0}Event, object oEventData)", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(string.Format(@"        public bool HandleEvent({0}SystemEventsEnum e{0}Event, object oEventData)", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            bool bEventHandled = false;");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(string.Format(@"            if (!m_dictEventByEnum.ContainsKey(e{0}Event))", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"            {");
                oSbStateMachineSourceCode.AppendLine(@"                ms_iLogger.Log(ELogLevel.Error, ""Invalid Event passed to HandleEvent!"");");
                oSbStateMachineSourceCode.AppendLine(@"                return false;");
                oSbStateMachineSourceCode.AppendLine(@"            }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"            try");
                oSbStateMachineSourceCode.AppendLine(@"            {");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"                // Get event from enum passed in");
                oSbStateMachineSourceCode.AppendLine(string.Format(@"                NSFEvent oEvent = m_dictEventByEnum[e{0}Event];", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"                // Handle event data");
                oSbStateMachineSourceCode.AppendLine(@"                if (oEventData == null)");
                oSbStateMachineSourceCode.AppendLine(@"                {");
                oSbStateMachineSourceCode.AppendLine(@"                    // queue the event to the StateMachine");
                oSbStateMachineSourceCode.AppendLine(@"                    queueEvent(oEvent);");
                oSbStateMachineSourceCode.AppendLine(@"                    bEventHandled = true;");
                oSbStateMachineSourceCode.AppendLine(@"                }");
                oSbStateMachineSourceCode.AppendLine(@"                else");
                oSbStateMachineSourceCode.AppendLine(@"                {");
                oSbStateMachineSourceCode.AppendLine(@"                    // Queue the event and EventData to the state machine");
                oSbStateMachineSourceCode.AppendLine(@"                    CStateMachineEventData oStateMachineEventData = new CStateMachineEventData(oEventData);");
                oSbStateMachineSourceCode.AppendLine(@"                    queueEvent(oEvent, oStateMachineEventData);");
                oSbStateMachineSourceCode.AppendLine(@"                    bEventHandled = true;");
                oSbStateMachineSourceCode.AppendLine(@"                }");
                oSbStateMachineSourceCode.AppendLine(@"            ");
                oSbStateMachineSourceCode.AppendLine(@"                // Log Event");
                oSbStateMachineSourceCode.AppendLine(@"                if (bEventHandled)");
                oSbStateMachineSourceCode.AppendLine(@"                {");
                oSbStateMachineSourceCode.AppendLine(string.Format(@"                    string strMsg = string.Format(@"" Received Event {{0}}!"", e{0}Event.ToString());", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"                    ms_iLogger.Log(ELogLevel.Info, strMsg);");
                oSbStateMachineSourceCode.AppendLine(@"                }");
                oSbStateMachineSourceCode.AppendLine(@"                else");
                oSbStateMachineSourceCode.AppendLine(@"                {");
                oSbStateMachineSourceCode.AppendLine(@"                    ms_iLogger.Log(ELogLevel.Error, ""Invalid event type passed to HandleEvent!"");");
                oSbStateMachineSourceCode.AppendLine(@"                }");
                oSbStateMachineSourceCode.AppendLine(@"            ");
                oSbStateMachineSourceCode.AppendLine(@"                return true;");
                oSbStateMachineSourceCode.AppendLine(@"            }");
                oSbStateMachineSourceCode.AppendLine(@"            catch (Exception ex)");
                oSbStateMachineSourceCode.AppendLine(@"            {");
                oSbStateMachineSourceCode.AppendLine(string.Format(@"                string strMsg = String.Format(@""Error!  Unable to handle handle event {0}!"", e{0}Event.ToString());", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"                ms_iLogger.Log(ELogLevel.Error, strMsg);");
                oSbStateMachineSourceCode.AppendLine(@"            }");
                oSbStateMachineSourceCode.AppendLine(@"            ");
                oSbStateMachineSourceCode.AppendLine(@"            return false;");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        ");


                //oSbStateMachineSourceCode.AppendLine(@"        /// <summary>}");
                //oSbStateMachineSourceCode.AppendLine(@"        /// Called by the NSF whenever there is a state change}");
                //oSbStateMachineSourceCode.AppendLine(@"        /// </summary>}");
                //oSbStateMachineSourceCode.AppendLine(@"        /// <param name=""oContext""></param>}");
                //oSbStateMachineSourceCode.AppendLine(@"        private void handleStateChange(NSFStateMachineContext oContext)");
                //oSbStateMachineSourceCode.AppendLine(@"        {");
                //oSbStateMachineSourceCode.AppendLine(@"            // Log state change event");
                //oSbStateMachineSourceCode.AppendLine(string.Format(@"            E{0}State estateNew;", m_strStateMachineName));
                //oSbStateMachineSourceCode.AppendLine(@"            ");
                //oSbStateMachineSourceCode.AppendLine(@"            if (oContext.EnteringState is NSFChoiceState) //(strNewState.StartsWith(""Is""))");
                //oSbStateMachineSourceCode.AppendLine(@"            {");
                //oSbStateMachineSourceCode.AppendLine(@"                string strMsg = string.Format(@"" Evaluating {0} check!"", oContext.EnteringState.Name);");
                //oSbStateMachineSourceCode.AppendLine(@"                ms_iLogger.Log(ELogLevel.Info, strMsg);");
                //oSbStateMachineSourceCode.AppendLine(@"                return;");
                //oSbStateMachineSourceCode.AppendLine(@"            }");
                //oSbStateMachineSourceCode.AppendLine(@"            else");
                //oSbStateMachineSourceCode.AppendLine(@"            {");
                //oSbStateMachineSourceCode.AppendLine(@"                string strMsg = string.Format("" Changing from the {0} state to the {1} state!"", m_strSystemState, oContext.EnteringState.Name);");
                //oSbStateMachineSourceCode.AppendLine(@"                ms_iLogger.Log(ELogLevel.Info, strMsg);");
                //oSbStateMachineSourceCode.AppendLine(@"                // Capture new state");
                //oSbStateMachineSourceCode.AppendLine(@"                m_strSystemState = oContext.EnteringState.Name;");
                //oSbStateMachineSourceCode.AppendLine(@"                m_estatePrevious = m_estateCurrent;");
                //oSbStateMachineSourceCode.AppendLine(string.Format(@"                m_estateCurrent = Enum.TryParse(oContext.EnteringState.Name, out estateNew) ? (E{0}State?) estateNew : null;", m_strStateMachineName));
                //oSbStateMachineSourceCode.AppendLine(@"            }");
                //oSbStateMachineSourceCode.AppendLine(@"                ");
                //oSbStateMachineSourceCode.AppendLine(string.Format(@"            if (m_estatePrevious != null && m_estatePrevious == E{0}State.Init)", m_strStateMachineName));
                //oSbStateMachineSourceCode.AppendLine(@"                m_bInitComplete = true;");
                //oSbStateMachineSourceCode.AppendLine(@"                ");
                //oSbStateMachineSourceCode.AppendLine(string.Format(@"            if (m_estatePrevious != null && m_estatePrevious == E{0}State.InitAfterSettings)", m_strStateMachineName));
                //oSbStateMachineSourceCode.AppendLine(@"                m_bInitAfterSettingsComplete = true;");
                //oSbStateMachineSourceCode.AppendLine(@"                ");
                //oSbStateMachineSourceCode.AppendLine(@"            ");
                //oSbStateMachineSourceCode.AppendLine(@"            m_autoreseteventStateChange.Set();");
                //oSbStateMachineSourceCode.AppendLine(@"            ");
                //oSbStateMachineSourceCode.AppendLine(@"            // Is there an operator prompt to display for this new state?");
                //oSbStateMachineSourceCode.AppendLine(@"            string strOperatorPrompt = """";");
                //oSbStateMachineSourceCode.AppendLine(@"            m_odictOperatorPromptForState.TryGetValue(oContext.EnteringState, out strOperatorPrompt);");
                //oSbStateMachineSourceCode.AppendLine(@"            ");
                //oSbStateMachineSourceCode.AppendLine(@"            m_strLastOperatorPrompt = strOperatorPrompt;");
                //oSbStateMachineSourceCode.AppendLine(@"            ");
                //oSbStateMachineSourceCode.AppendLine(@"            // Notify anyone subscribed to the state change event");
                //oSbStateMachineSourceCode.AppendLine(@"            if (eventStateChange != null)");
                //oSbStateMachineSourceCode.AppendLine(@"            {");
                //oSbStateMachineSourceCode.AppendLine(@"                eventStateChange.Invoke(m_estatePrevious, m_estateCurrent, strOperatorPrompt);");
                //oSbStateMachineSourceCode.AppendLine(@"            }");
                //oSbStateMachineSourceCode.AppendLine(@"        }");
                //oSbStateMachineSourceCode.AppendLine(@"            ");


                oSbStateMachineSourceCode.AppendLine(@"        #region Auto-Generated State Machine Methods");
                oSbStateMachineSourceCode.AppendLine("  // DO NOT ADD ANY METHODS INSIDE THIS AUTO-GENERATED REGION (Below this line). TREAT IT AS A KEEP_OUT AREA AS USER ADDED CODE WILL BE ELIMINATED. Thanks!");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        private bool CreateStateMachine()");
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            try");
                oSbStateMachineSourceCode.AppendLine(@"            {");
                oSbStateMachineSourceCode.AppendLine(@"                // State Machine Components");
                oSbStateMachineSourceCode.AppendLine(@"                // Define and initialize in the order:");
                oSbStateMachineSourceCode.AppendLine(@"                //   1) Events");
                oSbStateMachineSourceCode.AppendLine(@"                //   2) Regions and states, from outer to inner");
                oSbStateMachineSourceCode.AppendLine(@"                //   3) Transitions, ordered internal, local, external");
                oSbStateMachineSourceCode.AppendLine(@"                //   4) Group states and transitions within a region together.");
                oSbStateMachineSourceCode.AppendLine(@" ");
                oSbStateMachineSourceCode.AppendLine(@"                // Instantiate All State Machine Events.");
                oSbStateMachineSourceCode.AppendLine(@" ");
                oSbStateMachineSourceCode.AppendLine(@" ");





                oSbStateMachineOperatorPrompts.AppendLine(@"        /// <summary>");
                oSbStateMachineOperatorPrompts.AppendLine(@"        /// Create the Operator Prompts for the State Machine.");
                oSbStateMachineOperatorPrompts.AppendLine(@"        /// Operator Prompt is Displayed when the State is Entered.");
                oSbStateMachineOperatorPrompts.AppendLine(@"        /// </summary>");
                oSbStateMachineOperatorPrompts.AppendLine(@"        private void CreateOperatorPrompts()");
                oSbStateMachineOperatorPrompts.AppendLine(@"        {");
                oSbStateMachineOperatorPrompts.AppendLine(@"            m_odictOperatorPromptForState = new Dictionary<NSFState, string>();");

                for (nIndex = 0; nIndex < OperatorPromptStates.Count(); nIndex++)
                {
                    oSbStateMachineOperatorPrompts.AppendLine(string.Format(@"            m_odictOperatorPromptForState.Add(o{0}, @""{1}"");", OperatorPromptStates[nIndex], OperatorPromptText[nIndex]));
                }

                oSbStateMachineOperatorPrompts.AppendLine(@"        }");
                oSbStateMachineOperatorPrompts.AppendLine(@" ");

                oSbStateMachineOperatorPrompts.AppendLine(@"        /// <summary>}");
                oSbStateMachineOperatorPrompts.AppendLine(@"        /// Called by the NSF whenever there is a state change}");
                oSbStateMachineOperatorPrompts.AppendLine(@"        /// </summary>}");
                oSbStateMachineOperatorPrompts.AppendLine(@"        /// <param name=""oContext""></param>}");
                oSbStateMachineOperatorPrompts.AppendLine(@"        private void handleStateChange(NSFStateMachineContext oContext)");
                oSbStateMachineOperatorPrompts.AppendLine(@"        {");
                oSbStateMachineOperatorPrompts.AppendLine(@"            // Log state change event");
                //oSbStateMachineOperatorPrompts.AppendLine(string.Format(@"            E{0}State estateNew;", m_strStateMachineName));
                oSbStateMachineOperatorPrompts.AppendLine(string.Format(@"            EState estateNew;", m_strStateMachineName));
                oSbStateMachineOperatorPrompts.AppendLine(@"            ");
                oSbStateMachineOperatorPrompts.AppendLine(@"            if (oContext.EnteringState is NSFChoiceState) //(strNewState.StartsWith(""Is""))");
                oSbStateMachineOperatorPrompts.AppendLine(@"            {");
                oSbStateMachineOperatorPrompts.AppendLine(@"                string strMsg = string.Format(@"" Evaluating {0} check!"", oContext.EnteringState.Name);");
                oSbStateMachineOperatorPrompts.AppendLine(@"                ms_iLogger.Log(ELogLevel.Info, strMsg);");
                oSbStateMachineOperatorPrompts.AppendLine(@"                return;");
                oSbStateMachineOperatorPrompts.AppendLine(@"            }");
                oSbStateMachineOperatorPrompts.AppendLine(@"            else");
                oSbStateMachineOperatorPrompts.AppendLine(@"            {");
                oSbStateMachineOperatorPrompts.AppendLine(@"                string strMsg = string.Format("" Changing from the {0} state to the {1} state!"", m_strSystemState, oContext.EnteringState.Name);");
                oSbStateMachineOperatorPrompts.AppendLine(@"                ms_iLogger.Log(ELogLevel.Info, strMsg);");
                oSbStateMachineOperatorPrompts.AppendLine(@"                // Capture new state");
                oSbStateMachineOperatorPrompts.AppendLine(@"                m_strSystemState = oContext.EnteringState.Name;");
                oSbStateMachineOperatorPrompts.AppendLine(@"                m_estatePrevious = m_estateCurrent;");
                //oSbStateMachineOperatorPrompts.AppendLine(string.Format(@"                m_estateCurrent = Enum.TryParse(oContext.EnteringState.Name, out estateNew) ? (E{0}State?) estateNew : null;", m_strStateMachineName));
                oSbStateMachineOperatorPrompts.AppendLine(string.Format(@"                m_estateCurrent = Enum.TryParse(oContext.EnteringState.Name, out estateNew) ? (EState?) estateNew : null;", m_strStateMachineName));
                oSbStateMachineOperatorPrompts.AppendLine(@"            }");
                oSbStateMachineOperatorPrompts.AppendLine(@"                ");
                //oSbStateMachineOperatorPrompts.AppendLine(string.Format(@"            if (m_estatePrevious != null && m_estatePrevious == E{0}State.Init)", m_strStateMachineName));
                oSbStateMachineOperatorPrompts.AppendLine(string.Format(@"            if (m_estatePrevious != null && m_estatePrevious == EState.Init)", m_strStateMachineName));
                oSbStateMachineOperatorPrompts.AppendLine(@"                m_bInitComplete = true;");
                oSbStateMachineOperatorPrompts.AppendLine(@"                ");
                //oSbStateMachineOperatorPrompts.AppendLine(string.Format(@"            if (m_estatePrevious != null && m_estatePrevious == E{0}State.InitAfterSettings)", m_strStateMachineName));
                oSbStateMachineOperatorPrompts.AppendLine(string.Format(@"            if (m_estatePrevious != null && m_estatePrevious == EState.InitAfterSettings)", m_strStateMachineName));
                oSbStateMachineOperatorPrompts.AppendLine(@"                m_bInitAfterSettingsComplete = true;");
                oSbStateMachineOperatorPrompts.AppendLine(@"                ");
                oSbStateMachineOperatorPrompts.AppendLine(@"            ");
                oSbStateMachineOperatorPrompts.AppendLine(@"            m_autoreseteventStateChange.Set();");
                oSbStateMachineOperatorPrompts.AppendLine(@"            ");
                oSbStateMachineOperatorPrompts.AppendLine(@"            // Is there an operator prompt to display for this new state?");
                oSbStateMachineOperatorPrompts.AppendLine(@"            string strOperatorPrompt = """";");
                oSbStateMachineOperatorPrompts.AppendLine(@"            m_odictOperatorPromptForState.TryGetValue(oContext.EnteringState, out strOperatorPrompt);");
                oSbStateMachineOperatorPrompts.AppendLine(@"            ");
                oSbStateMachineOperatorPrompts.AppendLine(@"            m_strLastOperatorPrompt = strOperatorPrompt;");
                oSbStateMachineOperatorPrompts.AppendLine(@"            ");
                oSbStateMachineOperatorPrompts.AppendLine(@"            // Notify anyone subscribed to the state change event");
                oSbStateMachineOperatorPrompts.AppendLine(@"            if (eventStateChange != null)");
                oSbStateMachineOperatorPrompts.AppendLine(@"            {");
                oSbStateMachineOperatorPrompts.AppendLine(@"                eventStateChange.Invoke(m_estatePrevious, m_estateCurrent, strOperatorPrompt);");
                oSbStateMachineOperatorPrompts.AppendLine(@"            }");
                oSbStateMachineOperatorPrompts.AppendLine(@"        }");
                oSbStateMachineOperatorPrompts.AppendLine(@"            ");

                // Combine StringBuilders
                oSbStateMachineSourceCode.AppendFormat("{0}{0}", Environment.NewLine);
                oSbStateMachineSourceCode.Append(oSbEventInstantiations); // Add Event Instantiations


                oSbStateMachineSourceCode.AppendFormat("{0}{0}", Environment.NewLine);
                oSbStateMachineSourceCode.Append(oSbStateMachineDictEventByEnum); // Add the Enum dictionary entries

                oSbStateMachineSourceCode.AppendFormat(@"{0}{0}", Environment.NewLine);
                oSbStateMachineSourceCode.Append(oSbStateInstantiations);

                oSbStateMachineSourceCode.AppendFormat(@"{0}", Environment.NewLine);
                oSbStateMachineSourceCode.AppendLine(@"             // ******************************************************");
                oSbStateMachineSourceCode.AppendLine(@"             // End of creating the Transitions for the State Machine");
                oSbStateMachineSourceCode.AppendLine(@"             // ******************************************************");
                oSbStateMachineSourceCode.AppendLine(@"                 return true;");
                oSbStateMachineSourceCode.AppendLine(@"            }");
                oSbStateMachineSourceCode.AppendLine(@"            catch (Exception ex)");
                oSbStateMachineSourceCode.AppendLine(@"            {");
                oSbStateMachineSourceCode.AppendLine(string.Format(@"                ms_iLogger.LogException(ELogLevel.Error, ""Unable to initiate the {0} State Machine!"", ex);", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"            }");
                oSbStateMachineSourceCode.AppendLine(@"            ");
                oSbStateMachineSourceCode.AppendLine(@"            return false;");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine();
                //oSbStateMachineSourceCode.AppendLine();
                //oSbStateMachineSourceCode.AppendLine();




                oSbStateMachineSourceCode.AppendFormat(@"{0}{0}", Environment.NewLine);
                oSbStateMachineSourceCode.Append(oSbStateMachineOperatorPrompts);

                oSbStateMachineSourceCode.AppendFormat(@"{0}{0}", Environment.NewLine);
                oSbStateMachineSourceCode.Append(oSbStateMachineTransitionGuards);

                oSbStateMachineSourceCode.AppendFormat(@"{0}{0}", Environment.NewLine);
                oSbStateMachineSourceCode.Append(oSbStateMachineTransitionActions);

                oSbStateMachineSourceCode.AppendFormat(@"{0}{0}", Environment.NewLine);
                oSbStateMachineSourceCode.Append(oSbEntryAndExitActions);

                oSbStateMachineSourceCode.AppendFormat(@"{0}{0}", Environment.NewLine);

                oSbStateMachineSourceCode.AppendLine("  // DO NOT ADD ANY METHODS INSIDE THIS AUTO-GENERATED REGION (Above this line). TREAT IT AS A KEEP_OUT AREA AS USER ADDED CODE WILL BE ELIMINATED. Thanks!");
                oSbStateMachineSourceCode.AppendLine(@"        #endregion Auto-Generated State Machine Methods");
                oSbStateMachineSourceCode.AppendFormat(@"{0}", Environment.NewLine);

                oSbStateMachineSourceCode.AppendLine(@"        /// <summary>");
                oSbStateMachineSourceCode.AppendLine(@"        /// Waits for the specified state.");
                oSbStateMachineSourceCode.AppendLine(@"        /// </summary>");
                oSbStateMachineSourceCode.AppendLine(@"        /// <param name=""eState"">The State to wait for.</param>");
                oSbStateMachineSourceCode.AppendLine(@"        /// <param name=""bIsNewState"">.</param>");
                oSbStateMachineSourceCode.AppendLine(@"        /// <returns>Nothing</returns>");
                //oSbStateMachineSourceCode.AppendLine(string.Format(@"        private void WaitForState(E{0}State eState, bool bIsNewState)", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(string.Format(@"        private void WaitForState(EState eState, bool bIsNewState)", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            while ((bIsNewState && m_estateCurrent != eState) || (!bIsNewState && m_estatePrevious != eState))");
                oSbStateMachineSourceCode.AppendLine(@"            {");
                oSbStateMachineSourceCode.AppendLine(@"                m_autoreseteventStateChange.WaitOne();");
                oSbStateMachineSourceCode.AppendLine(@"                //m_semaphoreStateChange.WaitOne();");
                oSbStateMachineSourceCode.AppendLine(@"            }");
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        /// <summary>");
                oSbStateMachineSourceCode.AppendLine(@"        /// Waits for Init Complete.");
                oSbStateMachineSourceCode.AppendLine(@"        /// </summary>");
                oSbStateMachineSourceCode.AppendLine(@"        /// <returns>Nothing</returns>");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        public void WaitForInitComplete()");
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            lock (m_objLock)");
                oSbStateMachineSourceCode.AppendLine(@"            {");
                oSbStateMachineSourceCode.AppendLine(@"                if (m_bInitComplete)");
                oSbStateMachineSourceCode.AppendLine(@"                    return;");
                oSbStateMachineSourceCode.AppendLine(@"            }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                //oSbStateMachineSourceCode.AppendLine(string.Format(@"            WaitForState(E{0}State.Init, false);", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(string.Format(@"            WaitForState(EState.Init, false);", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine();
                oSbStateMachineSourceCode.AppendLine(@"        /// <summary>");
                oSbStateMachineSourceCode.AppendLine(@"        /// Waits for Init After Settings Complete.");
                oSbStateMachineSourceCode.AppendLine(@"        /// </summary>");
                oSbStateMachineSourceCode.AppendLine(@"        /// <returns>Nothing</returns>");
                oSbStateMachineSourceCode.AppendLine(@"        public void WaitForInitAfterSettingsComplete()");
                oSbStateMachineSourceCode.AppendLine(@"        {");
                oSbStateMachineSourceCode.AppendLine(@"            lock (m_objLock)");
                oSbStateMachineSourceCode.AppendLine(@"            {");
                oSbStateMachineSourceCode.AppendLine(@"                if (m_bInitAfterSettingsComplete)");
                oSbStateMachineSourceCode.AppendLine(@"                    return;");
                oSbStateMachineSourceCode.AppendLine(@"            }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                //oSbStateMachineSourceCode.AppendLine(string.Format(@"            WaitForState(E{0}State.InitAfterSettings, false);", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(string.Format(@"            WaitForState(EState.InitAfterSettings, false);", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(@"        }");
                oSbStateMachineSourceCode.AppendLine(@"        ");
                oSbStateMachineSourceCode.AppendLine(@"        #endregion Methods"); // Right Place?

                oSbStateMachineSourceCode.AppendLine(@"        #region InnerClasses");
                oSbStateMachineSourceCode.AppendLine(@"        #endregion InnerClasses");

                oSbStateMachineSourceCode.AppendLine(string.Format(@"    }}  //end C{0}StateMachine", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine(string.Format(@"}}//end namespace {0}System", m_strStateMachineName));
                oSbStateMachineSourceCode.AppendLine();
                oSbStateMachineSourceCode.AppendLine();




                //oSbStateMachineOwnerMethods.Append(oSbStateMachineOwnerTransitionGuardProperties);  // Removed 12/26/2016
                oSbStateMachineOwnerMethods.AppendFormat(@"{0}{0}", Environment.NewLine);

                oSbStateMachineOwnerMethods.Append(oSbStateMachineOwnerTransitionActions);
                oSbStateMachineOwnerMethods.AppendFormat(@"{0}{0}", Environment.NewLine);

                // Write the State Machine Source Code File
                File.WriteAllText(m_strStateMachineSourceFilePath, oSbStateMachineSourceCode.ToString());
                File.WriteAllText(m_strStateMachineInterfaceFilePath, oSbStateMachineInterface.ToString());

                // Write the State Machine Interface File
                //File.WriteAllText(m_strStateMachineInterfaceFilePath, 



                // Now Start updating the Owner's Source Code File
                // This occurs in three parts:
                // Fields, Properties, and Methods


                // Update the State Machine Owner's Source Code File
                // Two regions are added or updated: Fields, Properties and Methods
                nOwnerInterfaceFileEndingLineNumber = m_iOwnerInterfaceFileUpdater.GetLineNumberOfLastLineOfTextInFile();

                bEnterFields = false;
                bEnterProperties = false;
                bEnterMethods = false;
                bFieldsEntered = false;
                bPropertiesEntered = false;
                bMethodsEntered = false;

                // Update the State Machine Owner's Source Code File
                // Three regions are added or updated: Fields, Properties and Methods
                nOwnerSourceCodeFileEndingLineNumber = m_iOwnerSourceCodeFileUpdater.GetLineNumberOfLastLineOfTextInFile();

                bEnterFields = false;
                bEnterProperties = false;
                bEnterMethods = false;
                bFieldsEntered = false;
                bPropertiesEntered = false;
                bMethodsEntered = false;
                bEndOfFileEntered = false;

                while ((bFieldsEntered == false) || (bPropertiesEntered == false) || (bMethodsEntered == false) || (bEndOfFileEntered == false) )
                {
                    // Determine how Owner Source Code Fields, Properties or Method are located with respect to each other
                    if (nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber < nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber)
                    {
                        if (nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber < nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber) // Check to see if Fields are also before Methods
                        {
                            if (bFieldsEntered == false) // Fields are First
                            {
                                bEnterFields = true;
                                m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(0, nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                            }
                            else if (nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber < nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber) // Check to see Fields are followed by either Properties or Methods
                            {
                                if (bPropertiesEntered == false) // Fields -> Properties -> Methods
                                {
                                    bEnterProperties = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineFieldRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else if (bMethodsEntered == false)
                                {
                                    bEnterMethods = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachinePropertyRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else
                                { 
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineMethodRegionEndingLineNumber + 1, nOwnerSourceCodeFileEndingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                    bEndOfFileEntered = true;
                                }
                            }
                            else                                //  Fields -> Methods -> Properties
                            {
                                if (bMethodsEntered == false)
                                {
                                    bEnterMethods = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineFieldRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else if (bPropertiesEntered == false)
                                {
                                    bEnterProperties = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineMethodRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else
                                { 
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachinePropertyRegionEndingLineNumber + 1, nOwnerSourceCodeFileEndingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                    bEndOfFileEntered = true;
                                }
                            }
                        }
                        else //  Methods are First
                        {
                            if (bMethodsEntered == false)
                            {
                                bEnterMethods = true;
                                m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(0, nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                            }
                            else if (nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber < nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber)
                            {
                                if (bFieldsEntered == false) // Methods -> Fields -> Properties
                                {
                                    bEnterFields = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineMethodRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else if (bPropertiesEntered == false)
                                {
                                    bEnterProperties = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineFieldRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else
                                { 
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachinePropertyRegionEndingLineNumber + 1, nOwnerSourceCodeFileEndingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                    bEndOfFileEntered = true;
                                }
                            }
                            else   // Methods -> Properties-> Fields 
                            {
                                if (bPropertiesEntered == false)
                                {
                                    bEnterProperties = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineMethodRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else if (bFieldsEntered == false)
                                {
                                    bEnterFields = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachinePropertyRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else
                                { 
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineFieldRegionEndingLineNumber + 1, nOwnerSourceCodeFileEndingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                    bEndOfFileEntered = true;
                                }
                            }
                        }
                    }
                    else  // Properties come before Fields
                    {
                        if (nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber < nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber) // Check to see if Properties are also before Methods
                        {
                            if (bPropertiesEntered == false)  // Properties Come first
                            {
                                bEnterProperties = true;
                                m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(0, nOwnerSourceCodeFileStateMachinePropertyRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                            }
                            else if (nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber < nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber) // Check to see if Fields are also before Methods
                            {
                                if (bFieldsEntered == false) // Properties -> Fields -> Methods
                                {
                                    bEnterFields = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachinePropertyRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else if (bMethodsEntered == false)
                                {
                                    bEnterMethods = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineFieldRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else
                                { 
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineMethodRegionEndingLineNumber + 1, nOwnerSourceCodeFileEndingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                    bEndOfFileEntered = true;
                                }
                            }
                            else                             // Properties -> Methods -> Fields
                            {
                                if (bMethodsEntered == false)
                                {
                                    bEnterMethods = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachinePropertyRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachineMethodRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else if (bFieldsEntered == false)
                                {
                                    bEnterFields = true;
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineMethodRegionEndingLineNumber + 1, nOwnerSourceCodeFileStateMachineFieldRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                }
                                else
                                { 
                                    m_iOwnerSourceCodeFileUpdater.GetLinesOfTextFromFile(nOwnerSourceCodeFileStateMachineFieldRegionEndingLineNumber + 1, nOwnerSourceCodeFileEndingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                                    sbOwnerSourceCode.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                                    bEndOfFileEntered = true;
                                }
                            }
                        }
                    }

                    sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);


                    if (bEnterFields == true)
                    {
                        bEnterFields = false;
                        bFieldsEntered = true;

                        // Add in State Machine Owner Fields
                        sbOwnerSourceCode.AppendLine("  // DO NOT ADD ANY FIELDS INSIDE THIS REGION. TREAT IT AS A KEEP_OUT AREA. Thanks!");
                        sbOwnerSourceCode.Append(oSbStateMachineOwnerFields);
                        sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);
                        sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);
                        sbOwnerSourceCode.AppendLine("  // DO NOT ADD ANY FIELDS INSIDE THIS REGION. TREAT IT AS A KEEP_OUT AREA. Thanks!");

                        sbOwnerSourceCode.AppendFormat("#endregion StateMachine{0}Fields", m_strStateMachineName);
                        sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);
                        // End of Adding in State Machine Owner Fields
                    }

                    if (bEnterProperties == true)
                    {
                        bEnterProperties = false;
                        bPropertiesEntered = true;

                        // Add in State Machine Owner Properties
                        sbOwnerSourceCode.AppendLine("  // DO NOT ADD ANY PROPERTIES INSIDE THIS REGION. TREAT IT AS A KEEP_OUT AREA. Thanks!");
                        sbOwnerSourceCode.Append(oSbStateMachineOwnerProperties);
                        sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);

                        sbOwnerSourceCode.Append(oSbStateMachineOwnerTransitionGuardProperties); // Add in the Transition Guard Properties
                        sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);

                        sbOwnerSourceCode.AppendLine("  // DO NOT ADD ANY PROPERTIES INSIDE THIS REGION. TREAT IT AS A KEEP_OUT AREA. Thanks!");

                        sbOwnerSourceCode.AppendFormat("#endregion StateMachine{0}Properties", m_strStateMachineName);
                        sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);
                        // End of Adding in State Machine Owner Properties
                    }

                    if (bEnterMethods == true)
                    {
                        bEnterMethods = false;
                        bMethodsEntered = true;

                        sbOwnerSourceCode.AppendLine("  // DO NOT ADD ANY METHODS INSIDE THIS REGION. TREAT IT AS A KEEP_OUT AREA. Thanks!");
                        sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);
                        sbOwnerSourceCode.Append(oSbStateMachineOwnerMethods); // Add in the Owner's Methods
                        sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);
                        sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);
                        sbOwnerSourceCode.AppendLine("  // DO NOT ADD ANY METHODS INSIDE THIS REGION. TREAT IT AS A KEEP_OUT AREA. Thanks!");
                        sbOwnerSourceCode.AppendFormat("#endregion StateMachine{0}Methods", m_strStateMachineName);
                        sbOwnerSourceCode.AppendFormat("{0}", Environment.NewLine);
                    }
                }

                File.WriteAllText(StateMachineOwnerSourceFilePath, sbOwnerSourceCode.ToString());


                // Update the State Machine Owner's Interface File
                // Two regions are added or updated: Properties and Methods

                nOwnerInterfaceFileEndingLineNumber = m_iOwnerInterfaceFileUpdater.GetLineNumberOfLastLineOfTextInFile(); // Get the line number of the end of the Owner's Interface file

                bEnterProperties = false;
                bEnterMethods = false;
                bPropertiesEntered = false;
                bMethodsEntered = false;
                bEndOfFileEntered = false;

                // Make sure Both Methods and Properties are Generated and the End of the original file is written
                while ( (bPropertiesEntered == false) || (bMethodsEntered == false) || (bEndOfFileEntered == false) )
                {
                    // Figure out if Properties or Method need to be entered first based on where their regions have been placed.
                    if (nOwnerInterfaceFileStateMachinePropertyRegionStartingLineNumber < nOwnerInterfaceFileStateMachineMethodRegionStartingLineNumber) // Then Properties come first
                    {
                        if (bPropertiesEntered == false)
                        {
                            bEnterProperties = true;
                            m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(0, nOwnerInterfaceFileStateMachinePropertyRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                            sbOwnerInterface.Append(sbTempSourceCode); // Add the Owner's Interface text to the StringBuilder of the new File being created.
                        }
                        else if (bMethodsEntered == false)
                        {
                            bEnterMethods = true;
                            m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(nOwnerInterfaceFileStateMachinePropertyRegionEndingLineNumber + 1, nOwnerInterfaceFileStateMachineMethodRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                            sbOwnerInterface.Append(sbTempSourceCode); // Add the Owner's Interface text to the StringBuilder of the new File being created.
                        }
                        else
                        {
                            bEndOfFileEntered = true;
                            m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(nOwnerInterfaceFileStateMachineMethodRegionEndingLineNumber + 1, nOwnerInterfaceFileEndingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                            sbOwnerInterface.Append(sbTempSourceCode); // Add the Owner's Interface text to the StringBuilder of the new File being created.
                        }
                    }
                    else  // Methods -> Properties
                    {
                        if (bMethodsEntered == false)
                        {
                            bEnterMethods = true;
                            m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(0, nOwnerInterfaceFileStateMachineMethodRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                            sbOwnerInterface.Append(sbTempSourceCode); // Add the Owner's Interface text to the StringBuilder of the new File being created.
                        }
                        else if (bPropertiesEntered == false)
                        {
                            bEnterProperties = true;
                            m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(nOwnerInterfaceFileStateMachineMethodRegionEndingLineNumber + 1, nOwnerInterfaceFileStateMachinePropertyRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                            sbOwnerInterface.Append(sbTempSourceCode); // Add the Owner's Interface text to the StringBuilder of the new File being created.
                        }
                        else
                        {
                            bEndOfFileEntered = true;
                            m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(nOwnerInterfaceFileStateMachinePropertyRegionEndingLineNumber + 1, nOwnerInterfaceFileEndingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                            sbOwnerInterface.Append(sbTempSourceCode); // Add the Owner's Interface text to the StringBuilder of the new File being created.
                        }
                    }

                    if (bEnterProperties == true)
                    {
                        bEnterProperties = false;

                        // Add in State Machine Owner Properties

                        // Get the Owner's Interface text immediately before where the Properties are to be inserted.
                        // Determine how Properties or Method are located with respect to each other
                        //if (nOwnerInterfaceFileStateMachinePropertyRegionStartingLineNumber < nOwnerInterfaceFileStateMachineMethodRegionStartingLineNumber)
                        //{
                        //    m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(0, nOwnerInterfaceFileStateMachinePropertyRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                        //}
                        //else
                        //{
                        //    m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(nOwnerInterfaceFileStateMachineMethodRegionEndingLineNumber + 1, nOwnerInterfaceFileStateMachinePropertyRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                        //}

                        //sbOwnerInterface.Append(sbTempSourceCode); // Add the Owner's Interface text to the StringBuilder of the new File being created.
                        sbOwnerInterface.AppendLine("  // DO NOT ADD ANY PROPERTIES INSIDE THIS REGION. TREAT IT AS A KEEP_OUT AREA. Thanks!");
                        sbOwnerInterface.Append(oSbStateMachineOwnerInterfaceProperties);  // Add in the Newly Generated Interface Properties
                        sbOwnerInterface.AppendFormat("{0}", Environment.NewLine);

                        sbOwnerInterface.Append(oSbStateMachineOwnerInterfaceTransitionGuardProperties); // Add in the Transition Guard Properties
                        sbOwnerInterface.AppendFormat("{0}", Environment.NewLine);

                        sbOwnerInterface.AppendLine("  // DO NOT ADD ANY PROPERTIES INSIDE THIS REGION. TREAT IT AS A KEEP_OUT AREA. Thanks!");

                        sbOwnerInterface.AppendFormat("#endregion StateMachine{0}Properties", m_strStateMachineName);
                        sbOwnerInterface.AppendFormat("{0}", Environment.NewLine);
                        // End of Adding in State Machine Owner Properties

                        bPropertiesEntered = true;
                    }

                    if (bEnterMethods == true)
                    {
                        bEnterMethods = false;

                        // Get the Owner's Interface text immediately before where the Methods are to be inserted.
                        // Determine how Properties or Method are located with respect to each other
                        //if (nOwnerInterfaceFileStateMachinePropertyRegionStartingLineNumber < nOwnerInterfaceFileStateMachineMethodRegionStartingLineNumber)
                        //{
                        //    m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(nOwnerInterfaceFileStateMachinePropertyRegionEndingLineNumber + 1, nOwnerInterfaceFileStateMachineMethodRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                        //}
                        //else // Methods come before Properties
                        //{
                        //    m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(0, nOwnerInterfaceFileStateMachineMethodRegionStartingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                        //}

                        //sbOwnerInterface.Append(sbTempSourceCode); // Add in the Owner's Existing code
                        sbOwnerInterface.AppendLine("  // DO NOT ADD ANY METHODS INSIDE THIS REGION. TREAT IT AS A KEEP_OUT AREA. Thanks!");
                        sbOwnerInterface.Append(oSbStateMachineOwnerInterfaceMethods); // Add in the Owner's Interface Methods
                        sbOwnerInterface.AppendFormat("{0}", Environment.NewLine);
                        sbOwnerInterface.AppendFormat("{0}", Environment.NewLine);
                        sbOwnerInterface.AppendLine("  // DO NOT ADD ANY METHODS INSIDE THIS REGION. TREAT IT AS A KEEP_OUT AREA. Thanks!");
                        sbOwnerInterface.AppendFormat("#endregion StateMachine{0}Methods", m_strStateMachineName);
                        sbOwnerInterface.AppendFormat("{0}", Environment.NewLine);


                        bMethodsEntered = true;
                    }

                }  // End of While for Entering Owner Interface Properties and Methods


                // Get the Owner's Interface text immediately after where the Methods or Properties were to be inserted until the end of the file.
                // Determine how Properties or Method are located with respect to each other
                //if (nOwnerInterfaceFileStateMachinePropertyRegionStartingLineNumber < nOwnerInterfaceFileStateMachineMethodRegionStartingLineNumber)
                //{
                //    m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(nOwnerInterfaceFileStateMachineMethodRegionEndingLineNumber + 1, nOwnerInterfaceFileEndingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                //}
                //else
                //{
                //    m_iOwnerInterfaceFileUpdater.GetLinesOfTextFromFile(nOwnerInterfaceFileStateMachinePropertyRegionEndingLineNumber + 1, nOwnerInterfaceFileEndingLineNumber, out sbTempSourceCode, "TREAT IT AS A KEEP_OUT AREA.");
                //}

                //sbOwnerInterface.Append(sbTempSourceCode);  // Add in the Owner's Existing code
                sbOwnerInterface.AppendFormat("{0}", Environment.NewLine);
                File.WriteAllText(StateMachineOwnerInterfaceFilePath, sbOwnerInterface.ToString()); // Write the File



            }
            catch (Exception Ex)
            {

            }

            return false; // Stop the thread.
        }

        // This is called when the thread stops, send an AbortEvent to the state machine.
        private void ApplicationTaskThreadExitAction()
        {
            m_oStateMachineCodeGeneratorStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.AbortEvent, null);
        }


        /// <summary>
        /// Adds a component, which realizes the IInitialization interface (in Corning.GenSys.Initialization), to a list of components requiring initialization.
        /// Adding to this list ensures that a component's Init() and InitAfterSettings() is called.
        /// </summary>
        /// <param name="iComponentRealizingIInitialization"></param>
        /// <returns>Returns Nothing</returns>
        public void AddComponentToInitializationList(IInitialization iComponentRealizingIInitialization)
        {
            m_lstiInitialization.Add(iComponentRealizingIInitialization);
        }


        ///  <summary>
        /// Starts the process of shutting down the system. Usually called by the GUI but it could be wired into a Point.
        /// </summary>
        /// <returns>Returns Nothing</returns>
        public void SystemClosing()
        {
            m_oSystemShutdownTask = new CThreadHandler("System Closing Task", ShutdownSystem, 100); // Delay interval isn't critical.
            m_oSystemShutdownTask.Start();
        }


        ///  <summary>
        /// Thread used to gracefully Shutdown the System.
        /// RaiseOnSystemShutdown(); is called after the system has finished shutting down.
        /// </summary>
        /// <returns>Return True if this routine should be called again, otherwise return false.</returns>
        public bool ShutdownSystem()
        {
            ms_iLogger.Log(ELogLevel.Info, string.Format("Executing System Shutdown sequence."));


            // Stopping the other components should be added here. 
            // Note that a sequenced shutdown can be built by having this routine return true, which will cause it to be called again after a short delay.



            m_oStateMachineCodeGeneratorStateMachine.StopStateMachine();

            // Serialize the system settings
            try
            {
                ms_iLogger.Log(ELogLevel.Info, "Serializing system Settings.");
                this.SerializeSettings();
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error Serializing Settings!", ex);
            }


            // Don't place any shutdown code past this comment.
            ms_iLogger.Log(ELogLevel.Info, "System Shutdown finished");
            m_bIsSystemShutdown = true; // This must come before RaiseOnSystemShutdown();
            RaiseOnSystemShutdown();    // Indicate the System has Shutdown. OK to Close GUI(s) if there are any.
            return false;               // System Shutdown has completed so stop the Shutdown thread.
        }


        //public void SetStateMachineCodeGeneratorGui(IStateMachineCodeGeneratorGui oGui)
        //{
        //    m_iStateMachineCodeGeneratorGui = oGui;
        //    m_iStateMachineCodeGeneratorGui.Init(m_iStateMachineCodeGeneratorMotion, m_iStateMachineCodeGeneratorIo);
        //    m_iStateMachineCodeGeneratorGui.OnDataModifiedByOperator += m_iStateMachineCodeGeneratorGui_OnDataModifiedByOperator;
        //}

        //void m_iStateMachineCodeGeneratorGui_OnDataModifiedByOperator(object obj)
        //{
        //    m_bDataModifiedByOperator = true;
        //}
        public void CreateSystemPoints()
        {
            // Create points for Alarms
            m_iPointSampleAlarm = new CPoint<double, double>(m_strSampleAlarmName, @"Point used soley to create a SampleAlarmFault.",
                      0.0, null, null, // Value, Min, Max
                      null, false, true, //Setpoint, ControlLoop, ControlLoopDisable
                      true, true, false, true, //Dynamic, Writeable, Hidden, Alarmable
                      @"", 1, 0, //Units, Slope, Offset
                      0.00001, false, 0.1, true, //ChangePercent, ChangePercentDisable, ChangeValue, ChangeValueDisable
                      false, 0.0, //FaultDisabled, FaultHysteresis,
                      0.5, 0.6, -5.0, -10.0, // HighFaultThreshold, HighHighFaultThreshold, LowFaultThreshold, LowLowFaultThreshold,
                      null, null, false); // aiPointAttributReadeDelegate,aiPointAttributeWriteDelegate,bBypassControlLoopIntegrityCheck = false);

            m_iPointScanAbortAlarm = new CPoint<double, double>(m_strScanAbortAlarmName, @"Point used soley to create a ScanAbortFault.",
                      0.0, null, null, // Value, Min, Max
                      null, false, true, //Setpoint, ControlLoop, ControlLoopDisable
                      true, true, false, true, //Dynamic, Writeable, Hidden, Alarmable
                      @"", 1, 0, //Units, Slope, Offset
                      0.00001, false, 0.1, true, //ChangePercent, ChangePercentDisable, ChangeValue, ChangeValueDisable
                      false, 0.0, //FaultDisabled, FaultHysteresis,
                      0.5, 0.6, -5.0, -10.0, // HighFaultThreshold, HighHighFaultThreshold, LowFaultThreshold, LowLowFaultThreshold,
                      null, null, false); // aiPointAttributReadeDelegate,aiPointAttributeWriteDelegate,bBypassControlLoopIntegrityCheck = false);

            m_iPointScanSensorFaultAlarm = new CPoint<double, double>(m_strScanSensorFaultAlarmName, @"Point used soley to create a ScanSensorFault.",
                      0.0, null, null, // Value, Min, Max
                      null, false, true, //Setpoint, ControlLoop, ControlLoopDisable
                      true, true, false, true, //Dynamic, Writeable, Hidden, Alarmable
                      @"", 1, 0, //Units, Slope, Offset
                      0.00001, false, 0.1, true, //ChangePercent, ChangePercentDisable, ChangeValue, ChangeValueDisable
                      false, 0.0, //FaultDisabled, FaultHysteresis,
                      0.5, 0.6, -5.0, -10.0, // HighFaultThreshold, HighHighFaultThreshold, LowFaultThreshold, LowLowFaultThreshold,
                      null, null, false); // aiPointAttributReadeDelegate,aiPointAttributeWriteDelegate,bBypassControlLoopIntegrityCheck = false);

            m_iPointMotionFaultAlarm = new CPoint<double, double>(m_strMotionFaultAlarmName, @"Point used soley to create a MotionFault.",
                      0.0, null, null, // Value, Min, Max
                      null, false, true, //Setpoint, ControlLoop, ControlLoopDisable
                      true, true, false, true, //Dynamic, Writeable, Hidden, Alarmable
                      @"", 1, 0, //Units, Slope, Offset
                      0.00001, false, 0.1, true, //ChangePercent, ChangePercentDisable, ChangeValue, ChangeValueDisable
                      false, 0.0, //FaultDisabled, FaultHysteresis,
                      0.5, 0.6, -5.0, -10.0, // HighFaultThreshold, HighHighFaultThreshold, LowFaultThreshold, LowLowFaultThreshold,
                      null, null, false); // aiPointAttributReadeDelegate,aiPointAttributeWriteDelegate,bBypassControlLoopIntegrityCheck = false);

            // Add points to PointServer
            m_oPointsServer.AddPoint(m_iPointSampleAlarm);

            m_oPointsServer.AddPoint(m_iPointScanAbortAlarm);
            m_oPointsServer.AddPoint(m_iPointScanSensorFaultAlarm);
            m_oPointsServer.AddPoint(m_iPointMotionFaultAlarm);
        }

        public bool EnableSampleAlarms(bool bEnable)
        {
            try
            {
                // Create a list of the rules to enable \ disable
                const int nNumRules = 2;
                IAlarmRule[] aiAlarmRules = new IAlarmRule[nNumRules];
                aiAlarmRules[0] = m_oAlarmManager.GetAlarmRule("DIn Sample1 True");
                aiAlarmRules[1] = m_oAlarmManager.GetAlarmRule("Sample Alarm Fault");

                //Enable or Disable based on what was passed in
                if (bEnable)
                {
                    m_oAlarmManager.EnableAlarmRules(aiAlarmRules);
                }
                else
                {
                    m_oAlarmManager.DisableAlarmRules(aiAlarmRules);
                }
                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!   Unable to enable / disable SampleAlarms!", ex);
            }
            return false;
        }

        public bool RegisterSettings(IObjectSetting iObjectSetting)
        {
            m_iSettingsManager.RegisterToSerializeAsFile(new[] { string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(m_strStateMachineCodeGeneratorSettingsFileName),
                                                                           iObjectSetting.ObjectSettingName,
                                                                           Path.GetExtension(m_strStateMachineCodeGeneratorSettingsFileName))},
                                                         iObjectSetting);
            try
            {
                m_iSettingsManager.LoadSettings(iObjectSetting); // Added by Zack 09/02/2016
                //m_iSettingsManager.SaveSettings(iObjectSetting);  // Removed by Zack 09/02/2016 because this call overwrites the old settings
                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, string.Format("Unable to register {0} Settings!", iObjectSetting.ObjectSettingName), ex);
                return false;
            }
        }

        public bool SaveSettings(IObjectSetting iObjectSetting)
        {
            try
            {
                m_iSettingsManager.SaveSettings(iObjectSetting);
                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, string.Format("Unable to save {0} Settings!", iObjectSetting.ObjectSettingName), ex);
                return false;
            }
        }

        public bool LoadSettings(IObjectSetting iObjectSetting)
        {
            try
            {
                m_iSettingsManager.LoadSettings(iObjectSetting);
                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, string.Format("Unable to load {0} Settings!", iObjectSetting.ObjectSettingName), ex);
                return false;
            }
        }

        public void DeserializeSettings()
        {
            try
            {
                //m_oPointsServer..DeserializeFromFile(m_strStateMachineCodeGeneratorSettingsPathAndFileName);
                m_iSettingsManager.LoadSettings(m_oPointsServer);
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Unable to Deserialize System Settings!", ex);
            }
        }

        public void SerializeSettings()
        {
            try
            {
                //m_oPointsServer.s.SerializeToFile(m_strStateMachineCodeGeneratorSettingsPathAndFileName);
                m_iSettingsManager.SaveSettings(m_oPointsServer);
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Unable to Serialize System Settings!", ex);
            }
        }

        public void ClearFaults()
        {
            m_iStateMachineCodeGeneratorMotion.AxisGroup.ClearFaults();
        }

        public void ScanAbort()
        {
            try
            {
                StopAllMotionAsync();
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to Abort the Scan!", ex);
            }
        }

        public bool SampleTaskSP()
        {
            try
            {
                const int nTimeout_ms = 2000;
                IStateMachineCodeGeneratorData iStateMachineCodeGeneratorData;

                // Block until a feature vector is obtained or the timeout occurs
                m_blocoliStateMachineCodeGeneratorData.TryTake(out iStateMachineCodeGeneratorData, (int)nTimeout_ms);

                // If a feature vector is obtained...
                if (iStateMachineCodeGeneratorData != null)
                {
                    bool bDataDiscarded = false;
                    try
                    {
                        m_iStateMachineCodeGeneratorRuleEngineSample.ProcessData(ref iStateMachineCodeGeneratorData);
                    }
                    catch (Exception oException)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to process data with rule engine!", oException);
                        bDataDiscarded = true;
                    }

                    try
                    {
                        // Only add the feature vector if it was not discarded in one of the checks above.
                        if (!bDataDiscarded)
                        {
                            if (!m_blocoliStateMachineCodeGeneratorDataProcessed.TryAdd(iStateMachineCodeGeneratorData))
                            {
                                ms_iLogger.Log(ELogLevel.Error, "Error!  Unable to add to the m_blocoliStateMachineCodeGeneratorDataProcessed!");
                            }
                        }
                    }
                    catch (Exception oException)
                    {
                        ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to add to the m_blocoliStateMachineCodeGeneratorDataProcessed!", oException);
                    }
                }
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!  Exception in SampleTaskSP!", ex);
            }

            return true;
        }

        public void LoadOfflineDataFileAsync(string strDataFilePath)
        {
            LoadOfflineDataFileAsync(strDataFilePath, new CancellationToken());
        }
        public Task<bool> LoadOfflineDataFileAsync(string strDataFilePath, CancellationToken oCancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    m_strOfflineViewFilePath = strDataFilePath;
                    m_bDataModifiedByOperator = false;

                    return LoadOfflineData(m_strOfflineViewFilePath);
                }
                catch (Exception oException)
                {
                    ms_iLogger.LogException(ELogLevel.Error,
                                            string.Format("Error while readind data file {0}", m_strOfflineViewFilePath),
                                            oException);
                    return false;
                }
            }, oCancellationToken);
        }

        public void NotifyMotionFaultAlarm()
        {
            m_iPointMotionFaultAlarm.CurrentValue = 1.0;
            m_iPointMotionFaultAlarm.CurrentValue = 0.0;
        }

        public void NotifyScanSensorFaultAlarm()
        {
            m_iPointScanSensorFaultAlarm.CurrentValue = 1.0;
            m_iPointScanSensorFaultAlarm.CurrentValue = 0.0;
        }

        public void NotifytSampleAlarm()
        {
            m_iPointSampleAlarm.CurrentValue = 1.0;
            m_iPointSampleAlarm.CurrentValue = 0.0;
        }

        public bool HomeAllAxis()
        {
            try
            {
                if (m_bSimulationMode)
                    return true;

                // Home all axes
                m_iStateMachineCodeGeneratorMotion.HomeAsync(m_iStateMachineCodeGeneratorMotion.AxisGroup, m_dblMotionHomingTimeout);
                m_bHomeInProgress = true;

                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to HomeAllAxis!", ex);
                return false;
            }
        }

        public bool SaveAllInspectionResultsAsync()
        {
            try
            {
                //TODO: We may want to replace DateTime.Now with start of scan time
                //TODO: Remove the hardcoding of the current config file once the info is available from ISystemSettings
                //m_oDataOutputManager.ExportToCsvFileAsynch(DateTime.Now, m_lstJudgedFeatureVector.Values, m_lstJudgedEdgeMeas, m_IStateMachineCodeGeneratorSetupRecipe,
                //                                           Path.Combine(m_iSettingsManager.SettingsDir,
                //                                                        @"LastSettingsUsed DO NOT MODIFY\StateMachineCodeGeneratorSettings.xml"));
                //m_iStateMachineCodeGeneratorGui.EnableCommitButton(false);
                if (IsInOfflineViewerMode)
                {
                    // Revert the StateMachineCodeGeneratorSetupRecipe interface to the object used in online mode
                    //StateMachineCodeGeneratorSetupRecipe = m_IStateMachineCodeGeneratorSetupRecipeOnline;
                    if (!m_bDataModifiedByOperator)
                    {
                        return true;
                    }
                    //else
                    //{
                    //    if (MessageBox.Show("Data has been modified. Do you want to save changes.", "Save Data", MessageBoxButtons.OKCancel,
                    //                        MessageBoxIcon.Question) != DialogResult.OK)
                    //        return true;
                    //}
                }
                //m_oDataOutputManager.ExportToCsvFileAsynch(m_efileTypeToSave, m_nMaxNgImagesToSave, m_datetimeStartOfScan, m_lstJudgedFeatureVector.Values, m_lstJudgedEdgeMeas,
                //                                           new CStateMachineCodeGeneratorSetupRecipe(m_iStateMachineCodeGeneratorSetupRecipeLast, m_iStateMachineCodeGeneratorSetupRecipeLast.InspectionTime),
                //                                           Path.Combine(m_iSettingsManager.SettingsDir, @"StateMachineCodeGeneratorSettings.xml"), IsInOfflineViewerMode, "");
                m_bDataModifiedByOperator = false;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to SaveAllInspectionResultsAsync!", ex);
            }

            return true;
        }

        public bool StartProcessingThreads()
        {
            try
            {
                // Start processing threads
                for (int i = 0; i < m_nNumSingleChanCalcThreads; i++)
                {
                    m_aoSampleTask[i].Start();
                }

                //m_oDataOutputManager.Start();

                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to StartProcessingThreads!", ex);
            }
            return false;
        }

        public bool StopAllMotionAsync()
        {
            try
            {
                if (m_bSimulationMode)
                    return true;

                m_iStateMachineCodeGeneratorMotion.HaltAllMotionAxis();
                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to StopAllMotionAsync!", ex);
                return false;
            }
        }

        public bool StopProcessingThreads()
        {
            try
            {
                // Stop processing threads
                for (int i = 0; i < m_nNumSingleChanCalcThreads; i++)
                {
                    m_aoSampleTask[i].Pause();
                }
                return true;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error!  Unable to StopProcessingThreads!", ex);
            }
            return false;
        }

        private void CopyRuleEngineSettings()
        {
            // Copy rule engine settings files from ms_strRuleEngineSettingsDirOriginal to m_strRuleEngineSettingsDir if
            // they do not exist in m_strRuleEngineSettingsDir
            if (!Directory.Exists(m_strRuleEngineSettingsDir))
            {
                Directory.CreateDirectory(m_strRuleEngineSettingsDir);
            }

            Tuple<string, string>[] tuplestrstrSourceDestPath
                = Directory.GetFiles(ms_strRuleEngineSettingsDirOriginal)
                           .Select(strPath => new Tuple<string, string>(strPath, Path.Combine(m_strRuleEngineSettingsDir, Path.GetFileName(strPath))))
                                                                         .ToArray();
            for (int nIndex = 0; nIndex < tuplestrstrSourceDestPath.Length; nIndex++)
            {
                if (!File.Exists(tuplestrstrSourceDestPath[nIndex].Item2))
                {
                    File.Copy(tuplestrstrSourceDestPath[nIndex].Item1, tuplestrstrSourceDestPath[nIndex].Item2);
                }
            }
        }

        #region Serialize/Deserialize
        #endregion Serialize/Deserialize

        #region StateMachineCodeGeneratorSystemTasks

        private void CreateTasks(string strName, ref CThreadHandler<bool, int>[] oThreadArray, int nNumTasks, Func<bool> oFunc, System.Threading.ThreadPriority oPriority, int nDelayInterval)
        {
            oThreadArray = new CThreadHandler<bool, int>[nNumTasks];

            for (int i = 0; i < nNumTasks; i++)
            {
                try
                {
                    string strTaskName = string.Format(strName, i + 1);
                    oThreadArray[i] = new CThreadHandler<bool, int>(strTaskName, oFunc, oPriority, nDelayInterval);
                }
                catch (Exception ex)
                {
                    ms_iLogger.LogException(ELogLevel.Fatal, "Error creating task in CreateTasks!", ex);
                }
            }
        }

        #endregion StateMachineCodeGeneratorSystemTasks

        #region StateMachineActionFunctions
        #endregion StateMachineActionFunctions

        private void InitDirCleanerSettings()
        {
            InitDirCleanerSettings(m_oLogDirectoryCleaner, m_strLogDirPath, m_uLogDirMaxFilesAllowed, m_uLogDirMaxSizeAllowed, m_oLogDirRetentionTime, m_dLogDirectoryCleaningInterval,
                                    ref m_strLogDirectoryCleaningIntervalUnits_MinutesHoursDays, ref m_strLogDirRetentionUnits_MinutesHoursDays);

            //TODO:  Put this back in when data output is setup for GenSysEasy-->  InitDirCleanerSettings(m_oDataOutputDirCleaner, StateMachineCodeGeneratorDataDataDir, m_uDataOutputDirMaxFilesAllowed, m_uDataOutputDirMaxSizeAllowed, m_oDataOutputDirRetentionTime, m_dDataOutputDirectoryCleaningInterval,
            //TODO:  Put this back in when data output is setup for GenSysEasy-->                          ref m_strDataOutputDirectoryCleaningIntervalUnits_MinutesHoursDays, ref m_strDataOutputDirRetentionUnits_MinutesHoursDays);
        }

        private void InitDirCleanerSettings(CDirCleaner oDirCleaner, string strDirPath, ulong ulMaxFilesAllowed, ulong ulMaxSizeAllowed, double oLogDirRetentionTime,
                                            double dCleaningInterval, ref string strIntervalUnits, ref string strRetentionUnits)
        {
            oDirCleaner.AddDirPath(strDirPath);
            oDirCleaner.MaxNumOfFiles = (ulong)ulMaxFilesAllowed;
            oDirCleaner.MaxCombinedDirSize = (ulong)ulMaxSizeAllowed;

            if (strIntervalUnits == null)
            {
                strIntervalUnits = "DAYS";
            }

            // ToDo: This should be put into the Directory Cleaner
            switch (strIntervalUnits.ToUpper())
            {
                case "DAYS":
                    {
                        oDirCleaner.CleaningIntervalInDays = dCleaningInterval;
                    }
                    break;

                case "HOURS":
                    {
                        oDirCleaner.CleaningIntervalInHours = dCleaningInterval;
                    }
                    break;

                case "MINUTES":
                    {
                        oDirCleaner.CleaningIntervalInMinutes = dCleaningInterval;
                    }
                    break;

                default:
                    {
                        ms_iLogger.Log(ELogLevel.Error, "Invalid Interval Units for Log Directory Cleaning. Must be Days, Hours, or Minutes.");
                    }
                    break;
            }

            if (strRetentionUnits == null)
            {
                strRetentionUnits = "DAYS";
            }

            switch (strRetentionUnits.ToUpper())
            {
                case "DAYS":
                    {
                        oDirCleaner.FileRetentionInDays = oLogDirRetentionTime;
                    }
                    break;

                case "HOURS":
                    {
                        oDirCleaner.FileRetentionInHours = oLogDirRetentionTime;
                    }
                    break;

                case "MINUTES":
                    {
                        oDirCleaner.FileRetentionInMinutes = oLogDirRetentionTime;
                    }
                    break;

                default:
                    {
                        ms_iLogger.Log(ELogLevel.Error, "Invalid Interval Units for Log Directory Units. Must be Days, Hours, or Minutes.");
                    }
                    break;
            }

            if (dCleaningInterval != 0.0)
            {
                oDirCleaner.PeriodicCleaningEnabled = true;  // Start the cleaning timer
            }
        }

        private bool LoadOfflineData(string strDataFilePath)
        {
            //ConcurrentDictionary<int, IStateMachineCodeGeneratorData> concdiriStateMachineCodeGeneratorFeatureVectorsById;
            //CStateMachineCodeGeneratorDataHeader oStateMachineCodeGeneratorDataHeader;
            //if (m_oDataOutputManager.ReadOfflinData(strDataFilePath, 0, out oStateMachineCodeGeneratorDataHeader, out concdiriStateMachineCodeGeneratorFeatureVectorsById))
            //{
            //    if (oStateMachineCodeGeneratorDataHeader != null)
            //    {
            //    }
            //    if (concdiriStateMachineCodeGeneratorFeatureVectorsById != null)
            //    {
            //    }
            //}
            return true;
        }

        private void m_oAlarmManager_OnAlarm(object sender, List<CAlarmEventArg> lstoAlarmRules)
        {
            foreach (CAlarmEventArg oAlarmEventArg in lstoAlarmRules)
            {
                // Is the alarm activating or clearing or just changing
                bool bAlarmChanged = (bool)oAlarmEventArg.ActiveStateChanged;
                bool bAlarmActive = oAlarmEventArg.AlarmRule.IsActive;
                // Determine logger severity based on Alarm severity and whether the alarm state changed Active
                ELogLevel eLogLevel = ELogLevel.Warning;
                if (oAlarmEventArg.AlarmRule.Severity == EAlarmSeverity.Critical)
                    eLogLevel = ELogLevel.Fatal;

                // Create message type
                string strMsg = "";
                if (bAlarmChanged && bAlarmActive && !oAlarmEventArg.AlarmRule.IsDisabled)
                {
                    strMsg = String.Format("     {0} Alarm Active: {1}!", oAlarmEventArg.AlarmRule.Name, oAlarmEventArg.AlarmRule.Description);
                }
                else if (bAlarmChanged && !bAlarmActive)
                {
                    eLogLevel = ELogLevel.Info;
                    strMsg = String.Format("     {0} Alarm Cleared!", oAlarmEventArg.AlarmRule.Name);
                }
                else if (!bAlarmChanged)
                {
                    eLogLevel = ELogLevel.Info;
                    strMsg = String.Format("     {0} Alarm Underlying Fault Changed:!", oAlarmEventArg.AlarmRule.Name);
                }

                // Log Alarm message
                if (strMsg != "")
                    ms_iLogger.Log(eLogLevel, strMsg);

                if (IsCriticalAlarms)
                {
                    m_iStateMachineCodeGeneratorIo.Sample1DOut.Value = true;
                }
                else if (IsWarningAlarms)
                {
                    m_iStateMachineCodeGeneratorIo.Sample2DOut.Value = true;
                }
                else
                {
                    m_iStateMachineCodeGeneratorIo.Sample1DOut.Value = false;
                    m_iStateMachineCodeGeneratorIo.Sample2DOut.Value = false;
                }
            }
        }


        private void m_oAlarmManager_OnAlarmAcknowledged(object objSender, List<CAlarmEventArg> lstoAlarmRules)
        {
            try
            {
                m_iStateMachineCodeGeneratorIo.Sample1DOut.Value = false;
                m_iStateMachineCodeGeneratorIo.Sample2DOut.Value = false;
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, "Error setting Sample1DOut and Sample2DOut .", ex);
            }
        }

        private void StateMachineCodeGeneratorMotion_OnMotionComplete(IAxisGroup iaxisgroupSender, CMotionCompleteEventArg oMotionCompleteEventArg)
        {
            try
            {
                // Stop the blinking stack light
                m_timerStackLight.Stop();
                if (m_bHomeInProgress)
                {
                    m_bHomeInProgress = false;
                    if (oMotionCompleteEventArg.Status == EMotionCompleteStatus.Succeeded)
                    {
                        m_oStateMachineCodeGeneratorStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.AllAxisHomedEvent, null);
                    }
                    else
                    {
                        m_oStateMachineCodeGeneratorStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.MotionFaultEvent, null);
                    }
                }
                else
                {
                    if (oMotionCompleteEventArg.Status == EMotionCompleteStatus.Succeeded)
                    {
                        m_oStateMachineCodeGeneratorStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.MoveCompleteEvent, null);
                    }
                    else if (oMotionCompleteEventArg.Status == EMotionCompleteStatus.Failed && oMotionCompleteEventArg.Error != null)
                    {
                        m_oStateMachineCodeGeneratorStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.MotionFaultEvent, null);
                        ms_iLogger.LogException(ELogLevel.Error, "Motion Error: ", oMotionCompleteEventArg.Error);
                    }
                    else
                    {
                        m_oStateMachineCodeGeneratorStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.MoveCompleteEvent, null);
                        ms_iLogger.Log(ELogLevel.Warning, "Invalid OnMotionComplete Event");  // This case shouldn't be hit, but log it just in case
                    }
                }
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, ex.Message, ex);
            }
        }

        private void StateMachineCodeGeneratorMotion_OnMotionStarted(IAxisGroup iAxisGroupSender, CMotionStartedEventArg oMotionStartedArg)
        {
            try
            {
                // Start the blinking stack light
                m_timerStackLight.Start();
            }
            catch (Exception ex)
            {
                ms_iLogger.LogException(ELogLevel.Error, ex.Message, ex);
            }
        }

        #region GuiActionRequestMethods

        /// <summary>
        /// Instruct the GUI to enable the Start button and disable the Stop or Abort button.
        /// </summary>
        void GuiEnableStartButton()
        {
            GuiActionRequestHelper(EGuiActionRequest.ENABLE_START_BUTTON);
        }

        /// <summary>
        /// Instruct the GUI to enable the Stop or Abort button and disable the Start button.
        /// </summary>
        void GuiEnableStopButton()
        {
            GuiActionRequestHelper(EGuiActionRequest.ENABLE_STOP_BUTTON);
        }


        /// <summary>
        /// Inform the GUI that a specific action has occurred which may need to be acted on.
        /// </summary>
        /// <param name="eActionRequest">The Action which the GUI needs to act on.</param> 
        /// <returns>Returns Nothing</returns>    
        void GuiActionRequestHelper(EGuiActionRequest eActionRequest)
        {
            // Send GUI action request if any GUI is connected to this event
            if (eventGuiActionRequest != null)
            {
                eventGuiActionRequest.Invoke(eActionRequest, null);
            }
        }

        /// <summary>
        /// Inform the GUI that a specific action has occurred which may need to be acted on.
        /// </summary>
        /// <param name="eActionRequest">The Action which the GUI needs to act on.</param> 
        /// <param name="oArgs">An object the GUI needs in order to process the request.</param> 
        /// <returns>Returns Nothing</returns> 
        void GuiActionRequestHelper(EGuiActionRequest eActionRequest, params object[] aoArgs)
        {
            // Send GUI action request if any GUI is connected to this event
            if (eventGuiActionRequest != null)
            {
                eventGuiActionRequest.Invoke(eActionRequest, aoArgs);
            }
        }


        #endregion



        #endregion Methods

        #region InnerClasses
        #endregion InnerClasses
    }//end CStateMachineCodeGeneratorSystem
}//end namespace StateMachineCodeGeneratorSystem
