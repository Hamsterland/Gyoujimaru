using System;

namespace Gyoujimaru.MyAnimeList.Characters
{
    public class Character
    {
        public int Id { get; internal set; }
        public string GrammaticalEnglishName { get; internal set; }
        public string CompleteName { get; internal set; }

        public string ImageUrl { get; internal set; }

        public string RomanisedName 
            => CompleteName
                .Substring(0, CompleteName.IndexOf("(", StringComparison.Ordinal))
                .Trim();

        public string JapaneseName
        {
            get
            {
                var from = CompleteName.IndexOf("(", StringComparison.Ordinal) + "(".Length;
                var to = CompleteName.LastIndexOf(")", StringComparison.Ordinal);
                return CompleteName.Substring(from, to - from);
            }
        }

        public string Url => $"https://myanimelist.net/character/{Id}";
    }
}