using Newtonsoft.Json;
using NodeSharp.NodeGraph.NodeData;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NodeSharp;

public class ScriptBrain
{
    public List<Connection> Connections = new List<Connection>() { };
    
    public string GraphType = "ForgeScriptingGraph";
    
    public int NGFVersion = 0;
    
    public List<Node> Nodes = new List<Node>() { };
    
    public int Version = 0;
    
    [JsonProperty("m_implementationNodeData")]
    public List<Property> ImplementationNodeData = new List<Property>() { new Property() };

    public ScriptBrain(List<Connection> connections = null, List<Node> nodes = null)
    {
        Connections = connections?? Connections;
        Nodes = nodes?? Nodes;
    }
    
    public int AddConnection(Connection connection)
    {
        connection.ConnectionID = Connections.Count;
        Connections.Add(connection);
        return connection.ConnectionID;
    }

    public int AddNode(Node node)
    {
        node.NodeID = Nodes.Count;
        node.Metadata[0].PositionX = (Nodes.Count * 500 + 50).ToString();
        node.Metadata[0].PositionY = 50.ToString();
        
        Nodes.Add(node);
        return node.NodeID;
    }
}