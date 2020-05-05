using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Constants;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    internal class ClassLibraryDotNetFrameworkProject : VisualStudioProject
    {
        public const string SpecializationType = "Class Library (.NET Framework)";
        public const string VSProjectTypeId = VisualStudioProjectTypeIds.CSharpLibrary;
        public ClassLibraryDotNetFrameworkProject(IElement element) : base(element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Cannot create a {nameof(ClassLibraryDotNetFrameworkProject)} from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
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