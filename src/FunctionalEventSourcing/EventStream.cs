using Azure.Data.Tables;

using static Azure.Data.Tables.TableTransactionActionType;

namespace FunctionalEventSourcing;

internal sealed class EventStream : IEventStream
{
    private  const string HeadRowKey = "0000000000";

    private readonly ISerializer serializer;
    private readonly TableClient table;

    public EventStream(ISerializer serializer, TableClient table, string streamId)
    {
        Preconditions.Check(serializer != null, nameof(serializer), "Must not be null");
        Preconditions.Check(table != null, nameof(table), "Must not be null");
        Preconditions.Check(!string.IsNullOrWhiteSpace(streamId), nameof(streamId), "Must not be null or white space");

        this.serializer = serializer;
        this.table = table;

        StreamId = streamId;
    }

    public string StreamId { get; }

    public int Version { get; private set; }

    public async Task<T> LoadAsync<T>(Func<T, object, T> fold, CancellationToken ct = default) where T : new()
    {
        Preconditions.Check(fold != null, nameof(fold), "Must not be null");

        // read all instance events
        var pages = table.QueryAsync<EventEntity>(o => o.PartitionKey == StreamId && o.RowKey != HeadRowKey, cancellationToken: ct).AsPages().ConfigureAwait(false);
        var records = new List<EventEntity>();
        await foreach (var item in pages)
        {
            records.AddRange(item.Values);
        }

        // rebuild instance
        Version = 0;
        var instance = new T();
        foreach (var item in records)
        {
            var @event = serializer.Deserialize(item.Type!, item.Data!);
            instance = fold(instance, @event);
            Version += 1;
        }

        return instance;
    }

    public async Task AppendAsync(object @event, CancellationToken ct = default)
    {
        Preconditions.Check(@event != null, nameof(@event), "Must not be null");

        var head = table.Query<EventEntity>(o => o.PartitionKey == StreamId && o.RowKey == HeadRowKey, cancellationToken: ct).SingleOrDefault();

        // optimistic concurrency check
        if (head != null && head.Version != Version)
        {
            throw new InvalidOperationException($"Stream has updated since last load (Loaded Version = {Version}, Latest Version = {head.Version})");
        }

        var actions = new List<TableTransactionAction>();

        // update the stream head
        if (head == null)
        {
            head = new EventEntity { PartitionKey = StreamId, RowKey = HeadRowKey, Version = 1, };

            actions.Add(new TableTransactionAction(Add, head));
        }
        else
        {
            head.Version += 1;

            actions.Add(new TableTransactionAction(UpdateMerge, head, head.ETag));
        }

        // append the event
        var type = @event.GetType().AssemblyQualifiedName!;
        var data = serializer.Serialize(@event);
        actions.Add(new TableTransactionAction(Add, new EventEntity(StreamId, head.Version, type, data)));

        await table.SubmitTransactionAsync(actions, ct).ConfigureAwait(false);

        Version = head.Version;
    }
}