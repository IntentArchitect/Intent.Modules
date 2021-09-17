using System.Collections.Generic;
using System.Data;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using JetBrains.Annotations;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelPartial", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class DTOModel
    {
        public IElementApplication Application => _element.Application;

        /// <summary>
        /// Returns fields contained in this <see cref="DTOModel"/> instance, as well as those in its <see cref="BaseType"/> hierarchy.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DTOFieldModel> GetAllFieldsInHierarchy()
        {
            var fields = new List<DTOFieldModel>(Fields);
            if (BaseType != null)
            {
                fields.AddRange(BaseType.GetAllFieldsInHierarchy());
            }

            return fields;
        }

        /// <summary>
        /// Returns the base <see cref="DTOModel"/> for this entity, if specified.
        /// </summary>
        [CanBeNull]
        public DTOModel BaseType => TypeReference.Element?.ToDTOModel();
    }
}