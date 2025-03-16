namespace CodeSmellDetectionTest;

using CodeSmellDetection;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class LongMethodDetectorTest
{
    private readonly Mock<IOptions<LongMethodDetectorOptions>> optionsMock;
    private readonly Mock<ILogger<LongMethodDetector>> loggerMock;
    private readonly LongMethodDetector detector;

    public LongMethodDetectorTest()
    {
        this.optionsMock = new Mock<IOptions<LongMethodDetectorOptions>>();
        this.loggerMock = new Mock<ILogger<LongMethodDetector>>();
        this.detector = new LongMethodDetector(
            this.optionsMock.Object,
            this.loggerMock.Object);

        _ = this.optionsMock.Setup(x => x.Value)
                       .Returns(new LongMethodDetectorOptions { MethodLineCountThreshold = 15 });
    }

    [Fact]
    public void Detect_LongMethodDetected_LogsInformation()
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
                    // long method
                    // Line 1
                    // Line 2
                    // Line 3
                    // Line 4
                    // Line 5
                    // Line 6
                    // Line 7
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
    public void Detect_NoLongMethodDetected_LogsInformation()
    {
        // Arrange
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
        var codeSmells = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Long Method Not Detected.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.Empty(codeSmells);
    }

    [Fact]
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
                    // long method
                    // Line 1

                    // Line 2

                    // Line 3
                    // Line 4
                    // Line 5
                    // Line 6
                    // Line 7
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
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Long Method Detected.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.NotEmpty(codeSmells[0].Code);
        Assert.Equal(9, codeSmells[0].StartLine);
    }
}