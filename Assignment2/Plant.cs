using System.Threading;

namespace Assignment2
{
    class Plant
    {
        pricate float currentPrice;
        private float previousPrice;
        private int numberOfCars;
        private int priceCuts;

        public Plant()
        {
            // Make previous price so high that we immediately fire a promo event
            this.previousPrice = (float)double.PositiveInfinity;
            this.numberOfCars = 0;
            this.priceCuts = 0;
        }

        public int getPriceCuts => this.priceCuts;

        public void produceCars(int numberOfCars)
        {
            this.numberOfCars += numberOfCars;
        }

        public void determinePrice() // INCOMPLETE
        {
            int numberOfOrders = 0, stockPrice = 0;

            float price = Pricing.CalculatePrice(numberOfOrders, this.numberOfCars, stockPrice);

            // IDK rn
        }

        public void getOrder(string encryptedOrder) // INCOMPLETE
        {
            Order order = EncDec.DecodeOrder(encryptedOrder);
            this.processOrder(order);
        }

        private void processOrder(Order order) // INCOMPLETE
        {
            // Make a new thread, do the thing, dab, close thread or whatever
        }

        private void emitPromotionalEvent() // INCOMPLETE
        {
            // Do what we gotta do

            ++this.priceCuts;
        }
    }

    class PlantThread
    {
        public void Run() // INCOMPLETE
        {
            const int MAX_PRICECUTS = 20;
            Plant plant = new Plant();

            while(plant.getPriceCuts < MAX_PRICECUTS)
            {
                plant.produceCars(10);
                plant.determinePrice();
            }
        }
    }
}
