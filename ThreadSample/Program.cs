using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadSample
{
    internal class Program
    {
        public class Status
        {
            public int threadId { get; set; }
            public DateTime date { get; set; }
        }
        async static Task Main(string[] args)
        {
            var myTask = Task.Factory.StartNew((obj) =>
            {
                Console.WriteLine("My Task Runned.");
                var status = obj as Status; // Aynı obje buraya da geldiğinden "date" property si dolu olarak gelecektir. // Status geleceğini 2. parametreden geldiği için biliyorum ve Status a cast ediyoruz.
                status.threadId = Thread.CurrentThread.ManagedThreadId;

            },new Status() { date = DateTime.Now});

            await myTask;

            Status status = myTask.AsyncState as Status;
            
            Console.WriteLine(status.date);
            Console.WriteLine(status.threadId);
        }
    }
}

// as keyword ü --> obj as Status; --> obj i Status a çevirebilirse çevirir çeviremezse geriye null döner.
// is keyword ü --> obj is Status; --> obj i Status a çevirebilirse true çeviremezse false döner.