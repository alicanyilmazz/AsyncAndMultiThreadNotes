using System;
using System.Threading;

namespace NetOldVersionThreadSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
           Thread mainThread = Thread.CurrentThread;
            mainThread.Name = "Main Thread";

            Thread thread1 = new Thread(() => CountDown("Timer #1"));
            Thread thread2 = new Thread(() => CountUp("Timer #2"));
            thread1.Start();
            thread2.Start();

            Console.WriteLine(mainThread.Name + "is complete!");

            Console.ReadLine();
        }

        public static void CountDown(string name)
        {
            for (int i = 10; i >= 0; i--)
            {
                Console.WriteLine($"{name} "+i+" seconds");
                Thread.Sleep(1000);
            }
            Console.WriteLine("Timer #1 is completed.");
        }

        public static void CountUp(string name)
        {
            for (int i = 0; i <= 10; i++)
            {
                Console.WriteLine($"{name} " + i + " seconds");
                Thread.Sleep(1000);
            }
            Console.WriteLine("Timer #2 is completed.");
        }
    }
}
