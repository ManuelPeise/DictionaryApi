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

                var cleanedDumpFileModel = GetCleanedDumpFileModel(dumpFileModel);

                // save updated dump file model with translations as json
                await _kakkiWordService.SaveKaikkiBackupJson(cleanedDumpFileModel);

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

        private KaikkiJsonModel GetCleanedDumpFileModel(KaikkiJsonModel dumpFileModel)
        {
            var model = new KaikkiJsonModel
            {
                TimeStamp = dumpFileModel.TimeStamp,
                Source = dumpFileModel.Source,
                Licence = dumpFileModel.Licence,
                Count = 0,
                Data = new List<KaikkiJsonDataModel>()
            };

            if (dumpFileModel?.Data == null || !dumpFileModel.Data.Any())
            {
                return model;
            }

            var processedEntries = new HashSet<string>(StringComparer.Ordinal);

            foreach (var set in dumpFileModel.Data)
            {
                if (set == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(set.PartOfSpeech) || string.IsNullOrEmpty(set.LanguageCode))
                {
                    continue;
                }

                if (set.Translations == null || !set.Translations.Any())
                {
                    continue;
                }

                var entryKey = $"{set.Word}|{set.PartOfSpeech}";
                if (!processedEntries.Add(entryKey))
                {
                    continue;
                }

                var sourceLanguage = GetLanguageTypeFromLanguageCode(set.LanguageCode);
                var cleanedTranslations = new List<WordTranslationModel>();

                foreach (var translation in set.Translations)
                {
                    if (translation == null || string.IsNullOrEmpty(translation.Word) || string.IsNullOrEmpty(translation.LanguageCode))
                    {
                        continue;
                    }

                    var translationLanguage = GetLanguageTypeFromLanguageCode(translation.LanguageCode);

                    cleanedTranslations.Add(new WordTranslationModel
                    {
                        Word = GetCapitalizedWord(translation.Word, set.PartOfSpeech, translationLanguage),
                        Ipa = translation.Ipa,
                        Sentence = translation.Sentence,
                        LanguageCode = translation.LanguageCode,
                        LanguageName = translation.LanguageName,
                        Synonyms = translation.Synonyms != null ? new List<string>(translation.Synonyms) : null
                    });
                }

                if (!cleanedTranslations.Any())
                {
                    continue;
                }

                model.Data.Add(new KaikkiJsonDataModel
                {
                    PartOfSpeech = set.PartOfSpeech,
                    Word = GetCapitalizedWord(set.Word, set.PartOfSpeech, sourceLanguage),
                    LanguageName = set.LanguageName,
                    LanguageCode = set.LanguageCode,
                    Ipa = set.Ipa,
                    Synonyms = set.Synonyms != null ? new List<string>(set.Synonyms) : null,
                    Translations = cleanedTranslations
                });
            }

            model.Count = model.Data.Count;
            return model;
        }

        private string GetCapitalizedWord(string word, string partOfSpeech, LanguageEnum language)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return word;
            }

            if (partOfSpeech == "proper noun" || (partOfSpeech == "noun" && language == LanguageEnum.German))
            {
                return word.Length == 1 ? char.ToUpperInvariant(word[0]).ToString() : char.ToUpperInvariant(word[0]) + word.Substring(1);
            }

            return word;
        }

        private void MapExtractionDataToRelatedTranslations(KaikkiJsonModel dumpModel, Dictionary<string, KaikkiJsonDataExtract> extractionData, string languageCode)
        {
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
            extractionData?.Clear();
        }

        private LanguageEnum GetLanguageTypeFromLanguageCode(string languageCode)
        {
            switch (languageCode)
            {
                case "en":
                    return LanguageEnum.English;
                case "de":
                    return LanguageEnum.German;
                case "da":
                    return LanguageEnum.Danish;
                default: return LanguageEnum.Unknown;
            }
        }
    }
}
