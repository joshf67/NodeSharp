using Newtonsoft.Json;
using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp;

/*
 * Maybe change variable node to not accept a connectedVariableNodeType and figure that out dynamically
 */

public class VariableConnection : Connection
{
    [JsonIgnore] public object Variable;
    [JsonIgnore] public Type ConnectedVariableNodeType;

    public VariableConnection(object variable, Type connectedVariableNodeType, int destinationId, string destinationPin, int originId, string originPin,
        List<Property> properties = null, int version = 0) : base(destinationId,
        destinationPin, originId, originPin, properties, version)
    {
        Variable = variable;
        ConnectedVariableNodeType = connectedVariableNodeType;
    }
    
    public VariableConnection(VariableConnection connection) : base(connection)
    {
        Variable = connection.Variable;
        ConnectedVariableNodeType = connection.ConnectedVariableNodeType;
    }
}