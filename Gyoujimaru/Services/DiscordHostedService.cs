using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Gyoujimaru.Notifications;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Gyoujimaru.Services
{
    public class DiscordHostedService : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IMediator _mediator;

        public DiscordHostedService(
            DiscordSocketClient client,
            CommandService commandService,
            IMediator mediator)
        {
            _client = client;
            _mediator = mediator;
            _commandService = commandService;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += MessageReceived;
            _client.Log += Log;
            _commandService.CommandExecuted += CommandExected;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived -= MessageReceived;
            _client.Log -= Log;
            _commandService.CommandExecuted -= CommandExected;
            return Task.CompletedTask;
        }

        private async Task CommandExected(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            await _mediator.Publish(new CommandExecutedNotification(command, context, result));
        }


        public async Task MessageReceived(SocketMessage message)
        {
            await _mediator.Publish(new MessageReceivedNotification(message));
        }
        
        private async Task Log(LogMessage log)
        {
            await _mediator.Publish(new LogMessageNotification(log));
        }
    }
}