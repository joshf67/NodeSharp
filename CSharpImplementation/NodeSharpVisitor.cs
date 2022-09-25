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

public class NSharpVisitor : NodeSharpParserBaseVisitor<object?>
{

    public static NSharpVisitor instance;
    public ScriptBrain testBrain = new ScriptBrain();
    
    private Dictionary<object, VariableNode> ConstantVariables { get; } = new();
    private Dictionary<string, VariableNode> LocalVariables { get; } = new();
    private Dictionary<string, VariableNode> GlobalVariables { get; } = new();

    public Dictionary<string, VariableNode> IdentifierVariables { get; } = new();
    //public Dictionary<string, VariableNode> NodeVariables { get; } = new();

    // private Dictionary<string, Func<object?[], object?>> InternalFunctions { get; } = new();
    private Dictionary<string, Func<object?[], object?>> LocalFunctions { get; } = new();
    private Dictionary<string, Func<object?[], object?>> GlobalFunctions { get; } = new();
    
    
    public Dictionary<string, VariableNode> InternalVariables { get; } = new();

    public NSharpVisitor()
    {
        instance = this;
        GlobalFunctions["Write"] = new Func<object?[], object?>(Write);
        
        // var NodeID = testBrain.AddNode(new DeclareNumberNode(0, (int)ScopeEnum.Global));
        // var data = new VariableData(0f, NodeID, IdentifierVariables.Count, scope: ScopeEnum.Global);
        // data.IdentifierName = "InternalFloat";
        //
        // IdentifierVariables.Add("InternalFloat", data);
        // InternalVariables.Add("InternalFloat", data);
        
        VariableData NodeData = new VariableData(0f, IdentifierVariables.Count, "InternalFloat", ScopeEnum.Global);
        VariableNode Node = new Nodes.Variable.NumberNode(testBrain, NodeData);
        IdentifierVariables.Add("InternalFloat", Node);
        InternalVariables.Add("InternalFloat", Node);
        
        // InternalFunctions.Add("Sin", state => NMath.Sin);
    }

    private object? Write(object?[] args)
    {
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }

