using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.NodeGraph.Nodes;

public class SquareRootNumberNode : Node, GetterInterface
{
    public SquareRootNumberNode(ScriptBrain brain, VariableNode a) : base(brain, NodeTypes.Square_Root_Number)
    {
        ImplementationNodeData = new[]
        {
            new Property(null, 
                new List<Values>() { 
                    new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true)
                })
        };
            
        Pins = new[]
        {
            new Pin(Keywords.Operand),
            new Pin(Keywords.Result)
        };
        
        brain.AddConnection(a.Getter(brain, NodeID, Keywords.Operand));
    }

    public VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(float), destinationId, destinationPin, NodeID, Keywords.Result);
    }
}