using System.Diagnostics;
using System.Numerics;
using Antlr4.Runtime.Tree;
using InfiniteForgeConstants.NodeGraphSettings;
using NodeSharp.Grammar;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.NodeGraph.Nodes;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp;

public class NodeSharpVisitorRefactor : NodeSharpParserBaseVisitor<object?>
{
    public static NodeSharpVisitorRefactor instance;
    public ScriptBrain testBrain = new ScriptBrain();
    
    /// <summary>
    /// Stores any nodes that exist in constant scope
    /// </summary>
    private Dictionary<object, VariableNode> ConstantVariables { get; } = new();
    
    /// <summary>
    /// Stores any nodes that exist in local scope
    /// </summary>
    private Dictionary<string, VariableNode> LocalVariables { get; } = new();
    
    /// <summary>
    /// Stores any nodes that exist in global scope
    /// </summary>
    private Dictionary<string, VariableNode> GlobalVariables { get; } = new();
    
    /// <summary>
    /// Stores any nodes that require identifiers to be setup
    /// </summary>
    public Dictionary<string, VariableNode> IdentifierVariables { get; } = new();

    public NodeSharpVisitorRefactor()
    {
        instance = this;
    }
    
    /// <summary>
    /// Handles converting a ANTLR node into a scope
    /// </summary>
    /// <param name="scope"> The ANTLR node of a scope </param>
    /// <returns> ScopeEnum </returns>
    private static ScopeEnum GetScope(ITerminalNode scope)
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
    private static ScopeEnum GetScope(object? scope)
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
    
    /// <summary>
    /// Handles finding variables and stopping execution if the variable required does not exist
    /// </summary>
    /// <param name="name"> The name of the variable to find </param>
    /// <param name="scope"> The scope of the variable to find </param>
    /// <param name="throwIfNull"> Controls whether this function call can return null </param>
    /// <returns> Variable Node/NULL</returns>
    /// <exception cref="Exception"> Throws if no variable exists and throwIfNull is true </exception>
    private VariableNode? GetVariableData(string name, ScopeEnum scope = 0, bool throwIfNull = true)
    {
        if (scope == ScopeEnum.Local)
        {
            if (!LocalVariables.ContainsKey(name))
            {
                if (!throwIfNull)
                    return null;
                
                throw new Exception($"Variable {name} is not defined in local scope");
            }

            return LocalVariables[name];
        }
        else if (scope == ScopeEnum.Global)
        {
            if (!GlobalVariables.ContainsKey(name))
            {
                if (!throwIfNull)
                    return null;
                throw new Exception($"Variable {name} is not defined in global scope");
            }

            return GlobalVariables[name];
        }

        if (LocalVariables.ContainsKey(name))
        {
            return LocalVariables[name];
        }

        if (GlobalVariables.ContainsKey(name))
            return GlobalVariables[name];

        // if (InternalVariables.ContainsKey(name))
        //     return InternalVariables[name];

        if (!throwIfNull)
            return null;
        
        throw new Exception($"Variable {name} is not defined in any scope");
    }
    
