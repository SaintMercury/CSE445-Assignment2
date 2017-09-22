using System;
using System.Threading;

namespace Assignment2
{
    public delegate void PriceCutEvent(float price); // emit the price and id of plant emitting event

    class Plant
    {
        public static event PriceCutEvent PriceCut; // Link event to delegate
        public OrderBuf OrderBuffer { get; set; }
        public OrderBuf ConfirmationBuffer { get; set; }
        public string PlantName { get; set; }

        private static Semaphore ActiveCountSemaphore = new Semaphore(1, 1);
        private static int NUMBER_OF_ACTIVE_PLANTS = 0;
        private const int MAX_PRICECUTS = 20;
        private const int CARS_PER_TICK = 100;
        private float _currentPrice;
        private float _previousPrice;
        private int _numberOfCars;
        private int priceCuts;
        private AutoResetEvent _modifyNumOfCarsEvent;

        public Plant(OrderBuf orderBuffer, OrderBuf confirmationBuffer)
        {
            // Make previous price so high that we immediately fire a promo event
            this._previousPrice = 0;
            this._currentPrice = 1000000;
            this._numberOfCars = 0;
            this.priceCuts = 0;
            this.OrderBuffer = orderBuffer;
            this.ConfirmationBuffer = confirmationBuffer;
            this._modifyNumOfCarsEvent = new AutoResetEvent(true);
        }

        public void PlantFunc()
        {
            this.startPlant();

            while (this.priceCuts < Plant.MAX_PRICECUTS)
            {
                Thread.Sleep(Program.WAIT_TIME);
                this.produceCars(Plant.CARS_PER_TICK);
                // Take the order from the queue of the orders; // Decide the price based on the orders 
                this.determinePriceAndEmitEvent();
                this.runOrderProcessingThreads();
            }

            this.shutDownPlant();
        }

        private void startPlant()
        {
            this.PlantName = Thread.CurrentThread.Name;
            ActiveCountSemaphore.WaitOne(-1);
            Plant.NUMBER_OF_ACTIVE_PLANTS++;
            Plant.ActiveCountSemaphore.Release();

            string plantName = Thread.CurrentThread.Name;
            Console.WriteLine("Plant {0} is starting up!", plantName);
        }

        private void shutDownPlant()
        {
            Plant.ActiveCountSemaphore.WaitOne(-1);
            Plant.NUMBER_OF_ACTIVE_PLANTS--;
            Plant.ActiveCountSemaphore.Release();

            string plantName = Thread.CurrentThread.Name;
            Console.WriteLine("Plant {0} is shutting down...", plantName);
        }

        private void runOrderProcessingThreads(int threadcount = 5)
        {
            for (int i = 0; i < threadcount ; i++)
            {
                Thread orderProcessor = new Thread(this.getAndProcessOrder);
                orderProcessor.Start();
                Thread.Sleep(Program.WAIT_TIME);
            }
        }

        private void produceCars(int numberOfCars)
        {
            _modifyNumOfCarsEvent.Reset(); // Prevent race condition

            this._numberOfCars += numberOfCars;

            _modifyNumOfCarsEvent.Set();
        }

        private void reduceCars(int numberOfCars)
        {
            _modifyNumOfCarsEvent.Reset();

            this._numberOfCars -= numberOfCars;

            _modifyNumOfCarsEvent.Set();
        }

        private float determinePriceAndEmitEvent()
        {
            int numberOfOrders = 1;
            float stockPrice = 100; //  (float)(new Random()).NextDouble() * 500.0f + 250.0f;

            this._previousPrice = this._currentPrice;
            this._currentPrice = this._previousPrice - 1;  // Pricing.CalculatePrice(numberOfOrders, this._numberOfCars, stockPrice);

            if (this._currentPrice < this._previousPrice)
            {
                if (PriceCut != null)
                {
                    PriceCut(this._currentPrice);
                }
                
                ++priceCuts;
            }

            return this._currentPrice;
        }

        private void getAndProcessOrder()
        {
            string encOrder = OrderBuffer.GetFirstAvailableCell();
            if (encOrder != null)
            {
                Order order = EncDec.DecodeOrder(encOrder);
                this.processOrder(order);
            }
        }

        private bool isValidCC(int ccNum)
        {
            return ccNum >= 5000 && ccNum <= 7000;
        }

        private void processOrder(Order order)
        {
            bool isValidCC = this.isValidCC(order.CardNo);
            if (isValidCC)
            {
                const float SALES_TAX = 1.09f;
                float subTotal = order.Amount * order.UnitPrice;
                float total = subTotal * SALES_TAX;

                this.reduceCars(order.Amount);

                // Date stamp
                order.TimeFulfilled = DateTime.Now;
                order.ReceiverId = PlantName;
                string encodedOrder = EncDec.EncodeOrder(order);

                // Send confirmation
                int index = int.Parse(order.SenderId);
                ConfirmationBuffer.SetCellByIndex(encodedOrder, index);
            }
            else
            {
                Console.WriteLine("\nOrder from Dealer {0} failed due to invalid credit card number", order.SenderId);
            }
        }

        public static int ActivePlantCount()
        {
            return Plant.NUMBER_OF_ACTIVE_PLANTS;
        }
    }
}
