using System.Numerics;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable.Getter;

namespace NodeSharp.Nodes.Variable;

public class DeclareVector3Node : VariableNode
{
    public DeclareVector3Node(ScriptBrain brain, VariableData data, int originId = -1, string originPin = "") : base(brain,
        NodeTypes.Vector3_Declare, data)
    {
        ImplementationNodeData = new[]
        {
            new Property(null,
                new List<Values>()
                {
                    new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
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
            Input.Add(new VariableConnection(typeof(Vector3), typeof(DeclareVector3Node), NodeId, Keywords.InitialValue, originId, originPin));
        }
    }
    
    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        if (GetterNode is not null)
        {
            return GetterNode.Getter(brain, destinationId, destinationPin);
        }       
        
        Vector3GetterNode node = new Vector3GetterNode(brain, NodeData, destinationId, destinationPin);
        GetterNode = node;
        brain.AddNode(node);
            
        brain.AddReferenceToNode(this);
        return GetterNode.Getter(brain, destinationId, destinationPin);
        
        // if (GetterNode is not null)
        // {
        //     // GetterNode.Output.Add(brain.NodeMap[destinationId].node);
        //     return new VariableConnection(typeof(Vector3), typeof(DeclareVector3Node), destinationId, destinationPin, GetterNode.NodeId,
        //         Keywords.Out);
        // }
        //
        // GetterNode = new Node(NodeTypes.Vector3_Getter,
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
        // return new VariableConnection(typeof(Vector3), typeof(DeclareVector3Node), destinationId, destinationPin, GetterNode.NodeId, Keywords.Out);
    }

    public override (Node SetterNode, VariableConnection SetterConnection) Setter(ScriptBrain brain, int originId = -1, string originPin = "")
    {
        Node setterNode = new Node(NodeTypes.Vector3_Setter, 
            new[]
            {
                new Property(null, 
                    new List<Values>() { 
                        new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                        new Values(Keywords.Scope, DataType.Int, NodeData.Scope),
                        new Values(Keywords.Identifier, DataType.UShort, NodeData.Identifier),
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
        
        return (setterNode, new VariableConnection(typeof(Vector3), typeof(DeclareVector3Node), setterNode.NodeId, Keywords.Value, originId, originPin));
    }
}