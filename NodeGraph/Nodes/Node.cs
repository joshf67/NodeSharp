using Newtonsoft.Json;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;

namespace NodeSharp;

public class Node
{
    [JsonProperty("m_implementationNodeData")]
    public Property[] ImplementationNodeData = new Property[] { };

    [JsonProperty("m_metadata")]
    public MetaData[] Metadata = new MetaData[] { new MetaData() };

    [JsonProperty("m_nodeID")]
    public int NodeId = -1;
    
    [JsonProperty("m_nodeName")]
    public string NodeName = "";
    
    [JsonProperty("m_nodeType")]
    public string NodeType = "";
    
    [JsonProperty("m_pins")]
    public Pin[] Pins = new Pin[] { };

    [JsonProperty("m_version")]
    public int Version = 0;

    [JsonIgnore] public bool Optimized = false;
    [JsonIgnore] public Type NodeVariableType;
    [JsonIgnore] public List<Connection> Input = new List<Connection>();
    [JsonIgnore] public List<Connection> Output = new List<Connection>();
    //Potentially set this up at the end, loop through inputs and setup output

    public Node(ScriptBrain brain, string nodeType)
    {
        NodeType = nodeType;
        brain.AddNode(this);
    }
    
    public Node(string nodeType, Property[] implementationNodeData, Pin[] pins)
    {
        NodeType = nodeType;
        ImplementationNodeData = implementationNodeData;
        Pins = pins;
    }

    /// <summary>
    /// Optimizes a node and any input nodes it has to decrease total node count
    /// </summary>
    /// <param name="brain"> The script brain that owns this node </param>
    /// <returns> Boolean determining if this node has been removed due to optimization </returns>
    public virtual bool Optimize(ScriptBrain brain)
    {
        for (int i = 0; i < Input.Count; i++)
        {
            if (brain.NodeMap[Input[i].OriginNode].node.Optimized != false) continue;
            if (!brain.NodeMap[Input[i].OriginNode].node.Optimize(brain)) continue;
            
            //brain.RemoveReferenceToNode(brain.NodeMap[Input[i].OriginNode].node);
            Input.RemoveAt(i);
            i--;
        }
        
        Optimized = true;
        return false;
    }
}