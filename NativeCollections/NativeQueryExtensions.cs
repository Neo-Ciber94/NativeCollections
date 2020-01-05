using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections
{
    public static class NativeQueryExtensions
    {
        public static T Min<T>(this NativeQuery<T> query) where T: unmanaged, IComparable<T>
        {
            T? min = null;

            foreach(ref var e in query)
            {
                if(min == null)
                {
                    min = e;
                }
                else
                {
                    int c = e.CompareTo(e);
                    if(c < 0)
                    {
                        min = e;
                    }
                }
            }

            if(min == null)
            {
                throw new InvalidOperationException("Cannot find the min value.");
            }

            query.Dispose();
            return min.Value;
        }

        public static T Max<T>(this NativeQuery<T> query) where T : unmanaged, IComparable<T>
        {
            T? max = null;

            foreach (ref var e in query)
            {
                if (max == null)
                {
                    max = e;
                }
                else
                {
                    int c = e.CompareTo(e);
                    if (c > 0)
                    {
                        max = e;
                    }
                }
            }

            if (max == null)
            {
                throw new InvalidOperationException("Cannot find the min value.");
            }

            query.Dispose();
            return max.Value;
        }

        public static int Sum(this NativeQuery<int> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            int total = 0;
            foreach(ref var e in query)
            {
                total += e;
            }

            return total;
        }

        public static long Sum(this NativeQuery<long> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            long total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        public static float Sum(this NativeQuery<float> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            float total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        public static double Sum(this NativeQuery<double> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            double total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        public static decimal Sum(this NativeQuery<decimal> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            decimal total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        public static int Average(this NativeQuery<int> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            int total = 0;
            foreach(ref var e in query)
            {
                total += e;
            }

            return total / query.Length;
        }

        public static long Average(this NativeQuery<long> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            long total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / query.Length;
        }

        public static float Average(this NativeQuery<float> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            float total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / query.Length;
        }

        public static double Average(this NativeQuery<double> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            double total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / query.Length;
        }

        public static decimal Average(this NativeQuery<decimal> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            decimal total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / query.Length;
        }
    }
}
