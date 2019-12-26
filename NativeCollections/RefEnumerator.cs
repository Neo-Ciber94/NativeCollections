using System;
using System.Runtime.CompilerServices;

namespace NativeCollections
{
    unsafe public ref struct RefEnumerator<T>
    {
        private void* _pointer;
        private int _length;
        private int _index;

        public RefEnumerator(void* pointer, int length)
        {
            _pointer = pointer;
            _length = length;
            _index = -1;
        }

        public RefEnumerator(T[] array)
        {
            _pointer = array.Length == 0? null: Unsafe.AsPointer(ref array[0]);
            _length = 0;
            _index = -1;
        }

        public ref T Current
        {
            get
            {
                if (_index < 0 || _index > _length)
                    throw new ArgumentOutOfRangeException("index", _index.ToString());

                ref T pointer = ref Unsafe.AsRef<T>(_pointer);
                return ref Unsafe.Add(ref pointer, _index);
            }
        }

        public void Dispose()
        {
            if (_pointer == null)
                return;

            _pointer = null;
            _length = 0;
            _index = 0;
        }

        public bool MoveNext()
        {
            int i = _index + 1;
            if(i < _length)
            {
                _index = i;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _index = -1;
        }
    }
}
