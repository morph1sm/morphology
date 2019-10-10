
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace Morphology.Data
{
    public static class Extensions
    {
        public static DirectoryInfo AddSubFolder(this DirectoryInfo dirinfo, string subfolder, bool propogate = true)
        {
            if (string.IsNullOrEmpty(subfolder))
            {
                throw new Exception("Subfolder is Null or Empty");
            }

            if (!propogate)
            {
                return dirinfo.CreateSubdirectory(subfolder);
            }

            dirinfo = new DirectoryInfo(Path.Combine(dirinfo.FullName, subfolder));
            return dirinfo;
        }


        public static T DeserializeFromFile<T>(this T value, string filePath)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            T result;

            using (TextReader reader = new StringReader(File.ReadAllText(filePath)))
            {
                result = (T)xmlSerializer.Deserialize(reader);
            }
            return result;
        }

        public static bool SerializeToFile<T>(this T value, string filePath)
        {
            if (value == null) return false;

            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var stringWriter = new Utf8StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
                {
                    xmlSerializer.Serialize(xmlWriter, value);
                    File.WriteAllText(filePath, stringWriter.ToString());
                    return true;
                }
            }
        }

        public static T FindVisualParent<T>(this UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }

            return null;
        }
    }
}
