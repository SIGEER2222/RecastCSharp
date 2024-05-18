using Scriban;

namespace RecastCSharpTest.JsonToRecord;
public class JsonToRecord
{
  [Fact]
  public async Task ShouldConvertJsonToRecord()
  {
    // Arrange
    var model = new
    {
      Namespace = "TestNamespace",
      tool_name = "RecastCSharp",
      tool_version = "1.0.0",
      Records = new List<object>
      {
        new
        {
          Name = "LogLevel",
          Properties = new List<object>
          {
              new { JsonName = "Default", Type = "string", Name = "Default" },
              new { JsonName = "System", Type = "string", Name = "System" },
              new { JsonName = "Microsoft", Type = "string", Name = "Microsoft" }
          }
        }
      }
    };

    var templateContent = File.ReadAllText(@"D:\桌面\RecastCSharp\Test\RecastCSharpTest\JsonToClass\JsonToRecord.sbntxt");
    var template = Template.Parse(templateContent);
    await Verify(async () => await template.RenderAsync(model));
  }
}