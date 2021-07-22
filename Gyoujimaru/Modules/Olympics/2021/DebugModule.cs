using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Gyoujimaru.Data;

namespace Gyoujimaru.Modules
{
    [Name("Debug Module")]
    [Summary("Commands to debug Gyoujimaru.")]
    [RequireOwner]
    public class DebugModule : ModuleBase<SocketCommandContext>
    {
        private readonly GyoujimaruContext _gyoujimaruContext;

        public DebugModule(GyoujimaruContext gyoujimaruContext)
        {
            _gyoujimaruContext = gyoujimaruContext;
        }

        private const string _echoDeleteDelimiter = "true";

        [Command("echo")]
        [Summary("Echoes a message. If the last word of the message is \"true\", then your command message will be deleted.")]
        public async Task Echo([Remainder] string message)
        {
            var split = message.Split(' ');

            if (split[^1] == _echoDeleteDelimiter)
            {
                await Context.Message.DeleteAsync();
                await ReplyAsync(message.Substring(0, message.Length - _echoDeleteDelimiter.Length));
                return;
            }

            await ReplyAsync(message);
        }
    }
}