using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable;

public class NumberNode : BaseVariableNode, IConstantType
{
    public NumberNode(ScriptBrain brain, VariableData data) : base(brain,
        NodeTypes.Number, data)
    {
        ImplementationNodeData = new[]
        {
            new Property(null,
                new List<Values>()
                {
                    new Values(Keywords.Out, DataType.Float, (float)(data.Data ?? 0))
                })
        };

        Pins = new[]
        {
            new Pin(Keywords.Out, 0)
        };
    }

    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
            return new VariableConnection(NodeData.Data, typeof(NumberNode), destinationId, destinationPin, NodeId, Keywords.Out);
    }

    public Type GetVariableNodeType()
    {
        return typeof(DeclareNumberNode);
    }
}