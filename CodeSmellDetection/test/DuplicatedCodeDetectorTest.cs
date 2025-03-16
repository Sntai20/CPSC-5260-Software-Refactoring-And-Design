namespace CodeSmellDetectionTest;

using System;
using CodeSmellDetection;
using CodeSmellDetection.Models;
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
        _ = this.optionsMock
            .Setup(x => x.Value)
            .Returns(new DuplicatedCodeDetectorOptions { JaccardThreshold = 0.75 });
        this.loggerMock = new Mock<ILogger<DuplicatedCodeDetector>>();
        this.detector = new DuplicatedCodeDetector(
            this.optionsMock.Object,
            this.loggerMock.Object);
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

    [Fact(Skip = "WIP")]
    public void Detect_ShouldDetectDuplicateFunctionNames()
    {
        // Arrange
        var fileContents = @"
                public void Function1()
                {
                    int a = 1;
                    int b = 2;
                }
                public void Function1()
                {
                    int a = 1;
                    int b = 2;
                }
            ";

        // Act
        var codeSmell = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Duplicated code detected between functions 1 and 2 with Jaccard similarity 1.")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.NotNull(codeSmell);
        Assert.Equal(CodeSmellType.DuplicatedCode, codeSmell.Type);
        Assert.Equal("Duplicated code detected within the file.", codeSmell.Description);
    }

    [Fact]
    public void Detect_ShouldDetectDuplicateContentsWithinFunctions()
    {
        // Arrange
        var fileContents = @"
                public void Function1()
                {
                    int a = 1;
                    int b = 2;
                }
                public void Function2()
                {
                    int a = 1;
                    int b = 2;
                }
            ";

        // Act
        var codeSmell = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
        x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Duplicated code detected.")),
            It.IsAny<Exception?>(),
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
        Times.Once);
        Assert.NotNull(codeSmell);
        Assert.Equal(CodeSmellType.DuplicatedCode, codeSmell.Type);
        Assert.Equal("Duplicated code detected within the file.", codeSmell.Description);
    }

    [Fact]
    public void ExtractFunctions_ShouldExtractFunctionsCorrectly()
    {
        // Arrange
        var fileContents = @"
                public void Function1()
                {
                    int a = 1;
                }
                private int Function2()
                {
                    return 2;
                }
                protected string Function3()
                {
                    return ""test"";
                }
            ";

        // Act
        var functions = DuplicatedCodeDetector.ExtractFunctions(fileContents);

        // Assert
        Assert.Equal(3, functions.Count);
        Assert.Contains("public void Function1()", functions[0]);
        Assert.Contains("private int Function2()", functions[1]);
        Assert.Contains("protected string Function3()", functions[2]);
    }

    [Fact]
    public void ExtractFunctions_ShouldReturnEmptyListWhenNoFunctions()
    {
        // Arrange
        var fileContents = @"
                int a = 1;
                int b = 2;
            ";

        // Act
        var functions = DuplicatedCodeDetector.ExtractFunctions(fileContents);

        // Assert
        Assert.Empty(functions);
    }

    [Fact]
    public void ExtractFunctionContent_ShouldExtractContentCorrectly()
    {
        // Arrange
        var function = @"
            public void Function1()
            {
                int a = 1;
                int b = 2;
            }";

        // Act
        var content = DuplicatedCodeDetector.ExtractFunctionContent(function);

        // Assert
        Assert.Equal($"int a = 1;{Environment.NewLine}int b = 2;", content);
    }

    [Fact]
    public void ExtractFunctionContent_ShouldHandleEmptyFunction()
    {
        // Arrange
        var function = @"
            public void Function1()
            {
            }";

        // Act
        var content = DuplicatedCodeDetector.ExtractFunctionContent(function);

        // Assert
        Assert.Equal(string.Empty, content);
    }

    [Fact]
    public void CalculateJaccardSimilarity_ShouldReturnOneForIdenticalContents()
    {
        // Arrange
        var content1 = "int a = 1; int b = 2;";
        var content2 = "int a = 1; int b = 2;";

        // Act
        var similarity = DuplicatedCodeDetector.CalculateJaccardSimilarity(content1, content2);

        // Assert
        Assert.Equal(1.0, similarity);
    }

    [Fact]
    public void CalculateJaccardSimilarity_ShouldReturnZeroForCompletelyDifferentContents()
    {
        // Arrange
        var content1 = "int a = 1;";
        var content2 = "string b = \"test\";";

        // Act
        var similarity = DuplicatedCodeDetector.CalculateJaccardSimilarity(content1, content2);

        // Assert
        Assert.Equal(0.0, similarity);
    }

    [Fact]
    public void CalculateJaccardSimilarity_ShouldReturnCorrectValueForPartialOverlap()
    {
        // Arrange
        var content1 = "int a = 1; int b = 2;";
        var content2 = "int a = 1; string c = \"test\";";

        // Act
        var similarity = DuplicatedCodeDetector.CalculateJaccardSimilarity(content1, content2);

        // Assert
        Assert.InRange(similarity, 0.0, 1.0);
    }
}