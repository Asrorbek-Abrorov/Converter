using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Converter.Domain.Configuration;
using Telegram.Bot.Types.Enums;
using Spectre.Console;

namespace Converter.TelegramBot.TelegramService;

public class TelegramService
{
    TelegramBotClient botClient;
    public TelegramService(TelegramBotClient telegramBotClient)
    {
        this.botClient = telegramBotClient;
    }

    public async Task Run()
    {
        botClient.StartReceiving(Update, Error);
        Console.ReadLine();
    }

    public async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var message = update.Message;

        AnsiConsole.MarkupLine($"[yellow]{message?.From?.FirstName}[/]  |  {message?.Text}");
        Console.WriteLine(update.Message?.Type);

        await SendMessageAsync(message.From.Id, "Hi");

        if (update.Message?.Type == MessageType.Photo)
        {
            await DownloadPhotoAsync(update.Message.Photo[3].FileId);
        }
        else if (update.Message?.Type == MessageType.Document)
        {
            await DownloadFileAsync(update.Message.Document.FileId, update.Message.Document.FileName);
        }
    }

    public async Task DownloadPhotoAsync(string fileId, string filePath = Constants.DOWNLOADS_PATH)
    {
        var file = await botClient.GetFileAsync(fileId);
        filePath += fileId + ".jpeg";

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await botClient.DownloadFileAsync(file.FilePath, stream);
        }
    }

    public async Task DownloadFileAsync(string fileId, string fileName, string filePath = Constants.DOWNLOADS_PATH)
    {
        var file = await botClient.GetFileAsync(fileId);
        filePath += fileName;
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await botClient.DownloadFileAsync(file.FilePath, stream);
        }
    }

    public async Task SendMessageAsync(long chatId, string message)
    {
        await botClient.SendTextMessageAsync(chatId, message);
    }

    public async Task SendMessageAsync(long chatId, string message, ReplyKeyboardMarkup replyKeyboardMarkup)
    {
        await botClient.SendTextMessageAsync(chatId, message, replyMarkup: replyKeyboardMarkup);
    }

    public async static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception.Message);
    }
}
