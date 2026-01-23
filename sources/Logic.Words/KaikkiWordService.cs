using Logic.Words.Interfaces;
using Shared.Enums;
using Shared.Models.Kaikki;
using Shared.Models.KaikkiJsonModels;
using System.Globalization;
using System.IO.Compression;
using System.Text.Json;

namespace Logic.Words
{
    public class KaikkiWordService : IKakkiWordService
    {
        public KaikkiWordService()
        {
        }

        public async Task<KaikkiJsonModel> ExecuteDumpService(DateTime timeStamp, int fileDateMonthOffset = 1)
        {
            var model = new KaikkiJsonModel
            {
                TimeStamp = timeStamp,
                Data = new List<KaikkiJsonDataModel>()
            };

            try
            {
                EnsureBackupDirectoryExists();

                var backupFiles = (from file in Directory.GetFiles(GetBackupFolderPath(), "*.json")
                                   let fileNameParts = Path.GetFileNameWithoutExtension(file).Split('_')
                                   group file by new { Type = fileNameParts[0], Date = GetParsedDate(fileNameParts.Last()) } into fileGroup
                                   where fileGroup.Key.Type == Constants.DumpFilePrefix
                                   select new
                                   {
                                       FileDate = fileGroup.Key.Date,
                                       FileFullPath = fileGroup.First()
                                   }).ToList();

                if (backupFiles.Any())
                {
                    var threshold = timeStamp.AddMonths(-fileDateMonthOffset);
                    var recentBackup = backupFiles.Where(x => x.FileDate >= threshold).OrderByDescending(x => x.FileDate).FirstOrDefault();

                    if (recentBackup != null)
                    {
                        return await LoadJsonModel<KaikkiJsonModel>(recentBackup.FileFullPath) ?? new KaikkiJsonModel();
                    }
                }

                var spellCheckers = SpellCheckerFactory.GetSpellCheckers();

                using (var client = new HttpClient())
                using (var response = await client.GetAsync(Constants.KaikkiDumpFileUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(new GZipStream(responseStream, CompressionMode.Decompress), bufferSize: 65536))
                    {
                        var models = new Dictionary<string, KaikkiJsonDataModel>();

                        string? line;

                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                continue;
                            }

                            var currentModel = ParseDumpFileLine(line, true, spellCheckers);

                            if (currentModel == null)
                            {
                                continue;
                            }

                            if (!models.ContainsKey(currentModel.Word))
                            {
                                models[currentModel.Word] = currentModel;
                            }
                        }

                        model.Data = models.Values.ToList();
                        model.Count = model.Data.Count;

                        await SaveJson(
                            model,
                            Constants.KaikkiFileNameTemplate
                                .Replace("{Type}", Constants.DumpFilePrefix)
                                .Replace("{TimeStamp}", timeStamp.ToString(Constants.KaikkiBackupDateFormat, CultureInfo.InvariantCulture)));

                        return model;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Dictionary<string, KaikkiJsonDataExtract>> GetExtratctions(KaikkiExtractionTypeEnum extractionType, DateTime timeStamp, int fileDateMonthOffset)
        {
            var dictionary = new Dictionary<string, KaikkiJsonDataExtract>();

            try
            {
                EnsureBackupDirectoryExists();

                var backupFiles = (from file in Directory.GetFiles(GetBackupFolderPath(), "*.json")
                                   let fileNameParts = Path.GetFileNameWithoutExtension(file).Split('_')
                                   group file by new { Type = fileNameParts[0], Date = GetParsedDate(fileNameParts.Last()) } into fileGroup
                                   where fileGroup.Key.Type == extractionType.ToString()
                                   select new
                                   {
                                       FileDate = fileGroup.Key.Date,
                                       FileFullPath = fileGroup.First()
                                   }).ToList();

                if (backupFiles.Any())
                {
                    var threshold = timeStamp.AddMonths(-fileDateMonthOffset);
                    var recentBackup = backupFiles.Where(x => x.FileDate >= threshold).OrderByDescending(x => x.FileDate).FirstOrDefault();

                    if (recentBackup != null)
                    {
                        return await LoadJsonModel<Dictionary<string, KaikkiJsonDataExtract>>(recentBackup.FileFullPath) ?? dictionary;
                    }
                }

                var spellCheckers = SpellCheckerFactory.GetSpellCheckers();

                var requestUrl = GetExtractionRequestUrl(extractionType);

                if (string.IsNullOrWhiteSpace(requestUrl))
                {
                    return dictionary;
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

                return dictionary;
            }
            catch (Exception)
            {
                return dictionary;
            }
        }

        public async Task<KaikkiJsonModel?> LoadKaikkiBackupFromJson()
        {
            var folderPath = GetBackupFolderPath();
            var filePath = Path.Combine(folderPath, Constants.KaikkiJsonBackupFileName);

            if(File.Exists(filePath))
            {
                var model = await LoadJsonModel<KaikkiJsonModel>(filePath);
                if(model != null)
                {
                    return model;
                }
            }

            return null;
        }

        public async Task SaveKaikkiBackupJson(KaikkiJsonModel dumpFileModel)
        {
            await SaveJson(dumpFileModel, Constants.KaikkiJsonBackupFileName);
        }

        private LanguageTypeEnum GetLanguage(string languageCode)
        {
            switch (languageCode)
            {
                case "en":
                    return LanguageTypeEnum.English;
                case "da":
                    return LanguageTypeEnum.Danish;
                case "de":
                    return LanguageTypeEnum.German;
                default:
                    return LanguageTypeEnum.Unknown;
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
                    return "Adjectiv";
                case "adv":
                case "adverb":
                    return "Adverb";
                case "article":
                    return "Artikel",
                case "noun":
                    return "Nomen";
                case "verb":
                    return "Verb";
                case "preposition":
                    return "Präposition",
                case "pronoun":
                    return "Pronomen";
                case "proper noun":
                    return "Eigenname";
                default:
                    return string.Empty;
            }
        }

        private List<WordTranslationModel> GetTranslations(KaikkiModel? model, Dictionary<LanguageTypeEnum, SpellChecker>? spellCheckers = null)
        {
            var translations = new List<WordTranslationModel>();

            if (model?.Translations == null || !model.Translations.Any())
            {
                return new List<WordTranslationModel>();
            }

            foreach (var t in model.Translations)
            {
                if (!Constants.ValidLanguages.Contains(t?.LanguageCode ?? string.Empty))
                {
                    continue;
                }

                var languageType = GetLanguage(t?.LanguageCode ?? string.Empty);
                var spellChecker = spellCheckers != null ? spellCheckers[languageType] : null;

                if (spellChecker == null || !spellChecker.IsValidWord(t?.Word ?? string.Empty))
                {
                    continue;
                }

                translations.Add(new WordTranslationModel
                {
                    LanguageCode = t?.LanguageCode ?? string.Empty,
                    LanguageName = t?.LanguageName ?? string.Empty,
                    Word = t?.Word ?? string.Empty,
                });
            }

            return translations.DistinctBy(x => x.Word).ToList();
        }

        private KaikkiJsonDataModel? ParseDumpFileLine(string line, bool useTranslation, Dictionary<LanguageTypeEnum, SpellChecker>? spellCheckers = null)
        {
            var model = JsonSerializer.Deserialize<KaikkiModel>(line) ?? new KaikkiModel();

            var rootLanguage = GetLanguage(model?.LanguageCode ?? string.Empty);

            if (model == null)
            {
                return null;
            }

            if (!Constants.ValidPartsOfSpeech.Contains(model.PartOfSpeech ?? string.Empty))
            {
                return null;
            }

            if (model.LanguageCode != "en" ||
                model.Word == null ||
                model.Word.Length == 1 ||
                model.Word.All(char.IsUpper) ||
                spellCheckers != null &&
                spellCheckers.TryGetValue(rootLanguage, out var rootSpellChecker) && !rootSpellChecker.IsValidWord(model.Word))
            {
                return null;
            }

            return new KaikkiJsonDataModel
            {
                Word = model.Word,
                PartOfSpeech = GetNormalizedPartOfSpeech(model?.PartOfSpeech ?? string.Empty),
                Ipa = model?.Sounds?.Where(s => !string.IsNullOrWhiteSpace(s?.Ipa))
                    .Select(s => s.Ipa)
                    .Distinct()
                    .FirstOrDefault() ?? string.Empty,
                LanguageCode = model?.LanguageCode ?? string.Empty,
                LanguageName = model?.LanguageName ?? string.Empty,
                Translations = GetTranslations(model, spellCheckers),
                Synonyms = model?.Senses?.Where(s => s?.Synonyms != null)
                    .SelectMany(s => s.Synonyms!)
                    .Select(x => x.Word)
                    .Where(w => !string.IsNullOrWhiteSpace(w))
                    .Distinct()
                    .ToList() ?? new List<string>()
            };
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

        private void EnsureBackupDirectoryExists()
        {
            var folderPath = GetBackupFolderPath();

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        private string GetBackupFolderPath()
        {
            var folderPath = Constants.KaikkiBackupFolder.Replace("{RootPath}_", AppDomain.CurrentDomain.BaseDirectory);

            return folderPath;
        }

        private async Task SaveJson<T>(T model, string fileName)
        {
            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            var folderPath = GetBackupFolderPath();
            var filePath = Path.Combine(folderPath, fileName);
            await File.WriteAllTextAsync(filePath, json);
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
