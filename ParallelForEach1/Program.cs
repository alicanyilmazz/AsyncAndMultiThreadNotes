using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelForEach1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // FOREACH MULTITHREAD SAMPLE

            Stopwatch sw = Stopwatch.StartNew();

            sw.Start();

            string picturesPath = @"C:\Users\Alican\Desktop\Pictures"; // Masaüstümüzde 20 tane kadar resim içeren bir dosyamız var oda dosyamızın path ' dir.

            var files = Directory.GetFiles(picturesPath); // Dosya içerisindeki herbir resmin dosya yolunu files string array ' ine koyduk.

            Parallel.ForEach(files, (item) =>   // item herbir string dizindeki eleman item değişkeni içerisine yerleştirilir.
            {
                Console.WriteLine("Thread no: " + Thread.CurrentThread.ManagedThreadId);
                Image image = new Bitmap(item);
                var thumbnail = image.GetThumbnailImage(50, 50, () => false, IntPtr.Zero);
                thumbnail.Save(Path.Combine(picturesPath,"thumbnail",Path.GetFileName(item)));
            });

            sw.Stop();

            Console.WriteLine("Process ended " + sw.ElapsedMilliseconds);

            sw.Reset();

            // AYNI İŞLEMİ NORMAL FOREACH İLE DE AŞAĞIDA DENİYORUZ.

            sw.Start();

            files.ToList().ForEach(x =>
            {
                Console.WriteLine("Thread no: " + Thread.CurrentThread.ManagedThreadId);
                Image image = new Bitmap(x);
                var thumbnail = image.GetThumbnailImage(50, 50, () => false, IntPtr.Zero);
                thumbnail.Save(Path.Combine(picturesPath, "thumbnail", Path.GetFileName(x)));
            });

            sw.Stop();

            Console.WriteLine("Process ended " + sw.ElapsedMilliseconds);
        }
    }
}
