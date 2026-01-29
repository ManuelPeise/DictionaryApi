using Logic.Words.Models;
using Shared.Enums;

namespace Logic.Words
{
    internal static class SpellCheckerFactory
    {
        private static string _directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SpellChecker");

        internal static Dictionary<LanguageTypeEnum, SpellChecker> GetSpellCheckers(string directory, List<LanguageTypeEnum>? languages = null)
        {
            var dictionary = new Dictionary<LanguageTypeEnum, SpellChecker>();
            var fileModels = GetFileModels(directory, languages);

            foreach (var key in fileModels.Keys)
            {
                if (!dictionary.TryGetValue(key, out var _))
                {
                    var fileModel = fileModels[key];

                    dictionary.Add(key, new SpellChecker(fileModel));
                }
            }

            return dictionary;
        }


        private static Dictionary<LanguageTypeEnum, DictionaryFileModel> GetFileModels(string directory, List<LanguageTypeEnum>? languages = null)
        {
            var files = Directory.GetFiles(directory);

            var groups = (from file in files
                          let firstFileNamePart = Path.GetFileNameWithoutExtension(file).Split('_').First()
                          group file by firstFileNamePart into fileGroup
                          select new DictionaryFileModel
                          {
                              LanguageType = GetLanguageType(fileGroup.Key),
                              Dict = fileGroup?.FirstOrDefault(x => Path.GetExtension(x) == ".dic") ?? string.Empty,
                              Aff = fileGroup?.FirstOrDefault(x => Path.GetExtension(x) == ".aff") ?? string.Empty
                          });

            if(languages != null && languages.Any())
            {
                groups = groups.Where(x => languages.Contains(x.LanguageType));
            }

            return groups.ToDictionary(x => x.LanguageType, x => x);
        }

        private static LanguageTypeEnum GetLanguageType(string key)
        {
            switch (key)
            {
                case "de": return LanguageTypeEnum.German;
                case "en": return LanguageTypeEnum.English;
                case "da": return LanguageTypeEnum.Danish;
                default: throw new ArgumentOutOfRangeException(key);
            }
        }
    }
}
