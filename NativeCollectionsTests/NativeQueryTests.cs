using NUnit.Framework;
using NativeCollections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using NativeCollections.Allocators;

namespace NativeCollections.Tests
{
    [TestFixture()]
    public class NativeQueryTests
    {
        [Test()]
        public void NativeArrayAsQuery()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery();

            var enumerator = query.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(3, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(5, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(4, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void NativeListAsQuery()
        {
            using NativeList<int> list = new NativeList<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = list.AsQuery();

            var enumerator = query.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(3, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(5, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(4, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void NativeSetAsQuery()
        {
            using NativeSet<int> set = new NativeSet<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = set.AsQuery();

            var enumerator = query.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(3, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(5, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(4, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void NativeStackAsQuery()
        {
            using NativeStack<int> stack = new NativeStack<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = stack.AsQuery();

            var enumerator = query.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(3, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(5, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(4, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void NativeQueueAsQuery()
        {
            using NativeQueue<int> queue = new NativeQueue<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = queue.AsQuery();

            var enumerator = query.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(3, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(5, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(4, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void NativeDequeAsQuery()
        {
            using NativeDeque<int> deque = new NativeDeque<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = deque.AsQuery();

            var enumerator = query.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(3, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(5, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(4, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        unsafe public void NativeSliceAsQuery()
        {
            int* p = stackalloc int[] { 1, 3, 5, 2, 4 };
            NativeSlice<int> slice = new NativeSlice<int>(p, 5);
            using NativeQuery<int> query = slice.AsQuery();

            var enumerator = query.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(3, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(5, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(4, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void NativeMapAsQuery()
        {
            using NativeMap<int, char> map = new NativeMap<int, char>(stackalloc (int, char)[]
            {
                    (1, 'a'), (2, 'b'), (3, 'c'), (4, 'd'), (5, 'e')
            });
            using NativeQuery<KeyValuePair<int, char>> query = map.AsQuery();

            var enumerator = query.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(KeyValuePair.Create(1, 'a'), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(KeyValuePair.Create(2, 'b'), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(KeyValuePair.Create(3, 'c'), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(KeyValuePair.Create(4, 'd'), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(KeyValuePair.Create(5, 'e'), enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void NativeSortedMapAsQuery()
        {
            using NativeSortedMap<int, char> map = new NativeSortedMap<int, char>(stackalloc (int, char)[]
            {
                (1, 'a'),
                (2, 'b'),
                (3, 'c'),
                (4, 'd'),
                (5, 'e')
            });
            using NativeQuery<KeyValuePair<int, char>> query = map.AsQuery();

            var enumerator = query.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(KeyValuePair.Create(1, 'a'), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(KeyValuePair.Create(2, 'b'), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(KeyValuePair.Create(3, 'c'), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(KeyValuePair.Create(4, 'd'), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(KeyValuePair.Create(5, 'e'), enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void NativeSortedSetAsQuery()
        {
            using NativeSortedSet<int> set = new NativeSortedSet<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = set.AsQuery();

            var enumerator = query.GetEnumerator();
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
        public void NativeStringAsQuery()
        {
            using NativeString str = "Hello";
            using NativeQuery<char> query = str.AsQuery();

            Assert.AreEqual(5, query.Length);
            Assert.AreEqual('H', query[0]);
            Assert.AreEqual('e', query[1]);
            Assert.AreEqual('l', query[2]);
            Assert.AreEqual('l', query[3]);
            Assert.AreEqual('o', query[4]);
        }

        // Transform

        [Test()]
        public void SelectTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<double> query = array.AsQuery().Select(e => e * 2.0);

            Assert.AreEqual(5, query.Length);

            Assert.AreEqual(2.0, query[0]);
            Assert.AreEqual(6.0, query[1]);
            Assert.AreEqual(10.0, query[2]);
            Assert.AreEqual(4.0, query[3]);
            Assert.AreEqual(8.0, query[4]);
        }

        [Test()]
        public void WhereTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().Where(e => e % 2 == 0);

            Assert.AreEqual(2, query.Length);

            Assert.AreEqual(2, query[0]);
            Assert.AreEqual(4, query[1]);
        }

        [Test()]
        public void WhereTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().Where(e => e > 5);

            Assert.AreEqual(0, query.Length);
            Assert.IsTrue(query.IsEmpty);
        }

        [Test()]
        public void TakeTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().Take(3);

            Assert.AreEqual(3, query.Length);

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(3, query[1]);
            Assert.AreEqual(5, query[2]);
        }

        [Test()]
        public void TakeTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().Take(0);

            Assert.AreEqual(0, query.Length);
            Assert.IsTrue(query.IsEmpty);
        }

        [Test()]
        public void SkipTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().Skip(3);

            Assert.AreEqual(2, query.Length);

            Assert.AreEqual(2, query[0]);
            Assert.AreEqual(4, query[1]);
        }

        [Test()]
        public void SkipTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().Skip(5);

            Assert.AreEqual(0, query.Length);
            Assert.IsTrue(query.IsEmpty);
        }

        [Test()]
        public void TakeWhileTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().TakeWhile(e => e < 5);

            Assert.AreEqual(2, query.Length);

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(3, query[1]);
        }

        [Test()]
        public void TakeWhileTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().TakeWhile(e => e > 10);

            Assert.AreEqual(0, query.Length);
            Assert.IsTrue(query.IsEmpty);
        }

        [Test()]
        public void SkipWhileTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().SkipWhile(e => e < 5);

            Assert.AreEqual(3, query.Length);

            Assert.AreEqual(5, query[0]);
            Assert.AreEqual(2, query[1]);
            Assert.AreEqual(4, query[2]);
        }

        [Test()]
        public void SkipWhileTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().SkipWhile(e => e > 0);

            Assert.AreEqual(0, query.Length);
            Assert.IsTrue(query.IsEmpty);
        }

        [Test()]
        public void TakeLastTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().TakeLast(3);

            Assert.AreEqual(3, query.Length);

            Assert.AreEqual(5, query[0]);
            Assert.AreEqual(2, query[1]);
            Assert.AreEqual(4, query[2]);
        }

        [Test()]
        public void SkipLastTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().SkipLast(2);

            Assert.AreEqual(3, query.Length);

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(3, query[1]);
            Assert.AreEqual(5, query[2]);
        }

        [Test()]
        public void ReverseTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().Reverse();

            Assert.AreEqual(5, query.Length);

            Assert.AreEqual(4, query[0]);
            Assert.AreEqual(2, query[1]);
            Assert.AreEqual(5, query[2]);
            Assert.AreEqual(3, query[3]);
            Assert.AreEqual(1, query[4]);
        }

        [Test()]
        public void CastTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5 });
            using NativeQuery<byte> query = array.AsQuery().Cast<byte>();

            Assert.AreEqual(12, query.Length);

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(0, query[1]);
            Assert.AreEqual(0, query[2]);
            Assert.AreEqual(0, query[3]);
            Assert.AreEqual(3, query[4]);
            Assert.AreEqual(0, query[5]);
            Assert.AreEqual(0, query[6]);
            Assert.AreEqual(0, query[7]);
            Assert.AreEqual(5, query[8]);
            Assert.AreEqual(0, query[9]);
            Assert.AreEqual(0, query[10]);
            Assert.AreEqual(0, query[11]);
        }

        [Test()]
        public void CastTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5 });
            using NativeQuery<short> query = array.AsQuery().Cast<short>();

            Assert.AreEqual(6, query.Length);

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(0, query[1]);
            Assert.AreEqual(3, query[2]);
            Assert.AreEqual(0, query[3]);
            Assert.AreEqual(5, query[4]);
            Assert.AreEqual(0, query[5]);
        }

        [Test()]
        public void CastTest2()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5 });
            using NativeQuery<float> query = array.AsQuery().Cast<float>();

            Assert.AreEqual(3, query.Length);

            Assert.AreEqual(Cast<int, float>(1), query[0]);
            Assert.AreEqual(Cast<int, float>(3), query[1]);
            Assert.AreEqual(Cast<int, float>(5), query[2]);
        }

        [Test()]
        public void CastTest3()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5 });

            Assert.Throws<InvalidCastException>(() =>
            {
                using NativeQuery<long> query = array.AsQuery().Cast<long>();
            });
        }

        [Test()]
        public void SortedTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().Sorted();

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(2, query[1]);
            Assert.AreEqual(3, query[2]);
            Assert.AreEqual(4, query[3]);
            Assert.AreEqual(5, query[4]);
        }

        [Test()]
        public void SortedTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().Sorted((x,y) => y.CompareTo(x));

            Assert.AreEqual(5, query[0]);
            Assert.AreEqual(4, query[1]);
            Assert.AreEqual(3, query[2]);
            Assert.AreEqual(2, query[3]);
            Assert.AreEqual(1, query[4]);
        }

        [Test()]
        public void SortedTest2()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().Sorted(Comparer<int>.Default);

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(2, query[1]);
            Assert.AreEqual(3, query[2]);
            Assert.AreEqual(4, query[3]);
            Assert.AreEqual(5, query[4]);
        }

        [Test()]
        public void OrderByTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().OrderBy(e => e);

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(2, query[1]);
            Assert.AreEqual(3, query[2]);
            Assert.AreEqual(4, query[3]);
            Assert.AreEqual(5, query[4]);
        }

        [Test()]
        public void OrderByDecendingTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<int> query = array.AsQuery().OrderByDecending(e => e);

            Assert.AreEqual(5, query[0]);
            Assert.AreEqual(4, query[1]);
            Assert.AreEqual(3, query[2]);
            Assert.AreEqual(2, query[3]);
            Assert.AreEqual(1, query[4]);
        }

        [Test()]
        public void SeekTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });

            int[] nums = new int[] { 1, 2, 3, 4, 5 };
            using NativeQuery<int> query = array.AsQuery().Seek(e =>
            {
                CollectionAssert.Contains(nums, e);
            });
        }

        [Test()]
        public void WithIndexTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeQuery<IndexedValue<int>> query = array.AsQuery().WithIndex();

            Assert.AreEqual(IndexedValue.Create(1, 0), query[0]);
            Assert.AreEqual(IndexedValue.Create(3, 1), query[1]);
            Assert.AreEqual(IndexedValue.Create(5, 2), query[2]);
            Assert.AreEqual(IndexedValue.Create(2, 3), query[3]);
            Assert.AreEqual(IndexedValue.Create(4, 4), query[4]);
        }

        [Test()]
        public void DistinctTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5 });
            using NativeQuery<int> query = array.AsQuery().Distinct();

            Assert.AreEqual(5, query.Length);

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(2, query[1]);
            Assert.AreEqual(3, query[2]);
            Assert.AreEqual(4, query[3]);
            Assert.AreEqual(5, query[4]);
        }

        // Reduce

        [Test()]
        public void AllTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            Assert.IsTrue(array.AsQuery().All(e => e > 0));
            Assert.IsFalse(array.AsQuery().All(e => e < 0));
        }

        [Test()]
        public void NoneTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            Assert.IsTrue(array.AsQuery().None(e => e < 0));
            Assert.IsFalse(array.AsQuery().None(e => e > 0));
        }

