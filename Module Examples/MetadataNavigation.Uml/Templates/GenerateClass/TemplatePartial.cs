using Intent.MetaModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Engine;

namespace MetadataNavigation.Uml.Templates.GenerateClass
{
    partial class Template : Intent.SoftwareFactory.Templates.IntentRoslynProjectItemTemplateBase<IClass>
    {
        public const string Identifier = "MetadataNavigation.Uml.GenerateClass";

        public Template(IProject project, IClass model)
            : base(Identifier, project, model)
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
                fileName: Model.Name,
                fileExtension: "cs",
                defaultLocationInProject: "MetadataNavigation\\Uml\\GenerateClass",
                className: Model.Name,
                @namespace: Project.Name + ".MetadataNavigation.Uml"
                );
        }

#warning replace this with GetPropertyName extension method in 1.4
        public string Name(IAssociationEnd associationEnd)
        {
            if (string.IsNullOrEmpty(associationEnd.Name))
            {
                var className = associationEnd.Class.Name;
                if (associationEnd.MaxMultiplicity == "*" || int.Parse(associationEnd.MaxMultiplicity) > 1)
                {
                    return className.EndsWith("y") ? className.Substring(0, className.Length - 1) + "ies" : string.Format("{0}s", className);
                }
                return associationEnd.Class.Name;
            }

            return associationEnd.Name;
        }

        public string CollectionInitializer(IAssociationEnd associatedClass, string memberName)
        {
            if (associatedClass.Multiplicity == Multiplicity.Many)
            {
                return String.Format(" ?? ({0} = new List<{1}>())", memberName, associatedClass.Class.Name + "");
            }
            return string.Empty;
        }

        public string GetReturnType(IOperation operation)
        {
            return operation.ReturnType != null ? Types.Get(operation.ReturnType.Type) : "void";
        }

        public string GetParameterDefinition(IOperation operation)
        {
            return operation.Parameters.Any() ? operation.Parameters.Select(x => Types.Get(x.Type) + " " + x.Name.ToCamelCase()).Aggregate((x, y) => x + ", " + y) : "";
        }

        public string GetAssociationType(IAssociationEnd associationEnd, bool readOnly = false)
        {
            if (associationEnd.Multiplicity == Multiplicity.Many)
            {
                if (readOnly)
                {
                    return "IEnumerable<" + associationEnd.Class.Name + ">";
                }
                else
                {
                    return "ICollection<" + associationEnd.Class.Name + ">";
                }
            }
            return associationEnd.Class.Name ;

        }
    }
}
