using FunctionalEventSourcing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace FunctionApp.Todos;

public static class CreateFunction
{
    [FunctionName(nameof(CreateFunction))]
    public static async Task<IActionResult> Run
    (
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "create")] Commands.V1.Create command,
        [EventStore("{Id}")] IEventStream events,
        CancellationToken ct
    )
    {
        // validate command
        if (!Guid.TryParse(command.Id, out _) || string.IsNullOrWhiteSpace(command.Description) || command.Description.Length > 50)
        {
            return new BadRequestResult();
        }

        // create event from command
        await events.AppendAsync(Events.V1.Created.From(command), ct);

        return new OkObjectResult(events.StreamId);
    }
}
