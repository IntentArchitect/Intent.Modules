using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using SearchOption = System.IO.SearchOption;

namespace Intent.Modules.Common.Templates.FileContent
{
    /// <summary>
    /// Inherit from this class to generate static content for each file in a folder and it's sub-folders
    /// in your module.
    /// </summary>
    public abstract class ModuleContentFilesTemplateRegistrations : TemplateRegistrationBase
    {
        /// <summary>
        /// Allows to sub-divide the "content" folder, in the event of other Template Registrations
        /// of type <see cref="ModuleContentFilesTemplateRegistrations"/>
        /// having to scan for files, by specifying the sub-folder path where the relevant content is located.
        /// Specify only the sub-folder name (or sub-path, using '/' as path delimiter).
        /// </summary>
        /// <remarks>
        /// By default it will scan all files inside the "content" folder.
        /// </remarks>
        /// <example>
        /// In your project's root folder you have the "content" folder. Inside of that folder there
        /// is "SolutionItems". "ContentSubFolder" should then just be "SolutionItems".
        /// </example>
        public virtual string ContentSubFolder => "/";

        /// <summary>
        /// Will look for entries strings with <code>&lt;#= {element.Key} #&gt;</code> and replace them with <code>{element.Value}</code>.
        /// </summary>
        public virtual IReadOnlyDictionary<string, string> Replacements => null;

        /// <inheritdoc />
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            var location = Path.GetFullPath(Path.Join(Path.GetDirectoryName(GetType().Assembly.Location), "../content", ContentSubFolder.NormalizePath()));
            var files = Directory
                .EnumerateFiles(location, "*.*", SearchOption.AllDirectories)
                .Select(x => new
                {
                    FullPath = x,
                    RelativePath = x.Substring(location.Length)
                })
                .ToArray();

            foreach (var file in files)
            {
                registry.RegisterTemplate(TemplateId, o => new FileContentTemplate(
                    sourcePath: file.FullPath,
                    relativeOutputPath: file.RelativePath,
                    templateId: TemplateId, outputTarget: o,
                    replacements: Replacements));
            }
        }
    }
}