# Template Registrations

For Intent Architect to know about your template you need to register it, this is done through a registration class. This class doesn't only tell Intent Architect to register the template, but how to construct instances of it and bind that instance to data.

>[!NOTE]
>A template registration is a factory, i.e. it will create a template instance for data model returned from the `GetModels` method.

There are 3 base classes you can inherit from to register your template.
- **NoModelTemplateRegistrationBase**, templates not bound to data
- **ModelTemplateRegistrationBase**, template bound to data
- **ListModelTemplateRegistrationBase**, template bound to a list of data

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