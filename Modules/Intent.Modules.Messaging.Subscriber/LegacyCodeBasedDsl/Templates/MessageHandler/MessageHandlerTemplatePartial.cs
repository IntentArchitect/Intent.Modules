using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Common;
using Intent.Templates

#pragma warning disable 618 // Old code based DSL

namespace Intent.Modules.Messaging.Subscriber.LegacyCodeBasedDsl.Templates.MessageHandler
{
    public partial class MessageHandlerTemplate : IntentRoslynProjectItemTemplateBase<TypeModel>, ITemplate, IHasNugetDependencies
    {
        public const string IDENTIFIER = "Intent.Messaging.LegacyCodeBasedDsl.Subscriber.MessageHandler";

        public MessageHandlerTemplate(IProject project, TypeModel model)
            : base(IDENTIFIER, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.TypeName}Handler",
                fileExtension: "cs",
                defaultLocationInProject: "MessageHandlers",
                className: "${Model.TypeName}Handler",
                @namespace: "${Project.ProjectName}.MessageHandlers");
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentEsbClient,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }
    }
}
