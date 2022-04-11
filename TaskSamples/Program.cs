using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TaskSamples
{
    public class Content
    {
        public string Site { get; set; }
        public int Len { get; set; }
    }

    internal class Program
    {
        async static Task Main(string[] args)
        {
            Console.WriteLine("Main Thread: "+Thread.CurrentThread.ManagedThreadId);
            List<string> urlList = new List<string>()
            {
                "https://www.google.com",
                "https://www.microsoft.com",
                "https://www.amazon.com",
                "https://www.netflix.com",
                "https://www.apple.com"
            };

            List<Task<Content>> taskList = new List<Task<Content>>();

            urlList.ToList().ForEach(x =>
            {
                taskList.Add(GetContentAsync(x));
            });

            Console.WriteLine("WaitAny methodundan önce");
     
            var contents = await Task.WhenAll(taskList);
            contents.ToList().ForEach(x =>
            {
                Console.WriteLine(x.Site);
            });

        }
        public static async Task<Content> GetContentAsync(string url)
        {
            Content c = new Content();
            var data = await new HttpClient().GetStringAsync(url);

            await Task.Delay(5000);         

            c.Site = url;
            c.Len = data.Length;
            Console.WriteLine("GetContentAsync thread " + Thread.CurrentThread.ManagedThreadId);
            return c;
        }
    }
}

