using System.Threading;

namespace Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            const int NUMBER_OF_DEALERS = 3;
            
            OrderBuf orderBuffer = new OrderBuf(),
                     confirmationBuffer = new Assignment2.OrderBuf();

            Dealer[] dealers = new Dealer[NUMBER_OF_DEALERS];
            Thread[] dealerThreads = new Thread[NUMBER_OF_DEALERS];

            for (int i = 0; i < NUMBER_OF_DEALERS; ++i)
            {
                dealers[i] = new Dealer(orderBuffer, confirmationBuffer);
                dealerThreads[i] = new Thread(new ThreadStart(dealers[i].DealerFunc));
                dealerThreads[i].Name = (i + 1).ToString();

                // Enlist those dealers into an event handler to server their country
                Plant.PriceCut += dealers[i].PriceCutHandler;

                // Politely ask our threads to start when they can. Be sure not rush the threads,
                // it helps to make sure they don't get spiteful and deadlock.
                dealerThreads[i].Start();
            }

            Plant plant1 = new Plant(orderBuffer, confirmationBuffer),
                  plant2 = new Plant(orderBuffer, confirmationBuffer);

            Thread plantThread1 = new Thread(new ThreadStart(plant1.PlantFunc)),
                   plantThread2 = new Thread(new ThreadStart(plant2.PlantFunc));

            plantThread1.Name = "Plant thread 1";
            plantThread2.Name = "Plant thread 2";

            plantThread1.Start();
            plantThread2.Start();
        }
    }
}
