// using System.Numerics;
// using System.Text.RegularExpressions;
// using System.Xml.Linq;
// using Antlr4.Runtime;
// using BondReader;
// using Newtonsoft.Json;
// using NodeSharp.Grammar;
// using NodeSharp;
// using NodeSharp.Game;
// using NodeSharp.XMLImplementation;
// using NodeSharp.XMLImplementation.Misc;
// using NodeSharp.XMLImplementation.Objects;
//using InfiniteForgePacker.XML.Application;
//using InfiniteForgeConstants.ObjectSettings;

// var fileName = "Test Files/test.ns";
// var fileContents = File.ReadAllText(fileName);

//BlenderGeometry.Convert("Test Files/Geometry.txt", "Test Files/CATA_Canvas__spawnblock_vertices.mvar.xml");

// AntlrInputStream inputStream = new AntlrInputStream(fileContents);
// NodeSharpGrammarLexer nSharpLexer = new NodeSharpGrammarLexer(inputStream);
// CommonTokenStream commonTokenStream = new CommonTokenStream(nSharpLexer);
// NodeSharpGrammarParser nSharpParser = new NodeSharpGrammarParser(commonTokenStream);
// NodeSharpGrammarParser.ProgramContext nSharpContext = nSharpParser.program();
// NSharpVisitor visitor = new NSharpVisitor();
//
// visitor.Visit(nSharpContext);

//
// var JSONOutput = JsonConvert.SerializeObject(visitor.testBrain, Formatting.Indented);
// Console.Write(JSONOutput);

// NSXML.CompileXML(visitor, "Test Files/Test.mvar.xml", "Test Files/Test.mvar.xml");

//ObjectExtractor.Extract("Test Files/All Objects And Nodes.xml", "Test Files/All Objects As Enum.txt");

// XDocument xml = XDocument.Load("Test Files/Behemoth.mvar.xml");
// NSXML.AddObject(xml, new XMLObject(new GameObject((int)ObjectId.PRIMITIVE_BLOCK, "object", Vector3.Zero, Vector3.Zero, new Vector3(10, 10, 10))));
// NSXML.EnsureSchemaOrdering(xml).Save("Test Files/Behemoth.mvar.xml");

//BondProcessor temp = new BondProcessor(2);
//temp.ProcessFile("Test Files/Test.mvar");

// ReplaceType.Convert("Test Files/Replace.mvar.xml", new ValueTuple<ObjectId, ObjectId>[]
// {
//     new (ObjectId.DESERT_ROCK_CLIFF, ObjectId.ALPINE_ROCK_CLIFF),
// });

//Loads a map's XML and apply scripting logic to it
// public static void CompileXML(NSharpVisitor visitor, string inPath, string outPath)
// {
     // XDocument document = XDocument.Load(inPath);
     //
     // XMLScriptBrain brain = new XMLScriptBrain("Script",
     //     new ScriptBrainObject(visitor.IdentifierVariables, "Script", Vector3.Zero, Vector3.Zero, Vector3.One));
     //
     // AddObject(document, brain);
     //
     // document.Save(outPath);
// }

using System.Numerics;
using System.Xml.Linq;
using Antlr4.Runtime;
using InfiniteForgeConstants.ObjectSettings;
using InfiniteForgePacker.XML;
using NodeSharp;
using NodeSharp.Game;
using NodeSharp.Grammar;
using NodeSharp.NodeGraph.NodeData;
using NodeSharp.Nodes.Variable;
using NodeSharp.XMLImplementation.Objects;

var fileName = "Test Files/test.ns";
var fileContents = File.ReadAllText(fileName);

AntlrInputStream inputStream = new AntlrInputStream(fileContents);
NodeSharpLexer nSharpLexer = new NodeSharpLexer(inputStream);
CommonTokenStream commonTokenStream = new CommonTokenStream(nSharpLexer);
NodeSharpParser nSharpParser = new NodeSharpParser(commonTokenStream);
NodeSharpParser.ProgramContext nSharpContext = nSharpParser.program();
//NSharpVisitor visitor = new NSharpVisitor();
NodeSharpVisitorRefactor visitor = new NodeSharpVisitorRefactor();

visitor.Visit(nSharpContext);
visitor.testBrain.Optimize();
visitor.testBrain.GenerateConnections();

XDocument document = XDocument.Load("Test Files/Test.mvar.xml");

XMLScriptBrain brain = new XMLScriptBrain("Script",
      new ScriptBrainObject(visitor.IdentifierVariables, visitor.testBrain, transform: new Transform(Vector3.Zero, Vector3.Zero, true, Vector3.One)));

XElement brainContainer = XMLWriter.WriteStructToContainer(XMLHelper.GetObjectList(document));
brain.WriteObject(brainContainer, document);

document.Save("Test Files/Test.mvar.xml");