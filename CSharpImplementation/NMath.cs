using System.Numerics;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.NodeGraph.Nodes;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp;

public static class NMath
{
    
    public static object? Arithmetic(ScriptBrain brain, string op, object? left, object? right)
    {
        return op switch
        {
            // "+=" => Add(brain, (VariableNode)left, (VariableNode)right),
            // "-=" => Subtract(brain, left, right),
            // "*=" => Multiply(left, right),
            // "/=" => Divide(left, right),
            // "^=" => Power(left, right),
            // "^^=" => Sqrt(left),
            _ => throw new NotImplementedException()
        };
    }

    public static object? Addition(ScriptBrain brain, string op, object? left, object? right)
    {
        return op switch
        {
            "+" => Add(brain, (GetterInterface)left, (GetterInterface)right),
            "-" => Subtract(brain, (GetterInterface)left, (GetterInterface)right),
            _ => throw new NotImplementedException()
        };
    }
    
    public static object? Multiplication(ScriptBrain brain, string op, object? left, object? right)
    {
        return op switch
        {
            "*" => Multiply(brain, (GetterInterface)left, (GetterInterface)right),
            "/" => Divide(brain, (GetterInterface)left, (GetterInterface)right),
            "%" => Remainder(left, right),
            "^" => Power(left, right),
            _ => throw new NotImplementedException()
        };
    }
    
    public static object? Compare(string op, object? left, object? right)
    {
        return op switch
        {
            "===" => Equals(left, right),
            "==" => IsEquals(left, right),
            "!=" => !IsEquals(left, right),
            "<" => IsLessThan(left, right),
            "<=" => IsLessThan(left, right) || IsEquals(left, right),
            ">" => IsGreaterThan(left, right),
            ">=" => IsGreaterThan(left, right) || IsEquals(left, right),
            _ => throw new NotImplementedException()
        };
    }
    
    public static Node Multiply(ScriptBrain brain, GetterInterface left, GetterInterface right)
    {
        // return left switch
        // {
        //     Vector3 lv when right is float r => new Vector3(lv.X * r, lv.Y * r, lv.Z * r),
        //     float l when right is Vector3 rv => new Vector3(l * rv.X, l * rv.Y, l * rv.Z),
        //     float lf when right is float rf => lf * rf,
        //     _ => throw new Exception($"Cannot multiply values of types {left?.GetType()} and {right?.GetType()}.")
        // };
        
        var ret = new MultiplyNumberNode(brain, left, right);
        return ret;
    }

    public static object? Power(object? left, object? right)
    {
        if (left is float l && right is float r)
            return (float)Math.Pow(l, r);

        throw new Exception($"Cannot power values of types {left?.GetType()} and {right?.GetType()}.");
    }
    
    public static Node Sqrt(ScriptBrain brain, GetterInterface left)
    {
        var ret = new SquareRootNumberNode(brain, left);
        return ret;
        // if (left is float l)
        //     return (float)Math.Sqrt(l);
        //
        // throw new Exception($"Cannot square root value with type {left?.GetType()}.");
    }
    
    public static Node Divide(ScriptBrain brain, GetterInterface left, GetterInterface right)
    {
        // return left switch
        // {
        //     Vector3 lv when right is float r => new Vector3(lv.X / r, lv.Y / r, lv.Z / r),
        //     float l when right is Vector3 rv => new Vector3(l / rv.X, l / rv.Y, l / rv.Z),
        //     float lf when right is float rf => lf / rf,
        //     _ => throw new Exception($"Cannot divide values of types {left?.GetType()} and {right?.GetType()}.")
        // };
        
        var ret = new DivideNumberNode(brain, left, right);
        return ret;

        // throw new Exception($"Cannot divide values of types {left?.GetType()} and {right?.GetType()}.");
    }
    
    public static object? Remainder(object? left, object? right)
    {
        if (left is float l && right is float r)
            return l % r;

        throw new Exception($"Cannot find remainder between values of types {left?.GetType()} and {right?.GetType()}.");
    }
    
