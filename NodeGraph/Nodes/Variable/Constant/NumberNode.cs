using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable;

public class NumberNode : VariableNode, IDefaultable
{
    public NumberNode(ScriptBrain brain, VariableData data, int originId = -1, string originPin = "") : base(brain, NodeTypes.Number, data)
    {
        if (data.Scope == ScopeEnum.Constant)
        {
            ImplementationNodeData = new[]
            {
                new Property(null,
                    new List<Values>()
                    {
                        new Values(Keywords.Out, DataType.Float, (float)(data.Data ?? 0))
                    })
            };

            Pins = new[]
            {
                new Pin(Keywords.Out, 0)
            };
        }
    }

    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
            return new VariableConnection(typeof(float), typeof(DeclareNumberNode), destinationId, destinationPin, NodeID, Keywords.Out);
    }

    public override (Node Setter, VariableConnection SetterConnection) Setter(ScriptBrain brain, int originId = -1, string originPin = "")
    {
        if (NodeData.Scope == ScopeEnum.Constant)
            throw new Exception("Trying to set a constant variable");
        
        Node setterNode = new Node(NodeTypes.Number_Setter, 
            new[]
            {
                new Property(null, 
                    new List<Values>() { 
                        new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                        new Values(Keywords.Scope, DataType.Int, NodeData.Scope),
                        new Values(Keywords.Identifier, DataType.UShort, NodeData.Identifier),
                        new Values(Keywords.Value, DataType.Float, (float)(NodeData.Data ?? 0))
                    })
            },
            new[]
            {
                new Pin(Keywords.ActionStart),
                new Pin(Keywords.Identifier),
                new Pin(Keywords.Object),
                new Pin(Keywords.Value),
                new Pin(Keywords.Scope),
                new Pin(Keywords.ActionComplete)
            });

        brain.AddNode(setterNode);
        
        return (setterNode, new VariableConnection(typeof(float), typeof(DeclareNumberNode), setterNode.NodeID, Keywords.Value, originId, originPin));
    }

    public override bool Optimize(ScriptBrain brain)
    {
        base.Optimize(brain);
        
        if (Input.Count is 0) return false;
        if (brain.NodeMap[Input[0].OriginNode].node is not VariableNode { NodeData.Scope: ScopeEnum.Constant } node) return false;
        
        brain.RemoveReferenceToNode(node);
        Input.Clear();

        ImplementationNodeData[0].Values[0].DataValue = node.NodeData.Data;
        return false;
    }
}