using Shared.Enums;
using Shared.Models.KaikkiJsonModels;

namespace Logic.Words.Interfaces
{
    public interface IKakkiWordService
    {
        Task<KaikkiJsonModel> ExecuteDumpService(DateTime timeStamp, int fileDateMonthOffset = 1);
        Task<Dictionary<string, KaikkiJsonDataExtract>> GetExtratctions(KaikkiExtractionTypeEnum extractionType, DateTime timeStamp, int fileDateMonthOffset);
        Task<KaikkiJsonModel?> LoadKaikkiBackupFromJson();
        Task SaveKaikkiBackupJson(KaikkiJsonModel dumpFileModel);
    }
}
