using System.Text.RegularExpressions;

namespace Gyoujimaru.MyAnimeList.Characters
{
    internal class CharacterFactory
    {
        private readonly CharacterPage _characterPage;
        private readonly Character _character = new();

        internal CharacterFactory(CharacterPage characterPage)
        {
            _characterPage = characterPage;
            _character.Id = int.Parse(_characterUrlRegex.Match(_characterPage.Url).Groups["id"].Value);
        }
        
        private readonly Regex _characterUrlRegex = new(@"(https://)?(myanimelist.net/character/)(?<id>[1-9]+)/([a-zA-z]+)?");

        internal CharacterFactory WithGrammaticalName()
        {
            _character.GrammaticalEnglishName = _characterPage.GetGrammaticalName();
            return this;
        }
        
        internal CharacterFactory WithSimpleName()
        {
            _character.CompleteName = _characterPage.GetSimpleName();
            return this;
        }

        internal CharacterFactory WithImageUrl()
        {
            _character.ImageUrl = _characterPage.GetImageUrl();
            return this;
        }
        
        internal Character Build()
        {
            return _character;
        }
    }
}