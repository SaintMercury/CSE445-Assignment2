using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var buffer = new OrderBuffer();
            var rand = new Random();
            var test1 = new Test(buffer, "thread_1", rand);
            var test2 = new Test(buffer, "thread_2", rand);
            var test3 = new Test(buffer, "thread_3", rand);
            var test4 = new Test(buffer, "thread_4", rand);
            var test5 = new Test(buffer, "thread_5", rand);
            var thread1 = new Thread(new ThreadStart(test1.TestThread));
            var thread2 = new Thread(new ThreadStart(test2.TestThread));
            var thread3 = new Thread(new ThreadStart(test3.TestThread));
            var thread4 = new Thread(new ThreadStart(test4.TestThread));
            var thread5 = new Thread(new ThreadStart(test5.TestThread));

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            thread5.Start();
        }
    }
}
