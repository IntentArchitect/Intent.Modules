using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using Microsoft.Extensions.FileSystemGlobbing;
using SearchOption = System.IO.SearchOption;

namespace Intent.Modules.Common.Templates.StaticContent
{
    /// <summary>
    /// Inherit from this class to generate static content for each file in a folder and it's sub-folders
    /// in your module.
    /// </summary>
    public abstract class StaticContentTemplateRegistration : TemplateRegistrationBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="StaticContentTemplateRegistration"/>.
        /// </summary>
        protected StaticContentTemplateRegistration(string templateId)
        {
            TemplateId = templateId;
        }

        /// <inheritdoc />
        public sealed override string TemplateId { get; }

        /// <summary>
        /// <para>
        /// Location of source in the "content" folder.
        /// </para>
        /// <para>
        /// Allows to sub-divide the "content" folder, in the event of other Template Registrations
        /// of type <see cref="StaticContentTemplateRegistration"/>
        /// having to scan for files, by specifying the sub-folder path where the relevant content is located.
        /// Specify only the sub-folder name (or sub-path, using '/' as path delimiter).
        /// </para>
        /// </summary>
        /// <remarks>
        /// By default it will scan all files inside the "content" folder.
        /// </remarks>
        /// <example>
        /// In your project's root folder you have the "content" folder. Inside of that folder there
        /// is "SolutionItems". "ContentSubFolder" should then just be "SolutionItems".
        /// </example>
        public virtual string ContentSubFolder => "/";

        public virtual string[] BinaryFileGlobbingPatterns => new string[0];


        /// <summary>
        /// Will look for keys in this format <code>&lt;#= {element.Key} #&gt;</code> and substitute them with <code>{element.Value}</code>.
        /// </summary>
        /// <param name="outputTarget">Leverage the output target context to know where the static content gets written to as part of the keyword substitution process</param>
        public virtual IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => null;

        /// <inheritdoc />
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            var location = Path.GetFullPath(Path.Join(Path.GetDirectoryName(GetType().Assembly.Location), "../content", ContentSubFolder.NormalizePath()));
            var allFiles = Directory
                .EnumerateFiles(location, "*.*", SearchOption.AllDirectories);

            var binaries = GetBinaryFiles(location, allFiles);
            var textFiles = allFiles.Except(binaries).Select(x => new
            {
                FullPath = x,
                RelativePath = x.Substring(location.Length).Trim(Path.DirectorySeparatorChar)
            }).ToArray();
            var binaryFiles = binaries.Select(x => new
            {
                FullPath = x,
                RelativePath = x.Substring(location.Length).Trim(Path.DirectorySeparatorChar)
            }).ToArray();

            foreach (var file in textFiles)
            {
                registry.RegisterTemplate(TemplateId, outputTarget => CreateTemplate(outputTarget, file.FullPath, file.RelativePath, DefaultOverrideBehaviour));
            }

            foreach (var file in binaryFiles)
            {
                registry.RegisterTemplate(TemplateId, outputTarget => CreateBinaryTemplate(outputTarget, file.FullPath, file.RelativePath, DefaultOverrideBehaviour));
            }
        }

        private IEnumerable<string> GetBinaryFiles(string location, IEnumerable<string> allfiles)
        {
            if (BinaryFileGlobbingPatterns.Length > 0)
            {
                var matcher = new Matcher();
                matcher.AddIncludePatterns(BinaryFileGlobbingPatterns);
                return matcher.GetResultsInFullPath(location);
            }
            return new List<string>();
        }

        /// <summary>
        /// Obsolete. Use <see cref="CreateTemplate(Intent.Engine.IOutputTarget,string,string,OverwriteBehaviour)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        protected virtual ITemplate CreateTemplate(IOutputTarget outputTarget, string fileFullPath, string fileRelativePath)
        {
            return CreateTemplate(outputTarget, fileFullPath, fileRelativePath, OverwriteBehaviour.OverwriteDisabled);
        }

        /// <summary>
        /// Factory method for creating the binary template instance.
        /// </summary>
        /// <remarks>
        /// The default implementation creates an instance of <see cref="StaticContentTemplate"/>.
        /// </remarks>
        /// <param name="outputTarget">The <see cref="IOutputTarget"/> for the template registration.</param>
        /// <param name="fileFullPath">The full file path of the source file, used to read the source file content.</param>
        /// <param name="fileRelativePath">The relative path of the file to the "root" content, used to determine where on the file system to output the file.</param>
        /// <param name="defaultOverwriteBehaviour">The incoming value is the value of <see cref="OverwriteBehaviour"/>.</param>
        protected virtual ITemplate CreateBinaryTemplate(IOutputTarget outputTarget, string fileFullPath, string fileRelativePath, OverwriteBehaviour defaultOverwriteBehaviour)
        {
            return new StaticBinaryContentTemplate(
                sourcePath: fileFullPath,
                relativeOutputPath: fileRelativePath,
                templateId: TemplateId,
                outputTarget: outputTarget,
                replacements: Replacements(outputTarget),
                overwriteBehaviour: defaultOverwriteBehaviour);
        }


        /// <summary>
        /// Factory method for creating the template instance.
        /// </summary>
        /// <remarks>
        /// The default implementation creates an instance of <see cref="StaticContentTemplate"/>.
        /// </remarks>
        /// <param name="outputTarget">The <see cref="IOutputTarget"/> for the template registration.</param>
        /// <param name="fileFullPath">The full file path of the source file, used to read the source file content.</param>
        /// <param name="fileRelativePath">The relative path of the file to the "root" content, used to determine where on the file system to output the file.</param>
        /// <param name="defaultOverwriteBehaviour">The incoming value is the value of <see cref="OverwriteBehaviour"/>.</param>
        protected virtual ITemplate CreateTemplate(IOutputTarget outputTarget, string fileFullPath, string fileRelativePath, OverwriteBehaviour defaultOverwriteBehaviour)
        {
            return new StaticContentTemplate(
                sourcePath: fileFullPath,
                relativeOutputPath: fileRelativePath,
                templateId: TemplateId,
                outputTarget: outputTarget,
                replacements: Replacements(outputTarget),
                overwriteBehaviour: defaultOverwriteBehaviour);
        }

        /// <summary>
        /// Change this value to change the default <see cref="OverwriteBehaviour"/> of templates.
        /// </summary>
        protected virtual OverwriteBehaviour DefaultOverrideBehaviour { get; } = OverwriteBehaviour.Always;
    }
}