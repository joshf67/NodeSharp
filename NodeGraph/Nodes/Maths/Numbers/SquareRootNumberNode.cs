using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.NodeGraph.Nodes;

public class SquareRootNumberNode : Node, IGetter
{
    public SquareRootNumberNode(ScriptBrain brain, IGetter a) : base(brain, NodeTypes.SquareRootNumber)
    {
        ImplementationNodeData = new[]
        {
            new Property(null, 
                new List<Values>() { 
                    new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                    new Values(Keywords.Operand, DataType.Float, 0),
                })
        };
            
        Pins = new[]
        {
            new Pin(Keywords.Operand),
            new Pin(Keywords.Result)
        };
        
        Input.Add(a.Getter(brain, NodeId, Keywords.Operand));
    }

    public VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        if (Input.Count != 1 || Input[0] is not VariableConnection a)
            throw new InvalidDataException($"Trying to get result from a SqrNumberNode with invalid inputs");

        return new VariableConnection(Math.Sqrt(Convert.ToSingle(a.Variable)), typeof(DeclareNumberNode),
            destinationId, destinationPin, NodeId, Keywords.Result);
    }

    public override bool Optimize(ScriptBrain brain)
    {
        base.Optimize(brain);
    
        if (Input.Count == 0) return false;
        if (brain.NodeMap[Input[0].OriginNode].node is not BaseVariableNode node) return false;
        if (node.NodeData.Scope != ScopeEnum.Constant) return false;
        
        //if no inputs, convert to number node, unless parent is defaultable??
        ImplementationNodeData[0].Values[1].DataValue = node.NodeData.Data;
        Input.RemoveAt(0);
        return true;
    }
}