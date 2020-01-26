using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeStackTests
    {
        [Test()]
        public void NativeStackTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            Assert.IsTrue(stack.IsValid);
            Assert.IsTrue(stack.IsEmpty);
            Assert.AreEqual(0, stack.Length);
            Assert.AreEqual(4, stack.Capacity);
        }

        [Test()]
        public void NativeStackTest1()
        {
            using NativeStack<int> stack = new NativeStack<int>(stackalloc int[] { 1, 2, 3, 4 });
            Assert.IsTrue(stack.IsValid);
            Assert.IsFalse(stack.IsEmpty);
            Assert.AreEqual(4, stack.Length);
            Assert.AreEqual(4, stack.Capacity);
        }

        [Test()]
        public void GetAllocatorTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);

            unsafe
            {
                var allocator = stack.GetAllocator();
                int* p = allocator.Allocate<int>(4);
                for(int i = 0; i < 4; i++)
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
        public void PushTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            Assert.AreEqual(4, stack.Length);
            Assert.AreEqual(4, stack.Capacity);

            Assert.IsTrue(stack.Contains(1));
            Assert.IsTrue(stack.Contains(2));
            Assert.IsTrue(stack.Contains(3));
            Assert.IsTrue(stack.Contains(4));
            Assert.IsFalse(stack.Contains(0));

            stack.Push(5);

            Assert.AreEqual(5, stack.Length);
            Assert.AreEqual(8, stack.Capacity);

            Assert.IsTrue(stack.Contains(1));
            Assert.IsTrue(stack.Contains(2));
            Assert.IsTrue(stack.Contains(3));
            Assert.IsTrue(stack.Contains(4));
            Assert.IsTrue(stack.Contains(5));
            Assert.IsFalse(stack.Contains(0));
        }

        [Test()]
        public void PopTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            Assert.AreEqual(3, stack.Pop());
            Assert.AreEqual(2, stack.Pop());
            Assert.AreEqual(1, stack.Pop());

            Assert.Throws<InvalidOperationException>(() => stack.Pop());
        }

        [Test()]
        public void PeekTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            Assert.AreEqual(3, stack.Peek());
            stack.Pop();

            Assert.AreEqual(2, stack.Peek());
            stack.Pop();

            Assert.AreEqual(1, stack.Peek());
            stack.Pop();

            Assert.Throws<InvalidOperationException>(() => stack.Peek());
        }

        [Test()]
        public void TryPopTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            Assert.IsTrue(stack.TryPop(out var v1));
            Assert.AreEqual(3, v1);

            Assert.IsTrue(stack.TryPop(out var v2));
            Assert.AreEqual(2, v2);

            Assert.IsTrue(stack.TryPop(out var v3));
            Assert.AreEqual(1, v3);

            Assert.IsFalse(stack.TryPop(out var v4));
        }

        [Test()]
        public void TryPeekTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            Assert.IsTrue(stack.TryPeek(out var v1));
            Assert.AreEqual(3, v1);
            stack.Pop();

            Assert.IsTrue(stack.TryPeek(out var v2));
            Assert.AreEqual(2, v2);
            stack.Pop();

            Assert.IsTrue(stack.TryPeek(out var v3));
            Assert.AreEqual(1, v3);
            stack.Pop();

            Assert.IsFalse(stack.TryPeek(out var v4));
        }

        [Test()]
        public void PeekReferenceTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            Assert.AreEqual(3, stack.PeekReference());
            stack.Pop();

            Assert.AreEqual(2, stack.PeekReference());
            stack.Pop();

            Assert.AreEqual(1, stack.PeekReference());
            stack.Pop();

            Assert.Throws<InvalidOperationException>(() => stack.PeekReference());
        }

        [Test()]
        public void TryPeekReferenceTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            Assert.IsTrue(stack.TryPeekReference(out var v1));
            Assert.AreEqual(3, v1.Value);
            stack.Pop();

            Assert.IsTrue(stack.TryPeekReference(out var v2));
            Assert.AreEqual(2, v2.Value);
            stack.Pop();

            Assert.IsTrue(stack.TryPeekReference(out var v3));
            Assert.AreEqual(1, v3.Value);
            stack.Pop();

            Assert.IsFalse(stack.TryPeekReference(out var v4));
        }

        [Test()]
        public void ClearTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            stack.Clear();

            Assert.IsTrue(stack.IsValid);
            Assert.IsTrue(stack.IsEmpty);
            Assert.AreEqual(0, stack.Length);
            Assert.AreEqual(4, stack.Capacity);
        }

        [Test()]
        public void ContainsTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            Assert.IsTrue(stack.Contains(1));
            Assert.IsTrue(stack.Contains(2));
            Assert.IsTrue(stack.Contains(3));
            Assert.IsTrue(stack.Contains(4));
            Assert.IsFalse(stack.Contains(0));
        }

        [Test()]
        public void TrimExcessTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(10);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            Assert.AreEqual(4, stack.Length);
            Assert.AreEqual(10, stack.Capacity);

            stack.TrimExcess();

            Assert.IsTrue(stack.Contains(1));
            Assert.IsTrue(stack.Contains(2));
            Assert.IsTrue(stack.Contains(3));
            Assert.IsTrue(stack.Contains(4));
            Assert.IsFalse(stack.Contains(0));

            Assert.AreEqual(4, stack.Length);
            Assert.AreEqual(4, stack.Capacity);
        }

        [Test()]
        public void TrimExcessTest1()
        {
            using NativeStack<int> stack = new NativeStack<int>(10);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            Assert.AreEqual(4, stack.Length);
            Assert.AreEqual(10, stack.Capacity);

            stack.TrimExcess(6);

            Assert.IsTrue(stack.Contains(1));
            Assert.IsTrue(stack.Contains(2));
            Assert.IsTrue(stack.Contains(3));
            Assert.IsTrue(stack.Contains(4));
            Assert.IsFalse(stack.Contains(0));

            Assert.AreEqual(4, stack.Length);
            Assert.AreEqual(6, stack.Capacity);
        }

        [Test()]
        public void EnsureCapacityTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(6);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            stack.EnsureCapacity(20);

            Assert.IsTrue(stack.Contains(1));
            Assert.IsTrue(stack.Contains(2));
            Assert.IsTrue(stack.Contains(3));
            Assert.IsTrue(stack.Contains(4));
            Assert.IsFalse(stack.Contains(0));

            Assert.AreEqual(4, stack.Length);
            Assert.AreEqual(20, stack.Capacity);
        }

        [Test()]
        public void CopyToTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(6);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            Span<int> span = stackalloc int[5];
            stack.CopyTo(span, 1, 3);

            Assert.AreEqual(0, span[0]);
            Assert.AreEqual(1, span[1]);
            Assert.AreEqual(2, span[2]);
            Assert.AreEqual(3, span[3]);
            Assert.AreEqual(0, span[0]);
        }

        [Test()]
        public void DisposeTest()
        {
            NativeStack<int> stack = new NativeStack<int>(6);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            stack.Dispose();

            Assert.IsFalse(stack.IsValid);
            Assert.IsTrue(stack.IsEmpty);
            Assert.AreEqual(0, stack.Length);
            Assert.AreEqual(0, stack.Capacity);
        }

        [Test()]
        public void GetUnsafePointerTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(6);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            unsafe
            {
                int* p = stack.GetUnsafePointer();
                Assert.AreEqual(1, p[0]);
                Assert.AreEqual(2, p[1]);
                Assert.AreEqual(3, p[2]);
                Assert.AreEqual(4, p[3]);
            }
        }

        [Test()]
        public void ToArrayTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(6);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            int[] array = stack.ToArray();
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void ToNativeArrayTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(6);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            using NativeArray<int> array = stack.ToNativeArray();
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void ToNativeArrayAndDisposeTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(6);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            using NativeArray<int> array = stack.ToNativeArrayAndDispose();
            Assert.AreEqual(4, array.Length);

            Assert.IsFalse(stack.IsValid);
            Assert.IsTrue(stack.IsEmpty);
            Assert.AreEqual(0, stack.Length);
            Assert.AreEqual(0, stack.Capacity);


            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void ToNativeArrayAndDisposeTest1()
        {
            using NativeStack<int> stack = new NativeStack<int>(6);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            using NativeArray<int> array = stack.ToNativeArrayAndDispose(false);
            Assert.AreEqual(6, array.Length);

            Assert.IsFalse(stack.IsValid);
            Assert.IsTrue(stack.IsEmpty);
            Assert.AreEqual(0, stack.Length);
            Assert.AreEqual(0, stack.Capacity);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(0, array[4]);
            Assert.AreEqual(0, array[5]);
        }

        [Test()]
        public void ToStringTest()
        {
            using NativeStack<int> stack = new NativeStack<int>(4);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            Assert.AreEqual("[1, 2, 3, 4]", stack.ToString());
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using NativeStack<int> list = new NativeStack<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            var enumerator = list.GetEnumerator();

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
    }
}