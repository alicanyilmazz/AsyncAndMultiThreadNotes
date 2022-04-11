// See https://aka.ms/new-console-template for more information


async Task<string> GetGoogleSourceCodeAsync()
{
    Console.WriteLine("GetGoogleSourceCodeAsync başladı.");
    var httpClient = new HttpClient();
    return await httpClient.GetStringAsync("http://www.google.com");
}

async Task<string> CallGoogleSourceCodeAsync()
{
    Console.WriteLine("CallGoogleSourceCodeAsync başladı.");
    var data = GetGoogleSourceCodeAsync();
    Console.WriteLine("CallGoogleSourceCodeAsync da arada işlemler yapıyorum.");
    var result = await data;
    Console.WriteLine("CallGoogleSourceCodeAsync Bitti.");
    return result;
}

void Main()
{
    Console.WriteLine("Main işlemler başladı");
    var data = CallGoogleSourceCodeAsync();
    Console.WriteLine("Main da arada işlemler yapıyorum.");
    var result = data.Result;
    Console.WriteLine("Main gelen datanın uzunlugu :" + result.Length);
    Console.WriteLine("Main işlemler bitti");
    Console.ReadLine();
}

Main();

/*
 
OUTPUT : 

Main islemler basladı
CallGoogleSourceCodeAsync basladı.
GetGoogleSourceCodeAsync basladı.
CallGoogleSourceCodeAsync da arada islemler yapıyorum.
Main da arada islemler yapıyorum.
CallGoogleSourceCodeAsync Bitti.
Main gelen datanın uzunlugu :49896
Main islemler bitti
 
 */