using DiscordBotClient.Common;
using dotenv.net;

DotEnv.Fluent().WithProbeForEnv(6).Load();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddStructuredLogging();
builder.Services.AddDiscordServices();

var host = builder.Build();

var discordClient = host.Services.CreateDiscordClient();
await discordClient.ConnectAsync();

host.Run();