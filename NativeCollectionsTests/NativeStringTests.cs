using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeStringTests
    {
        [Test()]
        public void NativeStringTest()
        {
            using NativeString str = "Hola";
            Assert.AreEqual("Hola", str);
            Assert.AreEqual(4, str.Length);
            Assert.IsTrue(str.IsValid);
            Assert.IsFalse(str.IsEmpty);

            using NativeString emptyString = string.Empty;
            Assert.AreEqual("", emptyString);
            Assert.AreEqual(0, emptyString.Length);
            Assert.IsFalse(emptyString.IsValid);
            Assert.IsTrue(emptyString.IsEmpty);
        }

        [Test()]
        public void NativeStringTest1()
        {
            Span<char> span = stackalloc char[] { 'H', 'e', 'l', 'l', 'o' };
            using NativeString str = new NativeString(span);
            Assert.AreEqual("Hello", str);
            Assert.AreEqual(5, str.Length);
            Assert.IsTrue(str.IsValid);
            Assert.IsFalse(str.IsEmpty);
        }

        [Test()]
        public void NativeStringTest2()
        {
            ReadOnlySpan<char> span = stackalloc char[] { 'H', 'e', 'l', 'l', 'o' };
            using NativeString str = new NativeString(span);
            Assert.AreEqual("Hello", str);
            Assert.AreEqual(5, str.Length);
            Assert.IsTrue(str.IsValid);
            Assert.IsFalse(str.IsEmpty);
        }

        [Test()]
        unsafe public void NativeStringTest3()
        {
            char* p = stackalloc char[] { 'H', 'e', 'l', 'l', 'o' };
            using NativeString str = new NativeString(p, 5);
            Assert.AreEqual("Hello", str);
            Assert.AreEqual(5, str.Length);
            Assert.IsTrue(str.IsValid);
            Assert.IsFalse(str.IsEmpty);
        }

        [Test()]
        public void GetAllocatorTest()
        {
            using NativeString str = new NativeString("Hello".AsSpan());

            unsafe
            {
                var allocator = str.GetAllocator();
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
        public void CopyToTest()
        {
            using NativeString str = new NativeString("Hello".AsSpan());
            Span<char> span = stackalloc char[5];

            str.CopyTo(span, 0, 5);
            Assert.AreEqual('H', span[0]);
            Assert.AreEqual('e', span[0]);
            Assert.AreEqual('l', span[0]);
            Assert.AreEqual('l', span[0]);
            Assert.AreEqual('o', span[0]);
        }

        [Test()]
        public void AsSpanTest()
        {
            using NativeString str = "Hello";
            ReadOnlySpan<char> span = str.AsSpan();
            Assert.AreEqual(str.ToString(), span.ToString());
        }

        [Test()]
        public void ToStringTest()
        {
            using NativeString str = "Ohaiyoo";
            Assert.AreEqual("Ohaiyoo", str.ToString());
        }

        [Test()]
        public void DisposeTest()
        {
            NativeString str = "Hello";
            str.Dispose();

            Assert.AreEqual("", str);
            Assert.AreEqual(0, str.Length);
            Assert.IsFalse(str.IsValid);
            Assert.IsTrue(str.IsEmpty);
        }

        [Test()]
        public void ToArrayTest()
        {
            using NativeString str = "Hello";
            CollectionAssert.AreEqual(new char[] { 'H', 'e', 'l', 'l', 'o' }, str.ToArray());
        }

        [Test()]
        public void ToNativeArrayTest()
        {
            using NativeString str = "Hola";
            using NativeArray<char> array = str.ToNativeArray();

            Assert.AreEqual(str[0], array[0]);
            Assert.AreEqual(str[1], array[1]);
            Assert.AreEqual(str[2], array[2]);
            Assert.AreEqual(str[3], array[3]);

            Assert.AreEqual(4, str.Length);
            Assert.IsTrue(str.IsValid);
            Assert.IsFalse(str.IsEmpty);
        }

        [Test()]
        public void ToNativeArrayAndDisposeTest()
        {
            using NativeString str = "Hola";
            using NativeArray<char> array = str.ToNativeArrayAndDispose();

            Assert.AreEqual('H', array[0]);
            Assert.AreEqual('o', array[1]);
            Assert.AreEqual('l', array[2]);
            Assert.AreEqual('a', array[3]);

            Assert.AreEqual(0, str.Length);
            Assert.IsFalse(str.IsValid);
            Assert.IsTrue(str.IsEmpty);
        }

        [Test()]
        public void EqualsTest()
        {
            using NativeString str = "Hola";
            object s = "Hola";
            object s1 = "Hello";

            Assert.IsTrue(str.Equals(s));
            Assert.IsFalse(str.Equals(s1));
        }

        [Test()]
        public void EqualsTest1()
        {
            using NativeString str = "Hola";
            Assert.IsTrue(str.Equals(new NativeString("Hola")));
            Assert.IsFalse(str.Equals(new NativeString("Hello")));
        }

        [Test()]
        public void EqualsTest2()
        {
            using NativeString str = "Hola";
            Assert.IsTrue(str.Equals("Hola"));
            Assert.IsFalse(str.Equals("Hello"));
        }

        [Test()]
        public void CompareToTest()
        {
            using NativeString str = "Hola";
            Assert.AreEqual(1, str.CompareTo(new NativeString("Hi")));
            Assert.AreEqual(1, str.CompareTo(new NativeString("Hello")));
            Assert.AreEqual(0, str.CompareTo(new NativeString("Hola")));
        }

        [Test()]
        public void CompareToTest1()
        {
            using NativeString str = "Hola";
            Assert.AreEqual(1, str.CompareTo("Hi"));
            Assert.AreEqual(1, str.CompareTo("Hello"));
            Assert.AreEqual(0, str.CompareTo("Hola"));
        }

        [Test()]
        public void CompareToTest2()
        {
            using NativeString str = "Hola";
            Assert.AreEqual(0, str.CompareTo("HOLA", StringComparison.CurrentCultureIgnoreCase));
        }

        [Test()]
        public void GetHashCodeTest()
        {
            NativeString str = "Hola";
            Assert.NotZero(str.GetHashCode());
            str.Dispose();

            Assert.Zero(str.GetHashCode());
        }

        [Test()]
        public void CloneTest()
        {
            using NativeString str = new NativeString("Adios");
            using NativeString clone = str.Clone();

            Assert.AreEqual(str, clone);
        }
    }
}