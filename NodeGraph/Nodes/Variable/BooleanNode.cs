﻿using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp.Nodes.Variable;

public class BooleanNode : VariableNode
{
    public BooleanNode(ScriptBrain brain, VariableData data, int originId = -1, string originPin = "") : base(brain,
        NodeTypes.Boolean, data)
    {
        if (data.Scope == ScopeEnum.Constant)
        {
            ImplementationNodeData = new[]
            {
                new Property(null,
                    new List<Values>()
                    {
                        new Values(Keywords.Out, DataType.Bool, (bool)(data.Data ?? false))
                    })
            };

            Pins = new[]
            {
                new Pin(Keywords.Out, 0)
            };
        }
        else
        {
            NodeType = NodeTypes.Boolean_Declare;
            
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
                brain.AddConnection(new VariableConnection(typeof(bool), NodeID, Keywords.InitialValue, originId, originPin));
        }
    }
    
    public override VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "")
    {
        if (NodeData.Scope == ScopeEnum.Constant)
        {
            return new VariableConnection(typeof(bool), destinationId, destinationPin, NodeID, Keywords.Out);
        }
        
        if (GetterNode is not null)
            return new VariableConnection(typeof(bool), destinationId, destinationPin, GetterNode.NodeID, Keywords.Out);;

        GetterNode = new Node(NodeTypes.Boolean_Getter,
            ImplementationNodeData = new[]
            {
                new Property(null,
                    new List<Values>()
                    {
                        new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                        new Values(Keywords.Scope, DataType.Int, NodeData.Scope),
                        new Values(Keywords.Identifier, DataType.UShort, NodeData.Identifier)
                    })
            },

            Pins = new[]
            {
                new Pin(Keywords.Identifier),
                new Pin(Keywords.Scope),
                new Pin(Keywords.Object),
                new Pin(Keywords.Out)
            });

        brain.AddNode(GetterNode);

        return new VariableConnection(typeof(bool), destinationId, destinationPin, GetterNode.NodeID, Keywords.Out);
    }

    public override (Node Setter, VariableConnection SetterConnection) Setter(ScriptBrain brain, int originId = -1, string originPin = "")
    {
        if (NodeData.Scope == ScopeEnum.Constant)
            throw new Exception("Trying to set a constant variable");
        
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
        
        return (setterNode, new VariableConnection(typeof(bool), setterNode.NodeID, Keywords.Value, originId, originPin));
    }
}