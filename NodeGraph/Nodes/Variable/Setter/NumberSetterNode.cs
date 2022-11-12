using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Nodes.Variable.Setter;

public class NumberSetterNode : Node, ISetterOptimizable, IDefaultable, IActionNode
{
    public NumberSetterNode(ScriptBrain brain, VariableData data, int originId = -1, string originPin = "") :
        base(brain, NodeTypes.NumberSetter)
    {
        ImplementationNodeData = new[]
        {
            new Property(null,
                new List<Values>()
                {
                    new Values(Keywords.ForgeNodeGraph_DisabledFlag, DataType.Bool, true),
                    new Values(Keywords.Scope, DataType.Int, data.Scope),
                    new Values(Keywords.Identifier, DataType.UShort, data.Identifier),
                    new Values(Keywords.Value, DataType.Float, (float)Math.Round(Convert.ToSingle(data.Data), 2))
                })
        };
        
        Pins = new[]
        {
            new Pin(Keywords.ActionStart),
            new Pin(Keywords.Identifier),
            new Pin(Keywords.Object),
            new Pin(Keywords.Value),
            new Pin(Keywords.Scope),
            new Pin(Keywords.ActionComplete)
        };
        
        if (originId != -1)
        {
            Input.Add(new VariableConnection(data.Data, typeof(NumberNode), NodeId, Keywords.Value, originId, originPin));
        }
    }

    public void DefaultSetter(object? value)
    {
        if (value.GetType() != typeof(float))
            throw new InvalidOperationException(
                $"Default setter for Number does not accept {value.GetType()} type, expected: float");

        ImplementationNodeData[0].Values[3].DataValue = value;
        Input.Clear();
    }
}