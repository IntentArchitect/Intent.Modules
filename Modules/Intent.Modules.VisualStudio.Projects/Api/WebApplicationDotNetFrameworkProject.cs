using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Constants;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    internal class WebApplicationDotNetFrameworkProject : VisualStudioProject
    {
        public const string SpecializationType = "ASP.NET Web Application (.NET Framework)";
        public const string VSProjectTypeId = VisualStudioProjectTypeIds.WebApiApplication;
        public WebApplicationDotNetFrameworkProject(IElement element) : base(element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Cannot create a {nameof(WebApplicationDotNetFrameworkProject)} from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
        }

        public override string ProjectTypeId => VSProjectTypeId;
        public override string Type => SpecializationType;

        public override string TargetFrameworkVersion()
        {
            return this.GetStereotypeProperty<string>(".NET Framework Settings", "Target Framework") ?? throw new Exception($"[.NET Framework Settings] stereotype is missing on project {Name}");
        }
    }
}