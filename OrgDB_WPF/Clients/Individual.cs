using System;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF.Clients
{
    public class Individual : Client
    {

        #region Поля

        // Имя, фамилия, отчетсво
        string firstName;
        string surName;
        string patronymic;

        // Привелигерованный клиент
        bool isVIP;

        #endregion Поля


        #region Свойства

        // Имя, фамилия, отчетсво
        public string FirstName { get { return firstName; } set { firstName = value; } }
        public string SurName { get { return surName; } set { surName = value; } }
        public string Patronymic { get { return patronymic; } set { patronymic = value; } }

        // Привелигерованный клиент
        public bool IsVIP { get { return isVIP; } set { isVIP = value; } }

        // Статус клиента
        public override ClientStatus ClientStatus
        {
            get { return clientStatus; }
          
        }

        #endregion Свойства


        #region Конструкторы

        public Individual(string Name, Guid Id, bool IsResident = true) : base(Name, Id, IsResident){}

        public Individual(string Name) : base(Name, Guid.NewGuid(), true) { }

        public Individual(XPathNavigator xPathNavigator) :base(xPathNavigator)
        {
            XPathNavigator selectedNode = xPathNavigator.SelectSingleNode("//FirstName");
            if (selectedNode != null) firstName = selectedNode.Value;

            selectedNode = xPathNavigator.SelectSingleNode("//SurName");
            if (selectedNode != null) surName = selectedNode.Value;

            selectedNode = xPathNavigator.SelectSingleNode("//Patronymic");
            if (selectedNode != null) patronymic = selectedNode.Value;

        }

        public Individual(JObject jClient) : base(jClient)
        {
            firstName = (string)jClient.SelectToken("FirstName");
            surName = (string)jClient.SelectToken("SurName");
            patronymic = (string)jClient.SelectToken("Patronymic");
            isVIP = (bool)jClient.SelectToken("IsVIP");
        }

        #endregion Конструкторы


        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {

            string EmptyID = Common.EmptyIDString();
            
            writer.WriteStartElement(GetType().Name);

            WriteXmlBasicProperties(writer);

            writer.WriteElementString("FirstName", FirstName);
            writer.WriteElementString("SurName", SurName);
            writer.WriteElementString("Patronymic", Patronymic);
            Common.WriteXMLElement(writer, "IsVIP", IsVIP);
            
            writer.WriteEndElement();
        }

        #endregion Запись в XML


        #region Запись в JSON
        public override void WriteJsonParticularProperties(JsonWriter writer)
        {

            writer.WritePropertyName("FirstName"); writer.WriteValue(FirstName);
            writer.WritePropertyName("SurName"); writer.WriteValue(SurName);
            writer.WritePropertyName("Patronymic"); writer.WriteValue(Patronymic);
            writer.WritePropertyName("IsVIP"); writer.WriteValue(IsVIP);

        }

        #endregion Запись в JSON

    }

}
