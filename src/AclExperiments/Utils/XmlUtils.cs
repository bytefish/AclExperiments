// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AclExperiments.Utils
{
    public static class XmlUtils
    {
        public static T? Deserialize<T>(string xmlString)
            where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringReader stringReader = new StringReader(xmlString))
            {
                return xmlSerializer.Deserialize(stringReader) as T;
            }
        }

        public static string? Serialize<T>(T element, XmlWriterSettings? settings = null)
            where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            
            StringBuilder stringBuilder = new StringBuilder();

            using(var xmlWriter = XmlWriter.Create(stringBuilder, settings: settings))
            {
                xmlSerializer.Serialize(xmlWriter, element);
            }

            return stringBuilder.ToString();            
        }
    }
}