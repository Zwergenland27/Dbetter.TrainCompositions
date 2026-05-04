using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DBetter.TrainCompositions.Infrastructure.OutboxPattern;

public static class DependencyInjection
{
    public static void AddOutbox(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            options.AddJob<OutboxProcessor>(OutboxProcessor.JobKey)
                .AddTrigger(trigger => trigger
                    .ForJob(OutboxProcessor.JobKey)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(10).RepeatForever()));
        });
        
        services.AddQuartzHostedService();
    }
}