using System;
using System.Linq;
using System.Threading.Tasks;

namespace RaceConditionSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int val = 0;

            Parallel.ForEach(Enumerable.Range(1, 100000).ToList(), (x) =>
            {
                val = x;  
            });

            Console.WriteLine(val);
        }
    }
}
