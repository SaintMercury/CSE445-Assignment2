using System.Threading;

namespace Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfCells = 3;
            const int NUMBER_OF_DEALERS = 3;
            
//            var buffer = new OrderBuffer(numberOfCells);
//            var rand = new Random();
//            var test1 = new Test(buffer, "thread_1", rand);
//            var test2 = new Test(buffer, "thread_2", rand);
//            var test3 = new Test(buffer, "thread_3", rand);
//            var test4 = new Test(buffer, "thread_4", rand);
//            var test5 = new Test(buffer, "thread_5", rand);
//            var thread1 = new Thread(new ThreadStart(test1.TestThread));
//            var thread2 = new Thread(new ThreadStart(test2.TestThread));
//            var thread3 = new Thread(new ThreadStart(test3.TestThread));
//            var thread4 = new Thread(new ThreadStart(test4.TestThread));
//            var thread5 = new Thread(new ThreadStart(test5.TestThread));
//
//            thread1.Start();
//            thread2.Start();
//            thread3.Start();
//            thread4.Start();
//            thread5.Start();

            
            OrderBuf buffer = new OrderBuf();

            Plant plant1 = new Plant(buffer),
                  plant2 = new Plant(buffer);

            Thread plantThread1 = new Thread(new ThreadStart(plant1.PlantFunc)),
                   plantThread2 = new Thread(new ThreadStart(plant2.PlantFunc));

            plantThread1.Name = "Plant thread 1";
            plantThread2.Name = "Plant thread 2";

            plantThread1.Start();
            plantThread2.Start();

            Dealer[] dealers = new Dealer[NUMBER_OF_DEALERS];
            Thread[] dealerThreads = new Thread[NUMBER_OF_DEALERS];

            for (int i = 0; i < NUMBER_OF_DEALERS; ++i) 
            {	// Start N retailer threads
                dealers[i] = new Assignment2.Dealer(buffer);
                dealerThreads[i] = new Thread(new ThreadStart(dealers[i].DealerFunc));
                dealerThreads[i].Name = (i + 1).ToString();

                // Enlist those dealers into an event handler to server their country
                Plant.PriceCut += dealers[i].PriceCutHandler;

                // Politely ask our threads to start when they can. Be sure not rush the threads,
                // it helps to make sure they don't get spiteful and deadlock.
                dealerThreads[i].Start();
            }

            //            var dealerList = new List<Dealer>();
            //            var dealerThreads = new List<Thread>();
            //            for (int i = 0; i < 2; i++)
            //            {
            //                dealerList.Add(new Dealer(buffer));
            //                Plant.PriceCut += dealerList[i].PriceCutHandler;
            //                dealerThreads.Add(new Thread(new ThreadStart(dealerList[i].DealerFunc)));
            //                dealerThreads[i].Start();
            //            }
        }
    }
}
