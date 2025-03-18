namespace CodeSmellDetectionTest;

using CodeSmellDetection;

public class IsMethodDeclarationHelperTest
{
    [Fact]
    public void IsMethodDeclaration_PublicMethod_ReturnsTrue()
    {
        // Arrange
        string line = "public void TestMethod()";

        // Act
        bool result = IsMethodDeclarationHelper.IsMethodDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMethodDeclaration_PrivateMethod_ReturnsTrue()
    {
        // Arrange
        string line = "private void TestMethod()";

        // Act
        bool result = IsMethodDeclarationHelper.IsMethodDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMethodDeclaration_ProtectedMethod_ReturnsTrue()
    {
        // Arrange
        string line = "protected void TestMethod()";

        // Act
        bool result = IsMethodDeclarationHelper.IsMethodDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMethodDeclaration_InternalMethod_ReturnsTrue()
    {
        // Arrange
        string line = "internal void TestMethod()";

        // Act
        bool result = IsMethodDeclarationHelper.IsMethodDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMethodDeclaration_NotMethodDeclaration_ReturnsFalse()
    {
        // Arrange
        string line = "public class TestClass";

        // Act
        bool result = IsMethodDeclarationHelper.IsMethodDeclaration(line);

        // Assert
        Assert.False(result);
    }
}