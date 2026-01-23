using Logic.Words.Models;
using WeCantSpell.Hunspell;

namespace Logic.Words
{
    public class SpellChecker
    {
        private readonly WordList _hunspell;

        public SpellChecker(DictionaryFileModel fileModel)
        {
            _hunspell = WordList.CreateFromFiles(fileModel.Dict, fileModel.Aff);
        }

        public Dictionary<string, bool> ValidateWords(IEnumerable<string> words)
        {
            return words.ToDictionary(w => w, w => _hunspell.Check(w));
        }

        public bool IsValidWord(string word)
        {

            return _hunspell.Check(word);
        }
    }
}
