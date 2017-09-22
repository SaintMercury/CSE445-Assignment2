using System;
using System.Security.Cryptography;
using System.Threading;

namespace Assignment2
{
    class Dealer
    {
        private static int NUMBER_OF_ACTIVE_DEALERS = 0;
        private static Semaphore ActiveCountSemaphore = new Semaphore(1, 1);
        private static Random rng = new Random();
        public long CardNo { get; set; }
        public OrderBuf OrderBuffer { get; set; }
        public OrderBuf ConfirmationBuffer { get; set; }
        public int OrderCount { get; set; }
        private int outstandingOrders;
        private int fulfilledOrders;
        public string ThreadName { get; set; }
        public float CurrentPrice { get; set; }
        public float PrevPrice { get; set; }
        public int DealerId { get; set; }
        

        public Dealer(OrderBuf orderBuffer, OrderBuf confirmationOrder, int threadId)
        {
            CardNo = (int)(new Random()).NextDouble() * 9999;

            this.OrderBuffer = orderBuffer;
            this.ConfirmationBuffer = confirmationOrder;
            this.outstandingOrders = 0;
            ThreadName = threadId.ToString();
        }

        private void SendOrder(object threadArg)
        {

            OrderThreadArg orderThreadArg = (OrderThreadArg) threadArg;
            Order order = GenerateOrder(orderThreadArg.Price, ThreadName);
            string encodedOrder = EncDec.EncodeOrder(order);
            OrderBuffer.SetFirstAvailableCell(encodedOrder);
        }

        public void PriceCutHandler(float price)
        {
            
            PrevPrice = CurrentPrice;
            CurrentPrice = price;
            OrderThreadArg threadArg = new OrderThreadArg(price, ThreadName);
            
            Console.WriteLine("Placing an order!");
            Thread sendOrderThread = new Thread(new ParameterizedThreadStart(SendOrder));
            sendOrderThread.Start(threadArg);
            
        }

        private Order GenerateOrder(float price, string threadName)
        {
            int amount = 10; // TODO: need a way to calculate amount
            string dealerName = threadName;
            Order order = new Order(dealerName, CardNo, amount, price);
            lock (this)
            {
                this.OrderCount++;
                this.outstandingOrders++;
            }
            return order;
        }

        public void DealerFunc()
        {
            this.ThreadName = Thread.CurrentThread.Name;

            Console.WriteLine(" Dealer {0} starting up", this.ThreadName);

            Dealer.ActiveCountSemaphore.WaitOne(-1);
            Dealer.NUMBER_OF_ACTIVE_DEALERS++;
            Dealer.ActiveCountSemaphore.Release();

            while (outstandingOrders > 0 || Plant.ActivePlantCount() > 0)
            {
                Thread.Sleep(Program.WAIT_TIME);
                if(this.outstandingOrders > 0)
                    GetOrderConfirmation();
            }

            Dealer.ActiveCountSemaphore.WaitOne(-1);
            Dealer.NUMBER_OF_ACTIVE_DEALERS--;
            Dealer.ActiveCountSemaphore.Release();

            Console.WriteLine("Dealer {0}: Sent to Fulfilled orders: ({1}, {2})", this.ThreadName, this.OrderCount, this.fulfilledOrders);
            Console.WriteLine("Dealer {0} shutting down", this.ThreadName);
        }

        private void GetOrderConfirmation()
        {
            int index = Int32.Parse(Thread.CurrentThread.Name);
            string encodedOrder = ConfirmationBuffer.GetCellByIndex(index);   
            if ( !string.IsNullOrEmpty(encodedOrder) )
            {
                Order order = EncDec.DecodeOrder(encodedOrder);

                lock (this)
                {
                    Console.WriteLine("\nDealer {0} receiving confirmation at: {1}", this.ThreadName, DateTime.Now);
                    Console.WriteLine("Order fulfilled by Plant {0} at {1}", order.ReceiverId, order.TimeFulfilled);
                    this.outstandingOrders--;
                    this.fulfilledOrders++;
                }   
            }
        }

        public static int ActiveDealerCount()
        {
            return Dealer.NUMBER_OF_ACTIVE_DEALERS;
        }
    }

    class OrderThreadArg
    {
        public float Price { get; set; }
        public string ThreadId { get; set; }

        public OrderThreadArg(float price, string threadId)
        {
            Price = price;
            ThreadId = threadId;
        }
    }
}
