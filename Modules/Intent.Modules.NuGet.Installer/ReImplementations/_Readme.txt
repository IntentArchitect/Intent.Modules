These are almost exact copy/pastes of files as they exist from

Repository: https://github.com/NuGet/NuGet.Client.git
Commit:     a12ef7d4dac7cdaeb7188a7143457b0263a3a9b8
Folder:     \nuget.client\src\nuget.clients\nuget.commandline\common\

The following methods are changed to be virtual so can be overridden in the CustomMsbuildProjectSystem class
- MSBuildUser.LoadAssemblies(...)
- MSBuildProjectSystem.AddFile(...)
- MSBuildProjectSystem.GetProject(...)

A new constructor was added so that _ignoreMissingFiles could be set prior to the project trying to be loaded.