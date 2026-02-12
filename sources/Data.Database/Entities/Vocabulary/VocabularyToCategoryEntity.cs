
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Database.Entities.Vocabulary
{
    public class VocabularyToCategoryEntity : AEntityBase
    {
        public int VocabularyId { get; set; }
        [ForeignKey(nameof(VocabularyId))]
        public VocabularyEntity Vocabulary { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public VocabularyCategoryEntity Category { get; set; }
    }
}
