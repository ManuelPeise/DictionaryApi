using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities.Files;
using Logic.Import.Interfaces;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models.Import;
using Shared.Models.KaikkiJsonModels;
using Shared.Models.Settings;
using Shared.Models.Words;
using System.Text.Json;

namespace Logic.Import
{
    public class VocabularyImporter : LogicBase, IVocabularyImporter
    {
        private const string WordColumn = "Word";
        private const string ArticleColumn = "Article";
        private const string LanguageColumn = "Language";
        private const string PartOfSpeechColumn = "PartOfSpeech";
        private const string SentenceColumn = "Sentence";

        private readonly Logger<VocabularyImporter> _logger;
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly IKaikkiParser _kaikkiParser;
        private readonly IFileImporter _fileImporter;

        public VocabularyImporter(
            DatabaseContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IKaikkiParser kaikkiParser,
            IFileImporter fileImporter,
            IOptions<ApiSettings> options)
            : base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<VocabularyImporter>(dbContext);
            _kaikkiParser = kaikkiParser;
            _fileImporter = fileImporter;
            _httpClient = new HttpClient();
            _apiSettings = options.Value;
        }

        public async Task<bool> ImportVocabularyFileAsync(VocabularyFileUpload fileUploadModel)
        {
            try
            {
                var importFileEntity = new ImportFileEntity
                {
                    FileName = fileUploadModel.FileName,
                    FileBytes = fileUploadModel.File,
                    Key = fileUploadModel.Topic,
                    SourceLanguage = fileUploadModel.SourceLanguage,
                    Translations = fileUploadModel.Translations,
                    Status = FileImportStatus.Pending,
                    IsImportedSuccessful = false
                };

                await ImportPendingFiles(new List<ImportFileEntity> { importFileEntity });

                importFileEntity.Status = FileImportStatus.Completed;
                importFileEntity.IsImportedSuccessful = true;

                await _fileImporter.ImportVocabularyFileAsync(fileUploadModel, false, true);

                await _logger.LogMessageAsync($"Vocabulary file {fileUploadModel.FileName} imported.", LogMessageTypeEnum.Info);

                return true;

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync("", LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);

                return false;
            }
        }

        public async Task ImportVocabularyFilesAsync()
        {
            try
            {
                var pendingVocabularyFiles = await UnitOfWork.ImportFileRepository.GetAllByAsync(x => x.Status == FileImportStatus.Pending);

                if (!pendingVocabularyFiles.Any())
                {
                    await _logger.LogMessageAsync(
                        "No pending vocabulary files found for import.",
                        LogMessageTypeEnum.Info);
                    return;
                }

                if (!pendingVocabularyFiles.Any())
                {
                    return;
                }

                await ImportPendingFiles(pendingVocabularyFiles.ToList());

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "An error occurred while importing vocabulary files.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);
                throw;
            }
        }

