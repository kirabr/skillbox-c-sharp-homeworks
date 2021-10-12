using System;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF.Clients
{
    public abstract class ClientStatus : IXmlServices, IJsonServices
    {

        #region Поля

        // Идентификатор
        Guid id;

        // Название статуса ("Gold", "Silver", "Basic", etc)
        string name;

        // Ранжирование статуса - статутсы ступенью ниже и выше
        ClientStatus previousClientStatus;
        ClientStatus nextClientStatus;

        // Идентификаторы ранжированных статусов
        Guid previousClientStatusId;
        Guid nextClientStatusId;

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
        public ClientStatus PreviousClientStatus 
        { 
            get { return previousClientStatus; } 
            set 
            { 
                previousClientStatus = value;
                if (previousClientStatus == null)
                    previousClientStatusId = Guid.Empty;
                else
                    previousClientStatusId = previousClientStatus.ID;
            } 
        }
        public ClientStatus NextClientStatus 
        { 
            get { return nextClientStatus; } 
            set 
            { 
                nextClientStatus = value;
                if (nextClientStatus == null)
                    nextClientStatusId = Guid.Empty;
                else
                    nextClientStatusId = nextClientStatus.ID;
            } 
        }

        // Идентификаторы ранжированных статусов
        public Guid PreviousClientStatusId { get { return previousClientStatusId; } }
        public Guid NextClientStatusId { get { return nextClientStatusId; } }

        // Уменьшение базовой ставки по кредиту
        public double CreditDiscountPercent { get { return creditDiscountPercent; } set { creditDiscountPercent = value; } }

        // Увеличение базовой ставки по вкладу
        public double DepositAdditionalPercent { get { return depositAdditionalPercent; } set { depositAdditionalPercent = value; } }

        #endregion Свойства

        #region Конструкторы

        public ClientStatus(string Name)
        {
            id = Guid.NewGuid();
            name = Name;
        }

        public ClientStatus(XmlReader reader)
        {
            ReadXmlBasicProperties(reader);
        }

        public ClientStatus(JObject jClientStatus)
        {
            id = new Guid((string)jClientStatus.SelectToken("id"));
            name = (string)jClientStatus.SelectToken("Name");
            previousClientStatusId = new Guid((string)jClientStatus.SelectToken("PreviousClientStatusId"));
            nextClientStatusId = new Guid((string)jClientStatus.SelectToken("NextClientStatusId"));
            creditDiscountPercent = (double)jClientStatus.SelectToken("CreditDiscountPercent");
            depositAdditionalPercent = (double)jClientStatus.SelectToken("DepositAdditionalPercent");

        }

        protected ClientStatus() { }

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

        #region Чтение из XML

        private void ReadXmlBasicProperties(XmlReader reader)
        {
            // Начинаем чтение, позиционируемся на первом элементе
            reader.Read();

            string nodeName = reader.Name;

            // Позиционируемся на атрибуте id, устанавливаем id этого сотрудника
            reader.MoveToAttribute("id");
            id = new Guid(reader.Value);

            // Позиционируемся в чтении на следующем элементе
            reader.Read();

            while (!(reader.Name == nodeName && reader.NodeType == XmlNodeType.EndElement))
            {
                switch (reader.Name)
                {
                    case "Name":
                        Name = reader.ReadElementContentAsString();
                        break;
                    case "PreviousClientStatusId":
                        previousClientStatusId = new Guid(reader.ReadElementContentAsString());
                        break;
                    case "NextClientStatusId":
                        nextClientStatusId = new Guid(reader.ReadElementContentAsString());
                        break;
                    case "CreditDiscountPercent":
                        CreditDiscountPercent = reader.ReadElementContentAsDouble();
                        break;
                    case "DepositAdditionalPercent":
                        DepositAdditionalPercent = reader.ReadElementContentAsDouble();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        #endregion Чтение из XML


        #region Реализация IJsonServices

        public virtual void SetDetails(JObject jClientStatus)
        {
            id = new Guid((string)jClientStatus.SelectToken("id"));
            name = (string)jClientStatus.SelectToken("Name");
            previousClientStatusId = new Guid((string)jClientStatus.SelectToken("PreviousClientStatusId"));
            nextClientStatusId = new Guid((string)jClientStatus.SelectToken("NextClientStatusId"));
            creditDiscountPercent = (double)jClientStatus.SelectToken("CreditDiscountPercent");
            depositAdditionalPercent = (double)jClientStatus.SelectToken("DepositAdditionalPercent");
        }

        #endregion Реализация IJsonServices


    }

}
