using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PostButtonBot
{
    internal class Config
    {
        public string TokenTelegramBot { get; private set; }
        public long AdminBotId { get; private set; }
        public long DestinationChatId { get; private set; }

        internal Config(string tokenTelegramBot, long adminBotId, long destinationChatId)
        {
            TokenTelegramBot = tokenTelegramBot;
            AdminBotId = adminBotId;
            DestinationChatId = destinationChatId;
        }

        internal static bool Read(Core core)
        {
            string config = Environment.CurrentDirectory + "/Config.txt";

            if (!File.Exists(config))
            {
                File.WriteAllText(config, New());
                Console.WriteLine("Конфиг файл отсутствует. Был создан новый конфиг файл");
                return false;
            }

            try
            {
                JObject json = JObject.Parse(File.ReadAllText(config));
                core.TgBot = new Telegram.Bot.TelegramBotClient(json.SelectToken("TokenTelegramBot").ToString());
                core.AdminId = (long)json.SelectToken("AdminBotId");
                core.DestinationChatId = (long)json.SelectToken("DestinationChatId");
            }
            catch (Exception e)
            {
                File.WriteAllText(config, New());
                Console.WriteLine($"Возникла ошибка при считывании конфиг файла: {e.Message}");
                Console.WriteLine("Конфиг файл перезаписан");
                return false;
            }

            Console.WriteLine("Конфиг файл успешно считан");
            return true;
        }

        private static string New()
        {
            Config config = new Config("ТОКЕН_БОТА", 0, 0);
            return JsonConvert.SerializeObject(config, Formatting.Indented);
        }
    }
}