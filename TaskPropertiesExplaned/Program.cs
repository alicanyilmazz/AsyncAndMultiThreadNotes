using System;
using System.Threading.Tasks;

namespace TaskPropertiesExplaned
{
    internal class Program
    {
        static async void Main(string[] args)
        {
            Task myTask = Task.Run(() =>
            {
                Console.WriteLine("myTask runned.");
            });

            await myTask; // Buraya DebugPrint koy // Sonrasında Watch özelliğine myTask yaz // IsCancelled , Exception , IsCompleted , IsCompletedSuccessfully , IFaulted gibi tüm bu sınıfa ait özellikler i görebilirsin.

            // myTask.IsCompleted // gibi kod içerisinde de görebilirsin istersen.
            // myTask.IsCanceled  // gibi kod içerisinde de görebilirsin istersen.

            Console.WriteLine("myTask ended.");
        }
    }
}

// IsCompletedSuccessfully --> Bizim Task imizin herhangi bir exception fırlatmadan başarı ile sonuclandıgının göstergesidir.
// IsCompleted --> Task bir exception fırlatsada fırlatmasada işlemin bittiğinin göstergesidir.
// IsFaulted --> Task imizin bir exception fırlatırsa true fırlatmazsa false olacaktır.

// Mesela Debug yaparken F10 yapıp await myTask; geçince  IsCompletedSuccessfully = true olacak , IsCompleted = true olacak , Status = RanToCompletion olacak zira await satırını geçtik çalışma bitti. cw kısmındayız debug da.

