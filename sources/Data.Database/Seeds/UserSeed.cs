using Data.Database.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Enums;
using System.Globalization;


namespace Data.Database.Seeds
{
    internal class UserSeed : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            var timeStamp = DateTime.Parse("2026.01.01", CultureInfo.InvariantCulture);

            builder.HasData(new UserEntity
            {
                Id = 1,
                IdExternal = new Guid("4fd79b86-8f6a-4202-9e6a-f556deb04da9"),
                FirstName = "Admin",
                LastName = "User",
                EmailAddress = "admin.user@app.com",
                DateOfBirth = DateTime.Parse("1980.04.20", CultureInfo.InvariantCulture),
                UserRole = UserRoleEnum.Admin,
                CreatedAt = timeStamp,
                CreatedBy = "System",
                UserCredentialsId = 1,
                UserSettingsId = 1
            });
        }
    }
}
