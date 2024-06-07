using System.IO.Abstractions;
using Moq;
using RecastCSharp.CodeModel;
using RecastCSharp.FileSystem;
using RecastCSharp.Scriban;
using Xunit.Abstractions;

namespace RecastCSharpTest.FileSystem;
public class WatcherTest
{
  private readonly ITestOutputHelper _output;
  public WatcherTest(ITestOutputHelper output)
  {
    _output = output;
  }
  [Fact]
  public void Watcher_CreatesFileChange_OnFileChanged()
  {
    // Arrange
    var projectPath = FileHelper.GetPathUpToFolder(AppContext.BaseDirectory, @"BlazorProj.csproj");
    _output.WriteLine(projectPath);

    var mockMainScript = new Mock<MainScript>();
    var mockSolution = new Mock<Solution>();
    var watcher = new Watcher("testFolder", mockMainScript.Object, mockSolution.Object);
  }
}