    /// <summary>
    /// Handles setting variables to values
    /// </summary>
    /// <param name="name"> The name of the variable being set </param>
    /// <param name="setTo"> The IGetter variable that the variable should be set to </param>
    /// <param name="scope"> The scope the variable being set should be in </param>
    /// <returns> Void </returns>
    private void SetVariable(string name, IGetter setTo, ScopeEnum scope = 0)
    {
        VariableNode data = GetVariableData(name, scope, false);
        if (data != null)
        {
            var Setter = data.Setter(testBrain);
            var Getter = setTo.Getter(testBrain, Setter.SetterConnection.DestinationNode, Setter.SetterConnection.DestinationPin);
            if (Setter.SetterConnection.ConnectionNodeType != Getter.ConnectionNodeType)
                throw new Exception($"Unable to set variable {name} to value type {Getter.ConnectionNodeType} as their types are different");
            
            
        }
        else
        {
            var Getter = setTo.Getter(testBrain);
            VariableData NodeData = new VariableData(Activator.CreateInstance(Getter.VariableType), IdentifierVariables.Count, name, scope);
            
            VariableNode? Node = null;
            
            if (testBrain.NodeMap[Getter.OriginNode].node is VariableNode getterNode && getterNode.NodeData.Scope == ScopeEnum.Constant)
            {
                if (Getter.ConnectionNodeType.GetInterface(nameof(IDefaultable)) != null)
                {
                    Node = (VariableNode)Activator.CreateInstance(Getter.ConnectionNodeType, testBrain, NodeData, -1, "");
                }
                else
                {
                    Node = (VariableNode)Activator.CreateInstance(Getter.ConnectionNodeType, testBrain, NodeData, Getter.OriginNode, Getter.OriginPin);
                }
            }
            else
            {
                Node = (VariableNode)Activator.CreateInstance(Getter.ConnectionNodeType, testBrain, NodeData, Getter.OriginNode, Getter.OriginPin);
            }
            
            if (Node is null)
                throw new NotImplementedException(
                    $"Node of type {Getter.ConnectionNodeType} has not been setup yet");
            
            IdentifierVariables.Add(name, Node);
            switch (scope)
            {
                case ScopeEnum.Global:
                    GlobalVariables[name] = Node;
                    return;
                case ScopeEnum.Local:
                    LocalVariables[name] = Node;
                    return;
            }
        }
        
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Handles visits that get variables 
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Variable Data/NULL</returns>
    public override object? VisitIdentifierExpression(NodeSharpParser.IdentifierExpressionContext context)
    {
        return GetVariableData(context.IDENTIFIER().GetText(), GetScope(context.SCOPE()));
    }
    
    /// <summary>
    /// Handles visits that set variables
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> NULL </returns>
    public override object? VisitAssignment(NodeSharpParser.AssignmentContext context)
    {
        var varName = context.IDENTIFIER().GetText();
        
        var scope = GetScope(context.SCOPE());        
        if (scope == ScopeEnum.Constant)
            throw new Exception($"Currently unable to set variables without scope");

        var value = Visit(context.expression());

        if (value is not IGetter val)
            throw new NotImplementedException();
        
        SetVariable(varName, val, GetScope(context.SCOPE()));

        return null;
    }
    
    /// <summary>
    /// Handles looking up and returning constant variables
    /// </summary>
    /// <param name="value"> The variable to look for </param>
    /// <returns> Variable Node/NULL</returns>
    private VariableNode? GetConstantVariable(object? value)
    {
        if (ConstantVariables.ContainsKey(value))
            return ConstantVariables[value];

        return null;
    }

    /// <summary>
    /// Handles any visits that result in a constant variable
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Constant Variable Node</returns>
    public override object? VisitConstant(NodeSharpParser.ConstantContext context)
    {
        VariableNode? ret = null;
        
        if (context.NUMBER() is { } i)
        {
            ret = GetConstantVariable(float.Parse(i.GetText()));
            if (ret != null) return ret;
            
            VariableData NodeData = new VariableData(float.Parse(i.GetText()), scope: ScopeEnum.Constant);
            ret = new DeclareNumberNode(testBrain, NodeData);
            ConstantVariables[float.Parse(i.GetText())] = ret;

            return ret;
        }
        
        if (context.VECTOR3() is {} v)
        {
            var values = v.GetText().Split(",");
            var value = new Vector3(float.Parse(values[0][8..]),
                float.Parse(values[1].Replace(" ", "")),
                float.Parse(values[2].Replace(" ", "")[..^1]));
            
            ret = GetConstantVariable(value);
            if (ret != null) return ret;
            
            VariableData NodeData = new VariableData(value, scope: ScopeEnum.Constant);
            ret = new DeclareVector3Node(testBrain, NodeData);
            ConstantVariables[value] = ret;

            return ret;
        }

        if (context.STRING() is {} s)
        {
            var value = (int)System.Enum.Parse(typeof(StringTypes), s.GetText()[7..], true);
                
            ret = GetConstantVariable(value);
            if (ret != null) return ret;
            
            VariableData NodeData = new VariableData(value, scope: ScopeEnum.Constant);
            ret = new DeclareStringNode(testBrain, NodeData);
            ConstantVariables[value] = ret;

            return ret;
        }

        if (context.BOOL() is {} b)
        {
            ret = GetConstantVariable(bool.Parse(b.GetText()));
            if (ret != null) return ret;
            
            VariableData NodeData = new VariableData(bool.Parse(b.GetText()), scope: ScopeEnum.Constant);
            ret = new DeclareBooleanNode(testBrain, NodeData);
            ConstantVariables[bool.Parse(b.GetText())] = ret;

            return ret;
        }

        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Handles visits when expressions are wrapped in parenthesies
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Expression </returns>
    public override object? VisitParenthesizedExpression(NodeSharpParser.ParenthesizedExpressionContext context)
    {
        return Visit(context.expression());
    }
    
    /// <summary>
    /// Handles comparison visits
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Comparison Node</returns>
    public override object? VisitComparisonExpression(NodeSharpParser.ComparisonExpressionContext context)
    {       
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));
        
        var op = context.comparisonOp().GetText();
        return NMath.Compare(op, left, right);
    }

    /// <summary>
    /// Handles addition and subtraction visits
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Addition/Subtraction Node</returns>
    public override object? VisitAdditiveExpression(NodeSharpParser.AdditiveExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.additionOp().GetText();
        return NMath.Addition(testBrain, op, left, right);
    }

    /// <summary>
    /// Handles multiplication and division visits
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Multiplication/Division Node</returns>
    public override object? VisitMultiplicativeExpression(NodeSharpParser.MultiplicativeExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.multiplyOp().GetText();
        return NMath.Multiplication(testBrain, op, left, right);
    }
    
    /// <summary>
    /// Handles basic trigonometry visits that use 1 parameter
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Trigonometry Node</returns>
    public override object? VisitTrigonometryExpression(NodeSharpParser.TrigonometryExpressionContext context)
    {
        var arg = Visit(context.expression());

        if (arg is null)
            throw new Exception($"Cannot {context.trigonometryOp().GetText()} argument because it does not exist {arg?.GetType()}.");

        if (arg is VariableData { Data: float or double } variable)
        {
            var op = context.trigonometryOp().GetText();
            return op switch
            {
                "sin" => NMath.Sin(variable),
                "cos" => NMath.Cos(variable),
                "tan" => NMath.Tan(variable),
                "arcsin" => NMath.ArcSin(variable),
                "arccos" => NMath.ArcCos(variable),
                _ => throw new NotImplementedException()
            };
        }

        throw new Exception($"Cannot {context.trigonometryOp().GetText()} argument because it is not a number {arg?.GetType()}.");
    }
    
    /// <summary>
    /// Handles ArcTan2 visit as it requires two parameters
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> ArcTan2 Node</returns>
    public override object? VisitTrigonometryArcTan2Expression(NodeSharpParser.TrigonometryArcTan2ExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        if (left is VariableData { Data: float or double } l && right is VariableData { Data: float or double } r)
            return NMath.Atan2(l, r);
        
        throw new Exception($"Cannot ArcTan2 arguments because one of them is not a number {left?.GetType()}, {right?.GetType()}.");
    }
}