using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Common
{
    public class XPLogger
    {

        #region properties

        #region ErrorLogSeq
        public int Seq { get; set; }
        #endregion ErrorLogSeq

        #region Id
        public string Id { get; private set; }
        #endregion Id

        #region ActiveErrors
        private ObservableCollection<ErrorLog> _activeErrors;
        public ObservableCollection<ErrorLog> ActiveErrors => _activeErrors ??= new ObservableCollection<ErrorLog>();
        #endregion ActiveErrors

        #region logs
        private static readonly Dictionary<string, XPLogger> LogsSource = new();
        private static ReadOnlyDictionary<string, XPLogger> _logs;
        public static ReadOnlyDictionary<string, XPLogger> Logs => _logs ??= new(LogsSource);
        #endregion logs

        #endregion properties


        #region singleton

        private const string DefaultId = "";

        public static XPLogger Instance => GetInstance(null);

        public static XPLogger GetInstance(string logId = null) {
            if (logId == null) {
                // default active log is always the last used log ...
                if (ActiveLogger != null) { return ActiveLogger; }
                // ... else swap back to default id to retrieve the default logger subsequently
                else { logId = DefaultId; } 
            }
            // if Log Id is provided return the Logger created with the same Id ...
            if (Logs.ContainsKey(logId)) { ActiveLogger = Logs[logId];}
            //..or if none is found create a new Logger instance
            else {
                ActiveLogger = new XPLogger(logId);
                LogsSource.TryAdd(ActiveLogger.Id, ActiveLogger);
            }

            return ActiveLogger;
        }
        public static XPLogger ActiveLogger { get; set; }
        private XPLogger(string id) { Id = id; }
        #endregion singleton


        #region methods

        public void AddError(ErrorLog error) {
            Seq++;
            error.Seq = Seq;
            ActiveErrors.Add(error);
        }
        public void ClearErrors() { ActiveErrors.Clear(); }
        #endregion methods

    }
}
