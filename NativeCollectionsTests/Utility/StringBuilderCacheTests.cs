using NUnit.Framework;
using System.Text;

namespace NativeCollections.Utility.Tests
{
    [TestFixture()]
    public class StringBuilderCacheTests
    {
        [Test()]
        public void Test1()
        {
            StringBuilder sb = StringBuilderCache.Acquire();

            for(int i = 0; i < 10; i++)
            {
                sb.Append('a');
            }

            StringBuilderCache.Release(ref sb!);
            Assert.IsNull(sb);
        }

        [Test()]
        public void Test2()
        {
            StringBuilder sb = StringBuilderCache.Acquire();

            for (int i = 0; i < 5; i++)
            {
                sb.Append('a');
            }

            string str = StringBuilderCache.ToStringAndRelease(ref sb!);
            Assert.IsNull(sb);
            Assert.AreEqual("aaaaa", str);
        }

        [Test()]
        public void Test3()
        {
            StringBuilder sb1 = StringBuilderCache.Acquire();
            StringBuilder sb2 = StringBuilderCache.Acquire();

            Assert.AreNotSame(sb1, sb2);
            StringBuilderCache.Release(ref sb1);
            StringBuilderCache.Release(ref sb2);

            Assert.IsNull(sb1);
            Assert.IsNull(sb2);
        }

        [Test()]
        public void Test4()
        {
            StringBuilder sb1 = StringBuilderCache.Acquire(600);
            StringBuilder sb2 = StringBuilderCache.Acquire(300);

            Assert.AreEqual(600, sb1.Capacity);
            Assert.AreEqual(300, sb2.Capacity);

            StringBuilderCache.Release(ref sb1);
            StringBuilderCache.Release(ref sb2);

            StringBuilder sb3 = StringBuilderCache.Acquire();
            Assert.AreEqual(300, sb3.Capacity);
            StringBuilderCache.Release(ref sb3!);

            Assert.IsNull(sb1);
            Assert.IsNull(sb2);
            Assert.IsNull(sb3);
        }
    }
}