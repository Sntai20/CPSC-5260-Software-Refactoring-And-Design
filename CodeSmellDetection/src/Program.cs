using CodeSmellDetection;

string filePath = "path/to/your/source/file.cs";

var longMethodDetector = new LongMethodDetector();
var longParameterListDetector = new LongParameterListDetector();
var duplicatedCodeDetector = new DuplicatedCodeDetector();

string? fileContents = File.ReadAllText(filePath);
longMethodDetector.DetectLongMethods(fileContents);
longParameterListDetector.DetectLongParameterLists(fileContents);
duplicatedCodeDetector.DetectDuplicatedCode(fileContents);