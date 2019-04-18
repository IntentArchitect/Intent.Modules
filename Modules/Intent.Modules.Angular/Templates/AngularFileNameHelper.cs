using System;
using System.Text;

namespace Intent.Modules.Angular.Templates
{
    public static class AngularFileNameHelper
    {
        public static string ToAngularFileName(this string name)
        {
            var sb = new StringBuilder(name);
            for (int i = 0; i < sb.Length; i++)
            {
                var c = sb[i];
                if (Char.IsUpper(c))
                {
                    sb.Remove(i, 1);
                    sb.Insert(i, Char.ToLower(c));
                    if (i != 0 && i < sb.Length - 1)
                    {
                        sb.Insert(i, "-");
                    }
                }
            }

            return sb.ToString();
        }
    }
}