using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;

namespace Intent.Modules.ModuleBuilder
{
    public static class ModuleBuilderMetadataManagerExtensions
    {
        //public static IEnumerable<FileTemplate> GetTemplateDefinitions(this IMetadataManager metadataManager, IApplication application)
        //{
        //    return new ModuleBuilderMetadataProvider(metadataManager).GetTemplateDefinitions(application.Id);
        //}

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

        //public static bool IsDecorator(this IElement model)
        //{
        //    return model.SpecializationType == "Decorator";
        //}

        //public static bool IsModeler(this IElement model)
        //{
        //    return ModelerReference.SpecializationType.Contains(model.SpecializationType);
        //}
    }
}