## NativeCollections

A library inspired in ```Unity.Collections``` that provides a set of containers that 
make use of unmanaged memory for hold their elements.

The main purpose is to provide containers that produces <i>zero allocations</i>
and must be ```Dispose``` after use.

------------------------

### This libray provides the follow containers:
```csharp
NativeArray<T>
NativeList<T>
NativeSet<T>
NativeStack<T>
NativeQueue<T>
NativeDeque<T>
NativeMap<TKey,TValue>
NativeSortedMap<TKey, TValue>
```

Each container inherite from ```INativeContainer``` and ```IDisposable```.

```chsarp
public interface INativeContainer<T>
{
	public int Length { get; }
	public bool IsEmpty { get; }
	public int IsValid { get; }

	public void CopyTo(in Span<T> span, int index, int count);
}
```

------------------------

### Examples:

#### Example1
```csharp
// Creates a new NativeArray<T>
NativeArray<int> array = new NativeArray<int>(4);
array[0] = 2;
array[1] = 4;
array[2] = 6;
array[3] = 8;

// Each container must be disposed after use
array.Dispose();
```

#### Example2
```csharp
// Creates a new NativeArray<T>, by with 'using' of C#8
// when 'array' goes out of scope 'Dispose' will be called.
using NativeArray<int> array = new NativeArray<int>(4);
array[0] = 2;
array[1] = 4;
array[2] = 6;
array[3] = 8;
```

