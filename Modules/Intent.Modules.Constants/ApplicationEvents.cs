namespace Intent.Modules.Constants
{
    public static class SoftwareFactoryEvents
    {
        public const string FileAdded = "Intent.SoftwareFactory.AddProjectItemEvent";
        public const string FileRemoved = "Intent.SoftwareFactory.RemoveProjectItemEvent";
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
        public const string AppStart_InitializationRequired = "AppStart.InitializationRequired";
        public const string Typescript_TypingsRequired = "Typescript.Typings";
    }

    public static class InitializationRequiredEvent
    {
        public const string EventId = "InitializationRequiredEvent";
        public const string UsingsKey = "Usings";
        public const string CallKey = "Call";
        public const string MethodKey = "Method";
        public const string PriorityKey = "Priority";
        public const string TemplateDependencyIdKey = "TemplateDependencyId";
    }

    public static class ServiceConfigurationRequiredEvent
    {
        public const string EventId = "ServiceConfigurationRequiredEvent";
        public const string UsingsKey = "Usings";
        public const string CallKey = "Call";
        public const string MethodKey = "Method";
        public const string PriorityKey = "Priority";
    }

    public static class ContainerRegistrationEvent
    {
        public const string EventId = "Container.RegistrationRequired";
        public const string UsingsKey = "Usings";
        public const string GroupKey = "Group";
        public const string InterfaceTypeKey = "InterfaceType";
        public const string ConcreteTypeKey = "ConcreteType";
        public const string InterfaceTypeTemplateIdKey = "InterfaceTypeTemplateId";
        public const string ConcreteTypeTemplateIdKey = "ConcreteTypeTemplateId";
        public const string LifetimeKey = "Lifetime";

        public const string TransientLifetime = "Transient";
        public const string SingletonLifetime = "Singleton";
        public const string PerServiceCallLifetime = "PerServiceCall";
    }

    public static class ContainerRegistrationForDbContextEvent
    {
        public const string EventId = "Container.RegistrationRequired.DbContext";
        public const string UsingsKey = "Usings";
        public const string ConcreteTypeKey = "ConcreteType";
        public const string ConcreteTypeTemplateIdKey = "ConcreteTypeTemplateId";
        public const string OptionsKey = "Options";
        public const string NugetDependency = "Microsoft.EntityFrameworkCore.SqlServer";
        public const string NugetDependencyVersion = "2.1.1";
    }

    public static class EntityFrameworkEvents
    {
        public const string SeedDataRequiredEvent = "EntityFrameworkEvents.SeedDataRequiredEvent";
        public const string SeedDataRequiredEventKey = "EntityFrameworkEvents.SeedDataRequiredEvent.Key";
    }

    public static class LaunchProfileRegistrationEvent
    {
        public const string EventId = nameof(LaunchProfileRegistrationEvent);
        public const string ProfileNameKey = "profileName";
        public const string CommandNameKey = "commandName";
        public const string LaunchBrowserKey = "launchBrowser";
        public const string LaunchUrlKey = "launchUrl";
        public const string ApplicationUrl = "applicationUrl";
    }
}
