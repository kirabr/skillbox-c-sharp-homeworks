using System;
using System.Xml;
using System.Xml.XPath;

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
            //set
            //{
            //    if (value.GetType() != typeof(IndividualStatus))
            //        throw new Exception("Для физического лица допускается установка статуса только физического лица.");
            //    clientStatus = value;
            //    clientStatusId = value.ID;
            //}
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

    }

}
