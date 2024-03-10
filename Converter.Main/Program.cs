using Converter.Data.AppDbContexts;

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
