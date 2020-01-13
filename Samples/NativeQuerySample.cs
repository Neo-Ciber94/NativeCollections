using System;
using System.Collections.Generic;
using System.Text;
using NativeCollections;

namespace Samples
{
    class NativeQuerySample
    {
        public static void Main()
        {
            var random = new RandomNumberGenerator(123);

            using NativeList<int> list = new NativeList<int>(10);

            for (int i = 0; i < 100; i++)
            {
                list.Add(random.NextInt(1000));
            }

            // Creates a query over the elements of the list,
            // this copies all the elements of the list into the query
            using NativeQuery<int> query = list.AsQuery();

            // Gets the min and max
            // Must call NativeQuery<T>.Clone() due most of the query operations Dispose the query after being called
            int min = query.Clone().Min();
            int max = query.Clone().Max();

            // Prints: (36, 920)
            Console.WriteLine((min, max));

            using NativeQuery<int> sorted = list
                .AsQuery()              // Creates a query
                .Sorted()               // Sorts the element of the query
                .Where(e => e < 100)    // Filters the elements by lower than 100
                .Reverse()              // Reverse the query
                .Take(5);               // Takes the first 5 elements

            // Must call ToString() to NativeQuery<T> is a ref struct
            // Prints: [94, 87, 86, 63, 57]
            Console.WriteLine(sorted.ToString());
        }
    }
}
