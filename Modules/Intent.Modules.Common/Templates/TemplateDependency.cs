using System;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public class TemplateDependency : ITemplateDependency
    {
        public string TemplateId { get; }
        public object Context { get; }
        //public string ClassName { get; }

        private Func<ITemplate, bool> _isMatch;

        private TemplateDependency(string templateId, Func<ITemplate, bool> isMatch, object context)
        {
            TemplateId = templateId;
            Context = context;
            _isMatch = isMatch;
            //MetadataModel = metadataModel;
            //ClassName = className;
        }

        public override string ToString()
        {
            return $"Template Id: {TemplateId}{(Context != null ? " - " + Context.ToString() : string.Empty)}";
        }

        public static ITemplateDependency OnTemplate(string templateIdOrName)
        {
            return new TemplateDependency(templateIdOrName, (t) => true, "Lookup single template");
        }

        public static ITemplateDependency OnTemplate(ITemplate template)
        {
            return new TemplateDependency(template.Id, (t) => t == template, template);
        }

        public static ITemplateDependency OnModel(string templateIdOrName, object metadataModel)
        {
            return new TemplateDependency(templateIdOrName, (t) => t is ITemplateWithModel model && model.Model.Equals(metadataModel), metadataModel);
        }

        public static ITemplateDependency OnModel(string templateIdOrName, IMetadataModel metadataModel)
        {
            return new TemplateDependency(templateIdOrName, (t) => t is ITemplateWithModel model && (model.Model as IMetadataModel)?.Id == metadataModel.Id, metadataModel);
        }


        //public static ITemplateDependency OnClassName(string templateIdOrName, string className)
        //{
        //    return new TemplateDependency(templateIdOrName, (t) => t.GetMetadata().CustomMetadata.ContainsKey("ClassName") && t.GetMetadata().CustomMetadata["ClassName"] == className);
        //}

        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch)
        {
            return TemplateDependency.OnModel(templateIdOrName, isMatch, null);
        }

        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch, object context)
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
                }, context);
        }


        public bool IsMatch(ITemplate t)
        {
            return _isMatch(t);
        }
    }
}
