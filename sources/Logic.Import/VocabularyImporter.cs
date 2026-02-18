using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities.Files;
using Logic.Import.Interfaces;
using Logic.Parsing.Interfaces;
using Logic.Parsing.Models;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models.Import;
using Shared.Models.Settings;
using Shared.Models.Vocabulary;
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
        private const string IpaColumn = "Ipa";

        private readonly Logger<VocabularyImporter> _logger;
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly IKaikkiDumpFileParser _kaikkiDumpFileParser;
        public VocabularyImporter(
            DatabaseContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IKaikkiDumpFileParser kaikkiDumpFileParser,
            IOptions<ApiSettings> options)
            : base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<VocabularyImporter>(dbContext);
            _kaikkiDumpFileParser = kaikkiDumpFileParser;
            _httpClient = new HttpClient();
            _apiSettings = options.Value;
        }

        public async Task<bool> ImportVocabularyFileAsync(VocabularyFileUpload fileUploadModel)
        {
            try
            {
                var currentUser = GetCurrentUser();

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

                var fileEntity = CreateImportFileEntity(fileUploadModel, false, true);

                await UnitOfWork.ImportFileRepository.AddAsync(fileEntity);

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
            var kaikkiWordExtractions = await _kaikkiDumpFileParser.GetKaikkiWordDictionary(
                new List<TranslationEnum> { TranslationEnum.En, TranslationEnum.Da, TranslationEnum.De });

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

        private async Task<TranslationModel> GetTranslations(
            TranslationModel translationModel,
            TranslationEnum sourceLanguage,
            TranslationEnum targetLanguage,
            Dictionary<KaikkiKey, List<KaikkiModel>> kaikkiWordDictionary)
        {
            var dictionary = new Dictionary<string, string>
            {
                { WordColumn, "" },
                { ArticleColumn, "" },
                { SentenceColumn, "" },
                { IpaColumn, ""  }
            };

            foreach (var key in dictionary.Keys)
            {
                if (key == WordColumn && !string.IsNullOrEmpty(translationModel.Word))
                {
                    var result = kaikkiWordDictionary.TryGetValue(new KaikkiKey { Word = translationModel.Word.ToLower(), Language = sourceLanguage }, out var entries);

                    dictionary[key] = result && entries != null && entries.Any() ?
                        entries.Where(entry => entry != null && !string.IsNullOrEmpty(entry.Word)).FirstOrDefault()?.Word ?? string.Empty :
                        await GetTranslation(translationModel.Word, sourceLanguage, targetLanguage);
                }
                else if (key == ArticleColumn && !string.IsNullOrEmpty(translationModel.Article))
                {
                    dictionary[key] = await GetTranslation(translationModel.Article, sourceLanguage, targetLanguage);
                }
                else if (key == SentenceColumn && !string.IsNullOrEmpty(translationModel.Sentence))
                {
                    dictionary[key] = await GetTranslation(translationModel.Sentence, sourceLanguage, targetLanguage);
                }
                else if (key == IpaColumn)
                {
                    var ipaResult = kaikkiWordDictionary.TryGetValue(new KaikkiKey { Word = translationModel.Word.ToLower(), Language = sourceLanguage }, out var entries);
                    dictionary[key] = ipaResult && entries != null && entries.Any() ?
                        entries.Where(entry => entry != null && !string.IsNullOrEmpty(entry.Word)).FirstOrDefault()?.Sounds.FirstOrDefault(s => s.Ipa != null)?.Ipa ?? string.Empty :
                        string.Empty;

                }
            }

            return new TranslationModel
            {
                Word = dictionary[WordColumn],
                Article = dictionary[ArticleColumn],
                Sentence = dictionary[SentenceColumn],
                PartOfSpeech = translationModel.PartOfSpeech,
                Language = targetLanguage,
                Ipa = dictionary[IpaColumn],
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
            Dictionary<KaikkiKey, List<KaikkiModel>> kaikkiWordDictionary)
        {
            var vocabularyTranslation = new Vocabulary
            {
                Topic = new VocabularyTopic { Name = file.Key, Language = file.SourceLanguage },
                Translations = new List<TranslationModel>()
            };

            kaikkiWordDictionary.TryGetValue(new KaikkiKey { Word = model.Word.ToLower(), Language = file.SourceLanguage }, out var entries);

            vocabularyTranslation.Translations.Add(new TranslationModel
            {
                Word = model.Word,
                Article = model.Article,
                Sentence = model.ExampleSentence,
                PartOfSpeech = model.PartOfSpeech,
                Language = file.SourceLanguage,
                Ipa = entries?.FirstOrDefault(e => e.Word.Equals(model.Word, StringComparison.InvariantCultureIgnoreCase))?.Sounds.FirstOrDefault(s => s.Ipa != null)?.Ipa ?? string.Empty
            });

            foreach (var translationType in file.Translations)
            {
                var translationWordModel = new TranslationModel
                {
                    Word = model.Word,
                    Article = model.Article,
                    Sentence = model.ExampleSentence,
                    PartOfSpeech = model.PartOfSpeech,
                    Language = translationType
                };

                var translationModel = await GetTranslations(translationWordModel, model.Language, translationType, kaikkiWordDictionary);

                vocabularyTranslation.Translations.Add(translationModel);
            }

            return vocabularyTranslation;
        }

        private ImportFileEntity CreateImportFileEntity(VocabularyFileUpload fileUploadModel, bool isPending, bool isCompleted)
        {

            var importFileEntity = new ImportFileEntity
            {
                FileName = fileUploadModel.FileName,
                FileBytes = fileUploadModel.File,
                Key = fileUploadModel.Topic,
                SourceLanguage = fileUploadModel.SourceLanguage,
                Translations = fileUploadModel.Translations,
                Status = isPending ? FileImportStatus.Pending : isCompleted ? FileImportStatus.Completed : FileImportStatus.Failed,
                IsImportedSuccessful = isCompleted
            };

            return importFileEntity;

        }
    }
}
