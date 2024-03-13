using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Converter.Domain.Configuration;
using Telegram.Bot.Types.Enums;
using Spectre.Console;
using Converter.Services.Services;
using Converter.Data.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Converter.Domain.Entities;

namespace Converter.TelegramBot.TelegramService;

public class TelegramService
{
    TelegramBotClient botClient;
    ConverterService converterService;
    AppDbContext appDbContext;
    public TelegramService(TelegramBotClient telegramBotClient, ConverterService converters)
    {
        this.botClient = telegramBotClient;
        this.converterService = converters;
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
        
        var userHistory = new UserHistory()
        {
            UserId = message.From.Id,
            FileName = message.Document?.FileName ?? message.Text, // Set the desired file name
        };

        await appDbContext.History.AddAsync(userHistory);
        await appDbContext.SaveChangesAsync();

        AnsiConsole.MarkupLine($"[yellow]{message?.From?.FirstName}[/]  |  {message?.Text} | {update.Message?.Type}");

        ReplyKeyboardMarkup reply = new(new[]
                {
                    new KeyboardButton[] { "History", "Categories of converting", "Account Info" } 
                })
        {
            ResizeKeyboard = true
        };

        await SendMessageAsync(message.From.Id, "Choose: ", reply);


        if (await IfUserExists(message.From.Id))
        {
            await SendMessageAsync(message.From.Id, "Hi");
        }
        else
         {
            await SendMessageAsync(message.From.Id, "Hello");
            await AddUser(message);
        }

        if (message.Text == "History")
        {
            await SendMessageAsync(message.From.Id, "History");
            await SendHistory(message.From.Id);
        }

        if (message.Text == "Account Info")
        {
            // Retrieve user information from the database
            var userId = message.From.Id;
            Domain.Entities.User user = await GetUser(userId);

            // Compose and send the account settings message
            var accountSettingsMessage = $"Account Info:\n\n" +
                $"First Name: {user.FirstName ?? "N/A"}\n" +
                $"Last Name: {user.LastName ?? "N/A"}\n" +
                $"Username: {user.UserName ?? "N/A"}\n" +
                $"Phone: {user.Phone ?? "N/A"}\n" +
                $"Email: {user.Email ?? "N/A"}";

            await SendMessageAsync(message.From.Id, accountSettingsMessage);
        }

        if (message.Text == "Categories of converting")
        {
            List<string> methodNames = converterService.GetMethodNames();

            // Create the reply keyboard buttons
            //var keyboardButtons = methodNames
            //    .Select(methodName => new KeyboardButton(methodName))
            //    .ToArray();
            var buttons = new List<KeyboardButton[]>();

            foreach (var methodName in methodNames)
            {
                var button = new KeyboardButton(methodName);
                buttons.Add(new KeyboardButton[] { button });
            }

            // Create the reply keyboard markup
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(buttons);

            // Set the reply keyboard markup as the reply markup of the message
            var replyMarkup = new ReplyKeyboardMarkup(buttons);

            // Send a message to the user with the reply keyboard
            await SendMessageAsync(update.Message.Chat.Id, "Select a method:", replyMarkup: replyMarkup);
        }
        else if (message?.Text?.ToUpper().Contains("ASCII=") ?? false)
        {
            var text = message.Text.Substring(message.Text.IndexOf("ASCII=") + 7);
            var ascii = await converterService.TextToAsciiConverter(text);
            await SendMessageAsync(message.From.Id, $"ASCII = {ascii}");
        }
        else if (message?.Text?.ToUpper().Contains("TEXT=") ?? false)
        {
            var binaryData = message.Text.Substring(message.Text.IndexOf("TEXT=") + 6);
            var text = await converterService.BinaryToTextConverter(binaryData);
            await SendMessageAsync(message.From.Id, $"Text = {text}");
        }
        else if (message?.Text?.ToUpper().Contains("BINARY=") ?? false)
        {
            var binaryData = message.Text.Substring(message.Text.IndexOf("BINARY=") + 8);
            var binary = await converterService.TextToBinaryConverter(binaryData);
            var res = string.Join("", binary);
            await SendMessageAsync(message.From.Id, $"BINARY = {res}");
        }
        else if (message?.Text?.ToUpper().Contains("DOC=") ?? false)
        {
            var decimalString = message.Text.Substring(message.Text.IndexOf("DOC=") + 5);
            if (decimal.TryParse(decimalString, out decimal decimalValue))
            {
                var octal = await converterService.DecimalToOctalConverter(decimalValue);
                await SendMessageAsync(message.From.Id, $"OCTAL = {octal}");
            }
            else
            {
                await SendMessageAsync(message.From.Id, "Invalid decimal value. Please provide a valid decimal number after 'DOC='.");
            }
        }
        else if (message?.Text?.ToUpper().Contains("DHEX=") ?? false)
        {
            var decimalString = message.Text.Substring(message.Text.IndexOf("DHEX=") + 6);
            if (decimal.TryParse(decimalString, out decimal decimalValue))
            {
                var hexadecimal = await converterService.DecimalToHexadecimalConverter(decimalValue);
                await SendMessageAsync(message.From.Id, $"HEXADECIMAL = {hexadecimal}");
            }
            else
            {
                await SendMessageAsync(message.From.Id, "Invalid decimal value. Please provide a valid decimal number after 'DHEX='.");
            }
        }
        else if (message?.Text?.ToUpper().Contains("DPER=") ?? false)
        {
            var decimalString = message.Text.Substring(message.Text.IndexOf("DPER=") + 6);
            if (decimal.TryParse(decimalString, out decimal decimalValue))
            {
                var percent = await converterService.DecimalToPercent(decimalValue);
                await SendMessageAsync(message.From.Id, $"PERCENT = {percent}%");
            }
            else
            {
                await SendMessageAsync(message.From.Id, "Invalid decimal value. Please provide a valid decimal number after 'DPER='.");
            }
        }
        else if (message?.Text?.ToUpper().Contains("DECIMAL=") ?? false)
        {
            var decimalString = message.Text.Substring(message.Text.IndexOf("DECIMAL=") + 9);
            var converted = await converterService.ConvertFromDecimal(int.Parse(decimalString));
            await SendMessageAsync(message.From.Id, $"Converted value = {converted}");
        }
        else if (message?.Text?.ToUpper().Contains("TODEC=") ?? false)
        {
            var numberString = message.Text.Substring(message.Text.IndexOf("TODEC=") + 7);
            var decimalValue = await converterService.ConvertToDecimal(numberString);
            await SendMessageAsync(message.From.Id, $"Decimal value = {decimalValue}");
        }
        else if (message?.Text?.ToUpper().Contains("CHAR=") ?? false)
        {
            var charValue = message.Text.Substring(message.Text.IndexOf("CHAR=") + 6);
            if (int.TryParse(charValue, out int parsedCharValue))
            {
                var value = await converterService.GetCharValue(parsedCharValue);
                await SendMessageAsync(message.From.Id, $"Character value = {value}");
            }
            else
            {
                await SendMessageAsync(message.From.Id, "Invalid character value. Please enter a valid integer value.");
            }
        }
        else if (message?.Text?.ToUpper().Contains("DGT=") ?? false)
        {
            var digitValue = message.Text.Substring(message.Text.IndexOf("DGT=") + 5)[0];
            var value = await converterService.GetDigitValue(digitValue);
            await SendMessageAsync(message.From.Id, $"Digit value = {value}");
        }
        else if (message?.Text?.ToUpper().Contains("PDFTOWORDCONVERTER") ?? false)
        {
            await SendMessageAsync(message.From.Id, "Send me a PDF file");
        }
        else if (message?.Text?.ToUpper().Contains("FILETOBYTECONVERTER") ?? false)
        {
            await SendMessageAsync(message.From.Id, "Send me a file");
        }
        else if (message?.Text?.ToUpper().Contains("FILETOBINARYCONVERTER") ?? false)
        {
            await SendMessageAsync(message.From.Id, "Send me a file");
        }

        else if (update.Message?.Type == MessageType.Photo)
        {
            await DownloadPhotoAsync(update.Message.Photo[3].FileId);
            await DownloadFileAsync(update.Message.Document.FileId, update.Message.Document.FileName);
            var resPath = await converterService.PdfToWordConverter(Constants.DOWNLOADS_PATH + update.Message.Document.FileName, converterService.GenerateFileName(".docx"));
            await SendFileAsync(update.Message.From.Id, resPath);
        }
        else if (update.Message?.Type == MessageType.Document)
        {
            var document = update.Message.Document;
            if (document.MimeType == "application/pdf")
            {
                await DownloadFileAsync(document.FileId, document.FileName);
                var resPath = await converterService.PdfToWordConverter(Constants.DOWNLOADS_PATH + document.FileName, converterService.GenerateFileName(".docx"));
                await SendFileAsync(update.Message.From.Id, resPath);
            }
            else if (document.MimeType == "image/jpeg" || document.MimeType == "image/png")
            {
                await DownloadFileAsync(document.FileId, document.FileName);
                //var resPath = await converterService.ImageToSomethingConverter(Constants.DOWNLOADS_PATH + document.FileName, converterService.GenerateFileName(".extension"));
                //await SendFileAsync(update.Message.From.Id, resPath);
            }
            else
            {
                await DownloadFileAsync(document.FileId, document.FileName);
            }
        }
        if (converterService.GetMethodNames().Any(m => m == message.Text))
        {
            switch (message.Text)
            {
                case "PdfToWordConverter":
                    await SendMessageAsync(message.From.Id, "Send me a PDF file");
                    break;
                case "TextToAsciiConverter":
                    await SendMessageAsync(message.From.Id, "Write me a text. And it should be (ASCII=....)");
                    break;
                case "FileToByteConverter":
                    await SendMessageAsync(message.From.Id, "Send me a file");
                    break;
                case "FileToBinaryConverter":
                    await SendMessageAsync(message.From.Id, "Send me a file");
                    break;
                case "TextToBinaryConverter":
                    await SendMessageAsync(message.From.Id, "Write me a text. And it should be (BINARY=....)");
                    break;
                case "BinaryToTextConverter":
                    await SendMessageAsync(message.From.Id, "Write me binary data. And it should be (TEXT=....)");
                    break;
                case "DecimalToBinaryConverter":
                    await SendMessageAsync(message.From.Id, "Enter a decimal number. And it should be (DBIN=....)");
                    break;
                case "DecimalToOctalConverter":
                    await SendMessageAsync(message.From.Id, "Enter a decimal number. And it should be (DOC=....)");
                    break;
                case "DecimalToHexadecimalConverter":
                    await SendMessageAsync(message.From.Id, "Enter a decimal number. And it should be (DHEX=....)");
                    break;
                case "DecimalToPercent":
                    await SendMessageAsync(message.From.Id, "Enter a decimal number. And it should be (DPER=....)");
                    break;
                case "ConvertFromDecimal":
                    await SendMessageAsync(message.From.Id, "Enter a decimal number. And it should be (DECIMAL=...)");
                    break;
                case "ConvertToDecimal":
                    await SendMessageAsync(message.From.Id, "Enter a number in another base. And it should be (TODEC=....)");
                    break;
                case "GetCharValue":
                    await SendMessageAsync(message.From.Id, "Enter a character. And it should be (CHAR=....)");
                    break;
                case "GetDigitValue":
                    await SendMessageAsync(message.From.Id, "Enter a digit. And it should be (DGT=....)");
                    break;
                default:
                    // Handle the case where the user's message doesn't match any method name
                    await SendMessageAsync(message.From.Id, "Invalid method name");
                    break;
            }
            await SendMessageAsync(message.From.Id, "Send me a file");
        }
    }

