using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Database.Entities.Vocabulary
{
    public class VocabularySessionResultEntity: AEntityBase
    {
        public int UserId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public int SessionId { get; set; }
        [ForeignKey(nameof(SessionId))]
        public VocabularySessionEntity Session { get; set; }
    }
}
