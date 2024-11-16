using Serilog;
using Serilog.Events;

namespace DiscordBotClient.Common;

public static class LoggingExt
{
    private const string ConsoleOutputFormat =
        "[{Timestamp:dd-MM-yyyy HH:mm:ss.fff} {Level:u3}]{NewLine}{SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}";

    private const string FileOutputFormat =
        "[{Timestamp:dd-MM-yyyy HH:mm:ss.fff} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}";

    public static LoggerConfiguration ConfigureSerilog(this LoggerConfiguration config)
    {
        var logPath = "LOG__PATH".FromEnvRequired();
        // TODO set up seq
        // var seqApiKey = "SEQ__API_KEY".FromEnvRequired();

        return config
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Quartz", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithAssemblyName()
            .Enrich.WithProperty("ApplicationName", "DiscordBot")
            .WriteTo.Debug()
            .WriteTo.Console(outputTemplate: ConsoleOutputFormat)
            // .WriteTo.Seq("http://seq:5342", apiKey: seqApiKey)
            .WriteTo.File(
                logPath,
                outputTemplate: FileOutputFormat,
                flushToDiskInterval: TimeSpan.FromSeconds(10),
                fileSizeLimitBytes: 100_000_000,
                rollOnFileSizeLimit: true,
                rollingInterval: RollingInterval.Day);
    }
}