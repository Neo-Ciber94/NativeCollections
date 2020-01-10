using NUnit.Framework;
using NativeCollections.Allocators;
using System;
using System.Collections.Generic;
using System.Text;
using NativeCollections.Allocators.Internal;

namespace NativeCollections.Allocators.Tests
{
    internal sealed class ForwardAllocator : Allocator, IDisposable
    {
        private readonly Allocator _allocator;

        public ForwardAllocator(Allocator allocator) : base(true)
        {
            _allocator = allocator;
        }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            return _allocator.Allocate(elementCount, elementSize, initMemory);
        }

        public override unsafe void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            return _allocator.Reallocate(pointer, elementCount, elementSize, initMemory);
        }

        public override unsafe void Free(void* pointer)
        {
            _allocator.Free(pointer);
        }

        public void Dispose()
        {
            if(_allocator is IDisposable disposable)
            {
                disposable.Dispose();
            }

            Dispose(true);
        }
    }

    [TestFixture()]
    unsafe public class AllocatorTests
    {
        [Test()]
        public void DefaultCAllocatorTest()
        {
            DefaultCppAllocator allocator = DefaultCppAllocator.Instance;
            Assert.AreSame(allocator, Allocator.GetAllocatorByID(allocator.ID));
            Assert.IsTrue(Allocator.IsCached(allocator));

            int* p = allocator.Allocate<int>(4);

            for(int i = 0; i < 4; i++)
            {
                p[i] = i + 1;
            }

            Assert.AreEqual(1, p[0]);
            Assert.AreEqual(2, p[1]);
            Assert.AreEqual(3, p[2]);
            Assert.AreEqual(4, p[3]);

            p = allocator.Reallocate(p, 6);
            Assert.AreEqual(1, p[0]);
            Assert.AreEqual(2, p[1]);
            Assert.AreEqual(3, p[2]);
            Assert.AreEqual(4, p[3]);
            Assert.AreEqual(0, p[4]);
            Assert.AreEqual(0, p[5]);

            allocator.Free(p);
        }

        [Test()]
        public void DefaultHeapAllocatorTest()
        {
            DefaultHeapAllocator allocator = DefaultHeapAllocator.Instance;
            Assert.AreSame(allocator, Allocator.GetAllocatorByID(allocator.ID));
            Assert.IsTrue(Allocator.IsCached(allocator));

            int* p = allocator.Allocate<int>(4);

            for (int i = 0; i < 4; i++)
            {
                p[i] = i + 1;
            }

            Assert.AreEqual(4 * 4, allocator.SizeOf(p));

            Assert.AreEqual(1, p[0]);
            Assert.AreEqual(2, p[1]);
            Assert.AreEqual(3, p[2]);
            Assert.AreEqual(4, p[3]);

            p = allocator.Reallocate(p, 6);
            Assert.AreEqual(6 * 4, allocator.SizeOf(p));

            Assert.AreEqual(1, p[0]);
            Assert.AreEqual(2, p[1]);
            Assert.AreEqual(3, p[2]);
            Assert.AreEqual(4, p[3]);
            Assert.AreEqual(0, p[4]);
            Assert.AreEqual(0, p[5]);

            allocator.Free(p);
        }

        [Test()]
        public void DefaultLocalAllocatorTest()
        {
            DefaultLocalAllocator allocator = DefaultLocalAllocator.Instance;
            Assert.AreSame(allocator, Allocator.GetAllocatorByID(allocator.ID));
            Assert.IsTrue(Allocator.IsCached(allocator));

            int* p = allocator.Allocate<int>(4);

            for (int i = 0; i < 4; i++)
            {
                p[i] = i + 1;
            }

            Assert.AreEqual(4 * 4, allocator.SizeOf(p));

            Assert.AreEqual(1, p[0]);
            Assert.AreEqual(2, p[1]);
            Assert.AreEqual(3, p[2]);
            Assert.AreEqual(4, p[3]);

            p = allocator.Reallocate(p, 6);
            Assert.AreEqual(6 * 4, allocator.SizeOf(p));

            Assert.AreEqual(1, p[0]);
            Assert.AreEqual(2, p[1]);
            Assert.AreEqual(3, p[2]);
            Assert.AreEqual(4, p[3]);
            Assert.AreEqual(0, p[4]);
            Assert.AreEqual(0, p[5]);

            allocator.Free(p);
        }

        [Test()]
        public void ArenaAllocatorTest()
        {
            ArenaAllocator allocator = new ArenaAllocator(1000);
            Assert.AreSame(allocator, Allocator.GetAllocatorByID(allocator.ID));
            Assert.IsTrue(Allocator.IsCached(allocator));

            int* p = allocator.Allocate<int>(4);

            for (int i = 0; i < 4; i++)
            {
                p[i] = i + 1;
            }

            Assert.AreEqual(1, p[0]);
            Assert.AreEqual(2, p[1]);
            Assert.AreEqual(3, p[2]);
            Assert.AreEqual(4, p[3]);

            p = allocator.Reallocate(p, 6);

            Assert.AreEqual(1, p[0]);
            Assert.AreEqual(2, p[1]);
            Assert.AreEqual(3, p[2]);
            Assert.AreEqual(4, p[3]);
            Assert.AreEqual(0, p[4]);
            Assert.AreEqual(0, p[5]);

            allocator.Free(p);
            allocator.Dispose();
            Assert.IsFalse(Allocator.IsCached(allocator));
        }

        [Test()]
        public void StackAllocatorTest()
        {
            StackAllocator allocator = new StackAllocator(1000);
            Assert.AreSame(allocator, Allocator.GetAllocatorByID(allocator.ID));
            Assert.IsTrue(Allocator.IsCached(allocator));

            int* p = allocator.Allocate<int>(4);

            for (int i = 0; i < 4; i++)
            {
                p[i] = i + 1;
            }

            Assert.AreEqual(1, p[0]);
            Assert.AreEqual(2, p[1]);
            Assert.AreEqual(3, p[2]);
            Assert.AreEqual(4, p[3]);

            p = allocator.Reallocate(p, 6);

            Assert.AreEqual(1, p[0]);
            Assert.AreEqual(2, p[1]);
            Assert.AreEqual(3, p[2]);
            Assert.AreEqual(4, p[3]);
            Assert.AreEqual(0, p[4]);
            Assert.AreEqual(0, p[5]);

            allocator.Free(p);
            allocator.Dispose();
            Assert.IsFalse(Allocator.IsCached(allocator));
        }

        [Test()]
        public void FixedMemoryPoolAllocatorTest()
        {
            using FixedMemoryPoolAllocator allocator = new FixedMemoryPoolAllocator(10, 400);

            unsafe
            {
                int* p = allocator.Allocate<int>(4);
                Assert.AreEqual(0, p[0]);
                Assert.AreEqual(0, p[1]);
                Assert.AreEqual(0, p[2]);
                Assert.AreEqual(0, p[3]);

                for(int i = 0; i < 4; i++)
                {
                    p[i] = i + 1;
                }

                Assert.AreEqual(1, p[0]);
                Assert.AreEqual(2, p[1]);
                Assert.AreEqual(3, p[2]);
                Assert.AreEqual(4, p[3]);

                allocator.Free(p);

                Assert.Throws<OutOfMemoryException>(() => allocator.Allocate(600));
            }
        }

        [Test()]
        public void FixedMemoryPoolAllocatorTest1()
        {
            using FixedMemoryPoolAllocator allocator = new FixedMemoryPoolAllocator(10, 400);

            unsafe
            {
                IntPtr* ptrs = stackalloc IntPtr[10];

                for(int i = 0; i < 10; i++)
                {
                    ptrs[i] = (IntPtr)allocator.Allocate(100);
                }

                Assert.Throws<OutOfMemoryException>(() =>
                {
                    var p = allocator.Allocate<IntPtr>(5);
                });

                for(int i = 0; i < 10; i++)
                {
                    allocator.Free(ptrs[i].ToPointer());
                }
            }
        }

        [Test()]
        public void GetAllocatorByIDTest()
        {
            ForwardAllocator allocator = new ForwardAllocator(Allocator.Default);
            Assert.IsTrue(Allocator.IsCached(allocator));
            Assert.IsTrue(Allocator.IsCached(allocator.ID));

            allocator.Dispose();

            Assert.IsFalse(Allocator.IsCached(allocator));
            Assert.IsFalse(Allocator.IsCached(allocator.ID));
        }

        [Test()]
        [Order(10000)]
        public void AllocatorCacheLimitTest()
        {
            ForwardAllocator[] allocators = new ForwardAllocator[20];
            for(int i = 0; i < allocators.Length; i++)
            {
                allocators[i] = new ForwardAllocator(Allocator.Default);
            }

            var allocator1 = new ForwardAllocator(Allocator.Default);
            Assert.IsFalse(Allocator.IsCached(allocator1));
            Assert.IsFalse(Allocator.IsCached(allocator1.ID));
            allocator1.Dispose();

            for (int i = 0; i < allocators.Length; i++)
            {
                allocators[i].Dispose();
            }

            var allocator2 = new ForwardAllocator(Allocator.Default);
            Assert.IsTrue(Allocator.IsCached(allocator2));
            Assert.IsTrue(Allocator.IsCached(allocator2.ID));
        }

        [Test()]
        public void AllocatorBorrowTest()
        {
            Allocator allocator = Allocator.Default;

            allocator.Borrow<int>(100, (span, length) =>
            {
                Assert.AreEqual(100, span.Length);
                
                for(int i = 0; i < length; i++)
                {
                    Assert.AreEqual(0, span[i]);
                }

                span.Fill(2);

                for (int i = 0; i < length; i++)
                {
                    Assert.AreEqual(2, span[i]);
                }
            });
        }

        [Test()]
        public void AllocatorBorrowTest1()
        {
            Allocator allocator = Allocator.Default;

            allocator.Borrow<int>(100, stackAlloc: true, (span, length) =>
            {
                Assert.AreEqual(100, span.Length);

                for (int i = 0; i < length; i++)
                {
                    Assert.AreEqual(0, span[i]);
                }

                span.Fill(2);

                for (int i = 0; i < length; i++)
                {
                    Assert.AreEqual(2, span[i]);
                }
            });
        }
    }
}