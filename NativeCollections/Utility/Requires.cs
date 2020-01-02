using System;
using System.Diagnostics;

namespace NativeCollections.Utility
{
    public static class Requires
    {
        [Conditional("DEBUG")]
        public static void IsNull(object? obj, string? msg = null)
        {
            if (obj != null)
            {
                throw new ArgumentNullException(msg);
            }
        }

        [Conditional("DEBUG")]
        public static void NotNull(object? obj, string? msg = null)
        {
            if(obj == null)
            {
                throw new ArgumentNullException(msg);
            }
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, string? msg = null)
        {
            if (!condition)
            {
                throw new Exception(msg?? "The condition is not true");
            }
        }

        [Conditional("DEBUG")]
        public static void IsFalse(bool condition, string? msg = null)
        {
            if (condition)
            {
                throw new Exception(msg?? "The condition is not false");
            }
        }
    }
}
