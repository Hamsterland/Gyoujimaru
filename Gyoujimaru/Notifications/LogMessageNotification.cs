using Discord;
using MediatR;

namespace Gyoujimaru.Notifications
{
    public class LogMessageNotification : INotification
    {
        public LogMessage LogMessage { get; }

        public LogMessageNotification(LogMessage logMessage)
        {
            LogMessage = logMessage;
        }
    }
}