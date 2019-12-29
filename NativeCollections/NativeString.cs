using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NativeCollections.Allocators;

namespace NativeCollections
{
    unsafe public struct NativeString : INativeContainer<char>, IDisposable, IEquatable<NativeString>, IEquatable<string>, IComparable<NativeString>
    {
        private char* _buffer;
        private int _length;

        public NativeString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                this = default;
            }
            else
            {
                int length = str.Length;
                _buffer = Allocator.Default.Allocate<char>(length);
                _length = length;

                fixed(char* pointer = str)
                {
                    Unsafe.CopyBlock(_buffer, pointer, (uint)(sizeof(char) * length));
                }
            }
        }

        public NativeString(Span<char> span)
        {
            if (span.IsEmpty)
            {
                this = default;
            }
            else
            {
                int length = span.Length;
                _buffer = Allocator.Default.Allocate<char>(length);
                _length = length;

                char* pointer = (char*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
                Unsafe.CopyBlock(_buffer, pointer, (uint)(sizeof(char) * length));
            }
        }

        public NativeString(ReadOnlySpan<char> span)
        {
            if (span.IsEmpty)
            {
                this = default;
            }
            else
            {
                int length = span.Length;
                _buffer = Allocator.Default.Allocate<char>(length);
                _length = length;

                char* pointer = (char*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
                Unsafe.CopyBlock(_buffer, pointer, (uint)(sizeof(char) * length));
            }
        }

        public NativeString(char* pointer, int length, bool copy = false)
        {
            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            if (length <= 0)
                throw new ArgumentException("length should be greater than 0");

            if (copy)
            {
                _buffer = Allocator.Default.Allocate<char>(length);
                _length = length;
                Unsafe.CopyBlock(_buffer, pointer, (uint)(sizeof(char) * length));
            }
            else
            {
                _buffer = pointer;
                _length = length;
            }
        }

        public int Length => _length;

        public bool IsValid => _buffer != null;

        public bool IsEmpty => _length == 0;

        public readonly ref char this[int index]
        {
            get
            {
                if (index < 0 || index > _length)
                    throw new ArgumentOutOfRangeException(nameof(index), index.ToString());

                return ref _buffer[index];
            }
        }

        public void CopyTo(in Span<char> span, int destinationIndex, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty");

            if (_buffer == null)
                throw new InvalidOperationException("NativeString is invalid");

            if (destinationIndex < 0 || destinationIndex > span.Length)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex), destinationIndex.ToString());

            if (count < 0 || count > _length || count > (span.Length - destinationIndex))
                throw new ArgumentNullException(nameof(count), count.ToString());

            char* pointer = (char*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
            void* src = _buffer;
            void* dst = pointer + destinationIndex;
            Unsafe.CopyBlock(dst, src, (uint)(sizeof(char) * count));
        }
        
        public ReadOnlySpan<char> AsSpan()
        {
            return new ReadOnlySpan<char>(_buffer, _length);
        }

        public override string ToString()
        {
            if (_buffer == null)
                throw new InvalidOperationException("NativeString is invalid");

            return new string(_buffer, 0, _length);
        }

        public void Dispose()
        {
            if (_buffer == null)
                return;

            Allocator.Default.Free(_buffer);
            _buffer = null;
            _length = 0;
        }

        public NativeArray<char> ToNativeArrayAndDispose()
        {
            if (_buffer == null)
                throw new InvalidOperationException("NativeString is invalid");

            // NativeArray now owns this NativeString memory
            NativeArray<char> array = new NativeArray<char>(_buffer, _length);

            // No actual dispose, just invalidate this NativeString
            _buffer = null;
            _length = 0;

            return array;
        }

        public char[] ToCharArray()
        {
            char[] array = new char[_length];
            CopyTo(array, 0, _length);
            return array;
        }

        public override bool Equals(object? obj)
        {
            if (obj is string s)
            {
                return Equals(s);
            }

            return obj is NativeString str && Equals(str);
        }

        public bool Equals(NativeString other)
        {
            return AsSpan().SequenceEqual(other.AsSpan());
        }

        public bool Equals(string other)
        {
            return AsSpan().SequenceEqual(other.AsSpan());
        }

        public int CompareTo(NativeString other, StringComparison comparisonType)
        {
            return AsSpan().CompareTo(other.AsSpan(), comparisonType);
        }

        public int CompareTo(NativeString other)
        {
            return CompareTo(other, StringComparison.CurrentCulture);
        }

        public override int GetHashCode()
        {
            return (int)(((long)_buffer) >> 4);
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
    
        public struct Enumerator
        {
            private char* _buffer;
            private int _length;
            private int _index;

            public Enumerator(ref NativeString str)
            {
                _buffer = str._buffer;
                _length = str._length;
                _index = -1;
            }

            public readonly ref char Current
            {
                get
                {
                    if (_index < 0 || _index > _length)
                        throw new ArgumentOutOfRangeException(nameof(_index), _index.ToString());

                    if (_buffer == null)
                        throw new InvalidOperationException("Enumerator is invalid");

                    return ref _buffer[_index];
                }
            }

            public bool MoveNext()
            {
                int i = _index + 1;
                if(i < _length)
                {
                    _index = i;
                }
                return false;
            }

            public void Dispose()
            {
                if (_buffer == null)
                    return;

                _buffer = null;
                _length = 0;
                _index = 0;
            }
        }
    }
}
