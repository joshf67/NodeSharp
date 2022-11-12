using System.Numerics;
using InfiniteForgeConstants.NodeGraphSettings;
using NodeSharp.Grammar;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Interface;
using NodeSharp.Nodes.Variable;

namespace NodeSharp.Visitor;

public static class VariableVisitor
{
    /// <summary>
    /// Stores any nodes that exist in constant scope
    /// </summary>
    private static Dictionary<object, BaseVariableNode> ConstantVariables { get; } = new();
    
    /// <summary>
    /// Stores any nodes that exist in local scope
    /// </summary>
    private static Dictionary<string, BaseVariableNode> LocalVariables { get; } = new();
    
    /// <summary>
    /// Stores any nodes that exist in global scope
    /// </summary>
    private static Dictionary<string, BaseVariableNode> GlobalVariables { get; } = new();
    
    /// <summary>
    /// Handles finding variables and stopping execution if the variable required does not exist
    /// </summary>
    /// <param name="name"> The name of the variable to find </param>
    /// <param name="scope"> The scope of the variable to find </param>
    /// <param name="throwIfNull"> Controls whether this function call can return null </param>
    /// <returns> Variable Node/NULL</returns>
    /// <exception cref="Exception"> Throws if no variable exists and throwIfNull is true </exception>
    public static BaseVariableNode? GetVariableData(string name, ScopeEnum scope = 0, bool throwIfNull = true)
    {
        if (scope == ScopeEnum.Local)
        {
            if (!LocalVariables.ContainsKey(name))
            {
                if (!throwIfNull)
                    return null;
                
                throw new Exception($"Variable {name} is not defined in local scope");
            }

            return LocalVariables[name];
        }
        else if (scope == ScopeEnum.Global)
        {
            if (!GlobalVariables.ContainsKey(name))
            {
                if (!throwIfNull)
                    return null;
                throw new Exception($"Variable {name} is not defined in global scope");
            }

            return GlobalVariables[name];
        }

        if (LocalVariables.ContainsKey(name))
        {
            return LocalVariables[name];
        }

        if (GlobalVariables.ContainsKey(name))
            return GlobalVariables[name];

        // if (InternalVariables.ContainsKey(name))
        //     return InternalVariables[name];

        if (!throwIfNull)
            return null;
        
        throw new Exception($"Variable {name} is not defined in any scope");
    }
    
    /// <summary>
    /// Handles setting variables to values
    /// </summary>
    /// <param name="parentVisitor"> Visitor that called this function </param>
    /// <param name="brain"> The ScriptBrain that this node is being added to </param>
    /// <param name="name"> The name of the variable being set </param>
    /// <param name="setTo"> The IGetter variable that the variable should be set to </param>
    /// <param name="scope"> The scope the variable being set should be in </param>
    /// <returns> Declare Variable Node </returns>
    public static Node SetVariable(NodeSharpVisitorRefactor parentVisitor, ScriptBrain brain, string name, IGetter setTo, ScopeEnum scope = 0)
    {
        BaseVariableNode variable = GetVariableData(name, scope, false);
        if (variable != null)
        {
            if (variable is not DynamicVariableNode dynamicVariableNode)
                throw new InvalidOperationException($"Cannot set a constant variable");
            
            //Set up variable connection then update variable data value
            var getterConnection = setTo.Getter(brain);

            if (getterConnection.Variable.GetType() != variable.NodeData.Data.GetType())
                throw new InvalidOperationException(
                    $"Trying to set variable '{name}' to a different type {getterConnection.Variable.GetType()} vs {variable.NodeData.Data.GetType()}");
            
            //Create the setter for this variable
            var setterData = dynamicVariableNode.Setter(brain, getterConnection.OriginNode, getterConnection.OriginPin);
            
            //Insert setter into action scope
            parentVisitor.CurrentActionScope = parentVisitor.CurrentActionScope.AddScope(
                setterData.SetterNode as IActionNode, parentVisitor.CurrentActionScope.ActionNode.GetActionSendPin());
            
            variable.NodeData.Data = getterConnection.Variable;
            
            if (variable.NodeData.Scope != ScopeEnum.Constant && variable.NodeData.Scope != ScopeEnum.Object)
                parentVisitor.IdentifierVariables[name].AddUse();
            
            return variable;
        }
        else
        {
            Node? newVariableNode = null;
            var getterConnection = setTo.Getter(brain);
            VariableData variableData =  new VariableData(getterConnection.Variable, parentVisitor.IdentifierVariables.Count, name, scope);
            
            //Handle if the variable being set from is constant, if it is, convert it to variable type
            if (brain.NodeMap[getterConnection.OriginNode].node is IConstantType constNode)
            {
                newVariableNode = (Node)Activator.CreateInstance(constNode.GetVariableNodeType(), brain, variableData, getterConnection.OriginNode, getterConnection.OriginPin);
            }
            else
            {
                //Use generic getter/setter setup to link setTo with the new variable
                newVariableNode = (Node)Activator.CreateInstance(getterConnection.ConnectedVariableNodeType, brain, variableData, getterConnection.OriginNode, getterConnection.OriginPin);
            }

            if (newVariableNode is null)
                throw new InvalidOperationException(
                    $"newVariableNode is null, this shouldn't be possible.");
            
            //parentVisitor.IdentifierVariables.Add(name, (BaseVariableNode)newVariableNode);
            switch (scope)
            {
                case ScopeEnum.Global:
                    GlobalVariables[name] = (BaseVariableNode)newVariableNode;
                    parentVisitor.IdentifierVariables.Add(name, new Identifier(variableData.IdentifierName, variableData.Identifier));
                    break;
                case ScopeEnum.Local:
                    LocalVariables[name] = (BaseVariableNode)newVariableNode;
                    parentVisitor.IdentifierVariables.Add(name, new Identifier(variableData.IdentifierName, variableData.Identifier));
                    break;
            }

            return newVariableNode;
        }
    }
    
