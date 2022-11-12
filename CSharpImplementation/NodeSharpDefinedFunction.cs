using Antlr4.Runtime.Tree;
using NodeSharp.Grammar;
using NodeSharp.Nodes.Interface;

namespace NodeSharp;

public class NodeSharpDefinedFunction
{
    public List<ITerminalNode> FunctionParameters = new List<ITerminalNode>();
    public NodeSharpParser.BlockContext? FunctionBlock;
    public IActionNode? EventNode = null;
    public string FunctionName;
    
    public NodeSharpDefinedFunction(string funcName, NodeSharpParser.FunctionCreationParametersContext[]? parameters = null, NodeSharpParser.BlockContext? funcBlock = null)
    {
        if (parameters != null && parameters?.Length == 1)
        {
            FunctionParameters = parameters[0].IDENTIFIER().ToList();
        }
        else
        {
            FunctionParameters = new List<ITerminalNode>();
        }

        FunctionBlock = funcBlock;
        FunctionName = funcName;
    }
}