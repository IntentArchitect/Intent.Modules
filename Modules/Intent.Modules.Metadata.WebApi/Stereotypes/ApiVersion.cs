using System.Collections.Generic;

namespace Intent.Modules.Metadata.WebApi.Stereotypes;

public class ApiVersion
{
    public IList<ApiApplicableVersion> ApplicableVersions { get; set; }
}

public class ApiApplicableVersion
{
    public string DefinitionName { get; set; }
    public string Version { get; set; }
    public bool IsDeprecated { get; set; }
}