namespace Shared.Models.Words
{
    public class Vocabulary
    {
        public VocabularyTopic Topic { get; set; }
        public List<TranslationModel> Translations { get; set; } = new List<TranslationModel>();
    }
}
