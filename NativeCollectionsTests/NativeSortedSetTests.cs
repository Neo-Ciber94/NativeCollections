using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeSortedSetTests
    {
        [Test()]
        public void NativeSortedSetTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(4);
            Assert.IsTrue(set.IsEmpty);
            Assert.IsTrue(set.IsValid);
            Assert.AreEqual(0, set.Length);
            Assert.AreEqual(4, set.Capacity);
        }

        [Test()]
        public void NativeSortedSetTest1()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 2, 3, 4, 4 });
            Assert.IsFalse(set.IsEmpty);
            Assert.IsTrue(set.IsValid);
            Assert.AreEqual(4, set.Length);
            Assert.AreEqual(6, set.Capacity);
        }

        [Test()]
        public void GetAllocatorTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(4);

            unsafe
            {
                var allocator = set.GetAllocator();
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
        public void AddTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(4);
            Assert.IsTrue(set.Add(1));
            Assert.IsTrue(set.Add(2));
            Assert.IsTrue(set.Add(3));
            Assert.IsTrue(set.Add(4));

            Assert.AreEqual(4, set.Length);
            Assert.AreEqual(4, set.Capacity);

            Assert.IsFalse(set.Add(1));
            Assert.IsFalse(set.Add(2));
            Assert.IsFalse(set.Add(3));
            Assert.IsFalse(set.Add(4));

            Assert.IsTrue(set.Add(5));
            Assert.AreEqual(5, set.Length);
            Assert.AreEqual(8, set.Capacity);
        }

        [Test()]
        public void AddAllTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(4);
            Assert.AreEqual(4, set.AddAll(stackalloc[] { 1, 2, 3, 4 }));
            Assert.AreEqual(3, set.AddAll(stackalloc[] { 1, 2, 5 }));

            Assert.AreEqual(5, set.Length);
            Assert.AreEqual(8, set.Capacity);
        }

        [Test()]
        public void RemoveTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 3, 4 });
            Assert.IsTrue(set.Remove(1));
            Assert.AreEqual(3, set.Length);

            Assert.IsTrue(set.Remove(2));
            Assert.AreEqual(2, set.Length);

            Assert.IsTrue(set.Remove(3));
            Assert.AreEqual(1, set.Length);

            Assert.IsTrue(set.Remove(4));
            Assert.AreEqual(0, set.Length);

            Assert.IsFalse(set.Remove(1));
            Assert.IsFalse(set.Remove(0));
            Assert.AreEqual(0, set.Length);
        }

        [Test()]
        public void RemoveIfTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            set.RemoveIf((e) => e % 2 != 0);

            Assert.IsTrue(set.Contains(2));
            Assert.IsTrue(set.Contains(4));
            Assert.IsTrue(set.Contains(6));
            Assert.IsTrue(set.Contains(8));
            Assert.IsTrue(set.Contains(10));

            Assert.IsFalse(set.Contains(1));
            Assert.IsFalse(set.Contains(3));
            Assert.IsFalse(set.Contains(5));
            Assert.IsFalse(set.Contains(9));
        }

        [Test()]
        public void ClearTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(4);
            Assert.IsTrue(set.Add(1));
            Assert.IsTrue(set.Add(2));
            Assert.IsTrue(set.Add(3));

            set.Clear();

            Assert.AreEqual(0, set.Length);
            Assert.AreEqual(4, set.Capacity);

            Assert.IsTrue(set.Add(1));
            Assert.IsTrue(set.Add(2));
            Assert.IsTrue(set.Add(3));

            Assert.IsTrue(set.Contains(1));
            Assert.IsTrue(set.Contains(2));
            Assert.IsTrue(set.Contains(3));

            Assert.IsFalse(set.Contains(0));
            Assert.IsFalse(set.Contains(5));
        }

        [Test()]
        public void ContainsTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 3 });

            Assert.IsTrue(set.Contains(1));
            Assert.IsTrue(set.Contains(2));
            Assert.IsTrue(set.Contains(3));

            Assert.IsFalse(set.Contains(0));
            Assert.IsFalse(set.Contains(5));

            set.Remove(1);
            Assert.IsFalse(set.Contains(1));

            set.Remove(2);
            Assert.IsFalse(set.Contains(2));

            set.Remove(3);
            Assert.IsFalse(set.Contains(3));
        }

        [Test()]
        public void ContainsAllTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 3 });
            Assert.IsTrue(set.ContainsAll(stackalloc int[] { 1, 2, 3 }));
            Assert.IsFalse(set.ContainsAll(stackalloc int[] { 1, 2, 3, 5 }));
        }

        [Test()]
        public void GetRangeTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6});
            CollectionAssert.AreEqual(new int[] { 2, 3, 4, 5 }, set.GetRange(2, 5).ToArray());
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, set.GetRange(1, 6).ToArray());
            CollectionAssert.AreEqual(new int[0] , set.GetRange(4, 4).ToArray());

            CollectionAssert.AreEqual(new int[0], set.GetRange(7, 10).ToArray());
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, set.GetRange(-1, 3).ToArray());
            CollectionAssert.AreEqual(new int[] { 4, 5, 6 }, set.GetRange(4, 10).ToArray());
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, set.GetRange(-3, 8).ToArray());
        }

        [Test()]
        public void GetRangeTest1()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6 });

            CollectionAssert.AreEqual(new int[] { 2, 3, 4, 5 }, set.GetRange(1..5).ToArray());
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, set.GetRange(0..6).ToArray());
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, set.GetRange(..).ToArray());

            CollectionAssert.AreEqual(new int[0], set.GetRange(2..2).ToArray());
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, set.GetRange(0..3).ToArray());
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, set.GetRange(..3).ToArray());
            CollectionAssert.AreEqual(new int[] { 3, 4, 5, 6 }, set.GetRange(2..6).ToArray());
            CollectionAssert.AreEqual(new int[] { 3, 4, 5, 6 }, set.GetRange(2..).ToArray());
        }

        [Test()]
        public void UnionWithTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 3 });
            set.UnionWith(stackalloc int[] { 1, 2, 3, 4, 5, 6 });

            Assert.AreEqual(6, set.Length);

            Assert.IsTrue(set.Contains(1));
            Assert.IsTrue(set.Contains(2));
            Assert.IsTrue(set.Contains(3));
            Assert.IsTrue(set.Contains(4));
            Assert.IsTrue(set.Contains(5));
            Assert.IsTrue(set.Contains(6));
        }

        [Test()]
        public void IntersectionWithTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            set.IntersectionWith(stackalloc int[] { 2, 3, 4 });

            Assert.AreEqual(3, set.Length);

            Assert.IsTrue(set.Contains(2));
            Assert.IsTrue(set.Contains(3));
            Assert.IsTrue(set.Contains(4));

            Assert.IsFalse(set.Contains(1));
            Assert.IsFalse(set.Contains(5));
        }

        [Test()]
        public void DifferenceWithTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            set.DifferenceWith(stackalloc int[] { 4, 5, 6, 7 });

            Assert.AreEqual(3, set.Length);

            Assert.IsTrue(set.Contains(1));
            Assert.IsTrue(set.Contains(2));
            Assert.IsTrue(set.Contains(3));

            Assert.IsFalse(set.Contains(4));
            Assert.IsFalse(set.Contains(5));
        }

        [Test()]
        public void SymmetricDifferenceWithTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            set.SymmetricDifferenceWith(stackalloc int[] { 2, 3, 4});

            Assert.AreEqual(2, set.Length);

            Assert.IsTrue(set.Contains(1));
            Assert.IsTrue(set.Contains(5));

            Assert.IsFalse(set.Contains(2));
            Assert.IsFalse(set.Contains(3));
            Assert.IsFalse(set.Contains(4));
        }

        [Test()]
        public void TrimExcessTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(10);
            set.Add(1);
            set.Add(3);
            set.Add(2);
            set.Add(3);

            set.TrimExcess();
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual(3, set.Capacity);
        }

        [Test()]
        public void TrimExcessTest1()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(10);
            set.Add(1);
            set.Add(3);
            set.Add(2);
            set.Add(3);

            set.TrimExcess(6);
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual(6, set.Capacity);
        }

        [Test()]
        public void EnsureCapacityTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(5);
            set.Add(1);
            set.Add(2);
            set.Add(2);
            set.Add(3);

            set.EnsureCapacity(10);
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual(10, set.Capacity);
        }

        [Test()]
        public void CopyToTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(6);
            set.Add(2);
            set.Add(1);
            set.Add(3);
            set.Add(5);
            set.Add(4);

            Span<int> span = stackalloc int[5];
            set.CopyTo(span, 1, 3);

            Assert.AreEqual(0, span[0]);
            Assert.AreEqual(1, span[1]);
            Assert.AreEqual(2, span[2]);
            Assert.AreEqual(3, span[3]);
            Assert.AreEqual(0, span[0]);
        }

        [Test()]
        public void ToArrayTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(6);
            set.Add(4);
            set.Add(2);
            set.Add(1);
            set.Add(2);
            set.Add(3);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, set.ToArray());
        }

        [Test()]
        public void ToNativeArrayTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(6);
            set.Add(2);
            set.Add(1);
            set.Add(4);
            set.Add(3);

            using NativeArray<int> array = set.ToNativeArray();
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void ToNativeArrayAndDisposeTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(6);
            set.Add(2);
            set.Add(1);
            set.Add(4);
            set.Add(3);

            using NativeArray<int> array = set.ToNativeArrayAndDispose();

            Assert.IsFalse(set.IsValid);
            Assert.IsTrue(set.IsEmpty);
            Assert.AreEqual(0, set.Length);
            Assert.AreEqual(0, set.Capacity);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void ToStringTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(6);
            set.Add(2);
            set.Add(1);
            set.Add(4);
            set.Add(3);

            Assert.AreEqual("[1, 2, 3, 4]", set.ToString());
        }

        [Test()]
        public void DisposeTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(6);
            set.Add(1);
            set.Add(2);
            set.Add(3);

            set.Dispose();
            Assert.IsFalse(set.IsValid);
            Assert.IsTrue(set.IsEmpty);
            Assert.AreEqual(0, set.Length);
            Assert.AreEqual(0, set.Capacity);
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 5, 3, 2, 4, 1 });
            var enumerator = set.GetEnumerator();

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