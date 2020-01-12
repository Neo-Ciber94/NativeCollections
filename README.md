## NativeCollections <span style="color:red">[Experimental]</span>

A library inspired in ```Unity.Collections``` that provides a set of containers that 
make use of unmanaged memory for hold their elements.

The main purpose is to provide containers that produces *zero allocations*
and call ```Dispose``` after use.

------------------------

### This libray provides the follow containers:


| Collection  | Description										   |
| ----------- | -------------------------------------------------- |
| NativeArray | A fixed length collection						   |
| NativeList  | A dynamic size collection						   |
| NativeMap   | A dynamic size key-value collection				   |
| NativeSet	  | A dynamic size collection of different items	   |
| NativeStack | A dynamic size FILO (first-in last-out) collection |
| NativeQueue | A dynamic size FIFO (first-in first-out) collection|
| NativeDeque | A dynamic size double-ended queue collection	   |
| NativeSortedSet | A dynamic size sorted collection of different items |
| NativeSortedMap | A dynamic size sorted key-value collection     |
| NativeString | A fixed size colleciton of ``char``			   |


### Others:
| Collection  | Description										   |
| ----------- | -------------------------------------------------- |
| NativeQuery | Provides methods for query over a NativeContainer  |
| NativeSlice | A view to a contiguous region of memory			   |
| ByReference | A reference to a value							   |
| ByReadOnlyReference | A read-only reference to a value	       |

Each container inherit from ```INativeContainer<T>``` and ```IDisposable```.

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