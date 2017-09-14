using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.SoftwareFactory.Plugins.TagWeaving
{
    //EXTENSIONTAG + @"(.*?)\r\n"
    public class TokenParser
    {
        public string Name { get; private set; }
        public string Pattern { get; private set; }

        public TokenParser(string name, string pattern)
        {
            Name = name;
            Pattern = pattern;
        }

        public virtual ParsedToken Parse(int index, int length, string value)
        {
            return new ParsedToken(index, length, Name, value);
        }
    }
}
