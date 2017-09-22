using System;
using System.Diagnostics;
using System.Threading;

namespace Assignment2
{
    class Program
    {
        public static int WAIT_TIME = 50;

        static void initPlants(OrderBuf orderBuffer, OrderBuf confirmationBuffer, int plantCount = 2)
        {
            for(int i = 0; i < plantCount; ++i)
            {
                Plant plant = new Plant(orderBuffer, confirmationBuffer);
                Thread thread = new Thread(new ThreadStart(plant.PlantFunc));
                thread.Name = "Plant Thread: " + i.ToString();
                thread.Start();
            }
        }

        static void initDealers(OrderBuf orderBuffer, OrderBuf confirmationBuffer, int dealerCount = 5)
        {
            for(int i = 0; i < dealerCount; ++i)
            {
                Dealer dealer = new Dealer(orderBuffer, confirmationBuffer);
                Plant.PriceCut += dealer.PriceCutHandler;
                Thread thread = new Thread(new ThreadStart(dealer.DealerFunc));
                thread.Name = "Dealer Thread: " + i.ToString();
                thread.Start();
            }
        }

        static void Main(string[] args)
        {
            const int NUMBER_OF_DEALERS = 5;
            const int NUMBER_OF_PLANTS = 2;

            OrderBuf orderBuffer = new OrderBuf(),
                     confirmationBuffer = new OrderBuf();

            initPlants(orderBuffer, confirmationBuffer, NUMBER_OF_PLANTS);
            initDealers(orderBuffer, confirmationBuffer, NUMBER_OF_DEALERS);
        }
    }
}
