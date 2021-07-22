using AngleSharp.Html.Dom;

namespace Gyoujimaru.MyAnimeList.Characters
{
    internal class CharacterPage
    {
        public IHtmlDocument HtmlDocument { get; }
        public string Url { get; }

        public CharacterPage(IHtmlDocument htmlDocument, string url)
        {
            HtmlDocument = htmlDocument;
            Url = url;
        }
    }
}