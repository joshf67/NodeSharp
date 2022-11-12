namespace NodeSharp;

public static class NodeTypes
{
    //Variable Keywords
    public const string Boolean = "Bool";
    public const string BooleanDeclare = "Declare Bool Variable";
    public const string BooleanSetter = "Set Bool Variable";
    public const string BooleanGetter = "Get Bool Variable";
    
    public const string Number = "Number";
    public const string NumberDeclare = "Declare Number Variable";
    public const string NumberSetter = "Set Number Variable";
    public const string NumberGetter = "Get Number Variable";
    
    public const string String = "String";
    public const string StringDeclare = "Declare String Variable";
    public const string StringSetter = "Set String Variable";
    public const string StringGetter = "Get String Variable";
    
    public const string Vector3 = "Vector3";
    public const string Vector3Declare = "Declare Vector3 Variable";
    public const string Vector3Setter = "Set Vector3 Variable";
    public const string Vector3Getter = "Get Vector3 Variable";
    
    public const string Object = "Object Reference";
    public const string ObjectDeclare = "Declare Object Variable";
    public const string ObjectSetter = "Set Object Variable";
    public const string ObjectGetter = "Get Object Variable";
    
    //Math Keywords
    public const string AddNumber = "Add";
    public const string DivideNumber = "Divide";
    public const string MultiplyNumber = "Multiply";
    public const string SquareRootNumber = "Square Root";
    public const string SubtractNumber = "Subtract";
    
    //Events
    public const string OnGameStart = "On Game Start";
    public const string OnCustomEvent = "On Custom Event";
    public const string OnCustomEventGlobal = "On Custom Event Global";
    public const string TriggerCustomEvent = "Trigger Custom Event";
    public const string TriggerCustomEventGlobal = "Trigger Custom Event Global";
    
    //Logic
    public const string Branch = "Branch";
}