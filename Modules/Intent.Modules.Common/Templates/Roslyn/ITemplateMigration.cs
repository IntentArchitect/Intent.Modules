using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.RoslynMerge.Migrations
{
    public interface ITemplateMigration
    {
        TemplateMigrationCriteria Criteria { get; }
        string Execute(string solutionFileLocation);
    }
}
