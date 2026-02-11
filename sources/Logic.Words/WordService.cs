using Data.Database;
using Data.Database.Entities.Vocabulary;
using Logic.Shared;
using Logic.Words.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shared.Enums;
using Shared.Models.KaikkiJsonModels;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Logic.Words
{
    public class WordService : IWordService
    {
        private readonly IKakkiWordService _kakkiWordService;
        private readonly IWordUnitOfWork _wordUnitOfWork;
        private readonly Logger<WordService> _logger;

        public WordService(DatabaseContext databaseContext, IKakkiWordService kakkiWordService, IWordUnitOfWork wordUnitOfWork)
        {
            _kakkiWordService = kakkiWordService;
            _wordUnitOfWork = wordUnitOfWork;
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

                var vocabularyEntities = await GetVocabularyEntities(dumpFileModel, germanWordExtraction, danishWordExtraction);

                await SaveVocabularies(vocabularyEntities);

                // cleanup to free memory
                CleanupExtractions(germanWordExtraction);
                CleanupExtractions(danishWordExtraction);

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

        private async Task<List<VocabularyEntity>> GetVocabularyEntities(
            KaikkiJsonModel model,
            Dictionary<string, KaikkiJsonDataExtract> germanWordExtraction,
            Dictionary<string, KaikkiJsonDataExtract> danishWordExtraction)
        {
            var entities = new List<VocabularyEntity>();

            foreach (var entry in model.Data)
            {
                var partOfSpeachId = await _wordUnitOfWork.GetPartOfSpeachId(entry.PartOfSpeech);

                if (string.IsNullOrEmpty(entry.PartOfSpeech) ||
                    string.IsNullOrEmpty(entry.LanguageCode) ||
                    partOfSpeachId == null ||
                    entry.Translations == null)
                {
                    continue;
                }

                var vocabularyGuid = Guid.NewGuid();

                var languageId = await _wordUnitOfWork.GetLanguageId(GetLanguageTypeFromLanguageCode(entry.LanguageCode));

                if (languageId == null)
                {
                    continue;
                }

                var translations = new List<VocabularyTranslationEntity>
                {
                    new VocabularyTranslationEntity
                    {
                        g = vocabularyGuid,
                        LanguageId = (int)languageId,
                        Word = GetCapitalizedWord(entry.Word, entry.PartOfSpeech, GetLanguageTypeFromLanguageCode(entry.LanguageCode)),
                        Ipa = entry.Ipa ?? string.Empty,
                        Synonyms = entry.Synonyms != null ? string.Join(", ", entry.Synonyms) : string.Empty
                    }
                };

                foreach (var t in entry.Translations)
                {
                    var extractions = GetExtractions(t.LanguageCode, germanWordExtraction, danishWordExtraction);
                    var translationEntity = await GetTranslationEntity(t, entry.PartOfSpeech, vocabularyGuid, extractions);

                    if (translationEntity != null)
                    {
                        translations.Add(translationEntity);
                    }
                }

                var entity = new VocabularyEntity
                {
                    VocabularyGuid = vocabularyGuid,
                    Topic = string.Empty,
                    PartOfSpeachId = (int)partOfSpeachId,
                    Translations = translations ?? new List<TranslationEntity>()
                };


                if (entity.Translations.Any())
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        private Dictionary<string, KaikkiJsonDataExtract> GetExtractions(
            string? languageCode,
            Dictionary<string, KaikkiJsonDataExtract> germanWordExtraction,
            Dictionary<string, KaikkiJsonDataExtract> danishWordExtraction)
        {
            var normalizedCode = languageCode?.Trim().ToLowerInvariant();

            switch (normalizedCode)
            {
                case "de":
                    return germanWordExtraction;
                case "da":
                    return danishWordExtraction;
                default:
                    return new Dictionary<string, KaikkiJsonDataExtract>();
            }
        }

        private string GetCapitalizedWord(string? word, string partOfSpeech, LanguageEnum language)
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

        private async Task<VocabularyTranslationEntity?> GetTranslationEntity(
            WordTranslationModel? model,
            string partOfSpeach,
            Guid vocabularyGuid,
            Dictionary<string, KaikkiJsonDataExtract> wordExtraction)
        {
            var extractionData = IsValidLanguageModel(model, wordExtraction);

            if (model == null || extractionData == null)
            {
                return null;
            }

            var ipa = extractionData?.Ipa ?? model.Ipa ?? string.Empty;

            var languageId = await _wordUnitOfWork.GetLanguageId(GetLanguageTypeFromLanguageCode(model.LanguageCode));

            if (languageId == null)
            {
                return null;
            }

            return new TranslationEntity
            {
                VocabularyGuid = vocabularyGuid,
                LanguageId = (int)languageId,
                Word = GetCapitalizedWord(model.Word, partOfSpeach, GetLanguageTypeFromLanguageCode(model.LanguageCode)),
                Ipa = ipa,
                Sentence = string.Empty,
                Synonyms = extractionData?.Synonyms != null ? string.Join(", ", extractionData.Synonyms) : string.Empty
            };
        }

        private KaikkiJsonDataExtract? IsValidLanguageModel(
            WordTranslationModel? model,
            Dictionary<string, KaikkiJsonDataExtract> wordExtraction)
        {
            if (model == null || string.IsNullOrEmpty(model.Word) || string.IsNullOrEmpty(model.LanguageCode))
            {
                return null;
            }

            wordExtraction.TryGetValue(model.Word, out var extractionData);

            return extractionData;
        }

        private async Task SaveVocabularies(List<VocabularyEntity> vocabularyEntities)
        {
            var wordList = await GetWordList();

            var entitiesToAdd = vocabularyEntities
                .Where(e =>
                {
                    var translationsToAdd = e.Translations.Where(t => !wordList.Contains(t.Word)).ToList();

                    if (!translationsToAdd.Any())
                    {
                        return false;
                    }

                    e.Translations = translationsToAdd;

                    return true;
                }).ToList();

            await _wordUnitOfWork.VocabularyRepository.AddRangeAsync(entitiesToAdd);

            await _wordUnitOfWork.SaveChangesAsync("System");
        }

        public async Task<List<string>> GetWordList()
        {
            var vocabularyList = await _wordUnitOfWork.TranslationRepository.GetAllAsync();

            return vocabularyList.Select(t => t.Word).ToList();
        }

        private void CleanupExtractions(Dictionary<string, KaikkiJsonDataExtract>? extractionData)
        {
            extractionData?.Clear();
        }

        private LanguageEnum GetLanguageTypeFromLanguageCode(string? languageCode)
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
