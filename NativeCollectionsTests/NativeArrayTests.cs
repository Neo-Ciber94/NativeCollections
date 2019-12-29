using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeArrayTests
    {
        [Test()]
        public void NativeArrayTest()
        {
            using NativeArray<int> array = new NativeArray<int>(4);
            Assert.AreEqual(4, array.Length);
            Assert.IsTrue(array.IsValid);
            Assert.IsFalse(array.IsEmpty);
        }

        [Test()]
        public void NativeArrayTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4 });
            Assert.AreEqual(4, array.Length);
            Assert.IsTrue(array.IsValid);
            Assert.IsFalse(array.IsEmpty);
        }

        [Test()]
        public void FillTest()
        {
            using NativeArray<int> array = new NativeArray<int>(4);
            array.Fill(5);

            foreach(var e in array)
            {
                Assert.AreEqual(5, e);
            }
        }

        [Test()]
        public void ReverseTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            array.Reverse();

            Assert.AreEqual(5, array[0]);
            Assert.AreEqual(4, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(2, array[3]);
            Assert.AreEqual(1, array[4]);
        }

        [Test()]
        public void ReverseTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            array.Reverse(2);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(5, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(3, array[4]);
        }

        [Test()]
        public void ReverseTest2()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            array.Reverse(2, 4);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(5, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(3, array[4]);
        }

        [Test()]
        public void IndexOfTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(4, array.IndexOf(5));
            Assert.AreEqual(3, array.IndexOf(4));
            Assert.AreEqual(2, array.IndexOf(3));
            Assert.AreEqual(1, array.IndexOf(2));
            Assert.AreEqual(0, array.IndexOf(1));

            Assert.AreEqual(-1, array.IndexOf(6));
            Assert.AreEqual(-1, array.IndexOf(7));
        }

        [Test()]
        public void IndexOfTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.Throws<ArgumentOutOfRangeException>(() => array.IndexOf(6, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => array.IndexOf(-1, 10));
        }

        [Test()]
        public void IndexOfTest2()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.Throws<ArgumentOutOfRangeException>(() => array.IndexOf(0, 6, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => array.IndexOf(-1, 3, 10));
        }

        [Test()]
        public void ContainsTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.IsTrue(array.Contains(1));
            Assert.IsTrue(array.Contains(2));
            Assert.IsTrue(array.Contains(3));
            Assert.IsTrue(array.Contains(4));
            Assert.IsTrue(array.Contains(5));
            Assert.IsFalse(array.Contains(6));
        }

        [Test()]
        public void LastIndexOfTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(4, array.LastIndexOf(5));
            Assert.AreEqual(3, array.LastIndexOf(4));
            Assert.AreEqual(2, array.LastIndexOf(3));
            Assert.AreEqual(1, array.LastIndexOf(2));
            Assert.AreEqual(0, array.LastIndexOf(1));

            Assert.AreEqual(-1, array.LastIndexOf(6));
            Assert.AreEqual(-1, array.LastIndexOf(7));
        }

        [Test()]
        public void LastIndexOfTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.Throws<ArgumentOutOfRangeException>(() => array.LastIndexOf(6, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => array.LastIndexOf(-1, 10));
        }

        [Test()]
        public void LastIndexOfTest2()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.Throws<ArgumentOutOfRangeException>(() => array.LastIndexOf(0, 6, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => array.LastIndexOf(-1, 3, 10));
        }

        [Test()]
        public void CopyToTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Span<int> span = stackalloc int[5];

            array.CopyTo(span);
            Assert.AreEqual(1, span[0]);
            Assert.AreEqual(2, span[1]);
            Assert.AreEqual(3, span[2]);
            Assert.AreEqual(4, span[3]);
            Assert.AreEqual(5, span[4]);
        }

        [Test()]
        public void CopyToTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Span<int> span = stackalloc int[5];

            array.CopyTo(span, 3);
            Assert.AreEqual(1, span[0]);
            Assert.AreEqual(2, span[1]);
            Assert.AreEqual(3, span[2]);
            Assert.AreEqual(0, span[3]);
            Assert.AreEqual(0, span[4]);
        }

        [Test()]
        public void CopyToTest2()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Span<int> span = stackalloc int[5];

            array.CopyTo(span, 2, 3);
            Assert.AreEqual(0, span[0]);
            Assert.AreEqual(0, span[1]);
            Assert.AreEqual(1, span[2]);
            Assert.AreEqual(2, span[3]);
            Assert.AreEqual(3, span[4]);
        }

        [Test()]
        public void CopyToTest3()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Span<int> span = stackalloc int[5];

            array.CopyTo(span, 2, 2, 3);
            Assert.AreEqual(0, span[0]);
            Assert.AreEqual(0, span[1]);
            Assert.AreEqual(3, span[2]);
            Assert.AreEqual(4, span[3]);
            Assert.AreEqual(5, span[4]);
        }

        [Test()]
        public void ToArrayTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, array.ToArray());
        }

        [Test()]
        public void ToStringTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual("[1, 2, 3, 4, 5]", array.ToString());
        }

        [Test()]
        public void DisposeTest()
        {
            NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            array.Dispose();

            Assert.AreEqual(0, array.Length);
            Assert.IsFalse(array.IsValid);
            Assert.IsTrue(array.IsEmpty);
        }

        [Test()]
        unsafe public void DisposeTest1()
        {
            NativeArray<NativeString> array = new NativeArray<NativeString>(3);
            array[0] = "Hello";
            array[1] = "Hola";
            array[2] = "Nihao";

            NativeString* s1 = (NativeString*)Unsafe.AsPointer(ref array[0]);
            NativeString* s2 = (NativeString*)Unsafe.AsPointer(ref array[1]);
            NativeString* s3 = (NativeString*)Unsafe.AsPointer(ref array[2]);

            Assert.IsTrue(s1->IsValid);
            Assert.IsTrue(s2->IsValid);
            Assert.IsTrue(s3->IsValid);

            array.Dispose(true);

            // This could fail, the memory could be taken for other process
            Assert.IsFalse(s1->IsValid);
            Assert.IsFalse(s2->IsValid);
            Assert.IsFalse(s3->IsValid);
        }

        [Test()]
        public void ToNativeListAndDisposeTest()
        {
            Assert.Fail("NativeList is not tested yet");
        }

        [Test()]
        unsafe public void GetUnsafePointerTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });

            int* pointer = array.GetUnsafePointer();
            for(int i = 0; i < 5; i++)
            {
                pointer[i]++;
            }

            Assert.AreEqual(2, array[0]);
            Assert.AreEqual(3, array[1]);
            Assert.AreEqual(4, array[2]);
            Assert.AreEqual(5, array[3]);
            Assert.AreEqual(6, array[4]);
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5 });
            var enumerator = array.GetEnumerator();

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
        public void SortTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 4, 1, 3, 5, 2 });
            array.Sort();

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);
        }

        [Test()]
        public void SortTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 4, 5, 3, 1, 2 });
            array.Sort(1, 3);

            Assert.AreEqual(4, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(5, array[3]);
            Assert.AreEqual(2, array[4]);
        }

        [Test()]
        public void SortByTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 4, 1, 3, 5, 2 });
            array.SortBy(e => e);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);
        }

        [Test()]
        public void SortByTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 4, 5, 3, 1, 2 });
            array.SortBy(1, 3, e => e);

            Assert.AreEqual(4, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(5, array[3]);
            Assert.AreEqual(2, array[4]);
        }

        [Test()]
        public void SortByDecendingTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 4, 1, 3, 5, 2 });
            array.SortByDecending(e => e);

            Assert.AreEqual(5, array[0]);
            Assert.AreEqual(4, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(2, array[3]);
            Assert.AreEqual(1, array[4]);
        }

        [Test()]
        public void SortByDecendingTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 4, 5, 3, 1, 2 });
            array.SortByDecending(1, 3, e => e);

            Assert.AreEqual(4, array[0]);
            Assert.AreEqual(5, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(1, array[3]);
            Assert.AreEqual(2, array[4]);
        }

        [Test()]
        public void BinarySearchTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            Assert.AreEqual(4, array.BinarySearch(5));
            Assert.AreEqual(8, array.BinarySearch(9));

            Assert.AreEqual(~0, array.BinarySearch(0));
            Assert.AreEqual(~9, array.BinarySearch(11));
        }
    }
}