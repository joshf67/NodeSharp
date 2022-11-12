using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable.Getter;

public class StringGetterNode : Node, IGetter
{
    public StringGetterNode(ScriptBrain brain, VariableData data, int destinationId = -1, string destinationPin = "") : base(brain, NodeTypes.Boolean_Getter)
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
            new Pin(Keywords.Value, 0)
        };

        if (destinationId != -1)
        {
            Input.Add(new VariableConnection(typeof(int), typeof(StringGetterNode), destinationId, destinationPin, NodeId, Keywords.Value));
        }
    }

    public VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(int), typeof(StringGetterNode), destinationId, destinationPin, NodeId,
            Keywords.Value);
    }
}