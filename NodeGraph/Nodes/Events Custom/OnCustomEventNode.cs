using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Events_Custom;

public class OnCustomEventNode : Node, IActionNode
{
    public OnCustomEventNode(ScriptBrain brain, VariableData data) : base(brain, NodeTypes.OnCustomEvent)
    {
        ImplementationNodeData = new[]
        {
            new Property(null, new List<Values>()
            {
                new Values(Keywords.Identifier, DataType.UShort, data.Identifier)
            })
        };

        Pins = new[]
        {
            new Pin(Keywords.Identifier, 0),
            new Pin(Keywords.Number, 0),
            new Pin(Keywords.EventTriggered, 0),
            new Pin(Keywords.ObjectList, 0),
            new Pin(Keywords.Object, 0)
        };
    }

    public string? GetActionSendPin()
    {
        return Keywords.EventTriggered;
    }

    public string? GetActionReceivePin()
    {
        return null;
    }
}