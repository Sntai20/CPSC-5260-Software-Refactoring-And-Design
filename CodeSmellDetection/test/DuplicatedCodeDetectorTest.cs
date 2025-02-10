namespace CodeSmellDetectionTest;

using System;
using CodeSmellDetection;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class DuplicatedCodeDetectorTest
{
    [Fact]
    public void DetectDuplicatedCode_ShouldDetectDuplicatedLines()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<DuplicatedCodeDetectorOptions>>();
        var loggerMock = new Mock<ILogger<DuplicatedCodeDetector>>();
        var detector = new DuplicatedCodeDetector(
            optionsMock.Object,
            loggerMock.Object);

        _ = optionsMock.Setup(x => x.Value)
                       .Returns(new DuplicatedCodeDetectorOptions { JaccardThreshold = 0.75 });

        var fileContents = @"
                int a = 1;
                int b = 2;
                int a = 1;
                int c = 3;
            ";

        // Act
        detector.DetectDuplicatedCode(fileContents);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Duplicated code detected between lines 2 and 4")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void DetectDuplicatedCode_ShouldNotDetectDuplicatedLines()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<DuplicatedCodeDetectorOptions>>();
        var loggerMock = new Mock<ILogger<DuplicatedCodeDetector>>();
        var detector = new DuplicatedCodeDetector(
            optionsMock.Object,
            loggerMock.Object);

        _ = optionsMock.Setup(x => x.Value)
                       .Returns(new DuplicatedCodeDetectorOptions { JaccardThreshold = 0.75 });

        var fileContents = @"
                int a = 1;
                int b = 2;
                int c = 3;
                int d = 4;
            ";

        // Act
        detector.DetectDuplicatedCode(fileContents);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Duplicated code detected")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Never);
    }
}