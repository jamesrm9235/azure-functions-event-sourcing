namespace FunctionApp.Todos;

public static class Events
{
    public static class V1
    {
        public sealed record Created(string Id, string Description, DateTime Timestamp)
        {
            public static Created From(Commands.V1.Create command) =>
                new(command.Id, command.Description, DateTime.UtcNow);
        }

        public sealed record LoggedProgress(string Id, int Progress, DateTime Timestamp)
        {
            public static LoggedProgress From(Commands.V1.LogProgress command) =>
                new(command.Id, command.Progress, DateTime.UtcNow);
        }

        public sealed record Completed(string Id, DateTime Timestamp)
        {
            public static Completed From(Commands.V1.LogProgress command) =>
                new(command.Id, DateTime.UtcNow);
        }
    }
}