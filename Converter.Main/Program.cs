using Converter.Data.AppDbContexts;
using Converter.Data.Entities;

namespace Converter.Main;

public class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        using (AppDbContext context = new AppDbContext())
        {
            
        }
    }
}
