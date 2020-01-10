using System;
using BenchmarkDotNet.Running;

namespace NativeCollectionsBenchmark
{
    class Runner
    {
        static void Main()
        {
            //BenchmarkRunner.Run<NativeArrayVsArray>();
            //BenchmarkRunner.Run<NativeListVsList>();
            BenchmarkRunner.Run<NativeMapVsDictionary>();
        }
    }
}
