using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeCollections.Utility
{

    unsafe public struct NativeString : IDisposable, IEquatable<NativeString>, IEquatable<string>
    {
        private char* _value;

        public NativeString(string str)
        {
            fixed (char* ptr = str)
            {
                int byteCount = sizeof(char) * str.Length;
                _value = (char*)Marshal.AllocHGlobal(byteCount);
                Length = str.Length;
                Unsafe.CopyBlock(_value, ptr, (uint)byteCount);
            }
        }

        public int Length { get; private set; }

        public Span<char> AsSpan()
        {
            if (Length == 0)
                return Span<char>.Empty;

            return new Span<char>(_value, Length);
        }

        public void Dispose()
        {
            if (_value == null)
                return;

            Marshal.FreeHGlobal((IntPtr)_value);
            _value = null;
            Length = 0;
        }

        public override string ToString()
        {
            return _value == null ? "null" : new string(_value, 0, Length);
        }

        public override bool Equals(object? obj)
        {
            if(obj is string s)
            {
                return Equals(s);
            }

            return obj is NativeString @string && Equals(@string);
        }

        public bool Equals([AllowNull] NativeString other)
        {
            return AsSpan().SequenceEqual(other.AsSpan());
        }

        public bool Equals([AllowNull] string other)
        {
            return AsSpan().SequenceEqual(other.AsSpan());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ToString());
        }

        public static implicit operator NativeString(string value)
        {
            return new NativeString(value);
        }

        public static bool operator ==(NativeString left, NativeString right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NativeString left, NativeString right)
        {
            return !(left == right);
        }

        public static bool operator ==(NativeString left, string right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NativeString left, string right)
        {
            return !(left == right);
        }

        public static bool operator ==(string left, NativeString right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(string left, NativeString right)
        {
            return !(left == right);
        }
    }
}
