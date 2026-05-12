using Intent.Engine;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Common.Templates.DotMcpDotJsonFile
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DotMcpDotJsonFileTemplate : IntentTemplateBase<object>, IDataFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Common.DotMcpDotJsonFile";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DotMcpDotJsonFileTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var mcpServerLaunchInfo = outputTarget.ExecutionContext.GetMcpServerLaunchInfo();

            DataFile = new DataFile($"DotMcpDotJsonFile", overwriteBehaviour: OverwriteBehaviour.OnceOff)
                .WithFileName(".mcp")
                .WithRelativeLocation("..")
                .WithJsonWriter()
                .WithRootObject(this, root =>
                {
                    root.WithObject("mcpServers", mcpServers =>
                    {
                        mcpServers.WithObject("intent-architect", ia =>
                        {
                            ia.WithValue("type", "stdio");
                            ia.WithValue("command", mcpServerLaunchInfo.Executable);
                            ia.WithArray("args", args =>
                            {
                                foreach (var argument in mcpServerLaunchInfo.Arguments)
                                {
                                    args.WithValue(argument);
                                }
                            });
                            ia.WithObject("env");
                        });
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public IDataFile DataFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => DataFile.GetConfig();

        [IntentManaged(Mode.Fully)]
        public override string TransformText() => DataFile.ToString();
    }
}