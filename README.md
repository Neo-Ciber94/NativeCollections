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
### Purpose:
NativeCollections offers containers that make use of unmanaged memory who is provide by an ``Allocator``.

The containers are constrained to only store ``ValueTypes``that don't contains
reference type on its fields.

At the time all the memory used for a container must be free manually
or by declaring the container with ``using``.

As all the containers use unmanaged memory they produce zero allocations,
thus the Garbage Collection pressure is reduce, but keep in mind that
that this allocations could be slower or faster 
than those that relied in GC depending the allocator used. 

Here some benchmark of ``List<T>`` vs ``NativeList<T>``, some optimization could be done in special with small
allocations, by default all containers use ``DefaultHeapAllocator`` which
make use of the *HeapAllocator* of ``kernel32.dll``.

|     Method | ListSize |          Mean |         Error |       StdDev |           Min |           Max | Ratio | RatioSD |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|----------- |--------- |--------------:|--------------:|-------------:|--------------:|--------------:|------:|--------:|---------:|---------:|---------:|----------:|
| NativeList |       10 |      72.41 ns |     10.244 ns |     0.562 ns |      71.83 ns |      72.94 ns |  6.22 |    0.03 |        - |        - |        - |         - |
|       List |       10 |      11.64 ns |      2.729 ns |     0.150 ns |      11.50 ns |      11.80 ns |  1.00 |    0.00 |   0.0229 |        - |        - |      96 B |
|            |          |               |               |              |               |               |       |         |          |          |          |           |
| NativeList |      100 |     553.38 ns |    466.290 ns |    25.559 ns |     537.25 ns |     582.84 ns | 17.69 |    1.75 |        - |        - |        - |         - |
|       List |      100 |      31.45 ns |     50.273 ns |     2.756 ns |      28.80 ns |      34.30 ns |  1.00 |    0.00 |   0.1090 |        - |        - |     456 B |
|            |          |               |               |              |               |               |       |         |          |          |          |           |
| NativeList |     1000 |     150.16 ns |     26.022 ns |     1.426 ns |     148.86 ns |     151.68 ns |  0.64 |    0.03 |        - |        - |        - |         - |
|       List |     1000 |     234.37 ns |    195.939 ns |    10.740 ns |     226.89 ns |     246.67 ns |  1.00 |    0.00 |   0.9689 |        - |        - |    4056 B |
|            |          |               |               |              |               |               |       |         |          |          |          |           |
| NativeList |    10000 |   1,023.86 ns |    203.408 ns |    11.149 ns |   1,012.66 ns |   1,034.96 ns |  0.51 |    0.02 |        - |        - |        - |         - |
|       List |    10000 |   2,020.51 ns |  1,777.895 ns |    97.452 ns |   1,908.96 ns |   2,089.12 ns |  1.00 |    0.00 |   9.5215 |   1.1864 |        - |   40056 B |
|            |          |               |               |              |               |               |       |         |          |          |          |           |
| NativeList |   100000 |  25,303.80 ns |  3,472.495 ns |   190.339 ns |  25,191.98 ns |  25,523.57 ns |  1.43 |    0.10 |        - |        - |        - |         - |
|       List |   100000 |  17,771.31 ns | 22,109.278 ns | 1,211.884 ns |  17,053.69 ns |  19,170.52 ns |  1.00 |    0.00 | 124.9695 | 124.9695 | 124.9695 |  400056 B |
|            |          |               |               |              |               |               |       |         |          |          |          |           |
| NativeList |  1000000 |  59,913.78 ns | 13,118.763 ns |   719.084 ns |  59,097.39 ns |  60,453.19 ns |  0.52 |    0.02 |        - |        - |        - |         - |
|       List |  1000000 | 114,757.36 ns | 74,610.167 ns | 4,089.635 ns | 111,657.79 ns | 119,392.53 ns |  1.00 |    0.00 | 999.7559 | 999.7559 | 999.7559 | 4000056 B |