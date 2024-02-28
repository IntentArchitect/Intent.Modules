using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.Java.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Java.Weaving.Annotations.Templates.IntentCanAdd;
using Intent.Modules.Java.Weaving.Annotations.Templates.IntentCanRemove;
using Intent.Modules.Java.Weaving.Annotations.Templates.IntentCanUpdate;
using Intent.Modules.Java.Weaving.Annotations.Templates.IntentIgnore;
using Intent.Modules.Java.Weaving.Annotations.Templates.IntentIgnoreBody;
using Intent.Modules.Java.Weaving.Annotations.Templates.IntentManage;
using Intent.Modules.Java.Weaving.Annotations.Templates.IntentManageClass;
using Intent.Modules.Java.Weaving.Annotations.Templates.IntentMerge;
using Intent.Modules.Java.Weaving.Annotations.Templates.ModeEnum;

namespace Intent.Modules.Java
{
    public static class JavaIntentAnnotations
    {
        public static string IntentIgnoreAnnotation<T>(this IntentTemplateBase<T> template)
        {
            return "@" + template.GetTypeName(IntentIgnoreTemplate.TemplateId);
        }

        public static string IntentIgnoreBodyAnnotation<T>(this IntentTemplateBase<T> template)
        {
            return "@" + template.GetTypeName(IntentIgnoreBodyTemplate.TemplateId);
        }

        public static string IntentMergeAnnotation<T>(this IntentTemplateBase<T> template)
        {
            return "@" + template.GetTypeName(IntentMergeTemplate.TemplateId);
        }

        public static string IntentManageAnnotation<T>(this IntentTemplateBase<T> template)
        {
            return "@" + template.GetTypeName(IntentManageTemplate.TemplateId);
        }

        public static string IntentManageClassAnnotation<T>(this IntentTemplateBase<T> template)
        {
            return "@" + template.GetTypeName(IntentManageClassTemplate.TemplateId);
        }

        public static string IntentCanAddAnnotation<T>(this IntentTemplateBase<T> template)
        {
            return "@" + template.GetTypeName(IntentCanAddTemplate.TemplateId);
        }

        public static string IntentCanUpdateAnnotation<T>(this IntentTemplateBase<T> template)
        {
            return "@" + template.GetTypeName(IntentCanUpdateTemplate.TemplateId);
        }

        public static string IntentCanRemoveAnnotation<T>(this IntentTemplateBase<T> template)
        {
            return "@" + template.GetTypeName(IntentCanRemoveTemplate.TemplateId);
        }

        public static string IntentModeIgnore<T>(this IntentTemplateBase<T> template)
        {
            return $"{template.GetTypeName(ModeEnumTemplate.TemplateId)}.Ignore";
        }

        public static string IntentModeMerge<T>(this IntentTemplateBase<T> template)
        {
            return $"{template.GetTypeName(ModeEnumTemplate.TemplateId)}.Merge";
        }

        public static string IntentModeManage<T>(this IntentTemplateBase<T> template)
        {
            return $"{template.GetTypeName(ModeEnumTemplate.TemplateId)}.Manage";
        }

        public static string IntentModeCanAdd<T>(this IntentTemplateBase<T> template)
        {
            return $"{template.GetTypeName(ModeEnumTemplate.TemplateId)}.CanAdd";
        }

        public static string IntentModeCanUpdate<T>(this IntentTemplateBase<T> template)
        {
            return $"{template.GetTypeName(ModeEnumTemplate.TemplateId)}.CanUpdate";
        }

        public static string IntentModeCanRemove<T>(this IntentTemplateBase<T> template)
        {
            return $"{template.GetTypeName(ModeEnumTemplate.TemplateId)}.CanRemove";
        }
    }
}
