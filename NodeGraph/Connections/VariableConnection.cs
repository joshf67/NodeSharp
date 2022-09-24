using Newtonsoft.Json;
using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp;

public class VariableConnection : Connection
{
    [JsonIgnore] public Type ConnectionVariableType;

    public VariableConnection(Type connectionVariableType, int destinationID, string destinationPin, int originID, string originPin,
        List<Property> properties = null, int version = 0) : base(destinationID,
        destinationPin, originID, originPin, properties, version)
    {
        ConnectionVariableType = connectionVariableType;
    }
}