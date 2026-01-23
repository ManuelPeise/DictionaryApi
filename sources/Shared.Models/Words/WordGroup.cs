namespace Shared.Models.Words
{
    public class WordGroup
    {
        public Guid WordGroupGuid { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<WordModel> Words { get; set; } = new List<WordModel>();
    }
}
