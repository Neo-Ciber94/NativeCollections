using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class MultiValueNativeMapTests
    {
        [Test()]
        public void MultiValueNativeMapTest()
        {
            using NativeMultiValueMap<char, int> map = new NativeMultiValueMap<char, int>(10);
            Assert.AreEqual(10, map.Slots);
            Assert.AreEqual(0, map.Length);
            Assert.IsTrue(map.IsValid);
            Assert.IsTrue(map.IsEmpty);
        }

        [Test()]
        public void GetAllocatorTest()
        {
            using NativeMultiValueMap<char, int> map = new NativeMultiValueMap<char, int>(10);

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
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(3);
            map.Add("Even", 2);
            map.Add("Even", 4);
            map.Add("Even", 6);
            map.Add("Odd", 1);
            map.Add("Odd", 3);

            Assert.AreEqual(5, map.Length);
            Assert.AreEqual(3, map.Slots);

            map.Add("Primes", 1);
            map.Add("Primes", 3);
            map.Add("Primes", 5);
            map.Add("TheAnswer", 42);

            Assert.AreEqual(9, map.Length);
            Assert.AreEqual(6, map.Slots);
            map.DisposeMapAndKeys();
        }

        [Test()]
        public void AddTest1()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(3);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            Assert.AreEqual(5, map.Length);
            Assert.AreEqual(3, map.Slots);

            map.Add("Primes", stackalloc int[] { 1, 3, 5});
            map.Add("TheAnswer", stackalloc int[] { 42 });
            Assert.AreEqual(9, map.Length);
            Assert.AreEqual(6, map.Slots);

            map.DisposeMapAndKeys();
        }

        [Test()]
        public void AddTest2()
        {
            using NativeMultiValueMap<char, int> map = new NativeMultiValueMap<char, int>(3);
            map.Add('e', 2);
            map.Add('e', 4);
            map.Add('e', 6);
            map.Add('o', 1);
            map.Add('o', 3);

            Assert.AreEqual(5, map.Length);
            Assert.AreEqual(3, map.Slots);

            map.Add('p', 1);
            map.Add('p', 3);
            map.Add('p', 5);
            map.Add('t', 42);

            Assert.AreEqual(9, map.Length);
            Assert.AreEqual(6, map.Slots);
        }

        [Test()]
        public void ReplaceValuesTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(3);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            Assert.IsTrue(map.ReplaceValues("Even", stackalloc int[] { 6, 4, 2 }));
            Assert.IsFalse(map.ReplaceValues("Primes", stackalloc int[] { 1, 2, 3, 5, 7 }));

            var values = map["Even"];
            CollectionAssert.AreEqual(new int[] { 6, 4, 2 }, values.ToArray());

            map.DisposeMapAndKeys();
        }

        [Test()]
        public void RemoveTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            Assert.IsTrue(map.Remove("Even"));
            Assert.IsFalse(map.Remove("Primes"));

            Assert.AreEqual(2, map.Length);
            map.DisposeMapAndKeys();
        }

        [Test()]
        public void RemoveValueTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            Assert.IsTrue(map.RemoveValue("Even", 2));
            Assert.IsFalse(map.RemoveValue("Even", 2));
            Assert.IsFalse(map.RemoveValue("Even", 5));
            Assert.IsFalse(map.RemoveValue("Primes", 7));
            Assert.AreEqual(4, map.Length);

            map.DisposeMapAndKeys();
        }

        [Test()]
        public void ClearTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            map.Clear();

            Assert.IsTrue(map.IsValid);
            Assert.IsTrue(map.IsEmpty);
            Assert.AreEqual(0, map.Length);
            Assert.AreEqual(10, map.Slots);

            map.DisposeMapAndKeys();
        }

        [Test()]
        public void GetValuesTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            CollectionAssert.AreEqual(new int[] { 2, 4, 6 }, map.GetValues("Even").ToArray());
            CollectionAssert.AreEqual(new int[] { 1, 3 }, map.GetValues("Odd").ToArray());

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var values = map.GetValues("Primes");
            });
            map.DisposeMapAndKeys();
        }

        [Test()]
        public void TryGetValuesTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10); 
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            Assert.IsTrue(map.TryGetValues("Even", out var v1));
            CollectionAssert.AreEqual(new int[] { 2, 4, 6 }, v1.ToArray());

            Assert.IsTrue(map.TryGetValues("Odd", out var v2));
            CollectionAssert.AreEqual(new int[] { 1, 3 }, v2.ToArray());

            Assert.IsFalse(map.TryGetValues("Primes", out _));

            map.DisposeMapAndKeys();
        }

        [Test()]
        public void ContainsKeyTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            Assert.IsTrue(map.ContainsKey("Even"));
            Assert.IsTrue(map.Remove("Even"));
            Assert.IsFalse(map.ContainsKey("Even"));

            Assert.IsTrue(map.ContainsKey("Odd"));
            Assert.IsTrue(map.Remove("Odd"));
            Assert.IsFalse(map.ContainsKey("Odd"));

            map.DisposeMapAndKeys();
        }

        [Test()]
        public void ContainsValueTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            Assert.IsTrue(map.ContainsValue(2));
            Assert.IsTrue(map.ContainsValue(4));
            Assert.IsTrue(map.ContainsValue(6));
            Assert.IsTrue(map.ContainsValue(1));
            Assert.IsTrue(map.ContainsValue(3));
            Assert.IsFalse(map.ContainsValue(0));

            map.DisposeMapAndKeys();
        }

        [Test()]
        public void ContainsValueTest1()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            Assert.IsTrue(map.ContainsValue("Even", 2));
            Assert.IsTrue(map.ContainsValue("Even", 4));
            Assert.IsTrue(map.ContainsValue("Even", 6));
            Assert.IsTrue(map.ContainsValue("Odd", 1));
            Assert.IsTrue(map.ContainsValue("Odd", 3));

            Assert.IsFalse(map.ContainsValue("Even", 9));
            Assert.IsFalse(map.ContainsValue("Odd", 4));

            map.DisposeMapAndKeys();
        }

        [Test()]
        public void CopyToTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            Span<KeyValuePair<NativeString, int>> span = stackalloc KeyValuePair<NativeString, int>[5];
            map.CopyTo(span, 0, 5);

            Assert.AreEqual(KeyValuePair.Create("Even", 2), span[0]);
            Assert.AreEqual(KeyValuePair.Create("Even", 4), span[1]);
            Assert.AreEqual(KeyValuePair.Create("Even", 6), span[2]);
            Assert.AreEqual(KeyValuePair.Create("Odd", 1), span[3]);
            Assert.AreEqual(KeyValuePair.Create("Odd", 3), span[4]);

            map.DisposeMapAndKeys();
        }

        [Test()]
        public void ToStringTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            Assert.AreEqual("[{Even, [2, 4, 6]}, {Odd, [1, 3]}]", map.ToString());

            map.DisposeMapAndKeys();
        }

        [Test()]
        public void CloneTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            NativeMultiValueMap<NativeString, int> clone = map.Clone();
            clone.Add("Even", 8);
            clone.Add("Odd", 5);

            Assert.AreEqual(7, clone.Length);
            Assert.AreEqual(10, clone.Slots);

            clone.Remove("Odd");
            Assert.AreEqual(4, clone.Length);

            Assert.IsTrue(map.ContainsKey("Odd"));
            Assert.IsTrue(map.ContainsValue(1));
            Assert.IsTrue(map.ContainsValue(3));
            Assert.IsFalse(map.ContainsValue(5));

            map.DisposeMapAndKeys();
            clone.DisposeMapAndKeys();
        }

        [Test()]
        public void DisposeTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            // Internally call map.Dispose();
            map.DisposeMapAndKeys();

            Assert.AreEqual(0, map.Slots);
            Assert.AreEqual(0, map.Length);
            Assert.IsFalse(map.IsValid);
            Assert.IsTrue(map.IsEmpty);
        }

        [Test()]
        public void IndexerTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(10);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            var values1 = map["Even"];
            Assert.AreEqual(new int[] { 2, 4, 6 }, values1.ToArray());

            var values2 = map["Odd"];
            Assert.AreEqual(new int[] { 1, 3 }, values2.ToArray());
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            NativeMultiValueMap<NativeString, int> map = new NativeMultiValueMap<NativeString, int>(4);
            map.Add("Even", stackalloc int[] { 2, 4, 6 });
            map.Add("Odd", stackalloc int[] { 1, 3 });

            NativeString key;
            NativeSlice<int> values;

            var enumerator = map.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            key = enumerator.Current.Key;
            values = enumerator.Current.Values;

            Assert.AreEqual("Even", key);
            CollectionAssert.AreEqual(new int[] { 2, 4, 6 }, values.ToArray());

            Assert.IsTrue(enumerator.MoveNext());

            key = enumerator.Current.Key;
            values = enumerator.Current.Values;

            Assert.AreEqual("Odd", key);
            CollectionAssert.AreEqual(new int[] { 1, 3}, values.ToArray());

            Assert.IsFalse(enumerator.MoveNext());
            map.DisposeMapAndKeys();
        }
    }
}