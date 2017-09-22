using System.Threading;

namespace Assignment2
{
    class Program
    {
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

        static void Main(string[] args)
        {
            const int NUMBER_OF_DEALERS = 5;
            const int NUMBER_OF_PLANTS = 2;
            const int ORDER_BUFFER_SIZE = 3;
            const int CONF_BUFFER_SIZE = 5;
            
            OrderBuf orderBuffer = new OrderBuf(ORDER_BUFFER_SIZE),
                     confirmationBuffer = new OrderBuf(CONF_BUFFER_SIZE);

            initPlants(orderBuffer, confirmationBuffer, NUMBER_OF_PLANTS);

            // Subscribing particular threads to an event remains unsolved
            Dealer[] dealers = new Dealer[NUMBER_OF_DEALERS];
            Thread[] dealerThreads = new Thread[NUMBER_OF_DEALERS];
            
            for (int i = 0; i < NUMBER_OF_DEALERS; ++i)
            {
                dealers[i] = new Dealer(orderBuffer, confirmationBuffer);
                dealerThreads[i] = new Thread(new ThreadStart(dealers[i].DealerFunc));
                dealerThreads[i].Name = i.ToString();

                // Enlist those dealers into an event handler to server their country
                Plant.PriceCut += dealers[i].PriceCutHandler;

                // Politely ask our threads to start when they can. Be sure not rush the threads,
                // it helps to make sure they don't get spiteful and deadlock.
                dealerThreads[i].Start();
            }
        }
    }
}
