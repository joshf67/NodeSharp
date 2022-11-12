namespace NodeSharp.Nodes.Interface;

public interface ISetterOptimizable
{
    void DefaultSetter(object? value)
    {
        throw new NotImplementedException("Setter node default has not been implemented on this node type");
    }
}