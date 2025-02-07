namespace CodeSmellDetectionTest;

using CodeSmellDetection;
using System.IO;
using Xunit;

public class LongParameterListDetectorTest
{
    [Fact]
    public void DetectLongParameterLists_ShouldDetectMethodsWithLongParameterLists()
    {
        // Arrange
        var detector = new LongParameterListDetector();
        var testCode = @"
            public class TestClass
            {
                public void MethodWithFewParameters(int a, int b) { }
                public void MethodWithManyParameters(int a, int b, int c, int d) { }
            }";
        var testFilePath = "TestFile.cs";
        File.WriteAllText(testFilePath, testCode);

        // Act
        using (var sw = new StringWriter())
        {
            Console.SetOut(sw);
            detector.DetectLongParameterLists(testFilePath);
            var result = sw.ToString();

            // Assert
            Assert.Contains("Method 'MethodWithManyParameters' has a long parameter list with 4 parameters.", result);
            Assert.DoesNotContain("Method 'MethodWithFewParameters' has a long parameter list with 2 parameters.", result);
        }

        // Cleanup
        File.Delete(testFilePath);
    }
}