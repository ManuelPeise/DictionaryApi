using Data.Accessor.Interfaces;
using Data.Database;
using Logic.Import.Interfaces;
using Logic.Parsing.Interfaces;
using Logic.Parsing.Models;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Models.Import;
using Shared.Models.Vocabulary;
using Shared.Models.Words;

namespace Logic.Import
{
    public class VocabularyImporter : LogicBase, IVocabularyImporter
    {
        private const string WordColumn = "Word";
        private const string ArticleColumn = "Article";
        private const string LanguageColumn = "Language";
        private const string PartOfSpeechColumn = "PartOfSpeech";
        private const string SentenceColumn = "Sentence";
        private const string IpaColumn = "Ipa";

        private readonly Logger<VocabularyImporter> _logger;

        private readonly IKaikkiDumpFileParser _kaikkiDumpFileParser;
        private readonly ILibreTranslateClient _libreTranslateClient;

        public VocabularyImporter(
            DatabaseContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IKaikkiDumpFileParser kaikkiDumpFileParser,
            ILibreTranslateClient libreTranslateClient)
            : base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<VocabularyImporter>(dbContext);
            _kaikkiDumpFileParser = kaikkiDumpFileParser;
            _libreTranslateClient = libreTranslateClient;

        }

        public async Task<bool> ImportVocabularyFileAsync(VocabularyFileUpload fileUploadModel)
        {
            try
            {
                var currentUser = GetCurrentUser();

                await ImportFile(fileUploadModel);

                await UnitOfWork.SaveChangesAsync(currentUser.EmailAddress);

                await _logger.LogMessageAsync($"Vocabulary file {fileUploadModel.FileName} imported.", LogMessageTypeEnum.Info);

                return true;

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync("", LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);

                return false;
            }
        }

        private async Task ImportFile(VocabularyFileUpload fileUploadModel)
        {
            var kaikkiWordExtractions = await _kaikkiDumpFileParser.GetKaikkiWordDictionary(
                new List<TranslationEnum> { TranslationEnum.En, TranslationEnum.Da, TranslationEnum.De });

            var lines = ReadFileLines(fileUploadModel.File);

            if (!lines.Any())
            {
                await _logger.LogMessageAsync(
                    $"The file contains no valid lines to import.",
                    LogMessageTypeEnum.Warning);

                return;
            }

            var vocabularies = ParseLines(lines, GetMappedColums(lines.First()), fileUploadModel.SourceLanguage.ToString());

            var vocabularyTranslations = new List<Vocabulary>();

            foreach (var vocabulary in vocabularies)
            {
                vocabularyTranslations.Add(await GetVocabularyTranslation(fileUploadModel.Topic, fileUploadModel.SourceLanguage, fileUploadModel.Translations, vocabulary, kaikkiWordExtractions));
            }

            var store = new VocabularyStorage(UnitOfWork);

            await store.StoreVocabularies(vocabularyTranslations, fileUploadModel.Topic, fileUploadModel.SourceLanguage);
        }

        private List<string> ReadFileLines(List<byte> fileBytes)
        {
            var lines = new List<string>();
            using (var stream = new MemoryStream(fileBytes.ToArray()))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }

        private Dictionary<string, int> GetMappedColums(string headerRow)
        {
            var columnDefinition = GetColumnDefinition();

            var headerColumns = headerRow.Split(',').Select(c => c.Trim()).ToArray();

            foreach (var column in headerColumns)
            {
                if (columnDefinition.TryGetValue(column, out var entry))
                {
                    columnDefinition[column] = Array.IndexOf(headerColumns, column);
                }
            }

            return columnDefinition;
        }

        private Dictionary<string, int> GetColumnDefinition()
        {
            return new Dictionary<string, int>
            {
                { WordColumn, -1 },
                { ArticleColumn, -1  },
                { LanguageColumn, -1  },
                { PartOfSpeechColumn, -1 },
                { SentenceColumn, -1 }
            };
        }

        private List<VocabularyImportWordModel> ParseLines(List<string> lines, Dictionary<string, int> columnMapping, string fallbackLanguage)
        {
            var vocabularyEntries = new List<VocabularyImportWordModel>();

            foreach (var line in lines.Skip(1))
            {
                var columns = line.Split(',');

                var language = (TranslationEnum)Enum.Parse(typeof(TranslationEnum), columns[columnMapping[LanguageColumn]] ?? fallbackLanguage);

                var wordModel = new VocabularyImportWordModel
                {
                    Word = columns[columnMapping[WordColumn]].Trim(),
                    Article = columns[columnMapping[ArticleColumn]].Trim(),
                    Language = language,
                    PartOfSpeech = GetNormalizedPartOfSpeech(columns[columnMapping[PartOfSpeechColumn]].Trim()),
                    ExampleSentence = columns[columnMapping[SentenceColumn]].Trim(),
                    Ipa = string.Empty,
                    Translations = new List<VocabularyImportWordModel>(),

                };

                vocabularyEntries.Add(wordModel);
            }
            return vocabularyEntries;
        }

