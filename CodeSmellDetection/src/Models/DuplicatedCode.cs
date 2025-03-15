namespace CodeSmellDetection.Models;

public class DuplicatedCode
{
    public string Description { get; set; } = "Duplicated code detected within the file.";

    public double Similarity { get; set; }
}