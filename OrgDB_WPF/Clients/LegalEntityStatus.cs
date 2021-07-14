using System.Xml;

namespace OrgDB_WPF.Clients
{
    public class LegalEntityStatus : ClientStatus
    {
        public LegalEntityStatus(string Name) : base(Name)
        {
        }

        public LegalEntityStatus(XmlReader reader) : base(reader) { }

        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            WriteXmlBasicProperties(writer);
            writer.WriteEndElement();
        }

        #endregion Запись в XML
    }


}
