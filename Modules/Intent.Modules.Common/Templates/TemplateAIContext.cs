using System.Collections.Generic;
using Intent.Templates;

namespace Intent.Modules.Common.Templates;

public class TemplateAIContext
{
    public string Role { get; set; }
    public string UsageGuidelines { get; set; }
    public string Instructions { get; set; }
    //public IList<IRelatedFile> RelatedFiles { get; set; }
    public IDictionary<string, object> AdditionalMetadata { get; set; }
}

public class RelatedFile
{
    public string FilePath { get; init; }
    public string Reason { get; init; }
}