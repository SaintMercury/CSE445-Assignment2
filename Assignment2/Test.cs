using System;
using System.Collections.Generic;
using System.Threading;

namespace Assignment2
{
    class Test
    {
        public OrderBuffer OrderBuffer { get; set; }
        public string ThreadName { get; set; }
        public Random rand { get; set; }

        protected List<Order> OrderList;
        

        public Test(OrderBuffer orderBuffer, string threadName, Random random)
        {
            OrderBuffer = orderBuffer;
            rand = random;
            ThreadName = threadName;
            var orderList = new List<Order>(10);
            for (var i = 0; i < 10; ++i)
            {
                orderList.Add(new Order(ThreadName, i, i, i));
            }
            OrderList = orderList;
        }

        public void TestThread()
        {
            Thread.Sleep(100);
            int randInt;
            
            while (true)
            {
                randInt = rand.Next(0, 10);

                if (randInt % 2 == 0)
                {
                    var orderStr = EncDec.EncodeOrder(OrderList[randInt]);
                    Console.WriteLine("Setting cell");
                    OrderBuffer.SetAvailablCell(orderStr);
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.WriteLine("Getting cell...");
                    var str = OrderBuffer.GetAvailableCell();
                    Thread.Sleep(500);
                    Console.WriteLine(str);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
