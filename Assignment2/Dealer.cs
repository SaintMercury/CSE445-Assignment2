using System;
using System.Threading;

namespace Assignment2
{
    class Dealer
    {
        public OrderBuf OrderBuffer { get; set; }
        public OrderBuf ConfirmationBuffer { get; set; }
        public int OrderCount { get; set; }
        public string ThreadName { get; set; }

        private float _currentPrice;
        private float _prevPrice;
        private AutoResetEvent _priceEvent;
        private static int NUMBER_OF_ACTIVE_DEALERS = 0;
        private static Semaphore _activeCountSemaphore = new Semaphore(1, 1);
        private static Random _rng;
        private int _outstandingOrders;
        private int _fulfilledOrders;
        private static AutoResetEvent _printEvent = new AutoResetEvent(true);

        public Dealer(OrderBuf orderBuffer, OrderBuf confirmationOrder, int threadId, Random rng)
        {
            this.OrderBuffer = orderBuffer;
            this.ConfirmationBuffer = confirmationOrder;
            this._outstandingOrders = 0;
            ThreadName = threadId.ToString();
            _priceEvent = new AutoResetEvent(true);
            _rng = rng;
        }

        public void DealerFunc()
        {
            startDealer();

            while (Plant.ActivePlantCount() > 0)
            {
                // Periodically check for order confirmations
                Thread.Sleep(Program.WAIT_TIME);
                GetOrderConfirmation();
            }

            shutDownDealer();
        }

        public void PriceCutHandler(float price)
        {
            // Store a snapshot of the current state using a synchronized method
            OrderThreadArg threadArg = captureCurrentState(price);

            if (threadArg.PreviousPrice > price) // got ourselves a deal
            {
                // Instantiate thread
                Thread sendOrderThread = new Thread(new ParameterizedThreadStart(generateAndSendOrder));

                // Start thread and supply it with our cached snapshot (needed to generate order)
                sendOrderThread.Start(threadArg);
            }
        }

        public static int ActiveDealerCount()
        {
            return Dealer.NUMBER_OF_ACTIVE_DEALERS;
        }

        private void generateAndSendOrder(object threadArg)
        {
            OrderThreadArg orderThreadArg = (OrderThreadArg) threadArg;
            Order order = GenerateOrder(orderThreadArg.PreviousPrice, orderThreadArg.Price, ThreadName);

            string encodedOrder = EncDec.EncodeOrder(order);
            OrderBuffer.SetFirstAvailableCell(encodedOrder);
        }

        private int calculateAmount(float prevPrice, float price)
        {
            int amount = ( (int)prevPrice - (int)price) * 5;
            
            return amount;
        }

        private Order GenerateOrder(float prevPrice, float price, string threadName)
        {
            int amount = this.calculateAmount(prevPrice, price);

            int cardNo = _rng.Next(4700, 7000); // Use a different card every time and plant will validate
            Order order = new Order(threadName, cardNo, amount, price);

            lock (this)
            {
                this.OrderCount++;
                this._outstandingOrders++;
            }

            return order;
        }

        private void startDealer()
        {
            Console.WriteLine("Dealer {0} starting up", this.ThreadName);
            Dealer._activeCountSemaphore.WaitOne(-1);
            Dealer.NUMBER_OF_ACTIVE_DEALERS++;
            Dealer._activeCountSemaphore.Release();
        }

        private void shutDownDealer()
        {
            Dealer._activeCountSemaphore.WaitOne(-1);

            Dealer.NUMBER_OF_ACTIVE_DEALERS--;
            Console.WriteLine("\nDealer {0}: Total orders sent: {1} \nOrders fulfilled: {2}", this.ThreadName, this.OrderCount, this._fulfilledOrders);
            Console.WriteLine("Orders failed due to invalid credit card: {0}", this._outstandingOrders);
            Console.WriteLine("Dealer {0} shutting down", this.ThreadName);

            Dealer._activeCountSemaphore.Release();
        }

        private void GetOrderConfirmation()
        {
            int index = int.Parse(this.ThreadName);
            string encodedOrder = ConfirmationBuffer.GetCellByIndex(index);   
            if ( !string.IsNullOrEmpty(encodedOrder) )
            {
                Order order = EncDec.DecodeOrder(encodedOrder);

                _printEvent.WaitOne();  // Ensures that we don't get interrupted and print out of order

                Console.WriteLine("\nDealer {0} receiving confirmation at: {1}", this.ThreadName, DateTime.Now);
                Console.WriteLine("Order fulfilled by Plant {0} at {1}", order.ReceiverId, order.TimeFulfilled);
                Console.WriteLine("Amount of cars: {0} at ${1} per vehicle.", order.Amount, order.UnitPrice);
                this._outstandingOrders--;
                this._fulfilledOrders++;

                _printEvent.Set();
            }
        }

        private OrderThreadArg captureCurrentState(float price)
        {
            _priceEvent.WaitOne(); // Block other threads until state has been captured and stored

            _prevPrice = _currentPrice; 
            _currentPrice = price;
            OrderThreadArg threadArg = new OrderThreadArg(_currentPrice, this.ThreadName, _prevPrice);

            _priceEvent.Set();  // state captured, let another thread in

            return threadArg;
        }
    }
}
