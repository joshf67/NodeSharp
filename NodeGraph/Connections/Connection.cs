using Newtonsoft.Json;
using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp;

public class Connection
{
    [JsonProperty("m_connectionID")]
    public int ConnectionID = -1;
    
    [JsonProperty("m_connectionType")]
    public string ConnectionType = "DefaultConnection";
    
    [JsonProperty("m_destinationNode")]
    public int DestinationNode = -1;
    
    [JsonProperty("m_destinationPin")]
    public string DestinationPin = "";

    [JsonProperty("m_originNode")]
    public int OriginNode = -1;
    
    [JsonProperty("m_originPin")]
    public string OriginPin = "";

    [JsonProperty("m_properties")]
    public List<Property> Properties = new List<Property>()
    {
        new Property()
    };

    [JsonProperty("m_version")]
    public int Version = 0;

    public Connection(int destinationID, string destinationPin, int originID, string originPin,
        List<Property> properties = null, int version = 0)
    {
        DestinationNode = destinationID;
        DestinationPin = destinationPin;
        OriginNode = originID;
        OriginPin = originPin;
        Properties = properties?? Properties;
        Version = version;
    }
}