namespace NodeSharp.Nodes.Interface;

public interface IActionNode
{
    public string? GetActionReceivePin()
    {
        return Keywords.ActionStart;
    }

    public string? GetActionSendPin()
    {
        return Keywords.ActionComplete;
    }
}