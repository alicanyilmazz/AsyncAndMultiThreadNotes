# AsyncAndMultiThreadNotes

void --> Task
string --> Task<string>

private string ReadFile(){
  string data = string.empty;
  using(StreamReader s = new StreamReader("filename.txt")){
    Thread.Sleep(5000);
    data = s.ReadToEnd();
  }
}

  
  ## Asycn İşlemlerin Tanımlanması

SmartyPants converts ASCII punctuation characters into "smart" typographic punctuation HTML entities. For example:

|             Async                          Sycn             |
|-------------------------------|-----------------------------|
|            Task               |            void             |
|         Task<string>          |           string            |

