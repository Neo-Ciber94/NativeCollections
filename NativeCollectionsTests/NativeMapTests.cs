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
        static void DisposeNativeStrings(NativeMap<int, NativeString> map)
        {
            foreach (ref var e in map)
            {
                e.Value.Dispose();
            }
        }

        [Test()]
        public void NativeMapTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            Assert.AreEqual(4, map.Capacity);
            Assert.AreEqual(0, map.Length);
            Assert.IsTrue(map.IsValid);
            Assert.IsTrue(map.IsEmpty);

            map.Dispose();
            Assert.AreEqual(0, map.Capacity);
            Assert.AreEqual(0, map.Length);
            Assert.IsFalse(map.IsValid);
            Assert.IsTrue(map.IsEmpty);

            DisposeNativeStrings(map);
        }

        [Test()]
        public void NativeMapTest2()
        {
            using var map = new NativeMap<int, NativeString>(stackalloc (int, NativeString)[] { (0, "cero"), (1, "uno"), (2, "dos") });

            Assert.AreEqual(3, map.Capacity);
            Assert.AreEqual(3, map.Length);
            Assert.IsTrue(map.IsValid);
            Assert.IsFalse(map.IsEmpty);

            DisposeNativeStrings(map);
        }

        [Test()]
        public void AddTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
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

            DisposeNativeStrings(map);
        }

        [Test()]
        public void AddOrUpdateTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
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

            DisposeNativeStrings(map);
        }

        [Test()]
        public void ReplaceTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
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

            DisposeNativeStrings(map);
        }

        [Test()]
        public void RemoveTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
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

            DisposeNativeStrings(map);
        }

        [Test()]
        public void TryGetValueTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
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

            DisposeNativeStrings(map);
        }

        [Test()]
        public void TryGetValueReferenceTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);

            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            if (map.TryGetValueReference(1, out var value))
            {
                Assert.AreEqual("one", value.Value.ToString());
            }

            if (map.TryGetValueReference(4, out var otherValue))
            {
                Assert.IsFalse(otherValue.HasValue);
            }

            DisposeNativeStrings(map);
        }

        [Test()]
        public void ContainsKeyTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            // Looking for default values
            Assert.IsFalse(map.ContainsKey(0));

            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            Assert.IsTrue(map.ContainsKey(0));
            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));
            Assert.IsFalse(map.ContainsKey(3));

            DisposeNativeStrings(map);
        }

        [Test()]
        public void ContainsValueTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);

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

            DisposeNativeStrings(map);
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

            DisposeNativeStrings(map);
            map.Dispose();
        }

        [Test()]
        unsafe public void CopyToTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);

            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");
            map.Add(3, "three");
            map.Add(4, "four");

            Span<KeyValuePair<int, NativeString>> span = stackalloc KeyValuePair<int, NativeString>[3];
            map.CopyTo(span, 3);

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

            DisposeNativeStrings(map);
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
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

            DisposeNativeStrings(map);
        }

        [Test()]
        public void KeysTests()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            var keys = map.Keys;
            Assert.IsTrue(keys.Contains(0));
            Assert.IsTrue(keys.Contains(1));
            Assert.IsTrue(keys.Contains(2));

            Span<int> span = stackalloc int[3];
            keys.CopyTo(span);

            Assert.AreEqual(span[0], 0);
            Assert.AreEqual(span[1], 1);
            Assert.AreEqual(span[2], 2);

            DisposeNativeStrings(map);
        }

        [Test()]
        public void ValuesTests()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            map.Add(0, "zero");
            map.Add(1, "one");
            map.Add(2, "two");

            var values = map.Values;
            Assert.IsTrue(values.Contains("zero"));
            Assert.IsTrue(values.Contains("one"));
            Assert.IsTrue(values.Contains("two"));

            Span<NativeString> span = stackalloc NativeString[3];
            values.CopyTo(span);

            Assert.AreEqual(span[0], "zero");
            Assert.AreEqual(span[1], "one");
            Assert.AreEqual(span[2], "two");

            DisposeNativeStrings(map);
        }

        [Test()]
        public void EnsureCapacityTests()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
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

            DisposeNativeStrings(map);
        }

        [Test()]
        public void TrimExcessTests()
        {
            using NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(10);
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

            DisposeNativeStrings(map);
        }

        [Test()]
        public void DisposeTest()
        {
            NativeMap<int, NativeString> map = new NativeMap<int, NativeString>(4);
            Assert.IsTrue(map.IsValid);

            map.Dispose();
            Assert.IsFalse(map.IsValid);
            Assert.DoesNotThrow(() => map.Dispose());

            DisposeNativeStrings(map);
        }
    }
}