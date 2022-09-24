using Newtonsoft.Json;
using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp;

public class Node
{
    [JsonProperty("m_implementationNodeData")]
    public Property[] ImplementationNodeData = new Property[] { };

    [JsonProperty("m_metadata")]
    public MetaData[] Metadata = new MetaData[] { new MetaData() };

    [JsonProperty("m_nodeID")]
    public int NodeID = -1;
    
    [JsonProperty("m_nodeName")]
    public string NodeName = "";
    
    [JsonProperty("m_nodeType")]
    public string NodeType = "";
    
    [JsonProperty("m_pins")]
    public Pin[] Pins = new Pin[] { };

    [JsonProperty("m_version")]
    public int Version = 0;

    public Node(ScriptBrain brain, string nodeType)
    {
        NodeType = nodeType;
        brain.AddNode(this);
    }
    
    public Node(string nodeType, Property[] implementationNodeData, Pin[] pins)
    {
        NodeType = nodeType;
        ImplementationNodeData = implementationNodeData;
        Pins = pins;
    }
}