using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.Configuration
{
    public class AppSettingRegistrationRequest
    {
        public AppSettingRegistrationRequest(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
