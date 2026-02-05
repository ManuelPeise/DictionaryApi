using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Database.Entities.Vocabulary
{
    public class VocabularyEntity : AEntityBase
    {
        public Guid VocabularyGuid { get; set; }
        public string Topic { get; set; } = string.Empty;
        public int PartOfSpeachId { get; set; }
        [ForeignKey(nameof(PartOfSpeachId))]
        public PartOfSpeachEntity PartOfSpeach { get; set; }
        public ICollection<TranslationEntity> Translations { get; set; } = new List<TranslationEntity>();
    }
}
