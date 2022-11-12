using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable.Getter;

namespace NodeSharp.Nodes.Variable;

public class DeclareBooleanNode : VariableNode, IDefaultable
{
    public DeclareBooleanNode(ScriptBrain brain, VariableData data, int originId = -1, string originPin = "") : base(brain,
        NodeTypes.Boolean_Declare, data)
    {
        ImplementationNodeData = new[]
        {
            new Property(null,
                new List<Values>()
                {
                    // new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                    new Values(Keywords.InitialValue, DataType.Bool, (bool)(data.Data ?? false)),
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
            Input.Add(new VariableConnection(typeof(bool), typeof(DeclareBooleanNode), NodeId, Keywords.InitialValue, originId,
                originPin));
        }
    }
    
    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        if (GetterNode is not null)
        {
            return GetterNode.Getter(brain, destinationId, destinationPin);
        }       
        
        BooleanGetterNode node = new BooleanGetterNode(brain, NodeData, destinationId, destinationPin);
        GetterNode = node;
        brain.AddNode(node);
            
        brain.AddReferenceToNode(this);
        return GetterNode.Getter(brain, destinationId, destinationPin);
        
        // if (GetterNode is not null)
        // {
        //     // GetterNode.Output.Add(brain.NodeMap[destinationId].node);
        //     return new VariableConnection(typeof(bool), typeof(DeclareBooleanNode), destinationId, destinationPin, GetterNode.NodeId, Keywords.Out);
        // }
        //
        // GetterNode = new Node(NodeTypes.Boolean_Getter,
        //     new[]
        //     {
        //         new Property(null,
        //             new List<Values>()
        //             {
        //                 new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
        //                 new Values(Keywords.Scope, DataType.Int, NodeData.Scope),
        //                 new Values(Keywords.Identifier, DataType.UShort, NodeData.Identifier)
        //             })
        //     },
        //
        //     new[]
        //     {
        //         new Pin(Keywords.Identifier),
        //         new Pin(Keywords.Scope),
        //         new Pin(Keywords.Object),
        //         new Pin(Keywords.Out)
        //     });
        //
        // brain.AddNode(GetterNode);
        //     
        // // GetterNode.Output.Add(brain.NodeMap[destinationId].node);
        //     
        // return new VariableConnection(typeof(bool), typeof(DeclareBooleanNode), destinationId, destinationPin, GetterNode.NodeId, Keywords.Out);
    }

    public override (Node SetterNode, VariableConnection SetterConnection) Setter(ScriptBrain brain, int originId = -1, string originPin = "")
    {
        Node setterNode = new Node(NodeTypes.Boolean_Setter, 
            new[]
            {
                new Property(null, 
                    new List<Values>() { 
                        new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                        new Values(Keywords.Scope, DataType.Int, NodeData.Scope),
                        new Values(Keywords.Identifier, DataType.UShort, NodeData.Identifier),
                        new Values(Keywords.Value, DataType.Bool, (bool)(NodeData.Data ?? false))
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
        
        return (setterNode, new VariableConnection(typeof(bool), typeof(DeclareBooleanNode), setterNode.NodeId, Keywords.Value, originId, originPin));
    }
}