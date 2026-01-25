using Data.Database.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Globalization;


namespace Data.Database.Seeds
{
    public class UserCredentialsSeed : IEntityTypeConfiguration<UserCredentialsEntity>
    {
        private const string PasswortHash = "$2a$12$ZUdEbhrrKfyY2zomnIEXXOUVtIQ6J8VeWFk40bcGtceHfRG5cBlVC";
        public void Configure(EntityTypeBuilder<UserCredentialsEntity> builder)
        {
            var timeStamp = DateTime.Parse("2026.01.01", CultureInfo.InvariantCulture);

           
            builder.HasData(new UserCredentialsEntity
            {
                Id = 1,
                PasswordHash = PasswortHash,
                CreatedAt = timeStamp,
                CreatedBy = "System"
            });
        }
    }
}
