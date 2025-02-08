namespace CodeSmellDetection;

internal class LongMethodDetector
{
    public void DetectLongMethods(string[]? lines)
    {
        int methodLineCount = 0;
        bool inMethod = false;

        foreach (var line in lines)
        {
            if (line.Trim().StartsWith("public") ||
                line.Trim().StartsWith("private") ||
                line.Trim().StartsWith("protected") ||
                line.Trim().StartsWith("internal"))
            {
                if (inMethod && methodLineCount >= 15)
                {
                    Console.WriteLine("Long Method Detected");
                }

                inMethod = true;
                methodLineCount = 0;
            }

            if (inMethod && !string.IsNullOrWhiteSpace(line))
            {
                methodLineCount++;
            }

            if (line.Trim() == "}")
            {
                if (inMethod && methodLineCount >= 15)
                {
                    Console.WriteLine("Long Method Detected");
                }

                inMethod = false;
            }
        }
    }
}