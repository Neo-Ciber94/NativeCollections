using System;
using System.Runtime.InteropServices;

namespace NativeCollections.Internal
{
    [Flags]
    public enum LocalMemoryFlags : uint
    {
        /// <summary>
        /// Allocates fixed memory. The return value is a pointer to the memory object
        /// </summary>
        LHND = 0x0042,
        /// <summary>
        /// Allocates fixed memory. The return value is a pointer to the memory object
        /// </summary>
        LMEM_FIXED = 0x0000,
        /// <summary>
        /// Allocates movable memory. Memory blocks are never moved in physical memory, but they can be moved within the default heap.
        /// The return value is a handle to the memory object. To translate the handle to a pointer, use the LocalLock function.
        /// This value cannot be combined with <see cref="LocalMemoryFlags.LMEM_FIXED"/>.
        /// <para></para>
        /// If reallocating it allocates fixed or movable memory.
        /// If the memory is a locked <see cref="LocalMemoryFlags.LMEM_MOVEABLE"/> memory block or 
        /// a <see cref="LocalMemoryFlags.LMEM_FIXED"/> memory block and this flag is not specified, the memory can only be reallocated in place.
        /// </summary>
        LMEM_MOVEABLE = 0x0002,
        /// <summary>
        /// Initializes memory contents to zero.
        /// <para></para>
        /// If reallocating it causes the additional memory contents to be initialized to zero if the memory object is growing in size.
        /// </summary>
        LMEM_ZEROINIT = 0x0040,
        /// <summary>
        /// Combines LMEM_FIXED and LMEM_ZEROINIT.
        /// </summary>
        LPTR = 0x0040,
        /// <summary>
        /// Bytes when calling <see cref="Kernel32LocalMemory.LocalReAlloc(void*, uint, LocalMemoryFlags)"/> will be ignored.
        /// </summary>
        LMEM_MODIFY = 0x0080,
        /// <summary>
        /// May be returned by <see cref="Kernel32LocalMemory.LocalFlags(void*)"/>
        /// </summary>
        LMEM_DISCARDED = 0x4000,
        /// <summary>
        /// May be returned by <see cref="Kernel32LocalMemory.LocalFlags(void*)"/>
        /// </summary>
        LMEM_LOCKCOUNT = 0x00FF,
        /// <summary>
        /// An invalid pointer.
        /// </summary>
        LMEM_INVALID_HANDLE = 0x8000,
    }

    unsafe public static class Kernel32LocalMemory
    {

        /// <summary>
        /// Allocates the specified number of bytes from the heap.
        /// </summary>
        /// <param name="flags">The memory allocation attributes. The default is the <see cref="LocalMemoryFlags.LMEM_FIXED"/> value. 
        /// This parameter can be one or more of the following values, except for the incompatible combinations that are specifically noted.</param>
        /// <param name="totalBytes">The number of bytes to allocate. 
        /// If this parameter is zero and the uFlags parameter specifies <see cref="LocalMemoryFlags.LMEM_MOVEABLE"/>, the function returns a handle to a memory object that is marked as discarded.</param>
        /// <returns>If the function succeeds, the return value is a handle to the newly allocated memory object.
        /// If the function fails, the return value is NULL. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
        [DllImport("kernel32.dll")]
        public static extern void* LocalAlloc(LocalMemoryFlags flags, uint totalBytes);

        /// <summary>
        /// Changes the size or the attributes of a specified local memory object. The size can increase or decrease.
        /// </summary>
        /// <param name="pointer">A handle to the local memory object to be reallocated. 
        /// This handle is returned by either:
        /// <para>- <see cref="LocalAlloc(LocalMemoryFlags, uint)"/></para>
        /// <para>- <see cref="LocalReAlloc(void*, uint, LocalMemoryFlags)"/></para>
        /// </param>
        /// <param name="totalBytes">The new size of the memory block, in bytes. If uFlags specifies <see cref="LocalMemoryFlags.LMEM_MODIFY"/>, this parameter is ignored.</param>
        /// <param name="flags">The reallocation options. If <see cref="LocalMemoryFlags.LMEM_MODIFY"/> is specified, 
        /// the function modifies the attributes of the memory object only (the totalBytes parameter is ignored.) 
        /// Otherwise, the function reallocates the memory object.
        /// You can optionally combine <see cref="LocalMemoryFlags.LMEM_MODIFY"/> with th<see cref="LocalMemoryFlags.LMEM_MOVEABLE"/> or <see cref="LocalMemoryFlags.LMEM_ZEROINIT"/>.</param>
        /// <returns>If the function succeeds, the return value is a handle to the reallocated memory object.
        /// If the function fails, the return value is NULL. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
        [DllImport("kernel32.dll")]
        public static extern void* LocalReAlloc(void* pointer, uint totalBytes, LocalMemoryFlags flags);

