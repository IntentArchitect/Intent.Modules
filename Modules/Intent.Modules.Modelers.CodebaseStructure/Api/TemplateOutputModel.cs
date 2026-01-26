using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Intent.Configuration;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using DynamicExpressionParser = System.Linq.Dynamic.Core.DynamicExpressionParser;
using ParsingConfig = System.Linq.Dynamic.Core.ParsingConfig;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.CodebaseStructure.Api
{
    [IntentManaged(Mode.Merge)]
    public class TemplateOutputModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder, IOutputTargetTemplate
    {
        private static readonly ParsingConfig ParsingConfig = new();
        public const string SpecializationType = "Template Output";
        public const string SpecializationTypeId = "d421c322-7a51-4094-89fa-e5d8a0a97b27";
        protected readonly IElement _element;
        private object _cacheableRegistrationFilterLock = new();
        private Func<object, bool> _cacheableRegistrationFilter;

        [IntentManaged(Mode.Fully)]
        public TemplateOutputModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(_element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public FolderModel Folder { get; }

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(TemplateOutputModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TemplateOutputModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        #region IOutputTargetTemplate implementation

        public bool IsApplicableToModel<TModel>(TModel model)
        {
            LazyInitializer.EnsureInitialized(ref _cacheableRegistrationFilter, ref _cacheableRegistrationFilterLock, () =>
            {
                if (!this.TryGetTemplateOutputSettings(out var settings) ||
                    string.IsNullOrWhiteSpace(settings.RegistrationFilter()))
                {
                    return _ => true;
                }

                var compiledExpression = DynamicExpressionParser.ParseLambda<TModel, bool>(ParsingConfig, true, settings.RegistrationFilter()).Compile();
                return _cacheableRegistrationFilter = parameter => compiledExpression((TModel)parameter);
            });

            return _cacheableRegistrationFilter(model);
        }

        public bool IsEnabled()
        {
            // We don't use the generated API extension in case stereotype or "Is Enabled" property is not in the metadata:
            return this.GetStereotypeProperty(TemplateOutputModelStereotypeExtensions.TemplateOutputSettings.DefinitionId, "Is Enabled", true);
        }

        [IntentManaged(Mode.Ignore)]
        string IOutputTargetTemplate.Id => _element.Name;

        [IntentManaged(Mode.Ignore)]
        bool IOutputTargetTemplate.TryGetElementId([NotNullWhen(true)] out string id)
        {
            id = _element.Id;
            return true;
        }

        #endregion
    }

    [IntentManaged(Mode.Fully)]
    public static class TemplateOutputModelExtensions
    {

        public static bool IsTemplateOutputModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == TemplateOutputModel.SpecializationTypeId;
        }

        public static TemplateOutputModel AsTemplateOutputModel(this ICanBeReferencedType type)
        {
            return type.IsTemplateOutputModel() ? new TemplateOutputModel((IElement)type) : null;
        }

        /// <summary>
        /// Checks for duplicate Template Output elements in the sequence and throws an exception if any are found.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        /// <exception cref="ElementException"></exception>
        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<TemplateOutputModel> DetectDuplicates(this IEnumerable<TemplateOutputModel> sequence)
        {
            var templateNamesSet = new HashSet<string>();

            foreach (var templateOutputModel in sequence)
            {
                if (!templateNamesSet.Add(templateOutputModel.Name))
                {
                    throw new ElementException(templateOutputModel.InternalElement, $"Duplicate Template Output found at same location.");
                }
                yield return templateOutputModel;
            }
        }
    }
}