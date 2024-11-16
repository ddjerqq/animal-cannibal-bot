using System.Reflection;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.Commands.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.Extensions;
using Serilog;

namespace DiscordBotClient.Common;

public static class ServiceExt
{
    public static void AddStructuredLogging(this IServiceCollection services)
    {
        // global static logger
        Log.Logger = new LoggerConfiguration()
            .ConfigureSerilog()
            .CreateLogger();

        services.AddSerilog();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, true));
    }

    public static void AddDiscordServices(this IServiceCollection services)
    {
        var token = "BOT__TOKEN".FromEnvRequired();
        services.AddDiscordClient(token, DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents);
    }

    public static DiscordClient CreateDiscordClient(this IServiceProvider services)
    {
        var client = services.GetRequiredService<DiscordClient>();

        var commandsExtension = client.UseCommands(new CommandsConfiguration
        {
            UseDefaultCommandErrorHandler = false,
            DebugGuildId = 1299491186410782851,
        });

        var assembly = Assembly.GetExecutingAssembly();
        commandsExtension.AddChecks(assembly);
        commandsExtension.AddCommands(assembly);

        commandsExtension.CommandErrored += OnCommandErrored;

        return client;
    }

    private static async Task OnCommandErrored(CommandsExtension ctx, CommandErroredEventArgs args)
    {
        if (args.Exception is CommandNotFoundException)
        {
            Log.Logger.Error(args.Exception, "command not found");
            return;
        }

        var incidentId = Ulid.NewUlid();

        await args.Context.RespondAsync(new DiscordEmbedBuilder()
            .WithTitle("Something went wrong!")
            .WithColor(DiscordColor.Red)
            .WithDescription($"```js\n{args.Exception.Message}```")
            .WithUrl("https://github.com/ddjerqq/animal-cannibal-bot/issues")
            .WithFooter($"Incident id: {incidentId}")
        );

        Log.Logger.Error(args.Exception, "Incident {incidentId} has occured while executing command {CommandName}", incidentId, args.Context.Command.Name);

        throw new Exception($"Incident {incidentId} has occured while executing command {args.Context.Command.Name}", args.Exception);
    }
}