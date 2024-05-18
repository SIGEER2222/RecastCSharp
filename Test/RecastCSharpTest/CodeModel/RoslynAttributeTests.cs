using FluentAssertions;
using Microsoft.CodeAnalysis;
using RecastCSharp.CodeModel;

public class RoslynAttributeTests
{
  [Fact]
  public void Constructor_SetsAttributeData()
  {
    // Arrange
    var attributeData = CreateMockAttributeData("TestAttribute", "TestValue");

    // Act
    var roslynAttribute = new RoslynAttribute(attributeData);

    // Assert
    roslynAttribute.Name.Should().Be("Test");
    roslynAttribute.Value.Should().Be("TestValue");
  }

  [Fact]
  public void Values_ReturnsReadOnlyScriptArray()
  {
    // Arrange
    var attributeData = CreateMockAttributeData("TestAttribute", new object[] { "Value1", "Value2" });

    // Act
    var roslynAttribute = new RoslynAttribute(attributeData);

    // Assert
    var values = roslynAttribute.Values;
    Assert.True(values.IsReadOnly);
    Assert.Equal(2, values.Count);
    Assert.Equal("Value1", values[0]);
    Assert.Equal("Value2", values[1]);
  }

  private AttributeData CreateMockAttributeData(string attributeName, object value)
  {
    return null;
  }
}