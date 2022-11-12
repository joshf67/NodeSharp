using System.Numerics;
using InfiniteForgeConstants.ObjectSettings;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.Game;

public class ScriptBrainObject : GameObject
{
    public ScriptBrain Brain;
    public Dictionary<string, Identifier> IdentifierVariables { get; } = new();

    public ScriptBrainObject(Dictionary<string, Identifier> identifierVariables, ScriptBrain brain, string? name = "Object",
        Transform? transform = null) : base(name, ObjectId.SCRIPT_BRAIN, transform)
    {
        IdentifierVariables = identifierVariables;
        Brain = brain;
        transform.IsStatic = true;
    }
}