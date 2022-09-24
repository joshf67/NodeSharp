using Newtonsoft.Json;
using NodeSharp.JSONConverter;

namespace NodeSharp.NodeGraph.NodeData;

public class Property
{
    [JsonProperty("subGroups")]
    public object[] SubGroups = new object[] { null };
    
    [JsonProperty("values")]
    [JsonConverter(typeof(ValuesConverter))]
    public List<Values> Values = new List<Values>() { null };

    public Property(object[] _subGroups = null, List<Values> values = null)
    {
        SubGroups = _subGroups?? SubGroups;
        Values = values?? Values;
    }

}