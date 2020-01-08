using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeSortedMapTests
    {
        [Test()]
        public void NativeSortedMapTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            Assert.IsTrue(map.IsEmpty);
            Assert.IsTrue(map.IsValid);
            Assert.AreEqual(0, map.Length);
            Assert.AreEqual(4, map.Capacity);
        }

        [Test()]
        public void NativeSortedMapTest1()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(stackalloc (int, char)[] { (1, 'a'), (2, 'b'), (3, 'c') });
            Assert.IsFalse(map.IsEmpty);
            Assert.IsTrue(map.IsValid);
            Assert.AreEqual(3, map.Length);
            Assert.AreEqual(3, map.Capacity);
        }

        [Test()]
        public void GetAllocatorTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);

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
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');
            map.Add(4, 'd');

            Assert.AreEqual(4, map.Length);
            Assert.AreEqual(4, map.Capacity);

            map.Add(5, 'e');

            Assert.AreEqual(5, map.Length);
            Assert.AreEqual(8, map.Capacity);
        }

        [Test()]
        public void TryAddTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            Assert.IsTrue(map.TryAdd(1, 'a'));
            Assert.IsTrue(map.TryAdd(2, 'b'));
            Assert.IsTrue(map.TryAdd(3, 'c'));

            Assert.IsFalse(map.TryAdd(1, 'A'));
            Assert.IsFalse(map.TryAdd(2, 'B'));

            Assert.AreEqual(3, map.Length);
            Assert.AreEqual(4, map.Capacity);
        }

        [Test()]
        public void AddOrUpdateTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.AddOrUpdate(1, 'a');
            map.AddOrUpdate(2, 'b');
            map.AddOrUpdate(3, 'c');

            map.AddOrUpdate(1, 'A');

            Assert.AreEqual(3, map.Length);
            Assert.AreEqual(4, map.Capacity);
        }

        [Test()]
        public void ReplaceTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.IsTrue(map.Replace(1, 'A'));
            Assert.IsTrue(map.Replace(2, 'B'));
            Assert.IsTrue(map.Replace(3, 'C'));

            Assert.IsFalse(map.Replace(4, 'd'));

            Assert.AreEqual(3, map.Length);
            Assert.AreEqual(4, map.Capacity);
        }

        [Test()]
        public void RemoveTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.IsTrue(map.Remove(1));
            Assert.AreEqual(2, map.Length);

            Assert.IsTrue(map.Remove(2));
            Assert.AreEqual(1, map.Length);

            Assert.IsTrue(map.Remove(3));
            Assert.AreEqual(0, map.Length);

            Assert.IsFalse(map.Remove(0));
            Assert.AreEqual(0, map.Length);
        }

        [Test()]
        public void RemoveAtTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            map.RemoveAt(2);
            Assert.AreEqual(2, map.Length);

            map.RemoveAt(1);
            Assert.AreEqual(1, map.Length);

            map.RemoveAt(0);
            Assert.AreEqual(0, map.Length);
        }

        [Test()]
        public void SubMapTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');
            map.Add(4, 'c');
            map.Add(5, 'c');
            map.Add(6, 'c');

            using NativeSortedMap<int, char> map2 = map.SubMap(2, 5);
            Assert.IsTrue(map2.ContainsKey(2));
            Assert.IsTrue(map2.ContainsKey(3));
            Assert.IsTrue(map2.ContainsKey(4));
            Assert.IsTrue(map2.ContainsKey(5));

            Assert.IsFalse(map2.ContainsKey(1));
            Assert.IsFalse(map2.ContainsKey(6));

            Assert.AreEqual(4, map2.Length);
            Assert.AreEqual(4, map2.Capacity);
        }

        [Test()]
        public void SubMapTest1()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');
            map.Add(4, 'c');
            map.Add(5, 'c');
            map.Add(6, 'c');

            using NativeSortedMap<int, char> map2 = map.SubMap(1, 6);
            Assert.IsTrue(map2.ContainsKey(1));
            Assert.IsTrue(map2.ContainsKey(2));
            Assert.IsTrue(map2.ContainsKey(3));
            Assert.IsTrue(map2.ContainsKey(4));
            Assert.IsTrue(map2.ContainsKey(5));
            Assert.IsTrue(map2.ContainsKey(6));

            Assert.AreEqual(6, map2.Length);
            Assert.AreEqual(6, map2.Capacity);
        }

        [Test()]
        public void SubMapTest2()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');
            map.Add(4, 'c');
            map.Add(5, 'c');
            map.Add(6, 'c');

            using NativeSortedMap<int, char> map2 = map.SubMap(2, 3);
            Assert.IsTrue(map2.ContainsKey(2));
            Assert.IsTrue(map2.ContainsKey(3));

            Assert.IsFalse(map2.ContainsKey(1));
            Assert.IsFalse(map2.ContainsKey(4));
            Assert.IsFalse(map2.ContainsKey(5));
            Assert.IsFalse(map2.ContainsKey(6));

            Assert.AreEqual(2, map2.Length);
            Assert.AreEqual(2, map2.Capacity);

            using NativeSortedMap<int, char> map3 = map.SubMap(1, 2);
            Assert.IsTrue(map3.ContainsKey(1));
            Assert.IsTrue(map3.ContainsKey(2));

            Assert.AreEqual(2, map3.Length);
            Assert.AreEqual(2, map3.Capacity);

            using NativeSortedMap<int, char> map4 = map.SubMap(5, 6);
            Assert.IsTrue(map4.ContainsKey(5));
            Assert.IsTrue(map4.ContainsKey(6));

            Assert.AreEqual(2, map4.Length);
            Assert.AreEqual(2, map4.Capacity);
        }

        [Test()]
        public void SubMapRangeTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');
            map.Add(4, 'c');
            map.Add(5, 'c');
            map.Add(6, 'c');

            using NativeSortedMap<int, char> map2 = map.SubMap(1..5);
            Assert.IsTrue(map2.ContainsKey(2));
            Assert.IsTrue(map2.ContainsKey(3));
            Assert.IsTrue(map2.ContainsKey(4));
            Assert.IsTrue(map2.ContainsKey(5));

            Assert.IsFalse(map2.ContainsKey(1));
            Assert.IsFalse(map2.ContainsKey(6));

            Assert.AreEqual(4, map2.Length);
            Assert.AreEqual(4, map2.Capacity);
        }

        [Test()]
        public void SubMapRangeTest1()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');
            map.Add(4, 'c');
            map.Add(5, 'c');
            map.Add(6, 'c');

            using NativeSortedMap<int, char> map2 = map.SubMap(0..6);
            Assert.IsTrue(map2.ContainsKey(1));
            Assert.IsTrue(map2.ContainsKey(2));
            Assert.IsTrue(map2.ContainsKey(3));
            Assert.IsTrue(map2.ContainsKey(4));
            Assert.IsTrue(map2.ContainsKey(5));
            Assert.IsTrue(map2.ContainsKey(6));

            Assert.AreEqual(6, map2.Length);
            Assert.AreEqual(6, map2.Capacity);

            using NativeSortedMap<int, char> map3 = map.SubMap(..);
            Assert.IsTrue(map3.ContainsKey(1));
            Assert.IsTrue(map3.ContainsKey(2));
            Assert.IsTrue(map3.ContainsKey(3));
            Assert.IsTrue(map3.ContainsKey(4));
            Assert.IsTrue(map3.ContainsKey(5));
            Assert.IsTrue(map2.ContainsKey(6));

            Assert.AreEqual(6, map3.Length);
            Assert.AreEqual(6, map3.Capacity);
        }

        [Test()]
        public void SubMapRangeTest2()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');
            map.Add(4, 'c');
            map.Add(5, 'c');
            map.Add(6, 'c');

            using NativeSortedMap<int, char> map2 = map.SubMap(1..3);
            Assert.IsTrue(map2.ContainsKey(2));
            Assert.IsTrue(map2.ContainsKey(3));

            Assert.IsFalse(map2.ContainsKey(1));
            Assert.IsFalse(map2.ContainsKey(4));
            Assert.IsFalse(map2.ContainsKey(5));
            Assert.IsFalse(map2.ContainsKey(6));

            Assert.AreEqual(2, map2.Length);
            Assert.AreEqual(2, map2.Capacity);

            using NativeSortedMap<int, char> map3 = map.SubMap(..2);
            Assert.IsTrue(map3.ContainsKey(1));
            Assert.IsTrue(map3.ContainsKey(2));

            Assert.AreEqual(2, map3.Length);
            Assert.AreEqual(2, map3.Capacity);

            using NativeSortedMap<int, char> map4 = map.SubMap(4..);
            Assert.IsTrue(map4.ContainsKey(5));
            Assert.IsTrue(map4.ContainsKey(6));

            Assert.AreEqual(2, map4.Length);
            Assert.AreEqual(2, map4.Capacity);
        }

        [Test()]
        public void ClearTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            map.Clear();
            Assert.AreEqual(0, map.Length);
            Assert.AreEqual(4, map.Capacity);
        }

        [Test()]
        public void TryGetValueTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.IsTrue(map.TryGetValue(1, out char v1));
            Assert.AreEqual('a', v1);

            Assert.IsTrue(map.TryGetValue(2, out char v2));
            Assert.AreEqual('b', v2);

            Assert.IsTrue(map.TryGetValue(3, out char v3));
            Assert.AreEqual('c', v3);

            Assert.IsFalse(map.TryGetValue(4, out char _));
        }

        [Test()]
        public void TryGetValueReferenceTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.IsTrue(map.TryGetValueReference(1, out var v1));
            Assert.AreEqual('a', v1.Value);

            Assert.IsTrue(map.TryGetValueReference(2, out var v2));
            Assert.AreEqual('b', v2.Value);

            Assert.IsTrue(map.TryGetValueReference(3, out var v3));
            Assert.AreEqual('c', v3.Value);

            Assert.IsFalse(map.TryGetValueReference(4, out var _));
        }

        [Test()]
        public void GetValueTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual('a', map.GetValue(1));
            Assert.AreEqual('b', map.GetValue(2));
            Assert.AreEqual('c', map.GetValue(3));

            Assert.Throws<KeyNotFoundException>(() => map.GetValue(4));
        }

        [Test()]
        public void GetValueOrDefaultTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual('a', map.GetValueOrDefault(1, 'A'));
            Assert.AreEqual('b', map.GetValueOrDefault(2, 'B'));
            Assert.AreEqual('c', map.GetValueOrDefault(3, 'C'));

            Assert.AreEqual('d', map.GetValueOrDefault(4, 'd'));
            Assert.AreEqual('e', map.GetValueOrDefault(5, 'e'));
        }

        [Test()]
        public void GetValueReferenceTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual('a', map.GetValueReference(1));
            Assert.AreEqual('b', map.GetValueReference(2));
            Assert.AreEqual('c', map.GetValueReference(3));

            Assert.Throws<KeyNotFoundException>(() => map.GetValueReference(4));
        }

        [Test()]
        public void LowerKeyTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual(1, map.LowerKey(2));
            Assert.AreEqual(2, map.LowerKey(3));
            Assert.AreEqual(3, map.LowerKey(4));
            Assert.AreEqual(3, map.LowerKey(5));

            Assert.IsNull(map.LowerKey(1));
        }

        [Test()]
        public void HigherKeyTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual(2, map.HigherKey(1));
            Assert.AreEqual(3, map.HigherKey(2));
            Assert.AreEqual(1, map.HigherKey(0));
            Assert.AreEqual(1, map.HigherKey(-1));

            Assert.IsNull(map.HigherKey(3));
        }

        [Test()]
        public void ContainsKeyTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.IsTrue(map.ContainsKey(1));
            Assert.IsTrue(map.ContainsKey(2));
            Assert.IsTrue(map.ContainsKey(3));

            Assert.IsFalse(map.ContainsKey(4));
            Assert.IsFalse(map.ContainsKey(5));
        }

        [Test()]
        public void ContainsValueTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.IsTrue(map.ContainsValue('a'));
            Assert.IsTrue(map.ContainsValue('b'));
            Assert.IsTrue(map.ContainsValue('c'));
        }

        [Test()]
        public void IndexOfKeyTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual(0, map.IndexOfKey(1));
            Assert.AreEqual(1, map.IndexOfKey(2));
            Assert.AreEqual(2, map.IndexOfKey(3));

            Assert.AreEqual(-1, map.IndexOfKey(0));
            Assert.AreEqual(-1, map.IndexOfKey(5));
        }

        [Test()]
        public void IndexOfValueTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual(0, map.IndexOfValue('a'));
            Assert.AreEqual(1, map.IndexOfValue('b'));
            Assert.AreEqual(2, map.IndexOfValue('c'));

            Assert.AreEqual(-1, map.IndexOfValue('d'));
            Assert.AreEqual(-1, map.IndexOfValue('e'));
        }

        [Test()]
        public void ElementAtTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual(KeyValuePair.Create(1, 'a'), map.ElementAt(0));
            Assert.AreEqual(KeyValuePair.Create(2, 'b'), map.ElementAt(1));
            Assert.AreEqual(KeyValuePair.Create(3, 'c'), map.ElementAt(2));
        }

        [Test()]
        public void IndexerTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual('a', map[1]);
            Assert.AreEqual('b', map[2]);
            Assert.AreEqual('c', map[3]);
        }

        [Test()]
        public void FirstKeyTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual(1, map.FirstKey);
        }

        [Test()]
        public void LastKeyTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual(3, map.LastKey);
        }

        [Test()]
        public void KeysTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            var keys = map.Keys;
            int[] array = new int[3];
            keys.CopyTo(array, 0, 3);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
        }

        [Test()]
        public void ValuesTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            var keys = map.Values ;
            char[] array = new char[3];
            keys.CopyTo(array, 0, 3);

            Assert.AreEqual('a', array[0]);
            Assert.AreEqual('b', array[1]);
            Assert.AreEqual('c', array[2]);
        }

        [Test()]
        public void TrimExcessTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(10);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            map.TrimExcess();
            Assert.AreEqual(3, map.Length);
            Assert.AreEqual(3, map.Capacity);
        }

        [Test()]
        public void TrimExcessTest1()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(10);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            map.TrimExcess(6);
            Assert.AreEqual(3, map.Length);
            Assert.AreEqual(6, map.Capacity);
        }

        [Test()]
        public void EnsureCapacityTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(10);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            map.EnsureCapacity(20);
            Assert.AreEqual(3, map.Length);
            Assert.AreEqual(20, map.Capacity);
        }

        [Test()]
        public void CopyToTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(5);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');
            map.Add(4, 'd');
            map.Add(5, 'e');

            Span<KeyValuePair<int, char>> span = stackalloc KeyValuePair<int, char>[5];
            map.CopyTo(span, 0, 3);

            Assert.AreEqual(KeyValuePair.Create(1, 'a'), span[0]);
            Assert.AreEqual(KeyValuePair.Create(2, 'b'), span[1]);
            Assert.AreEqual(KeyValuePair.Create(3, 'c'), span[2]);
        }

        [Test()]
        public void ToStringTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(5);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            Assert.AreEqual("[{1, a}, {2, b}, {3, c}]", map.ToString());
        }

        [Test()]
        public void ToArrayTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(5);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            KeyValuePair<int, char>[] array = map.ToArray();
            Assert.AreEqual(KeyValuePair.Create(1, 'a'), array[0]);
            Assert.AreEqual(KeyValuePair.Create(2, 'b'), array[1]);
            Assert.AreEqual(KeyValuePair.Create(3, 'c'), array[2]);
        }

        [Test()]
        public void ToNativeArrayTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(5);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            NativeArray<KeyValuePair<int, char>> array = map.ToNativeArray();
            Assert.AreEqual(KeyValuePair.Create(1, 'a'), array[0]);
            Assert.AreEqual(KeyValuePair.Create(2, 'b'), array[1]);
            Assert.AreEqual(KeyValuePair.Create(3, 'c'), array[2]);
        }

        [Test()]
        public void ToNativeArrayAndDisposeTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(5);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            using NativeArray<KeyValuePair<int, char>> array = map.ToNativeArrayAndDispose();
            Assert.AreEqual(KeyValuePair.Create(1, 'a'), array[0]);
            Assert.AreEqual(KeyValuePair.Create(2, 'b'), array[1]);
            Assert.AreEqual(KeyValuePair.Create(3, 'c'), array[2]);
        }

        [Test()]
        public void DisposeTest()
        {
            NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(5);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            map.Dispose();
            Assert.IsTrue(map.IsEmpty);
            Assert.IsFalse(map.IsValid);
            Assert.AreEqual(0, map.Length);
            Assert.AreEqual(0, map.Capacity);
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(stackalloc (int, char)[] { (1, 'a'), (2, 'b'), (3, 'c') });
            var enumerator = map.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(KeyValuePair.Create(1, 'a'), enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(KeyValuePair.Create(2, 'b'), enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(KeyValuePair.Create(3, 'c'), enumerator.Current);;

            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void ForEachTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            KeyValuePair<int, char>[] array = map.ToArray();
            foreach(var e in map)
            {
                CollectionAssert.Contains(array, e);
            }
        }

        [Test()]
        public void CloneTest()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(4);
            map.Add(1, 'a');
            map.Add(2, 'b');
            map.Add(3, 'c');

            using NativeSortedMap<int, char> clone = map.Clone();

            Assert.AreEqual(3, clone.Length);
            Assert.AreEqual(4, clone.Capacity);

            Assert.IsTrue(clone.ContainsKey(1));
            Assert.IsTrue(clone.ContainsKey(2));
            Assert.IsTrue(clone.ContainsKey(3));

            Assert.IsTrue(clone.ContainsValue('a'));
            Assert.IsTrue(clone.ContainsValue('b'));
            Assert.IsTrue(clone.ContainsValue('c'));

            clone.Add(4, 'd');
            Assert.AreNotEqual(map.Length, clone.Length);
            Assert.IsFalse(map.ContainsKey(4));

            Assert.IsTrue(clone.ContainsKey(1));
            Assert.IsTrue(clone.ContainsKey(2));
            Assert.IsTrue(clone.ContainsKey(3));
            Assert.IsTrue(clone.ContainsKey(4));

            Assert.IsTrue(clone.ContainsValue('a'));
            Assert.IsTrue(clone.ContainsValue('b'));
            Assert.IsTrue(clone.ContainsValue('c'));
            Assert.IsTrue(clone.ContainsValue('d'));
        }
    }
}