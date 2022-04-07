# AsyncAndMultiThreadNotes

`Asenkron Programlama; asenkron methodları hangi thread den çağırdıysak o thread in bloklanmaması üzerine kuruludur.`

## Asycn İşlemlerin Tanımlanması

|                |Async                          |Sync                         |
|----------------|-------------------------------|-----------------------------|
|Return Type     |`Task`                         |`'void'`                     |
|Return Type     |`Task<string>`                 |`'string'`                   |

### sync dosyadan veri okuma işlemi

```csharp
private string ReadFile(){
  string data = string.empty;
  using(StreamReader s = new StreamReader("filename.txt")){
    Thread.Sleep(5000);
    data = s.ReadToEnd();
  }
}
```

### async dosyadan veri okuma işlemi
Eğerki async bir method un içerisinde başka bir asycn bir method u çağıracaksanız , async ve await ikilisini kullanmanız gerekiyor. Eğer çağırmayacaksak direkt döneceğiz. 
private async derken compiler a bu method içerisinde bir async method çağırımı yapacağımı bildiriyorum.

```csharp
private async Task<string> ReadFileAsync(){
  string data = string.empty;
  using(StreamReader s = new StreamReader("filename.txt")){
    Task<string> mytask = s.ReadToEndAsync(); // IO Driver
    await Task.Delay(5000); // Thread.Sleep() den farkı ana thread i bloklamaz sanki 5 sn lik bir işlem yapmışız gibi davranır. Task döndüğünden dolayı await ile işaretliyorum ki 5 sn boyunca orda beklesin.
    // örnek http client işlemi new HttpClient()
    
    data = await mytask;
  }
}
```

> 1.) Eğer http client işlemimiz 10 sn sürerse mytask in data okuma işlemi ise 5 sn sürerse httpclient işlemi bitine kadar dosya okuma işlemi coktan bitmiş olacak 
bu yüzden httpclient işlemi biter bitmez data = await mytask; işlemi çalıştırılacaktır.

> 2.) Fakat dosya okuma işlemimiz 10 sn sürerken httpClient işlemimiz 3 sn sürer ise o zamanda dosya okuma işlemi bitmediğinden data = await mytask; kısmında durup dosyanın okunmasının bitmesini bekleyecektir.

> Not : Async methodların illa yapacagı işlem için ekstra bir thread kullanmasına gerek yoktur. Dosyadan veri okuma , yazma ->  IO Driver a ve işletim sistemine devrediyor yani benim ana thread im yine boşa çıkıyor çünkü okuma işlemini devretti IO Driver ' a veya `httpClient().GetAsync("url")` -> de de ekstra bir thread e ihtiyac yok Ağ kartı bu işi halleder.
> Not 2 : Her async method thread kullanmaz dedik ama thread kullanamaz demedik thread kullandıgı , kullanabildiği durumlar da vardır.

### Method un Çağırımı

```csharp
private string BtnRealFile_Click(object sender , EventArgs e){
  string data = ReadFile();
  richTextBox1.Text = data;
}
```
> Yukarıdaki ReadFile() sync methodunun cagırımı.

```csharp
private async string BtnRealFile_Click(object sender , EventArgs e){
  string data = await ReadFileAsync();
  richTextBox1.Text = data;
}
```

> Yukarıdaki ReadFileAsync() asycn methodunun cagırımı.

> Async `ReadFileAsync()` methodunu click methodunundan çağırırken async keyword ünü eklediğimize ve yine `ReadFileAsync()` önüne await attığımıza dikkat edelim. Peki await i daha sonra bir yerde atamazmıydık bu senaryoda atamadık zira hemen aldındaki `richTextBox1.Text = data;` satırında bu veriye ihtiyacımız var eğer ihtiyacımız olmayan baska işlemlerde olsaydı zaman alabilecek onlardan sonra await ile alır veriyi textboxt a basardık.

##### Olayı bir tik ilerletelim ve Method Çağırımında kullandığımız methoda eklemeler yapalım.

```csharp
private async string BtnRealFile_Click(object sender , EventArgs e){
  string data = string.Empty;
  Task<String> readFile = ReadFileAsync();
  richTextBox2.Text = await new HttpClient().GetStringAsync("url");
  data = await readFile;
  richTextBox1.Text = data;
}
```
> Burada öenmli bir şeyi tekrar edelim , okuma işlemi uzun sürerse yukarıda  `data = await readFile;` kısmında bekleyecek fakat bu bizim main thread imizi bloklamıyor.

> Main thread bloklanmaz okuma işlemi ilgili IO Driver dan veri gelene kadar orda beklenilir Main thread bu iş için bloklanmaz.

> !NOT : Şimdi de async - await kullanmadan ReadFile methodunu yazalım.
```csharp
private Task<string> ReadFileAsync2(){
   StreamReader s = new StreamReader("filename.txt"))
    return s.ReadToEndAsync();
}
```
> Using Blogu kullanmadık cunku işlem async olarak ilerliyor be await kullanmadıgımızdan dosya okuma işlemi bitmeden StreamReader dispose oluyor ondan hata almamak için using i kaldırdık.

