using System;
using System.Collections.Generic;

namespace NativeCollections.Utility
{
    internal static class SpanHelper
    {
        public static bool Contains<T>(in Span<T> span, T value)
        {
            var comparer = EqualityComparer<T>.Default;

            foreach(ref T e in span)
            {
                if(comparer.Equals(e, value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
