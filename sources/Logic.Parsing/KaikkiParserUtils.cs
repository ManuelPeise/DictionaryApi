

using Logic.Parsing.Models;
using Shared.Enums;
using System.Text.Json;

namespace Logic.Parsing
{
    internal static class KaikkiParserUtils
    {
        internal static TranslationEnum? MapLanguageCodeToTranslationEnum(string languageCode)
        {
            switch (languageCode.ToLower())
            {
                case "de":
                    return TranslationEnum.De;
                case "en":
                    return TranslationEnum.En;
                case "da":
                    return TranslationEnum.Da;
                default:
                    return null;
            }
        }

        internal static string GetKaikkiKey(KaikkiModel model)
        {
            return JsonSerializer.Serialize(new KaikkiKey
            {
                Word = model.NormalizedWord.ToLower(),
                Language = MapLanguageCodeToTranslationEnum(model.LanguageCode ?? string.Empty) ?? TranslationEnum.En
            });
        }
    }
}
