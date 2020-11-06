using Intent.Modules.VisualStudio.Projects.Parsing;
using Intent.Engine;
using Intent.Templates;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Plugins;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution
{
    public class VisualStudio2015SolutionTemplateModel
    {
        public VisualStudio2015SolutionTemplateModel(IApplication application)
        {
            Application = application;
        }

        public IApplication Application { get; }
    }

    // NB! Solution Project Type GUIDS: http://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
    partial class VisualStudio2015SolutionTemplate : ITemplate, IConfigurableTemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.VisualStudio2015Solution";
        private IFileMetadata _fileMetadata;

        public VisualStudio2015SolutionTemplate(IApplication application, VisualStudioSolutionModel model, IEnumerable<IVisualStudioProject> projects)
        {
            Application = application;
            Model = model;
            //_fileMetadata = CreateMetadata();
            BindingContext = new TemplateBindingContext(new VisualStudio2015SolutionTemplateModel(Application));
            Projects = projects;
            //ExistingSolution = existingSolution;
            SolutionFolders = Projects.Where(x => x.ParentFolder != null && x.ParentFolder.Id != Application.Id)
                .GroupBy(x => x.ParentFolder.Name)
                .ToDictionary(x => x.Key, x => x.ToList())
                .Select(x => new SolutionFolder(x.Key, x.Value))
                .ToList();
        }

        public string Id { get; } = Identifier;
        public IApplication Application { get; }
        public VisualStudioSolutionModel Model { get; }

        public IEnumerable<IVisualStudioProject> Projects { get; }
        //public SolutionFile ExistingSolution { get; }
        public IList<SolutionFolder> SolutionFolders { get; }

        public string RunTemplate()
        {
            string targetFile = GetMetadata().GetFullLocationPathWithFileName();

            if (!File.Exists(targetFile))
            {
                return TransformText();
            }
            else
            {
                SolutionFile existingSolution = SolutionFile.Parse(targetFile);

                foreach (var solutionFolder in SolutionFolders)
                {
                    var existingProject = existingSolution.ProjectsByGuid.FirstOrDefault(x => x.Value.ProjectName == solutionFolder.FolderName && x.Value.ProjectType == SolutionProjectType.SolutionFolder);
                    if (existingProject.Key != null)
                    {
                        solutionFolder.Id = Guid.Parse(existingProject.Key);
                    }
                }

                var existingFolders = existingSolution.ProjectsByGuid.Values.Where(p => p.ProjectType == SolutionProjectType.SolutionFolder).Select(x => x.ProjectName).ToList();
                var existingProjects = existingSolution.ProjectsByGuid.Values.Where(p => p.ProjectType != SolutionProjectType.SolutionFolder).Select(x => x.ProjectName).ToList();

                var missingSolutionFolders = SolutionFolders.Select(f => f.FolderName).Except(existingFolders);
                var missingProjects = Projects.Select(f => f.Name).Except(existingProjects);

                var fileContent = File.ReadAllText(targetFile);

                //Missing the condition : If solution folder has changed
                if (missingSolutionFolders.Count() == 0 && missingProjects.Count() == 0)
                {
                    return fileContent;
                }

                var parser = new EditableParser(new StringBuilder(fileContent));

                //Check all the relevant sections exist in the file, as they are optional
                var places = parser.FindAndBookmark("Global", "GlobalSection(ProjectConfigurationPlatforms)", "GlobalSection(NestedProjects)", "EndGlobal").ToDictionary(x => x.Token);

                if (!places.ContainsKey("GlobalSection(NestedProjects)"))
                {
                    parser.Insert(places["EndGlobal"].Position + 2, "	GlobalSection(NestedProjects) = postSolution\r\n	EndGlobalSection\r\n", true);
                }
                if (!places.ContainsKey("GlobalSection(ProjectConfigurationPlatforms)"))
                {
                    parser.Insert(places["EndGlobal"].Position + 2, "	GlobalSection(ProjectConfigurationPlatforms) = postSolution\r\n	EndGlobalSection\r\n", true);
                }

                parser.ChangePos(places["Global"].Position);
                parser.Consume("\r\n");

                //Add Missing Projects
                foreach (var projectName in missingProjects)
                {
                    var project = Projects.Where(f => f.Name == projectName).First();
                    parser.Insert($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{project.Name}\", \"{project.Name}\\{project.Name}.csproj\", \"{{{project.Id.ToString().ToUpper()}}}\"\r\nEndProject\r\n");
                }

                //Add Solution Folders
                foreach (var solutionFolderName in missingSolutionFolders)
                {
                    var solutionFolder = SolutionFolders.Where(f => f.FolderName == solutionFolderName).First();
                    parser.Insert($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"{solutionFolder.FolderName}\", \"{solutionFolder.FolderName}\", \"{{{solutionFolder.Id.ToString().ToUpper()}}}\"\r\nEndProject\r\n");
                }

                while (parser.SeekStartsWith("GlobalSection"))
                {
                    switch (parser.GetToken())
                    {
                        case "GlobalSection(ProjectConfigurationPlatforms)":
                            parser.Seek("EndGlobalSection");
                            parser.Consume("\r\n");
                            foreach (var projectName in missingProjects)
                            {
                                var project = Projects.First(f => f.Name == projectName);
                                parser.Insert($"\t\t{{{project.Id.ToString().ToUpper()}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\r\n");
                                parser.Insert($"\t\t{{{project.Id.ToString().ToUpper()}}}.Debug|Any CPU.Build.0 = Debug|Any CPU\r\n");
                                parser.Insert($"\t\t{{{project.Id.ToString().ToUpper()}}}.Release|Any CPU.ActiveCfg = Release|Any CPU\r\n");
                                parser.Insert($"\t\t{{{project.Id.ToString().ToUpper()}}}.Release|Any CPU.Build.0 = Release|Any CPU\r\n");
                            }
                            break;
                        case "GlobalSection(NestedProjects)":
                            parser.Seek("EndGlobalSection");
                            parser.Consume("\r\n");
                            foreach (var projectName in missingProjects)
                            {
                                var project = Projects.First(f => f.Name == projectName);
                                var solutionFolder = SolutionFolders.FirstOrDefault(f => f.FolderName == project.ParentFolder?.Name);
                                if (solutionFolder != null)
                                {
                                    parser.Insert($"\t\t{{{project.Id.ToString().ToUpper()}}} = {{{solutionFolder.Id.ToString().ToUpper()}}}\r\n");
                                }
                            }
                            break;
                    }
                }
                return parser.Content();
            }
        }

        public IFileMetadata GetMetadata()
        {
            if (_fileMetadata == null)
            {
                throw new Exception("File Metadata must be specified.");
            }
            return _fileMetadata;
        }

        public void ConfigureFileMetadata(IFileMetadata fileMetadata)
        {
            _fileMetadata = fileMetadata;
        }

        public ITemplateFileConfig GetTemplateFileConfig()
        {
            return new SolutionFileMetadata(
                outputType: "VisualStudio2015Solution",
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledWeave,
                fileName: $"{Model.Name}",
                fileLocation: Application.RootLocation);
        }

        public ITemplateBindingContext BindingContext { get; }
    }

    public class SolutionFolder
    {
        public Guid Id { get; set; }
        public string FolderName { get; }
        public List<IVisualStudioProject> AssociatedProjects { get; }

        public SolutionFolder(string folderName, List<IVisualStudioProject> associatedProjects)
        {
            Id = Guid.NewGuid();
            FolderName = folderName;
            AssociatedProjects = associatedProjects;
        }

    }
}
