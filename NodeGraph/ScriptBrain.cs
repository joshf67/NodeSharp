using Newtonsoft.Json;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NodeSharp;

public class ScriptBrain
{
    public List<Connection> Connections = new List<Connection>() { };
    
    public string GraphType = "ForgeScriptingGraph";
    
    public int NGFVersion = 0;
    
    public List<Node> Nodes = new List<Node>() { };

    [JsonIgnore] public Dictionary<int, Connection> ConnectionMap = new Dictionary<int, Connection>();
    [JsonIgnore] public Dictionary<int, (Node node, int connections)> NodeMap = new Dictionary<int, (Node node, int connections)>();

    public int Version = 0;
    
    [JsonProperty("m_implementationNodeData")]
    public List<Property> ImplementationNodeData = new List<Property>() { new Property() };

    public ScriptBrain(List<Connection> connections = null, List<Node> nodes = null)
    {
        Connections = connections?? Connections;
        Nodes = nodes?? Nodes;
    }
    
    public void AddConnection(Connection connection)
    {
        connection.ConnectionID = Connections.Count;
        Connections.Add(connection);
        ConnectionMap.Add(connection.ConnectionID, connection);
    }

    public void AddNode(Node node)
    {
        node.NodeID = Nodes.Count;
        node.Metadata[0].PositionX = (Nodes.Count * 500 + 50).ToString();
        node.Metadata[0].PositionY = 50.ToString();
        
        Nodes.Add(node);
        NodeMap.Add(node.NodeID, (node, 0));
    }

    public void RemoveReferenceToNode(Node node)
    {
        if (NodeMap.ContainsKey(node.NodeID))
            NodeMap[node.NodeID] = (NodeMap[node.NodeID].node, NodeMap[node.NodeID].connections - 1);
    }

    public void AddReferenceToNode(Node node)
    {
        if (NodeMap.ContainsKey(node.NodeID))
            NodeMap[node.NodeID] = (NodeMap[node.NodeID].node, NodeMap[node.NodeID].connections + 1);
    }

    public void Optimize()
    {
        foreach (var nodeData in Nodes)
        {
            for (int i = 0; i < nodeData.Input.Count; i++ )
            {
                var outputConnection = (Connection)Activator.CreateInstance(nodeData.Input[i].GetType(), nodeData.Input[i]);
                NodeMap[nodeData.Input[i].OriginNode].node.Output.Add(outputConnection);
            }
        }

        GenerateReferences();
        
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].Optimized != false) continue;
            Nodes[i].Optimize(this);
        }
        
        for (int i = 0; i < Nodes.Count; i++)
        {
            // if (NodeMap[Nodes[i].NodeID].connections < 0)
            //     throw new Exception("Node usage is below 0 when it shouldn't be possible to be");


            if (NodeMap[Nodes[i].NodeID].connections > 0) continue;
            if (NodeMap[Nodes[i].NodeID].connections == 0 && NodeMap[Nodes[i].NodeID].node is IDefaultable) continue;
            
            NodeMap.Remove(Nodes[i].NodeID);
            Nodes.RemoveAt(i);
            i--;
        }
    }

    public void GenerateReferences()
    {
        foreach (var nodeData in Nodes)
        {
            foreach (var inputData in nodeData.Input)
            {
                NodeMap[inputData.OriginNode] = (NodeMap[inputData.OriginNode].node,
                    NodeMap[inputData.OriginNode].connections + 1);
            }
        }
    }
    
    public void GenerateConnections()
    {
        //Two foreach loops to arrange the connections nicely in XML
        foreach (var nodeData in Nodes)
        {
            foreach (var inputData in nodeData.Input)
            {
                AddConnection(inputData);
            }
        }
    }

    public void ReplaceNodeInput(Node oldNode, Node newNode, string inputPin)
    {
        NodeMap[newNode.NodeID] = NodeMap[oldNode.NodeID] with { node = newNode };
        NodeMap[oldNode.NodeID] = NodeMap[oldNode.NodeID] with { connections = 0};

        foreach (var outputData in oldNode.Output)
        {
            var node = NodeMap[outputData.DestinationNode].node;
            for (int i = 0; i < node.Input.Count; i++)
            {
                if (node.Input[i].OriginNode != oldNode.NodeID) continue;

                node.Input[i].OriginNode = newNode.NodeID;
                node.Input[i].OriginPin = inputPin;
                i = node.Input.Count;
            }
        }

    }
}