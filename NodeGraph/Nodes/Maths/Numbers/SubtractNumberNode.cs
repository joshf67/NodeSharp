using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.NodeGraph.Nodes;

public class SubtractNumberNode : Node, GetterInterface
{
    public SubtractNumberNode(ScriptBrain brain, VariableNode a, VariableNode b) : base(brain, NodeTypes.Subtract_Number)
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
            new Pin(Keywords.OperandA),
            new Pin(Keywords.OperandB),
            new Pin(Keywords.Result)
        };
        
        brain.AddConnection(a.Getter(brain, NodeID, Keywords.OperandA));
        brain.AddConnection(b.Getter(brain, NodeID, Keywords.OperandB));
    }

    public VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(float), destinationId, destinationPin, NodeID, Keywords.Result);
    }
}