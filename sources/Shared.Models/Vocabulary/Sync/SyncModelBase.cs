namespace Shared.Models.Vocabulary.Sync
{
    public class SyncModelBase
    {
        public Guid IdExternal { get; set; }
        public bool IsDirty { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
