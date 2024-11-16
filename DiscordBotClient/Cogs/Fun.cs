// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace DiscordBotClient.Cogs;

public sealed class Fun(ILogger<Fun> logger)
{
    [Command("ping")]
    [Description("ping the bot, see if its up")]
    public async Task Ping(CommandContext context)
    {
        logger.LogInformation("ping request by {UserId}", context.User.Id);
        await context.DeferResponseAsync();
        await context.FollowupAsync($"pong! {context.Client.Ping}ms");
    }

    [Command("embed")]
    [Description("send an embed")]
    public async Task Embed(
        CommandContext context,
        [Description("the title of the embed")] string title,
        [Description("the description of the embed")] string description,
        [Description("the color of the embed")] string color)
    {
        logger.LogInformation("embed request by {UserId}", context.User.Id);
        await context.RespondAsync(new DiscordEmbedBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .WithColor(new DiscordColor(color)));
    }

    [Command("sha256")]
    [Description("sha256 the given text")]
    public async Task Sha256(CommandContext context, string input)
    {
        logger.LogInformation("sha256 request by {UserId}", context.User.Id);
        await context.DeferResponseAsync();

        // IAsyncDisposable for objects that are asynchronously disposable
        // normal IDisposable is same as `with` in python
        await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var hash = await SHA256.HashDataAsync(ms);
        var result = BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);

        await using var file = new MemoryStream(Encoding.UTF8.GetBytes(result));
        await context.FollowupAsync(new DiscordMessageBuilder()
            .WithContent("Here is your sha256 hash!")
            .WithAllowedMention(new UserMention(context.User))
            .AddFile("sha256.txt", file, AddFileOptions.CopyStream));
    }

    [Command("info")]
    [Description("get info about a user")]
    public async Task Info(CommandContext context, DiscordUser user)
    {
        logger.LogInformation("info request by {UserId}", context.User.Id);
        await context.RespondAsync(new DiscordEmbedBuilder()
            .WithAuthor(user.Username, iconUrl: user.AvatarUrl)
            .AddField("Id", user.Id.ToString(), inline: true)
            .AddField("Is bot", user.IsBot.ToString(), inline: true)
            .AddField("Is system", user.IsSystem?.ToString() ?? "false", inline: true)
            .AddField("Is current", user.IsCurrent.ToString(), inline: true)
            .AddField("Mention", user.Mention, inline: true)
            .AddField("Created at", user.CreationTimestamp.ToString(), inline: true)
            .WithColor(DiscordColor.Gold));
    }

    [Command("dice")]
    [Description("roll a dice")]
    public async Task DelayedCounter(
        CommandContext context,
        [Description("predict a number (1-6)")] int prediction)
    {
        string[] dice = ["\u2680", "\u2681", "\u2682", "\u2683", "\u2684", "\u2685"];
        logger.LogInformation("dice request by {UserId}", context.User.Id);

        if (prediction is < 1 or > 6)
        {
            await context.RespondAsync("pick a number between 1 and 6 please");
            return;
        }

        await context.Channel.TriggerTypingAsync();
        await context.RespondAsync("# Rolling the dice... 🎲");

        var num = 0;
        for (var i = 0; i < 3; i++)
        {
            num = Random.Shared.Next(6);
            await Task.Delay(750);
            await context.EditResponseAsync($"# Rolling the dice... {dice[num]}");
        }

        var state = num == prediction - 1 ? "won!! 🎊" : "lost";
        await context.EditResponseAsync($"# The dice rolled {dice[num]} you predicted {prediction}. you {state}");
    }
}