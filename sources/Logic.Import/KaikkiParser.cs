using Logic.Import.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models;
using Shared.Models.Kaikki;
using Shared.Models.KaikkiJsonModels;
using System.Globalization;
using System.Text.Json;

namespace Logic.Import
{
    public class KaikkiParser: IKaikkiParser
    {
        private readonly FileSystemConfiguration _fileSystemConfiguration;

        public KaikkiParser(IOptions<FileSystemConfiguration> fileSystemConfiguration)
        {
            _fileSystemConfiguration = fileSystemConfiguration.Value;
        }

        /// <summary>
        /// Asynchronously loads the most recent Kaikki.org JSON extraction backup file for the specified extraction type
        /// and parses its contents into a dictionary.
        /// </summary>
        /// <remarks>The method searches the backup directory for JSON files matching the specified extraction
        /// type, selects the most recent one by date, and parses its contents. The operation is performed asynchronously.
        /// The backup directory must exist and be accessible.</remarks>
        /// <param name="extractionType">The type of extraction to load. Specifies which set of backup files to search for and parse.</param>
        /// <returns>A dictionary mapping string keys to <see cref="KaikkiJsonDataExtract"/> objects, representing the parsed data
        /// from the most recent backup file for the specified extraction type. Returns an empty dictionary if the backup
        /// file is empty.</returns>
        /// <exception cref="FileNotFoundException">Thrown if no backup files are found or if no valid backup file exists for the specified extraction type.</exception>
        public async Task<Dictionary<string, KaikkiJsonDataExtract>> ParseKaikkiJsonExtractionFile(KaikkiExtractionTypeEnum extractionType)
        {
            var dictionary = new Dictionary<string, KaikkiJsonDataExtract>();

            try
            {
                EnsureBackupDirectoryExists();

                var backupFiles = (from file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileSystemConfiguration.KaikkiBackupFolder), "*.json")
                                   let fileNameParts = Path.GetFileNameWithoutExtension(file).Split('_')
                                   group file by new { Type = fileNameParts[0], Date = GetParsedDate(fileNameParts.Last()) } into fileGroup
                                   where fileGroup.Key.Type == extractionType.ToString()
                                   select new
                                   {
                                       FileDate = fileGroup.Key.Date,
                                       FileFullPath = fileGroup.First()
                                   }).ToList();

                if (!backupFiles.Any())
                {
                    throw new FileNotFoundException($"No backup files found for extraction type '{extractionType}'.");
                }

                var recentBackup = backupFiles.OrderByDescending(x => x.FileDate).FirstOrDefault();

                if (recentBackup != null)
                {
                    return await LoadJsonModel<Dictionary<string, KaikkiJsonDataExtract>>(recentBackup.FileFullPath) ?? dictionary;
                }

