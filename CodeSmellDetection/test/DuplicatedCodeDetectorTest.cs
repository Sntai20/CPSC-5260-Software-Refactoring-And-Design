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
    private readonly Mock<IOptions<DuplicatedCodeDetectorOptions>> optionsMock;
    private readonly Mock<ILogger<DuplicatedCodeDetector>> loggerMock;
    private readonly DuplicatedCodeDetector detector;

    public DuplicatedCodeDetectorTest()
    {
        this.optionsMock = new Mock<IOptions<DuplicatedCodeDetectorOptions>>();
        this.loggerMock = new Mock<ILogger<DuplicatedCodeDetector>>();
        this.detector = new DuplicatedCodeDetector(
            this.optionsMock.Object,
            this.loggerMock.Object);

        _ = this.optionsMock.Setup(x => x.Value)
                       .Returns(new DuplicatedCodeDetectorOptions { JaccardThreshold = 0.75 });
    }

    [Fact]
    public void Detect_ShouldDetectDuplicatedLines()
    {
        // Arrange
        var fileContents = @"
                int a = 1;
                int b = 2;
                int a = 1;
                int c = 3;
            ";

        // Act
        var codeSmell = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Duplicated code detected between lines 2 and 4")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.NotEmpty(codeSmell.Code);
    }

    [Fact]
    public void Detect_ShouldNotDetectDuplicatedLines()
    {
        // Arrange
        var fileContents = @"
                int a = 1;
                int b = 2;
                int c = 3;
                int d = 4;
            ";

        // Act
        var codeSmell = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Duplicated code not detected.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.Null(codeSmell);
    }
}