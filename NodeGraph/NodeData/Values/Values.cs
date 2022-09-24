using Newtonsoft.Json;

namespace NodeSharp.NodeGraph.NodeData;

public class Values
{
    [JsonProperty("dataKey")]
    public string DataKey = "";
    
    [JsonProperty("dataType")]
    public string DataType = "";
    
    [JsonProperty("dataValue")]
    public object DataValue = null;

    public Values(string dataKey, string dataType, object dataValue)
    {
        DataKey = dataKey;
        DataType = dataType;
        DataValue = dataValue;
    }
}