using Antlr4.Runtime.Tree;
using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp.Visitor;

/// <summary>
/// Holds functions related to SCOPE
/// </summary>
public static class ScopeVisitor
{
    /// <summary>
    /// Handles converting a ANTLR node into a scope
    /// </summary>
    /// <param name="scope"> The ANTLR node of a scope </param>
    /// <returns> ScopeEnum </returns>
    public static ScopeEnum GetScope(ITerminalNode scope)
    {
        if (scope == null)
            return ScopeEnum.Constant;

        return GetScope(scope.GetText());
    }
    
    /// <summary>
    /// Handles converting a string from ANTLR into a scope
    /// </summary>
    /// <param name="scope"> The string interpretation of a scope </param>
    /// <returns> ScopeEnum </returns>
    /// <exception cref="NotImplementedException"> Throws if the scope is invalid </exception>
    public static ScopeEnum GetScope(object? scope)
    {
        if (scope is not string s || s == "")
            return ScopeEnum.Constant;

        var identifier = s[..6].ToLower();
        return identifier switch
        {
            "local " => ScopeEnum.Local,
            "global" => ScopeEnum.Global,
            "object" => ScopeEnum.Object,
            _ => throw new NotImplementedException()
        };
    }
}