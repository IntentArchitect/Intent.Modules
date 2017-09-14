using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Plugins.TagWeaving
{
    public class TagToken : ParsedToken
    {
        public string Identifier { get; protected set; }

        public TagToken(int index, int length, string name, string value) 
            : base(index, length, name, value)
        {

        }
    }
}
