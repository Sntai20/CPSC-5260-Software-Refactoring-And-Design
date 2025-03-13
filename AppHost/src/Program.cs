var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CodeSmellDetection>("CodeSmellDetection");

builder.AddProject<Projects.UserInterface>("UserInterface");

builder.Build().Run();