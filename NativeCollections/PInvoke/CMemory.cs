using System.Runtime.InteropServices;

namespace NativeCollections.PInvoke
{
    /// <summary>
    /// Options for memory allocation.
    /// </summary>
    public enum CMemoryFlags : uint
    {
        /// <summary>
        /// The memory will be unitializated
        /// </summary>
        UNITIALIZATED,
        /// <summary>
        /// The memory will be initializated with 0.
        /// </summary>
        ZERO_MEMORY
    }

    unsafe public static class CMemory
    {
        /// <summary>
        /// Allocates memory on the heap. Equlivalent to C++ <c>malloc(size_t)</c>.
        /// </summary>
        /// <param name="size">The number of bytes to allocate.</param>
        /// <param name="flags">The allocation options.</param>
        [DllImport("NativeCollectionsCpp.dll")]
        public static extern void* Malloc(uint size, CMemoryFlags flags);

        /// <summary>
        /// Reallocates the memory of the specified memory block. Equivalent to C++ <c>realloc(void*, size_t)</c>.
        /// </summary>
        /// <param name="pointer">The pointer to the memory block.</param>
        /// <param name="size">The new size in bytes.</param>
        /// <param name="flags">The allocation options.</param>
        [DllImport("NativeCollectionsCpp.dll")]
        public static extern void* Realloc(void* pointer, uint size, CMemoryFlags flags);

        /// <summary>
        /// Releases the allocated resources. Equivalent to C++ <c>free(void*)</c>.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        [DllImport("NativeCollectionsCpp.dll")]
        public static extern void* Free(void* pointer);
    }
}
