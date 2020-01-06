using System;

namespace NativeCollections.Utility
{
    /// <summary>
    /// Indicates that the method will dispose the instance after be called.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class DisposeAfterCallAttribute : Attribute { }
}
