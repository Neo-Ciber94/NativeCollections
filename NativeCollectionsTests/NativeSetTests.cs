using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeSetTests
    {
        [Test()]
        public void NativeSetTest()
        {
            using NativeSet<int> set = new NativeSet<int>(4);
            Assert.IsTrue(set.IsEmpty);
            Assert.IsTrue(set.IsValid);
            Assert.AreEqual(0, set.Length);
            Assert.AreEqual(4, set.Capacity);
        }

        [Test()]
        public void NativeSetTest1()
        {
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 2, 3, 4 });
            Assert.IsFalse(set.IsEmpty);
            Assert.IsTrue(set.IsValid);
            Assert.AreEqual(4, set.Length);
            Assert.AreEqual(4, set.Capacity);
        }

        [Test()]
        public void GetAllocatorTest()
        {
            using NativeSet<int> set = new NativeSet<int>(4);

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
            using NativeSet<int> set = new NativeSet<int>(4);
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
        public void AddRangeTest()
        {
            using NativeSet<int> set = new NativeSet<int>(4);
            Assert.AreEqual(4, set.AddRange(stackalloc[] { 1, 2, 3, 4 }));
            Assert.AreEqual(1, set.AddRange(stackalloc[] { 1, 2, 5 }));

            Assert.AreEqual(5, set.Length);
            Assert.AreEqual(8, set.Capacity);
        }

        [Test()]
        public void RemoveTest()
        {
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 2, 3, 4 });
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
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
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
            using NativeSet<int> set = new NativeSet<int>(4);
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
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 2, 3 });

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
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 2, 3 });
            Assert.IsTrue(set.ContainsAll(stackalloc int[] { 1, 2, 3 }));
            Assert.IsFalse(set.ContainsAll(stackalloc int[] { 1, 2, 3, 5 }));
        }

        [Test()]
        public void UnionWithTest()
        {
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 2, 3 });
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
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
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
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
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
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
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
            using NativeSet<int> set = new NativeSet<int>(10);
            set.Add(1);
            set.Add(2);
            set.Add(3);

            set.TrimExcess();
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual(3, set.Capacity);
        }

        [Test()]
        public void TrimExcessTest1()
        {
            using NativeSet<int> set = new NativeSet<int>(10);
            set.Add(1);
            set.Add(2);
            set.Add(3);

            set.TrimExcess(6);
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual(6, set.Capacity);
        }

        [Test()]
        public void EnsureCapacityTest()
        {
            using NativeSet<int> set = new NativeSet<int>(5);
            set.Add(1);
            set.Add(2);
            set.Add(3);

            set.EnsureCapacity(10);
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual(10, set.Capacity);
        }

        [Test()]
        public void CopyToTest()
        {
            using NativeSet<int> set = new NativeSet<int>(6);
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(4);
            set.Add(5);

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
            using NativeSet<int> set = new NativeSet<int>(6);
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(4);

            int[] array = set.ToArray();
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void ToNativeArrayTest()
        {
            using NativeSet<int> set = new NativeSet<int>(6);
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(4);

            using NativeArray<int> array = set.ToNativeArray();
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
        }

        [Test()]
        public void ToNativeArrayAndDisposeTest()
        {
            using NativeSet<int> set = new NativeSet<int>(6);
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(4);

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
            using NativeSet<int> set = new NativeSet<int>(6);
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(4);

            Assert.AreEqual("[1, 2, 3, 4]", set.ToString());
        }

        [Test()]
        public void DisposeTest()
        {
            using NativeSet<int> set = new NativeSet<int>(6);
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
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
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