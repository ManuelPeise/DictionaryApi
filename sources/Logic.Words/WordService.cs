using Data.Database;
using Logic.Shared;
using Logic.Words.Interfaces;
using Shared.Enums;
using Shared.Models.KaikkiJsonModels;
using System.Diagnostics;

namespace Logic.Words
{
    public class WordService : IWordService
    {
        private readonly IKakkiWordService _kakkiWordService;
        private readonly Logger<WordService> _logger;

        public WordService(DatabaseContext databaseContext, IKakkiWordService kakkiWordService)
        {
            _kakkiWordService = kakkiWordService;
            _logger = new Logger<WordService>(databaseContext);
        }

        public async Task ExecuteWordService(WordServiceType type)
        {
            var timeStamp = DateTime.UtcNow;

            switch (type)
            {
                case WordServiceType.Kaikki:
                    await ExecuteKakkiWordService(timeStamp);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private async Task ExecuteKakkiWordService(DateTime timeStamp)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();

                var dumpFileModel = await _kakkiWordService.ExecuteDumpService(timeStamp);

                var germanWordExtraction = await _kakkiWordService.GetExtratctions(KaikkiExtractionTypeEnum.GermanExtractions, timeStamp, 1);

                var danishWordExtraction = await _kakkiWordService.GetExtratctions(KaikkiExtractionTypeEnum.DanishExtractions, timeStamp, 1);

                // map ipa and synonyms to translations
                MapExtractionDataToRelatedTranslations(dumpFileModel, germanWordExtraction, "de");
                MapExtractionDataToRelatedTranslations(dumpFileModel, danishWordExtraction, "da");

                // cleanup to free memory
                CleanupExtractions(germanWordExtraction);
                CleanupExtractions(danishWordExtraction);

                // load existing dump file model from json and update with new data
                var backupDumpModel = await _kakkiWordService.LoadKaikkiBackupFromJson();


                // TODO: Merge logic to avoid duplicates

                // save updated dump file model with translations as json
                await _kakkiWordService.SaveKaikkiBackupJson(dumpFileModel);

                stopWatch.Stop();
                var elapsedTime = $"Minutes: {stopWatch.Elapsed.TotalMinutes:F2}, Seconds: {stopWatch.Elapsed.TotalSeconds:F2}";

                await _logger.LogMessageAsync(
                    "Kaikki word service executed successfully.",
                    LogMessageTypeEnum.Info,
                    $"Dump file model: {dumpFileModel}. German extractions: {germanWordExtraction}. Danish extractions: {danishWordExtraction}. Elapsed time: {elapsedTime}");
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "One or more errors occures while executing Kaikki word service.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
            }
        }

        private void MapExtractionDataToRelatedTranslations(KaikkiJsonModel dumpModel, Dictionary<string, KaikkiJsonDataExtract> extractionData, string languageCode)
        {
            var languageType = languageCode == "de" ? LanguageTypeEnum.German : LanguageTypeEnum.Danish;

            dumpModel.Data.ForEach(dataSet =>
            {
                dataSet.Translations?.ForEach(translation =>
                {
                    if (translation.LanguageCode == languageCode && extractionData.TryGetValue(dataSet.Word, out var extraction))
                    {
                        translation.Ipa = extraction.Ipa;
                        translation.Synonyms = extraction.Synonyms;
                    }
                });
            });
        }

        private void CleanupExtractions(Dictionary<string, KaikkiJsonDataExtract>? extractionData)
        {
            extractionData = null;
        }
    }
}
