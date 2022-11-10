namespace FunctionalEventSourcing;

public interface IEventStream
{
    string StreamId { get; }

    int Version { get; }

    Task<T> LoadAsync<T>(Func<T, object, T> fold, CancellationToken ct = default) where T : new();

    Task AppendAsync(object @event, CancellationToken ct = default);
}
