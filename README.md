# AsyncAndMultiThreadNotes

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

### Olayı bir tik ilerletelim ve Method Çağırımında kullandığımız methoda eklemeler yapalım.

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



```
