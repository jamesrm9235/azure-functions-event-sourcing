using Azure;
using Azure.Data.Tables;

namespace FunctionalEventSourcing;

public sealed class EventEntity : ITableEntity
{
    public EventEntity()
    {
    }

    public EventEntity(string streamId, int version, string type, byte[] data)
    {
        Preconditions.Check(!string.IsNullOrWhiteSpace(streamId), nameof(streamId), "Must not be null or white space");
        Preconditions.Check(version != 0, nameof(version), "Must not be zero");
        Preconditions.Check(!string.IsNullOrWhiteSpace(type), nameof(type), "Must not be null or white space");
        Preconditions.Check(data != null, nameof(data), "Must not be null");

        Data = data;
        PartitionKey = streamId;
        RowKey = version.ToString("0000000000");
        Type = type;
        Version = version;
    }

    public byte[]? Data { get; set; }
    public string? Type { get; set; }
    public int Version { get; set; }

    public string PartitionKey { get; set; } = "";
    public string RowKey { get; set; } = "";
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}