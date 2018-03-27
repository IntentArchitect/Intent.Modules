# Template Registrations

For Intent Architect to know about your template you need to register it, this is done through a registration class. This class doesn't only tell Intent Architect to register the template, but how to construct instances of it and bind that instance to data.

>[!NOTE]
>A template registration is a factory, i.e. it will create a template instance for data model returned from the `GetModels` method.

Fundamentally, Intent Architect uses the `IProjectTemplateRegistration` interface to register Templates with the Software Factory. A few base classes have been created for convenience to make the common use cases easier to implement:
- **NoModelTemplateRegistrationBase** - For cases when the template is not dependent on any metadata (e.g. outputting a static file).
- **ModelTemplateRegistrationBase** - For cases when there is a _one-to-one_ relationship between each metadata model and output file. This is the most common use case (e.g. a `ServiceImplementation` output for each `Service` model from the metadata).
- **ListModelTemplateRegistrationBase** - For cases when the template requires a collection of metadata models (e.g. a Dependency Injection `Registrations` class that registers up each of the services from the metadata)

Here is an example of a template registration.

```csharp

    [Description("Intent Entity Base Template")]
    public class Registrations : ModelTemplateRegistrationBase<IClass>
    {
        private IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        // This should be the same identifier as that of the template.
        public override string TemplateId
        {
            get
            {
                return DomainEntityTemplate.Identifier;
            }
        }

        //Factory method for creating template instances
        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new DomainEntityTemplate(model, project);
        }

        //The set of Data to create template instances for
        public override IEnumerable<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application);
        }
    }

```