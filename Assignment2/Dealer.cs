using System;
using System.Threading;

namespace Assignment2
{
    class Dealer
    {
        private static int NUMBER_OF_ACTIVE_DEALERS = 0;
        private static Semaphore ActiveCountSemaphore = new Semaphore(1, 1);

        public long CardNo { get; set; }
        public OrderBuf OrderBuffer { get; set; }
        public OrderBuf ConfirmationBuffer { get; set; }
        public int OrderCount { get; set; }
        public string ThreadName { get; set; }
        public float CurrentPrice { get; set; }
        private int outstandingOrders;
        

        public Dealer(OrderBuf orderBuffer, OrderBuf confirmationOrder)
        {
            CardNo = (int)(new Random()).NextDouble() * 9999;

            this.OrderBuffer = orderBuffer;
            this.ConfirmationBuffer = confirmationOrder;
            this.outstandingOrders = 0;
        }

        private void SendOrder()
        {
            Order order = GenerateOrder(CurrentPrice);
            string encodedOrder = EncDec.EncodeOrder(order);
            OrderBuffer.SetCell(encodedOrder);
        }

        public void PriceCutHandler(float price)
        {
            CurrentPrice = price;
            Console.WriteLine("Placing an order!");
            SendOrder();
        }

        private Order GenerateOrder(float price)
        {
            int amount = 10; // TODO: need a way to calculate amount
            string dealerName = this.ThreadName;
            Order order = new Order(dealerName, CardNo, amount, price);
            this.OrderCount++;
            this.outstandingOrders++;

            return order;
        }

        public void DealerFunc()
        {
            this.ThreadName = Thread.CurrentThread.Name;

            Console.WriteLine("{0} starting up", this.ThreadName);

            Dealer.ActiveCountSemaphore.WaitOne(-1);
            Dealer.NUMBER_OF_ACTIVE_DEALERS++;
            Dealer.ActiveCountSemaphore.Release();

            while (Plant.ActivePlantCount() > 0)
            {
                Thread.Sleep(Program.WAIT_TIME);
                GetOrderConfirmation();
                Console.WriteLine("Plant Count that I ({1}) see: {0}", Plant.ActivePlantCount(), this.ThreadName);
            }

            Dealer.ActiveCountSemaphore.WaitOne(-1);
            Dealer.NUMBER_OF_ACTIVE_DEALERS--;
            Dealer.ActiveCountSemaphore.Release();

            Console.WriteLine("{0} shutting down", this.ThreadName);
        }

        private void GetOrderConfirmation()
        {
            string encodedOrder = ConfirmationBuffer.GetCell();   
            if ( !string.IsNullOrEmpty(encodedOrder) )
            {
                Order order = EncDec.DecodeOrder(encodedOrder);
                Console.WriteLine("\n{0} receiving confirmation at: {1}", this.ThreadName, DateTime.Now);
                Console.WriteLine("Order fulfilled by {0} at {1}", order.ReceiverId, order.TimeFulfilled);
                this.outstandingOrders--;
            }
        }

        public static int ActiveDealerCount()
        {
            return Dealer.NUMBER_OF_ACTIVE_DEALERS;
        }
    }
}
