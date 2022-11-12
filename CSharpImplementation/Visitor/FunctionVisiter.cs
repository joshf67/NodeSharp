using NodeSharp.Functions;
using NodeSharp.Grammar;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Events_Custom;
using NodeSharp.Nodes.Interface;

namespace NodeSharp.Visitor;

public static class FunctionVisitor
{
    /// <summary>
    /// Stores any local scope functions created by N# code to be called later
    /// </summary>
    public static Dictionary<string, NodeSharpDefinedFunction> NsLocalFunctions { get; } = new();
    
    /// <summary>
    /// Stores any global scope functions created by N# code to be called later
    /// </summary>
    public static Dictionary<string, NodeSharpDefinedFunction> NsGlobalFunctions { get; } = new();

    /// <summary>
    /// Stores any functions exposed to N# to be called later
    /// </summary>
    public static Dictionary<string, Func<ScriptBrain, List<object>, Node?>> InternalFunctions { get; } = new()
    {
        //Setup base functions
        { "sqrt", MathFunctions.sqrt }
    };

    /// <summary>
    /// Handles creating functions from N# code
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> null </returns>
    /// <exception cref="InvalidOperationException"> Fails if the function already exists </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Fails if scope is invalid </exception>
    public static object? VisitFunctionCreation(NodeSharpParser.FunctionCreationContext context)
    {
        var newFunc = new NodeSharpDefinedFunction(context.IDENTIFIER().GetText(), context.functionCreationParameters(), context.block());

        return FunctionCreation(newFunc, ScopeVisitor.GetScope(context.SCOPE()));
    }
    
    public static object? FunctionCreation(NodeSharpDefinedFunction func, ScopeEnum scope)
    {
        switch (scope)
        {
            case ScopeEnum.Local:
                if (NsLocalFunctions.ContainsKey(func.FunctionName))
                    throw new InvalidOperationException($"Function '{func.FunctionName}' already exists in local scope");
                NsLocalFunctions.Add(func.FunctionName, func);
                break;
            case ScopeEnum.Global:
                if (NsGlobalFunctions.ContainsKey(func.FunctionName))
                    throw new InvalidOperationException($"Function '{func.FunctionName}' already exists in global scope");
                NsGlobalFunctions.Add(func.FunctionName, func);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Trying to create function {func.FunctionName} in invalid scope");
        }

        return null;
    }
    
    /// <summary>
    /// Handles ANTLR calling any functions
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static object? VisitFunctionCall(NodeSharpVisitorRefactor parentVisitor, ScriptBrain brain, NodeSharpParser.FunctionCallContext context)
    {
        var funcName = context.IDENTIFIER().GetText();
        var funcParams = new List<object>();
        if (context.functionParameters() != null) {
            funcParams = (parentVisitor.Visit(context.functionParameters()) ?? new List<object>()) as List<object>;
        }

        return FunctionCall(parentVisitor, brain, funcName, funcParams);
    }
    

    public static object? FunctionCall(NodeSharpVisitorRefactor parentVisitor, ScriptBrain brain, string funcName, List<object> funcParams)
    {
        if (InternalFunctions.ContainsKey(funcName))
        {
            return InternalFunctions[funcName](brain, funcParams);
        } 
        else if (NsLocalFunctions.ContainsKey(funcName))
        {
            return FunctionCall(parentVisitor, brain, NsLocalFunctions[funcName], funcParams, ScopeEnum.Local);
        }
        else if (NsGlobalFunctions.ContainsKey(funcName))
        {
            return FunctionCall(parentVisitor, brain, NsLocalFunctions[funcName], funcParams, ScopeEnum.Global);
        }

        throw new NotImplementedException($"The function '{funcName}' does not exist");
    }
    
    /// <summary>
    /// Handles calling any user defined U# functions
    /// </summary>
    /// <param name="parentVisitor"> Visitor that called this function </param>
    /// <param name="brain"> The ScriptBrain that this node is being added to </param>
    /// <param name="functionDefinition"> The function data </param>
    /// <param name="functionParamters"> The parameters to send to the function </param>
    /// <param name="scope"> The scope of the function </param>
    /// <returns> The result/last node of the function </returns>
    /// <exception cref="NotImplementedException"> Invalid parameter count </exception>
    /// <exception cref="NotImplementedException"> Invalid parameters </exception>
    public static (IActionNode eventTrigger, IActionNode onEvent) FunctionCall(NodeSharpVisitorRefactor parentVisitor, ScriptBrain brain, NodeSharpDefinedFunction functionDefinition, List<object> functionParamters, ScopeEnum scope)
    {
        if (functionParamters.Count != functionDefinition.FunctionParameters.Count)
            throw new NotImplementedException($"Default/extra parameters are not currently valid in function calls");

        for (int i = 0; i < functionDefinition.FunctionParameters.Count; i++)
        {
            if (functionParamters[i] is not IGetter)
                throw new NotImplementedException($"user defined functions do not support non-node variables");
            
            VariableVisitor.SetVariable(parentVisitor, brain, functionDefinition.FunctionParameters[i].GetText(), (IGetter)functionParamters[i], ScopeEnum.Local);
        }

        var identifier = parentVisitor.IdentifierVariables.Count;
        if (parentVisitor.IdentifierVariables.ContainsKey(functionDefinition.FunctionName))
            identifier = parentVisitor.IdentifierVariables[functionDefinition.FunctionName].Order;
        
        var variableData = new VariableData(null, identifier, functionDefinition.FunctionName, scope);
        var eventCaller = new TriggerCustomEventNode(brain, variableData);

        parentVisitor.CurrentActionScope = parentVisitor.CurrentActionScope.AddScope(eventCaller,
            parentVisitor.CurrentActionScope.ActionNode.GetActionSendPin());
        var eventCallerScope = parentVisitor.CurrentActionScope;
        
        if (functionDefinition.EventNode == null)
        {
            functionDefinition.EventNode = new OnCustomEventNode(brain, variableData);
            parentVisitor.CurrentActionScope =
                parentVisitor.CurrentActionScope.AddScope(functionDefinition.EventNode, null);

            if (functionDefinition.FunctionBlock != null) parentVisitor.Visit(functionDefinition.FunctionBlock);
            parentVisitor.IdentifierVariables.Add(functionDefinition.FunctionName,
                new Identifier(functionDefinition.FunctionName, parentVisitor.IdentifierVariables.Count));
        }
        
        parentVisitor.IdentifierVariables[functionDefinition.FunctionName].AddUse();
        parentVisitor.CurrentActionScope = eventCallerScope;
        return (eventCaller, functionDefinition.EventNode);
    }
}