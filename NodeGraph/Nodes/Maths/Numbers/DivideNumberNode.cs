using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.NodeGraph.Nodes;

public class DivideNumberNode : Node, IGetter
{
    public DivideNumberNode(ScriptBrain brain, IGetter a, IGetter b) : base(brain, NodeTypes.DivideNumber)
    {
        ImplementationNodeData = new[]
        {
            new Property(null, 
                new List<Values>() { 
                    new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                    new Values(Keywords.OperandA, DataType.Float, 0),
                    new Values(Keywords.OperandB, DataType.Float, 0)
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
        
        Input.Add(a.Getter(brain, NodeId, Keywords.OperandA));
        Input.Add(b.Getter(brain, NodeId, Keywords.OperandB));
    }

    public VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        if (Input.Count != 2 || Input[0] is not VariableConnection a || Input[1] is not VariableConnection b)
            throw new InvalidDataException($"Trying to get result from a DivideNumberNode with invalid inputs");

        return new VariableConnection((float)a.Variable / (float)b.Variable, typeof(DeclareNumberNode),
            destinationId, destinationPin, NodeId, Keywords.Quotient);
    }

    public VariableConnection Remainder(int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(null, typeof(DeclareNumberNode), destinationId, destinationPin,
            NodeId, Keywords.Remainder);
    }
    
    public VariableConnection IntResult(int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(null, typeof(DeclareNumberNode), destinationId, destinationPin,
            NodeId, Keywords.Whole);
    }

    public override bool Optimize(ScriptBrain brain)
    {
        base.Optimize(brain);
    
        for (int i = 0; i < Input.Count; i++)
        {
            if (brain.NodeMap[Input[i].OriginNode].node is not BaseVariableNode node) continue;
            if (node.NodeData.Scope != ScopeEnum.Constant) continue;
            
            if (Input[i].DestinationPin == Keywords.OperandA)
            {
                ImplementationNodeData[0].Values[1].DataValue = node.NodeData.Data;
            }
            else
            {
                ImplementationNodeData[0].Values[2].DataValue = node.NodeData.Data;
            }
    
            Input.RemoveAt(i);
            i--;
        }
        
        //if no inputs, convert to number node, unless parent is defaultable??
        if (Input.Count != 0) return false;
    
        var data = new VariableData(
            (float)ImplementationNodeData[0].Values[1].DataValue / (float)ImplementationNodeData[0].Values[2].DataValue,
            scope: ScopeEnum.Constant);
        var newNode = new DeclareNumberNode(brain, data);
        var connector = newNode.Getter(brain);
    
        brain.ReplaceNodeInput(this, newNode, connector.OriginPin);
        return true;
    }
}