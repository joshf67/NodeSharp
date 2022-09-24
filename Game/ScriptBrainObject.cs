using System.Numerics;
using InfiniteForgeConstants.ObjectSettings;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.Game;

public class ScriptBrainObject : GameObject
{
    public ScriptBrain Brain;
    public Dictionary<string, VariableNode> IdentifierVariables { get; } = new();

    public ScriptBrainObject(Dictionary<string, VariableNode> identifierVariables, ScriptBrain brain, string? name = "Object",
        Transform? transform = null) : base(name, ObjectId.SCRIPT_BRAIN, transform)
    {
        IdentifierVariables = identifierVariables;
        Brain = brain;
        transform.IsStatic = true;
    }
}