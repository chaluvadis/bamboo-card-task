using ADotNet.Clients.Builders;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;

string buildScriptPath =
            "../BambooCardTask.API/.github/workflows/dotnet.yml";

string directoryPath = Path.GetDirectoryName(buildScriptPath);

if (Directory.Exists(directoryPath) is false)
{
    Directory.CreateDirectory(directoryPath);
}

GitHubPipelineBuilder.CreateNewPipeline()
    .SetName("BambooCard API Build")
        .OnPush("main")
        .OnPullRequest("main")
            .AddJob("build", job => job
            .WithName("Build")
            .RunsOn(BuildMachines.Windows2022)
                .AddCheckoutStep("Check out")
                .AddSetupDotNetStep(version: "10.0.0-preview.3.25171.5")
                .AddRestoreStep()
                .AddBuildStep()
                .AddTestStep(
                    command: "dotnet test --no-build --verbosity normal --filter " +
                        "'FullyQualifiedName!~Integrations'"))

.SaveToFile(buildScriptPath);
