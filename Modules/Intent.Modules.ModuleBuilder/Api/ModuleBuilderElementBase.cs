using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api
{
    internal abstract class ModuleBuilderElementBase : IModuleBuilderElement, IEquatable<ModuleBuilderElementBase>
    {
        protected readonly IElement _element;

        protected ModuleBuilderElementBase(IElement element)
        {
            _element = element;
            Folder = Api.Folder.SpecializationType.Equals(_element.ParentElement?.SpecializationType, StringComparison.OrdinalIgnoreCase) ? new Folder(_element.ParentElement) : null;
        }

        public string Id => _element.Id;
        public ModuleBuilderElementType Type
        {
            get
            {
                switch (_element.SpecializationType)
                {
                    case "C# Template":
                        return ModuleBuilderElementType.CSharpTemplate;
                    case "File Template":
                        return ModuleBuilderElementType.FileTemplate;
                    case "Decorator":
                        return ModuleBuilderElementType.Decorator;
                    default:
                        throw new Exception($"Could not determine {nameof(ModuleBuilderElementType)} from {_element.SpecializationType}");
                }
            }
        }

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;
        public string SpecializationType => _element.SpecializationType;
        public IFolder Folder { get; }
        public IEnumerable<IGenericType> GenericTypes => _element.GenericTypes;
        public IElement ParentElement => _element.ParentElement;
        public IElementApplication Application => _element.Application;
        public string Comment => _element.Comment;

        public bool IsTemplate()
        {
            return Type == ModuleBuilderElementType.CSharpTemplate || Type == ModuleBuilderElementType.FileTemplate;
        }

        public bool IsDecorator()
        {
            return Type == ModuleBuilderElementType.Decorator;
        }

        public bool Equals(ModuleBuilderElementBase other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModuleBuilderElementBase) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }

    public static class FolderExtensions
    {
        public static IList<IFolder> GetFolderPath(this IHasFolder model, bool includePackage = true)
        {
            List<IFolder> result = new List<IFolder>();

            var current = model.Folder;
            while (current != null && (includePackage || !current.IsPackage))
            {
                result.Insert(0, current);
                current = current.ParentFolder;
            }
            return result;
        }

        public static IStereotype GetStereotypeInFolders(this IHasFolder model, string stereotypeName)
        {
            var folder = model.Folder;
            while (folder != null)
            {
                if (folder.HasStereotype(stereotypeName))
                {
                    return folder.GetStereotype(stereotypeName);
                }
                folder = folder.ParentFolder;
            }
            return null;
        }
    }
}