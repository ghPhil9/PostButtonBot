using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace PostButtonBot
{
    internal class Core
    {
        static void Main(string[] args) => new Core();

        internal TelegramBotClient TgBot { get; set; }
        internal long AdminId { get; set; }
        internal long DestinationChatId { get; set; }

        internal Core()
        {
            Console.Title = $"PostButtonBot v0.1 [https://t.me/CSharpHive]";

            if (!Config.Read(this))
            {
                Console.WriteLine("Нажмите любую кнопку, чтобы закрыть консоль...");
                Console.ReadKey();
                return;
            }

            TgBot.OnMessage += TelegramBot_OnMessage;
            var bot = TgBot.GetMeAsync().Result;
            botMessage = new BotMessage(this);

            Console.WriteLine($"Бот @{bot.Username} в сети!");
            TgBot.StartReceiving();

            Console.ReadKey();
            TgBot.StopReceiving();
        }

        private BotMessage botMessage;

        private void TelegramBot_OnMessage(object sender, MessageEventArgs e)
        {
            string text = e.Message.Text;

            try
            {
                // Пропуск всех сообщений от неизвестных
                if (e.Message.From.Id != AdminId)
                {
                    Console.WriteLine($"{e.Message.From.FirstName} @{e.Message.From.Username} пишет: {text}");
                    return;
                }

                // Пропуск лишних данных
                if (text == null && e.Message.Caption == null) return;

                // Считывание картинки и описания
                if (botMessage.Caption == null)
                {
                    // Обходим возможную ошибку
                    if (e.Message.Type != MessageType.Photo) return;

                    botMessage.Caption = e.Message.Caption;
                    botMessage.CaptionEntities = e.Message.CaptionEntities;
                    botMessage.Photo = new InputOnlineFile(e.Message.Photo[2].FileId);
                    return;
                }

                // Считывание ссылки для кнопки
                if (botMessage.UrlButton == null)
                {
                    botMessage.UrlButton = text;
                    return;
                }

                // Считывание текста для кнопки
                if (botMessage.TextButton == null)
                {
                    botMessage.TextButton = text;
                    botMessage.Preview().Wait();
                    return;
                }

                // Отправка поста в целевой чат
                if (text == "+")
                {
                    botMessage.Send().Wait();
                    return;
                }

                // Обнуление данных
                if (text == "-")
                {
                    botMessage.Clear();
                    return;
                }
            }
            catch (Exception ex) { Console.WriteLine($"Возникла неизвестная ошибка: {ex.Message}"); }
        }
    }
}