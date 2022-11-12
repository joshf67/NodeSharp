using NodeSharp.Grammar;
using NodeSharp.NodeGraph.Nodes;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Functions;

public class MathFunctions
{
    public static Node? sqrt(ScriptBrain brain, List<object> args)
    {
        if (args.Count != 1)
            throw new InvalidOperationException($"Square root called with invalid parameters");

        if (args[0] is not IGetter getter)
            throw new NotImplementedException($"sqrt does not accept non-getter parameters");
        
        return new SquareRootNumberNode(brain, getter);
    }
}