using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TaskFlow
{
    internal class Program
    {
        static async void Main(string[] args)
        {
            Console.WriteLine("1. Adım");

            var myTask = GetContent();

            Console.WriteLine("2. Adım");

            var content = await myTask;

            Console.WriteLine("3. Adım"+content.Length);
        }

        public static async Task<string> GetContent()
        {
           var content = await new HttpClient().GetStringAsync("https://www.google.com");
           
            return content;
        }
    }
}
