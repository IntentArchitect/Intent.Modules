using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Bower.Contracts
{
    public interface IBowerPackageInfo : IEquatable<IBowerPackageInfo>
    {
        string Name { get; }
        string Version { get; }
        bool InstallsTypings { get; }
        string TypingsName { get; }
        string TypingsLocation { get; }
        IList<string> JsSources { get; }
        IList<string> CssSources { get; }
        IList<IBowerPackageInfo> Dependencies { get; }
        int GetFullDependencyCount();
    }
}
