namespace NativeCollections
{
    public readonly struct IndexedValue<T>
    {
        public T Value { get; }
        public int Index { get; }

        public IndexedValue(T value, int index)
        {
            Value = value;
            Index = index;
        }

        public void Deconstruct(out T value, out int index)
        {
            value = Value;
            index = Index;
        }

        public override string ToString()
        {
            return $"({Value}, {Index})";
        }
    }
}
