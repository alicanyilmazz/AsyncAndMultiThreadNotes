# AsyncAndMultiThreadNotes

void --> Task
string --> Task<string>



  
  ## Asycn İşlemlerin Tanımlanması

SmartyPants converts ASCII punctuation characters into "smart" typographic punctuation HTML entities. For example:

|                |Async                          |Sync                         |
|----------------|-------------------------------|-----------------------------|
|Return Type     |`Task`                         |`'void'`                     |
|Return Type          |`Task<string>`                 |`'string'`              |



```csharp
private string ReadFile(){
  string data = string.empty;
  using(StreamReader s = new StreamReader("filename.txt")){
    Thread.Sleep(5000);
    data = s.ReadToEnd();
  }
}
```
