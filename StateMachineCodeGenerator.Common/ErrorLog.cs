using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Common
{
    public class ErrorLog 
    {
        public int Seq { get; set; }
        public ErrorId Id { get; }
        public string Message { get; set; }
        public ErrorSeverity Severity { get; set; }
        public string CallingMemberName { get; set; }


        #region Errors
        private static readonly Dictionary<ErrorId, ErrorLog> ErrorsSource = new();
        private static ReadOnlyDictionary<ErrorId, ErrorLog> _errors;
        public static ReadOnlyDictionary<ErrorId, ErrorLog> Errors =>
            _errors ??= new ReadOnlyDictionary<ErrorId, ErrorLog>(ErrorsSource);

        #endregion Errors
        public ErrorLog(ErrorId id, string message, ErrorSeverity severity,[CallerMemberName] string callingMemberName = null)
            : this(id, severity, callingMemberName) {
            Message = message ?? string.Empty;
            ErrorsSource.TryAdd(Id, this);
        }

        private ErrorLog(ErrorId id, ErrorSeverity severity, [CallerMemberName] string callingMemberName = null) {
            Id = id;
            Severity = severity;
            CallingMemberName = callingMemberName;
        }

        public void Remote(ErrorId id) { ErrorsSource.Remove(id); }

        public void Clear() { ErrorsSource.Clear(); }

        public static ErrorLog GetEditedErrorLog(ErrorId errId, object[] args, [CallerMemberName] string callingMemberName = null) {
            if (Errors.ContainsKey(errId) == false) { throw new ArgumentException("Error Id '" + errId + "' Not Found"); }
            var tempErr = Errors[errId];
            var msg = string.Format(tempErr.Message, args);
            var editedError = new ErrorLog(tempErr.Id, tempErr.Severity, callingMemberName);
            editedError.Message = msg;
            return editedError;

        }
    }
}
