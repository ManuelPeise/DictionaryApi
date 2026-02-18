using Logic.Parsing.Models;
using Shared.Enums;

namespace Logic.Parsing.Interfaces
{
    public interface IKaikkiDumpFileParser
    {
        Task ParseFileStream(List<TranslationEnum> translations);
        Task<Dictionary<KaikkiKey, List<KaikkiModel>>> GetKaikkiWordDictionary(List<TranslationEnum> translations);
    }
}
