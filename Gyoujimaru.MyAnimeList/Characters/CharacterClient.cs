using System.Threading.Tasks;

namespace Gyoujimaru.MyAnimeList.Characters
{
    public class CharacterClient
    {
        private readonly CharacterDownloader _characterDownloader = new();
        
        public async Task<Character> GetCharacterFromUrl(string url)
        {
            var characterPage = await _characterDownloader.DownloadCharacterDocument(url);

            return new CharacterFactory(characterPage)
                .WithGrammaticalName()
                .WithSimpleName()
                .WithImageUrl()
                .Build();
        }
        
        public async Task<Character> GetCharacterFromId(int id)
        {
            return await GetCharacterFromUrl($"https://myanimelist.net/character/{id}/");
        }
    }
}