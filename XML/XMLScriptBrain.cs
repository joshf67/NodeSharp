using System.Numerics;
using System.Xml.Linq;
using InfiniteForgeConstants.ObjectSettings;
using InfiniteForgePacker.XML;
using Newtonsoft.Json;
using NodeSharp.Game;
using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp.XMLImplementation.Objects;

public class XMLScriptBrain : XMLObject
{
    public int ScriptIndex;
    public ScriptBrainObject BrainObject;

    public XMLScriptBrain(string objectName, ScriptBrainObject brain, int scriptIndex = 0) : base(brain)
    {
        ScriptIndex = scriptIndex;
        BrainObject = brain;
    }

    public override void WriteObjectSpecifics(XDocument document, XContainer objectContainer = null)
    {
        //Grab the top level struct for where script brains exist or create it if it doesn't exist 
        XElement brainStruct = XMLReader.GetXContainer(document.Root, "struct", 7, createIfNull: true);

        XElement brainList = XMLReader.GetXContainer(brainStruct, "list", 1, "struct", true, true);
        XElement brainHolder = XMLWriter.WriteStructToContainer(brainList);
        
        XMLWriter.WriteObjectToContainer(brainHolder, JsonConvert.SerializeObject(BrainObject.Brain, Formatting.Indented), 1);
        
        
        
        //Grab the top level struct for where identifiers are or create it if it doesn't exist 
        XElement identifierStruct = XMLReader.GetXContainer(document.Root, "struct", 10, createIfNull: true);

        //Grab the individual parts of the identifier struct and insure required parts are clear
        var identifierNameList = XMLReader.GetXContainer(identifierStruct, "list", 1, "wstring", true, true);
        
        var identifierFreeIndexList = XMLReader.GetXContainer(identifierStruct, "list", 2, "uint32", clearOnFind: true);
        var identifierIndexList = XMLReader.GetXContainer(identifierStruct, "list", 3, "uint16", clearOnFind: true);

        // Add all identifiers to the correct parts
         foreach (var var in BrainObject.IdentifierVariables.Values.OrderBy(data => data.NodeData.Identifier))
         {
             if (var.NodeData.Scope == ScopeEnum.Constant || var.NodeData.IdentifierName == "")
                 continue;

             XMLWriter.WriteObjectToContainer(identifierNameList, var.NodeData.IdentifierName, -1);
         }
    }
}