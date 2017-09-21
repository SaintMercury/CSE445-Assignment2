using System;
using System.Threading;

namespace Assignment2
{
    class Dealer
    {
        public long CardNo { get; set; }
        public OrderBuf OrderBuffer { get; set; }
        public OrderBuf ConfirmationBuffer { get; set; }
        public int OrderCount { get; set; }
        public string ThreadName { get; set; }
        

        public Dealer(OrderBuf orderBuffer, OrderBuf confirmationOrder)
        {
            CardNo = 5000;

            this.OrderBuffer = orderBuffer;
            this.ConfirmationBuffer = confirmationOrder;
        }

        public void PriceCutHandler(float price, string plantId)
        {
            Order order = GenerateOrder(price, plantId);
            string encodedOrder = EncDec.EncodeOrder(order);
            OrderBuffer.SetCell(encodedOrder);
        }

        public Order GenerateOrder(float price, string receiverId)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = ThreadName;
            }

            int amount = 10; // TODO: need a way to calculate amount
            string dealerName = this.ThreadName;
            Order order = new Order(dealerName, CardNo, amount, price, receiverId);
            this.OrderCount++;
            Console.WriteLine("Order no: " + OrderCount + " generated");

            return order;
        }

        public void DealerFunc()
        {
            while(true) // Hmmm... the dealers need to know how many plants still exist inorder to shut down
            {
                Thread.Sleep(1000);
                //Console.WriteLine("Awaiting promotional event");
            }
        }
    }
}
