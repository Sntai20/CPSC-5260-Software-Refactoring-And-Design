namespace CodeSmellDetectionTest.Detections;

using CodeSmellDetection.Detections;
using CodeSmellDetection.Models;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class LongMethodTest
{
    private readonly Mock<IOptions<LongMethodOptions>> optionsMock;
    private readonly Mock<ILogger<LongMethod>> loggerMock;
    private readonly LongMethod detector;

    public LongMethodTest()
    {
        this.optionsMock = new Mock<IOptions<LongMethodOptions>>();
        this.loggerMock = new Mock<ILogger<LongMethod>>();
        this.detector = new LongMethod(
            this.optionsMock.Object,
            this.loggerMock.Object);

        _ = this.optionsMock.Setup(x => x.Value)
                       .Returns(new LongMethodOptions { MethodLineCountThreshold = 9 });
    }

    [Fact(Skip = "WIP")]
    public void Detect_LongMethodDetected_LogsWarning()
    {
        // Arrange
        string fileContents = @"
            public class TestClass
            {
                public void ShortMethod()
                {
                    // short method
                }

                public void LongMethod()
                {
                    int n = 10;
                    for (int i = 0; i < n; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{n}, "");
                        }
                    }

                    int q = 10;
                    for (int i = 0; i < q; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{q}, "");
                        }
                    }

                    int t = 10;
                    for (int i = 0; i < t; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{t}, "");
                        }
                    }
                }
            }";

        // Act
        var codeSmells = this.detector.Detect(fileContents);

        // Assert
        /*this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Long Method Detected from line 1 to 2.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);*/
        Assert.Empty(codeSmells);
        Assert.NotEmpty(codeSmells[0].Code);
        Assert.Equal(9, codeSmells[0].StartLine);
    }

    [Fact]
    public void Detect_LongMethodNotDetected_LogsInformation()
    {
        // Arrange
        string fileContents = @"
            public class TestClass
            {
                public void ShortMethod()
                {
                     // short method
                    int n = 10;
                    for (int i = 0; i < n; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{n}, "");
                        }
                    }
                }
            }";

        // Act
        var codeSmells = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Long Methods Not Detected.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.Empty(codeSmells);
    }

    [Fact(Skip = "WIP")]
    public void Detect_LongMethodDetected_BlankLineIgnored()
    {
        // Arrange
        string fileContents = @"
            public class TestClass
            {
                public void ShortMethod()
                {
                    // short method
                }

                public void LongMethod()
                {

                    int n = 10;
                    for (int i = 0; i < n; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{n}, "");
                        }
                    }

                    int t = 10;
                    for (int i = 0; i < t; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{t}, "");
                        }
                    }

                }
            }";

        // Act
        var codeSmells = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Long Method Detected.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.NotEmpty(codeSmells[0].Code);
        Assert.Equal(9, codeSmells[0].StartLine);
    }

    [Fact]
    public void IsClassDeclaration_PublicClass_ReturnsTrue()
    {
        // Arrange
        string line = "public class TestClass";

        // Act
        bool result = LongMethod.IsClassDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsClassDeclaration_PrivateClass_ReturnsTrue()
    {
        // Arrange
        string line = "private class TestClass";

        // Act
        bool result = LongMethod.IsClassDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsClassDeclaration_ProtectedClass_ReturnsTrue()
    {
        // Arrange
        string line = "protected class TestClass";

        // Act
        bool result = LongMethod.IsClassDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsClassDeclaration_InternalClass_ReturnsTrue()
    {
        // Arrange
        string line = "internal class TestClass";

        // Act
        bool result = LongMethod.IsClassDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMethodDeclaration_PublicMethod_ReturnsTrue()
    {
        // Arrange
        string line = "public void TestMethod()";

        // Act
        bool result = LongMethod.IsMethodDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMethodDeclaration_PrivateMethod_ReturnsTrue()
    {
        // Arrange
        string line = "private void TestMethod()";

        // Act
        bool result = LongMethod.IsMethodDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMethodDeclaration_ProtectedMethod_ReturnsTrue()
    {
        // Arrange
        string line = "protected void TestMethod()";

        // Act
        bool result = LongMethod.IsMethodDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMethodDeclaration_InternalMethod_ReturnsTrue()
    {
        // Arrange
        string line = "internal void TestMethod()";

        // Act
        bool result = LongMethod.IsMethodDeclaration(line);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMethodDeclaration_NotMethodDeclaration_ReturnsFalse()
    {
        // Arrange
        string line = "public class TestClass";

        // Act
        bool result = LongMethod.IsMethodDeclaration(line);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreateCodeSmell_ValidInput_ReturnsCodeSmell()
    {
        // Arrange
        string fileContents = "public void TestMethod() { }";
        int startLine = 1;
        int endLine = 10;

        // Act
        var codeSmell = LongMethod.CreateCodeSmell(fileContents, startLine, endLine);

        // Assert
        Assert.NotNull(codeSmell);
        Assert.Equal(CodeSmellType.LongMethod, codeSmell.Type);
        Assert.Equal($"Long Method Detected from line {startLine} to {endLine}.", codeSmell.Description);
        Assert.Equal(startLine, codeSmell.StartLine);
        Assert.Equal(endLine, codeSmell.EndLine);
        Assert.Equal(fileContents, codeSmell.Code);
    }

    [Fact]
    public void RemoveComments_SingleLineComment_RemovesComment()
    {
        // Arrange
        string line = "int x = 0; // this is a comment";

        // Act
        string result = LongMethod.RemoveComments(line);

        // Assert
        Assert.Equal("int x = 0; ", result);
    }

    [Fact]
    public void RemoveComments_MultiLineComment_RemovesComment()
    {
        // Arrange
        string line = "int x = 0; /* this is a comment */ int y = 1;";

        // Act
        string result = LongMethod.RemoveComments(line);

        // Assert
        Assert.Equal("int x = 0;  int y = 1;", result);
    }

    [Fact]
    public void RemoveComments_NoComment_ReturnsOriginalLine()
    {
        // Arrange
        string line = "int x = 0;";

        // Act
        string result = LongMethod.RemoveComments(line);

        // Assert
        Assert.Equal(line, result);
    }

    [Fact(Skip = "WIP")]
    public void Detect_LongMethodDetected_MultipleMethods()
    {
        // Arrange
        string fileContents = @"
            public class TestClass
            {
                public void ShortMethod()
                {
                    // short method
                }

                public void LongMethod()
                {
                    int n = 10;
                    for (int i = 0; i < n; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{n}, "");
                        }
                    }

                    int q = 10;
                    for (int i = 0; i < q; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{q}, "");
                        }
                    }

                    int t = 10;
                    for (int i = 0; i < t; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{t}, "");
                        }
                    }
                }

                public void AnotherLongMethod()
                {
                    int n = 10;
                    for (int i = 0; i < n; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{n}, "");
                        }
                    }

                    int q = 10;
                    for (int i = 0; i < q; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{q}, "");
                        }
                    }

                    int t = 10;
                    for (int i = 0; i < t; i++)
                    {
                        if (i == 1)
                        {
                            Console.WriteLine($""{t}, "");
                        }
                    }
                }
            }";

        // Act
        var codeSmells = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Long Method Detected.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Exactly(2));
        Assert.Equal(2, codeSmells.Count);
    }

    [Fact]
    public void Detect_LongMethodNotDetected_EmptyMethod()
    {
        // Arrange
        string fileContents = @"
            public class TestClass
            {
                public void EmptyMethod()
                {
                }
            }";

        // Act
        var codeSmells = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Long Methods Not Detected.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.Empty(codeSmells);
    }
}