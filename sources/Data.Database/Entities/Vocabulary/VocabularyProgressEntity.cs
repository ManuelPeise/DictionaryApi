using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Database.Entities.Vocabulary
{
    public class VocabularyProgressEntity : AEntityBase
    {
        public int UserId { get; set; }
        public int TimesSeen { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public int VocabularyId { get; set; }
        [ForeignKey(nameof(VocabularyId))]
        public VocabularyEntity Vocabulary { get; set; }
    }
}