        [Test()]
        public void AnyTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            Assert.IsTrue(array.AsQuery().Any(e => e == 3));
            Assert.IsFalse(array.AsQuery().Any(e => e == 6));
        }

        [Test()]
        public void ContainsTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            Assert.IsTrue(array.AsQuery().Contains(1));
            Assert.IsFalse(array.AsQuery().Contains(6));
        }

        [Test()]
        public void CountTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            Assert.AreEqual(2, array.AsQuery().Count(e => e > 3));
            Assert.AreEqual(0, array.AsQuery().Count(e => e > 5));
        }

        [Test()]
        public void FirstTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(1, array.AsQuery().First());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().First());
        }

        [Test()]
        public void FirstTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(3, array.AsQuery().First(e => e > 2));
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().First(e => e > 2));
        }

        [Test()]
        public void FirstOrDefaultTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(1, array.AsQuery().FirstOrDefault());
            Assert.AreEqual(default(int), emptyArray.AsQuery().FirstOrDefault());
        }

        [Test()]
        public void FirstOrDefaultTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(3, array.AsQuery().FirstOrDefault(e => e > 2));
            Assert.AreEqual(default(int), emptyArray.AsQuery().FirstOrDefault(e => e > 2));
        }

        [Test()]
        public void LastTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(4, array.AsQuery().Last());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Last());
        }

        [Test()]
        public void LastTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(4, array.AsQuery().Last(e => e > 2));
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Last(e => e > 2));
        }

        [Test()]
        public void LastOrDefaultTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(4, array.AsQuery().LastOrDefault());
            Assert.AreEqual(default(int), emptyArray.AsQuery().LastOrDefault());
        }

        [Test()]
        public void LastOrDefaultTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 3, 5, 2, 4 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(4, array.AsQuery().LastOrDefault(e => e > 2));
            Assert.AreEqual(default(int), emptyArray.AsQuery().LastOrDefault(e => e > 2));
        }

        [Test()]
        public void ReduceTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(6, array.AsQuery().Reduce((total, cur) => total + cur));
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Reduce((total, cur) => total + cur));
        }

        [Test()]
        public void ReduceTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(8, array.AsQuery().Reduce(2, (total, cur) => total + cur));
            Assert.AreEqual(2, emptyArray.AsQuery().Reduce(2, (total, cur) => total + cur));
        }

        [Test()]
        public void SequenceEqualsTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });

            Assert.IsTrue(array.AsQuery().SequenceEquals(stackalloc int[] { 1, 2, 3 }));
            Assert.IsFalse(array.AsQuery().SequenceEquals(stackalloc int[] { 3, 2, 1}));
            Assert.IsFalse(array.AsQuery().SequenceEquals(stackalloc int[] { 1, 2, 4 }));
            Assert.IsFalse(array.AsQuery().SequenceEquals(stackalloc int[] { 1, 2, 3, 4 }));
        }

        // Extensions

        [Test()]
        public void MinTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 5, 4, 1, 2, 3 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(1, array.AsQuery().Min());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Min());
        }

        [Test()]
        public void MaxTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 5, 4, 1, 2, 3 });
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(5, array.AsQuery().Max());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Max());
        }

        [Test()]
        public void SumTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3});
            using NativeArray<int> emptyArray = default;

            Assert.AreEqual(6, array.AsQuery().Sum());
            Assert.AreEqual(0, emptyArray.AsQuery().Sum());
        }

        [Test()]
        public void SumTest1()
        {
            using NativeArray<long> array = new NativeArray<long>(stackalloc long[] { 1, 2, 3 });
            using NativeArray<long> emptyArray = default;

            Assert.AreEqual(6L, array.AsQuery().Sum());
            Assert.AreEqual(0L, emptyArray.AsQuery().Sum());
        }

        [Test()]
        public void SumTest2()
        {
            using NativeArray<uint> array = new NativeArray<uint>(stackalloc uint[] { 1, 2, 3 });
            using NativeArray<uint> emptyArray = default;

            Assert.AreEqual(6u, array.AsQuery().Sum());
            Assert.AreEqual(0u, emptyArray.AsQuery().Sum());
        }

        [Test()]
        public void SumTest3()
        {
            using NativeArray<ulong> array = new NativeArray<ulong>(stackalloc ulong[] { 1, 2, 3 });
            using NativeArray<ulong> emptyArray = default;

            Assert.AreEqual(6uL, array.AsQuery().Sum());
            Assert.AreEqual(0uL, emptyArray.AsQuery().Sum());
        }

        [Test()]
        public void SumTest4()
        {
            using NativeArray<float> array = new NativeArray<float>(stackalloc float[] { 1, 2, 3 });
            using NativeArray<float> emptyArray = default;

            Assert.AreEqual(6f, array.AsQuery().Sum());
            Assert.AreEqual(0f, emptyArray.AsQuery().Sum());
        }

        [Test()]
        public void SumTest5()
        {
            using NativeArray<double> array = new NativeArray<double>(stackalloc double[] { 1, 2, 3 });
            using NativeArray<double> emptyArray = default;

            Assert.AreEqual(6.0, array.AsQuery().Sum());
            Assert.AreEqual(0.0, emptyArray.AsQuery().Sum());
        }

        [Test()]
        public void SumTest6()
        {
            using NativeArray<decimal> array = new NativeArray<decimal>(stackalloc decimal[] { 1, 2, 3 });
            using NativeArray<decimal> emptyArray = default;

            Assert.AreEqual(6m, array.AsQuery().Sum());
            Assert.AreEqual(0m, emptyArray.AsQuery().Sum());
        }
        
        [Test()]
        public void AverageTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeArray<decimal> emptyArray = default;
            
            Assert.AreEqual(2, array.AsQuery().Average());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Average());
        }

        [Test()]
        public void AverageTest1()
        {
            using NativeArray<long> array = new NativeArray<long>(stackalloc long[] { 1, 2, 3 });
            using NativeArray<long> emptyArray = default;

            Assert.AreEqual(2L, array.AsQuery().Average());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Average());
        }

        [Test()]
        public void AverageTest2()
        {
            using NativeArray<uint> array = new NativeArray<uint>(stackalloc uint[] { 1, 2, 3 });
            using NativeArray<uint> emptyArray = default;

            Assert.AreEqual(2u, array.AsQuery().Average());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Average());
        }

        [Test()]
        public void AverageTest3()
        {
            using NativeArray<ulong> array = new NativeArray<ulong>(stackalloc ulong[] { 1, 2, 3 });
            using NativeArray<ulong> emptyArray = default;

            Assert.AreEqual(2uL, array.AsQuery().Average());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Average());
        }

        [Test()]
        public void AverageTest4()
        {
            using NativeArray<float> array = new NativeArray<float>(stackalloc float[] { 1, 2, 3 });
            using NativeArray<float> emptyArray = default;

            Assert.AreEqual(2f, array.AsQuery().Average());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Average());
        }

        [Test()]
        public void AverageTest5()
        {
            using NativeArray<double> array = new NativeArray<double>(stackalloc double[] { 1, 2, 3 });
            using NativeArray<double> emptyArray = default;

            Assert.AreEqual(2.0, array.AsQuery().Average());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Average());
        }

        [Test()]
        public void AverageTest6()
        {
            using NativeArray<decimal> array = new NativeArray<decimal>(stackalloc decimal[] { 1, 2, 3 });
            using NativeArray<decimal> emptyArray = default;

            Assert.AreEqual(2m, array.AsQuery().Average());
            Assert.Throws<InvalidOperationException>(() => emptyArray.AsQuery().Average());
        }

        [Test()]
        public void DefaultIfEmptyTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            NativeQuery<int> query = array.AsQuery().DefaultIfEmpty(13);

            Assert.AreEqual(new int[] { 1, 2, 3 }, ToArrayAndDispose(ref query));

            NativeQuery<int> emptyQuery = new NativeQuery<int>().DefaultIfEmpty(13);
            Assert.AreEqual(new int[] { 13 }, ToArrayAndDispose(ref emptyQuery));
        }

        [Test()]
        public void DefaultIfEmptyTest1()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            NativeQuery<int> query = array.AsQuery().DefaultIfEmpty(stackalloc int[] { 11, 12, 13 });

            Assert.AreEqual(new int[] { 1, 2, 3 }, ToArrayAndDispose(ref query));

            NativeQuery<int> emptyQuery = new NativeQuery<int>().DefaultIfEmpty(stackalloc int[] { 11, 12, 13 });
            Assert.AreEqual(new int[] { 11, 12, 13 }, ToArrayAndDispose(ref emptyQuery));
        }

        [Test()]
        public void ZipTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeQuery<int> query = array.AsQuery().Zip(stackalloc int[] { 3, 2, 1 }, (a, b) => a + b);

            Assert.AreEqual(4, query[0]);
            Assert.AreEqual(4, query[1]);
            Assert.AreEqual(4, query[2]);
        }

        [Test()]
        public void ConcatTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeQuery<int> query = array.AsQuery().Concat(stackalloc int[] { 4, 5, 6 });

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(2, query[1]);
            Assert.AreEqual(3, query[2]);
            Assert.AreEqual(4, query[3]);
            Assert.AreEqual(5, query[4]);
            Assert.AreEqual(6, query[5]);
        }

        [Test()]
        public void AppendTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeQuery<int> query = array.AsQuery().Append(4);

            Assert.AreEqual(1, query[0]);
            Assert.AreEqual(2, query[1]);
            Assert.AreEqual(3, query[2]);
            Assert.AreEqual(4, query[3]);
        }

        [Test()]
        public void PrependTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeQuery<int> query = array.AsQuery().Prepend(0);

            Assert.AreEqual(0, query[0]);
            Assert.AreEqual(1, query[1]);
            Assert.AreEqual(2, query[2]);
            Assert.AreEqual(3, query[3]);
        }

        // ToNativeContainer

        [Test]
        public void ToNativeArrayTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeArray<int> result = array.AsQuery().Select(e => e * 2).ToNativeArray();

            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(4, result[1]);
            Assert.AreEqual(6, result[2]);
        }

        [Test]
        public void ToNativeListTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeList<int> result = array.AsQuery().Select(e => e * 2).ToNativeList();

            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(4, result[1]);
            Assert.AreEqual(6, result[2]);
        }

        [Test]
        public void ToNativeSetTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 1, 2, 3, 3 });
            using NativeSet<int> result = array.AsQuery().Select(e => e * 2).ToNativeSet();

            Assert.AreEqual(3, result.Length);
            Assert.IsTrue(result.Contains(2));
            Assert.IsTrue(result.Contains(4));
            Assert.IsTrue(result.Contains(6));
        }

        [Test]
        public void ToNativeStackTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeStack<int> result = array.AsQuery().Select(e => e * 2).ToNativeStack();

            Assert.AreEqual(3, result.Length);
            Assert.IsTrue(result.Contains(2));
            Assert.IsTrue(result.Contains(4));
            Assert.IsTrue(result.Contains(6));
        }

        [Test]
        public void ToNativeQueueTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeQueue<int> result = array.AsQuery().Select(e => e * 2).ToNativeQueue();

            Assert.AreEqual(3, result.Length);
            Assert.IsTrue(result.Contains(2));
            Assert.IsTrue(result.Contains(4));
            Assert.IsTrue(result.Contains(6));
        }

        [Test]
        public void ToNativeDequeTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 1, 2, 3 });
            using NativeDeque<int> result = array.AsQuery().Select(e => e * 2).ToNativeDeque();

            Assert.AreEqual(3, result.Length);
            Assert.IsTrue(result.Contains(2));
            Assert.IsTrue(result.Contains(4));
            Assert.IsTrue(result.Contains(6));
        }

        [Test]
        public void ToNativeMapTest()
        {
            using NativeArray<char> array = new NativeArray<char>(stackalloc char[] { 'a','b','c' });
            using NativeMap<int, char> result = array.AsQuery().ToNativeMap(e => e - 96);

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual('a', result[1]);
            Assert.AreEqual('b', result[2]);
            Assert.AreEqual('c', result[3]);
        }

        [Test]
        public void ToNativeMapTest1()
        {
            using NativeArray<char> array = new NativeArray<char>(stackalloc char[] { 'a', 'b', 'c' });
            using NativeMap<int, char> result = array.AsQuery().ToNativeMap(e => e - 96, e => (char)(e + 1));

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual('b', result[1]);
            Assert.AreEqual('c', result[2]);
            Assert.AreEqual('d', result[3]);
        }

        [Test]
        public void ToNativeSortedMapTest()
        {
            using NativeArray<char> array = new NativeArray<char>(stackalloc char[] { 'b', 'a', 'c' });
            using NativeSortedMap<int, char> result = array.AsQuery().ToNativeSortedMap(e => e - 96);

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(1, result.FirstKey);
            Assert.AreEqual(3, result.LastKey);

            Assert.AreEqual('a', result[1]);
            Assert.AreEqual('b', result[2]);
            Assert.AreEqual('c', result[3]);
        }

        [Test]
        public void ToNativeSortedMapTest1()
        {
            using NativeArray<char> array = new NativeArray<char>(stackalloc char[] { 'a', 'b', 'c' });
            using NativeSortedMap<int, char> result = array.AsQuery().ToNativeSortedMap(e => e - 96, e => (char)(e + 1));

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(1, result.FirstKey);
            Assert.AreEqual(3, result.LastKey);

            Assert.AreEqual('b', result[1]);
            Assert.AreEqual('c', result[2]);
            Assert.AreEqual('d', result[3]);
        }

        [Test]
        public void ToNativeSortedSetTest()
        {
            using NativeArray<int> array = new NativeArray<int>(stackalloc int[] { 2, 2, 1, 1, 4, 3 });
            using NativeSortedSet<int> result = array.AsQuery().Select(e => e * 2).ToNativeSortedSet();

            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(4, result[1]);
            Assert.AreEqual(6, result[2]);
            Assert.AreEqual(8, result[3]);
        }

        //TearDown

        [TearDown]
        public void CheckMemory()
        {
            DebugAllocator allocator = Allocator.Default as DebugAllocator;
            if(allocator != null)
            {
                Assert.IsTrue(allocator.BytesAllocated == 0, $"Memory Leak: {allocator.BytesAllocated}");
            }
        }

        // Utils

        unsafe private static TTo Cast<TFrom, TTo>(TFrom value) where TFrom: unmanaged where TTo: unmanaged
        {
            Debug.Assert(sizeof(TTo) <= sizeof(TFrom));
            return *(TTo*)&value;
        }

        private static T[] ToArrayAndDispose<T>(ref NativeQuery<T> query) where T: unmanaged
        {
            using var arr = query.ToNativeArray();
            query.Dispose();
            return arr.ToArray();
        }
    }
}