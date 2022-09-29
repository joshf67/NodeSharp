using Newtonsoft.Json;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable;

public abstract class VariableNode : Node, IGetter
{
    [JsonIgnore] public VariableData NodeData;
    [JsonIgnore] public Node? GetterNode;
    
    public VariableNode(ScriptBrain brain, string nodeType, VariableData data, int originId = -1, string originPin = "") : base (brain, nodeType)
    {
        NodeData = data;
    }

    public virtual VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        throw new NotImplementedException("Setter node has not been implemented on this node type");
    }

    public virtual (Node Setter, VariableConnection SetterConnection) Setter(ScriptBrain brain, int originId = -1, string originPin = "")
    {
        throw new NotImplementedException("Setter node has not been implemented on this node type");
    }
}