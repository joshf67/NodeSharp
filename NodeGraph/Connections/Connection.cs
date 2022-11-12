using Newtonsoft.Json;
using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp;

public class Connection
{
    [JsonProperty("m_connectionID")]
    public int ConnectionId = -1;
    
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

    public Connection(int destinationId, string destinationPin, int originId, string originPin,
        List<Property> properties = null, int version = 0)
    {
        DestinationNode = destinationId;
        DestinationPin = destinationPin;
        OriginNode = originId;
        OriginPin = originPin;
        Properties = properties?? Properties;
        Version = version;
    }

    public Connection(Connection connection)
    {
        DestinationNode = connection.DestinationNode;
        DestinationPin = connection.DestinationPin;
        OriginNode = connection.OriginNode;
        OriginPin = connection.OriginPin;
        Properties = connection.Properties?? Properties;
        Version = connection.Version;
    }
}