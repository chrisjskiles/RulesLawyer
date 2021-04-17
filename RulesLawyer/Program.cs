using Discord;
using System;
using System.Threading.Tasks;
using System.Configuration;
using Discord.WebSocket;
using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;

namespace RulesLawyer
{
    class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _handler;
        private readonly IServiceProvider _services;
        private readonly NameValueCollection _settings = ConfigurationManager.AppSettings;

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50
            });

            var commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false
            });

            //setup DI
            _services = new ServiceCollection().AddSingleton(new Initialize(commands, _client)).BuildServiceProvider();

            //subscribe the log to the command service and client
            _client.Log += Log;
            commands.Log += Log;

            _handler = new CommandHandler(_client, commands, _services);
        }

        public async Task MainAsync()
        {
            await _handler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, _settings.Get("BotToken"));
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
