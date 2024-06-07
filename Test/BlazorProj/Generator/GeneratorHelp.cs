namespace BlazorProj.Generator;
public static class GeneratorHelp
{
  public static readonly int TSRuntimeCount = 1;
  public static Task GetOrLoadModuleAsync(this Microsoft.JSInterop.TSRuntime tsRuntime) => tsRuntime.GetOrLoadModule(0, "Test");
}