    private async Task SendHistory(long userId)
    {
        // Retrieve user history from the database
        List<UserHistory> userHistory;

        userHistory = await appDbContext.History
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.TimeOfConverted)
            .ToListAsync();

        // Compose and send the history message
        var historyMessage = "History:\n\n";
        foreach (var historyEntry in userHistory)
        {
            historyMessage += $"File or Text: {historyEntry.FileName}\n" +
                $"Time of Conversion or written: {historyEntry.TimeOfConverted}\n\n";
        }

        await SendMessageAsync(userId, historyMessage);
    }


    private async Task AddUser(Message message)
    {
        var user = new Domain.Entities.User
        {
            TelegramId = message.From.Id,
            FirstName = message.From?.FirstName,
            LastName = message.From?.LastName,
            UserName = message.From?.Username,
            Phone = "N/A",
            Email = "N/A"
        };
        await appDbContext.Users.AddAsync(user);
        await appDbContext.SaveChangesAsync();
    }

    public async Task<Domain.Entities.User> GetUser(long id)
        => await appDbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == id);

    public async Task<bool> IfUserExists(long id)
    {
        var users = appDbContext.Users.ToList();
        if (await appDbContext.Users.DefaultIfEmpty().FirstOrDefaultAsync(u => u.TelegramId == id) == null)
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

    public async Task SendMessageAsync(long chatId, string message, ReplyKeyboardMarkup replyMarkup)
    {
        await botClient.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup);
    }

    public async static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception.Message);
    }
}
