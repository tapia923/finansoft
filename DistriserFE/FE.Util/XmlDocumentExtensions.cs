using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FE.Util
{
    public static class XmlDocumentExtensions
    {
        public static string ToXmlString(this XmlDocument document, bool omitXmlDeclaration = false)
        {
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings
                {
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace,
                    Indent = true,
                    OmitXmlDeclaration = omitXmlDeclaration
                };

                using (var xw = XmlWriter.Create(sw, settings))
                {
                    document.WriteTo(xw);

                    xw.Flush();
                    sw.Flush();

                    return sw.ToString();
                }
            }
        }
    }
}
