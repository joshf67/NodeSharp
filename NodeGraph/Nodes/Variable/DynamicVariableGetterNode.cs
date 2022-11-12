using Newtonsoft.Json;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable;

public abstract class DynamicVariableGetterNode : Node
{
    [JsonIgnore] public VariableData NodeData;

    protected DynamicVariableGetterNode(ScriptBrain brain, VariableData data, string nodeType) : base(brain, nodeType)
    {
        NodeData = data;
    }

    public abstract VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "");
}