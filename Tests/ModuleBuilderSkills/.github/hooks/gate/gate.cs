#:property PublishAot=false
#:include SemVer.cs
#:include HarnessProtocol.cs
#:include GitSupport.cs
#:include IntentMetadataGuard.cs
#:include ModuleVersionAuditor.cs
#:include CloseOutAuditor.cs
#:include GuardVersionSupport.cs
#:include Cli.cs

using Intent.Agent.Gate;

return Cli.Run(args, Console.In, Console.Out, Console.Error, Directory.GetCurrentDirectory());