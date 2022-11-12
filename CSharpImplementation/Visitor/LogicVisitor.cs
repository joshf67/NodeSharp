using NodeSharp.Grammar;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Logic;

namespace NodeSharp.Visitor;

public static class LogicVisitor
{

    public static int totalMergeBranchCount = 0;
    
    /// <summary>
    /// Handles comparison visits
    /// </summary>
    /// <param name="parentVisitor"> Visitor that called this function </param>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Comparison Node</returns>
    public static object? VisitComparisonExpression(NodeSharpVisitorRefactor parentVisitor, NodeSharpParser.ComparisonExpressionContext context)
    {       
        var left = parentVisitor.Visit(context.expression(0));
        var right = parentVisitor.Visit(context.expression(1));
        
        var op = context.comparisonOp().GetText();
        return NMath.Compare(op, left, right);
    }

    /// <summary>
    /// Handles branching visits (if, if else, else)
    /// </summary>
    /// <param name="parentVisitor"> Visitor that called this function </param>
    /// <param name="brain"> The ScriptBrain that this node is being added to </param>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns></returns>
    public static object? VisitBranchBlock(NodeSharpVisitorRefactor parentVisitor, ScriptBrain brain, NodeSharpParser.BranchBlockContext context, string? continueAfter = null) //Add recursive continue block
    {
        //Change to IComparable/Compare Connection
        var variableData = new VariableData(false);
        var branchNode = new BranchNode(brain, variableData);
        NodeSharpDefinedFunction? initialCall = null;

        //Generate merge branch events if required and supply to recursion
        if (continueAfter == null && context.continueBlock() != null)
        {
            continueAfter = "Branch" + totalMergeBranchCount++;
            initialCall = new NodeSharpDefinedFunction(continueAfter);
            FunctionVisitor.FunctionCreation(initialCall, ScopeEnum.Local);
        }
        else
        {
            continueAfter = "";
        }

        //Use action shell to allow branching from the same node
        var actionShell = new ActionNodeShell(Keywords.ActionStart, Keywords.ExecuteIfTrue);
        parentVisitor.CurrentActionScope = parentVisitor.CurrentActionScope.AddScope(actionShell,
            parentVisitor.CurrentActionScope.ActionNode.GetActionSendPin());
        var actionShellScope = parentVisitor.CurrentActionScope;
        
        //Visit true branch
        parentVisitor.Visit(context.conditionalBlock());

        if (continueAfter != "")
            FunctionVisitor.FunctionCall(parentVisitor, brain, continueAfter, new List<object>());
        
        //Visit false branch
        parentVisitor.CurrentActionScope = actionShellScope;
        actionShell.SendPin = Keywords.ExecuteIfFalse;
        if (context.branchBlock() != null)
        {
            VisitBranchBlock(parentVisitor, brain, context.branchBlock(), continueAfter);
        } 
        else if (context.block() != null)
        {
            parentVisitor.Visit(context.block());
            
            //Merge branch if needed
            if (continueAfter != "")
                FunctionVisitor.FunctionCall(parentVisitor, brain, continueAfter, new List<object>());
        }

        //Update scope shell to point to branch node instead of fake branching class
        actionShellScope.ActionNode = branchNode;
        
        //Set merge branch to current scope
        if (initialCall != null)
        {
            parentVisitor.CurrentActionScope = parentVisitor.CurrentActionScope.AddScope(initialCall.EventNode, null);
        }
        
        return null;
    }
}