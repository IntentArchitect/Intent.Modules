using System;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Common.Templates
{
    public class TemplateDependancy : ITemplateDependancy
    {
        public string TemplateIdOrName { get; }
        //public object MetaDataModel { get; }
        //public string ClassName { get; }

        private Func<ITemplate, bool> _isMatch;

        private TemplateDependancy(string templateIdOrName, Func<ITemplate, bool> isMatch)
        {
            TemplateIdOrName = templateIdOrName;
            _isMatch = isMatch;
            //MetaDataModel = metaDataModel;
            //ClassName = className;
        }

        public static ITemplateDependancy OnTemplate(string templateIdOrName)
        {
            return new TemplateDependancy(templateIdOrName, (t) => true);
        }

        public static ITemplateDependancy OnModel(string templateIdOrName, object metaDataModel)
        {
            return new TemplateDependancy(templateIdOrName, (t) => (t as ITemplateWithModel)?.Model == metaDataModel);
        }

        public static ITemplateDependancy OnClassName(string templateIdOrName, string className)
        {
            return new TemplateDependancy(templateIdOrName, (t) => t.GetMetaData().CustomMetaData.ContainsKey("ClassName") && t.GetMetaData().CustomMetaData["ClassName"] == className);
        }

        public static ITemplateDependancy OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch)
        {
            return new TemplateDependancy(
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
