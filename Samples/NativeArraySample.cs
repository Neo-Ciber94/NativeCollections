using System;
using NativeCollections;

namespace Samples
{
    class NativeArraySample
    {
        struct Client : IDisposable
        {
            public NativeString Name { get; }

            public Client(NativeString name)
            {
                Name = name;
            }

            public void Dispose()
            {
                Name.Dispose();
            }
        }

        struct Order : IDisposable
        {
            public NativeString ProductName { get; }
            public Client Client { get; }
            public int ID { get; }
            public double Price { get; }

            public Order(NativeString productName, Client client, int iD, double price)
            {
                ProductName = productName;
                Client = client;
                ID = iD;
                Price = price;
            }

            public void Dispose()
            {
                ProductName.Dispose();
                Client.Dispose();
            }
        }

        public static void Main()
        {
            // Creating an NativeArray of 10 elements
            NativeArray<Order> orders = new NativeArray<Order>(4);

            // Fills the NativeArray
            orders[0] = new Order("Laptop", new Client("Carlos"),   0,  400.0);
            orders[1] = new Order("Shirt",  new Client("Maria"),    1,  200.0);
            orders[2] = new Order("Desk",   new Client("Carlos"),   2,  100.0);
            orders[3] = new Order("Book",   new Client("Takashi"),  3,  50.0);

            // Also 'using' can be used and Dispose will be called for the NativeArray
            using NativeArray<double> prices = new NativeArray<double>(orders.Length);
            int count = 0;

            // Iterate over each element 'by reference'
            foreach (ref Order order in orders)
            {
                prices[count++] = order.Price;
            }

            // [300.0, 200.0, 100.0, 50.0]
            Console.WriteLine(prices);

            double priceSum = 0.0;

            foreach(ref double d in prices)
            {
                priceSum += 0;
            }

            // Dispose the NativeArray
            // By calling Dispose(true), Dispose will be called for each Disposable element in orders.
            orders.Dispose(true);
        }
    }
}
