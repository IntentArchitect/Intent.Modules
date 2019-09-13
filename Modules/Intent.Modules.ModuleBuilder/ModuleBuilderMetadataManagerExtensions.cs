using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;

namespace Intent.Modules.ModuleBuilder
{
    public static class ModuleBuilderMetadataManagerExtensions
    {
        public static IEnumerable<ITemplateDefinition> GetTemplateDefinitions(this IMetadataManager metadataManager, IApplication application)
        {
            return new ModuleBuilderMetadataProvider(metadataManager).GetTemplateDefinitions(application);
        }

        public static IEnumerable<ITemplateDefinition> GetCSharpTemplates(this IMetadataManager metadataManager, IApplication application)
        {
            return new ModuleBuilderMetadataProvider(metadataManager).GetCSharpTemplates(application);
        }

        public static IEnumerable<ITemplateDefinition> GetFileTemplates(this IMetadataManager metadataManager, IApplication application)
        {
            return new ModuleBuilderMetadataProvider(metadataManager).GetFileTemplates(application);
        }

        public static IEnumerable<IDecoratorDefinition> GetDecorators(this IMetadataManager metadataManager, IApplication application)
        {
            return new ModuleBuilderMetadataProvider(metadataManager).GetDecorators(application);
        }

        public static bool IsCSharpTemplate(this IElement model)
        {
            return model.SpecializationType == "C# Template";
        }

        public static bool IsFileTemplate(this IElement model)
        {
            return model.SpecializationType == "File Template";
        }

        public static bool IsTemplate(this IElement model)
        {
            return model.IsCSharpTemplate() || model.IsFileTemplate();
        }

        public static bool IsDecorator(this IElement model)
        {
            return model.SpecializationType == "Decorator";
        }

        public static bool IsModeler(this IElement model)
        {
            return model.SpecializationType == Modeler.SpecializationType;
        }
    }
}