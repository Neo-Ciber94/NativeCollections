using NUnit.Framework;
using NativeCollections.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Buffers.Tests
{
    [TestFixture()]
    public class NativeBufferTests
    {
        [Test()]
        public void NativeBufferTest()
        {
            using NativeBuffer buffer = new NativeBuffer(40);
            Assert.AreEqual(40, buffer.Capacity);
            Assert.AreEqual(0, buffer.Length);
            Assert.IsTrue(buffer.IsValid);
            Assert.IsTrue(buffer.IsEmpty);
        }

        [Test()]
        public void WriteTest()
        {
            using NativeBuffer buffer = new NativeBuffer(20);
            buffer.Write(10);
            buffer.Write(40L);
            buffer.Write('a');
            buffer.Write('z');
            buffer.Write(4f);

            Assert.AreEqual(20, buffer.Length);
            Assert.Throws<InvalidOperationException>(() => buffer.Write(10));
        }

        [Test()]
        public void TryWriteTest()
        {
            using NativeBuffer buffer = new NativeBuffer(20);
            Assert.IsTrue(buffer.TryWrite(10));
            Assert.IsTrue(buffer.TryWrite(40L));
            Assert.IsTrue(buffer.TryWrite('a'));
            Assert.IsTrue(buffer.TryWrite('z'));
            Assert.IsTrue(buffer.TryWrite(4f));

            Assert.AreEqual(20, buffer.Length);
            Assert.IsFalse(buffer.TryWrite(6f));
        }

        [Test()]
        public void ReadTest()
        {
            using NativeBuffer buffer = new NativeBuffer(20);
            buffer.Write(10);
            buffer.Write(40L);
            buffer.Write('a');
            buffer.Write('z');
            buffer.Write(4f);

            Assert.AreEqual(10, buffer.Read<int>(0));
            Assert.AreEqual(40L, buffer.Read<long>(4));
            Assert.AreEqual('a', buffer.Read<char>(12));
            Assert.AreEqual('z', buffer.Read<char>(14));
            Assert.AreEqual(4f, buffer.Read<float>(16));

            Assert.Throws<InvalidOperationException>(() => buffer.Read<byte>(-1));
            Assert.Throws<InvalidOperationException>(() => buffer.Read<byte>(20));
        }

        [Test()]
        public void TryReadTest()
        {
            using NativeBuffer buffer = new NativeBuffer(20);
            buffer.Write(10);
            buffer.Write(40L);
            buffer.Write('a');
            buffer.Write('z');
            buffer.Write(4f);

            Assert.IsTrue(buffer.TryRead<int>(0, out var v1));
            Assert.AreEqual(10, v1);

            Assert.IsTrue(buffer.TryRead<long>(4, out var v2));
            Assert.AreEqual(40L, v2);

            Assert.IsTrue(buffer.TryRead<char>(12, out var v3));
            Assert.AreEqual('a', v3);

            Assert.IsTrue(buffer.TryRead<char>(14, out var v4));
            Assert.AreEqual('z', v4);

            Assert.IsTrue(buffer.TryRead<float>(16, out var v5));
            Assert.AreEqual(4f, v5);
        }

        [Test()]
        public void TryReadTest1()
        {
            using NativeBuffer buffer = new NativeBuffer(20);
            buffer.Write(0);
            buffer.Write(1);
            buffer.Write(2);
            buffer.Write(4);
            buffer.Write(8);

            Span<byte> span = stackalloc byte[8];
            Assert.IsTrue(buffer.TryRead(4, 8, span));
            Assert.AreEqual(1, span[0]);
            Assert.AreEqual(0, span[1]);
            Assert.AreEqual(0, span[2]);
            Assert.AreEqual(0, span[3]);
            Assert.AreEqual(2, span[4]);
            Assert.AreEqual(0, span[5]);
            Assert.AreEqual(0, span[6]);
            Assert.AreEqual(0, span[7]);
        }

        [Test()]
        public void ClearTest()
        {
            using NativeBuffer buffer = new NativeBuffer(20);
            buffer.Write(0);
            buffer.Write(1);
            buffer.Write(2);
            buffer.Write(4);

            Assert.AreEqual(16, buffer.Length);
            buffer.Clear();

            Assert.AreEqual(0, buffer.Length);
            Assert.AreEqual(20, buffer.Capacity);
        }

        [Test()]
        public void CopyToTest()
        {
            using NativeBuffer buffer = new NativeBuffer(20);
            buffer.Write(1);
            buffer.Write(2);
            buffer.Write(4);
            buffer.Write(8);

            Span<byte> span = stackalloc byte[8];
            buffer.CopyTo(span, 0, 8);
            Assert.AreEqual(1, span[0]);
            Assert.AreEqual(0, span[1]);
            Assert.AreEqual(0, span[2]);
            Assert.AreEqual(0, span[3]);
            Assert.AreEqual(2, span[4]);
            Assert.AreEqual(0, span[5]);
            Assert.AreEqual(0, span[6]);
            Assert.AreEqual(0, span[7]);
        }

        [Test()]
        unsafe public void GetUnsafePointerTest()
        {
            using NativeBuffer buffer = new NativeBuffer(20);
            buffer.Write(1);
            buffer.Write(2);
            buffer.Write(4);
            buffer.Write(8);

            byte* ptr = buffer.GetUnsafePointer();
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(0, ptr[1]);
            Assert.AreEqual(0, ptr[2]);
            Assert.AreEqual(0, ptr[3]);
            Assert.AreEqual(2, ptr[4]);
            Assert.AreEqual(0, ptr[5]);
            Assert.AreEqual(0, ptr[6]);
            Assert.AreEqual(0, ptr[7]);
            Assert.AreEqual(4, ptr[8]);
            Assert.AreEqual(0, ptr[9]);
            Assert.AreEqual(0, ptr[10]);
            Assert.AreEqual(0, ptr[11]);
            Assert.AreEqual(8, ptr[12]);
            Assert.AreEqual(0, ptr[13]);
            Assert.AreEqual(0, ptr[14]);
            Assert.AreEqual(0, ptr[15]);
        }

        [Test()]
        public void ToArrayTest()
        {
            using NativeBuffer buffer = new NativeBuffer(20);
            buffer.Write(1);
            buffer.Write(2);

            byte[] arr = buffer.ToArray();
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(0, arr[1]);
            Assert.AreEqual(0, arr[2]);
            Assert.AreEqual(0, arr[3]);
            Assert.AreEqual(2, arr[4]);
            Assert.AreEqual(0, arr[5]);
            Assert.AreEqual(0, arr[6]);
            Assert.AreEqual(0, arr[7]);
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using NativeBuffer buffer = new NativeBuffer(20);
            buffer.Write(1);
            buffer.Write(2);

            var enumerator = buffer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(0, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(0, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(0, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(0, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(0, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());

            Assert.AreEqual(0, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void DisposeTest()
        {
            NativeBuffer buffer = new NativeBuffer(20);
            buffer.Write(1);
            buffer.Write(2);

            buffer.Dispose();
            Assert.AreEqual(0, buffer.Length);
            Assert.AreEqual(0, buffer.Capacity);
            Assert.IsFalse(buffer.IsValid);
            Assert.IsTrue(buffer.IsEmpty);
        }
    }
}