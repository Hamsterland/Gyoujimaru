using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Gyoujimaru.Notifications;
using MediatR;
using Serilog;

namespace Hamsterland.MyAnimeList.Services.Commands
{
    public class CommandExecutedHandler : INotificationHandler<CommandExecutedNotification>
    {
        private readonly ILogger _logger;

        public CommandExecutedHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Handle(CommandExecutedNotification notification, CancellationToken cancellationToken)
        {
            var (_, _, result) = notification.Deconstruct();

            if (result.IsSuccess)
            {
                return;
            }

            switch (result)
            {
                case ExecuteResult executeResult:
                {
                    var message = new StringBuilder()
                        .AppendLine(Format.Bold("ERROR REASON"))
                        .AppendLine(result.ErrorReason)
                        .AppendLine()
                        .AppendLine(Format.Bold("COMMAND ERROR"))
                        .AppendLine(result.Error?.ToString())
                        .AppendLine()
                        .AppendLine(Format.Bold("EXCEPTION"))
                        .AppendLine(executeResult.Exception.Message)
                        .AppendLine(executeResult.Exception.Source)
                        .AppendLine(executeResult.Exception.HelpLink)
                        .AppendLine(executeResult.Exception.StackTrace)
                        .ToString();

                    _logger.Error("{Message}", message);
                    break;
                }
            }
        }
    }
}