using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Common
{
    public partial struct ErrorId : IEquatable<ErrorId>
    {
        private string _id;

        public ErrorId(string id) : this() { _id = id; }

        public override string ToString() { return _id; }

        public static implicit operator ErrorId(string i) {
            return new ErrorId { _id = i };
        }

        public static implicit operator string(ErrorId p) {
            return p._id;
        }

        public override bool Equals(object obj) => this.Equals((ErrorId)obj);  

        public bool Equals(ErrorId eid) {

            // If run-time types are not exactly the same, return false.
            if (this.GetType() !=eid.GetType()) {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            var result = eid._id == this._id;
            return result;
        }
        public bool Equals(ErrorId eId1, ErrorId eId2) {
            return eId1._id == eId2._id;
        }

        public override int GetHashCode() {
            return _id.GetHashCode();
        }
    }
}
