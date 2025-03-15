namespace CodeSmellDetectionTest;

using CodeSmellDetection;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class LongMethodDetectorTest
{
    [Fact]
    public void DetectLongMethods_LongMethodDetected_LogsInformation()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<LongMethodDetectorOptions>>();
        var loggerMock = new Mock<ILogger<LongMethodDetector>>();
        var detector = new LongMethodDetector(
            optionsMock.Object,
            loggerMock.Object);

        _ = optionsMock.Setup(x => x.Value)
                       .Returns(new LongMethodDetectorOptions { MethodLineCountThreshold = 15 });

        string fileContents = @"
            public class TestClass
            {
                public void ShortMethod()
                {
                    // short method
                }

                public void LongMethod()
                {
                    // long method
                    // Line 1
                    // Line 2
                    // Line 3
                    // Line 4
                    // Line 5
                    // Line 6
                    // Line 7
                    // Line 8
                    // Line 9
                    // Line 10
                    // Line 11
                    // Line 12
                    // Line 13
                    // Line 14
                    // Line 15
                    // Line 16
                }
            }";

        // Act
        var codeSmell = detector.Detect(fileContents);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Long Method Detected.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.NotEmpty(codeSmell.Code);
        Assert.Equal(9, codeSmell.StartLine);
        Assert.Equal(28, codeSmell.LineNumber);
    }

    [Fact]
    public void DetectLongMethods_NoLongMethodDetected_LogsInformation()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<LongMethodDetectorOptions>>();
        var loggerMock = new Mock<ILogger<LongMethodDetector>>();
        var detector = new LongMethodDetector(
            optionsMock.Object,
            loggerMock.Object);

        _ = optionsMock.Setup(x => x.Value)
                       .Returns(new LongMethodDetectorOptions { MethodLineCountThreshold = 15 });

        string fileContents = @"
            public class TestClass
            {
                public void ShortMethod()
                {
                     // short method
                    // Line 1
                    // Line 2
                    // Line 3
                }
            }";

        // Act
        var codeSmell = detector.Detect(fileContents);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Long Method Not Detected.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.Null(codeSmell);
    }
}