> Dikkat ! : Async methodu  `ReadToEndAsync();` methodu await ile kullanmadığımız direkt döndüğümüz için async - await ikilisine ihtiyacımız yok. 

> Not: async - await ikilisini async bir method çağırdıktan sonra geriye direkt döneceksem kullanmama gerek yok ama o işlem bitene kadar arada başka işlemler daha yaptıracaksam evet o zaman kullanmamız mantıklı olur.

## TASK METHODLARI

### ContinueWith Kullanımı
> Task sınıfından sonra ContinueWith method unu kullanırsanız , Task in içersindeki işlem tamamlandıktan sonra bu ContinueWith içerisindeki kodlarınız çalışır.

```csharp
namespace TaskSamples
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            Console.WriteLine("Hello World!"); // 1 burası çalışır ilk olarak.

            var mytask = new HttpClient().GetStringAsync("https://www.google.com").ContinueWith((data) =>
            {
                Console.WriteLine("The length of the data" + data.Result.Length); // 4 burası çalışır.
            });

            Console.WriteLine("arada yapılacak işler"); // 2 burası çalışır çünkü await atmadık üste

            await mytask; // 3 burası çalışır 
        }
    }
}
```
> Peki ContinueWith i iptal edip aynı kodu async - await kullanarak yapsaydık o zamanda kodumuz aşağıdaki gibi olurdu.

```csharp
namespace TaskSamples
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var mytask = new HttpClient().GetStringAsync("https://www.google.com");

            Console.WriteLine("arada yapılacak işler"); 

            var data = await mytask;

            Console.WriteLine("The length of the data" + data.Length); 

        }
    }
}

```
> Şimdide ContinueWith i kodumuzu uzatmadan baska biryerde olusturdugumuz method u cagırırken nasıl kullanacagız.

```csharp
namespace TaskSamples
{
    internal class Program
    {
        public static void run(Task<string> data)
        {
            Console.WriteLine("The length of the data" + data.Result.Length);
        }
        async static Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var mytask = new HttpClient().GetStringAsync("https://www.google.com").ContinueWith(run);

            Console.WriteLine("arada yapılacak işler"); 

            await mytask;
        }
    }
}


```
### WhenAll Kullanımı
> WhenAll methodu parametre olarak bir Task Array i alır ve Array içerisindeki Task lerin hepsi tamamlanıncaya kadar bekler. Yani WhenAll methodunu kullandığımızda bu satırdan sonraki kod satırına geçildiğinde WhenAll Array inin içerisindeki Task lerin hepsi bitmiş olur. 

```csharp
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

            var content = await Task.WhenAll(taskList.ToArray());

            content.ToList().ForEach(x =>
            {
                Console.WriteLine($"{x.Site} length : {x.Len}");
            });
        }
        public static async Task<Content> GetContentAsync(string url)
        {
            Content c = new Content();
            var data = await new HttpClient().GetStringAsync(url);

            c.Site = url;
            c.Len = data.Length;
            Console.WriteLine("GetContentAsync thread " + Thread.CurrentThread.ManagedThreadId);
            return c;
        }
    }
}

```
> OUTPUT :
```comment
 Main Thread: 1
GetContentAsync thread 11
GetContentAsync thread 11
GetContentAsync thread 12
GetContentAsync thread 12
GetContentAsync thread 12
https://www.google.com length : 49747
https://www.microsoft.com length : 181526
https://www.amazon.com length : 434868
https://www.netflix.com length : 431898
https://www.apple.com length : 68403
```
> Dikkat : Gördüğünüz async işlemlerde illa thread kullanılacak diye bir şart yok dedik ama kullanabilir de dedik burda thread id lerinden de gördünüz gibi async bu işlemde 1 -> Main thread, 11 ve 12 id li thread lerde kullanılmıştır.

> Aşağıdaki gibi de kullanabilirsiniz.

```csharp

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

            var content = Task.WhenAll(taskList.ToArray());

            Console.WriteLine("WhenAll method undan sonra başka işlemler yapıldı.");

            var data = await content;

            data.ToList().ForEach(x =>
            {
                Console.WriteLine($"{x.Site} length : {x.Len}");
            });
        }
        public static async Task<Content> GetContentAsync(string url)
        {
            Content c = new Content();
            var data = await new HttpClient().GetStringAsync(url);

            c.Site = url;
            c.Len = data.Length;
            Console.WriteLine("GetContentAsync thread " + Thread.CurrentThread.ManagedThreadId);
            return c;
        }
    }
}

```
### WhenAny Kullanımı
> WhenAll methodu parametre olarak bir Task Array i alır ve Array içerisindeki ilk biten Task i bize döndürür.

> Diyelim ki 4 tane Task imiz var bunlardan hangisi biterse ilk olarak onu bize Task olarak döndürür.

```csharp
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

            var firstData = await Task.WhenAny(taskList);
            Console.WriteLine($"{firstData.Result.Site} - {firstData.Result.Len}");

        }
        public static async Task<Content> GetContentAsync(string url)
        {
            Content c = new Content();
            var data = await new HttpClient().GetStringAsync(url);

            c.Site = url;
            c.Len = data.Length;
            Console.WriteLine("GetContentAsync thread " + Thread.CurrentThread.ManagedThreadId);
            return c;
        }
    }
}

```

