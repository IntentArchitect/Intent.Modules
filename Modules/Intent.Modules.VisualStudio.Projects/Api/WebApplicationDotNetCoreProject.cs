using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Constants;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    internal class WebApplicationDotNetCoreProject : VisualStudioProject
    {
        public const string SpecializationType = "ASP.NET Core Web Application";
        public const string VSProjectTypeId = VisualStudioProjectTypeIds.CoreWebApp;
        public WebApplicationDotNetCoreProject(IElement element) : base(element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Cannot create a {nameof(WebApplicationDotNetCoreProject)} from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
        }

        public override string ProjectTypeId => VSProjectTypeId;
        public override string Type => SpecializationType;

        public override string TargetFrameworkVersion()
        {
            return this.GetStereotypeProperty<string>(".NET Core Settings", "Target Framework") ?? throw new Exception($"[.NET Core Settings] stereotype is missing on project {Name}");
        }
    }
}