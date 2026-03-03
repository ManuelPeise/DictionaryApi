using Data.Accessor.Interfaces;
using Data.Database;
using Logic.Parsing.Interfaces;
using Logic.Parsing.Models;
using Logic.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models;
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

        public async Task<Dictionary<KaikkiKey, List<KaikkiModel>>> GetKaikkiWordDictionary(List<TranslationEnum> translations)
        {
            var dictionary = new Dictionary<KaikkiKey, List<KaikkiModel>>();
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

            var jsonDictionary = JsonSerializer.Deserialize<Dictionary<string, List<KaikkiModel>>>(json) ?? new Dictionary<string, List<KaikkiModel>>();

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

        public async Task<bool> ParseFileStream(List<TranslationEnum> translations)
        {
            var dictionary = new Dictionary<string, List<KaikkiModel>>();

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

                            if (model != null && !KeyIsAlreadyIncludedInDictionary(model, dictionary))
                            {
                                dictionary[KaikkiParserUtils.GetKaikkiKey(model)] = new List<KaikkiModel> { model };
                            }
                            else if (model != null && KeyIsAlreadyIncludedInDictionary(model, dictionary))
                            {
                                dictionary[KaikkiParserUtils.GetKaikkiKey(model)].Add(model);
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

        private bool KeyIsAlreadyIncludedInDictionary(KaikkiModel model, Dictionary<string, List<KaikkiModel>> dictionary)
        {
            return dictionary.TryGetValue(KaikkiParserUtils.GetKaikkiKey(model), out _);

        }
    }
}
