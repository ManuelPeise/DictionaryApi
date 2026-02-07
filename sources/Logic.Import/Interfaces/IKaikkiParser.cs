using Shared.Enums;
using Shared.Models.KaikkiJsonModels;

namespace Logic.Import.Interfaces
{
    public interface IKaikkiParser
    {
        Task<Dictionary<string, KaikkiJsonDataExtract>> ParseKaikkiJsonExtractionFile(KaikkiExtractionTypeEnum extractionType);
        Task ParseFileStream(List<KaikkiExtractionTypeEnum> extractionTypes);
    }
}
