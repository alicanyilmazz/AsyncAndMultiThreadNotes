// See https://aka.ms/new-console-template for more information

// CPU Bounds
// IO Bounds

// CLR (Common Language Runtime) Thread Pool
// Syncranization Context -> Google Network kartı , beni şu thread şu zaman çalıştırdı , şunun için çalıştırdı bu bilgileri yeni gelen thread e veriyor.

Console.WriteLine("Hello, World!");

var thread = new Thread(() => {
    Console.WriteLine("iş yapıldımı");
});

thread.Start();
thread.Join();
thread.Interrupt();
 if (thread.IsAlive)
    Console.WriteLine("thread is alive");

Console.WriteLine("Hello, World!2");

/*
OUTPUT:
 
Hello, World!
Hello, World!2
is yapıldımı
 
 */


//-------------------------------------------------------------------------------------------------------------------------------------------------------------


// Yukarıdaki TPL(Task Parallel Library) çıkmadan önceki yapı idi.
// Sonrasında ise Task yapıdısı geldi enson.

// Task objesi ile aslında şöyle bir durum oluşuyor  

var t = new Task(() =>
{
    Console.WriteLine("Task is runned.");
    Task.Delay(1000);
});

t.Start(); // Başlatabilirsiniz.

Console.WriteLine("Arada işlemler var");

t.ContinueWith(t => {
   Console.WriteLine("bu işlem ile devam et bitince");
});

/*
 
OUTPUT:

Arada islemler var
Task is runned.
bu islem ile devam et bitince
 
 
 */

Console.ReadLine();