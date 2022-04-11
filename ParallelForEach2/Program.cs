using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelForEach2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            long FileByte = 0; // Herbir resim dosyanının boyutunu bu değişkene kaydedeceğiz.

            Stopwatch sw = Stopwatch.StartNew();

            sw.Start();

            string picturesPath = @"C:\Users\Alican\Desktop\Pictures"; // Masaüstümüzde 20 tane kadar resim içeren bir dosyamız var oda dosyamızın path ' dir.

            var files = Directory.GetFiles(picturesPath); // Dosya içerisindeki herbir resmin dosya yolunu files string array ' ine koyduk.

            Parallel.ForEach(files, (item) =>   // item herbir string dizindeki eleman item değişkeni içerisine yerleştirilir.
            {
                Console.WriteLine("Thread no: " + Thread.CurrentThread.ManagedThreadId);
                FileInfo file = new FileInfo(item);

                Interlocked.Add(ref FileByte, file.Length); // Interlock methodu FileByte a aynı anda birden fazla thread in erişmesin engeller o yüzden Race Condition oluşmaz.
                Interlocked.Decrement(ref FileByte);
                Interlocked.Exchange(ref FileByte, file.Length); // Exchange ilede değer i değiştirebiliriz.
            });

            sw.Stop();

            Console.WriteLine("Process ended " + sw.ElapsedMilliseconds);
        }
    }
}
