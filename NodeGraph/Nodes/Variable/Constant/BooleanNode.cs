using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable;

public class BooleanNode : VariableNode
{
    public BooleanNode(ScriptBrain brain, VariableData data) : base(brain,
        NodeTypes.Boolean, data)
    {
        ImplementationNodeData = new[]
        {
            new Property(null,
                new List<Values>()
                {
                    new Values(Keywords.Out, DataType.Float, (bool)(data.Data ?? false))
                })
        };

        Pins = new[]
        {
            new Pin(Keywords.Out, 0)
        };
    }

    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
            return new VariableConnection(typeof(bool), typeof(BooleanNode), destinationId, destinationPin, NodeId, Keywords.Out);
    }
}