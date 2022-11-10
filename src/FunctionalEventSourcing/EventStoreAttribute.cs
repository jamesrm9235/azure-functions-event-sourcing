using Microsoft.Azure.WebJobs.Description;

namespace FunctionalEventSourcing;

[Binding]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class EventStoreAttribute : Attribute
{
    public EventStoreAttribute(string streamId)
    {
        Preconditions.Check(!string.IsNullOrWhiteSpace(streamId), nameof(streamId), "Must not be null or white space");
        StreamId = streamId;
    }

    [AutoResolve]
    public string StreamId { get; }
}