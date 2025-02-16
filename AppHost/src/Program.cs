var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CodeSmellDetection>("CodeSmellDetection");

builder.Build().Run();