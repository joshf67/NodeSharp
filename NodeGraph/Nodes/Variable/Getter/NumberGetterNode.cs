using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable.Getter;

public class NumberGetterNode : DynamicVariableGetterNode
{
    public NumberGetterNode(ScriptBrain brain, VariableData data) : base(brain, data, NodeTypes.NumberGetter)
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
            new Pin(Keywords.Out, 0)
        };
    }

    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(NodeData.Data, typeof(DeclareNumberNode), destinationId, destinationPin, NodeId,
            Keywords.Out);
    }
}