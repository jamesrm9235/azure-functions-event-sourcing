using Azure.Data.Tables;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Options;

namespace FunctionalEventSourcing;

public sealed class EventStoreBinding : IExtensionConfigProvider
{
    private readonly ISerializer serializer;
    private readonly TableClient tableClient;

    public EventStoreBinding(IOptions<EventStoreOptions> options, ISerializer serializer)
    {
        Preconditions.Check(serializer != null, nameof(serializer), "Must not be null");
        this.serializer = serializer;

        Preconditions.Check(options.Value != null, nameof(options.Value), "Must not be null");
        var bindingOptions = options.Value;

        Preconditions.Check(!string.IsNullOrWhiteSpace(bindingOptions.TableConnectionString), nameof(bindingOptions.TableConnectionString), "Must not be null or white space");
        Preconditions.Check(!string.IsNullOrWhiteSpace(bindingOptions.TableName), nameof(bindingOptions.TableName), "Must not be null or white space");
        tableClient = new TableClient(options.Value.TableConnectionString, options.Value.TableName);
        tableClient.CreateIfNotExists();
    }

    public void Initialize(ExtensionConfigContext context)
    {
        context.AddBindingRule<EventStoreAttribute>().BindToInput<IEventStream>(attribute => new EventStream(serializer, tableClient, attribute.StreamId));
    }
}