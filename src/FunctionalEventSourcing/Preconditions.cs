using System.Diagnostics.CodeAnalysis;

namespace FunctionalEventSourcing;

internal static class Preconditions
{
    public static void Check([DoesNotReturnIf(false)] bool condition, string item, string message)
    {
        if (!condition)
        {
            throw new ArgumentException(message, item);
        }
    }
}