        return null;
    }

    // public override object? VisitFunction(NodeSharpParser.FunctionContext context)
    // {
    //     var funcName = context.IDENTIFIER().GetText();
    //     if (context.SCOPE().GetText() == "global ")
    //     {
    //         if (GlobalFunctions.ContainsKey(funcName))
    //             throw new Exception($"Function {funcName} already exists");
    //             
    //             //GlobalFunctions[funcName] = new Func<object?[], object?>()
    //     }
    //     else
    //     {
    //         if (LocalFunctions.ContainsKey(funcName))
    //             throw new Exception($"Function {funcName} already exists");
    //         
    //         
    //     }
    //     
    //     return null;
    // }

    public override object? VisitArray(NodeSharpParser.ArrayContext context)
    {
        var objectArr = context.expression_list().expression().Select(Visit).ToArray();
        return objectArr;
    }

    private ScopeEnum GetScope(ITerminalNode scope)
    {
        if (scope == null)
            return ScopeEnum.Constant;

        return GetScope(scope.GetText());
    }

    private ScopeEnum GetScope(object? scope)
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

    private object? GetVariable(string name, ScopeEnum scope = 0)
    {
        return GetVariableData(name, scope);
    }

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

        if (InternalVariables.ContainsKey(name))
            return InternalVariables[name];

        if (!throwIfNull)
            return null;
        
        throw new Exception($"Variable {name} is not defined in any scope");
    }

    // private void SetLocalVariable(string name, object? value)
    // {
    //     VariableData data = GetVariableData(name, ScopeEnum.Local, false);
    //     if (data == null)
    //     {
    //         int ID = -1;
    //         if (value is float f)
    //             ID = testBrain.AddNode(new DeclareNumberNode(f, (int)ScopeEnum.Local));
    //
    //         LocalVariables[name] = new VariableData(value, ID);
    //     }
    //     else
    //     {
    //         if (value is float f) 
    //             testBrain.AddNode(new SetNumberNode(f, (int)ScopeEnum.Local));
    //     }
    //         
    //     LocalVariables[name].Data = value;
    // }
    //
    // private void SetGlobalVariable(string name, object? value)
    // {
    //     VariableData data = GetVariableData(name, ScopeEnum.Global, false);
    //     if (data == null)
    //     {
    //         int ID = -1;
    //         if (value is float f)
    //             ID = testBrain.AddNode(new DeclareNumberNode(f, (int)ScopeEnum.Global));
    //
    //         GlobalVariables[name] = new VariableData(value, ID);
    //     }
    //     else
    //     {
    //         if (value is float f) 
    //             testBrain.AddNode(new SetNumberNode(f, (int)ScopeEnum.Global));
    //     }
    //         
    //     GlobalVariables[name].Data = value;
    // }

    /*
     *
     *  Implement way to use new VariableNode Getter/Setter instead
     * 
     */
    private void SetVariable(string name, GetterInterface setTo, ScopeEnum scope = 0)
    {
        VariableNode data = GetVariableData(name, scope, false);
        if (data != null)
        {
            // if (setTo.NodeData.Data is float && data.NodeData.Data is float)
            // {
            //     
            //     // var SetterNodeID =
            //     //     testBrain.AddNode(new SetNumberNode((float)variable.Data, (int)scope, data.Identifier));
            //     // var GetterNodeID = 
            //     //     testBrain.AddNode(new GetNumberNode((int)scope, data.Identifier));
            //     // testBrain.AddConnection(new GetNumberConnection(GetterNodeID, SetterNodeID, Keywords.Value));
            //     return;
            // }
            
            var Setter = data.Setter(testBrain);
            var Getter = setTo.Getter(testBrain, Setter.SetterConnection.DestinationNode, Setter.SetterConnection.DestinationPin);
            if (Setter.SetterConnection.ConnectionVariableType != Getter.ConnectionVariableType)
                throw new Exception($"Unable to set variable {name} to value type {Getter.ConnectionVariableType} as their types are different");
            
            testBrain.AddConnection(Getter);
            
            throw new NotImplementedException();
        }
        else
        {
            // if (setTo.NodeData.Data is not float)
            //     throw new NotImplementedException();

            var Getter = setTo.Getter(testBrain);
            VariableData NodeData = new VariableData(Activator.CreateInstance(Getter.ConnectionVariableType), IdentifierVariables.Count, name, scope);
            VariableNode? Node = null;
            
            if (Getter.ConnectionVariableType == typeof(float))
                Node = new NumberNode(testBrain, NodeData, Getter.OriginNode, Getter.OriginPin);

            if (Getter.ConnectionVariableType == typeof(bool))
                Node = new BooleanNode(testBrain, NodeData, Getter.OriginNode, Getter.OriginPin);

            if (Getter.ConnectionVariableType == typeof(StringTypes))
                Node = new StringNode(testBrain, NodeData, Getter.OriginNode, Getter.OriginPin);
            
            if (Getter.ConnectionVariableType == typeof(Vector3))
                Node = new Vector3Node(testBrain, NodeData, Getter.OriginNode, Getter.OriginPin);

            if (Node is null)
                throw new NotImplementedException(
                    $"Node of type {Getter.ConnectionVariableType} has not been setup yet");
            
            IdentifierVariables.Add(name, Node);
            
            // var NodeID = testBrain.AddNode(new DeclareNumberNode(0, (int)scope));
            // data = new VariableData(variable.Data, NodeID, IdentifierVariables.Count, scope: scope);
            // data.IdentifierName = name;
            // IdentifierVariables.Add(name, data);

            // if (setTo.NodeData.Scope == ScopeEnum.Constant)
            // {
            //     //testBrain.AddConnection(new NumberConnection(variable.NodeID, NodeID));
            // }
            // else
            // {
            //     //var GetterNodeID = 
            //     //testBrain.AddNode(new GetNumberNode((int)scope, data.Identifier));
            //     //testBrain.AddConnection(new NumberConnection(GetterNodeID, NodeID));
            // }

            
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
    
    // private void SetVariable(string name, object? value, ScopeEnum scope = 0)
    // {
    //     if (scope == ScopeEnum.Global)
    //     {
    //         SetGlobalVariable(name, value);
    //     }
    //     // else if (scope == ScopeEnum.Local)
    //     // {
    //     //     SetLocalVariable(name, value);
    //     // }
    //     
    //     SetLocalVariable(name, value);
    //     
    //     // if (value is float val) 
    //     //     testBrain.AddNode(new NumberNode(val));
    //     //
    //     // LocalVariables[name] = value;
    // }

    public override object? VisitGetProperty(NodeSharpParser.GetPropertyContext context)
    {
        if (context.IDENTIFIER().GetValue(0) is not ITerminalNode l)
            throw new Exception($"Value is invalid: {context.IDENTIFIER().GetValue(0)}");
        
        if (context.IDENTIFIER().GetValue(1) is not ITerminalNode i)
            throw new Exception($"Value accessor is invalid: {context.IDENTIFIER().GetValue(1)}");

        var variable = GetVariable(l.GetText(), GetScope(context.SCOPE()));
        if (variable == null)
            throw new Exception($"object is invalid: {l}");

        var prop = variable.GetType().GetField(i.GetText());
        if (prop == null)
            throw new Exception($"Property {i.GetText()} does not exist on object: {l}");
        
        return prop.GetValue(variable);
    }

    public override object? VisitIndexLookup(NodeSharpParser.IndexLookupContext context)
    {
        var varName = context.IDENTIFIER().GetText();

        if (GetVariable(varName, GetScope(context.SCOPE())) is not object[] variable)
            throw new Exception($"Variable {varName} is not an array");

        var lookupIndex = Visit(context.expression());
        if (lookupIndex is not float lookup)
            throw new Exception($"Lookup index {lookupIndex} is not a number");
            
        if (lookup < 0 || lookup > variable.Length)
            throw new Exception($"Trying to access Array at invalid index: {lookupIndex}");

        return variable[(int)lookup];
    }

    // public override object? VisitFunctionCall(NodeSharpParser.FunctionCallContext context)
    // {
    //     var funcName = context.IDENTIFIER().GetText();
    //     var funcScope = context.SCOPE()?.GetText();
    //     var args = context.expression_list().expression().Select(Visit).ToArray();
    //
    //     // if (InternalFunctions.ContainsKey(funcName))
    //     //     return InternalFunctions[funcName](args);
    //
    //     if (LocalFunctions.ContainsKey(funcName) && funcScope != "global ") 
    //         return LocalFunctions[funcName](args);
    //
    //     if (!GlobalFunctions.ContainsKey(funcName) && funcScope != "local")
    //         throw new Exception($"Function {funcName} is not defined");
    //
    //     return GlobalFunctions[funcName](args);
    // }

    public override object? VisitIdentifierExpression(NodeSharpParser.IdentifierExpressionContext context)
    {
        return GetVariable(context.IDENTIFIER().GetText(), GetScope(context.SCOPE()));
    }

    public override object? VisitAssignment(NodeSharpParser.AssignmentContext context)
    {
        var varName = context.IDENTIFIER().GetText();
        
        var scope = GetScope(context.SCOPE());        
        if (scope == ScopeEnum.Constant)
            throw new Exception($"Currently unable to set variables without scope");

        var value = Visit(context.expression());

        if (value is not GetterInterface val)
            throw new NotImplementedException();
        
        SetVariable(varName, val, GetScope(context.SCOPE()));

        return null;
    }
    
    private VariableNode? GetConstantVariable(object? value)
    {
        if (ConstantVariables.ContainsKey(value))
            return ConstantVariables[value];

        return null;



        // foreach (var pair in ConstantVariables)
        // {
        //     if (pair.Value.Data == value)
        //     {
        //         return pair.Value;
        //     }
        // }
        //
        // return null;
    }

    public override object? VisitConstant(NodeSharpParser.ConstantContext context)
    {
        VariableNode? ret = null;
        
        if (context.NUMBER() is { } i)
        {
            ret = GetConstantVariable(float.Parse(i.GetText()));
            if (ret == null)
            {
                VariableData NodeData = new VariableData(float.Parse(i.GetText()), scope: ScopeEnum.Constant);
                ret = new NumberNode(testBrain, NodeData);
                ConstantVariables[float.Parse(i.GetText())] = ret;
            }

            return ret;
        }
        
        if (context.VECTOR3() is {} v)
        {
            var values = v.GetText().Split(",");
            var value = new Vector3(float.Parse(values[0][8..]),
                float.Parse(values[1].Replace(" ", "")),
                float.Parse(values[2].Replace(" ", "")[..^1]));
            
            ret = GetConstantVariable(value);
            if (ret == null)
            {
                VariableData NodeData = new VariableData(value, scope: ScopeEnum.Constant);
                ret = new Vector3Node(testBrain, NodeData);
                ConstantVariables[value] = ret;
            }

            return ret;
        }

        if (context.STRING() is {} s)
        {
            var value = (int)System.Enum.Parse(typeof(StringTypes), s.GetText()[7..], true);
                
            ret = GetConstantVariable(value);
            if (ret == null)
            {
                VariableData NodeData = new VariableData(value, scope: ScopeEnum.Constant);
                ret = new StringNode(testBrain, NodeData);
                ConstantVariables[value] = ret;
            }

            return ret;
        }

        if (context.BOOL() is {} b)
        {
            ret = GetConstantVariable(bool.Parse(b.GetText()));
            if (ret == null)
            {
                VariableData NodeData = new VariableData(bool.Parse(b.GetText()), scope: ScopeEnum.Constant);
                ret = new BooleanNode(testBrain, NodeData);
                ConstantVariables[bool.Parse(b.GetText())] = ret;
            }

            return ret;
        }

        if (context.NULL() is {})
            return null;

        throw new NotImplementedException();
    }

    public override object? VisitComparisonExpression(NodeSharpParser.ComparisonExpressionContext context)
    {       
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));
        
        var op = context.comparisonOp().GetText();
        return NMath.Compare(op, left, right);
    }

    public override object? VisitAdditiveExpression(NodeSharpParser.AdditiveExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.additionOp().GetText();
        return NMath.Addition(testBrain, op, left, right);
    }
    
    public override object? VisitParenthesizedExpression(NodeSharpParser.ParenthesizedExpressionContext context)
    {
        return Visit(context.expression());
    }

    public override object? VisitMultiplicativeExpression(NodeSharpParser.MultiplicativeExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.multiplyOp().GetText();
        return NMath.Multiplication(testBrain, op, left, right);
    }
    
    public override object? VisitWhileBlock(NodeSharpParser.WhileBlockContext context)
    {
        while (Visit(context.expression()) as bool? ?? false)
        {
            Visit(context.block());
        }

        return null;
    }

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
    
    public override object? VisitTrigonometryArcTan2Expression(NodeSharpParser.TrigonometryArcTan2ExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        if (left is VariableData { Data: float or double } l && right is VariableData { Data: float or double } r)
            return NMath.Atan2(l, r);
        
        throw new Exception($"Cannot ArcTan2 arguments because one of them is not a number {left?.GetType()}, {right?.GetType()}.");
    }

    public override object? VisitPrefixExpression(NodeSharpParser.PrefixExpressionContext context)
    {
        var variable = Visit(context.IDENTIFIER());
        
        if (variable is not float left)
            throw new Exception($"Cannot apply arithmetic because left side not a number.");

        throw new NotImplementedException();
        
        // float res = 0;
        // var op = context.affixOp().GetText();
        // switch (op)
        // {
        //     case "++":
        //         res = (float)NMath.Add(left, 1);
        //         SetVariable(context.IDENTIFIER().GetText(), res, GetScope(context.SCOPE()));
        //         return res;
        //     case "--":
        //         res = (float)NMath.Subtract(left, 1);
        //         SetVariable(context.IDENTIFIER().GetText(), res, GetScope(context.SCOPE()));
        //         return res;
        //     default:
        //         throw new NotImplementedException();
        // }
    }

    public override object? VisitSuffixExpression(NodeSharpParser.SuffixExpressionContext context)
    {
        var variable = Visit(context.IDENTIFIER());
        
        if (variable is not float left)
            throw new Exception($"Cannot apply arithmetic because left side not a number.");
        
        throw new NotImplementedException();
        
        // var op = context.affixOp().GetText();
        // switch (op)
        // {
        //     case "++":
        //         SetVariable(context.IDENTIFIER().GetText(), NMath.Add(left, 1), GetScope(context.SCOPE()));
        //         return variable;
        //     case "--":
        //         SetVariable(context.IDENTIFIER().GetText(), NMath.Subtract(left, 1), GetScope(context.SCOPE()));
        //         return variable;
        //     default:
        //         throw new NotImplementedException();
        // }
    }

    public override object? VisitArithmeticExpression(NodeSharpParser.ArithmeticExpressionContext context)
    {
        var variable = Visit(context.IDENTIFIER());
                
        throw new NotImplementedException();

        // if (variable is not float left)
        //     throw new Exception($"Cannot apply arithmetic because left side not a number.");
        //
        // var op = context.arithmeticOp().GetText();
        // object? res = 0;
        //
        //
        // var right = Visit(context.expression());
        // if (right is not float r)
        //     throw new Exception($"Cannot apply arithmetic because right side not a number.");
        //
        // res = NMath.Arithmetic(op, left, r);
        //
        // SetVariable(context.IDENTIFIER().GetText(), res, GetScope(context.SCOPE()));
        // return res;
    }
}