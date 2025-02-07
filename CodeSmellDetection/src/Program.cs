
using CodeSmellDetection;

string filePath = "path/to/your/source/file.cs";

var longMethodDetector = new LongMethodDetector();
var longParameterListDetector = new LongParameterListDetector();

longMethodDetector.DetectLongMethods(filePath);
longParameterListDetector.DetectLongParameterLists(filePath);

