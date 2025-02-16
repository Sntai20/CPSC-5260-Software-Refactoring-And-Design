var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CodeSmellDetection>("code-smell-detection");

builder.Build().Run();