using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Common
{
    public struct ErrorStructure
    {
        public ErrorId Id { get; }

        public List<string> PropertyNames { get; set; }

        public string ErrorMessage { get; set; }

        public ErrorStructure(ErrorId id, List<string> propertyNames, string errorMessage) {
            Id = id;
            PropertyNames = propertyNames ?? new List<string>();
            ErrorMessage = errorMessage;
        }

        public ErrorStructure(ErrorId id, string errorMessage, params string[] propertyNameArr) {
            Id = id;
            PropertyNames = new List<string>(propertyNameArr);
            ErrorMessage = errorMessage;
        }

        public ErrorStructure(ErrorId id, string errorMessage): this(id, errorMessage, new string[]{}) {}
        public ErrorStructure(ErrorId id): this(id, null) { }

        //public ErrorStructure(ErrorId id, List<string> propertyNames) : this(id, propertyNames, null) { }
        //public ErrorStructure(ErrorId id) : this(id, new string[]) { }


    }
}
