using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.NodeGraph.Nodes;

public class SquareRootNumberNode : Node, IGetter
{
    public SquareRootNumberNode(ScriptBrain brain, IGetter a) : base(brain, NodeTypes.Square_Root_Number)
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
        
        Input.Add(a.Getter(brain, NodeID, Keywords.Operand));
    }

    public VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(float), typeof(DeclareNumberNode), destinationId, destinationPin, NodeID, Keywords.Result);
    }
}