        /// <summary>
        /// Frees the specified local memory object and invalidates its handle.
        /// </summary>
        /// <param name="pointer">A handle to the local memory object. 
        /// This handle is returned by either:
        /// <para>- <see cref="LocalAlloc(LocalMemoryFlags, uint)"/></para>
        /// <para>- <see cref="LocalReAlloc(void*, uint, LocalMemoryFlags)"/></para>
        /// <para>- <see cref="LocalHandle(void*)"/></para>
        /// </param>
        /// <returns>If the function succeeds, the return value is NULL.
        /// If the function fails, the return value is NULL. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
        [DllImport("kernel32.dll")]
        public static extern void* LocalFree(void* pointer);

        /// <summary>
        /// Retrieves the current size of the specified local memory object, in bytes.
        /// </summary>
        /// <param name="pointer">A handle to the local memory object. This handle is returned by either:
        /// <para>- <see cref="LocalAlloc(LocalMemoryFlags, uint)"/></para>
        /// <para>- <see cref="LocalReAlloc(void*, uint, LocalMemoryFlags)"/></para>
        /// <para>- <see cref="LocalHandle(void*)"/></para>
        /// </param>
        /// <returns>If the function succeeds, the return value is the size of the specified local memory object, in bytes. 
        /// If the specified handle is not valid or if the object has been discarded, the return value is zero. 
        /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
        [DllImport("kernel32.dll")]
        public static extern uint LocalSize(void* pointer);

        /// <summary>
        /// Retrieves information about the specified local memory object.
        /// </summary>
        /// <param name="pointer">A handle to the local memory object. This handle is returned by either:
        /// <para>- <see cref="LocalAlloc(LocalMemoryFlags, uint)"/></para>
        /// <para>- <see cref="LocalReAlloc(void*, uint, LocalMemoryFlags)"/></para>
        /// <para>- <see cref="LocalHandle(void*)"/></para>
        /// </param>
        /// <returns>If the function succeeds, the return value specifies the allocation values and the lock count for the memory object.
        ///If the function fails, the return value is <see cref="LocalMemoryFlags.LMEM_INVALID_HANDLE"/>, indicating that the local handle is not valid.
        /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
        [DllImport("kernel32.dll")]
        public static extern LocalMemoryFlags LocalFlags(void* pointer);

        /// <summary>
        /// Retrieves the handle associated with the specified pointer to a local memory object.
        /// </summary>
        /// <param name="pointer">A pointer to the first byte of the local memory object. This pointer is returned by the <see cref="Kernel32LocalMemory.LocalLock(void*)"/> function.</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified local memory object.
        /// If the function fails, the return value is NULL.        
        /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
        [DllImport("kernel32.dll")]
        public static extern void* LocalHandle(void* pointer);

        /// <summary>
        /// Locks a local memory object and returns a pointer to the first byte of the object's memory block.
        /// </summary>
        /// <param name="pointer">A handle to the local memory object. This handle is returned by either:
        /// <para>- <see cref="LocalAlloc(LocalMemoryFlags, uint)"/></para>
        /// <para>- <see cref="LocalReAlloc(void*, uint, LocalMemoryFlags)"/></para>
        /// </param>
        /// <returns>If the function succeeds, the return value is a pointer to the first byte of the memory block.
        /// If the function fails, the return value is NULL.        
        /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
        [DllImport("kernel32.dll")]
        public static extern void* LocalLock(void* pointer);

        /// <summary>
        /// Decrements the lock count associated with a memory object that was allocated with <see cref="LocalMemoryFlags.LMEM_MOVEABLE"/>. 
        /// This function has no effect on memory objects allocated with <see cref="LocalMemoryFlags.LMEM_FIXED"/>.
        /// </summary>
        /// <param name="pointer">A handle to the local memory object. This handle is returned by either:
        /// <para>- <see cref="LocalAlloc(LocalMemoryFlags, uint)"/></para>
        /// <para>- <see cref="LocalReAlloc(void*, uint, LocalMemoryFlags)"/></para>
        /// </param>
        /// <returns>If the memory object is still locked after decrementing the lock count, the return value is nonzero. 
        /// If the memory object is unlocked after decrementing the lock count, the function returns zero and <see cref="Marshal.GetLastWin32Error"/> returns NO_ERROR.
        /// If the function fails, the return value is zero and <see cref="Marshal.GetLastWin32Error"/> returns a value other than NO_ERROR.</returns>
        [DllImport("kernel32.dll")]
        public static extern void* LocalUnLock(void* pointer);
    }
}