> OUTPUT :
```comment
 Main Thread: 1
GetContentAsync thread 7
https://www.apple.com - 68403 
```
> Aralarında en hızlı biten olan Task işlmei -> apple.com bize döndü.

### WaitAll Kullanımı
> WhenAll methodu parametre olarak bir Task Array i alır ve Array içerisindeki taskler tamamlanana kadar beklemektedir.

> Yalnız WhenAll methodundan farkı şudur WhenAll methodu bizim Main Thread(UI Thread) mizi bloklamıyorken WaitAll methodu bloklama işlemi gerçekleştirmektedir.

> Yani WaitAll dan sonra bizim Main Thread(UI Thread) imiz bloklanıyor ve alt satıra gecmiyor veya WinForm kullanıyorsanız kullanıcı etkileşimlerine cevap vermeyecektir.

> WhenAll dan İkinci önemli farkı ise WaitAll methodu parametre olarak milisaniye cinsinden veri alıyor diyelim ki 3000 milisaniye verdiğiniz zaman WaitAll artık geriye true yada false dönüyor.

> Yani vermiş oldugumuz süre içerisinde WaitAll(task1,task2,task3,task4) içerisindeki görevler tamamlanırsa true tamamlanamazsa false döner.

> Eğer WaitAll ' a milisaniye cinsinden parametresini vermezseniz geriye hiçbirsey dönmez dönüş tipi void olur.

```csharp
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

            Console.WriteLine("WaitAll methodundan önce");
            Task.WaitAll(taskList.ToArray());
            Console.WriteLine("WaitAll methodundan sonra");
            
            Console.WriteLine($"{taskList.First().Result.Site} - {taskList.First().Result.Len}");

        }
        public static async Task<Content> GetContentAsync(string url)
        {
            Content c = new Content();
            var data = await new HttpClient().GetStringAsync(url);

            c.Site = url;
            c.Len = data.Length;
            Console.WriteLine("GetContentAsync thread " + Thread.CurrentThread.ManagedThreadId);
            return c;
        }
    }
}

```

> OUTPUT :
```comment
 Main Thread: 1
WaitAll methodundan önce
GetContentAsync thread 7
GetContentAsync thread 7
GetContentAsync thread 7
GetContentAsync thread 7
GetContentAsync thread 7
WaitAll methodundan sonra
https://www.google.com - 49788
```

### WaitAny Kullanımı
> WaitAny methodu da çağırıldığı thread i (Main Thread üzerinden çağırdıysan Main thread i UI Thread üzerinden çağırdıysanız UI Thread i bloklar.) bloklar.

> WaitAny methodu Task Array i almaktadır. Array olarak aldığı Task lerden herhangi biri tamamlandığı zaman bir integer değer döner dönmüş olduğu integer değer ise tamamlanan task in index numarasıdır.

> Bu index numarası üzerinden tamamlanan task i alabilirsiniz.

```csharp

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
     
            var firstTaskIndex = Task.WaitAny(taskList.ToArray()); // ilk tamamlanan task in index bilgisini dönecek.

            Console.WriteLine($"{taskList[firstTaskIndex].Result.Site} - {taskList[firstTaskIndex].Result.Len}");

        }
        public static async Task<Content> GetContentAsync(string url)
        {
            Content c = new Content();
            var data = await new HttpClient().GetStringAsync(url);

            c.Site = url;
            c.Len = data.Length;
            Console.WriteLine("GetContentAsync thread " + Thread.CurrentThread.ManagedThreadId);
            return c;
        }
    }
}

```

> OUTPUT :
```comment
Main Thread: 1
WaitAny methodundan önce
GetContentAsync thread 7
https://www.apple.com - 68403
```

### Task.Delay() Kullanımı
> Asenkron bir şekilde gecikme gerçekleştirir. Ama bu geciktirmeyi gerçekleştirirkende güncel thread i bloklamaz. Birde Thread imizin Sleep methodu vardı.

> Sleep methodu Main or UI Thread imizi bloklarken Delay methodu herhangi bir sekilde UI veya Main Thread i bloklamaz.


```csharp

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
            // Biz 5 kere istek yapacağız ve her biri için 5 sn bekleyecek fakat suna dikkat
            // Async olarak işlem yapıyoruz ve Delay Main thread i bloklamıyor
            // O yuzden 
            // 5sn
            // 5sn
            // 5sn  --> Bunların hepsi art arda fonksiyona girecek bu sayede milisaniyeler aralarında fark olacagından neredeyse 5 i için 5*5 = 20 saniye değil
            // 5sn  --> toplamda yaklaşık olarak 5 sn beklemiş olacagız eğer asenkron değilde senkron olarak bu işlemi yapsakdık 20 sn olurdu burdaki farka dikkat edelim.
            // 5sn

            // Thread.Sleep(); -> kullansaydık mevcut thread i 5 sn boyunca bloklayacaktı.
         

            c.Site = url;
            c.Len = data.Length;
            Console.WriteLine("GetContentAsync thread " + Thread.CurrentThread.ManagedThreadId);
            return c;
        }
    }
}

```

