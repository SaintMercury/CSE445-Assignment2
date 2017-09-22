using System;
using System.Threading;

namespace Assignment2
{
    class Dealer
    {
        private static int NUMBER_OF_ACTIVE_DEALERS = 0;

        public long CardNo { get; set; }
        public OrderBuf OrderBuffer { get; set; }
        public OrderBuf ConfirmationBuffer { get; set; }
        public int OrderCount { get; set; }
        public string ThreadName { get; set; }
        public bool WasRecentPriceCut { get; set; }
        public float CurrentPrice { get; set; }
        public float PrevPrice { get; set; }
        

        public Dealer(OrderBuf orderBuffer, OrderBuf confirmationOrder)
        {
            CardNo = 5000;

            this.OrderBuffer = orderBuffer;
            this.ConfirmationBuffer = confirmationOrder;
        }

        private void SendOrder()
        {
            Order order = GenerateOrder(CurrentPrice);
            string encodedOrder = EncDec.EncodeOrder(order);
            OrderBuffer.SetFirstAvailableCell(encodedOrder);
        }

        public void PriceCutHandler(float price)
        {
            WasRecentPriceCut = true;
            PrevPrice = CurrentPrice;
            CurrentPrice = price;
        }

        private Order GenerateOrder(float price)
        {
            int amount = 10; // TODO: need a way to calculate amount
            string dealerName = Thread.CurrentThread.Name;
            Order order = new Order(dealerName, CardNo, amount, price);
            this.OrderCount++;

            return order;
        }

        public void DealerFunc()
        {
            Console.WriteLine("Dealer {0} starting up", Thread.CurrentThread.Name);

            while (Plant.ActivePlantCount() > 0)
            {
                Thread.Sleep(400);
                if (WasRecentPriceCut)
                {
                    SendOrder();
                    WasRecentPriceCut = false;
                }
                GetOrderConfirmation();
            }

            Console.WriteLine("Dealer {0} shutting down", Thread.CurrentThread.Name);
        }

        private void GetOrderConfirmation()
        {
            int index = Int32.Parse(Thread.CurrentThread.Name);
            string encodedOrder = ConfirmationBuffer.GetCellByIndex(index);   
            if ( !string.IsNullOrEmpty(encodedOrder) )
            {
                Order order = EncDec.DecodeOrder(encodedOrder);
                Console.WriteLine("\nDealer {0} receiving order confirmation at: {1}", Thread.CurrentThread.Name, DateTime.Now);
                Console.WriteLine("Order fulfilled by {0} at {1}", order.ReceiverId, order.TimeFulfilled);
            }
        }

        public static int ActiveDealerCount()
        {
            return Dealer.NUMBER_OF_ACTIVE_DEALERS;
        }
    }
}
