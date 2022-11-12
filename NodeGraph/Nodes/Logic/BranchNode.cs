using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Logic;

public class BranchNode : Node, IActionNode
{
    public BranchNode(ScriptBrain brain, VariableData data) : base(brain, NodeTypes.Branch)
    {
        ImplementationNodeData = new[]
        {
            new Property(null, new List<Values>()
            {
                new Values(Keywords.Condition, DataType.Bool, (bool)(data.Data ?? false))
            })
        };

        Pins = new[]
        {
            new Pin(Keywords.ActionStart, 0),
            new Pin(Keywords.ExecuteIfTrue, 0),
            new Pin(Keywords.Condition, 0),
            new Pin(Keywords.ExecuteIfFalse, 0)
        };
    }

    public string? GetActionSendPin()
    {
        return null;
    }

    public string? GetActionReceivePin()
    {
        return Keywords.ActionStart;
    }
}