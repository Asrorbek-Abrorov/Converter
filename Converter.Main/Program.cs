using Converter;
using Converter.Data.AppDbContexts;
using Converter.Domain.Configuration;
using Converter.TelegramBot.TelegramService;
using Telegram.Bot;

namespace Converter.Main;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        TelegramBotClient client = new TelegramBotClient(Constants.TelegramBotToken);
        var telegramService = new TelegramService(client);
        telegramService.Run();
    }
}
