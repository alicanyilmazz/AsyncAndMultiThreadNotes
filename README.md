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
    Task.Delay(5000); // Thread.Sleep() den farkı ana thread i bloklamaz sanki 5 sn lik bir işlem yapmışız gibi davranır. Task döndüğünden dolayı await ile işaretliyorum ki 5 sn boyunca orda beklesin.
    // örnek http client işlemi new HttpClient()
    
    data = await mytask;
  }
}
```

> 1.) Eğer http client işlemimiz 10 sn sürerse mytask in data okuma işlemi ise 5 sn sürerse httpclient işlemi bitine kadar dosya okuma işlemi coktan bitmiş olacak 
bu yüzden httpclient işlemi biter bitmez data = await mytask; işlemi çalıştırılacaktır.

> 2.) Fakat dosya okuma işlemimiz 10 sn sürerken httpClient işlemimiz 3 sn sürer ise o zamanda dosya okuma işlemi bitmediğinden data = await mytask; kısmında durup dosyanın okunmasının bitmesini bekleyecektir.

> Not : Async methodların illa yapacagı işlem için ekstra bir thread kullanmasına gerek yoktur. Dosyadan veri okuma , yazma ->  IO Driver a ve işletim sistemine devrediyor yani benim ana thread im yine boşa çıkıyor çünkü okuma işlemini devretti IO Driver ' a veya  httpClient().GetAsync("url") -> de de ekstra bir thread e ihtiyac yok Ağ kartı bu işi halleder.
> Not 2 : Her async method thread kullanmaz dedik ama thread kullanamaz demedik thread kullandıgı , kullanabildiği durumlar da vardır.
