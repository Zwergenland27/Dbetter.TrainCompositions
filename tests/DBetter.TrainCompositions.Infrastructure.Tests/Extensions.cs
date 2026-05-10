using System.Reflection;
using CleanMessageBus.Abstractions;
using DBetter.TrainCompositions.Domain.Abstractions;

namespace DBetter.TrainCompositions.Infrastructure.Tests;

public record TestDomainEvent(string Content) : IDomainEvent
{
    public static TestDomainEvent Example => new ("Example");
}

public static class Extensions
{
    public static void RaiseTestDomainEvent<TId>(this AggregateRoot<TId> aggregate, TestDomainEvent domainEvent) where TId : notnull
    {
        var method = typeof(AggregateRoot<TId>)
            .GetMethod("RaiseDomainEvent", BindingFlags.NonPublic | BindingFlags.Instance)!;
        
        method.Invoke(aggregate, [domainEvent]);
    }
}