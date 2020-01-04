using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeSliceTests
    {
        [Test()]
        unsafe public void NativeSliceTest()
        {
            int* p = stackalloc int[] { 1, 2, 3, 4 };
            NativeSlice<int> slice = new NativeSlice<int>(p, 4);

            Assert.IsFalse(slice.IsEmpty);
            Assert.AreEqual(4, slice.Length);

            slice[0]++;
            slice[1]++;
            slice[2]++;
            slice[3]++;

            Assert.AreEqual(2, p[0]);
            Assert.AreEqual(3, p[1]);
            Assert.AreEqual(4, p[2]);
            Assert.AreEqual(5, p[3]);

            Assert.AreEqual(2, slice[0]);
            Assert.AreEqual(3, slice[1]);
            Assert.AreEqual(4, slice[2]);
            Assert.AreEqual(5, slice[3]);
        }

        [Test()]
        public void NativeSliceTest1()
        {
            int[] array = new int[] { 1, 2, 3, 4 };
            NativeSlice<int> slice = new NativeSlice<int>(array);

            Assert.IsFalse(slice.IsEmpty);
            Assert.AreEqual(4, slice.Length);

            slice[0]++;
            slice[1]++;
            slice[2]++;
            slice[3]++;

            Assert.AreEqual(2, array[0]);
            Assert.AreEqual(3, array[1]);
            Assert.AreEqual(4, array[2]);
            Assert.AreEqual(5, array[3]);

            Assert.AreEqual(2, slice[0]);
            Assert.AreEqual(3, slice[1]);
            Assert.AreEqual(4, slice[2]);
            Assert.AreEqual(5, slice[3]);
        }

        [Test()]
        public void NativeSliceTest2()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5 };
            NativeSlice<int> slice = new NativeSlice<int>(array, 3);

            Assert.IsFalse(slice.IsEmpty);
            Assert.AreEqual(3, slice.Length);

            slice[0]++;
            slice[1]++;
            slice[2]++;

            Assert.AreEqual(2, array[0]);
            Assert.AreEqual(3, array[1]);
            Assert.AreEqual(4, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);

            Assert.AreEqual(2, slice[0]);
            Assert.AreEqual(3, slice[1]);
            Assert.AreEqual(4, slice[2]);
        }

        [Test()]
        public void NativeSliceTest3()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5 };
            NativeSlice<int> slice = new NativeSlice<int>(array, 1, 3);

            Assert.IsFalse(slice.IsEmpty);
            Assert.AreEqual(3, slice.Length);

            slice[0]++;
            slice[1]++;
            slice[2]++;

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(3, array[1]);
            Assert.AreEqual(4, array[2]);
            Assert.AreEqual(5, array[3]);
            Assert.AreEqual(5, array[4]);

            Assert.AreEqual(3, slice[0]);
            Assert.AreEqual(4, slice[1]);
            Assert.AreEqual(5, slice[2]);
        }

        [Test()]
        public void CopyToTest()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };
            int[] array = new int[5];

            slice.CopyTo(array);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);
        }

        [Test()]
        public void CopyToTest1()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };
            int[] array = new int[5];

            slice.CopyTo(array, 3);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(0, array[3]);
            Assert.AreEqual(0, array[4]);
        }

        [Test()]
        public void CopyToTest2()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };
            int[] array = new int[5];

            slice.CopyTo(array, 1, 3);
            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(2, array[2]);
            Assert.AreEqual(3, array[3]);
            Assert.AreEqual(0, array[4]);
        }

        [Test()]
        public void ToArrayTest()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4 };
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, slice.ToArray());
        }

        [Test()]
        public void IndexerTest()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4 };
            slice[^0]++;
            slice[^1]++;
            slice[^2]++;
            slice[^3]++;

            Assert.AreEqual(5, slice[^0]);
            Assert.AreEqual(4, slice[^1]);
            Assert.AreEqual(3, slice[^2]);
            Assert.AreEqual(2, slice[^3]);
        }

        [Test()]
        public void SliceTest()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };

            NativeSlice<int> s2 = slice.Slice(2);
            Assert.AreEqual(3, s2.Length);

            Assert.AreEqual(3, s2[0]);
            Assert.AreEqual(4, s2[1]);
            Assert.AreEqual(5, s2[2]);
        }

        [Test()]
        public void SliceTest1()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };

            NativeSlice<int> s2 = slice.Slice(1, 3);

            Assert.AreEqual(3, s2.Length);
            Assert.AreEqual(2, s2[0]);
            Assert.AreEqual(3, s2[1]);
            Assert.AreEqual(4, s2[2]);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                NativeSlice<int> nativeSlice = new int[] { 1, 2, 3, 4, 5 };
                var s = nativeSlice.Slice(0, 6);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                NativeSlice<int> nativeSlice = new int[] { 1, 2, 3, 4, 5 };
                var s = nativeSlice.Slice(2, 4);
            });
        }

        [Test()]
        public void SliceTest2()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };
            NativeSlice<int> s1 = slice[1..4];

            Assert.AreEqual(3, s1.Length);
            Assert.AreEqual(2, s1[0]);
            Assert.AreEqual(3, s1[1]);
            Assert.AreEqual(4, s1[2]);
        }

        [Test()]
        public void SliceTest3()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };
            NativeSlice<int> s1 = slice[1..];

            Assert.AreEqual(4, s1.Length);
            Assert.AreEqual(2, s1[0]);
            Assert.AreEqual(3, s1[1]);
            Assert.AreEqual(4, s1[2]);
            Assert.AreEqual(5, s1[3]);
        }

        [Test()]
        public void SliceTest4()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };
            NativeSlice<int> s1 = slice[..4];

            Assert.AreEqual(4, s1.Length);
            Assert.AreEqual(1, s1[0]);
            Assert.AreEqual(2, s1[1]);
            Assert.AreEqual(3, s1[2]);
            Assert.AreEqual(4, s1[3]);
        }

        [Test()]
        public void SliceTest5()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };
            NativeSlice<int> s1 = slice[..];

            Assert.AreEqual(5, s1.Length);
            Assert.AreEqual(1, s1[0]);
            Assert.AreEqual(2, s1[1]);
            Assert.AreEqual(3, s1[2]);
            Assert.AreEqual(4, s1[3]);
            Assert.AreEqual(5, s1[4]);
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4 };
            var enumerator = slice.GetEnumerator();

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

        // Extensions

        [Test()]
        public void ToNativeArrayTest()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };
            using NativeArray<int> array = slice[1..4].ToNativeArray();

            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(2, array[0]);
            Assert.AreEqual(3, array[1]);
            Assert.AreEqual(4, array[2]);
        }

        [Test()]
        public void ToNativeListTest()
        {
            NativeSlice<int> slice = new int[] { 1, 2, 3, 4, 5 };
            using NativeList<int> list = slice[1..4].ToNativeList();
            list.Add(5);

            Assert.AreEqual(4, list.Length);
            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(3, list[1]);
            Assert.AreEqual(4, list[2]);
            Assert.AreEqual(5, list[3]);
        }
    }
}