    public static Node Add(ScriptBrain brain, GetterInterface left, GetterInterface right)
    {
        
        // if (left.NodeData.Data is Vector3 lv && right.NodeData.Data is Vector3 rv)
        // {
        //     throw new NotImplementedException();
        //     return new Vector3(lv.X + rv.X, lv.Y + rv.Y, lv.Z + rv.Z);
        // }

        // if (left.NodeData.Data is float l && right.NodeData.Data is float f)
        // {
        //     if (left.NodeData.Scope == ScopeEnum.Constant && right.NodeData.Scope == ScopeEnum.Constant)
        //     {
                //IF BOTH CONSTANT RETURN THE PRE-COMPILED VALUE
                
            // }
            // else
            // {
            //     
            // }
            //
            // brain.AddConnection(new Connection(nodeId, Keywords.OperandA, left.NodeID, Keywords.Out));
            // }
            // else
            // {
            //     //Make this better (don't create a getter every single time)
            //     var GetterNodeID = 
            //         brain.AddNode(new GetNumberNode((int)left.NodeData.Scope, left.NodeData.Identifier));
            //     brain.AddConnection(new Connection(nodeId, Keywords.OperandA, GetterNodeID, Keywords.Out));
            // }
            //
            // if (right.NodeData.Scope == ScopeEnum.Constant)
            // {
            //     brain.AddConnection(new Connection(nodeId, Keywords.OperandB, right.NodeID, Keywords.Out));
            // }
            // else
            // {
            //     //Make this better (don't create a getter every single time)
            //     var GetterNodeID = 
            //         brain.AddNode(new GetNumberNode((int)right.NodeData.Scope, right.NodeData.Identifier));
            //     brain.AddConnection(new Connection(nodeId, Keywords.OperandB, GetterNodeID, Keywords.Out));
            // }

            // VariableNode data = NSharpVisitor.instance.InternalVariables["InternalFloat"];
            // data.NodeData.Data = l + f;

            // var returnValueNodeId = brain.AddNode(new SetNumberNode((float)data.NodeData.Data, (int)ScopeEnum.Global, data.NodeData.Identifier));
            // brain.AddConnection(new Connection(returnValueNodeId, Keywords.Value, nodeId, Keywords.Result));

            var ret = new AddNumberNode(brain, left, right);
            return ret;
        // }

        // throw new Exception($"Cannot add values of types {left?.GetType()} and {right?.GetType()}.");
    }
    
    public static object? Subtract(ScriptBrain brain, GetterInterface left, GetterInterface right)
    {
        var ret = new SubtractNumberNode(brain, left, right);
        return ret;
        // if (left is Vector3 lv && right is Vector3 rv)
        //     return new Vector3(lv.X - rv.X, lv.Y - rv.Y, lv.Z - rv.Z);
        //
        // if (left is float l && right is float f)
        //     return l - f;

        // throw new Exception($"Cannot add values of types {left?.GetType()} and {right?.GetType()}.");
    }
    
    public static bool IsEquals(object? left, object? right)
    {
        if (left is float l && right is float r)
            return Math.Abs(l - r) < 0.0005;

        return left == right;
    }

    public static bool IsLessThan(object? left, object? right)
    {
        if (left is float l && right is float r)
            return l - r < 0.0005;

        throw new Exception($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}.");
    }
    
    public static bool IsGreaterThan(object? left, object? right)
    {
        if (left is float l && right is float r)
            return l - r > 0.0005;

        throw new Exception($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}.");
    }
    
    public static object? Sin(VariableData arg)
    {
        return (float)Math.Sin((float)arg.Data);
    }

    public static object? Cos(VariableData arg)
    {
        return (float)Math.Cos((float)arg.Data);
    }
    
    public static object? Tan(VariableData arg)
    {
        return (float)Math.Tan((float)arg.Data);
    }
    
    public static object? ArcSin(VariableData arg)
    {
        return (float)Math.Asin((float)arg.Data);
    }
    
    public static object? ArcCos(VariableData arg)
    {
        return (float)Math.Acos((float)arg.Data);
    }
    
    public static object? Atan2(VariableData arg, VariableData arg2)
    {
        return (float)Math.Atan2((float)arg.Data, (float)arg2.Data);
    }
}