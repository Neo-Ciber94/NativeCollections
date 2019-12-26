using System.Runtime.CompilerServices;

namespace NativeCollections
{
    unsafe public ref struct RefEnumerator<T>
    {
        private void* _pointer;
        private int _length;
        private int pos;

        public RefEnumerator(void* pointer, int length)
        {
            _pointer = pointer;
            _length = length;
            pos = -1;
        }

        public RefEnumerator(T[] array)
        {
            _pointer = array.Length == 0? null: Unsafe.AsPointer(ref array[0]);
            _length = 0;
            pos = 0;
        }

        public ref T Current
        {
            get
            {
                ref T pointer = ref Unsafe.AsRef<T>(_pointer);
                return ref Unsafe.Add(ref pointer, pos);
            }
        }

        public void Dispose()
        {
            if (_pointer == null)
                return;

            _pointer = null;
            _length = 0;
            pos = 0;
        }

        public bool MoveNext()
        {
            int i = pos + 1;
            if(i < _length)
            {
                pos = i;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            pos = 0;
        }
    }
}
