using Data.Database.Entities;
using Data.Database.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models.Settings;
using System.Globalization;

namespace Data.Database.Seeds
{
    internal class ScheduledTaskSeed : IEntityTypeConfiguration<ScheduledTaskEntity>
    {
        private readonly ApiSettings _apiSettings;
        public ScheduledTaskSeed(ApiSettings settings)
        {
            _apiSettings = settings;
        }

        public void Configure(EntityTypeBuilder<ScheduledTaskEntity> builder)
        {
            var timeStamp = DateTime.Parse("2026.01.01", CultureInfo.InvariantCulture);

            builder.HasData(
                new ScheduledTaskEntity
                {
                    Id = 1,
                    Type = ScheduledTaskType.KaikkiWordService,
                    Message = "Load words from Kaikki.org",
                    Status = ScheduledTaskStatus.Pending,
                    Interval = ScheduleInterval.None,
                    ScheduledFireTime = null,
                    LastFireTime = null,
                    CreatedAt = timeStamp,
                    CreatedBy = "System",
                    RequestUrl = $"{_apiSettings.ApiBaseUrl}WordService/Execute" 
                }
            );
        }
    }
}
