using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Events_Custom;

public class OnGameStartNode : Node, IActionNode
{
    public OnGameStartNode(ScriptBrain brain) : base(brain, NodeTypes.OnGameStart)
    {
        Pins = new[]
        {
            new Pin(Keywords.EventTriggered, 0)
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