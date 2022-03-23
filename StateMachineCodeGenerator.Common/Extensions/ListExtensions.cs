using System;
using System.Collections.Generic;

namespace StateMachineCodeGenerator.Common.Extensions
{
    public static class ListExtensions
    {
        public static string Join<T>(this List<T> list, string delimiter = "") {
            var result = String.Join(delimiter, list);
            return result;
        }

    }
}
