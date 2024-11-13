// See https://aka.ms/new-console-template for more information

using Discord.WebSocket;

internal class Program 
{
    private readonly DiscordSocketClient client; 
    private string token = File.ReadAllText("../../../token.txt");

    public Program()
    {
        this.client = new DiscordSocketClient();
        this.client.MessageReceived += MessageHandler;
    }

    public async Task StartBotAsync()
    {
        await this.client.LoginAsync(Discord.TokenType.Bot, token);
        await this.client.StartAsync();
        await Task.Delay(-1);
    }

    private async Task MessageHandler(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        await ReplyAsync(message, "C# Response works!");
    }

    private async Task ReplyAsync(SocketMessage message, string response)
    {
        await message.Channel.SendMessageAsync(response);
    }

    static async Task Main(string[] args)
    {
        var myBot = new Program();
        await myBot.StartBotAsync();
    }
}
