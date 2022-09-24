using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.NodeGraph.Nodes;

public class DivideNumberNode : Node, GetterInterface
{
    public DivideNumberNode(ScriptBrain brain, VariableNode a, VariableNode b) : base(brain, NodeTypes.Divide_Number)
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
            new Pin(Keywords.Remainder),
            new Pin(Keywords.Quotient),
            new Pin(Keywords.Whole)
        };
        
        brain.AddConnection(a.Getter(brain, NodeID, Keywords.OperandA));
        brain.AddConnection(b.Getter(brain, NodeID, Keywords.OperandB));
    }

    public VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(float), destinationId, destinationPin, NodeID, Keywords.Quotient);
    }

    public VariableConnection Remainder(int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(float), destinationId, destinationPin, NodeID, Keywords.Remainder);
    }
    
    public VariableConnection IntResult(int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(float), destinationId, destinationPin, NodeID, Keywords.Whole);
    }
}