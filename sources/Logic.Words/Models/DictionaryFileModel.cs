using Shared.Enums;

namespace Logic.Words.Models
{
    public class DictionaryFileModel
    {
        internal LanguageTypeEnum LanguageType { get; set; }
        internal string Dict { get; set; }
        internal string Aff { get; set; }
    }
}
