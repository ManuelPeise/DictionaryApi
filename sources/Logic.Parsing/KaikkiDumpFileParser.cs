using Data.Accessor.Interfaces;
using Data.Database;
using Logic.Parsing.Interfaces;
using Logic.Parsing.Models;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models;
using Shared.Models.KaikkiJsonModels;
using Shared.Models.Words;
using System.IO.Compression;
using System.Text.Json;

namespace Logic.Parsing
{
    public class KaikkiDumpFileParser : LogicBase, IKaikkiDumpFileParser
    {
        private readonly Logger<KaikkiDumpFileParser> _logger;
        private readonly string _requestUrl = "https://kaikki.org/dictionary/raw-wiktextract-data.jsonl.gz";
        private readonly FileSystemConfiguration _fileSystemConfiguration;
        private readonly HttpClient _httpClient;

        public KaikkiDumpFileParser(
            DatabaseContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IOptions<FileSystemConfiguration> fileSystemConfiguration)
            : base(dbContext, httpContextAccessor, unitOfWork)
        {
            _logger = new Logger<KaikkiDumpFileParser>(dbContext);
            _httpClient = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
            _fileSystemConfiguration = fileSystemConfiguration.Value;
        }

        public async Task<Dictionary<KaikkiKey, TranslationJsonModel>> GetKaikkiWordDictionary(List<TranslationEnum> translations)
        {
            var dictionary = new Dictionary<KaikkiKey, TranslationJsonModel>();
            string json;

            using (var stream = new MemoryStream(Resx.Files.Kaikki))
            using (var reader = new StreamReader(stream))
            {
                json = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(json))
            {
                return dictionary;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonDictionary = JsonSerializer.Deserialize<Dictionary<string, TranslationJsonModel>>(json, options) ?? new Dictionary<string, TranslationJsonModel>();

            foreach (var key in jsonDictionary.Keys)
            {
                var deserializedKey = JsonSerializer.Deserialize<KaikkiKey>(key);

                if (deserializedKey != null)
                {
                    dictionary[deserializedKey] = jsonDictionary[key];
                }
            }

            return dictionary;
        }

        public async Task<TranslationJsonModel?> LoadWord(string word)
        {
            var dictionary = await GetKaikkiWordDictionary(new List<TranslationEnum>
            {
                TranslationEnum.De,
                TranslationEnum.En,
                TranslationEnum.Da
            });

            return dictionary.TryGetValue(new KaikkiKey { Word = word.ToLower(), Language = TranslationEnum.En }, out var entry) ? entry : null;
        }

        public async Task<bool> ParseFileStream(List<TranslationEnum> translations)
        {
            var dictionary = new Dictionary<string, TranslationJsonModel>();

            try
            {
                var languageCodes = translations.Select(translation => translation.ToString().ToLower()).ToList();
                using (var response = await new HttpClient().GetAsync(_requestUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    using (var streamReader = new StreamReader(gzipStream))
                    {

                        string? line;

                        while ((line = await streamReader.ReadLineAsync()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                continue;
                            }

                            var model = await ParseLine(line, languageCodes);

                            var translationModels = BuildTranslationModels(model, TranslationEnum.En);

                            foreach (var translationModel in translationModels)
                            {
                                if(!dictionary.TryGetValue(KaikkiParserUtils.GetKaikkiKey(translationModel), out _))
                                {
                                    dictionary[KaikkiParserUtils.GetKaikkiKey(translationModel)] = translationModel;
                                }
                            }
                        }
                    }

                    var fileName = "raw-wiktextract-data.json";
                    var directory = Path.Combine(AppContext.BaseDirectory, _fileSystemConfiguration.KaikkiJsonFolder);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var filePath = Path.Combine(directory, fileName);

                    if (File.Exists(filePath) && dictionary.Keys.Any())
                    {
                        File.Delete(filePath);
                    }

                    if (dictionary.Keys.Any())
                    {
                        var serialitzerOptions = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                        };

                        var json = JsonSerializer.Serialize(dictionary, serialitzerOptions);

                        File.WriteAllText(filePath, json);
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Parsing of Kaikki dump file failed",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);

                return false;
            }
        }

        private List<TranslationJsonModel> BuildTranslationModels(KaikkiModel? model, TranslationEnum defaultLanguage)
        {
            var models = new List<TranslationJsonModel>
            {
                new TranslationJsonModel
                {
                    Word = model?.Word ?? string.Empty,
                    PartOfSpeech = model?.PartOfSpeech ?? string.Empty,
                    Ipa = model?.Sounds?.FirstOrDefault()?.Ipa ?? string.Empty,
                    Language = defaultLanguage,
                    LanguageCode = model?.LanguageCode ?? string.Empty,
                    
                }
            };

            model?.Translations.ForEach(translation =>
            {
                if (translation.LanguageCode != null && Enum.TryParse<TranslationEnum>(translation.LanguageCode, true, out var languageEnum))
                {
                    models.Add(new TranslationJsonModel
                    {
                        Word = translation.Word,
                        PartOfSpeech = model?.PartOfSpeech ?? string.Empty,
                        Ipa = string.Empty,
                        Language = languageEnum,
                        LanguageCode = translation.LanguageCode
                    });
                }
            });

            return models;
        }

        private async Task<KaikkiModel?> ParseLine(string line, List<string> languageCodes)
        {
            try
            {
                var model = JsonSerializer.Deserialize<KaikkiModel>(line);

                if (model == null || model.Translations == null || !model.Translations.Any())
                {
                    return null;
                }

                if (!languageCodes.Contains(model?.LanguageCode?.ToLower() ?? string.Empty))
                {
                    return null;
                }

                model?.Translations = model.Translations
                    .Where(translation => translation.LanguageCode != null && languageCodes.Contains(translation.LanguageCode.ToString().ToLower()))
                    .DistinctBy(t => t.LanguageCode)
                    .DistinctBy(t => t.NormalizedWord)
                    .ToList();

                model?.Forms = model.Forms != null ? model.Forms
                    .Where(f => !string.IsNullOrEmpty(f.Form))
                    .ToList()
                    : new List<KaikkiForm>();

                model?.Sounds = model.Sounds
                    .Where(s => !string.IsNullOrEmpty(s.Ipa))
                    .Select(s => new KaikkiSounds
                    {
                        Ipa = $"[{s.Ipa?.Normalize().Trim('/')}]",
                        Tags = s.Tags.Where(tag => tag != null).ToList()
                    }).ToList();

                return model;
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Parsing of line in Kaikki dump file failed",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);

                return null;
            }
        }

    }
}
