using System.Numerics;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable.Getter;

public class Vector3GetterNode : Node, IGetter
{
    public Vector3GetterNode(ScriptBrain brain, VariableData data, int destinationId = -1, string destinationPin = "") : base(brain, NodeTypes.Boolean_Getter)
    {
        ImplementationNodeData = new[]
        {
            new Property(null,
                new List<Values>()
                {
                    // new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                    new Values(Keywords.Identifier, DataType.UShort, data.Identifier),
                    new Values(Keywords.Scope, DataType.Int, data.Scope)
                })
        };

        Pins = new[]
        {
            new Pin(Keywords.Identifier, 0),
            new Pin(Keywords.Scope, 0),
            new Pin(Keywords.Vector, 0)
        };

        if (destinationId != -1)
        {
            Input.Add(new VariableConnection(typeof(Vector3), typeof(Vector3GetterNode), destinationId, destinationPin, NodeId, Keywords.Vector));
        }
    }

    public VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(Vector3), typeof(Vector3GetterNode), destinationId, destinationPin, NodeId,
            Keywords.Vector);
    }
}