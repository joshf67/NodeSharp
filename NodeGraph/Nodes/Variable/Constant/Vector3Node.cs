using System.Numerics;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable;

public class Vector3Node : VariableNode
{
    public Vector3Node(ScriptBrain brain, VariableData data) : base(brain,
        NodeTypes.Vector3, data)
    {
        Vector3 val = (Vector3)(data.Data ?? Vector3.Zero);
        ImplementationNodeData = new[]
        {
            new Property(null,
                new List<Values>()
                {
                    new Values(Keywords.X, DataType.Float, val.X),
                    new Values(Keywords.Y, DataType.Float, val.Y),
                    new Values(Keywords.Z, DataType.Float, val.Z)
                })
        };

        Pins = new[]
        {
            new Pin(Keywords.Out, 0)
        };
    }

    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(Vector3), typeof(Vector3Node), destinationId, destinationPin, NodeId, Keywords.Out);
    }
}