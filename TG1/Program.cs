using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TG1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var t = GetGoogleSourceCode().ContinueWith(task =>
            {
                Console.WriteLine(task.Result+"calist");
            });
            Console.WriteLine("Process Finished.");
        }

        static Task<string> GetGoogleSourceCode()
        {
            var httpClient = new HttpClient();
            var t = Task.Run(() =>
            {
                Console.WriteLine("girdi");
                return httpClient.GetStringAsync("http://www.google.com").Result;
            });

            return t;
        }
    }
}

