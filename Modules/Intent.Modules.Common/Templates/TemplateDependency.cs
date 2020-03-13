using System;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public class TemplateDependency : ITemplateDependency
    {
        public string TemplateIdOrName { get; }
        //public object MetadataModel { get; }
        //public string ClassName { get; }

        private Func<ITemplate, bool> _isMatch;

        private TemplateDependency(string templateIdOrName, Func<ITemplate, bool> isMatch)
        {
            TemplateIdOrName = templateIdOrName;
            _isMatch = isMatch;
            //MetadataModel = metadataModel;
            //ClassName = className;
        }

        public static ITemplateDependency OnTemplate(string templateIdOrName)
        {
            return new TemplateDependency(templateIdOrName, (t) => true);
        }

        public static ITemplateDependency OnTemplate(ITemplate template)
        {
            return new TemplateDependency(template.Id, (t) => t == template);
        }

        public static ITemplateDependency OnModel(string templateIdOrName, object metadataModel)
        {
            return new TemplateDependency(templateIdOrName, (t) => t is ITemplateWithModel model && model.Model.Equals(metadataModel));
        }

        public static ITemplateDependency OnClassName(string templateIdOrName, string className)
        {
            return new TemplateDependency(templateIdOrName, (t) => t.GetMetadata().CustomMetadata.ContainsKey("ClassName") && t.GetMetadata().CustomMetadata["ClassName"] == className);
        }

        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch)
        {
            return new TemplateDependency(
                templateIdOrName,
                (t) =>
                {
                    var model = (t as ITemplateWithModel)?.Model;
                    if (model is TModel)
                    {
                        return isMatch((TModel)model);
                    }
                    return false;
                });
        }
        
        public bool IsMatch(ITemplate t)
        {
            return _isMatch(t);
        }
    }
}
