using System.Threading;

namespace Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfCells = 3;
            
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

            
            var buffer = new OrderBuf();
            var plant1 = new Plant(buffer);
            var plant2 = new Plant(buffer);
            var plantThread1 = new Thread(new ThreadStart(plant1.PlantFunc));
            var plantThread2 = new Thread(new ThreadStart(plant2.PlantFunc));

            plantThread1.Start();
            plantThread2.Start();

            var dealer = new Dealer(buffer);
            Plant.PriceCut += dealer.PriceCutHandler;
            Thread[] dealerThreads = new Thread[3];
            for (int i = 0; i < 3; i++) //N= 3here 
            {	// Start N retailer threads
                dealerThreads[i] = new Thread(new ThreadStart(dealer.DealerFunc));
                dealerThreads[i].Name = (i + 1).ToString(); dealerThreads[i].Start();
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
