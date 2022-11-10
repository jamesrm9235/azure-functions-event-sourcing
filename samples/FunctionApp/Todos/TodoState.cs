namespace FunctionApp.Todos;

public enum Status : int { Backlogged = 0, Doing = 1, Done = 2, }

public sealed record TodoState
{
    public Status Status { get; init; } = Status.Backlogged;
    public int Progress { get; init; }

    public static TodoState Apply (TodoState state, object @event) =>
        @event switch
        {
            Events.V1.Created e => state with { Status = Status.Backlogged, Progress = 0 },
            Events.V1.LoggedProgress e => state with { Status = Status.Doing, Progress = (state.Progress + e.Progress) },
            Events.V1.Completed e => state with { Status = Status.Done, Progress = 100 },
            _ => throw new ArgumentException ($"Cannot apply '{@event.GetType().Name}'", nameof (@event))
        };
}