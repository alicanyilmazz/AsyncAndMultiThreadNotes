using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TaskInstanceSample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(GetData());
        }

        public static string GetData()
        {
            var task = new HttpClient().GetStringAsync("http://www.google.com");

            return task.Result; // Burada thread bloklanır eğer UI Thread ise application donar.
        }

        private async void Main()
        {
            var task = new HttpClient().GetStringAsync("https://www.google.com").ContinueWith((data) =>
            {
                var d = data.Result; // Burda da Result property si Thread imizi herhangi bir şekilde bloklamaz çünkü ContinueWith data nın geldiğinde çalıştırılacaktır.
            });
            
        }
    }
}
// Ben bunun içerisinde async method kullanacağım async method kullanabilmek içinde await keyword ' üne ihtiyacım var 
// Ama benim GetData Methodum sync bir method geriye Task<string> dönmüyor string dönüyor geriye.
// İşte burda Result propertisi devreye giriyor. Sonucu alırken Thread i blokladığından dolayı GetData() methodu geriye string dönmesi sorun olmuyor öyle olmasaydı Task<string> dönmesini isteyecekti.