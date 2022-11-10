using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(FunctionalEventSourcing.Startup))]
namespace FunctionalEventSourcing;

public sealed class Startup : IWebJobsStartup
{
    public void Configure(IWebJobsBuilder builder)
    {
        builder.AddExtension<EventStoreBinding>();
    }
}