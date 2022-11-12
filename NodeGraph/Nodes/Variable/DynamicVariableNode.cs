using Newtonsoft.Json;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable;

public abstract class DynamicVariableNode : BaseVariableNode
{
    [JsonIgnore] protected DynamicVariableGetterNode? GetterNode = null;

    protected DynamicVariableNode(ScriptBrain brain, string nodeType, VariableData data) : base(brain, nodeType, data) { }

    public abstract (Node SetterNode, VariableConnection SetterConnection) Setter(ScriptBrain brain, int originId = -1,
        string originPin = "");
}