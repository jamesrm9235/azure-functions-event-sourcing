using System;
using FunctionalEventSourcing;

using static System.Text.Encoding;
using static Newtonsoft.Json.JsonConvert;

namespace FunctionApp;

public sealed class EventSerializer : ISerializer
{
    public object Deserialize(string type, byte[] data) =>
        DeserializeObject(
            UTF8.GetString(data), Type.GetType(type));

    public byte[] Serialize(object @event) =>
        UTF8.GetBytes(
            SerializeObject(@event));
}