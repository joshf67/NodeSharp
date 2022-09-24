using Newtonsoft.Json;

namespace NodeSharp.NodeGraph.NodeData;

public class MetaData
{
    [JsonProperty("label")]
    public string Label = "Position";
    
    [JsonProperty("positionX")]
    public string PositionX = "0";
    
    [JsonProperty("positionY")]
    public string PositionY = "0";

    public MetaData(float positionX = 0, float positionY = 0, string label = "Position")
    {
        Label = label;
        PositionX = positionX.ToString();
        PositionY = positionY.ToString();
    }
}