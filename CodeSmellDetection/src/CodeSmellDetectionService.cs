using CodeSmellDetection;

public class CodeSmellDetectionService(DuplicatedCodeDetector detector)
{
    private readonly DuplicatedCodeDetector detector = detector;

    public List<CodeSmell> DetectCodeSmells(string code)
    {
        return this.detector.Detect(code);
    }

    public class CodeSmell
    {
        public string Type { get; set; }

        public string Description { get; set; }

        public int LineNumber { get; set; }

        public double Similarity { get; set; }
        
        public string FilePath { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public string Code { get; set; }
        public string MethodName { get; set; }
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public string SmellType { get; set; }
        public string SmellCategory { get; set; }
        public string SmellSeverity { get; set; }
        public string SmellDescription { get; set; }
        public string SmellRecommendation { get; set; }
        public string SmellLocation { get; set; }
        public string SmellCode { get; set; }
        public string SmellCodeType { get; set; }
        public string SmellCodeCategory { get; set; }
        public string SmellCodeSeverity { get; set; }
        public string SmellCodeRecommendation { get; set; }
        public string SmellCodeLocation { get; set; }
        public string SmellCodeDescription { get; set; }
        public string SmellCodeName { get; set; }
        public string SmellCodeNamespace { get; set; }
        public string SmellCodeClass { get; set; }
        public string SmellCodeMethod { get; set; }
        public string SmellCodeFile { get; set; }
    }

}