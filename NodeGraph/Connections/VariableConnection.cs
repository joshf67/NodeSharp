using Newtonsoft.Json;
using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp;

public class VariableConnection : Connection
{
    [JsonIgnore] public Type VariableType;
    [JsonIgnore] public Type ConnectionNodeType;

    public VariableConnection(Type variableType, Type connectionNodeType, int destinationID, string destinationPin, int originID, string originPin,
        List<Property> properties = null, int version = 0) : base(destinationID,
        destinationPin, originID, originPin, properties, version)
    {
        VariableType = variableType;
        ConnectionNodeType = connectionNodeType;
    }
    
    public VariableConnection(VariableConnection connection) : base(connection)
    {
        VariableType = connection.VariableType;
        ConnectionNodeType = connection.ConnectionNodeType;
    }
}