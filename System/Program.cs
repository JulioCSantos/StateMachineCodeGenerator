using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Corning.GenSys.Logger;
using StateMachineCodeGenerator.StateMachineCodeGeneratorGui;
using StateMachineCodeGenerator.StateMachineCodeGeneratorSystem;
using StateMachineCodeGenerator.Interfaces;

using Microsoft.Win32.SafeHandles;

namespace StateMachineCodeGeneratorSystem
{
    static class Program
    {
        private static ILogger ms_iLogger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CLoggerFactory.SetLogCreationFunction(Corning.GenSys.Logger_Nlog.CLogger_Nlog.CreateLog);
		    ms_iLogger = CLoggerFactory.CreateLog("StateMachineCodeGeneratorApplication");
            ms_iLogger.Log(ELogLevel.Info, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            ms_iLogger.Log(ELogLevel.Info, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Application Start <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            ms_iLogger.Log(ELogLevel.Info, string.Format("Version: {0}", CStateMachineCodeGeneratorSystem.VersionS));
            ms_iLogger.Log(ELogLevel.Info, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

            //string[] astrLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(oAssembly => oAssembly.FullName).ToArray();

            // Is the program already running?
            if (IsProcessAlreadyRunning())
            {
                // exist after logging
                ms_iLogger.Log(ELogLevel.Info, "  ");
                ms_iLogger.Log(ELogLevel.Info, "  ");
                ms_iLogger.Log(ELogLevel.Info, " StateMachineCodeGeneratorSystem is already running so this instance is closing! ");
                ms_iLogger.Log(ELogLevel.Info, "  ");
                ms_iLogger.Log(ELogLevel.Info, "  ");
                ms_iLogger.Log(ELogLevel.Info, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                ms_iLogger.Log(ELogLevel.Info, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Application End <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                ms_iLogger.Log(ELogLevel.Info, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IStateMachineCodeGeneratorSystem oStateMachineCodeGeneratorSystem = new CStateMachineCodeGeneratorSystem();
            FrmStateMachineCodeGeneratorGui frmStateMachineCodeGeneratorGui = new FrmStateMachineCodeGeneratorGui(oStateMachineCodeGeneratorSystem);

            // Set reference of Gui back to system
            //((CStateMachineCodeGeneratorSystem)oStateMachineCodeGeneratorSystem).SetStateMachineCodeGeneratorGui((IStateMachineCodeGeneratorGui)frmStateMachineCodeGeneratorGui);

            // Make sure that the StateMachine is in the Init State (i.e. eliminate any race condition where the StateMachine has not had enough CPU to run yet)
            while (oStateMachineCodeGeneratorSystem.SystemStateMachine.SystemState != "Init")
            {
                System.Threading.Thread.Sleep(20);
                ms_iLogger.Log(ELogLevel.Info, "Waiting for State Machine to be Initialized!");
            }

            ((CStateMachineCodeGeneratorSystem)oStateMachineCodeGeneratorSystem).SystemStateMachine.HandleEvent(StateMachineCodeGeneratorSystemEventsEnum.InitCompleteEvent, 0);


            Application.Run(frmStateMachineCodeGeneratorGui);

            ms_iLogger.Log(ELogLevel.Info, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            ms_iLogger.Log(ELogLevel.Info, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Application End <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            ms_iLogger.Log(ELogLevel.Info, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, uint windowStyle);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        ///// <summary>Checks whether a process is being debugged.</summary>
        ///// <remarks>
        ///// The "remote" in CheckRemoteDebuggerPresent does not imply that the debugger
        ///// necessarily resides on a different computer; instead, it indicates that the 
        ///// debugger resides in a separate and parallel process.
        ///// <para/>
        ///// Use the IsDebuggerPresent function to detect whether the calling process 
        ///// is running under the debugger.
        ///// </remarks>
        //[DllImport("Kernel32.dll", SetLastError = true, ExactSpelling = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool CheckRemoteDebuggerPresent(
        //    SafeHandle hProcess,
        //    [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);

        //[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        //static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);



        [DllImport("Kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CheckRemoteDebuggerPresent(SafeHandle hProcess, [MarshalAs(UnmanagedType.Bool)]ref bool isDebuggerPresent);








        public static bool IsProcessAlreadyRunning()
        {
            bool bIsDebuggerAttached = false;
            List<Process> lstoProcess = new List<Process>();
            //List<string> lststrProcessNames = new List<string>();
            Process processCurrent = Process.GetCurrentProcess();
            //string strProcessName = processCurrent.ProcessName;
            //string strProcessNameOther = processCurrent.ProcessName;
            //lststrProcessNames.Add(strProcessName);


            //if (strProcessName.Contains(".vshost"))
            //{
            //    //lstoProcess.Add(processCurrent);
            //    strProcessNameOther = strProcessName.Replace(".vshost", "");
            //    //lststrProcessNames.Add(strProcessName.Replace(".vshost", ""));
            //}
            //else
            //{
            //    strProcessNameOther = string.Format("{0}.vshost", strProcessName);
            //    //lststrProcessNames.Add(string.Format("{0}.vshost", strProcessName));
            //}

            //if (System.Diagnostics.Debugger.IsAttached == true)
            //{
            //    ms_iLogger.Log(ELogLevel.Info, string.Format("Debugger Attached"));
            //}


            // Loop through all of the processes on the system
            foreach (Process oProcess in Process.GetProcesses())
            {
                if (oProcess.ProcessName.Contains("StateMachineCodeGeneratorSystem") == true) // Look for the Process associated with this Application
                {
                    ms_iLogger.Log(ELogLevel.Info, string.Format("\r\nSaw Process {0}", oProcess.ProcessName));

                    if (oProcess.ProcessName.EndsWith(".vshost") == true) // Check to see if the Process is running under Visual Studio. It could be that the IDE is up but not debugging the application.
                    {
                        bIsDebuggerAttached = false;

                        //if (!CheckRemoteDebuggerPresent(new SafeProcessHandle(oProcess.Handle, true), ref bIsDebuggerAttached)) // Check to see if a Debugger is attached to the process
                        //{
                        //    ms_iLogger.Log(ELogLevel.Info, string.Format("Unable to obtain IsDebuggerAttached informmation for Process {0}", oProcess.ProcessName));
                        //}
                        //else // Was able to obtain a IsDebuggerAttached for the Application
                        //{
                        //    if (bIsDebuggerAttached == true)  // Check to see if Visual Studio is debugging the application. 
                        //    {
                        //        lstoProcess.Add(oProcess); // It is so add it to the list
                        //        ms_iLogger.Log(ELogLevel.Info, string.Format("Adding Visual Studio Process {0}", oProcess.ProcessName));
                        //    }
                        //}
                    }
                    else if (oProcess.ProcessName == "StateMachineCodeGeneratorSystem") // Running but Not under Visual Studio so add it to the list
                    {
                        lstoProcess.Add(oProcess);
                        ms_iLogger.Log(ELogLevel.Info, string.Format("Adding Process {0}", oProcess.ProcessName));
                    }

                }



                //if (oProcess.ProcessName == strProcessName || oProcess.ProcessName == strProcessNameOther || oProcess.ProcessName.ToLower().Contains("StateMachineCodeGeneratorSystem"))
                //{
                //    ms_iLogger.Log(ELogLevel.Info, string.Format("Adding Process {0}", oProcess.ProcessName));
                //    lstoProcess.Add(oProcess);
                //}
            }

            bool bAppAlreadyRunning = (lstoProcess.Count > 1);
            if (bAppAlreadyRunning)
            {
                MessageBox.Show("StateMachineCodeGenerator application already running", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    foreach (var oProcess in lstoProcess)
                    {
                        if (processCurrent != oProcess)
                        {
                            ShowWindow(oProcess.MainWindowHandle, 3);
                            SetForegroundWindow(oProcess.MainWindowHandle);
                        }
                    }
                }
                catch
                {
                }
            }
            return bAppAlreadyRunning;
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ms_iLogger.Log(ELogLevel.Info,
                          string.Format(
                              "\r\nXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX Application Ended with unhandled exception in Main thread XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\r\n{0}\r\nXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\r\n",
                              ExceptionToString(e.Exception)));
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ms_iLogger.Log(ELogLevel.Info,
                          string.Format(
                              "\r\nXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX Application Ended with unhandled exception in a child thread XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\r\n{0}\r\nXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\r\n",
                ExceptionToString(e.ExceptionObject as Exception)));
        }

        static string ExceptionToString(Exception oException)
        {
            if (oException == null) return "";
            StringBuilder oStringBuilder = new StringBuilder();
            oStringBuilder.Append("Message = ").AppendLine(oException.Message);
            oStringBuilder.Append("Source = ").AppendLine(oException.Source);
            oStringBuilder.AppendLine("Stack = ").AppendLine(oException.StackTrace);
            if (oException.InnerException != null)
            {
                oStringBuilder.AppendLine("---------- InnerException ---------");
                oStringBuilder.Append(ExceptionToString(oException.InnerException));
            }
            return oStringBuilder.ToString();
        }

    }
}
