using System;
using System.Threading.Tasks;

namespace ValueTaskSample
{
    internal class Program
    {
        private static int cacheData { get; set; } = 150;
        static async void Main(string[] args)
        {
            await GetData();
            await GetDataWith(); // Hızlı verinin geleceğini bildiğimiz yerlerde ValueTask i kullanabiliriz. Memory deki bir veriyi getirmek gibi.
            // Ayrıca ValueTask , Task Class ının sahip oldugu methodların aynısına yine sahiptir.
        }

        public static Task<int> GetData()
        {
            return Task.FromResult(cacheData);
        }

        public static ValueTask<int> GetDataWith()
        {
            return new ValueTask<int>(cacheData);
        }
    }
}
