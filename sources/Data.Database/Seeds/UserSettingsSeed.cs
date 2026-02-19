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
                    IdExternal = new Guid("fade763a-f81c-4872-8e42-d31839406565"),
                    Culture = CultureEnum.English,
                    IsAutoDataSyncEnabled = true,
                    UseLocalDataStore = false,
                    IsDirty = false,
                    CreatedAt = timeStamp,
                    CreatedBy = "System",
                }
            );
        }
    }
}
