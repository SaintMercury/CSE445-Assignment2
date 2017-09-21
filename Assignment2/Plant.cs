using System;
using System.Threading;

namespace Assignment2
{
    public delegate void PriceCutEvent(float price, string plantId); // emit the price and id of plant emitting event

    class Plant
    {
        public static event PriceCutEvent PriceCut; // Link event to delegate
        private const int MAX_PRICECUTS = 3;

        private float currentPrice;
        private float previousPrice;
        private int numberOfCars;
        private int priceCuts;
        public OrderBuf OrderBuffer { get; set; }
        public OrderBuf ConfirmationBuffer { get; set; }

        public Plant(OrderBuf orderBuffer, OrderBuf confirmationOrder)
        {
            // Make previous price so high that we immediately fire a promo event
            this.previousPrice = (float)double.PositiveInfinity;
            this.numberOfCars = 0;
            this.priceCuts = 0;
            this.OrderBuffer = orderBuffer;
            this.ConfirmationBuffer = confirmationOrder;
        }

        public int getPriceCuts => this.priceCuts;

        public void PlantFunc()
        {
            string plantName = Thread.CurrentThread.Name;

            Console.WriteLine("{0} is starting up!", plantName);

            while (this.priceCuts < MAX_PRICECUTS)
            {
                Thread.Sleep(100);
                // Take the order from the queue of the orders; // Decide the price based on the orders 
                getOrder(plantName);
                currentPrice = determinePrice();
            }

            Console.WriteLine("{0} is shutting down...", plantName);
            OrderBuffer.ShutDownPlant();
        }

        public void produceCars(int numberOfCars)
        {
            this.numberOfCars += numberOfCars;
        }

        public float determinePrice() // INCOMPLETE
        {
            int numberOfOrders = 0, stockPrice = 0;

            float price = Pricing.CalculatePrice(numberOfOrders, this.numberOfCars, stockPrice);

            if (price < currentPrice)
            {
                var plantName = Thread.CurrentThread.Name;
                if (PriceCut != null)
                    PriceCut(price, plantName);

                ++priceCuts;
            }

            return price;
        }

        public void getOrder(string plantName) // INCOMPLETE
        {
            string encOrder = OrderBuffer.GetCell();
            if (encOrder != null)
            {
                Order order = EncDec.DecodeOrder(encOrder);
                processOrder(order);
            }
        }

        private void processOrder(Order order) // INCOMPLETE
        {
            // Make a new thread, do the thing, dab, close thread or whatever

            const float SALES_TAX = 0.9f;
            float subTotal = order.Amount * order.UnitPrice;
            float total = (subTotal * SALES_TAX) + subTotal;
            order.TimeFulfilled = DateTime.Now;

            string encodedOrder = EncDec.EncodeOrder(order);
            ConfirmationBuffer.SetCell(encodedOrder);

            //TODO: Send confirmation to dealer
        }

        private void emitPromotionalEvent() // INCOMPLETE
        {
            // Do what we gotta do

        }
    }
}