> OUTPUT :
```comment
Main Thread: 1
WaitAny methodundan önce
GetContentAsync thread 12
GetContentAsync thread 12
GetContentAsync thread 12
GetContentAsync thread 12
GetContentAsync thread 12
https://www.google.com
https://www.microsoft.com
https://www.amazon.com
https://www.netflix.com
https://www.apple.com
```
>  Dikkat ederseniz methodumuzda 12 nolu thread görev alıyor. Tek bir thread bloklanmadan asenkron bir şekilde 4 kez methodu çalıştırmış.Duruma göre birden fazla thread de metoduza girebilirdi.

>  Bu methoda arka arkaya milisaniyeler içerisinde girdiğinden dolayı(bloklanmadığı) için bu sonucu aldık.

### Run() Methodu Kullanımı
> Daha öncede demiştik async methodlar illaki thread kullanmaz thread kullanacağı durumlarda olur kullanmayacağı durumlarda olur. Bizim ilgilendiğimiz nokta async methodumuzu çağırdığımızda o anki thread imizin bloklanmamasıdır.

> Asenkron programlamanın mantığında bu var.

> Peki biz bazı kodlarımızı ayrı bir thread üzerinde çalıştırmak istersek ne yapmamız gerekiyor , işte burada Task sınıfımızın Run() methodu devreye giriyor.

> Run() methodu içerisinde yazmış olduğumuz kodlar tamamen ayrı bir thread üzerinde çalışır. 

> Yani bizzat biz bu kodların ayrı bir thread üzerinde çalışması gerektiğini bildirmiş oluyoruz.

> Genelde bizim işlemleri asenkron olarak çalıştırmamız mantıklıdır herşey için ayrı thread açılmaz. 

> Mesela yogun matematiksel işlemlerin olduğu bir methodumuz var trigonometri , integral vb içeren işte bu gibi durumlar da  bunu ayrı thread üzerinde çalıştırmak mantıklıdır.

> Aşağıda iki tane progressbar yüklenmesi kodunun senkron olarak çalışması verilmiştir.
> Tabiki 2 progresBar yüklenene kadar UI Thread imiz bloklanacağından kullanıcı hiçbir işlem yapmayacaktır.

```csharp
 private void btnStart_Click(object sender , EventArgs e){
      Go(progressBar1);
      Go(progressBar2);
 }
 
 public void Go(ProgressBar pb){
    Enumerable.Range(1,100).ToList().ForEach(x =>
    {
        Thread.Sleep(100);
        pb.Value = x;
    });
 }

```

> Run() methodu içerisine yazdığımızdan dolayı artık Go methodunu 2 kere cagırdık bunlarda ayrı ayrı threadlerda çalıştırılacaktır ve asenkron olarak.

> Thread.Sleep(100); amacı senkronda kod o kadar hızlı çalışıyorki biz uı da 2 progress bar sanki aynı anda yükeniyor gibi görüyoruz işte bu dur kalk şeklinde yüklenme görüntüsünü sağlamak için kullanılıyor tabiki bunu Main Thread i bloklayarak yapıyor.

> Fakat şuna dikkat edelim Run() methodu içerisine aldığımızda 2 ayrı thread oluşuyor ve bu ayrı thread Thread.Sleep(100); ayrı ayrı bloklanıyor ama asenkron olarak çalışıyorlar ve progressBarların 2 side beraber dolmaya başladığını olarak görebiliriz.

```csharp
 private async void btnStart_Click(object sender , EventArgs e){
      var aTask = Go(progressBar1);
      var bTask = Go(progressBar2);
      await Task.WhenAll(aTask,bTask);
 }
 
 public async Task Go(ProgressBar pb){
    await Task.Run(() =>
    {
         Enumerable.Range(1,100).ToList().ForEach(x =>
        {
             Thread.Sleep(100);
             pb.Invoke((MethodInvoker) delegate { pb.Value = x; }); // pb.Value = x; kaldırdık cunku winForm UI elementine UI Thread dışında başka bir thread üzerinden erişmemize izin vermiyor yani olay WinForm ile alakalı Run() methodu ile alakalı değil.
        });
    });
 }

```
### StartNew() Methodu Kullanımı

> StartNew() methodu da Run() methodu da yazmış olduğumuz kodları ayrı bir thread üzerinde çalıştırır.

> Peki Run() methodundan farkı nedir? Run() methoduna Task imizi oluştururken bir obje geçemiyorken StartNew() methoduna bir obje geçebiliyoruz.

> Task işlemi bittiğinde StartNew() methoduna geçmiş olduğumuz objeyi alabiliyoruz. Bu obje dediğimiz herhangi bir value type veya reference type da olabilir.

> Tipi obje kısacası yani c# da object tüm sınıfların en üst sınıfı oldugundan dolayı her tipe cast işlemi gerçekleştirebilirsiniz.

> Kısaca bir Task iniz çalışırken bir obje göndermek istiyorsanız ve arkasından da bu çalışma bittikten sonra bu objeyi elde etmek istiyorsanız kullanabileceğiniz bir method'dur.

