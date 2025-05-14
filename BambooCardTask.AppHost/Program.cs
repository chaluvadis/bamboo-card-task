var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BambooCardTask_Api>("api");

builder.Build().Run();
