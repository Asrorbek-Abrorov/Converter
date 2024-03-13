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
        ConverterService converterService = new ConverterService();
        var telegramService = new TelegramService(client, converterService);
        await telegramService.Run();
    }
}
