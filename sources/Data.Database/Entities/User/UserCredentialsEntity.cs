namespace Data.Database.Entities.User
{
    public class UserCredentialsEntity : AEntityBase
    {
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
