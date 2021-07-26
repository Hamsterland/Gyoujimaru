using System.Threading.Tasks;
using Discord.Commands;
using Gyoujimaru.Data;
using Gyoujimaru.MyAnimeList.Characters;
using Gyoujimaru.Preconditions;
using Gyoujimaru.Services.Olympics._2021;
using Gyoujimaru.Services.Olympics._2021.Tracker;
using Interactivity;

namespace Gyoujimaru.Modules
{
    [Name("Character Module")]
    [Summary("The majority of important Event commands.")]
    [RequireContext(ContextType.DM)]
    [BlockControl(BlockOptions.RequireUnblocked)]
    public partial class CharacterModule : ModuleBase<SocketCommandContext>
    {
        private readonly GyoujimaruContext _gyoujimaruContext;
        private readonly CharacterClient _characterClient;
        private readonly InteractivityService _interactivityService;
        private readonly CharacterService _characterService;
        private readonly TrackerService _trackerService;

        public CharacterModule(
            GyoujimaruContext gyoujimaruContext,
            CharacterClient characterClient,
            InteractivityService interactivityService, 
            CharacterService characterService, 
            TrackerService trackerService)
        {
            _gyoujimaruContext = gyoujimaruContext;
            _characterClient = characterClient;
            _interactivityService = interactivityService;
            _characterService = characterService;
            _trackerService = trackerService;
        }
        
        // [Command("info")]
        // [Summary("Shows information about your villain.")]
        // public async Task Info()
        // {
        //     var character = await _characterService.GetSubmission(Context.User.Id);
        //
        //     if (character is null)
        //     {
        //         await ReplyAsync("You don't have a claimed villain.");
        //         return;
        //     }
        //
        //     var infoEmbed = new CharacterInfoEmbedBuilder(character, Context.Client)
        //         .WithAllOptions()
        //         .Build();
        //     
        //     await ReplyAsync(embed: infoEmbed);
        // }
    }
}