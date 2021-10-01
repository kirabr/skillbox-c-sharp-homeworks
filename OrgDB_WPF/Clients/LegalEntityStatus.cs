using System.Xml;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF.Clients
{
    public class LegalEntityStatus : ClientStatus
    {
        public LegalEntityStatus(string Name) : base(Name)
        {
        }

        public LegalEntityStatus(XmlReader reader) : base(reader) { }

        public LegalEntityStatus(JObject jClientStatus) : base(jClientStatus) { }

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
