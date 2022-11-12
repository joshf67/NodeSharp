using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable;

public class StringNode : VariableNode
{
    public StringNode(ScriptBrain brain, VariableData data) : base(brain,
        NodeTypes.String, data)
    {
        ImplementationNodeData = new[]
        {
            new Property(null,
                new List<Values>()
                {
                    new Values(Keywords.Out, DataType.Float, (int)(data.Data ?? 0))
                })
        };

        Pins = new[]
        {
            new Pin(Keywords.Out, 0)
        };
    }

    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
            return new VariableConnection(typeof(int), typeof(StringNode), destinationId, destinationPin, NodeId, Keywords.Out);
    }
}