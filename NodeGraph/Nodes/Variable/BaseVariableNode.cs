using Newtonsoft.Json;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable;

public abstract class BaseVariableNode : Node, IGetter
{
    [JsonIgnore] public VariableData NodeData;

    protected BaseVariableNode(ScriptBrain brain, string nodeType, VariableData data) : base (brain, nodeType)
    {
        NodeData = data;
    }
    
    public abstract VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "");
}