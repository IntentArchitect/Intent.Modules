using Intent.Packages.AngularJs.Shell.Templates.AngularApp;
using Intent.Packages.AngularJs.Shell.Templates.AngularCommon;
using Intent.Packages.AngularJs.Shell.Templates.AngularConfigJs;
using Intent.Packages.AngularJs.Shell.Templates.AngularShellState;
using Intent.Packages.AngularJs.Shell.Templates.AngularShellView;
using Intent.Packages.AngularJs.Shell.Templates.AngularShellViewModel;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Modules.Templates.Website;
using Intent.SoftwareFactory.Registrations;

namespace Intent.AngularJs.Shell
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterTemplate(AngularAppTemplate.Identifier, project => new AngularAppTemplate(project, application.EventDispatcher));
            RegisterTemplate(AngularCommonTemplate.Identifier, project => new AngularCommonTemplate(project));
            RegisterTemplate(AngularConfigJsTemplate.Identifier, project => new AngularConfigJsTemplate(project, application.EventDispatcher));
            RegisterTemplate(IndexHtmlFileTemplate.Identifier, project => new IndexHtmlFileTemplate(project, application.EventDispatcher));
            RegisterTemplate(AngularShellStateTemplate.Identifier, project => new AngularShellStateTemplate(project));
            RegisterTemplate(AngularShellViewModelTemplate.Identifier, project => new AngularShellViewModelTemplate(project));
            RegisterTemplate(AngularShellViewTemplate.Identifier, project => new AngularShellViewTemplate(project));
        }
    }
}
