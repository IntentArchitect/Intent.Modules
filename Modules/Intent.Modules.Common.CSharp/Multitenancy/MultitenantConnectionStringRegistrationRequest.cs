using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.CSharp.Multitenancy
{
    public class MultitenantConnectionStringRegistrationRequest
    {
        public MultitenantConnectionStringRegistrationRequest(string name, string connectionStringTemplate)
        {
            Name = name;
            ConnectionStringTemplate = connectionStringTemplate;
        }

        public string Name { get; }
        public string ConnectionStringTemplate { get; }
    }
}
