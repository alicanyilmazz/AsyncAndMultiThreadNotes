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

> Async ReadFileAsync() methodunu click methodunundan çağırırken async keyword ünü eklediğimize ve yine ReadFileAsync() önüne await attığımıza dikkat edelim. Peki await i daha sonra bir yerde atamazmıydık bu senaryoda atamadık zira hemen aldındaki richTextBox1.Text = data; satırında bu veriye ihtiyacımız var eğer ihtiyacımız olmayan baska işlemlerde olsaydı zaman alabilecek onlardan sonra await ile alır veriyi textboxt a basardık.

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
