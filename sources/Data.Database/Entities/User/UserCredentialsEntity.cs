namespace Data.Database.Entities.User
{
    public class UserCredentialsEntity : AEntityBase
    {
        public string Salt { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? ApiKey { get; set; }
    }
}
