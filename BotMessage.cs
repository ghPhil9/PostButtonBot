using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;

namespace PostButtonBot
{
    internal class BotMessage
    {
        internal string Caption { get; set; } // Сообщение
        internal MessageEntity[] CaptionEntities { get; set; } // Разметка сообщения
        internal InputOnlineFile Photo { get; set; } // Картинка
        internal string UrlButton { get; set; } // Ссылка кнопки
        internal string TextButton { get; set; } // Текст кнопки

        internal BotMessage(Core core) => this.core = core;

        private Core core;

        internal async Task Preview()
        {
            string text = "Так будет выглядеть пост.\nНапиши \"+\", чтобы отправить его в целевой чат, или \"-\", чтобы обнулить данные";
            await core.TgBot.SendPhotoAsync(core.AdminId, Photo, Caption, captionEntities: CaptionEntities, replyMarkup: BuildMarkup());
            await core.TgBot.SendTextMessageAsync(core.AdminId, text);
        }

        internal async Task Send()
        {
            if (Caption == null || UrlButton == null || TextButton == null)
            {
                throw new Exception("Не все данные заполнены для отправки поста!");
            }

            await core.TgBot.SendPhotoAsync(core.DestinationChatId, Photo, Caption, captionEntities: CaptionEntities, replyMarkup: BuildMarkup());
            Console.WriteLine("Пост отправлен в чат! Данные для поста очищены");
            Clear();
        }

        internal void Clear() => Caption = TextButton = UrlButton = null;

        private InlineKeyboardMarkup BuildMarkup()
        {
            InlineKeyboardButton button = new InlineKeyboardButton() { Url = UrlButton, Text = TextButton };
            return new InlineKeyboardMarkup(button);
        }
    }
}