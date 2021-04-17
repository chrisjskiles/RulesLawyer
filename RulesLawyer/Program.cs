using Discord;
using System;
using System.Threading.Tasks;
using System.Configuration;
using Discord.WebSocket;

namespace RulesLawyer
{
    class Program
    {
        private DiscordSocketClient _client;
        private ApplicationSettingsBase _settings = new AppSettings();

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50
            });
            _client.Log += Log;

            var token = _settings;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();


            await Task.Delay(-1);
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
