using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeQueueTests
    {
        [Test()]
        public void NativeQueueTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            Assert.IsTrue(queue.IsValid);
            Assert.IsTrue(queue.IsEmpty);
            Assert.AreEqual(0, queue.Length);
            Assert.AreEqual(4, queue.Capacity);
        }

        [Test()]
        public void NativeQueueTest1()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(stackalloc int[] { 1, 2, 3, 4 });
            Assert.IsTrue(queue.IsValid);
            Assert.IsFalse(queue.IsEmpty);
            Assert.AreEqual(4, queue.Length);
            Assert.AreEqual(4, queue.Capacity);
        }

        [Test()]
        public void GetAllocatorTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);

            unsafe
            {
                var allocator = queue.GetAllocator();
                int* p = allocator.Allocate<int>(4);
                for (int i = 0; i < 4; i++)
                {
                    p[i] = i;
                }

                Assert.AreEqual(0, p[0]);
                Assert.AreEqual(1, p[1]);
                Assert.AreEqual(2, p[2]);
                Assert.AreEqual(3, p[3]);

                allocator.Free(p);
            }
        }

        [Test()]
        public void EnqueueTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            Assert.IsTrue(queue.IsValid);
            Assert.IsFalse(queue.IsEmpty);
            Assert.AreEqual(4, queue.Length);
            Assert.AreEqual(4, queue.Capacity);

            Assert.IsTrue(queue.Contains(1));
            Assert.IsTrue(queue.Contains(2));
            Assert.IsTrue(queue.Contains(3));
            Assert.IsFalse(queue.Contains(0));

            queue.Enqueue(5);

            Assert.IsTrue(queue.IsValid);
            Assert.IsFalse(queue.IsEmpty);
            Assert.AreEqual(5, queue.Length);
            Assert.AreEqual(8, queue.Capacity);

            Assert.IsTrue(queue.Contains(1));
            Assert.IsTrue(queue.Contains(2));
            Assert.IsTrue(queue.Contains(3));
            Assert.IsTrue(queue.Contains(4));
            Assert.IsFalse(queue.Contains(0));
        }

        [Test()]
        public void DequeueTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(2, queue.Length);

            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(1, queue.Length);

            Assert.AreEqual(3, queue.Dequeue()); 
            Assert.AreEqual(0, queue.Length);

            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        }

        [Test()]
        public void EnqueueAndDequeueTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(2, queue.Dequeue());

            queue.Enqueue(4);
            queue.Enqueue(5);

            Assert.AreEqual(3, queue.Dequeue());

            Assert.AreEqual(4, queue.Dequeue());
            Assert.AreEqual(5, queue.Dequeue());

            queue.Enqueue(6);
            queue.Enqueue(7);

            Assert.AreEqual(6, queue.Dequeue());
            Assert.AreEqual(7, queue.Dequeue());

            queue.Enqueue(8);
            queue.Enqueue(9);

            Assert.AreEqual(8, queue.Dequeue());
            Assert.AreEqual(9, queue.Dequeue());

            queue.Enqueue(10);
            queue.Enqueue(11);

            Assert.AreEqual(10, queue.Dequeue());
            Assert.AreEqual(11, queue.Dequeue());

            queue.Enqueue(12);
            queue.Enqueue(13);

            Assert.AreEqual(12, queue.Dequeue());
            Assert.AreEqual(13, queue.Dequeue());

            Assert.AreEqual(0, queue.Length);
            Assert.AreEqual(4, queue.Capacity);
        }

        [Test()]
        public void PeekTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.AreEqual(1, queue.Peek());
            queue.Dequeue();

            Assert.AreEqual(2, queue.Peek());
            queue.Dequeue();

            Assert.AreEqual(3, queue.Peek());
            queue.Dequeue();
        }

        [Test()]
        public void TryDequeueTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.IsTrue(queue.TryDequeue(out var v1));
            Assert.AreEqual(1, v1);

            Assert.IsTrue(queue.TryDequeue(out var v2));
            Assert.AreEqual(2, v2);

            Assert.IsTrue(queue.TryDequeue(out var v3));
            Assert.AreEqual(3, v3);

            Assert.IsFalse(queue.TryDequeue(out var _));
        }

        [Test()]
        public void TryPeekTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.IsTrue(queue.TryPeek(out var v1));
            Assert.AreEqual(1, v1);
            queue.Dequeue();

            Assert.IsTrue(queue.TryPeek(out var v2));
            Assert.AreEqual(2, v2);
            queue.Dequeue();

            Assert.IsTrue(queue.TryPeek(out var v3));
            Assert.AreEqual(3, v3);
            queue.Dequeue();

            Assert.IsFalse(queue.TryPeek(out var _));
        }

        [Test()]
        public void ContainsTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.IsTrue(queue.Contains(1));
            Assert.IsTrue(queue.Contains(2));
            Assert.IsTrue(queue.Contains(3));
            Assert.IsFalse(queue.Contains(0));

            queue.Dequeue();
            queue.Dequeue();
            queue.Dequeue();

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.IsTrue(queue.Contains(1));
            Assert.IsTrue(queue.Contains(2));
            Assert.IsTrue(queue.Contains(3));
            Assert.IsFalse(queue.Contains(0));
        }

        [Test()]
        public void ContainsTest1()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            Assert.IsTrue(queue.Contains(1));
            Assert.IsTrue(queue.Contains(2));
            Assert.IsTrue(queue.Contains(3));
            Assert.IsTrue(queue.Contains(4));
            Assert.IsFalse(queue.Contains(0));

            queue.Dequeue();
            queue.Dequeue();
            queue.Dequeue();
            queue.Dequeue();

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            Assert.IsTrue(queue.Contains(1));
            Assert.IsTrue(queue.Contains(2));
            Assert.IsTrue(queue.Contains(3));
            Assert.IsTrue(queue.Contains(4));
            Assert.IsFalse(queue.Contains(0));
        }

        [Test()]
        public void TrimExcessTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(10);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            queue.TrimExcess();
            Assert.AreEqual(3, queue.Capacity);
        }

        [Test()]
        public void TrimExcessTest1()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(10);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            queue.TrimExcess(6);
            Assert.AreEqual(6, queue.Capacity);
        }

        [Test()]
        public void ToArrayTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(6);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            int[] array = queue.ToArray();
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void ToNativeArrayTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(6);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            using NativeArray<int> array = queue.ToNativeArray();
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void ToNativeArrayAndDisposeTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(6);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            using NativeArray<int> array = queue.ToNativeArrayAndDispose();
            Assert.AreEqual(4, array.Length);

            Assert.IsFalse(queue.IsValid);
            Assert.IsTrue(queue.IsEmpty);
            Assert.AreEqual(0, queue.Length);
            Assert.AreEqual(0, queue.Capacity);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void CopyToTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(6);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            Span<int> span = stackalloc int[5];
            queue.CopyTo(span, 1, 3);

            Assert.AreEqual(0, span[0]);
            Assert.AreEqual(1, span[1]);
            Assert.AreEqual(2, span[2]);
            Assert.AreEqual(3, span[3]);
            Assert.AreEqual(0, span[0]);
        }

        [Test()]
        public void EnsureCapacityTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(6);
            queue.EnsureCapacity(20);

            Assert.AreEqual(20, queue.Capacity);
        }

        [Test()]
        public void DisposeTest()
        {
            NativeQueue<int> queue = new NativeQueue<int>(6);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Dispose();

            Assert.IsTrue(queue.IsEmpty);
            Assert.IsFalse(queue.IsValid);
            Assert.AreEqual(0, queue.Capacity);
        }

        [Test()]
        public void ToStringTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            Assert.AreEqual("[1, 2, 3, 4]", queue.ToString());
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            var enumerator = queue.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(4, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(5, enumerator.Current);

            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void GetEnumeratorTest1()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            queue.Dequeue();
            queue.Dequeue();
            queue.Dequeue();

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            var enumerator = queue.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);

            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void GetEnumeratorTest2()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            var enumerator = queue.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(4, enumerator.Current);

            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void GetEnumeratorTest3()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            queue.Dequeue();
            queue.Dequeue();
            queue.Dequeue();
            queue.Dequeue(); 
            
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            var enumerator = queue.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(4, enumerator.Current);

            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void ForEachTest()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(4);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            int[] arr = queue.ToArray();
            foreach(var e in queue)
            {
                CollectionAssert.Contains(arr, e);
            }

            queue.Enqueue(4);

            arr = queue.ToArray();
            foreach (var e in queue)
            {
                CollectionAssert.Contains(arr, e);
            }
        }
    }
}