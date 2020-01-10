using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using NativeCollections.Utility;

namespace NativeCollections.Tests
{

    [TestFixture()]
    public class NativeMapTests
    {
        [Test()]
        public void NativeMapTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            Assert.AreEqual(4, map.Capacity);
            Assert.AreEqual(0, map.Length);
            Assert.IsTrue(map.IsValid);
            Assert.IsTrue(map.IsEmpty);

            map.Dispose();
            Assert.AreEqual(0, map.Capacity);
            Assert.AreEqual(0, map.Length);
            Assert.IsFalse(map.IsValid);
            Assert.IsTrue(map.IsEmpty);

            map.DisposeMapAndValues();
        }

        [Test()]
        public void NativeMapTest2()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(stackalloc (int, NativeString)[] { (0, "cero"), (1, "uno"), (2, "dos") });

            Assert.AreEqual(3, map.Capacity);
            Assert.AreEqual(3, map.Length);
            Assert.IsTrue(map.IsValid);
            Assert.IsFalse(map.IsEmpty);

            map.DisposeMapAndValues();
        }

        [Test()]
        public void GetAllocatorTest()
        {
            using NativeMap<int, char> map = new NativeMap<int, char>(4);

            unsafe
            {
                var allocator = map.GetAllocator();
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
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");
            map.Add(4, "four");

            Assert.AreEqual(4, map.Capacity);
            Assert.AreEqual(4, map.Length);

            Assert.Catch(() => map.Add(0, "cero"));
            Assert.Catch(() => map.Add(1, "uno"));
            Assert.Catch(() => map.Add(2, "dos"));

            // ContainsKey
            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));
            Assert.IsFalse(map.ContainsKey(3));

            // ContainsValue
            Assert.IsTrue(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsTrue(map.ContainsValue("two"));
            Assert.IsFalse(map.ContainsValue("three"));

            map.Add(3, "three"); //Resize
            Assert.AreEqual(8, map.Capacity);
            Assert.AreEqual(5, map.Length);

            // ContainsKey
            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));
            Assert.IsTrue(map.ContainsKey(3));

            // ContainsValue
            Assert.IsTrue(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsTrue(map.ContainsValue("two"));
            Assert.IsTrue(map.ContainsValue("three"));

            map.DisposeMapAndValues();
        }

        [Test()]
        public void AddOrUpdateTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.AddOrUpdate(0, "zero");
            map.AddOrUpdate(1, "one");
            map.AddOrUpdate(2, "two");

            Assert.AreEqual(4, map.Capacity);
            Assert.AreEqual(3, map.Length);

            // ContainsKey
            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));

            // ContainsValue
            Assert.IsTrue(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsTrue(map.ContainsValue("two"));

            map.AddOrUpdate(0, "cero");
            map.AddOrUpdate(1, "uno");
            map.AddOrUpdate(2, "dos");

            // ContainsKey
            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));

            // ContainsValue
            Assert.IsFalse(map.ContainsValue("zero"));
            Assert.IsFalse(map.ContainsValue("one"));
            Assert.IsFalse(map.ContainsValue("two"));

            Assert.IsTrue(map.ContainsValue("cero"));
            Assert.IsTrue(map.ContainsValue("uno"));
            Assert.IsTrue(map.ContainsValue("dos"));

            map.DisposeMapAndValues();
        }

        [Test()]
        public void ReplaceTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            Assert.AreEqual(4, map.Capacity);
            Assert.AreEqual(3, map.Length);

            // ContainsKey
            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));

            // ContainsValue
            Assert.IsTrue(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsTrue(map.ContainsValue("two"));

            map.Replace(0, "cero");
            map.Replace(1, "uno");
            map.Replace(2, "dos");

            Assert.IsFalse(map.Replace(3, "three"));
            Assert.IsFalse(map.Replace(4, "four"));
            Assert.IsFalse(map.Replace(5, "five"));

            map.DisposeMapAndValues();
        }

        [Test()]
        public void RemoveTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            //Assert.AreEqual(4, map.Capacity);
            Assert.AreEqual(3, map.Length);

            map.Remove(0);
            map.Remove(2);

            Assert.AreEqual(1, map.Length);

            // ContainsKey
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsFalse(map.ContainsKey(0));
            Assert.IsFalse(map.ContainsKey(2));

            // ContainsValue
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsFalse(map.ContainsValue("zero"));
            Assert.IsFalse(map.ContainsValue("two"));

            Assert.AreEqual(4, map.Capacity);
            Assert.AreEqual(1, map.Length);

            map.Add(0, "zero");
            map.Add(2, "two");
            map.Add(3, "three");
            map.Add(4, "four");  // Resize

            Assert.AreEqual(8, map.Capacity);
            Assert.AreEqual(5, map.Length);

            map.Remove(0);
            map.Remove(4);

            Assert.AreEqual(3, map.Length);

            // ContainsKey
            Assert.IsFalse(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));
            Assert.IsTrue(map.ContainsKey(3));
            Assert.IsFalse(map.ContainsKey(4));

            // ContainsValue
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsFalse(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("two"));
            Assert.IsTrue(map.ContainsValue("three"));
            Assert.IsFalse(map.ContainsValue("four"));

            map.DisposeMapAndValues();
        }

        [Test()]
        public void ClearTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            map.Clear();

            Assert.AreEqual(0, map.Length);
            Assert.AreEqual(4, map.Capacity);

            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));
            Assert.IsFalse(map.ContainsKey(3));
        }

        [Test()]
        public void TryGetValueTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            if (map.TryGetValue(1, out NativeString value))
            {
                Assert.AreEqual("one", value.ToString());
            }

            if (map.TryGetValue(4, out NativeString otherValue))
            {
                Assert.AreEqual(default(NativeString), otherValue);
            }

            map.DisposeMapAndValues();
        }

        [Test()]
        public void TryGetValueReferenceTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);

            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            if (map.TryGetValueReference(1, out var value))
            {
                Assert.AreEqual("one", value.Value.ToString());
            }

            if (map.TryGetValueReference(4, out var otherValue))
            {
                Assert.IsFalse(otherValue.IsNull);
            }

            map.DisposeMapAndValues();
        }

        [Test()]
        public void GetValueOrDefaultTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            var value0 = new NativeString("zero");
            var value3 = new NativeString("three");

            Assert.AreEqual(value0, map.GetValueOrDefault(0, value3));
            Assert.AreEqual(value3, map.GetValueOrDefault(3, value3));

            value0.Dispose();
            value3.Dispose();

            map.DisposeMapAndValues();
        }

        [Test()]
        public void ContainsKeyTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            // Looking for default values
            Assert.IsFalse(map.ContainsKey(0));

            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));
            Assert.IsFalse(map.ContainsKey(3));

            map.DisposeMapAndValues();
        }

        [Test()]
        public void ContainsKeyTest1()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);

            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));
            Assert.IsFalse(map.ContainsKey(3));

            Assert.IsTrue(map.Remove(0));
            Assert.IsFalse(map.ContainsKey(0));

            Assert.IsTrue(map.Remove(1));
            Assert.IsFalse(map.ContainsKey(1));

            Assert.IsTrue(map.Remove(2));
            Assert.IsFalse(map.ContainsKey(2));

            map.DisposeMapAndValues();
        }

        [Test()]
        public void ContainsValueTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);

            // Looking for default values
            Assert.IsFalse(map.ContainsValue(string.Empty));
            Assert.IsFalse(map.ContainsValue("null"));

            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            Assert.IsTrue(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsTrue(map.ContainsValue("two"));
            Assert.IsFalse(map.ContainsValue("three"));

            map.DisposeMapAndValues();
        }

        [Test()]
        public void ContainsValueTest1()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);

            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            Assert.IsTrue(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsTrue(map.ContainsValue("two"));
            Assert.IsFalse(map.ContainsValue("three"));

            Assert.IsTrue(map.Remove(0));
            Assert.IsFalse(map.ContainsValue("zero"));

            Assert.IsTrue(map.Remove(1));
            Assert.IsFalse(map.ContainsValue("one"));

            Assert.IsTrue(map.Remove(2));
            Assert.IsFalse(map.ContainsValue("two"));

            map.DisposeMapAndValues();
        }

        [Test()]
        public void IndexerTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map[0] = "zero";
            map[1] = "one";
            map[2] = "two";

            Assert.AreEqual("zero", map[0]);
            Assert.AreEqual("one", map[1]);
            Assert.AreEqual("two", map[2]);

            map[0] = "cero";
            map[1] = "uno";
            map[2] = "dos";

            Assert.AreEqual("cero", map[0]);
            Assert.AreEqual("uno", map[1]);
            Assert.AreEqual("dos", map[2]);

            map.DisposeMapAndValues();
            map.Dispose();
        }

        [Test()]
        unsafe public void CopyToTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);

            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");
            map.Add(3, "three");
            map.Add(4, "four");

            Span<KeyValuePair<int, NativeString>> span = stackalloc KeyValuePair<int, NativeString>[3];
            map.CopyTo(span, 0, 3);

            Assert.AreEqual(span[0].Key, 0);
            Assert.AreEqual(span[1].Key, 1);
            Assert.AreEqual(span[2].Key, 2);

            Assert.AreEqual(span[0].Value, "zero");
            Assert.AreEqual(span[1].Value, "one");
            Assert.AreEqual(span[2].Value, "two");

            var array = map.ToArray();
            Assert.AreEqual(5, array.Length);

            Assert.AreEqual(array[0].Key, 0);
            Assert.AreEqual(array[1].Key, 1);
            Assert.AreEqual(array[2].Key, 2);
            Assert.AreEqual(array[3].Key, 3);
            Assert.AreEqual(array[4].Key, 4);

            Assert.AreEqual(array[0].Value, "zero");
            Assert.AreEqual(array[1].Value, "one");
            Assert.AreEqual(array[2].Value, "two");
            Assert.AreEqual(array[3].Value, "three");
            Assert.AreEqual(array[4].Value, "four");

            map.DisposeMapAndValues();
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            var enumerator = map.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext())
            {
                i++;
            }

            Assert.AreEqual(3, i);

            map.DisposeMapAndValues();
        }

        [Test()]
        public void KeysTests()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            var keys = map.Keys;
            Assert.IsTrue(keys.Contains(0));
            Assert.IsTrue(keys.Contains(1));
            Assert.IsTrue(keys.Contains(2));

            Span<int> span = stackalloc int[3];
            keys.CopyTo(span, 0, 3);

            Assert.AreEqual(span[0], 0);
            Assert.AreEqual(span[1], 1);
            Assert.AreEqual(span[2], 2);

            map.DisposeMapAndValues();
        }

        [Test()]
        public void ValuesTests()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            var values = map.Values;
            Assert.IsTrue(values.Contains("zero"));
            Assert.IsTrue(values.Contains("one"));
            Assert.IsTrue(values.Contains("two"));

            Span<NativeString> span = stackalloc NativeString[3];
            values.CopyTo(span, 0, 3);

            Assert.AreEqual(span[0], "zero");
            Assert.AreEqual(span[1], "one");
            Assert.AreEqual(span[2], "two");

            map.DisposeMapAndValues();
        }

        [Test()]
        public void EnsureCapacityTests()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            Assert.AreEqual(4, map.Capacity);
            Assert.AreEqual(3, map.Length);

            // ContainsKey
            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));

            // ContainsValue
            Assert.IsTrue(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsTrue(map.ContainsValue("two"));

            map.EnsureCapacity(10);

            Assert.AreEqual(10, map.Capacity);
            Assert.AreEqual(3, map.Length);

            // ContainsKey
            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));

            // ContainsValue
            Assert.IsTrue(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsTrue(map.ContainsValue("two"));

            map.DisposeMapAndValues();
        }

        [Test()]
        public void TrimExcessTests()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(10);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            Assert.AreEqual(10, map.Capacity);
            Assert.AreEqual(3, map.Length);

            // ContainsKey
            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));

            // ContainsValue
            Assert.IsTrue(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsTrue(map.ContainsValue("two"));

            map.TrimExcess(5);

            Assert.AreEqual(5, map.Capacity);
            Assert.AreEqual(3, map.Length);

            // ContainsKey
            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));

            // ContainsValue
            Assert.IsTrue(map.ContainsValue("zero"));
            Assert.IsTrue(map.ContainsValue("one"));
            Assert.IsTrue(map.ContainsValue("two"));

            map.Add(3, "three");
            map.Add(4, "four");

            map.Remove(0);
            map.Remove(1);

            // ContainsKey
            Assert.IsFalse(map.ContainsKey(0));
            Assert.IsFalse(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(3));
            Assert.IsTrue(map.ContainsKey(4));

            map.DisposeMapAndValues();
        }

        [Test()]
        public void TrimExcessTests1()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(10);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            map.TrimExcess();

            Assert.AreEqual(3, map.Length);
            Assert.AreEqual(3, map.Capacity);

            map.DisposeMapAndValues();
        }

        [Test()]
        public void TrimExcessTests2()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(10);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            map.TrimExcess(6);

            Assert.AreEqual(3, map.Length);
            Assert.AreEqual(6, map.Capacity);

            map.DisposeMapAndValues();
        }

        [Test()]
        public void DisposeTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            Assert.IsTrue(map.IsValid);

            map.Dispose();
            Assert.IsFalse(map.IsValid);
            Assert.DoesNotThrow(() => map.Dispose());

            map.DisposeMapAndValues();
        }
    }
}