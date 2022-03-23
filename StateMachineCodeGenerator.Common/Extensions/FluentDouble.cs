using System;

namespace StateMachineCodeGenerator.Common.Extensions
{
    public static class DoubleExtensions
    {
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        public static bool IsNear(this double arg1, double arg2, double tolerance, bool percentTolerance = false)
        {
            if (arg2 == 0d && percentTolerance == true) {
                throw new ArgumentException("percent tolerance can not be applied to zero value");
            }

            var absoluteDifference = Math.Abs(arg2 - arg1);
            var delta = percentTolerance ? Math.Abs(arg2 * tolerance) : Math.Abs(tolerance);
            return absoluteDifference <= delta;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute()]
        public static FluentDouble IsEqualTo(this double arg1, double arg2) {
            var fluentApi = new FluentDouble(arg1, arg2);
            return fluentApi;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute()]
        public static FluentWithin Within(this FluentDouble fluent, double tolerance) {
            var isWithin = new FluentWithin(fluent, tolerance);
            return isWithin;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute()]
        public static FluentPercent Percent(this FluentWithin within) {
            var percent = new FluentPercent(within);
            return percent;
        }
    }

    public class FluentDouble {
        public double Arg1 { get; }
        public double Arg2 { get; }

        public double Tolerance {get; protected set; }

        public FluentDouble(double arg1, double arg2) {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public bool Result() {
            return Arg1.Equals(Arg2);
        }

        public static implicit operator bool(FluentDouble fluentDouble)
        {
            return fluentDouble.Result();
        }
    }


    public class FluentWithin 
    {
        public FluentDouble FluentDouble { get;  }

        public double Tolerance { get; private set; }

        public FluentWithin(FluentDouble fluentDouble, double tolerance)
        {
            FluentDouble = fluentDouble;
            Tolerance = tolerance;
        }

        public bool Result()
        {
            var result = Math.Abs(FluentDouble.Arg1 - FluentDouble.Arg2) < Tolerance;
            return result;
        }

        public static implicit operator Boolean(FluentWithin within)
        {
            return within.Result();
        }
    }

    public class FluentPercent
    {
        public FluentWithin Within { get; }
        public FluentPercent(FluentWithin within) {
            Within = within;
        }

        public bool Result()
        {
            var result = Math.Abs(Within.FluentDouble.Arg1 - Within.FluentDouble.Arg2) < 
                         Within.FluentDouble.Arg2 * Within.Tolerance;

            return result;
        }


        public static implicit operator Boolean(FluentPercent percent)
        {
            return percent.Result();
        }
    }
}
