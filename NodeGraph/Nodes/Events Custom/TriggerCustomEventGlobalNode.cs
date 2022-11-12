using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Events_Custom;

public class TriggerCustomEventGlobalNode : Node, IActionNode
{
    public TriggerCustomEventGlobalNode(ScriptBrain brain, VariableData data) : base(brain, NodeTypes.TriggerCustomEventGlobal)
    {
        ImplementationNodeData = new[]
        {
            new Property(null, new List<Values>()
            {
                new Values(Keywords.Number, DataType.Float, Convert.ToSingle(data.Data ?? 0)),
                new Values(Keywords.Identifier, DataType.UShort, data.Identifier)
            })
        };

        Pins = new[]
        {
            new Pin(Keywords.ActionStart, 0),
            new Pin(Keywords.Identifier, 0),
            new Pin(Keywords.ObjectList, 0),
            new Pin(Keywords.Object, 0),
            new Pin(Keywords.Number, 0),
            new Pin(Keywords.ActionComplete, 0)
        };
    }

    public string? GetActionSendPin()
    {
        return Keywords.ActionComplete;
    }

    public string? GetActionReceivePin()
    {
        return Keywords.ActionStart;
    }
}