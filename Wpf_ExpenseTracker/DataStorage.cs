using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Wpf_ExpenseTracker
{
    public class DataStorage
    {

        internal static void WriteXML<T>(T data, string fileName)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream stream;
                stream = new FileStream(fileName, FileMode.Create);
                serializer.Serialize(stream, data);
                stream.Close();
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
                throw;
            }
        }

        internal static T ReadXML<T>(string fileName)
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(sr);
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("Error: " + x, "Caution...");
                return (T)default;
            }
        }

        internal static void WriteXml<T>(T data, string fileName)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream stream;
                stream = new FileStream(fileName, FileMode.Create);
                serializer.Serialize(stream, data);
                stream.Close();
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
                throw;
            }
        }
    }
}