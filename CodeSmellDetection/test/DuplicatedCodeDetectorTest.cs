namespace CodeSmellDetectionTest;

using System;
using System.IO;
using CodeSmellDetection;
using Xunit;

public class DuplicatedCodeDetectorTest
{
    [Fact]
    public void DetectDuplicatedCode_ShouldDetectDuplicatedLines()
    {
        // Arrange
        var detector = new DuplicatedCodeDetector();
        var fileContents = @"
                int a = 1;
                int b = 2;
                int a = 1;
                int c = 3;
            ";

        // Act
        using var sw = new StringWriter();
        Console.SetOut(sw);
        detector.DetectDuplicatedCode(fileContents);
        var result = sw.ToString();

        // Assert
        Assert.Contains(
            "Duplicated code detected between lines 2 and 4",
            result,
            StringComparison.Ordinal);
    }

    [Fact]
    public void DetectDuplicatedCode_ShouldNotDetectDuplicatedLines()
    {
        // Arrange
        var detector = new DuplicatedCodeDetector();
        var fileContents = @"
                int a = 1;
                int b = 2;
                int c = 3;
                int d = 4;
            ";

        // Act
        using var sw = new StringWriter();
        Console.SetOut(sw);
        detector.DetectDuplicatedCode(fileContents);
        var result = sw.ToString();

        // Assert
        Assert.DoesNotContain(
            "Duplicated code detected",
            result,
            StringComparison.Ordinal);
    }
}