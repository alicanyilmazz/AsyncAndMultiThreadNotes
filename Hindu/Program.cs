// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");



async void Process1()
{
    await Task.Delay(1000);
    Console.WriteLine("File1 download.");
}

async void Process2()
{
    await Task.Delay(1000);
    Console.WriteLine("File2 download.");
}

void Main()
{
    Process1();
    Process2();
    Console.WriteLine("Main method completed.");

    Task.Factory.StartNew(Process1);
    Task.Factory.StartNew(Process2);

    Console.ReadLine();
}

Main();