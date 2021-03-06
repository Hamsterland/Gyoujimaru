using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Gyoujimaru.CustomExtensions;
using Gyoujimaru.Notifications;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hamsterland.MyAnimeList.Services.Commands
{
    public class CommandHandler : INotificationHandler<MessageReceivedNotification>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public CommandHandler(
            DiscordSocketClient client, 
            CommandService commandService, 
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _client = client;
            _commandService = commandService;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }
        
        public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
        {
            if (notification.Message is not SocketUserMessage { Author: IUser user } message || user.IsBot)
            {
                return;
            }
            
            var argPos = 0;
            var prefix = _configuration.GetPrefix();
            
            if (message.HasStringPrefix(prefix, ref argPos))
            {
                var scope = _serviceProvider.CreateScope();
                var context = new SocketCommandContext(_client, message);
                await _commandService.ExecuteAsync(context, argPos, scope.ServiceProvider); 
            }
        }
    }
}