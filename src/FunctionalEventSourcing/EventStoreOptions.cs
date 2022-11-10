namespace FunctionalEventSourcing;

public sealed class EventStoreOptions
{
    public string TableConnectionString { get; set; } = "UseDevelopmentStorage=true;";
    public string TableName { get; set; } = "EventStore";
}