using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;

namespace Intent.Modules.ModuleBuilder
{
    public static class ModuleBuilderMetadataManagerExtensions
    {
        public static IEnumerable<ICSharpTemplate> GetCSharpTemplates(this IMetadataManager metadataManager, IApplication application)
        {
            return new ModuleBuilderMetadataProvider(metadataManager).GetCSharpTemplates(application);
        }

        public static IEnumerable<IFileTemplate> GetFileTemplates(this IMetadataManager metadataManager, IApplication application)
        {
            return new ModuleBuilderMetadataProvider(metadataManager).GetFileTemplates(application);
        }

        public static IEnumerable<IDecoratorDefinition> GetDecorators(this IMetadataManager metadataManager, IApplication application)
        {
            return new ModuleBuilderMetadataProvider(metadataManager).GetDecorators(application);
        }

        public static IEnumerable<IModuleBuilderElement> GetAllElements(this IMetadataManager metadataManager, IApplication application)
        {
            return new ModuleBuilderMetadataProvider(metadataManager).GetAllElements(application);
        }

        public static bool IsCSharpTemplate(this IElement model)
        {
            return model.SpecializationType == "C# Template";
        }

        public static bool IsFileTemplate(this IElement model)
        {
            return model.SpecializationType == "File Template";
        }

        public static bool IsDecorator(this IElement model)
        {
            return model.SpecializationType == "Decorator";
        }
    }
}