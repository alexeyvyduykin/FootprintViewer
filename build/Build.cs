using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    readonly string Configuration = "Release";

    const string RunProjectName = "FootprintViewer.Avalonia";

    [Solution] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() => SourceDirectory
            .GlobFiles("**/*.Avalonia.csproj")
            .ForEach(path =>
            {
                DotNetRestore(settings => settings
                    .SetProjectFile(path));
            }));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => SourceDirectory
            .GlobFiles("**/*.Avalonia.csproj")
            .ForEach(path =>
            {
                DotNetBuild(settings => settings
                    .SetProjectFile(path)
                    .SetConfiguration(Configuration)
                    .EnableNoRestore());
            }));

    Target Run => _ => _
        .DependsOn(Compile)
        .Executes(() => SourceDirectory
            .GlobFiles($"**/{RunProjectName}.csproj")
            .ForEach(path =>
            {
                DotNetRun(settings => settings
                    .SetProjectFile(path)
                    .SetConfiguration(Configuration)
                    .EnableNoRestore()
                    .EnableNoBuild());
            }));

    Target Publish => _ => _
        .DependsOn(Clean)
        .DependsOn(Compile)
        .Executes(() =>
        {
            var rids = new[] { "win-x64", "linux-x64" };

            DotNetPublish(settings => settings
                .SetProject(Solution.GetProject("FootprintViewer.Avalonia"))
                .SetPublishSingleFile(true)
                .SetSelfContained(true)
                .SetConfiguration(Configuration)
                .CombineWith(rids, (settings, rid) => settings
                    .SetRuntime(rid)
                    .SetOutput(ArtifactsDirectory / "Publish" / rid)));
        });
}
