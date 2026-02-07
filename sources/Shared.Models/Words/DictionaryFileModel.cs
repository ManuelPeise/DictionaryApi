using Shared.Enums;

namespace Shared.Models.Words
{
    public class DictionaryFileModel
    {
        public LanguageTypeEnum LanguageType { get; set; }
        public string Dict { get; set; }
        public string Aff { get; set; }
    }
}
