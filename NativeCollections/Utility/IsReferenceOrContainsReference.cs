using System;
using System.Reflection;

namespace NativeCollections.Utility
{
    /// <summary>
    /// Holds a value to check if a value is or contains a reference value.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public static class IsReferenceOrContainsReference<T>
    {
        /// <summary>
        /// The value indicating if the type <see cref="T"/> is or contains a reference type, if false <see cref="T"/> is an unmanaged type.
        /// </summary>
        public static readonly bool Value;

        static IsReferenceOrContainsReference()
        {
            Value = Check(typeof(T));
        }

        /// <summary>
        /// Checks if the specified type contains or is a reference type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if contains or is a reference type or false if is an umnanaged type.</returns>
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
