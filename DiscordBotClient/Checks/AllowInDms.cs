using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

namespace DiscordBotClient.Checks;


// for parameters use IParameterCheck
// https://dsharpplus.github.io/DSharpPlus/articles/commands/custom_context_checks.html#parameter-checks
public sealed class AllowInDmsAttribute(DirectMessageUsage usage = DirectMessageUsage.AllowDMs) : ContextCheckAttribute
{
    public DirectMessageUsage Usage { get; init; } = usage;
}

public sealed class DirectMessageUsageCheck : IContextCheck<AllowInDmsAttribute>
{
    public ValueTask<string?> ExecuteCheckAsync(AllowInDmsAttribute attribute, CommandContext context)
    {
        switch (context.Channel.IsPrivate)
        {
            // When the command is sent via DM and the attribute allows DMs, allow the command to be executed.
            // When the command is sent outside of DM and the attribute allows non-DMs, allow the command to be executed.
            case true when attribute.Usage is not (DirectMessageUsage.DenyDMs or DirectMessageUsage.RequireDMs):
                return ValueTask.FromResult<string?>(null);

            // The command was sent via DM but the attribute denies DMs
            // The command was sent outside of DM but the attribute requires DMs.
            default:
            {
                var dmStatus = context.Channel.IsPrivate ? "inside a DM" : "outside a DM";
                var requirement = attribute.Usage switch
                {
                    DirectMessageUsage.DenyDMs => "denies DM usage",
                    DirectMessageUsage.RequireDMs => "requires DM usage",
                    DirectMessageUsage.AllowDMs => throw new NotImplementedException("uhhh"),
                    _ => throw new NotImplementedException($"A new DirectMessageUsage value was added and not implemented in the {nameof(DirectMessageUsageCheck)}: {attribute.Usage}")
                };

                return ValueTask.FromResult<string?>($"The executed command {requirement} but was executed {dmStatus}.");
            }
        }
    }
}

