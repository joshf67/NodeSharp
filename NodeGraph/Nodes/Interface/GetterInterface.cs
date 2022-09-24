namespace NodeSharp.Nodes.Interface;

public interface GetterInterface
{
    VariableConnection Getter(ScriptBrain brain, int destinationId = -1, string destinationPin = "");
}