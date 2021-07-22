using System.Linq;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;

namespace Gyoujimaru.MyAnimeList.Characters
{
    internal static class CharacterParser
    {
        public static string GetGrammaticalName(this CharacterPage characterPage)
        {
            return characterPage
                .HtmlDocument
                .QuerySelector("#contentWrapper > div:nth-child(1) > div > div.h1-title > h1")
                .TextContent;
        }

        public static string GetSimpleName(this CharacterPage characterPage)
        {
            var name = characterPage
                .HtmlDocument
                .QuerySelectorAll("#content > table > tbody > tr > td:nth-child(2) > h2.normal_header")
                .FirstOrDefault()
                .TextContent;

            return name;
        }

        public static string GetImageUrl(this CharacterPage characterPage)
        {
            var image = characterPage
                .HtmlDocument
                .QuerySelector("#content > table > tbody > tr > td.borderClass > div:nth-child(1) > a > img")?
                .GetAttribute("data-src");

            return image;
        }
    }
}