using FunctionalEventSourcing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace FunctionApp.Todos;

public static class LogProgressFunction
{
    [FunctionName(nameof(LogProgressFunction))]
    public static async Task<IActionResult> Run
    (
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "update")] Commands.V1.LogProgress command,
        [EventStore("{Id}")] IEventStream events,
        CancellationToken ct
    )
    {
        // validate command
        if (string.IsNullOrWhiteSpace(command.Id) || command.Progress < 0 || command.Progress > 100)
        {
            return new BadRequestResult();
        }

        // load state to make decisions on how to process command
        var instance = await events.LoadAsync<TodoState>(TodoState.Apply, ct);

        if (instance.Status == Status.Done)
        {
            return new BadRequestResult();
        }
        else if (instance.Progress + command.Progress >= 100)
        {
            await events.AppendAsync(Events.V1.Completed.From(command), ct);
        }
        else
        {
            await events.AppendAsync(Events.V1.LoggedProgress.From(command), ct);
        }

        return new OkResult();
    }
}