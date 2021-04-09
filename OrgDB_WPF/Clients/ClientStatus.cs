using System;
using System.Xml;
using System.Xml.Serialization;

namespace OrgDB_WPF.Clients
{
    public abstract class ClientStatus : OrgDB_WPF.IXmlServices
    {

        #region Поля

        // Идентификатор
        Guid id;

        // Название статуса ("Gold", "Silver", "Basic", etc)
        string name;

        // Ранжирование статуса - статутсы ступенью ниже и выше
        ClientStatus previousClientStatus;
        ClientStatus nextClientStatus;

        // Уменьшение базовой ставки по кредиту
        double creditDiscountPercent;

        // Увеличение базовой ставки по вкладу
        double depositAdditionalPercent;

        #endregion Поля

        #region Свойства

        // Идентификатор
        public Guid ID { get { return id; } }

        // Название статуса ("Gold", "Silver", "Basic", etc)
        public string Name { get { return name; } set { name = value; } }

        // Ранжирование статуса - статутсы ступенью ниже и выше
        public ClientStatus PreviousClientStatus { get { return previousClientStatus; } set { previousClientStatus = value; } }
        public ClientStatus NextClientStatus { get { return nextClientStatus; } set { nextClientStatus = value; } }

        // Уменьшение базовой ставки по кредиту
        public double CreditDiscountPercent { get { return creditDiscountPercent; } set { creditDiscountPercent = value; } }

        // Увеличение базовой ставки по вкладу
        public double DepositAdditionalPercent { get { return depositAdditionalPercent; } set { depositAdditionalPercent = value; } }

        #endregion Свойства

        #region Конструкторы

        public ClientStatus(string Name)
        {
            id = new Guid();
            name = Name;
        }

        #endregion Конструкторы

        #region Запись в XML

        public abstract void WriteXml(XmlWriter writer);

        public void WriteXmlBasicProperties(XmlWriter writer)
        {

            string EmptyID = Common.EmptyIDString();

            writer.WriteAttributeString("id", id.ToString());
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("PreviousClientStatusId", previousClientStatus == null ? EmptyID : previousClientStatus.ID.ToString());
            writer.WriteElementString("NextClientStatusId", nextClientStatus == null ? EmptyID : nextClientStatus.ID.ToString());
            writer.WriteStartElement("CreditDiscountPercent"); writer.WriteValue(CreditDiscountPercent); writer.WriteEndElement();
            writer.WriteStartElement("DepositAdditionalPercent"); writer.WriteValue(DepositAdditionalPercent); writer.WriteEndElement();

        }

        #endregion Запись в XML

    }


}
