using System;
using System.IO;
using System.Threading.Tasks;

namespace ThreadSamplesContinue
{
    internal class Program
    {
        public static string CacheData { get; set; }
        static async void Main(string[] args)
        {
            CacheData = await GetDataAsync();
        }

        public static Task<string> GetDataAsync()
        {
            if (String.IsNullOrEmpty(CacheData))
            {
                return File.ReadAllTextAsync("sample.txt");
            }
            else
            {
                return Task.FromResult<string>(CacheData);
            } 
        }
    }
}



