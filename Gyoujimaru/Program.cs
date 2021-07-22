using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Gyoujimaru.CustomExtensions;
using Gyoujimaru.Data;
using Gyoujimaru.MyAnimeList.Characters;
using Gyoujimaru.Services;
using Gyoujimaru.Services.Olympics._2021;
using Gyoujimaru.Services.Olympics._2021.Tracker;
using Interactivity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

using Serilog.Sinks.SystemConsole.Themes;

namespace Gyoujimaru
{
    public class Program
    {
        public static async Task Main(string[] args) => await Host.CreateDefaultBuilder()
            .UseSerilog((_, configuration) =>
            {
                configuration
                    // .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Console(theme: SystemConsoleTheme.Literate);
            })
            .ConfigureAppConfiguration((_, builder) =>
            {
                builder
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true);
            })
            .ConfigureServices((context, collection) =>
            {
                var client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 10000,
                    LogLevel = LogSeverity.Verbose
                });

                var commands = new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Sync,
                    LogLevel = LogSeverity.Verbose,
                    ThrowOnError = true
                });
                
                collection
                    .AddHostedService<StartupHostedService>()
                    .AddHostedService<DiscordHostedService>()
                    .AddSingleton<TrackerService>()
                    .AddSingleton<CharacterClient>()
                    .AddSingleton<CharacterService>()
                    .AddSingleton<BlockedUserService>()
                    .AddSingleton<InteractivityService>()
                    .AddSingleton(new InteractivityConfig { DefaultTimeout = TimeSpan.FromSeconds(20) })
                    .AddMediatR(typeof(Program).Assembly)
                    .AddSingleton(client)
                    .AddSingleton(provider =>
                    {
                        commands.AddModulesAsync(typeof(Program).Assembly, provider);
                        return commands;
                    })
                    .AddDbContext<GyoujimaruContext>(options =>
                    {
                        options.UseNpgsql(context.Configuration.GetConnectionString());
                    });
            }).RunConsoleAsync();
    }
}