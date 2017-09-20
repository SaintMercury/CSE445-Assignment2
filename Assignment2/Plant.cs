using System;
using System.Threading;

namespace Assignment2
{
    public delegate void PriceCutEvent(float price, string plantId); // emit the price and id of plant emitting event

    class Plant
    {
        private const int MAX_PRICECUTS = 20;
        private float currentPrice;
        private float previousPrice;
        private int numberOfCars; // not sure we need this
        private int priceCuts;
        public static event PriceCutEvent PriceCut; // Link event to delegate
        public OrderBuf Buffer { get; set; } 

        public Plant(OrderBuf buffer)
        {
            // Make previous price so high that we immediately fire a promo event
            this.previousPrice = (float)double.PositiveInfinity;
            this.numberOfCars = 0;
            this.priceCuts = 0;
            Buffer = buffer;
        }

        public int getPriceCuts => this.priceCuts;

        public void PlantFunc()
        {
            var plantName = Thread.CurrentThread.Name;

            while (priceCuts < MAX_PRICECUTS)
            {
                Thread.Sleep(1000);
                // Take the order from the queue of the orders; // Decide the price based on the orders 
                getOrder(plantName);
                currentPrice = determinePrice();
            }
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

                Console.WriteLine("promotion");
                ++priceCuts;
            }

            return price;
        }

        public void getOrder(string plantName) // INCOMPLETE
        {
            var encOrder = Buffer.GetCell(plantName);
            if (encOrder != null)
            {
                Order order = EncDec.DecodeOrder(encOrder);
                processOrder(order);
            }
        }

        private void processOrder(Order order) // INCOMPLETE
        {
            // Make a new thread, do the thing, dab, close thread or whatever

            const double SALES_TAX = .9;
            var subTotal = order.Amount * order.UnitPrice;
            var total = (subTotal * SALES_TAX) + subTotal;
            var fulfillmentTime = DateTime.Now;

            Console.WriteLine("Order for " + order.SenderId + 
                " processed at " + fulfillmentTime + " for total of: " + total);

            //TODO: Send confirmation to dealer
        }

        private void emitPromotionalEvent() // INCOMPLETE
        {
            // Do what we gotta do

        }
    }

    class PlantThread
    {
        public void Run(OrderBuf buffer) // INCOMPLETE
        {
            const int MAX_PRICECUTS = 20;
            Plant plant = new Plant(buffer);

            while(plant.getPriceCuts < MAX_PRICECUTS)
            {
                plant.produceCars(10);
                plant.determinePrice();
            }
        }
    }
}
