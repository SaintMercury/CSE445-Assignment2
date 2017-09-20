using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment2
{
    class Dealer
    {
        public long CardNo { get; set; }
        public OrderBuf Buffer { get; set; }
        public int OrderCount { get; set; }
        public string ThreadName { get; set; }
        

        public Dealer(OrderBuf buffer)
        {
            CardNo = 5000;
            Buffer = buffer;
        }
        public void PriceCutHandler(float price, string plantId)
        {
            var order = GenerateOrder(price, plantId);
            var encodedOrder = EncDec.EncodeOrder(order);
            Buffer.SetCell(encodedOrder);
        }

        public Order GenerateOrder(float price, string receiverId)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = ThreadName;
            }

            var amount = 10; // TODO: need a way to calculate amount
            var dealerName = Thread.CurrentThread.Name;
            var order = new Order(dealerName, CardNo, amount, price, receiverId);
            OrderCount++;
            Console.WriteLine("Order no: " + OrderCount + " generated");
            return order;
        }

        public void DealerFunc()
        {
            for (int i = 0; i < 50; i++)
            {
                Thread.Sleep(1000);
                //Console.WriteLine("Awaiting promotional event");
            }
        }
    }
}
