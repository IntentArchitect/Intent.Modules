using System;
using System.Collections.Generic;

namespace Intent.Modules.Common
{
    public static class ExtensionHelpers
    {
        public static string GetSetting(this IDictionary<string, string> settings, string name)
        {
            return GetSetting(settings, name, (s) => s);
        }


        public static T GetSetting<T>(this IDictionary<string, string> settings, string name, Func<string, T> convert, T defaultValue = default(T))
        {
            if (settings.ContainsKey(name) && !string.IsNullOrWhiteSpace(settings[name]))
            {
                return convert(settings[name]);
            }
            return defaultValue;
        }

        public static void SetIfSupplied(this IDictionary<string, string> settings, string name, Action<string> setSetting)
        {
            settings.SetIfSupplied(name, setSetting, (s) => s );
        }

        public static void  SetIfSupplied<T>(this IDictionary<string, string> settings, string name, Action<T> setSetting, Func<string, T> convert)
        {
            if (settings.ContainsKey(name) && !string.IsNullOrWhiteSpace(settings[name]))
            {
                T convertedValue = default(T);
                try
                {
                    convertedValue = convert(settings[name]);
                }
                catch (Exception e)
                {
                    throw new Exception($"Unable to convert setting {name} to type : {typeof(T).Name} value : {settings[name]}", e);
                }
                setSetting(convertedValue );
            }
        }
    }
}
