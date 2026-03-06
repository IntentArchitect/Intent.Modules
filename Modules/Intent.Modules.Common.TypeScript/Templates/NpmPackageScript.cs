using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Templates;

public class NpmPackageScript
{

    public NpmPackageScript(string name, string command)
    {
        Name = name;
        Command = command;
    }

    public string Name  { get; set; }

    public string Command { get; set; }
}
