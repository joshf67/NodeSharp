using NodeSharp.Grammar;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Events_Custom;
using NodeSharp.Nodes.Logic;
using NodeSharp.Nodes.Variable;
using NodeSharp.Visitor;

namespace NodeSharp;

public class NodeSharpVisitorRefactor : NodeSharpParserBaseVisitor<object?>
{
    public ScriptBrain testBrain = new ScriptBrain();
    public ActionScope StartingActionNodeScope;
    public ActionScope CurrentActionScope;
    
    /// <summary>
    /// Stores any nodes that require identifiers to be setup
    /// </summary>
    public Dictionary<string, Identifier> IdentifierVariables { get; } = new();


    public NodeSharpVisitorRefactor()
    {
        //Setup code enter
        var GameStart = new OnGameStartNode(testBrain);
        StartingActionNodeScope = new ActionScope(GameStart);
        CurrentActionScope = StartingActionNodeScope;
    }

    /// <summary>
    /// Handles calling blocks and return values because ANTLR doesn't return block values by default
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns></returns>
    public override object? VisitBlock(NodeSharpParser.BlockContext context)
    {
        for (int i = 0; i < context.line().Length - 1; i++)
        {
            Visit(context.line(i));
        }
        
        //return the final section of the block to keep loop continuing
        return Visit(context.line(context.line().Length - 1));
    }

    /// <summary>
    /// Handles calling statements and return values because ANTLR doesn't return statement values by default
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns></returns>
    public override object? VisitStatement(NodeSharpParser.StatementContext context)
    {
        if (context.assignment() != null) return VisitAssignment(context.assignment());
        if (context.functionCall() != null) return VisitFunctionCall(context.functionCall());
        if (context.internal_functionCall() != null) return VisitInternal_functionCall(context.internal_functionCall());
        return null;
    }

    /// <summary>
    /// Handles visiting branch nodes (if, if else, else)
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns></returns>
    public override object? VisitBranchBlock(NodeSharpParser.BranchBlockContext context)
    {
        return LogicVisitor.VisitBranchBlock(this, testBrain, context);
    }

    /// <summary>
    /// Handles creating functions from N# code
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> null </returns>
    /// <exception cref="InvalidOperationException"> Fails if the function already exists </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Fails if scope is invalid </exception>
    public override object? VisitFunctionCreation(NodeSharpParser.FunctionCreationContext context)
    {
        return FunctionVisitor.VisitFunctionCreation(context);
    }
    
    /// <summary>
    /// Handles calling any functions
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override object? VisitFunctionCall(NodeSharpParser.FunctionCallContext context)
    {
        return FunctionVisitor.VisitFunctionCall(this, testBrain, context);
    }

    /// <summary>
    /// Converts function parameters to their visited equivalent 
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> List of object, probably a list of Nodes </returns>
    public override object? VisitFunctionParameters(NodeSharpParser.FunctionParametersContext context)
    {
        return (from child in context.children where child.GetText() != "," select Visit(child)).ToList();
    }
    
    /// <summary>
    /// Handles visits that get variables 
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Variable Data/NULL</returns>
    public override object? VisitIdentifierExpression(NodeSharpParser.IdentifierExpressionContext context)
    {
        return VariableVisitor.GetVariableData(context.IDENTIFIER().GetText(), ScopeVisitor.GetScope(context.SCOPE()));
    }
    
    /// <summary>
    /// Handles visits that set variables
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> NULL </returns>
    public override object? VisitAssignment(NodeSharpParser.AssignmentContext context)
    {
        return VariableVisitor.VisitAssignment(this, testBrain, context);
    }

    /// <summary>
    /// Handles any visits that result in a constant variable
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Constant Variable Node</returns>
    public override object? VisitConstant(NodeSharpParser.ConstantContext context)
    {
        return VariableVisitor.VisitConstant(testBrain, context);
    }
    
    /// <summary>
    /// Handles visits when expressions are wrapped in parenthesis
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
        return LogicVisitor.VisitComparisonExpression(this, context);
    }

    /// <summary>
    /// Handles addition and subtraction visits
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Addition/Subtraction Node</returns>
    public override object? VisitAdditiveExpression(NodeSharpParser.AdditiveExpressionContext context)
    {
        return MathVisitor.VisitAdditiveExpression(this, testBrain, context);
    }

    /// <summary>
    /// Handles multiplication and division visits
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Multiplication/Division Node</returns>
    public override object? VisitMultiplicativeExpression(NodeSharpParser.MultiplicativeExpressionContext context)
    {
        return MathVisitor.VisitMultiplicativeExpression(this, testBrain, context);
    }

    /// <summary>
    /// Handles square root visits
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Multiplication/Division Node</returns>
    public override object? VisitSquareRootExpression(NodeSharpParser.SquareRootExpressionContext context)
    {
        return MathVisitor.VisitSquareRootExpression(this, testBrain, context);
    }
    
    /// <summary>
    /// Handles basic trigonometry visits that use 1 parameter
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Trigonometry Node</returns>
    public override object? VisitTrigonometryExpression(NodeSharpParser.TrigonometryExpressionContext context)
    {
        return MathVisitor.VisitTrigonometryExpression(this, context);
    }
    
    /// <summary>
    /// Handles ArcTan2 visit as it requires two parameters
    /// </summary>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> ArcTan2 Node</returns>
    public override object? VisitTrigonometryArcTan2Expression(NodeSharpParser.TrigonometryArcTan2ExpressionContext context)
    {
        return MathVisitor.VisitTrigonometryArcTan2Expression(this, context);
    }
}