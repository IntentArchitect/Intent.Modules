using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Parsing
{
    public class Bookmark
    {
        public int Position { get; internal set; }
        public string Token { get; }

        public Bookmark(string token, int position)
        {
            Token = token;
            Position = position;
        }
    }
}
