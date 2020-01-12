using System;
using BenchmarkDotNet.Running;
using NativeCollections;
using NativeCollections.Allocators;

namespace NativeCollectionsBenchmark
{
    unsafe class Program
    {
        static void Main()
        {
            //BenchmarkRunner.Run<NativeArrayVsArray>();
            BenchmarkRunner.Run<NativeListVsList>();
            //BenchmarkRunner.Run<NativeMapVsDictionary>();

            //BenchmarkRunner.Run<AllocatorsBenchmark>();
            //BenchmarkRunner.Run<CppAllocatorBenchmark>();
        }
    }
}