                throw new FileNotFoundException($"No valid backup files found for extraction type '{extractionType}'.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// called by scheduler
        /// Parses data streams for the specified extraction types and saves the extracted information as JSON files.
        /// </summary>
        /// <remarks>A separate JSON file is created for each extraction type in the list. The method
        /// downloads and processes data for each type, saving the results with a timestamped filename. If an extraction
        /// type does not have a valid request URL, it is skipped.</remarks>
        /// <param name="extractionTypes">A list of extraction types that determine which data streams to parse. Each type specifies a distinct
        /// extraction operation to perform.</param>
        /// <returns>A task that represents the asynchronous parse operation.</returns>
        public async Task ParseFileStream(List<KaikkiExtractionTypeEnum> extractionTypes)
        {
            var timeStamp = DateTime.UtcNow;

            try
            {
                EnsureBackupDirectoryExists();

                var spellCheckers = SpellCheckerFactory.GetSpellCheckers(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileSystemConfiguration.SpellCkeckerFolder));

                foreach (var extractionType in extractionTypes)
                {
                    var dictionary = new Dictionary<string, KaikkiJsonDataExtract>();

                    var requestUrl = GetExtractionRequestUrl(extractionType);

                    if (string.IsNullOrWhiteSpace(requestUrl))
                    {
                        continue;
                    }

                    using (var client = new HttpClient())
                    using (var response = await client.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        using (var reader = new StreamReader(responseStream, bufferSize: 65536))
                        {
                            string? line;
                            while ((line = await reader.ReadLineAsync()) != null)
                            {
                                if (string.IsNullOrWhiteSpace(line))
                                {
                                    continue;
                                }

                                var model = ParseExtractionLine(line, spellCheckers);

                                if (model != null && !dictionary.TryGetValue(model.Word, out var _))
                                {
                                    dictionary.Add(model.Word, model);
                                }
                            }
                        }
                    }

                    await SaveJson(
                        dictionary,
                        Constants.KaikkiFileNameTemplate
                            .Replace("{Type}", extractionType.ToString())
                            .Replace("{TimeStamp}", timeStamp.ToString(Constants.KaikkiBackupDateFormat, CultureInfo.InvariantCulture)));

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string? GetExtractionRequestUrl(KaikkiExtractionTypeEnum type)
        {
            switch (type)
            {
                case KaikkiExtractionTypeEnum.EnglishExtractions:
                    return "https://kaikki.org/dictionary/English/kaikki.org-dictionary-English.jsonl";
                case KaikkiExtractionTypeEnum.GermanExtractions:
                    return "https://kaikki.org/dictionary/German/words/kaikki.org-dictionary-German-words.jsonl";
                case KaikkiExtractionTypeEnum.DanishExtractions:
                    return "https://kaikki.org/dictionary/Danish/words/kaikki.org-dictionary-Danish-words.jsonl";
                default:
                    return string.Empty;
            }
        }

        private string GetNormalizedPartOfSpeech(string partOfSpeech)
        {
            switch (partOfSpeech.ToLowerInvariant())
            {
                case "adj":
                case "adjective":
                    return "adjective";
                case "adv":
                case "adverb":
                    return "adverb";
                case "article":
                    return "article";
                case "noun":
                    return "noun";
                case "verb":
                    return "verb";
                case "preposition":
                    return "preposition";
                case "pronoun":
                    return "pronoun";
                case "proper noun":
                    return "proper noun";
                default:
                    return string.Empty;
            }
        }

        private void EnsureBackupDirectoryExists()
        {
            var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileSystemConfiguration.KaikkiBackupFolder);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        private async Task SaveJson<T>(T model, string fileName)
        {
            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileSystemConfiguration.KaikkiBackupFolder);
            var filePath = Path.Combine(folderPath, fileName);
            await File.WriteAllTextAsync(filePath, json);
        }

        private KaikkiJsonDataExtract? ParseExtractionLine(string? line, Dictionary<LanguageTypeEnum, SpellChecker> spellCheckers)
        {
            var model = JsonSerializer.Deserialize<KaikkiModel>(line ?? string.Empty) ?? new KaikkiModel();

            var rootLanguage = GetLanguage(model?.LanguageCode ?? string.Empty);

            if (model == null)
            {
                return null;
            }

            if (!Constants.ValidPartsOfSpeech.Contains(model.PartOfSpeech ?? string.Empty))
            {
                return null;
            }

            if (model.Word == null ||
                model.Word.Length == 1 ||
                model.Word.All(char.IsUpper) ||
                spellCheckers != null &&
                spellCheckers.TryGetValue(rootLanguage, out var rootSpellChecker) && !rootSpellChecker.IsValidWord(model.Word))
            {
                return null;
            }

            return new KaikkiJsonDataExtract
            {
                Word = model.Word ?? string.Empty,
                PartOfSpeech = GetNormalizedPartOfSpeech(model?.PartOfSpeech ?? string.Empty),
                Ipa = model?.Sounds?.Where(s => !string.IsNullOrWhiteSpace(s.Ipa))
                    .Select(s => s.Ipa)
                    .Distinct()
                    .FirstOrDefault() ?? string.Empty,
                LanguageCode = model?.LanguageCode ?? string.Empty,
                Synonyms = model?.Senses?.Where(s => s?.Synonyms != null)
                    .SelectMany(s => s.Synonyms!)
                    .Select(x => x.Word)
                    .Where(w => !string.IsNullOrWhiteSpace(w) || w.Length == 1 || w.All(Char.IsUpper))
                    .Distinct()
                    .ToList() ?? new List<string>()
            };
        }

        private LanguageTypeEnum GetLanguage(string languageCode)
        {
            return languageCode.ToLowerInvariant() switch
            {
                "en" => LanguageTypeEnum.English,
                "de" => LanguageTypeEnum.German,
                "da" => LanguageTypeEnum.Danish,
                _ => LanguageTypeEnum.Unknown
            };
        }

        private async Task<T?> LoadJsonModel<T>(string filePath)
        {
            if (!File.Exists(filePath))
                return default;
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }

        private DateTime GetParsedDate(string dateString)
        {
            if (DateTime.TryParseExact(dateString, Constants.KaikkiBackupDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }

            throw new FormatException($"Date string '{dateString}' is not in the expected format '{Constants.KaikkiBackupDateFormat}'.");
        }
    }
}
