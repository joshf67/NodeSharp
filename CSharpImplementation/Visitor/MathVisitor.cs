using NodeSharp.Grammar;

namespace NodeSharp.Visitor;

public static class MathVisitor
{
    /// <summary>
    /// Handles addition and subtraction visits
    /// </summary>
    /// <param name="parentVisitor"> Visitor that called this function </param>
    /// <param name="brain"> The ScriptBrain that this node is being added to </param>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Addition/Subtraction Node</returns>
    public static object? VisitAdditiveExpression(NodeSharpVisitorRefactor parentVisitor, ScriptBrain brain, NodeSharpParser.AdditiveExpressionContext context)
    {
        var left = parentVisitor.Visit(context.expression(0));
        var right = parentVisitor.Visit(context.expression(1));

        var op = context.additionOp().GetText();
        return NMath.Addition(brain, op, left, right);
    }

    /// <summary>
    /// Handles multiplication and division visits
    /// </summary>
    /// <param name="parentVisitor"> Visitor that called this function </param>
    /// <param name="brain"> The ScriptBrain that this node is being added to </param>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Multiplication/Division Node</returns>
    public static object? VisitMultiplicativeExpression(NodeSharpVisitorRefactor parentVisitor, ScriptBrain brain, NodeSharpParser.MultiplicativeExpressionContext context)
    {
        var left = parentVisitor.Visit(context.expression(0));
        var right = parentVisitor.Visit(context.expression(1));

        var op = context.multiplyOp().GetText();
        return NMath.Multiplication(brain, op, left, right);
    }

    /// <summary>
    /// Handles square root visits
    /// </summary>
    /// <param name="parentVisitor"> Visitor that called this function </param>
    /// <param name="brain"> The ScriptBrain that this node is being added to </param>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Multiplication/Division Node</returns>
    public static object? VisitSquareRootExpression(NodeSharpVisitorRefactor parentVisitor, ScriptBrain brain, NodeSharpParser.SquareRootExpressionContext context)
    {
        var left = parentVisitor.Visit(context.expression());
        return NMath.Multiplication(brain, "^^", left, null);
    }
    
    /// <summary>
    /// Handles basic trigonometry visits that use 1 parameter
    /// </summary>
    /// <param name="parentVisitor"> Visitor that called this function </param>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Trigonometry Node</returns>
    public static object? VisitTrigonometryExpression(NodeSharpVisitorRefactor parentVisitor, NodeSharpParser.TrigonometryExpressionContext context)
    {
        var arg = parentVisitor.Visit(context.expression());

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
    /// <param name="parentVisitor"> Visitor that called this function </param>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> ArcTan2 Node</returns>
    public static object? VisitTrigonometryArcTan2Expression(NodeSharpVisitorRefactor parentVisitor, NodeSharpParser.TrigonometryArcTan2ExpressionContext context)
    {
        var left = parentVisitor.Visit(context.expression(0));
        var right = parentVisitor.Visit(context.expression(1));

        if (left is VariableData { Data: float or double } l && right is VariableData { Data: float or double } r)
            return NMath.Atan2(l, r);
        
        throw new Exception($"Cannot ArcTan2 arguments because one of them is not a number {left?.GetType()}, {right?.GetType()}.");
    }
}