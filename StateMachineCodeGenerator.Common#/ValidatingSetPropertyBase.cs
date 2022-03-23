using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using StateMachineCodeGenerator.Common;

namespace StateMachineCodeGenerator.Common
{
    public class ValidatingSetPropertyBase : SetPropertyBase, INotifyDataErrorInfo
    {
        #region properties

        #region INotifyDataErrorInfo
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        
        #region HasErrors
        public bool HasErrors => ErrorStructuresByErrorId.Any();
        #endregion HasErrors

        #region GetErrors
        public IEnumerable GetErrors(string propertyName) {
            var errorsList = new List<string>();
            var errIds = ErrorIdsByPropertyName.Where(er => er.Key == propertyName)
                .SelectMany(er => er.Value);
            foreach (string errId in errIds) {
                errorsList.Add(ErrorStructuresByErrorId[errId].ErrorMessage);
                //errorsList.Add(ErrorStructuresByErrorId[errId].ErrorMessage + Esc2ClearLiteral);
            }

            return errorsList;
        }
        #endregion GetErrors

        #endregion INotifyDataErrorInfo

        #region Esc2ClearLiteral
        public readonly string Esc2ClearLiteral = Environment.NewLine + " - 'Esc' to clear";
        #endregion Esc2ClearLiteral

        #region ErrorStructuresByErrorId
        private Dictionary<ErrorId, ErrorStructure> _errorStructuresByErrorId;
        public Dictionary<ErrorId, ErrorStructure> ErrorStructuresByErrorId {
            get => _errorStructuresByErrorId ?? (_errorStructuresByErrorId = new Dictionary<ErrorId, ErrorStructure>());
            set => SetProperty(ref _errorStructuresByErrorId, value);
        }
        #endregion ErrorStructuresByErrorId    

        #region ErrorIdsByPropertyName
        private Dictionary<string, List<ErrorId>> _errorIdsByPropertyName;
        public Dictionary<string, List<ErrorId>> ErrorIdsByPropertyName {
            get => _errorIdsByPropertyName ?? (_errorIdsByPropertyName = new Dictionary<string, List<ErrorId>>());
            set => _errorIdsByPropertyName = value;
        }
        #endregion ErrorIdsByPropertyName

        //#region ErrorMessagesByPropertyName
        //private Dictionary<string, List<string>> _errorMessagesByPropertyName;
        //protected Dictionary<string, List<string>> ErrorMessagesByPropertyName
        //{
        //    get => _errorMessagesByPropertyName ?? (_errorMessagesByPropertyName = new Dictionary<string, List<string>>());
        //    set { _errorMessagesByPropertyName = value; }
        //}
        //#endregion ErrorMessagesByPropertyName

        //#region OrderedProperties
        //private List<string> _orderedProperties;
        //public List<string> OrderedProperties {
        //    get { return _orderedProperties ?? (_orderedProperties = new List<string>()); }
        //    set { _orderedProperties = value; }
        //}
        //#endregion OrderedProperties

        #endregion properties

        #region events

        public event PropertyChangingEventHandler PropertyChanging;
        #endregion events

        #region methods

        public void AddError(ErrorStructure errorStructure) {
            // Update ErrorStructuresByErrorId
            // if ErrorId already reported just add any new property name to ErrorStructuresByErrorId and publish the new error...
            if (ErrorStructuresByErrorId.ContainsKey(errorStructure.Id)) {
                var errStrKeyVal = ErrorStructuresByErrorId[errorStructure.Id];
                foreach (string propName in errorStructure.PropertyNames) {
                    if (errStrKeyVal.PropertyNames.Contains(propName) == false) {
                        errStrKeyVal.PropertyNames.Add(propName);
                    }
                }
            }
            else { ErrorStructuresByErrorId.Add(errorStructure.Id, errorStructure); }

            // Update ErrorIdsByPropertyName
            foreach (string propName in errorStructure.PropertyNames) {
                if (ErrorIdsByPropertyName.ContainsKey(propName)) {
                    var errorIdsList = ErrorIdsByPropertyName[propName];
                    errorIdsList.Add(errorStructure.Id);
                }
                else { ErrorIdsByPropertyName.Add(propName, new List<ErrorId>(){errorStructure.Id}); }

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propName));
            }
        }

        public void ClearErrorsOnErrorId(ErrorId errStructId) {
            if (ErrorStructuresByErrorId.ContainsKey(errStructId) == false) { return; }

            var errPropList = ErrorStructuresByErrorId[errStructId];
            ErrorStructuresByErrorId.Remove(errStructId);
            foreach (var propName in errPropList.PropertyNames) {
                ErrorIdsByPropertyName.Remove(propName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propName));
            }
        }

        public void ClearErrorsOnPropertyName(string propertyName) {
            if (ErrorIdsByPropertyName.ContainsKey(propertyName) == false) { return; }

            var errorIds = ErrorIdsByPropertyName[propertyName];
            foreach (ErrorId errorId in errorIds) {
                ErrorStructuresByErrorId.Remove(errorId);
            }

            ErrorIdsByPropertyName.Remove(propertyName);
        }

        public void ClearFirstError() {
            if (ErrorIdsByPropertyName.Any() == false) { return; }
            ClearErrorsOnErrorId(ErrorIdsByPropertyName.First().Value.First());
        }

        [DebuggerStepThrough]
        public override bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null) {
            
            if (Equals(value, backingField)) { return false; }

            // confirm if should proceed with changes
            var propertyChangingArgs = new CancellablePropertyChangingEventArgs(propertyName, value);
            if (UIDispatcher?.CheckAccessFunc != null && UIDispatcher.CheckAccessFunc()) {
                UIDispatcher.InvokeFunc.Invoke(() => PropertyChanging?.Invoke(this, propertyChangingArgs));
            }
            else { PropertyChanging?.Invoke(this, propertyChangingArgs); }

            if (propertyChangingArgs.Cancel) { return false; }

            base.SetProperty(ref backingField, value, propertyName);

            return true;
        }
        #endregion methods

    }
    public class CancellablePropertyChangingEventArgs : PropertyChangingEventArgs
    {
        public CancellablePropertyChangingEventArgs(string propertyName, object newValue) : base(propertyName) {
            NewValue = newValue;
        }

        public bool Cancel { get; set; }

        public object NewValue { get; }

    }
}
