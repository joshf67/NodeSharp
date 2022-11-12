using NodeSharp.Nodes.Interface;

namespace NodeSharp;

public class ActionScope
{
    public ActionScope? ParentScope;
    public string? ParentPin;
    
    public IActionNode ActionNode;
    
    private List<ActionScope> ScopeNodes = new List<ActionScope>();
    private int CurrentScope = 0;
    
    public ActionScope(IActionNode actionNode, ActionScope? parentScope = null, string? parentPin = null)
    {
        ParentScope = parentScope;
        ParentPin = parentPin;
        ActionNode = actionNode;
    }

    public ActionScope AddScope(IActionNode actionNode, string parentPin)
    {
        var newScope = new ActionScope(actionNode, this, parentPin);
        ScopeNodes.Add(newScope);
        return newScope;
    }

    public ActionScope? GetNextNode()
    {
        while (true)
        {
            if (CurrentScope >= ScopeNodes.Count) return null;
            var ret = ScopeNodes[CurrentScope].GetNextNode();

            if (ret != null) return ret;
            return ScopeNodes[CurrentScope++];
            //throw new InvalidOperationException($"Trying to return next Node, out of bounds");
        }
    }
}