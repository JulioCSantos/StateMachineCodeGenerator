using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Common
{
    public class ErrorSeverity
    {
        public static ErrorSeverity Info => new ErrorSeverity(0, nameof(Info));
        public static ErrorSeverity Warning => new ErrorSeverity(1, nameof(Warning));
        public static ErrorSeverity Error => new ErrorSeverity(2, nameof(Error));

        public int Key { get; }
        public string Value { get;}

        public static ReadOnlyDictionary<int,ErrorSeverity> Severities { get; }

        static ErrorSeverity() {
            var errs = typeof(ErrorSeverity)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(predicate: p => p.PropertyType == typeof(ErrorSeverity))
                .Select(pi => (ErrorSeverity)pi.GetValue(null))
                .OrderBy(p => p.Key);
            var errsDict = new Dictionary<int, ErrorSeverity>();
            errs.ToList().ForEach(e => errsDict.Add(e.Key, e));
            Severities = new ReadOnlyDictionary<int, ErrorSeverity>(errsDict);
        }

        private ErrorSeverity(int key, string value) {
            Key = key;
            Value = value;
        }


    }
}
