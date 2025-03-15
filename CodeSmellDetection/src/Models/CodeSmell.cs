namespace CodeSmellDetection.Models;

public class CodeSmell
{
    public CodeSmellType Type { get; set; }

    public string FileName { get; set; }

    public string Description { get; set; }

    public int LineNumber { get; set; }

    public int StartLine { get; set; }

    public int EndLine { get; set; }

    public string Code { get; set; }

    public string SmellRecommendation { get; set; }

    public string SmellCodeRecommendation { get; set; }
}