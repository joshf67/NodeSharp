using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable.Getter;
using NodeSharp.Nodes.Variable.Setter;

namespace NodeSharp.Nodes.Variable;

public class DeclareNumberNode : DynamicVariableNode, IDefaultable
{
    public DeclareNumberNode(ScriptBrain brain, VariableData data, int originId = -1, string originPin = "") : base(brain, NodeTypes.NumberDeclare, data)
    {
        ImplementationNodeData = new[]
            {
                new Property(null,
                    new List<Values>()
                    {
                        // new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                        new Values(Keywords.InitialValue, DataType.Float, (float)Math.Round(Convert.ToSingle(data.Data), 2)),
                        new Values(Keywords.Identifier, DataType.UShort, data.Identifier),
                        new Values(Keywords.Scope, DataType.Int, data.Scope)
                    })
            };

            Pins = new[]
            {
                new Pin(Keywords.InitialValue, 0),
                new Pin(Keywords.Identifier, 0),
                new Pin(Keywords.Scope, 0)
            };

            if (originId != -1)
            {
                Input.Add(new VariableConnection(data.Data, typeof(NumberNode), NodeId, Keywords.InitialValue, originId,
                    originPin));
            }
    }
    
    

    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        if (GetterNode is not null)
            return GetterNode.Getter(brain, destinationId, destinationPin);
        
        NumberGetterNode node = new NumberGetterNode(brain, NodeData);
        GetterNode = node;
            
        return GetterNode.Getter(brain, destinationId, destinationPin);
    }

    public override (Node SetterNode, VariableConnection SetterConnection) Setter(ScriptBrain brain, int originId = -1, string originPin = "")
    {
        NumberSetterNode setterNode = new NumberSetterNode(brain, NodeData, originId, originPin);
        
        return (setterNode, new VariableConnection(null, typeof(DeclareNumberNode), setterNode.NodeId, Keywords.Value, originId, originPin));
    }

    public override bool Optimize(ScriptBrain brain)
    {
        base.Optimize(brain);
        
        if (Input.Count is 0) return false;
        if (brain.NodeMap[Input[0].OriginNode].node is not BaseVariableNode { NodeData.Scope: ScopeEnum.Constant } node) return false;
        
        Input.Clear();

        ImplementationNodeData[0].Values[0].DataValue = node.NodeData.Data;
        return false;
    }
}