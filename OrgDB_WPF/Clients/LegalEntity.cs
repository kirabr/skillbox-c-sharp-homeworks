using System;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF.Clients
{
    public class LegalEntity : Client
    {

        #region Поля

        // Полное наименование юридического лица
        string fullName;

        // ИНН, КПП
        string inn;
        string kpp;

        // Корпоративный клиент
        bool isCorporate;

        #endregion Поля


        #region Свойства

        // Полное наименование юридического лица
        public string FullName { get { return fullName; } set { fullName = value; } }

        // ИНН, КПП
        public string INN { get { return inn; } set { inn = value; } }
        public string KPP { get { return kpp; } set { kpp = value; } }

        // Корпоративный клиент
        public bool IsCorporate { get { return isCorporate; } set { isCorporate = value; } }

        // Статус клиента
        public override ClientStatus ClientStatus 
        {
            get { return clientStatus; }
        }

        #endregion Свойства


        #region Конструкторы

        public LegalEntity(string Name, bool IsResident = true):base(Name, Guid.NewGuid(), IsResident) { }

        public LegalEntity(string Name, Guid Id, bool IsResident = true) : base(Name, Id, IsResident)
        {
        }
                
        public LegalEntity(XPathNavigator xPathNavigator) : base(xPathNavigator) 
        {

            XPathNavigator selectedNode = xPathNavigator.SelectSingleNode("//FullName");
            if (selectedNode != null) fullName = selectedNode.Value;

            selectedNode = xPathNavigator.SelectSingleNode("//INN");
            if (selectedNode != null) inn = selectedNode.Value;

            selectedNode = xPathNavigator.SelectSingleNode("//KPP");
            if (selectedNode != null) kpp = selectedNode.Value;

            selectedNode = xPathNavigator.SelectSingleNode("//IsCorporate");
            if (selectedNode != null) isCorporate = selectedNode.ValueAsBoolean;

        }

        public LegalEntity(JObject jClient):base(jClient)
        {
            fullName = (string)jClient.SelectToken("FullName");
            inn = (string)jClient.SelectToken("INN");
            kpp = (string)jClient.SelectToken("KPP");
            isCorporate = (bool)jClient.SelectToken("IsCorporate");
        }

        #endregion Конструкторы

        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {
            string EmptyID = Common.EmptyIDString();

            writer.WriteStartElement(GetType().Name);

            WriteXmlBasicProperties(writer);

            writer.WriteElementString("FullName", FullName);
            writer.WriteElementString("INN", INN);
            writer.WriteElementString("KPP", KPP);
            Common.WriteXMLElement(writer, "IsCorporate", IsCorporate);

            writer.WriteEndElement();
        }

        #endregion Запись в XML

        #region Запись в JSON

        public override void WriteJsonParticularProperties(JsonWriter writer)
        {
            writer.WritePropertyName("FullName"); writer.WriteValue(FullName);
            writer.WritePropertyName("INN"); writer.WriteValue(INN);
            writer.WritePropertyName("KPP"); writer.WriteValue(KPP);
            writer.WritePropertyName("IsCorporate"); writer.WriteValue(IsCorporate);
        }

        #endregion Запись в JSON

    }


}
