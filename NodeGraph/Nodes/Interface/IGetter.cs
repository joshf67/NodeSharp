namespace NodeSharp.Nodes.Interface;

public interface IGetter
{
    VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "");
}