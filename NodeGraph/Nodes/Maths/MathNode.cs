using NodeSharp.Nodes.Interface;

namespace NodeSharp.NodeGraph.Nodes;

public abstract class MathNode : Node
{
    protected MathNode(ScriptBrain brain, string nodeType) : base (brain, nodeType) { }
}