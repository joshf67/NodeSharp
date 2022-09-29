using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.NodeGraph.Nodes;

public class SubtractNumberNode : Node, IGetter
{
    public SubtractNumberNode(ScriptBrain brain, IGetter a, IGetter b) : base(brain, NodeTypes.Subtract_Number)
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
        
        Input.Add(a.Getter(brain, NodeID, Keywords.OperandA));
        Input.Add(b.Getter(brain, NodeID, Keywords.OperandB));
    }

    public VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(float), typeof(DeclareNumberNode), destinationId, destinationPin, NodeID, Keywords.Result);
    }
}