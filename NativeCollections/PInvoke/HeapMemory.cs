using System;
using System.Runtime.InteropServices;

namespace NativeCollections.Internal
{
    [Flags]
    public enum HeapMemoryFlags : uint
    {
        /// <summary>
        /// No flags.
        /// </summary>
        NONE = 0x00000000,
        /// <summary>
        /// The system will raise an exception to indicate a function failure, such as an out-of-memory condition, instead of returning NULL.
        /// To ensure that exceptions are generated for all calls to this function, specify HEAP_GENERATE_EXCEPTIONS in the call to HeapCreate. 
        /// In this case, it is not necessary to additionally specify HEAP_GENERATE_EXCEPTIONS in this function call.
        /// </summary>
        HEAP_GENERATE_EXCEPTIONS = 0x00000004,
        /// <summary>
        /// Serialized access will not be used for this allocation.
        /// To ensure that serialized access is disabled for all calls to this function, specify HEAP_NO_SERIALIZE in the call to HeapCreate. 
        /// In this case, it is not necessary to additionally specify HEAP_NO_SERIALIZE in this function call. 
        /// This value should not be specified when accessing the process's default heap. 
        /// The system may create additional threads within the application's process, such as a CTRL+C handler, that simultaneously access the process's default heap.
        /// </summary>
        HEAP_NO_SERIALIZE = 0x00000001,
        /// <summary>
        /// All memory blocks that are allocated from this heap allow code execution, if the hardware enforces data execution prevention. 
        /// Use this flag heap in applications that run code from the heap. If HEAP_CREATE_ENABLE_EXECUTE is not specified and an application attempts to run code from a protected page, 
        /// the application receives an exception with the status code STATUS_ACCESS_VIOLATION.
        /// </summary>
        HEAP_CREATE_ENABLE_EXECUTE = 0x00040000,
        /// <summary>
        /// The allocated memory will be initialized to zero. Otherwise, the memory is not initialized to zero.
        /// <para></para>
        /// When reallocating if the reallocation request is for a larger size, the additional region of memory beyond the original size be initialized to zero. 
        /// The contents of the memory block up to its original size are unaffected.
        /// </summary>
        HEAP_ZERO_MEMORY = 0x00000008,
        /// <summary>
        /// There can be no movement when reallocating a memory block. If this value is not specified, the function may move the block to a new location. 
        /// If this value is specified and the block cannot be resized without moving, the function fails, leaving the original memory block unchanged.
        /// </summary>
        HEAP_REALLOC_IN_PLACE_ONLY = 0x00000010
    }

    unsafe public static class HeapMemory
    {
        /// <summary>
        /// Retrieves a handle to the default heap of the calling process. This handle can then be used in subsequent calls to the heap functions.
        /// </summary>
        /// <returns>If the function succeeds, the return value is a handle to the calling process's heap.
        /// If the function fails, the return value is NULL.To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll")]
        public static extern void* GetProcessHeap();

        /// <summary>
        /// Creates a private heap object that can be used by the calling process. 
        /// The function reserves space in the virtual address space of the process and allocates physical storage for a specified initial portion of this block.
        /// </summary>
        /// <param name="flags">The heap allocation options. These options affect subsequent access to the new heap through calls to the heap functions. This parameter can be 0 or any of <see cref="HeapMemoryFlags"/>.</param>
        /// <param name="initialSize">The initial size of the heap, in bytes. This value determines the initial amount of memory that is committed for the heap. 
        /// The value is rounded up to a multiple of the system page size. The value must be smaller than maxSize.</param>
        /// <param name="maxSize">The maximum size of the heap, in bytes. 
        /// The HeapCreate function rounds maxSize up to a multiple of the system page size and then reserves a block of that size in the process's virtual address space for the heap. 
        /// If allocation requests made by the HeapAlloc or HeapReAlloc functions exceed the size specified by initialSize, 
        /// the system commits additional pages of memory for the heap, up to the heap's maximum size.</param>
        /// <returns>If the function succeeds, the return value is a handle to the newly created heap.
        /// If the function fails, the return value is NULL.To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll")]
        public static extern uint HeapCreate(HeapMemoryFlags flags, uint initialSize, uint maxSize);

        /// <summary>
        /// Destroys the specified heap object. It decommits and releases all the pages of a private heap object, and it invalidates the handle to the heap.
        /// </summary>
        /// <param name="head">A handle to the heap to be destroyed. 
        /// This handle is returned by the HeapCreate function. Do not use the handle to the process heap returned by the GetProcessHeap function.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll")]
        public static extern bool HeapDestroy(void* head);

