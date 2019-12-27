using System;
using System.Text;

namespace NativeCollections.Utility
{
    public static class StringBuilderCache
    {
        private const int DefaultCapacity = 32;
        private const int MaxCapacity = 360;

        [ThreadStatic]
        private static StringBuilder? _cachedInstance;

        public static StringBuilder Acquire(int capacity = DefaultCapacity)
        {
            if(capacity <= MaxCapacity)
            {
                StringBuilder stringBuilder = _cachedInstance!;
                if(stringBuilder != null && capacity <= stringBuilder.Capacity)
                {
                    _cachedInstance = null;
                    stringBuilder.Clear();
                    return stringBuilder;
                }
            }

            return new StringBuilder(capacity);
        }

        public static void Release(ref StringBuilder? stringBuilder)
        {
            if(stringBuilder != null && stringBuilder.Capacity <= MaxCapacity)
            {
                _cachedInstance = stringBuilder;
                stringBuilder = null;
            }
        }

        public static string ToStringAndRelease(ref StringBuilder? stringBuilder)
        {
            string result = stringBuilder!.ToString();
            Release(ref stringBuilder);
            return result;
        }
    }
}
