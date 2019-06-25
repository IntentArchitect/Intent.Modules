using Intent.Modules.VisualStudio.Projects.Parsing;
using Intent.Engine;
using Intent.Templates;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution
{
    // NB! Solution Project Type GUIDS: http://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
    partial class VisualStudio2015SolutionTemplate : ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.VisualStudio2015Solution";
        private readonly IApplication _application;
        private readonly IFileMetadata _fileMetadata;

        public VisualStudio2015SolutionTemplate(IApplication application, SolutionFile existingSolution)
        {
            _application = application;
            _fileMetadata = CreateMetadata();
            Projects = _application.Projects;
            ExistingSolution = existingSolution;
            SolutionFolders = Projects.Where(x => x.SolutionFolder() != null && x.Folder.Id != application.Id)
                .GroupBy(x => x.SolutionFolder())
                .ToDictionary(x => x.Key, x => x.ToList())
                .Select(x => new SolutionFolder(x.Key, x.Value))
                .ToList();

            foreach (var solutionFolder in SolutionFolders)
            {
                UpdateSolutionFolder(solutionFolder);
            }
        }

        public string Id { get; } = Identifier;
        public IEnumerable<IProject> Projects { get; }
        public SolutionFile ExistingSolution { get; }
        public IList<SolutionFolder> SolutionFolders { get; }

        public void UpdateSolutionFolder(SolutionFolder solutionFolder)
        {
            if (ExistingSolution == null)
                return;

            var existingProject = ExistingSolution.ProjectsByGuid.FirstOrDefault(x => x.Value.ProjectName == solutionFolder.FolderName && x.Value.ProjectType == SolutionProjectType.SolutionFolder);
            if (existingProject.Key != null)
            {
                solutionFolder.Id = Guid.Parse(existingProject.Key);
            }
        }


        public string RunTemplate()
        {
            string targetFile = GetMetadata().GetFullLocationPathWithFileName();
            if (!File.Exists(targetFile))
            {
                return TransformText();
            }
            else
            {
                var existingFolders = ExistingSolution.ProjectsByGuid.Values.Where(p => p.ProjectType == SolutionProjectType.SolutionFolder).Select(x => x.ProjectName).ToList();
                var existingProjects = ExistingSolution.ProjectsByGuid.Values.Where(p => p.ProjectType != SolutionProjectType.SolutionFolder).Select(x => x.ProjectName).ToList();

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
                                parser.Insert( $"\t\t{{{project.Id.ToString().ToUpper()}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\r\n");
                                parser.Insert( $"\t\t{{{project.Id.ToString().ToUpper()}}}.Debug|Any CPU.Build.0 = Debug|Any CPU\r\n");
                                parser.Insert( $"\t\t{{{project.Id.ToString().ToUpper()}}}.Release|Any CPU.ActiveCfg = Release|Any CPU\r\n");
                                parser.Insert( $"\t\t{{{project.Id.ToString().ToUpper()}}}.Release|Any CPU.Build.0 = Release|Any CPU\r\n");
                            }
                            break;
                        case "GlobalSection(NestedProjects)":
                            parser.Seek("EndGlobalSection");
                            parser.Consume("\r\n");
                            foreach (var projectName in missingProjects)
                            {
                                var project = Projects.First(f => f.Name == projectName);
                                var solutionFolder = SolutionFolders.FirstOrDefault(f => f.FolderName == project.SolutionFolder());
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
            return _fileMetadata;
        }

        private IFileMetadata CreateMetadata()
        {            
            return new SolutionFileMetadata(
                outputType: "VisualStudio2015Solution", 
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledWeave,
                fileName: _application.ApplicationName,
                fileLocation: _application.RootLocation);
        }
    }

    public class SolutionFolder
    {
        public Guid Id { get; set;  }
        public string FolderName { get; }
        public List<IProject> AssociatedProjects { get; }

        public SolutionFolder(string folderName, List<IProject> associatedProjects)
        {
            Id = Guid.NewGuid();
            FolderName = folderName;
            AssociatedProjects = associatedProjects;
        }

    }
}
