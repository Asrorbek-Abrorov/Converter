using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Converter.Domain.Configuration;
using Telegram.Bot.Types.Enums;
using Spectre.Console;
using Converter.Services.Services;
using Converter.Data.AppDbContexts;
using Microsoft.EntityFrameworkCore;

namespace Converter.TelegramBot.TelegramService;

public class TelegramService
{
    TelegramBotClient botClient;
    Converters converters;
    AppDbContext appDbContext;
    public TelegramService(TelegramBotClient telegramBotClient, Converters converters)
    {
        this.botClient = telegramBotClient;
        this.converters = converters;
        appDbContext = new AppDbContext();
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

        if (await IfUserExists(message.From.Id))
        {
            await SendMessageAsync(message.From.Id, "Hi");
        }
        else
        {
            await SendMessageAsync(message.From.Id, "Hi");
            await AddUser(message);
        }
        if (update.Message?.Type == MessageType.Photo)
        {
            await DownloadPhotoAsync(update.Message.Photo[3].FileId);
        }
        else if (update.Message?.Type == MessageType.Document)
        {
            await DownloadFileAsync(update.Message.Document.FileId, update.Message.Document.FileName);
            var resPath = await converters.PdfToWordConverter(Constants.DOWNLOADS_PATH + update.Message.Document.FileName, converters.GenerateFileName(".docx"));
            await SendFileAsync(update.Message.From.Id, resPath);
        }
    }

    private async Task AddUser(Message message)
    {
        var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == message.From.Id);
        if (user == null)
        {
            user = new Domain.Entities.User
            {
                TelegramId = message.From.Id,
                FirstName = message.From.FirstName,
                LastName = message.From.LastName,
                UserName = message.From.Username,
            };
            await appDbContext.Users.AddAsync(user);
            await appDbContext.SaveChangesAsync();
        }
    }

    public async Task GetUser(long id)
        => await appDbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == id);

    public async Task<bool> IfUserExists(long id)
    {
        if (await appDbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == id) == null)
        {
            return false;
        }
        return true;
    }

    public async Task SendFileAsync(long chatId, string filePath)
    {
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            var fileName = Path.GetFileName(filePath);
            var fileToSend = new InputFileStream(stream, fileName);

            await botClient.SendDocumentAsync(chatId, fileToSend);
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
