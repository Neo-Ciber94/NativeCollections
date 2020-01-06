## NativeCollections

A library inspired in ```Unity.Collections``` that provides a set of containers that 
make use of unmanaged memory for hold their elements.

The main purpose is to provide containers that produces <i>zero allocations</i>
and call ```Dispose``` after use.

------------------------

### This libray provides the follow containers:
```csharp
// A fixed length collection
struct NativeArray<T> where T: unmanaged

// A dynamic size collection
struct NativeList<T> where T: unmanaged

// A dynamic size collection of differents items
struct NativeSet<T> where T: unmanaged

// A dynamic size FILO (first-in last-out) collection
struct NativeStack<T> where T: unmanaged

// A dynamic size FIFO (first-in first-out) collection
struct NativeQueue<T> where T: unmanaged

// A dynamic size double-ended queue  collection
struct NativeDeque<T> where T: unmanaged

// A dynamic size key-value collection
struct NativeMap<TKey,TValue> where TKey: unmanaged where TValue: unmanaged

// A dynamic size ordered key-value collection
struct NativeSortedMap<TKey, TValue>  where TKey: unmanaged where TValue: unmanaged

// A fixed size collection of 'char'
struct NativeString
```

### Others:
```csharp
// Provides LINQ utilities
readonly ref struct NativeQuery<T>

// A view to unmanaged memory
readonly ref struct NativeSlice<T>

// A reference to a value
ref struct ByReference<T>

// A read-only reference to a value
readonly ref struct ByReadOnlyReferene<T>
```

Each container inherit from ```INativeContainer``` and ```IDisposable```.

```csharp
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
// Creates a new NativeList<T> with 'using' of C#8
// when 'list' goes out of scope 'Dispose' will be called.
using NativeList<int> list = new NativeList<int>(4);
list.Add(2);
list.Add(4);
list.Add(6);
list.Add(8);
```

