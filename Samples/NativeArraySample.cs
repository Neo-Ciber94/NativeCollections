using System;
using NativeCollections;

namespace Samples
{
    class NativeArraySample
    {
        public static void Run()
        {
            // NativeArray can be created with 'using' so Dispose will be call when its out of scope,
            // Always should declare the initial capacity.
            using NativeArray<int> array = new NativeArray<int>(100);

            // A custom number generator
            RandomNumberGenerator random = new RandomNumberGenerator(123);

            // Most of the NativeContainers can be iterate 'by reference' and/or 'by readonly reference'
            foreach (ref int d in array)
            {
                d = random.NextInt(0, 1000);
            }

            // Is recommended to declare the collections with 'using' to avoid memory leaks.
            NativeArray<IndexedValue<int>> lowerThan200 = new NativeArray<IndexedValue<int>>(array.Length);

            // A LINQ-like ForEach that pass elements by reference
            array.ForEachRef((ref int value, int index) =>
            {
                if(value <= 200)
                {
                    lowerThan200[index] = IndexedValue.Create(value, index);
                }
            });

            // Sorts the array
            lowerThan200.SortBy(e => e.Value);

            var min = lowerThan200[0];
            var max = lowerThan200[^0];

            // Prints the min and max: (0, 184)
            Console.WriteLine((min.Value, max.Value));

            // Dispose the array
            lowerThan200.Dispose();
        }
    }
}
