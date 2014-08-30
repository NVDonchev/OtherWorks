using System;
using System.IO;
using System.Xml.Serialization;

namespace XMLGeneratorApp
{
    public class Program
    {
        public static void Main()
        {
            var xmlGenerator = new XMLGenerator<PurchaseOrder>();

            xmlGenerator.DeserializeFromXML("input.xml");
            xmlGenerator.SerializeToXML("output.xml");
        }
    }

    public class XMLGenerator<T>
    {
        private T DataObj { get; set; }

        public void DeserializeFromXML(string inputFilename)
        {
            using (var reader = new StreamReader(inputFilename))
            {
                var serializer = new XmlSerializer(typeof(T));
                DataObj = (T)serializer.Deserialize(reader);
            }
        }

        public void SerializeToXML(string outputFilename)
        {
            using (var writer = new StreamWriter(outputFilename))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, DataObj);
            }
        }
    }
}