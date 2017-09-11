using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Packages.Constants
{
    public static class SoftwareFactoryEvents
    {
        public const string AddProjectItemEvent = "Intent.SoftwareFactory.AddProjectItemEvent";
        public const string RemoveProjectItemEvent = "Intent.SoftwareFactory.RemoveProjectItemEvent";
        public const string AddTargetEvent = "Intent.SoftwareFactory.AddTargetEvent";
        public const string AddTaskEvent = "Intent.SoftwareFactory.AddTaskEvent";
        public const string ChangeProjectItemTypeEvent = "Intent.SoftwareFactory.ChangeProjectItemTypeEvent";
        public const string DeleteFileCommand = "Intent.SoftwareFactory.DeleteFileCommand";
        public const string OverwriteFileCommand = "Intent.SoftwareFactory.OverwriteFileCommand";
        public const string CreateFileCommand = "Intent.SoftwareFactory.CreateFileCommand";
        public const string CodeWeaveCodeLossEvent = "Intent.SoftwareFactory.CodeWeaveCodeLossEvent";
    }

    public static class ApplicationEvents
    {
        public const string IndexHtml_JsFileAvailable = "IndexHtml.JsFileAvailable";
        public const string AngularJs_ModuleRegistered = "AngularJs.ModuleRegistered";
        public const string AngularJs_ConfigurationRequired = "AngularJs.ConfigurationRequired";
        public const string Config_AppSetting = "Config.AppSetting";
        public const string Config_ConnectionString = "Config.ConnectionString";
        public const string Container_RegistrationRequired = "Container.RegistrationRequired";
        public const string AppStart_InitializationRequired = "AppStart.InitializationRequired";
        public const string Typescript_TypingsRequired = "Typescript.Typings";
    }
}


