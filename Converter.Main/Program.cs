using Converter;
using Converter.Data.AppDbContexts;
using Converter.Domain.Configuration;
using Converter.Services.Services;
using Converter.TelegramBot.TelegramService;
using Telegram.Bot;

namespace Converter.Main;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        TelegramBotClient client = new TelegramBotClient(Constants.TelegramBotToken);
        Converters converters = new Converters();
        var telegramService = new TelegramService(client, converters);
        await telegramService.Run();
    }
}
