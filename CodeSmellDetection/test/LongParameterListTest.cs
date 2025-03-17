namespace CodeSmellDetectionTest;

using CodeSmellDetection;
using CodeSmellDetection.Models;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class LongParameterListTest
{
    private readonly Mock<IOptions<LongParameterListOptions>> longParameterListDetectorOptions;
    private readonly Mock<ILogger<LongParameterList>> loggerMock;
    private readonly LongParameterList detector;

    public LongParameterListTest()
    {
        this.longParameterListDetectorOptions = new Mock<IOptions<LongParameterListOptions>>();
        this.loggerMock = new Mock<ILogger<LongParameterList>>();
        this.detector = new LongParameterList(
            this.longParameterListDetectorOptions.Object,
            this.loggerMock.Object);

        _ = this.longParameterListDetectorOptions.Setup(x => x.Value)
                       .Returns(new LongParameterListOptions { ParameterThreshold = 3 });
    }

    [Fact]
    public void Detect_ShouldDetectMethodsWithLongParameterLists()
    {
        // Arrange
        var fileContents = @"
            public class TestClass
            {
                public void MethodWithFewParameters(int a, int b) { }
                public void MethodWithManyParameters(int a, int b, int c, int d) { }
            }";

        // Act
        CodeSmell? codeSmell = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>(
                    (v, t) =>
                    v.ToString()
                     .Contains("Method 'MethodWithManyParameters' has a long parameter list with 4 parameters.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.NotEmpty(codeSmell.Code);
        Assert.Equal(5, codeSmell.LineNumber);
    }

    [Fact]
    public void Detect_ShouldNotDetectMethodsWithShortParameterLists()
    {
        // Arrange
        var fileContents = @"
            public class TestClass
            {
                public void MethodWithFewParameters(int a, int b) { }
                public void MethodWithManyParameters(int a, int b, int c) { }
            }";

        // Act
        CodeSmell? codeSmell = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>(
                    (v, t) =>
                    v.ToString()
                     .Contains("Methods with long parameter lists not detected.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.Null(codeSmell);
    }
}