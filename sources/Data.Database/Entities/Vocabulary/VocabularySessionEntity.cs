using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Database.Entities.Vocabulary
{
    public class VocabularySessionEntity : AEntityBase
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public VocabularyCategoryEntity Category { get; set; }
        public VocabularySessionType SessionType { get; set; }
        public ICollection<VocabularyEntity> Vocabularies { get; set; } = new List<VocabularyEntity>();
        public ICollection<VocabularySessionResultEntity> Results { get; set; } = new List<VocabularySessionResultEntity>();
        public ICollection<VocabularyProgressEntity> SessionProgress { get; set; } = new List<VocabularyProgressEntity>();
    }
}
