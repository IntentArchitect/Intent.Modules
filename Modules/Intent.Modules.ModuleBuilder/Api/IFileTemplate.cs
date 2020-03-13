using System;
using System.Collections;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface ITemplateCreationSettings : IHasStereotypes
    {
    }

    public interface ICSharpTemplate : ITemplateCreationSettings, IModuleBuilderElement
    {

    }

    public interface IFileTemplate : ITemplateCreationSettings, IModuleBuilderElement
    {
        string FileExtension { get; }
    }
}
