using Newtonsoft.Json;

namespace NodeSharp.NodeGraph.NodeData;

public class Pin
{
    [JsonProperty("m_nodeID")]
    public string NodeID = "";
    
    [JsonProperty("m_version")]
    public int Version = 0;

    public Pin(string nodeID, int version = 0)
    {
        NodeID = nodeID;
        Version = version;
    }
}