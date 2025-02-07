
using CodeSmellDetection;

string filePath = "path/to/your/source/file.cs";
var detector = new LongMethodDetector();

detector.DetectLongMethods(filePath);