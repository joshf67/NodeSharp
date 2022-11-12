namespace NodeSharp;

using NodeSharp.Nodes.Interface;

/// <summary>
/// Allows branching by faking node to be replaced later with actual node
/// </summary>
public class ActionNodeShell : IActionNode
{
    public string ReceivePin;
    public string SendPin;
    
    public ActionNodeShell(string receivePin, string sendPin)
    {
        ReceivePin = receivePin;
        SendPin = sendPin;
    }
    
    public string? GetActionReceivePin()
    {
        return ReceivePin;
    }

    public string? GetActionSendPin()
    {
        return SendPin;
    }
}