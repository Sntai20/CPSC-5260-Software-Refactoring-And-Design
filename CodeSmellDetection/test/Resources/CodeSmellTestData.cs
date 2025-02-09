namespace CodeSmellDetectionTest.Resources;

internal class CodeSmellTestData
{
    int a = 1;
    int b = 2;
    int a = 1;
    int c = 3;

    public void MethodWithFewParameters(int a, int b) { }
    public void MethodWithManyParameters(int a, int b, int c, int d) { }

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
}