```csharp

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
// is keyword ü --> obj is Status; --> obj i Status a çevirebilirse true çeviremezse false döner

```

### FromResult() Methodu Kullanımı

> FromResult() methodu da parametre olarak bir obje almaktadır. Almış olduğu bu obje değerini geriye bir Task nesne örneği ile beraber dönmektedir.

> Yani bir method dan geriye daha önce almış oldugunuz statik bir datayı dönmek istiyorsanız FromResult() methodunu kullanabilirsiniz.

> Genelde biz bu methodu cache lenmiş datayı dönmek için kullanırız.

```csharp

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

```

> Not: Aşağıdaki gibi kullanımlar oldugunu da hatırlatalım.

```csharp

    Task.FromResult("alican"); şeklinde kullanılabileceği gibi diğer overload edilen constructure larına baktığımız zaman generic tip güvenli halleride vardır.
    Task.FromResult<string>("alican") gibi

    Aynı şekilde, bu durum Task.Run() methodu içinde geçerlidir.
    
    Task.Run(() => {
        return "alican";
    });

    şeklinde kullanılabileceğimiz gibi , 
    
    Task.Run<string>(() => {
        return "alican";
    });
    
    şeklinde tip güvenli hale de getirebiliriz.

```

### CancellationToken Kullanımı

> Bir asenkron işlem başlatırken bazen bu işlemler çok uzun sürebilir. Ve bu uzun sürme anında bazen bu başlatmış olduğumuz asenkron işlemi iptal etmek isteyebiliriz.

> Mesela 30 dk sürecek bir asenkron işlem başlattınız , bu işlemi herhangi bir zaman diliminde iptal edebilirsiniz mesela 10. dakikasında iptal edebilirsiniz.

> İşte bu iptal işleminde CancellationToken adlı bir yapıdan faydalanıyoruz.

> Bir asenkron işlem başlatırken buna bir token veriyoruz ve daha sonra bu token ' ı iptal ettiğimiz zaman başlatmış olduğumuz asenkron operasyonda iptal olmuş oluyor.

> WindowsFrom üzerinde inceleyelim öncelikle sonrasında API tarafında da inceleyeceğiz.

#### WinForm Üzerinde İnceleme

```csharp

// 2 Buttonda erişebilsin diye Globalde tanımladık.
CancellationTokenSource ct = new CancellationTokenSource(); // CancellationToken oluşturmak için CancellationTokenSource class ından bir nesne oluşturuyorum ki bu class üzerinden bir CancellationToken alacağım. 

private async void button1_Click(object sender , EventArgs e) // Veri getirme butonu
{
    try
    {
        Task<HttpResponseMessage> myTask;

        myTask = new HttpClient().GetAsync().GetAsync("https://localhost:44366/api/home", ct.Token);

        await myTask;

        var content = myTask.Result.Content.ReadAsStringAsync(); // GetStringAsync de doğrudan string dönüyordu ama GetAsync den Content.ReadAsStringAsync ile veriyi okuyoruz.

        richTextBox1.Text = content;
    }
    catch (TaskCanceledException ex)
    {
        MessageBox.Show(ex.Message);
    }

    // myTask.isCancelled --> Cancel olup olmadığını
    // myTask.IsCompleted --> Veya tamamlanıp tamamlanmadığını kontrol edebiliriz.
}

private async void button2_Click(object sender, EventArgs e) // İptal Butonu
{
    ct.Cancel(); // Cancel() işlemi ile birlikte TasCanceledException fırlatır her zaman o zaman bu exception ı yakalarsam durdurma işlemini de yakalarım.
}

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetContentAsync()
    {
        Thread.Sleep(5000);
        var myTask = new HttpClient().GetStringAsync("https://www.google.com");
        var data = await myTask;
        return ok(data);
    }

}

// Bir asenkron methodun token alabilmesi için mutlaka bir constructure ında bir overload ' un olması lazım.
// Her asenkron methodun token parametresi olmayabilir.
// Örneğin ,
// new HttpClient().GetStringAsync() -> methoduna bakarsak GetStringAsync() methodunun 2 overload ı oldugunu göreceksiniz ve parametre olarak token alan bir constructure ' ı yok.
// Aynı sınıfın şimdide GetAsync() -> methoduna bakarsak GetStringAsync methodunun 4. overload ın da parametre olarak CancellationToken alan bir constructure ' ını görebiliriz.
// new HttpClient().GetAsync() --> 4. constructure ' ı parametre olarak CancellationToken alıyor demekki ben bu başlayan işlemi bu token üzerinden iptal edebilirim.

// O zaman diyebiliriz ki herbir asenkron methodun illaki CancellationToken 'ı olacak diye bir durum yok overload larına bakıp incelememiz gerekiyor.

```
#### Web Tarafında İnceleme

> Mesela Controller içerisinde bir IActionResult methodunuz var bu methodun içerisindeki async method dun da yaptığı işlem 10dk sürebilir , yani bir request yapıldığında 10 dk sürebilir response işleminin tamamlanması bu uzun işlemlerinizi CancellationToken kullanarak iptal edebilirsiniz.

