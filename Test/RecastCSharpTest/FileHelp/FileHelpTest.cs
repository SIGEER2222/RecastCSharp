
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

public class FileHelperTests
{
  private readonly ITestOutputHelper _output;

  public FileHelperTests(ITestOutputHelper output)
  {
    _output = output;
  }

  [Fact]
  public void GetPathUpToFolder_WithValidPath_ReturnsCorrectSubPath()
  {
    string fullPath = @"C:\Projects\xxx\RecastCSharp\Test\SomeFile.txt";
    string folderName = "Test";
    string expected = @"C:\Projects\xxx\RecastCSharp\Test\";

    string result = FileHelper.GetPathUpToFolder(fullPath, folderName);
    _output.WriteLine(result);
    Assert.Equal(expected, result);
  }

  [Fact]
  public void GetPathUpToFolder_WithValidRelativeFilePath_ReturnsCorrectFullPath()
  {
    string relativeFilePath = @"..\TestFolder\SubFolder\file.txt";
    string folderName = "TestFolder";
    string expected = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\TestFolder\"));

    string result = FileHelper.GetPathUpToFolder(relativeFilePath, folderName);

    Assert.Equal(expected, result);
  }

  [Fact]
  public void GetPathUpToFolder_WithValidRelativeFolderPath_ReturnsCorrectFullPath()
  {
    string relativeFolderPath = @"..\TestFolder\SubFolder";
    string folderName = "TestFolder";
    string expected = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\TestFolder\"));

    string result = FileHelper.GetPathUpToFolder(relativeFolderPath, folderName);

    Assert.Equal(expected, result);
  }

  [Fact]
  public void GetPathUpToFolder_WithNonExistingFolder_ReturnsEmptyString()
  {
    string relativePath = @"..\TestFolder\SubFolder\file.txt";
    string folderName = "NonExistingFolder";

    string result = FileHelper.GetPathUpToFolder(relativePath, folderName);

    Assert.Equal(string.Empty, result);
  }

  [Fact]
  public void GetPathUpToFolder_WithEmptyInput_ThrowsArgumentException()
  {
    string relativePath = "";
    string folderName = "TestFolder";

    var exception = Assert.Throws<ArgumentException>(() => FileHelper.GetPathUpToFolder(relativePath, folderName));
    Assert.Equal("Path and folder name cannot be null or empty.", exception.Message);
  }
}
