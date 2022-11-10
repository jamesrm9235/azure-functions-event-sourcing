namespace FunctionApp.Todos;

public static class Commands {
    public static class V1 {
        public record Create (string Id, string Description);

        public record LogProgress (string Id, int Progress);
    }
}