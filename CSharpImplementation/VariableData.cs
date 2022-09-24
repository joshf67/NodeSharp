using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp;

public class VariableData
{
    public object? Data = null;
    public int Identifier = -1;
    public string IdentifierName = "";
    public ScopeEnum Scope;
    
    public VariableData(object? data, int identifier = -1, string identifierName = "", ScopeEnum scope = ScopeEnum.Constant)
    {
        Data = data;
        Identifier = identifier;
        IdentifierName = identifierName;
        Scope = scope;
    }
}