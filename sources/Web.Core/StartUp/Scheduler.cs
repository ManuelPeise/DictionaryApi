using Data.Database;
using Microsoft.Extensions.Options;
using Quartz;
using Shared.Enums;
using Shared.Models.Settings;

namespace Web.Core.StartUp
{
    public class Scheduler
    {
        public static async Task StartScheduler(IServiceProvider services)
        {
            var schedulerFactory = services.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();

            var configuration = services.GetRequiredService<IConfiguration>();

            var apiBaseUrl = configuration.GetSection("ApiSettings").Get<ApiSettings>()?.ApiBaseUrl;

            var currentDateTime = DateTime.UtcNow;

            await AddJob(scheduler, "Scheduler service", "Scheduler service",
                new JobDataMap
                {
                    { "Url", $"{apiBaseUrl}TaskSchedule/ExecutePendingTasks" }
                },
                GetNextInterval(currentDateTime, 5),
                "0 0/5 * * * ?");

            // await scheduler.Start();
        }

        private static async Task AddJob(
           IScheduler scheduler,
           string key,
           string description,
           JobDataMap jobDataMap,
           DateTimeOffset start,
           string cronScheduleExpression)
        {
            var jobKey = new JobKey(key);
            var jobDetail = JobBuilder.Create<WebJob>()
                .WithIdentity(jobKey)
                .WithDescription(description)
                .SetJobData(jobDataMap)
                .Build();

            var trigger = GetJobTrigger(key, start, jobKey, cronScheduleExpression);

            await scheduler.ScheduleJob(jobDetail, trigger);
        }

        private static ITrigger GetJobTrigger(
            string key,
            DateTimeOffset start,
            JobKey jobKey,
            string cronScheduleExpression)
        {

            return TriggerBuilder.Create()
                .WithIdentity($"{key}Trigger")
                .StartAt(start)
                .WithCronSchedule(cronScheduleExpression)
                .ForJob(jobKey)
                .Build();
        }

        public static DateTimeOffset GetNextInterval(DateTimeOffset from, int intervalMinutes)
        {
            if (intervalMinutes <= 0 || intervalMinutes > 60)
                throw new ArgumentOutOfRangeException(nameof(intervalMinutes), "Interval must be between 1 and 60 minutes.");

            int minutes = from.Minute;
            int next = ((minutes / intervalMinutes) * intervalMinutes);
            if (minutes % intervalMinutes != 0)
                next += intervalMinutes;

            if (next >= 60)
            {
                from = from.AddHours(1);
                next = 0;
            }

            var result = new DateTimeOffset(
                from.Year,
                from.Month,
                from.Day,
                from.Hour,
                next,
                0,
                from.Offset
            );

            if (result <= from)
                result = result.AddMinutes(intervalMinutes);

            return result;
        }

        public class WebJob : IJob
        {
            private readonly Logic.Shared.Logger<WebJob> _logger;
            private readonly HttpClient _httpClient = new HttpClient();

            public WebJob(DatabaseContext dbContext)
            {
                _logger = new Logic.Shared.Logger<WebJob>(dbContext);
            }

            public async Task Execute(IJobExecutionContext context)
            {
                try
                {
                    var jobDataMap = context.MergedJobDataMap;
                    var url = jobDataMap.GetString("Url") ?? string.Empty;

                    if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    {
                        var requestMessage = new HttpRequestMessage
                        {
                            Method = HttpMethod.Post,
                            RequestUri = new Uri(url)
                        };

                        requestMessage.Headers.Add("X-Schedule-Job", "true");

                        var response = await _httpClient.SendAsync(requestMessage);

                        response.EnsureSuccessStatusCode();
                    }
                }
                catch (Exception exception)
                {
                    await _logger.LogMessageAsync($"Execute scheduled job [{context.FireInstanceId}] failed", LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);
                }
            }
        }
    }
}