> Nasıl meydana geliyor bu iptal olayı; kullanıcı bir istek yaptıktan sonra ilgili sekmeyi kapatırsa veya sayfayı refresh işlemi gerçekleştirirse , CancellationToken devreye girecek ve isteği sonlandıracak.

> Eğer CancellationToken kullanmazsak kullanıcı sayfayı kapatırsa veya sayfayı refresh yaparsa o istek arka planda çalıştırılmaya devam edecek ve sizin kaynaklarınızı tüketecek. Bu kaynak tüketimini engellemek için uzun süren işlemlerde CancellationToken kullanılabilir.

> Heryerde kullanmaya gerek yok sadece uzun süren async method larda kullanmak faydalı olacaktır.

```csharp

namespace Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetContentAsync()
        {
            _logger.LogInformation("istek başladı.");
            Thread.Sleep(5000);
            var mytask = new HttpClient().GetStringAsync("http://www.google.com");
            var response = await mytask;
            _logger.LogInformation("istek başladı.");
            return Ok(response);
        }
    }
}


```

> Yerine CancellationToken kullanarak nasıl yapabiliriz onu gösterelim.

```csharp
namespace Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetContentAsync(CancellationToken cancellationToken) // Bu parametreyi geçtiğimiz andan itibaren artık sayfayı işlem tamamlanmadan kapatırsak hemen TaskCanceledException fırlatacaktır. 
        {   // Bu TaskCanceledException ı ınıda aşağıda try catch blogu üzerinden handle edelim.
            try
            {
                _logger.LogInformation("istek başladı.");
                await System.Threading.Tasks.Task.Delay(5000, cancellationToken); // Delay ile async bir gecikmeyi simule ettik. Delay in 4. overload u zaman ve cancellationtoken alıyor bunu kullandık o yuzden.
                
                var mytask = new HttpClient().GetStringAsync("http://www.google.com");
                var response = await mytask;
                _logger.LogInformation("istek bitti.");
                return Ok(response);
            }
            catch (System.Exception e)
            {
                _logger.LogInformation("İstek İptal edildi." + e.Message);
                return BadRequest(e.Message);
            }
        }
    }
}


```
> NOT :  cancellationToken.ThrowIfCancellationRequested(); Eğer Async method kullanmıyorsam ama işlemimiz yine uzun sürüyorsa ThrowIfCancellationRequested ile de manuel olarak TaskCanceledException ı fırlattırabilirsiniz.

#### Maunel olarak token iptali

> Yukarıdaki Task.Delay(1000,cancellationToken); bir asenkron işlemi simule etmek için kullanılmıştı çünkü bütün kodları tamamlama imkanımız yok onu bu işlemi uzun süren async bir business method unun cağrımı gibi düşünebilirsiniz demiştik.

