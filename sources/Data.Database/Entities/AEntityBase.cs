namespace Data.Database.Entities
{
    public abstract class AEntityBase
    {
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public Guid IdExternal { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
