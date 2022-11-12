namespace NodeSharp.NodeGraph.NodeData;

public class Identifier
{
    public string IdentifierName = "";
    public int Order = -1;
    public int UseCount = 0;

    public Identifier(string name, int order)
    {
        IdentifierName = name;
        Order = order;
    }

    public void AddUse()
    {
        UseCount++;
    }
}