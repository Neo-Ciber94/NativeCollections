using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeDequeTests
    {
        [Test()]
        public void NativeDequeTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            Assert.IsTrue(deque.IsEmpty);
            Assert.IsTrue(deque.IsValid);
            Assert.AreEqual(0, deque.Length);
            Assert.AreEqual(4, deque.Capacity);
        }

        [Test()]
        public void NativeDequeTest1()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(stackalloc int[] { 1, 2, 3, 4 });
            Assert.IsFalse(deque.IsEmpty);
            Assert.IsTrue(deque.IsValid);
            Assert.AreEqual(4, deque.Length);
            Assert.AreEqual(4, deque.Capacity);
        }

        [Test()]
        public void GetAllocatorTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);

            unsafe
            {
                var allocator = deque.GetAllocator();
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
        public void AddFirstTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);
            deque.AddFirst(4);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));

            Assert.AreEqual(4, deque.Length);
            Assert.AreEqual(4, deque.Capacity);

            deque.AddFirst(5);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));
            Assert.IsTrue(deque.Contains(5));

            Assert.AreEqual(5, deque.Length);
            Assert.AreEqual(8, deque.Capacity);
        }

        [Test()]
        public void AddLastTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));

            Assert.AreEqual(4, deque.Length);
            Assert.AreEqual(4, deque.Capacity);

            deque.AddLast(5);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));
            Assert.IsTrue(deque.Contains(5));

            Assert.AreEqual(5, deque.Length);
            Assert.AreEqual(8, deque.Capacity);
        }

        [Test()]
        public void AddFirstAndLastTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddFirst(3);
            deque.AddFirst(4);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));

            deque.AddLast(5);
            deque.AddFirst(6);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));
            Assert.IsTrue(deque.Contains(5));
            Assert.IsTrue(deque.Contains(6));
        }

        [Test()]
        public void RemoveFirstTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);
            deque.AddFirst(4);

            Assert.AreEqual(4, deque.RemoveFirst());
            Assert.AreEqual(3, deque.RemoveFirst());
            Assert.AreEqual(2, deque.RemoveFirst());
            Assert.AreEqual(1, deque.RemoveFirst());

            Assert.Throws<InvalidOperationException>(() => deque.RemoveFirst());
        }

        [Test()]
        public void RemoveLastTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);
            deque.AddFirst(4);

            Assert.AreEqual(1, deque.RemoveLast());
            Assert.AreEqual(2, deque.RemoveLast());
            Assert.AreEqual(3, deque.RemoveLast());
            Assert.AreEqual(4, deque.RemoveLast());

            Assert.Throws<InvalidOperationException>(() => deque.RemoveLast());
        }

        [Test()]
        public void AddAndRemoveTest1()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);

            deque.RemoveLast();
            deque.RemoveLast();
            deque.RemoveLast();

            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
        }

        [Test()]
        public void AddAndRemoveTest2()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);

            deque.RemoveFirst();
            deque.RemoveFirst();
            deque.RemoveFirst();

            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
        }

        [Test()]
        public void PeekFirstTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);

            Assert.AreEqual(3, deque.PeekFirst());
            deque.RemoveFirst();

            Assert.AreEqual(2, deque.PeekFirst());
            deque.RemoveFirst();

            Assert.AreEqual(1, deque.PeekFirst());
            deque.RemoveFirst();

            Assert.Throws<InvalidOperationException>(() => deque.PeekFirst());
        }

        [Test()]
        public void PeekLastTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);

            Assert.AreEqual(1, deque.PeekLast());
            deque.RemoveLast();

            Assert.AreEqual(2, deque.PeekLast());
            deque.RemoveLast();

            Assert.AreEqual(3, deque.PeekLast());
            deque.RemoveLast();

            Assert.Throws<InvalidOperationException>(() => deque.PeekLast());
        }

        [Test()]
        public void TryRemoveFirstTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);

            Assert.IsTrue(deque.TryRemoveFirst(out var v1));
            Assert.AreEqual(3, v1);

            Assert.IsTrue(deque.TryRemoveFirst(out var v2));
            Assert.AreEqual(2, v2);

            Assert.IsTrue(deque.TryRemoveFirst(out var v3));
            Assert.AreEqual(1, v3);

            Assert.IsFalse(deque.TryRemoveFirst(out var _));
        }

        [Test()]
        public void TryRemoveLastTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);

            Assert.IsTrue(deque.TryRemoveLast(out var v1));
            Assert.AreEqual(1, v1);

            Assert.IsTrue(deque.TryRemoveLast(out var v2));
            Assert.AreEqual(2, v2);

            Assert.IsTrue(deque.TryRemoveLast(out var v3));
            Assert.AreEqual(3, v3);

            Assert.IsFalse(deque.TryRemoveLast(out var _));
        }

        [Test()]
        public void TryPeekFirstTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);

            Assert.IsTrue(deque.TryPeekFirst(out var v1));
            Assert.AreEqual(1, v1);
            deque.RemoveFirst();

            Assert.IsTrue(deque.TryPeekFirst(out var v2));
            Assert.AreEqual(2, v2);
            deque.RemoveFirst();

            Assert.IsTrue(deque.TryPeekFirst(out var v3));
            Assert.AreEqual(3, v3);
            deque.RemoveFirst();

            Assert.IsFalse(deque.TryPeekFirst(out var _));
        }

        [Test()]
        public void TryPeekLastTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);

            Assert.IsTrue(deque.TryPeekLast(out var v1));
            Assert.AreEqual(3, v1);
            deque.RemoveLast();

            Assert.IsTrue(deque.TryPeekLast(out var v2));
            Assert.AreEqual(2, v2);
            deque.RemoveLast();

            Assert.IsTrue(deque.TryPeekLast(out var v3));
            Assert.AreEqual(1, v3);
            deque.RemoveLast();

            Assert.IsFalse(deque.TryPeekLast(out var _));
        }

        [Test()]
        public void ContainsTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);
            deque.AddFirst(4);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));

            deque.AddFirst(5);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));
            Assert.IsTrue(deque.Contains(5));
        }

        [Test()]
        public void ContainsTest1()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
        }

        [Test()]
        public void ContainsTest2()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));

            deque.AddLast(5);

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));
            Assert.IsTrue(deque.Contains(5));
        }

        [Test()]
        public void ReverseTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);

            deque.Reverse();

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
        }

        [Test()]
        public void ReverseTest1()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            deque.Reverse();

            Assert.IsTrue(deque.Contains(1));
            Assert.IsTrue(deque.Contains(2));
            Assert.IsTrue(deque.Contains(3));
            Assert.IsTrue(deque.Contains(4));
        }

        [Test()]
        public void TrimExcessTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(6);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);

            deque.TrimExcess();
            Assert.AreEqual(3, deque.Length);
            Assert.AreEqual(3, deque.Capacity);
        }

        [Test()]
        public void TrimExcessTest1()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(10);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);

            deque.TrimExcess(5);
            Assert.AreEqual(3, deque.Length);
            Assert.AreEqual(5, deque.Capacity);
        }

        [Test()]
        public void CopyToTest1()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(6);
            deque.AddFirst(1);
            deque.AddFirst(2);
            deque.AddFirst(3);
            deque.AddFirst(4);

            Span<int> span = stackalloc int[5];
            deque.CopyTo(span, 1, 3);

            Assert.AreEqual(0, span[0]);
            Assert.AreEqual(4, span[1]);
            Assert.AreEqual(3, span[2]);
            Assert.AreEqual(2, span[3]);
            Assert.AreEqual(0, span[0]);
        }

        [Test()]
        public void CopyToTest2()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(6);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            Span<int> span = stackalloc int[5];
            deque.CopyTo(span, 1, 3);

            Assert.AreEqual(0, span[0]);
            Assert.AreEqual(1, span[1]);
            Assert.AreEqual(2, span[2]);
            Assert.AreEqual(3, span[3]);
            Assert.AreEqual(0, span[0]);
        }

        [Test()]
        public void CopyToTest3()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(6);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            deque.Reverse();
            Span<int> span = stackalloc int[5];
            deque.CopyTo(span, 1, 3);

            Assert.AreEqual(0, span[0]);
            Assert.AreEqual(4, span[1]);
            Assert.AreEqual(3, span[2]);
            Assert.AreEqual(2, span[3]);
            Assert.AreEqual(0, span[0]);
        }

        [Test()]
        public void EnsureCapcityTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.EnsureCapcity(10);

            Assert.AreEqual(10, deque.Capacity);
        }

        [Test()]
        public void ToArrayTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(6);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, deque.ToArray());
        }

        [Test()]
        public void ToNativeArrayTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(6);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            var array = deque.ToNativeArray();
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void ToNativeArrayAndDisposeTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(6);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            var array = deque.ToNativeArrayAndDispose();
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void DisposeTest()
        {
            NativeDeque<int> deque = new NativeDeque<int>(stackalloc int[] { 1, 2, 3, 4 });
            deque.Dispose();

            Assert.AreEqual(0, deque.Length);
            Assert.AreEqual(0, deque.Capacity);
            Assert.IsTrue(deque.IsEmpty);
            Assert.IsFalse(deque.IsValid);
        }

        [Test()]
        public void ToStringTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(stackalloc int[] { 1, 2, 3, 4 });
            Assert.AreEqual("[1, 2, 3, 4]", deque.ToString());
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(stackalloc int[] { 1, 2, 3, 4 });
            var enumerator = deque.GetEnumerator();

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
        public void GetEnumeratorTest1()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            deque.RemoveFirst();
            deque.RemoveFirst();
            deque.RemoveFirst();
            deque.RemoveFirst();

            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            var enumerator = deque.GetEnumerator();

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
            using NativeDeque<int> deque = new NativeDeque<int>(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);

            var array = deque.ToArray();
            foreach(var e in deque)
            {
                CollectionAssert.Contains(array, e);
            }
        }
    }
}