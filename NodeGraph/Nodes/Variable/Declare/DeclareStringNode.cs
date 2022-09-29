using InfiniteForgeConstants.NodeGraphSettings;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable;

public class DeclareStringNode : VariableNode, IDefaultable
{
    public DeclareStringNode(ScriptBrain brain, VariableData data, int originId = -1, string originPin = "") : base(brain, NodeTypes.String, data)
    {
        if (data.Scope == ScopeEnum.Constant)
        {
            ImplementationNodeData = new[]
            {
                new Property(null,
                    new List<Values>()
                    {
                        new Values(Keywords.Out, DataType.Int, (int)(data.Data ?? 0))
                    })
            };

            Pins = new[]
            {
                new Pin(Keywords.Out, 0)
            };
        }
        else
        {
            NodeType = NodeTypes.String_Declare;
            
            ImplementationNodeData = new[]
            {
                new Property(null,
                    new List<Values>()
                    {
                        // new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                        new Values(Keywords.InitialValue, DataType.Int, (int)(data.Data ?? 0)),
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
                Input.Add(new VariableConnection(typeof(int), typeof(DeclareStringNode), NodeID, Keywords.InitialValue, originId,
                    originPin));
            }
        }
    }

    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        if (NodeData.Scope == ScopeEnum.Constant)
        {
            // if (destinationId != -1)
                // Output.Add(brain.NodeMap[destinationId].node);
            return new VariableConnection(typeof(int), typeof(DeclareStringNode), destinationId, destinationPin, NodeID, Keywords.Out);
        }

        if (GetterNode is not null)
        {
            // GetterNode.Output.Add(brain.NodeMap[destinationId].node);
            return new VariableConnection(typeof(int), typeof(DeclareStringNode), destinationId, destinationPin, GetterNode.NodeID,
                Keywords.Out);
        }

        GetterNode = new Node(NodeTypes.String_Getter,
            new[]
            {
                new Property(null,
                    new List<Values>()
                    {
                        new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                        new Values(Keywords.Scope, DataType.Int, NodeData.Scope),
                        new Values(Keywords.Identifier, DataType.UShort, NodeData.Identifier)
                    })
            },

            new[]
            {
                new Pin(Keywords.Identifier),
                new Pin(Keywords.Scope),
                new Pin(Keywords.Object),
                new Pin(Keywords.Out)
            });

        brain.AddNode(GetterNode);

        // GetterNode.Output.Add(brain.NodeMap[destinationId].node);
            
        return new VariableConnection(typeof(int), typeof(DeclareStringNode), destinationId, destinationPin, GetterNode.NodeID, Keywords.Out);
    }

    public override (Node Setter, VariableConnection SetterConnection) Setter(ScriptBrain brain, int originId = -1, string originPin = "")
    {
        if (NodeData.Scope == ScopeEnum.Constant)
            throw new Exception("Trying to set a constant variable");
        
        Node setterNode = new Node(NodeTypes.String_Setter, 
            new[]
            {
                new Property(null, 
                    new List<Values>() { 
                        new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                        new Values(Keywords.Scope, DataType.Int, NodeData.Scope),
                        new Values(Keywords.Identifier, DataType.UShort, NodeData.Identifier),
                        new Values(Keywords.Value, DataType.Int, (int)(NodeData.Data ?? 0))
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
        
        return (setterNode, new VariableConnection(typeof(int), typeof(DeclareStringNode), setterNode.NodeID, Keywords.Value, originId, originPin));
    }
}