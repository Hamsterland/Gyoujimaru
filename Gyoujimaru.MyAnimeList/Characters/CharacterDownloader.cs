using System.Threading.Tasks;

namespace Gyoujimaru.MyAnimeList.Characters
{
    internal class CharacterDownloader : Downloader
    {
        public async Task<CharacterPage> DownloadCharacterDocument(string url)
        {
            var htmlDocument = await DownloadHtmlDocument(url);
            return new CharacterPage(htmlDocument, url);
        }
    }
}