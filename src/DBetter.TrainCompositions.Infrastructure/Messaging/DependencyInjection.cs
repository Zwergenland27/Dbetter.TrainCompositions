using CleanMessageBus.Abstractions.DependencyInjection;
using CleanMessageBus.RabbitMQ.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;

namespace DBetter.TrainCompositions.Infrastructure.Messaging;

public static class DependencyInjection
{
    public static void AddOutbox(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(options =>
        {
            options.AddJob<OutboxProcessor>(OutboxProcessor.JobKey)
                .AddTrigger(trigger => trigger
                    .ForJob(OutboxProcessor.JobKey)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(10).RepeatForever()));
        });
        
        services.AddQuartzHostedService();
        
        var settings = new RabbitMqSettings();
        configuration.Bind(RabbitMqSettings.SectionName, settings);
        settings.Validate();
        
        services.AddCleanMessageBus(options =>
        {
            options.UseRabbitMq(rabbitMqOptions =>
            {
                rabbitMqOptions.WithHostname(settings.Hostname);
            });
        });
    }
}