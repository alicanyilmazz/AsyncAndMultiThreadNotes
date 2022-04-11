// See https://aka.ms/new-console-template for more information


Task<string> GetGoogleSourceCode()
{
    var httpClient = new HttpClient();
    var t = Task.Run(() =>
    {
        return httpClient.GetStringAsync("http://www.google.com").Result;
    });

    return t;
}

void Main()
{
    var t = GetGoogleSourceCode().ContinueWith(task =>
    {
        Console.WriteLine(task.Result.Length);
    });
    Console.WriteLine("Process Finished.");
    Console.ReadLine();
}



Main();