    /// <summary>
    /// Handles visits that set variables
    /// </summary>
    /// <param name="parentVisitor"> Visitor that called this function </param>
    /// <param name="brain"> The ScriptBrain that this node is being added to </param>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> NULL </returns>
    public static object? VisitAssignment(NodeSharpVisitorRefactor parentVisitor, ScriptBrain brain, NodeSharpParser.AssignmentContext context)
    {
        var varName = context.IDENTIFIER().GetText();
        
        var scope = ScopeVisitor.GetScope(context.SCOPE());        
        if (scope == ScopeEnum.Constant)
            throw new Exception($"Currently unable to set variables without scope");

        var value = parentVisitor.Visit(context.assignmentParameters());

        if (value is not IGetter val)
            throw new NotImplementedException();
        
        return SetVariable(parentVisitor, brain, varName, val, scope);
    }
    
    /// <summary>
    /// Handles looking up and returning constant variables
    /// </summary>
    /// <param name="value"> The variable to look for </param>
    /// <returns> Variable Node/NULL</returns>
    private static BaseVariableNode? GetConstantVariable(object? value)
    {
        if (ConstantVariables.ContainsKey(value))
            return ConstantVariables[value];

        return null;
    }

    /// <summary>
    /// Handles any visits that result in a constant variable
    /// </summary>
    /// <param name="brain"> The ScriptBrain that this node is being added to </param>
    /// <param name="context"> Supplied by ANTLR </param>
    /// <returns> Constant Variable Node </returns>
    public static object? VisitConstant(ScriptBrain brain, NodeSharpParser.ConstantContext context)
    {
        BaseVariableNode? ret = null;
        
        if (context.NUMBER() is { } i)
        {
            ret = GetConstantVariable(float.Parse(i.GetText()));
            if (ret != null) return ret;
            
            VariableData NodeData = new VariableData(float.Parse(i.GetText()), scope: ScopeEnum.Constant);
            ret = new NumberNode(brain, NodeData);
            ConstantVariables[float.Parse(i.GetText())] = ret;

            return ret;
        }
        
        if (context.VECTOR3() is {} v)
        {
            var values = v.GetText().Split(",");
            var value = new Vector3(float.Parse(values[0][8..]),
                float.Parse(values[1].Replace(" ", "")),
                float.Parse(values[2].Replace(" ", "")[..^1]));
            
            ret = GetConstantVariable(value);
            if (ret != null) return ret;
            
            VariableData NodeData = new VariableData(value, scope: ScopeEnum.Constant);
            //ret = new DeclareVector3Node(brain, NodeData);
            ConstantVariables[value] = ret;

            return ret;
        }

        if (context.STRING() is {} s)
        {
            var value = (int)System.Enum.Parse(typeof(StringTypes), s.GetText()[7..], true);
                
            ret = GetConstantVariable(value);
            if (ret != null) return ret;
            
            VariableData NodeData = new VariableData(value, scope: ScopeEnum.Constant);
            //ret = new DeclareStringNode(brain, NodeData);
            ConstantVariables[value] = ret;

            return ret;
        }

        if (context.BOOL() is {} b)
        {
            ret = GetConstantVariable(bool.Parse(b.GetText()));
            if (ret != null) return ret;
            
            VariableData NodeData = new VariableData(bool.Parse(b.GetText()), scope: ScopeEnum.Constant);
            //ret = new DeclareBooleanNode(brain, NodeData);
            ConstantVariables[bool.Parse(b.GetText())] = ret;

            return ret;
        }

        throw new NotImplementedException();
    }
}