        private string GetNormalizedPartOfSpeech(string partOfSpeech)
        {
            switch (partOfSpeech.ToLowerInvariant())
            {
                case "adj":
                case "adjective":
                case "adjektiv":
                    return "adjective";
                case "adv":
                case "adverb":
                    return "adverb";
                case "article":
                case "artikel":
                    return "article";
                case "noun":
                case "nomen":
                case "substantiv":
                    return "noun";
                case "verb":
                    return "verb";
                case "preposition":
                case "präposition":
                    return "preposition";
                case "pronoun":
                case "pronomen":
                    return "pronoun";
                case "proper noun":
                case "eigenname":
                    return "proper noun";
                default:
                    return string.Empty;
            }
        }

        private async Task<TranslationModel?> GetTranslationModel(
            string? word,
            string partOfSpeech,
            string sentence,
            string? article,
            TranslationEnum sourceLanguage,
            TranslationEnum targetLanguage,
            Dictionary<KaikkiKey, TranslationJsonModel> kaikkiModels)
        {
            if (string.IsNullOrEmpty(word))
            {
                return null;
            }

            var model = new TranslationModel
            {
                Ipa = string.Empty,
                PartOfSpeech = partOfSpeech,
                Language = targetLanguage
            };

            var wordIncudingArticke = ($"{article} {word}").ToLower().Trim();

            var translatedWord = await GetLibeTranslation(word, sourceLanguage, targetLanguage);

            if (kaikkiModels.TryGetValue(new KaikkiKey { Word = wordIncudingArticke ?? string.Empty, Language = targetLanguage }, out var kaikkiModel))
            {
                model.Word = kaikkiModel.Word;
                model.Ipa = kaikkiModel.Ipa;
            }

            var translatedWordParts = translatedWord?.Split(" ")?.ToList() ?? new List<string> { string.Empty };

            model.Word = translatedWordParts.Any() && translatedWordParts.Count > 1 ? translatedWordParts[1] : translatedWordParts[0];
            model.Article = await GetLibeTranslation(article, sourceLanguage, targetLanguage) ?? string.Empty;
            model.Sentence = await GetLibeTranslation(sentence, sourceLanguage, targetLanguage) ?? string.Empty;

            return model;
        }



        private async Task<string?> GetLibeTranslation(string? value, TranslationEnum sourceLanguage, TranslationEnum targetLanguage)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var translatedWord = await _libreTranslateClient.TranslateWord(value, sourceLanguage.ToString(), targetLanguage.ToString());


            return translatedWord;
        }

        private TranslationJsonModel? GetKaikkiEntry(string word, TranslationEnum language, string normalizedPartOfSpeech, Dictionary<KaikkiKey, TranslationJsonModel> kaikkiWordDictionary)
        {
            if (kaikkiWordDictionary.TryGetValue(new KaikkiKey { Word = word.ToLower(), Language = language }, out var entry))
            {
                return entry;
            }

            return null;
        }


        private async Task<Vocabulary> GetVocabularyTranslation(
            string topic,
            TranslationEnum sourceLanguage,
            List<TranslationEnum> translationLanguages,
            VocabularyImportWordModel model,
            Dictionary<KaikkiKey, TranslationJsonModel> kaikkiWordDictionary)
        {
            var vocabularyTranslation = new Vocabulary
            {
                Topic = new VocabularyTopic { Name = topic, Language = sourceLanguage },
                Translations = new List<TranslationModel>()
            };

            var kaikkiEntry = GetKaikkiEntry(model.Word, sourceLanguage, GetNormalizedPartOfSpeech(model.PartOfSpeech), kaikkiWordDictionary);

            vocabularyTranslation.Translations.Add(new TranslationModel
            {
                Word = model?.Word ?? string.Empty,
                Article = model?.Article ?? string.Empty,
                Sentence = model?.ExampleSentence ?? string.Empty,
                PartOfSpeech = model?.PartOfSpeech ?? string.Empty,
                Language = sourceLanguage,
                Ipa = kaikkiEntry?.Ipa ?? string.Empty,
            });

            foreach (var translationType in translationLanguages)
            {
                var translationModel = await GetTranslationModel(
                    model.Word,
                    GetNormalizedPartOfSpeech(model.PartOfSpeech),
                    model.ExampleSentence,
                    model.Article,
                    sourceLanguage,
                    translationType,
                    kaikkiWordDictionary);

                if (translationModel != null)
                {
                    vocabularyTranslation.Translations.Add(translationModel);
                }
            }

            return vocabularyTranslation;
        }
    }
}
