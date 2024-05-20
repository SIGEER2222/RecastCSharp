using System.IO;
using System.Text.Json;

namespace Hachi.Data.Json;
internal static class JsonMerger
{
  public static void GenerateCode(string className, string source)
  {
    var namespaceName = "classSymbol.ContainingNamespace.ToDisplayString";
    var document = JsonDocument.Parse(source);
    var root = document.RootElement;
    var emitter = new JsonClassEmitter(namespaceName, className);
    var obj = new JsonObject(root, emitter);
    emitter.Emit(className, obj, true);

    File.WriteAllText($"{className}.g.cs", emitter.GetResult());
  }
}
