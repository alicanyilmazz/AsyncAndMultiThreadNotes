// See https://aka.ms/new-console-template for more information

async Task GetGoogleSourceCode()
{
    Console.WriteLine("Fonksiyon işlemler başladı.");
    var httpClient = new HttpClient();
    var result = await httpClient.GetStringAsync("http://www.google.com");
    Console.WriteLine(result.Length);
    Console.WriteLine("Fonksiyon işlemler bitti.");
}

void Main()
{
    Console.WriteLine("Ana işlemler başladı");
    GetGoogleSourceCode();
    Console.WriteLine("Ana işlemler bitti");
    Console.ReadLine();
}

Main();

/*
 OUTPUT :

 Ana islemler basladı
 Fonksiyon islemler basladı.
 Ana islemler bitti
 49929
 Fonksiyon islemler bitti.
 
 */
