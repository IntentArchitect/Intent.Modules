using Intent.SoftwareFactory.Engine;
using System;
using System.Collections.Generic;
using System.IO;

namespace Intent.Modules.VisualStudio.Projects.Sync.Events
{
    public class AddProjectItemEvent 
    {
        public IProject Project { get; }
        public string RelativeFileName { get; }
        public string ItemType { get; }
        public IList<KeyValuePair<string, string>> MetaData { get; } = new List<KeyValuePair<string, string>>();

        public AddProjectItemEvent(IProject project, string relativeFileName, string itemType = null)
        {
            Project = project;
            ItemType = itemType;
            if (ItemType == null)
            {
                ItemType = GetProjectItemType(relativeFileName);
            }
            RelativeFileName = relativeFileName;

            if (Path.GetExtension(relativeFileName).Equals(".config", StringComparison.InvariantCultureIgnoreCase))
            {
                MetaData.Add(new KeyValuePair<string, string>("CopyToOutputDirectory", "PreserveNewest"));
            }
        }

        public AddProjectItemEvent Generated(string codeGenType)
        {
            MetaData.Add(new KeyValuePair<string, string>("IntentGenerated", Project.Application.ApplicationName));
            MetaData.Add(new KeyValuePair<string, string>("IntentGenType", codeGenType));
            return this;
        }

        public AddProjectItemEvent DependentUpon(string relativeFileName)
        {
            MetaData.Add(new KeyValuePair<string, string>("DependentUpon", relativeFileName));
            return this;
        }

        public static string GetProjectItemType(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName)
                .Substring(1); //remove the '.'
            switch (fileExtension)
            {
                //case "ts":
                //    return "TypeScriptCompile";
                case "cs":
                    return "Compile";
                default:
                    return "Content";
            }
        }
    }
}
