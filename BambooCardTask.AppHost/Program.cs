
var builder = DistributedApplication.CreateBuilder(args);

// Add the API project
builder.AddProject<Projects.BambooCardTask_Api>("api");

builder.Build().Run();