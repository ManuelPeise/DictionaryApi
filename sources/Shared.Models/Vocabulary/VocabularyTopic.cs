using Shared.Enums;

namespace Shared.Models.Vocabulary
{
    public class VocabularyTopic
    {
        public string Name { get; set; } = string.Empty;
        public TranslationEnum Language { get; set; }
    }
}
