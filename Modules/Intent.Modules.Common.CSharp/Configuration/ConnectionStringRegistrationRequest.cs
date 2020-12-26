using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.Configuration
{
    public class ConnectionStringRegistrationRequest
    {
        public ConnectionStringRegistrationRequest(string name, string connectionString, string providerName)
        {
            Name = name;
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
    }
}
