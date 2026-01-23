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
                UserIdExternal = new Guid("95f6f461-b8eb-48f8-aecf-78bbd0cc0c3d"),
                FirstName = "Admin",
                LastName = "User",
                EmailAddress = "admin.user@app.com",
                DateOfBirth = DateTime.Parse("1980.04.20", CultureInfo.InvariantCulture),
                UserRole = UserRoleEnum.Admin,
                CreatedAt = timeStamp,
                CreatedBy = "System",
                UserCredentialsId = 1
            });
        }
    }
}
