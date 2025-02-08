using CodeSmellDetection;

string filePath = "path/to/your/source/file.cs";

var longMethodDetector = new LongMethodDetector();
var longParameterListDetector = new LongParameterListDetector();

string[]? fileContents = File.ReadAllLines(filePath);
longMethodDetector.DetectLongMethods(fileContents);
longParameterListDetector.DetectLongParameterLists(filePath);