> Aşağıda da senkron olan bir uzun süren bir business methodunun çağırımını  Thread.Sleep(1000); ile simule ettiğimizi düşününüz Enumerable.Range(1, 10).ToList().ForEach(x => kodumun bu methodu 10 kere çağırıp çalıştıracak gibi düşününüz.

> Döngünün 5 veya 6 sırasında sonraki business method çağırımı iptal olursa herhangi bir durumdan dolayı (WinForm da durdur butonu (token.Cancel()) ile bunu yapıyorduk hatırlarsanız , veya browserda sayfaya yapılan isteğin iptal edilmesi sayfanın kapatılması).

> Tabi burda Thread.Sleep(1000); iptal olmayacak ama bunu bir senkron business methodu gibi düşünürseniz ve orda yapılacak bir durumdan dolayı burdaki çağırımın iptal oldugunu düşünürsek işte bu durumda (bu durum döngünün 5. çağırımında gerçekleşsin.) o zaman try dan çıkıp catch e girecek bizde bu olayı handle edebiliyor olacağız.

> senkron bir methodun çağırdığımız zaman kodda da bu şekilde manuel olarak token iptal edilebilir.

> IActionResult methodumuz async , biz bunun içerisinde başka bir async veya sync bir methodu çağırırken cancellationtoken kullanarak işlem iptalini nasıl yaparız onu görmüş olduk.

```csharp

namespace Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetContentAsync(CancellationToken cancellationToken) // Bu parametreyi geçtiğimiz andan itibaren artık sayfayı işlem tamamlanmadan kapatırsak hemen TaskCanceledException fırlatacaktır. 
        {   // Bu TaskCanceledException ı ınıda aşağıda try catch blogu üzerinden handle edelim.
            try
            {
                _logger.LogInformation("istek başladı.");
                Enumerable.Range(1, 10).ToList().ForEach(x => // Bu arkadaş 10 kere dönecek ve her 10 kere döndüğünde 1 sn ye uyuyacak.
                {
                    Thread.Sleep(1000);
                    cancellationToken.ThrowIfCancellationRequested();
                }); 
                _logger.LogInformation("istek bitti.");
                return Ok();
            }
            catch (System.Exception e)
            {
                _logger.LogInformation("İstek İptal edildi." + e.Message);
                return BadRequest(e.Message);
            }
        }
    }
}



```

#### Task Instance

> Şu ana kadar Task sınıfımızın static methodlarını inceledik şimdi ise Task sınıfımızın nesne üzerinden gelen instance üzerinden ulaşağımız methodlarını inceleyeceğiz. 

> Task class ımızın instance üzerinden gelen Result propertisini inceleyeceğiz.

> Bildiğiniz gibi biz async method çağırımları yaptığımız zaman geriye bize yeni bir Task instance ' ı dönüyordu.

> Bu dönen örneğin bir tane Result isminde propertisi var.

> Bu property ile beraber biz dönen datayı alabiliriz.

> Yalnız bu Result propertisinin şöyle bir dezavantajı var bu result propertisi bizim o anki thread imizi bloklayan bir property.

> Peki neden böyle bir property ' ye ihtiyaç duyulmuş ?

> Bunun sebebi siz sync bir method içerisinden async bir method çağıracaksınız ama methodunuzda geriye herhangi bir Task vs dönmüyorsunuz.

> İşte bu gibi durumlarda Result property si üzerinden Thread i bloklayark istediğiniz data yı alabilirsiniz.

#### Task Instance Result Property

> 1. Kullanım `" Senkron method içerisinde çağırılmış olan Asenkron method un sonucunu almak için kullanılır. "`  

##### Console App Üzerinde Gösterelim

```csharp
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
    }
}
// Ben bunun içerisinde async method kullanacağım async method kullanabilmek içinde await keyword ' üne ihtiyacım var 
// Ama benim GetData Methodum sync bir method geriye Task<string> dönmüyor string dönüyor geriye.
// İşte burda Result propertisi devreye giriyor. Sonucu alırken Thread i blokladığından dolayı GetData() methodu geriye string dönmesi sorun olmuyor öyle olmasaydı Task<string> dönmesini isteyecekti.
// ÖNEMLİ NOT : Result property si thread i öncesinde veriyi await ile alsaydık (alındığını garantileseydi) thread i bloklamayacaktı burda bloklamasının nedeni çünkü öncesinde ContinueWith gibi await gibi verinin kesin geldiğine dair bir şey yok dolaysıyla işlemin bittiğini return yapmadan önce garanti altına alması için yukarıda return task.Result; kısmında Result thread i blokluyor. Detaylarına aşağıda değineceğiz.
```

##### WinForm App Üzerinde Gösterelim

```csharp
  private async void BntReadFile_Click(Object sender, EventArgs e)
  {
    var myTask = new HttpClient().GetStringAsync("https://www.hepsiburada.com");
    string data = mytask.Result; // Tabi bu veriyi biryere dönmüyoruz zira BntReadFile_Click bir buton clickleme event i. // Veri data ' ya gelene kadar UI   donacaktır.
  }
```
> 2. Kullanım ` "Verinin geldiği kesin olan durumlarda await veya ContinueWith kullanımı gibi Thread i bloklamadan veriyi almamızı sağlar."`

```csharp
 
 private async void Main()
 {
     var task = new HttpClient().GetStringAsync("https://www.google.com");
     await task;  // Burada Data nın geldiği kesin await keyword ü var çünkü
     var data = task.Result; // Burda Thread Bloklanmaz çünkü yukarıda verinin geldiği kesin.
 }
  
```

```csharp
 
 private async void Main()
 {
     var task = new HttpClient().GetStringAsync("https://www.google.com").ContinueWith((data) =>
     {
         var d = data.Result; // Burda da Result property si Thread imizi herhangi bir şekilde bloklamaz çünkü ContinueWith data nın geldiğinde çalıştırılacaktır.
     });           
 }
  
```
#### Task Instance Properties

> Bu kısımda Task class ımızın property ' lerini inceleyeceğiz , bu property ler gerçekten bize işleme koyduğumuz Task ile ilgili çok değerli bilgiler gönderiyor.

> Mesela bir Task in başarılı bir şekilde tamamlanıp tamamlanmadığı , başka bir method tarafından cancel edilip edilmediği gibi ekstra bilgiler veriyor.

> Veya bu Task içerisindeki işlem bir exception fırlatmışsa bunu bize veriyor.

```csharp
 
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
  
```

```csharp
 
namespace TaskPropertiesExplaned
{
    internal class Program
    {
        static async void Main(string[] args)
        {
            Task myTask = Task.Run(() =>
            {
                throw new ArgumentException("Opps An Error");
            });

            await myTask; // Burada ise Watch kısmında 'myTask' i yazıp bakarsak , Exception : Count = 1 olarak göreceksiniz. IsCompleted : True , IsCompletedSuccessfully : False

            Console.WriteLine("myTask ended.");
        }
    }
}
  
```

### ValueTask

> ValueTask ' ler c# a yeni gelen bir tipimiz , neden böyle bir tip geldi biraz ondan bahsedelim.

> Bizim şu ana kadar inceledeiğimiz

> `Task` `tipimiz bir class ' tır. Yani reference type ' dır.`

> `ValueTask` `tipimiz bir struct ' tır. Yani value type ' dır.`

> Peki neden böyle bir yapıya ihtiyaç duyulmuştur.

> Value type ' lar memory mizin Stack inde tutulur iken , Reference tipler memory mizin Heap alanında tutulur. (nesne kısmı heap - adresi tutan referans stack bölgesinde).

> Heap bölgesinde tutulan dataları GC silebilir ve bu maliyetli bir işlemdir. Belirli aralıklarla referansı olmayan yapıları siler memory i temizler. Bu yüzden Referans tipler maliyetlidir.

> Stack bölgesinde tutulan data lar ise herhangi bir scope dan cıktıgı anda bellekten otomatik bir şekilde düşer.

> Bir method içerisinde int değer kullanmak gibi veya struct gibi bir yapı kullanmak gibi.

> Biz her asycn method dan Task dönmesi heap bölgesinde yeni bir alanın allocate edilmesi anlamına geliyor.

> Eğer yogun bir iş gerektirmeyen bir async method dan data döndüğünüz zaman yeni bir Task dönmek yerine belleğin Stack bölgesinde tutulacak `ValueTask` isminde bir tip dönelim demişler ve bu şekilde memory i daha performanslı kullanalım demişler.

```csharp
 
namespace ValueTaskSample
{
    internal class Program
    {
        private static int cacheData { get; set; } = 150;
        static async void Main(string[] args)
        {
            await GetData();
            await GetDataWith(); // Hızlı verinin geleceğini bildiğimiz yerlerde ValueTask i kullanabiliriz. Memory deki bir veriyi getirmek gibi.
            // Ayrıca ValueTask , Task Class ının sahip oldugu methodların aynısına yine sahiptir.
        }

        public static Task<int> GetData()
        {
            return Task.FromResult(cacheData);
        }

        public static ValueTask<int> GetDataWith()
        {
            return new ValueTask<int>(cacheData);
        }
    }
}

  
```
### Task ' ın Akış Durumu

> Aşağıdaki kod blogu üzerinden inceleyelim.

```csharp
 
 namespace TaskFlow
{
    internal class Program
    {
        static async void Main(string[] args)
        {
            Console.WriteLine("1. Adım");  // 1 Numaralı Adım

            var myTask = GetContent();     // 2Numaralı Adım

            Console.WriteLine("2. Adım");  // 3 Numaralı Adım

            var content = await myTask;    // 4 Numaralı Adım

            Console.WriteLine("3. Adım"+content.Length); 
        }

        public static async Task<string> GetContent() // 5 Numaralı Adım
        {
           var content = await new HttpClient().GetStringAsync("https://www.google.com"); // 6 Numaralı Adım
           
            return content;
        }
    }
}
  
```
> ilk önce 1 numaralı adım çalıştırılır , sonrasında 2 numaralı adım çalıştırılır , ve method çağırlır 5 numaralı adım çalıştırılır , sonrasında 6 numaralı adım
 çalıştırlır fakat 6 numaralı adım da await oldugundan bu uzun sürecek bir işlem ana thread bloklanmaz ve hemen 3 numaralı adım çalıştılır , sonrasında 4 numaralı adım calıstırılır fakat burda da await var işte burda veri gelene kadar thread imiz bekler alt satıra gidilmez peki method içerisinde iken neden `GetStringAsync` dönmesini await olmasına ragmen beklemedi cagırıldıgı yere dönüp kodu işletmeye devam etti çünkü o cağıran üst method eğer  GetContent() da içersinde başka bir methoda gitseydi oda içinde async bir method cagırsaydı ve basında da await olsaydı o zaman oda beklemeden  `GetContent` methoduna dönecekti ,  `Main` içerisindeki await de beklemek zorunda cunku main den sonra kodun çağırılabileceği bir yer yok.

### Task Parallel Library (TPL)

#### Race Condition
> Çok thread li uygulamalarda birden çok thread in paylaşılan bir hafıza alanına aynı anda erişmeye çalışması ile bu durum meydana gelir.

> Multithread programlamada kullanmış oldugunuz thread ler paylaşımlı bir data ya erişmeye çalışıyorsa bu durumu mutlaka göz önünde bulundurmanız gerekiyor yoksa kodalarınız istediğiniz davranışı sergileyemeyebilir.

> Bunu engellemek için genelde paylaşımlı data lock lanır ki ilgili thread in işi bitmeden diğer thread aynı data üzerinde işlem yapamasın.

#### Parallel.ForEach

> Bu method içerisine almış oldugu bir Array i ve Array içerisindeki item ları farklı thread lerde çalıştırarak multithread bir kod yazmamıza imkan verir.

> `ÖNEMLİ : Öncelikle şunu belirtelim Multithread her zaman hızlıdır diye bir durum söz konusu değildir eğer veriniz küçükse yapılacak işlem cok yoğun uzun süren kompleks şeyler içermiyorsa single thread multithread den daha hızlı çalışması olasıdır. Çünkü Multithread de iş parçacıklarının bölünmesi bunları çalıştıracak thread lerin thread pool dan getirilmesi bunların hepsi bir maliyet unsurudur o yuzden yuksek iş yükü olan işlerde kullanmak daha mantıklıdır.` 
 
```csharp
 
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

  
```
##### Parallel.ForEach Paylaşımlı Data Üzerinde Çalışmak

```csharp
 
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

```

`` 
