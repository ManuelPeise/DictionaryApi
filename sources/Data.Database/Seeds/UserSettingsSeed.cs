using Data.Database.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Enums;
using System.Globalization;

namespace Data.Database.Seeds
{
    internal class UserSettingsSeed : IEntityTypeConfiguration<UserSettingsEntity>
    {
        public void Configure(EntityTypeBuilder<UserSettingsEntity> builder)
        {
            var timeStamp = DateTime.Parse("2026.01.01", CultureInfo.InvariantCulture);

            builder.HasData(
                new UserSettingsEntity
                {
                    Id = 1,
                    Culture = CultureEnum.English,
                    IsAutoDataSyncEnabled = true,
                    UseLocalDataStore = false,
                    CreatedAt = timeStamp,
                    CreatedBy = "System",
                }
            );
        }
    }
}
