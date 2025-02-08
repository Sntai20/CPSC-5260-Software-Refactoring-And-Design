namespace CodeSmellDetectionTest;

using CodeSmellDetection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class LongParameterListDetectorTest
{
    [Fact]
    public void DetectLongParameterLists_ShouldDetectMethodsWithLongParameterLists()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<LongParameterListDetector>>();
        var detector = new LongParameterListDetector(loggerMock.Object);
        var fileContents = @"
            public class TestClass
            {
                public void MethodWithFewParameters(int a, int b) { }
                public void MethodWithManyParameters(int a, int b, int c, int d) { }
            }";

        // Act
        detector.DetectLongParameterLists(fileContents);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Method 'MethodWithManyParameters' has a long parameter list with 4 parameters.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}