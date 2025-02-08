namespace CodeSmellDetectionTest;

using System.IO;
using CodeSmellDetection;
using Xunit;

public class LongMethodDetectorTest
{
    [Fact]
    public void DetectLongMethods_ShouldDetectLongMethod()
    {
        // Arrange
        var detector = new LongMethodDetector();
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
                    line1;
                    line2;
                    line3;
                    line4;
                    line5;
                    line6;
                    line7;
                    line8;
                    line9;
                    line10;
                    line11;
                    line12;
                    line13;
                    line14;
                    line15;
                    line16;
                }
            }";

        // Act
        using var sw = new StringWriter();
        Console.SetOut(sw);
        detector.DetectLongMethods(fileContents);

        // Assert
        var result = sw.ToString().Trim();
        Assert.Contains(
            "Long Method Detected",
            result,
            StringComparison.Ordinal);
    }

    [Fact]
    public void DetectLongMethods_ShouldNotDetectShortMethod()
    {
        // Arrange
        var detector = new LongMethodDetector();
        var fileContents = @"
            public class TestClass
            {
                public void ShortMethod()
                {
                    // short method
                }
            }";

        // Act
        using var sw = new StringWriter();
        Console.SetOut(sw);
        detector.DetectLongMethods(fileContents);

        // Assert
        var result = sw.ToString().Trim();
        Assert.DoesNotContain(
            "Long Method Detected",
            result,
            StringComparison.Ordinal);
    }
}