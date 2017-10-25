using Intent.Modules.Common.Plugins;
using Intent.Modules.OutputManager.TagWeaver.TokenParsers;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Plugins;
using Intent.SoftwareFactory.Plugins.FactoryExtensions;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intent.Modules.OutputManager.TagWeaver
{
    public class TagWeavingOMP : FactoryExtensionBase, ITransformOutput
    {
        private readonly ISoftwareFactoryEventDispatcher _eventDispatcher;
        private Dictionary<string, WeaveBehaviour> _codeGenTypeMap;
        private List<string> _cStyleFileExtensions = new List<string>();
        private List<string> _xmlFileExtensions = new List<string>();

        public const string KEEP_CODE_IN_TAGS = "Keep Code In Tags";
        public const string GENERATE_CODE_IN_TAGS = "Generate Code To Tags";

        public const string C_STYLE_PARSER_EXTENSIONS = "C Style Parser Extensions";
        public const string XML_STYLE_PARSER_EXTENSIONS = "Xml Style Extensions";

        public override string Id
        {
            get
            {
                return "Intent.OutputTransformer.TagWeaving";
            }
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            settings.SetIfSupplied(KEEP_CODE_IN_TAGS, (x) => AddMappings(x, WeaveBehaviour.KeepCodeInTags));
            settings.SetIfSupplied(GENERATE_CODE_IN_TAGS, (x) => AddMappings(x, WeaveBehaviour.GenerateCodeToTags));
            settings.SetIfSupplied(C_STYLE_PARSER_EXTENSIONS, (x) => AddExtensions(_cStyleFileExtensions, x));
            settings.SetIfSupplied(XML_STYLE_PARSER_EXTENSIONS, (x) => AddExtensions(_xmlFileExtensions, x));
        }

        private void AddExtensions(List<string> addTo, string setting)
        {
            var normalized = setting.Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLower());
            addTo.Clear();
            addTo.AddRange(normalized); 
        }

        private void AddMappings(string codeTypes, WeaveBehaviour behaviour)
        {
            var types = codeTypes.Split(',');
            foreach (var x in types)
            {
                var s = x.Trim();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    _codeGenTypeMap[s] = behaviour;
                }
            }
        }

        public TagWeavingOMP(ISoftwareFactoryEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _codeGenTypeMap = new Dictionary<string, WeaveBehaviour>();

        }

        public void Transform(IOutputFile output)
        {
            WeaveBehaviour behaviour;
            if (!_codeGenTypeMap.TryGetValue(output.FileMetaData.CodeGenType, out behaviour))
            {
                return;
            }

            switch (behaviour)
            {
                case WeaveBehaviour.KeepCodeInTags:
                    {
                        string targetFile = output.FileMetaData.GetFullLocationPathWithFileName();
                        if (File.Exists(targetFile))
                        {
                            var fileWeaver = new FileWeaver(_eventDispatcher, output.Project, output.FileMetaData);
                            output.ChangeContent(fileWeaver.Keep(GetTokenParser(output.FileMetaData.FileExtension), output.Content, File.ReadAllText(targetFile)));
                        }
                    }
                    break;
                case WeaveBehaviour.GenerateCodeToTags:
                    {
                        string targetFile = output.FileMetaData.GetFullLocationPathWithFileName();
                        if (File.Exists(targetFile))
                        {
                            var fileWeaver = new FileWeaver(_eventDispatcher, output.Project, output.FileMetaData);
                            output.ChangeContent(fileWeaver.Embed(GetTokenParser(output.FileMetaData.FileExtension), output.Content, File.ReadAllText(targetFile)));
                        }
                    }
                    break;
            }
        }

        private TokenParser GetTokenParser(string fileExtension)
        {
            fileExtension = fileExtension.ToLower();
            if (_cStyleFileExtensions.Contains(fileExtension))
            {
                return new CSCommentCodeGenTokenParser();
            }
            if (_xmlFileExtensions.Contains(fileExtension))
            {
                return new XmlCommentCodeGenTokenParser();
            }

            return new XmlCommentCodeGenTokenParser();
        }

    }
}

