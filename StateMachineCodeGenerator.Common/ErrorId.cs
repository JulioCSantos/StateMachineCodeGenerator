using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Common
{
    public partial struct ErrorId
    {
        private string _axisId;

        public ErrorId(string id) : this() { _axisId = id; }

        public override string ToString() { return _axisId; }

        public static implicit operator ErrorId(string i) {
            return new ErrorId { _axisId = i };
        }

        public static implicit operator string(ErrorId p) {
            return p._axisId;
        }
    }
}
