namespace DiscordBotClient.Common;

public static class Util
{
    /// <summary>
    /// String extensions method to get an environment variable (with optional default) or throw an exception if it is not present.
    /// </summary>
    /// <example>
    /// <code title="Sample usage">
    /// "SOME_NULLABLE_KEY.FromEnvRequired("default")
    /// "API_KEY".FromEnvRequired() // will throw if key is not present.
    /// </code>
    /// </example>
    public static string FromEnvRequired(this string name, string? orElse = default) =>
        Environment.GetEnvironmentVariable(name)
        ?? orElse
        ?? throw new InvalidOperationException($"'{name}' is not present in the environment");
}