namespace NativeCollections
{
    public delegate void RefAction<T>(ref T value);

    public delegate void RefIndexedAction<T>(ref T value, int index);
}