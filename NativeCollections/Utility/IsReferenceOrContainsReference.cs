using System;
using System.Reflection;

namespace NativeCollections.Utility
{
    public static class IsReferenceOrContainsReference<T>
    {
        public static readonly bool Value;

        static IsReferenceOrContainsReference()
        {
            Value = Check(typeof(T));
        }

        public static bool Check(Type type)
        {
            if (type.IsClass || type.IsInterface || !type.IsValueType)
                return true;

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for(int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var fieldType = field.FieldType;
                if (Check(fieldType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