        private async Task ImportPendingFiles(List<ImportFileEntity> pendingVocabularyFiles)
        {
            var kaikkiWordExtractions = await GetKaikkiExtractions(GetKaikkiExtractionTypes());

            foreach (var file in pendingVocabularyFiles)
            {
                var lines = ReadFileLines(file.FileBytes);

                if (!lines.Any())
                {
                    await _logger.LogMessageAsync(
                        $"The file with ID {file.Id} contains no valid lines to import.",
                        LogMessageTypeEnum.Warning);

                    file.Status = FileImportStatus.Failed;

                    continue;
                }

                var vocabularies = ParseLines(lines, GetMappedColums(lines.First()), file.SourceLanguage.ToString());

                var vocabularyTranslations = new List<Vocabulary>();

                foreach (var vocabulary in vocabularies)
                {
                    vocabularyTranslations.Add(await GetVocabularyTranslation(vocabulary, file, kaikkiWordExtractions));
                }

                var store = new VocabularyStorage(UnitOfWork);

                await store.StoreVocabularies(vocabularyTranslations, file.Key, file.SourceLanguage);
            }
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

        private List<KaikkiExtractionTypeEnum> GetKaikkiExtractionTypes()
        {
            var extractionDataTypes = new List<KaikkiExtractionTypeEnum>
            {
                KaikkiExtractionTypeEnum.EnglishExtractions,
                KaikkiExtractionTypeEnum.DanishExtractions,
                KaikkiExtractionTypeEnum.GermanExtractions
            };

            return extractionDataTypes.Distinct().ToList();
        }

        private async Task<Dictionary<TranslationEnum, Dictionary<string, KaikkiJsonDataExtract>>> GetKaikkiExtractions(List<KaikkiExtractionTypeEnum> extractionTypes)
        {
            var extractionDataDictionary = new Dictionary<TranslationEnum, Dictionary<string, KaikkiJsonDataExtract>>();

            foreach (var extractionType in extractionTypes)
            {
                if (extractionType == KaikkiExtractionTypeEnum.EnglishExtractions && !extractionDataDictionary.TryGetValue(TranslationEnum.En, out _))
                {
                    var englishData = await _kaikkiParser.ParseKaikkiJsonExtractionFile(KaikkiExtractionTypeEnum.EnglishExtractions);
                    extractionDataDictionary.Add(TranslationEnum.En, englishData);
                }
                else if (extractionType == KaikkiExtractionTypeEnum.GermanExtractions && !extractionDataDictionary.TryGetValue(TranslationEnum.De, out _))
                {
                    var germanData = await _kaikkiParser.ParseKaikkiJsonExtractionFile(KaikkiExtractionTypeEnum.GermanExtractions);
                    extractionDataDictionary.Add(TranslationEnum.De, germanData);
                }
                else if (extractionType == KaikkiExtractionTypeEnum.DanishExtractions && !extractionDataDictionary.TryGetValue(TranslationEnum.Da, out _))
                {
                    var danishData = await _kaikkiParser.ParseKaikkiJsonExtractionFile(KaikkiExtractionTypeEnum.DanishExtractions);
                    extractionDataDictionary.Add(TranslationEnum.Da, danishData);
                }
            }

            return extractionDataDictionary;
        }

        private async Task<TranslationModel> GetTranslations(
            TranslationModel translationModel,
            TranslationEnum sourceLanguage,
            TranslationEnum targetLanguage,
            Dictionary<string, KaikkiJsonDataExtract> extractionData)
        {
            var dictionary = new Dictionary<string, string>
            {
                { WordColumn, "" },
                { ArticleColumn, "" },
                { SentenceColumn, "" },
            };

            foreach (var key in dictionary.Keys)
            {
                if (key == WordColumn && !string.IsNullOrEmpty(translationModel.Word))
                {
                    dictionary[key] = await GetTranslation(translationModel.Word, sourceLanguage, targetLanguage);
                }
                else if (key == ArticleColumn && !string.IsNullOrEmpty(translationModel.Article))
                {
                    dictionary[key] = await GetTranslation(translationModel.Article, sourceLanguage, targetLanguage);
                }
                else if (key == SentenceColumn && !string.IsNullOrEmpty(translationModel.Sentence))
                {
                    dictionary[key] = await GetTranslation(translationModel.Sentence, sourceLanguage, targetLanguage);
                }

            }

            return new TranslationModel
            {
                Word = dictionary[WordColumn],
                Article = dictionary[ArticleColumn],
                Sentence = dictionary[SentenceColumn],
                PartOfSpeech = translationModel.PartOfSpeech,
                Language = targetLanguage,
                Ipa = extractionData.TryGetValue(dictionary[WordColumn] ?? string.Empty, out var dataExtract) ? dataExtract.Ipa : string.Empty
            };
        }

        private async Task<string> GetTranslation(string? value, TranslationEnum sourceLanguage, TranslationEnum targetLanguage)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var body = JsonSerializer.Serialize(new LibreTranslateRequestBody
            {
                Word = value,
                SourceLanguage = sourceLanguage.ToString(),
                TargetLanguage = targetLanguage.ToString(),
                Format = "text"
            });

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_apiSettings.LibreTranslateUrl}/translate"),
                Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(jsonContent))
            {
                return string.Empty;
            }

            var translationResult = JsonSerializer.Deserialize<LibreTranslation>(jsonContent);

            return translationResult?.TranslatedText ?? string.Empty;
        }

        private async Task<Vocabulary> GetVocabularyTranslation(
            VocabularyImportWordModel model,
            ImportFileEntity file,
            Dictionary<TranslationEnum, Dictionary<string, KaikkiJsonDataExtract>> kaikkiWordExtractions)
        {
            var vocabularyTranslation = new Vocabulary
            {
                Topic = new VocabularyTopic { Name = file.Key, Language = file.SourceLanguage },
                Translations = new List<TranslationModel>()
            };

            vocabularyTranslation.Translations.Add(new TranslationModel
            {
                Word = model.Word,
                Article = model.Article,
                Sentence = model.ExampleSentence,
                PartOfSpeech = model.PartOfSpeech,
                Language = file.SourceLanguage,
                Ipa = kaikkiWordExtractions.TryGetValue(file.SourceLanguage, out var extraction) &&
                      extraction.TryGetValue(model.Word, out var dataExtract) ?
                      dataExtract.Ipa :
                      string.Empty
            });

            foreach (var translationType in file.Translations)
            {
                if (!kaikkiWordExtractions.TryGetValue(translationType, out var extractionDictionary))
                {
                    continue;
                }

                var translationWordModel = new TranslationModel
                {
                    Word = model.Word,
                    Article = model.Article,
                    Sentence = model.ExampleSentence,
                    PartOfSpeech = model.PartOfSpeech,
                    Language = translationType
                };

                var translationModel = await GetTranslations(translationWordModel, model.Language, translationType, extractionDictionary);

                vocabularyTranslation.Translations.Add(translationModel);
            }

            return vocabularyTranslation;
        }
    }
}
