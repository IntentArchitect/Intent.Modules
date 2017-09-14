using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.SoftwareFactory.Plugins.TagWeaving
{
    public class ParsedToken
    {
        public ParsedToken(int index, int length, string name, string value )
        {
            Index = index;
            Length = length;
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
        public string Value { get; set; }
    }
}
