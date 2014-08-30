using System.Xml;

public class XMLGenerator
{
    public static void Main()
    {
        var settings = new XmlWriterSettings { Indent = true };

        using (var writer = XmlWriter.Create("filename.xml", settings))
        {
            writer.WriteStartDocument();

            writer.WriteStartElement("pricing-feed", "http://seller.marketplace.sears.com/pricing/v4");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xsi", "schemaLocation", null, "http://seller.marketplace.sears.com/pricing/v4 ../../../../../rest/pricing/import/v4/pricing.xsd");

            writer.WriteStartElement("fbm-pricing");

            writer.WriteStartElement("item");
            writer.WriteAttributeString("item-id", "xxx");

            writer.WriteElementString("standard-price", "xxx");

            writer.WriteElementString("map-price-indicator", "strict");

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteEndDocument();
        }
    }
}
