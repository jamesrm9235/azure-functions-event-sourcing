namespace FunctionalEventSourcing;

public interface ISerializer
{
    byte[] Serialize(object @event);

    object Deserialize(string type, byte[] data);
}