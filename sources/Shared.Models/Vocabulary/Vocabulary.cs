using Shared.Models.Words;

namespace Shared.Models.Vocabulary
{
    public class Vocabulary
    {
        public VocabularyTopic Topic { get; set; }
        public List<TranslationModel> Translations { get; set; } = new List<TranslationModel>();
    }
}
