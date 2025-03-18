namespace CodeSmellDetectionTest.Detections;

using System;
using CodeSmellDetection.Detections;
using CodeSmellDetection.Models;
using CodeSmellDetection.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class StructuralDuplicateCodeTest
{
    private readonly Mock<IOptions<StructuralDuplicateCodeOptions>> optionsMock;
    private readonly Mock<ILogger<StructuralDuplicateCode>> loggerMock;
    private readonly StructuralDuplicateCode detector;

    public StructuralDuplicateCodeTest()
    {
        this.optionsMock = new Mock<IOptions<StructuralDuplicateCodeOptions>>();
        _ = this.optionsMock
            .Setup(x => x.Value)
            .Returns(new StructuralDuplicateCodeOptions { JaccardThreshold = 0.75 });
        this.loggerMock = new Mock<ILogger<StructuralDuplicateCode>>();
        this.detector = new StructuralDuplicateCode(
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
        Assert.Empty(codeSmell);
    }

    [Fact]
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
        var codeSmells = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Duplicate function name detected: Function1")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        Assert.NotNull(codeSmells);
        Assert.Equal(CodeSmellType.DuplicatedCode, codeSmells[0].Type);
        Assert.Equal("Duplicate function name detected: Function1.", codeSmells[0].Description);
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
        var codeSmells = this.detector.Detect(fileContents);

        // Assert
        this.loggerMock.Verify(
        x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Duplicated code detected between functions")),
            It.IsAny<Exception?>(),
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
        Times.Once);
        Assert.NotNull(codeSmells);
        Assert.Equal(CodeSmellType.DuplicatedCode, codeSmells[0].Type);
        Assert.Equal("Duplicated code detected between functions 2 and 7 with Jaccard similarity 1.", codeSmells[0].Description);
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
        List<(string Content, int LineNumber)>? functions = StructuralDuplicateCode.ExtractFunctions(fileContents);

        // Assert
        Assert.Equal(3, functions.Count);
        Assert.Contains("public void Function1()", functions[0].Content);
        Assert.Contains("private int Function2()", functions[1].Content);
        Assert.Contains("protected string Function3()", functions[2].Content);
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
        List<(string Content, int LineNumber)>? functions = StructuralDuplicateCode.ExtractFunctions(fileContents);

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
        var content = StructuralDuplicateCode.ExtractFunctionContent(function);

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
        var content = StructuralDuplicateCode.ExtractFunctionContent(function);

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
        var similarity = StructuralDuplicateCode.CalculateJaccardSimilarity(content1, content2);

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
        var similarity = StructuralDuplicateCode.CalculateJaccardSimilarity(content1, content2);

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
        var similarity = StructuralDuplicateCode.CalculateJaccardSimilarity(content1, content2);

        // Assert
        Assert.InRange(similarity, 0.0, 1.0);
    }
}