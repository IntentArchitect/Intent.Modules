using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Intent.Modules.Common
{
    public static class XmlHelper
    {
        public static string ToStringUTF8(this XDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new Utf8StringWriter(builder))
            {
                doc.Save(writer);
            }
            return builder.ToString();
        }
    }

    internal class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter(StringBuilder sb) : base (sb)
        {

        }

        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
