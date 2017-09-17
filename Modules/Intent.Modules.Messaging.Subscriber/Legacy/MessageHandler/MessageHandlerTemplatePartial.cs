using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Common;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Messaging.Subscriber.Legacy.MessageHandler
{
    public partial class MessageHandlerTemplate : IntentRoslynProjectItemTemplateBase<TypeModel>, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Messaging.Subscriber.MessageHandler.Legacy";

        public MessageHandlerTemplate(IProject project, TypeModel eventType)
            : base (Identifier, project, eventType)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0")); ;
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.TypeName}Handler",
                fileExtension: "cs",
                defaultLocationInProject: "MessageHandlers"
                );
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