        /// <summary>
        /// Allocates a block of memory from a heap. The allocated memory is not movable.
        /// </summary>
        /// <param name="heap">A handle to the heap from which the memory will be allocated. This handle is returned by the HeapCreate or GetProcessHeap function.</param>
        /// <param name="flags">The heap allocation options. Specifying any of these values will override the corresponding value specified when the heap was created with HeapCreate. 
        /// This parameter can be one or more of the following values.</param>
        /// <param name="totalBytes">The number of bytes to be allocated.</param>
        /// <returns>If the function succeeds, the return value is a pointer to the allocated memory block.
        /// If the function fails and you have not specified HEAP_GENERATE_EXCEPTIONS, the return value is NULL.
        /// If the function fails and you have specified HEAP_GENERATE_EXCEPTIONS, the function may generate either of the exceptions listed in the following table.
        /// The particular exception depends upon the nature of the heap corruption.For more information, see GetExceptionCode.</returns>
        [DllImport("kernel32.dll")]
        public static extern void* HeapAlloc(void* heap, HeapMemoryFlags flags, uint totalBytes);

        /// <summary>
        /// Reallocates a block of memory from a heap. This function enables you to resize a memory block and change other memory block properties. The allocated memory is not movable.
        /// </summary>
        /// <param name="heap">A handle to the heap from which the memory is to be reallocated. This handle is a returned by either the HeapCreate or GetProcessHeap function.</param>
        /// <param name="flags">The heap reallocation options. Specifying a value overrides the corresponding value specified in the flOptions parameter when the heap was created by using the HeapCreate function.</param>
        /// <param name="pointer">A pointer to the block of memory that the function reallocates. This pointer is returned by an earlier call to the HeapAlloc or HeapReAlloc function.</param>
        /// <param name="totalBytes">The new size of the memory block, in bytes. A memory block's size can be increased or decreased by using this function.</param>
        /// <returns>If the function succeeds, the return value is a pointer to the reallocated memory block.
        /// If the function fails and you have not specified HEAP_GENERATE_EXCEPTIONS, the return value is NULL.
        /// If the function fails and you have specified HEAP_GENERATE_EXCEPTIONS, the function may generate either of the exceptions listed in the following table.For more information, see GetExceptionCode.</returns>
        [DllImport("kernel32.dll")]
        public static extern void* HeapReAlloc(void* heap, HeapMemoryFlags flags, void* pointer, uint totalBytes);

        /// <summary>
        /// Frees a memory block allocated from a heap by the HeapAlloc or HeapReAlloc function.
        /// </summary>
        /// <param name="heap">A handle to the heap whose memory block is to be freed. This handle is returned by either the HeapCreate or GetProcessHeap function.</param>
        /// <param name="flags">The heap free options.</param>
        /// <param name="pointer">A pointer to the memory block to be freed. This pointer is returned by the HeapAlloc or HeapReAlloc function. If this pointer is NULL, the behavior is undefined.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.An application can call GetLastError for extended error information.</returns>
        [DllImport("kernel32.dll")]
        public static extern bool HeapFree(void* heap, HeapMemoryFlags flags, void* pointer);

        /// <summary>
        /// Retrieves the size of a memory block allocated from a heap by the HeapAlloc or HeapReAlloc function.
        /// </summary>
        /// <param name="heap">A handle to the heap in which the memory block resides. This handle is returned by either the HeapCreate or GetProcessHeap function.</param>
        /// <param name="flags">The heap size options. Specifying the following value overrides the corresponding value specified in the flOptions parameter when the heap was created by using the HeapCreate function.</param>
        /// <param name="pointer">A pointer to the memory block whose size the function will obtain. This is a pointer returned by the HeapAlloc or HeapReAlloc function.
        /// The memory block must be from the heap specified by the hHeap parameter.</param>
        /// <returns>If the function succeeds, the return value is the requested size of the allocated memory block, in bytes.
        /// If the function fails, the return value is (SIZE_T)-1. The function does not call SetLastError. An application cannot call GetLastError for extended error information.</returns>
        [DllImport("kernel32.dll")]
        public static extern uint HeapSize(void* heap, HeapMemoryFlags flags, void* pointer);

        /// <summary>
        /// Returns the size of the largest committed free block in the specified heap. If the Disable heap coalesce on free global flag is set, this function also coalesces adjacent free blocks of memory in the heap.
        /// </summary>
        /// <param name="heap">A handle to the heap. This handle is returned by either the HeapCreate or GetProcessHeap function.</param>
        /// <param name="flags">The heap access options. This parameter can be the following value.</param>
        /// <returns>If the function succeeds, the return value is the size of the largest committed free block in the heap, in bytes.
        /// If the function fails, the return value is zero.To get extended error information, call GetLastError.
        /// In the unlikely case that there is absolutely no space available in the heap, the function return value is zero, and GetLastError returns the value NO_ERROR.</returns>
        [DllImport("kernel32.dll")]
        public static extern void* HeapCompact(void* heap, HeapMemoryFlags flags);

        /// <summary>
        /// Attempts to acquire the critical section object, or lock, that is associated with a specified heap.
        /// </summary>
        /// <param name="head">A handle to the heap to be locked. This handle is returned by either the HeapCreate or GetProcessHeap function.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll")]
        public static extern bool HeapLock (void* head);

        /// <summary>
        /// Releases ownership of the critical section object, or lock, that is associated with a specified heap. It reverses the action of the HeapLock function.
        /// </summary>
        /// <param name="head">A handle to the heap to be unlocked. This handle is returned by either the HeapCreate or GetProcessHeap function.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll")]
        public static extern bool HeapUnLock(void* head);
    }
}
