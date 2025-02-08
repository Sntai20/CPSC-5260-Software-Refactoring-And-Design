using CodeSmellDetection;

string filePath = "path/to/your/source/file.cs";

var longMethodDetector = new LongMethodDetector();
var longParameterListDetector = new LongParameterListDetector();

string? fileContents = File.ReadAllText(filePath);
longMethodDetector.DetectLongMethods(fileContents);
longParameterListDetector.DetectLongParameterLists(fileContents);