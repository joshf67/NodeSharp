using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.NodeGraph.Nodes;

public class AddNumberNode : Node, IGetter
{
    public AddNumberNode(ScriptBrain brain, IGetter a, IGetter b) : base(brain, NodeTypes.Add_Number)
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
            new Pin(Keywords.Result)
        };
        
        Input.Add(a.Getter(brain, NodeID, Keywords.OperandA));
        Input.Add(b.Getter(brain, NodeID, Keywords.OperandB));
    }

    public VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        return new VariableConnection(typeof(float), typeof(DeclareNumberNode), destinationId, destinationPin, NodeID, Keywords.Result);
    }

    public override bool Optimize(ScriptBrain brain)
    {
        base.Optimize(brain);

        for (int i = 0; i < Input.Count; i++)
        {
            if (brain.NodeMap[Input[i].OriginNode].node is not VariableNode node) continue;
            if (node.NodeData.Scope != ScopeEnum.Constant) continue;
            
            if (Input[i].DestinationPin == Keywords.OperandA)
            {
                ImplementationNodeData[0].Values[1].DataValue = node.NodeData.Data;
            }
            else
            {
                ImplementationNodeData[0].Values[2].DataValue = node.NodeData.Data;
            }

            brain.RemoveReferenceToNode(node);
            Input.RemoveAt(i);
            i--;
        }
        
        //if no inputs, convert to number node, unless parent is defaultable??
        if (Input.Count != 0) return false;

        var data = new VariableData(
            (float)ImplementationNodeData[0].Values[1].DataValue + (float)ImplementationNodeData[0].Values[2].DataValue,
            scope: ScopeEnum.Constant);
        var newNode = new DeclareNumberNode(brain, data);
        var connector = newNode.Getter(brain);

        brain.ReplaceNodeInput(this, newNode, connector.OriginPin);
        return true;
    }
}