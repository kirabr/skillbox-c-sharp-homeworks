﻿using System.Xml;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF.Clients
{
    public class IndividualStatus : ClientStatus
    {
        public IndividualStatus(string Name) : base(Name)
        {
        }

        public IndividualStatus(XmlReader reader) : base(reader) { }

        public IndividualStatus(JObject jClientStatus) : base(jClientStatus) { }

        public IndividualStatus() { }

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
