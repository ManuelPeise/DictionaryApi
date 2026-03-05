using Logic.Parsing.Models;
using Shared.Enums;
using Shared.Models.Words;

namespace Logic.Parsing.Interfaces
{
    public interface IKaikkiDumpFileParser
    {
        Task<bool> ParseFileStream(List<TranslationEnum> translations);
        Task<Dictionary<KaikkiKey, TranslationJsonModel>> GetKaikkiWordDictionary(List<TranslationEnum> translations);
        Task<TranslationJsonModel?> LoadWord(string word);
    }
}
