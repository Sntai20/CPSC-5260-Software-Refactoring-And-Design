namespace CodeSmellDetectionTest;

using CodeSmellDetection;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class LongParameterListDetectorTest
{
    [Fact]
    public void DetectLongParameterLists_ShouldDetectMethodsWithLongParameterLists()
    {
        // Arrange
        var longParameterListDetectorOptions = new Mock<IOptions<LongParameterListDetectorOptions>>();
        var loggerMock = new Mock<ILogger<LongParameterListDetector>>();
        var detector = new LongParameterListDetector(
            longParameterListDetectorOptions.Object,
            loggerMock.Object);

        _ = longParameterListDetectorOptions.Setup(x => x.Value)
                       .Returns(new LongParameterListDetectorOptions { ParameterThreshold = 3 });

        var fileContents = @"
            public class TestClass
            {
                public void MethodWithFewParameters(int a, int b) { }
                public void MethodWithManyParameters(int a, int b, int c, int d) { }
            }";

        // Act
        detector.Detect(fileContents);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Method 'MethodWithManyParameters' has a long parameter list with 4 parameters.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}