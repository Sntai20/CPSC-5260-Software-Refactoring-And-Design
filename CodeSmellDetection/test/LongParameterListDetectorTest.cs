namespace CodeSmellDetectionTest;

using System.IO;
using CodeSmellDetection;
using Xunit;

public class LongParameterListDetectorTest
{
    [Fact]
    public void DetectLongParameterLists_ShouldDetectMethodsWithLongParameterLists()
    {
        // Arrange
        var detector = new LongParameterListDetector();
        var fileContents = @"
            public class TestClass
            {
                public void MethodWithFewParameters(int a, int b) { }
                public void MethodWithManyParameters(int a, int b, int c, int d) { }
            }";

        // Act
        using var sw = new StringWriter();
        Console.SetOut(sw);
        detector.DetectLongParameterLists(fileContents);
        var result = sw.ToString();

        // Assert
        Assert.Contains(
            "Method 'MethodWithManyParameters' has a long parameter list with 4 parameters.",
            result,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "Method 'MethodWithFewParameters' has a long parameter list with 2 parameters.",
            result,
            StringComparison.Ordinal);
    }
}