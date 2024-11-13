// See https://aka.ms/new-console-template for more information

using Discord.WebSocket;

namespace animal_cannibal_bot
{
    internal class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly string _token = File.ReadAllText("../../../token.txt");

        private Program()
        {
            this._client = new DiscordSocketClient();
            this._client.MessageReceived += MessageHandler;
        }

        private async Task StartBotAsync()
        {
            await this._client.LoginAsync(Discord.TokenType.Bot, _token);
            await this._client.StartAsync();
            await Task.Delay(-1);
        }

        private static async Task MessageHandler(SocketMessage message)
        {
            // Temporarily static
            if (message.Author.IsBot) return;

            await ReplyAsync(message, "C# Response works!");
        }

        private static async Task ReplyAsync(SocketMessage message, string response)
        {
            // This is only temporarily static
            await message.Channel.SendMessageAsync(response);
        }

        private static async Task Main(string[] args)
        {
            var myBot = new Program();
            await myBot.StartBotAsync();
        }
    }
}
