using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeListTests
    {
        [Test()]
        public void NativeListTest()
        {
            using NativeList<int> list = new NativeList<int>(10);
            Assert.IsTrue(list.IsValid);
            Assert.IsTrue(list.IsEmpty);
            Assert.AreEqual(0, list.Length);
            Assert.AreEqual(10, list.Capacity);
        }

        [Test()]
        public void NativeListTest1()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.IsTrue(list.IsValid);
            Assert.IsFalse(list.IsEmpty);
            Assert.AreEqual(5, list.Length);
            Assert.AreEqual(5, list.Capacity);
        }

        [Test()]
        unsafe public void GetAllocatorTest()
        {
            using NativeList<int> list = new NativeList<int>(10);
            var allocator = list.GetAllocator();

            int* p = allocator.Allocate<int>(5);

            try
            {
                for (int i = 0; i < 5; i++)
                {
                    *p++ = i;
                }

                Span<int> span = new Span<int>(p, 5);
                Assert.AreEqual(0, span[0]);
                Assert.AreEqual(1, span[1]);
                Assert.AreEqual(2, span[2]);
                Assert.AreEqual(3, span[3]);
                Assert.AreEqual(4, span[4]);
            }
            finally
            {
                allocator.Free(p);
            }
        }

        [Test()]
        public void AddTest()
        {
            using NativeList<int> list = new NativeList<int>(3);
            list.Add(1);
            list.Add(2);
            list.Add(3);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);

            Assert.AreEqual(3, list.Length);
            Assert.AreEqual(3, list.Capacity);
            Assert.IsFalse(list.IsEmpty);

            list.Add(4);
            Assert.AreEqual(4, list.Length);
            Assert.AreEqual(6, list.Capacity);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(4, list[3]);
        }

        [Test()]
        public void AddRangeTest()
        {
            using NativeList<int> list = new NativeList<int>(3);
            list.AddRange(stackalloc int[] { 1, 2, 3 });

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);

            Assert.AreEqual(3, list.Length);
            Assert.AreEqual(3, list.Capacity);
            Assert.IsFalse(list.IsEmpty);

            list.AddRange(stackalloc int[] { 4, 5, 6, 7, 8 });
            Assert.AreEqual(8, list.Length);
            Assert.AreEqual(8, list.Capacity);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(4, list[3]);
            Assert.AreEqual(5, list[4]);
            Assert.AreEqual(6, list[5]);
            Assert.AreEqual(7, list[6]);
            Assert.AreEqual(8, list[7]);
        }

        [Test()]
        public void InsertTest()
        {
            using NativeList<int> list = new NativeList<int>(3);
            list.Add(1);
            list.Add(2);
            list.Add(3);

            list.Insert(0, 0);

            Assert.AreEqual(4, list.Length);
            Assert.AreEqual(6, list.Capacity);

            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);

            list.Insert(3, -3);

            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(-3, list[3]);
            Assert.AreEqual(3, list[4]);

            Assert.AreEqual(5, list.Length);
            Assert.AreEqual(6, list.Capacity);
        }

        [Test()]
        public void InsertRangeTest()
        {
            using NativeList<int> list = new NativeList<int>(3);
            list.Add(1);
            list.Add(2);
            list.Add(3);

            list.InsertRange(0, stackalloc int[] { -2, -1, 0 });

            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(-1, list[1]);
            Assert.AreEqual(0, list[2]);
            Assert.AreEqual(1, list[3]);
            Assert.AreEqual(2, list[4]);
            Assert.AreEqual(3, list[5]);

            Assert.AreEqual(6, list.Length);
            Assert.AreEqual(6, list.Capacity);

            list.InsertRange(5, stackalloc int[] { 10, 20, 30 });

            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(-1, list[1]);
            Assert.AreEqual(0, list[2]);
            Assert.AreEqual(1, list[3]);
            Assert.AreEqual(2, list[4]);
            Assert.AreEqual(10, list[5]);
            Assert.AreEqual(20, list[6]);
            Assert.AreEqual(30, list[7]);
            Assert.AreEqual(3, list[8]);

            Assert.AreEqual(9, list.Length);
            Assert.AreEqual(12, list.Capacity);
        }

        [Test()]
        public void InsertRangeTest1()
        {
            using NativeList<int> list = new NativeList<int>(3);
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { -2, -1, 0 });
            list.Add(1);
            list.Add(2);
            list.Add(3);

            list.InsertRange(0, array);

            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(-1, list[1]);
            Assert.AreEqual(0, list[2]);
            Assert.AreEqual(1, list[3]);
            Assert.AreEqual(2, list[4]);
            Assert.AreEqual(3, list[5]);

            Assert.AreEqual(6, list.Length);
            Assert.AreEqual(6, list.Capacity);
        }

        [Test()]
        public void RemoveTest()
        {
            using NativeList<int> list = new NativeList<int>(3);
            list.Add(1);
            list.Add(2);
            list.Add(3);

            Assert.AreEqual(3, list.Length);
            Assert.AreEqual(3, list.Capacity);

            Assert.True(list.Remove(1));

            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(3, list[1]);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(3, list.Capacity);

            Assert.True(list.Remove(3));
            Assert.False(list.Remove(0));

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(3, list.Capacity);
        }

        [Test()]
        public void RemoveRangeTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            list.RemoveRange(3, 7);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(8, list[3]);
            Assert.AreEqual(9, list[4]);
            Assert.AreEqual(10, list[5]);

            Assert.AreEqual(6, list.Length);
            Assert.AreEqual(10, list.Capacity);
        }

        [Test()]
        public void RemoveAtTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4 });
            list.RemoveAt(2);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(4, list[2]);

            list.RemoveAt(1);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(4, list[1]);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(4, list.Capacity);
        }

        [Test()]
        public void RemoveAtTest1()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4 });
            list.RemoveAt(3);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);

            Assert.AreEqual(3, list.Length);
            Assert.AreEqual(4, list.Capacity);
        }

        [Test()]
        public void RemoveIfTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            list.RemoveIf(e => e % 2 != 0);

            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(4, list[1]);
            Assert.AreEqual(6, list[2]);
            Assert.AreEqual(8, list[3]);
            Assert.AreEqual(10, list[4]);

            Assert.AreEqual(5, list.Length);
            Assert.AreEqual(10, list.Capacity);
        }

        [Test()]
        public void ReplaceAllTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 1, 2, 2, 3, 3 });
            list.ReplaceAll(2, -2);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(-2, list[2]);
            Assert.AreEqual(-2, list[3]);
            Assert.AreEqual(3, list[4]);
            Assert.AreEqual(3, list[5]);

            Assert.AreEqual(6, list.Length);
            Assert.AreEqual(6, list.Capacity);

            list.ReplaceAll(3, -3);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(-2, list[2]);
            Assert.AreEqual(-2, list[3]);
            Assert.AreEqual(-3, list[4]);
            Assert.AreEqual(-3, list[5]);

            list.ReplaceAll(1, -1);

            Assert.AreEqual(-1, list[0]);
            Assert.AreEqual(-1, list[1]);
            Assert.AreEqual(-2, list[2]);
            Assert.AreEqual(-2, list[3]);
            Assert.AreEqual(-3, list[4]);
            Assert.AreEqual(-3, list[5]);
        }

        [Test()]
        public void ReplaceIfTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 1, 2, 2, 3, 3 });
            list.ReplaceIf(0, s => s % 2 != 0);

            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(0, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(2, list[3]);
            Assert.AreEqual(0, list[4]);
            Assert.AreEqual(0, list[5]);

            Assert.AreEqual(6, list.Length);
            Assert.AreEqual(6, list.Capacity);
        }

        [Test()]
        public void ClearTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            list.Clear();

            Assert.AreEqual(0, list.Length);
            Assert.AreEqual(10, list.Capacity);
            Assert.IsTrue(list.IsEmpty);
        }

        [Test()]
        public void ReverseTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            list.Reverse();

            Assert.AreEqual(10, list[0]);
            Assert.AreEqual(9, list[1]);
            Assert.AreEqual(8, list[2]);
            Assert.AreEqual(7, list[3]);
            Assert.AreEqual(6, list[4]);
            Assert.AreEqual(5, list[5]);
            Assert.AreEqual(4, list[6]);
            Assert.AreEqual(3, list[7]);
            Assert.AreEqual(2, list[8]);
            Assert.AreEqual(1, list[9]);
        }

        [Test()]
        public void ReverseTest1()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            list.Reverse(5);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(4, list[3]);
            Assert.AreEqual(5, list[4]);
            Assert.AreEqual(10, list[5]);
            Assert.AreEqual(9, list[6]);
            Assert.AreEqual(8, list[7]);
            Assert.AreEqual(7, list[8]);
            Assert.AreEqual(6, list[9]);
        }

        [Test()]
        public void ReverseTest2()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            list.Reverse(2, 7);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(8, list[2]);
            Assert.AreEqual(7, list[3]);
            Assert.AreEqual(6, list[4]);
            Assert.AreEqual(5, list[5]);
            Assert.AreEqual(4, list[6]);
            Assert.AreEqual(3, list[7]);
            Assert.AreEqual(9, list[8]);
            Assert.AreEqual(10, list[9]);
        }

        [Test()]
        public void IndexOfTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            Assert.AreEqual(0, list.IndexOf(1));
            Assert.AreEqual(4, list.IndexOf(5));
            Assert.AreEqual(5, list.IndexOf(6));
            Assert.AreEqual(9, list.IndexOf(10));
        }

        [Test()]
        public void IndexOfTest1()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            Assert.AreEqual(6, list.IndexOf(7, 3));
            Assert.AreEqual(9, list.IndexOf(10, 5));
            Assert.AreEqual(-1, list.IndexOf(2, 3));
            Assert.AreEqual(-1, list.IndexOf(9, 9));
        }

        [Test()]
        public void IndexOfTest2()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            Assert.AreEqual(6, list.IndexOf(7, 3, 9));
            Assert.AreEqual(-1, list.IndexOf(2, 3, 6));
            Assert.AreEqual(-1, list.IndexOf(9, 4, 8));

            Assert.Catch(() => Assert.AreEqual(9, list.IndexOf(5, 10, 10)));
        }

        [Test()]
        public void LastIndexOfTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            Assert.AreEqual(0, list.LastIndexOf(1));
            Assert.AreEqual(4, list.LastIndexOf(5));
            Assert.AreEqual(5, list.LastIndexOf(6));
            Assert.AreEqual(9, list.LastIndexOf(10));
        }

        [Test()]
        public void LastIndexOfTest1()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            Assert.AreEqual(6, list.LastIndexOf(7, 3));
            Assert.AreEqual(9, list.LastIndexOf(10, 5));
            Assert.AreEqual(-1, list.LastIndexOf(2, 3));
            Assert.AreEqual(-1, list.LastIndexOf(9, 9));
        }

        [Test()]
        public void LastIndexOfTest2()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            Assert.AreEqual(6, list.LastIndexOf(7, 3, 9));
            Assert.AreEqual(-1, list.LastIndexOf(2, 3, 6));
            Assert.AreEqual(-1, list.LastIndexOf(9, 4, 8));

            Assert.Catch(() => Assert.AreEqual(9, list.LastIndexOf(5, 10, 10)));
        }

        [Test()]
        public void ContainsTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.IsTrue(list.Contains(1));
            Assert.IsTrue(list.Contains(2));
            Assert.IsTrue(list.Contains(3));
            Assert.IsTrue(list.Contains(4));
            Assert.IsTrue(list.Contains(5));

            Assert.IsFalse(list.Contains(9));
            Assert.IsFalse(list.Contains(0));
        }

        [Test()]
        public void ContainsAllTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.IsTrue(list.ContainsAll(stackalloc int[] { 1, 2, 4 }));
            Assert.IsFalse(list.ContainsAll(stackalloc int[] { 1, 8, 4 }));
        }

        [Test()]
        public void TrimExcessTest()
        {
            using NativeList<int> list = new NativeList<int>(4);
            list.Add(2);
            list.Add(3);

            list.TrimExcess();

            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(3, list[1]);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(2, list.Capacity);
        }

        [Test()]
        public void TrimExcessTest1()
        {
            using NativeList<int> list = new NativeList<int>(4);
            list.Add(2);
            list.Add(3);

            list.TrimExcess(3);

            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(3, list[1]);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(3, list.Capacity);
        }

        [Test()]
        public void EnsureCapacityTest()
        {
            using NativeList<int> list = new NativeList<int>(4);
            list.EnsureCapacity(10);

            Assert.AreEqual(0, list.Length);
            Assert.AreEqual(10, list.Capacity);
        }

        [Test()]
        public void CopyToTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Span<int> span = stackalloc int[5];

            list.CopyTo(span);

            Assert.AreEqual(1, span[0]);
            Assert.AreEqual(2, span[1]);
            Assert.AreEqual(3, span[2]);
            Assert.AreEqual(4, span[3]);
            Assert.AreEqual(5, span[4]);
        }

        [Test()]
        public void CopyToTest1()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Span<int> span = stackalloc int[5];

            list.CopyTo(span, 3);

            Assert.AreEqual(1, span[0]);
            Assert.AreEqual(2, span[1]);
            Assert.AreEqual(3, span[2]);
            Assert.AreEqual(0, span[3]);
            Assert.AreEqual(0, span[4]);
        }

        [Test()]
        public void CopyToTest2()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Span<int> span = stackalloc int[5];

            list.CopyTo(span, 2, 3);

            Assert.AreEqual(0, span[0]);
            Assert.AreEqual(0, span[1]);
            Assert.AreEqual(1, span[2]);
            Assert.AreEqual(2, span[3]);
            Assert.AreEqual(3, span[4]);
        }

        [Test()]
        public void ToArrayTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            int[] array = list.ToArray();

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);
        }

        [Test()]
        public void ToNativeArrayTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            using NativeArray<int> array = list.ToNativeArray();

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);

            Assert.AreEqual(5, array.Length);
        }

        [Test()]
        public void ToNativeArrayAndDisposeTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            using NativeArray<int> array = list.ToNativeArrayAndDispose();

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);

            Assert.AreEqual(5, array.Length);

            Assert.IsFalse(list.IsValid);
            Assert.IsTrue(array.IsValid);
            Assert.IsTrue(list.IsEmpty);
            Assert.AreEqual(0, list.Length);
            Assert.AreEqual(0, list.Capacity);
        }

        [Test()]
        unsafe public void GetUnsafePointerTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            int* ptr = list.GetUnsafePointer();

            for(int i = 0; i < list.Length; i++)
            {
                ptr[i] *= 2;
            }

            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(4, list[1]);
            Assert.AreEqual(6, list[2]);
            Assert.AreEqual(8, list[3]);
            Assert.AreEqual(10, list[4]);
        }

        [Test()]
        public void DisposeTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            list.Dispose();

            Assert.IsFalse(list.IsValid);
            Assert.IsTrue(list.IsEmpty);
            Assert.AreEqual(0, list.Length);
            Assert.AreEqual(0, list.Capacity);
        }

        [Test()]
        public void DisposeTest1()
        {
            NativeList<NativeString> list = new NativeList<NativeString>(4);
            list.Add("one");
            list.Add("two");
            list.Add("three");

            ref var p1 = ref list[0];
            ref var p2 = ref list[1];
            ref var p3 = ref list[2];

            list.Dispose(true);

            Assert.IsFalse(p1.IsValid);
            Assert.IsFalse(p2.IsValid);
            Assert.IsFalse(p3.IsValid);
        }

        [Test()]
        public void ToStringTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual("[1, 2, 3, 4, 5]", list.ToString());
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
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

        [Test()]
        public void IndexerTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });

            Assert.Throws<ArgumentOutOfRangeException>(() => { var e = list[-1]; });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var e = list[6]; });
        }

        [Test()]
        public void RangeIndexerTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            NativeSlice<int> slice = list[1..4];

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual(2, slice[0]);
            Assert.AreEqual(3, slice[1]);
            Assert.AreEqual(4, slice[2]);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, list[..].ToArray());
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, list[..3].ToArray());
            CollectionAssert.AreEqual(new int[] { 3, 4, 5 }, list[2..].ToArray());

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using NativeList<int> nativeArray = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
                NativeSlice<int> slice = list[3..6];
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using NativeList<int> nativeArray = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
                NativeSlice<int> slice = list[-1..];
            });
        }

        [Test()]
        public void CloneTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4 });
            using NativeList<int> clone = list.Clone();

            Assert.AreEqual(4, clone.Length);
            Assert.AreEqual(4, clone.Capacity);

            Assert.AreEqual(1, clone[0]);
            Assert.AreEqual(2, clone[1]);
            Assert.AreEqual(3, clone[2]);
            Assert.AreEqual(4, clone[3]);

            clone.Add(5);
            foreach(ref var e in clone)
            {
                e *= 2;
            }

            Assert.AreNotEqual(list.Length, clone.Length);
            Assert.AreNotEqual(list[0], clone[0]);
            Assert.AreNotEqual(list[1], clone[1]);
            Assert.AreNotEqual(list[2], clone[2]);
            Assert.AreNotEqual(list[3], clone[3]);
        }

        // Extensions

        [Test()]
        public void SortTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 4, 5, 3, 1, 2 });
            list.Sort();

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(4, list[3]);
            Assert.AreEqual(5, list[4]);
        }

        [Test()]
        public void SortTest1()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 4, 5, 3, 1, 2 });
            list.Sort(1, 3);

            Assert.AreEqual(4, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(5, list[3]);
            Assert.AreEqual(2, list[4]);
        }

        [Test()]
        public void SortByTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 4, 5, 3, 1, 2 });
            list.SortBy(e => e);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(4, list[3]);
            Assert.AreEqual(5, list[4]);
        }

        [Test()]
        public void SortByTest1()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 4, 5, 3, 1, 2 });
            list.SortBy(1, 3, e => e);

            Assert.AreEqual(4, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(5, list[3]);
            Assert.AreEqual(2, list[4]);
        }

        [Test()]
        public void SortByDecendingTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 4, 5, 3, 1, 2 });
            list.SortByDecending(e => e);

            Assert.AreEqual(5, list[0]);
            Assert.AreEqual(4, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(2, list[3]);
            Assert.AreEqual(1, list[4]);
        }

        [Test()]
        public void SortByDecendingTes1()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 4, 5, 3, 1, 2 });
            list.SortByDecending(1, 3, e => e);

            Assert.AreEqual(4, list[0]);
            Assert.AreEqual(5, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(1, list[3]);
            Assert.AreEqual(2, list[4]);
        }

        [Test()]
        public void FindAllTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            using NativeArray<int> result = list.FindAll(e => e % 2 == 0);

            Assert.IsTrue(result.Contains(2));
            Assert.IsTrue(result.Contains(4));
            Assert.IsTrue(result.Contains(6));
            Assert.IsTrue(result.Contains(8));
            Assert.IsTrue(result.Contains(10));
        }


        [Test()]
        public void FindFirstTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(2, list.FindFirst(e => e % 2 == 0));
            Assert.AreEqual(5, list.FindFirst(e => e > 4));

            Assert.IsNull(list.FindFirst(e => e == 10));
        }

        [Test()]
        public void FindLastTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(4, list.FindLast(e => e % 2 == 0));
            Assert.AreEqual(2, list.FindLast(e => e < 3));

            Assert.IsNull(list.FindLast(e => e == 10));
        }

        [Test()]
        public void FindFirstRefTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(2, list.FindFirstRef(e => e % 2 == 0));
            Assert.AreEqual(5, list.FindFirstRef(e => e > 4));

            Assert.Catch(() => list.FindFirstRef(e => e == 10));
        }

        [Test()]
        public void FindLastRefTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(4, list.FindLastRef(e => e % 2 == 0));
            Assert.AreEqual(2, list.FindLastRef(e => e < 3));

            Assert.Catch(() => list.FindLastRef(e => e == 10));
        }

        [Test()]
        public void AllMatchTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.IsTrue(list.AllMatch(e => e > 0));
            Assert.IsFalse(list.AllMatch(e => e > 2));
        }

        [Test()]
        public void NoneMatchTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.IsTrue(list.NoneMatch(e => e > 5));
            Assert.IsFalse(list.NoneMatch(e => e > 0));
        }

        [Test()]
        public void AnyMatchTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.IsTrue(list.AnyMatch(e => e == 5));
            Assert.IsFalse(list.AnyMatch(e => e < 0));
        }

        [Test()]
        public void BinarySearchTest()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            Assert.AreEqual(0, list.BinarySearch(1));
            Assert.AreEqual(1, list.BinarySearch(2));
            Assert.AreEqual(2, list.BinarySearch(3));
            Assert.AreEqual(3, list.BinarySearch(4));
            Assert.AreEqual(4, list.BinarySearch(5));
            Assert.AreEqual(5, list.BinarySearch(6));
            Assert.AreEqual(6, list.BinarySearch(7));
            Assert.AreEqual(7, list.BinarySearch(8));
            Assert.AreEqual(8, list.BinarySearch(9));
            Assert.AreEqual(9, list.BinarySearch(10));

            Assert.AreEqual(~0, list.BinarySearch(-1));
            Assert.AreEqual(~0, list.BinarySearch(0));
            Assert.AreEqual(~10, list.BinarySearch(11));
            Assert.AreEqual(~10, list.BinarySearch(12));
        }
    }
}