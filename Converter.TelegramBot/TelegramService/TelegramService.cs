using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Converter.TelegramBot.TelegramService
{
    public class TelegramService
    {
        TelegramBotClient client;
        public TelegramService(TelegramBotClient telegramBotClient)
        {
            this.client = telegramBotClient;
        }

        
        
        public async Task SendMessageAsync(long chatId, string message)
        {
            await client.SendTextMessageAsync(chatId, message);
        }

        public async Task SendMessageAsync(long chatId, string message, ReplyKeyboardMarkup replyKeyboardMarkup)
        {
            await client.SendTextMessageAsync(chatId, message, replyMarkup: replyKeyboardMarkup);
        }
    }
}
