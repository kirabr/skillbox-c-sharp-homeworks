using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF.Clients
{
    
    public abstract class Client : IXmlServices, IJsonServices
    {

        #region Поля

        // Название клиента
        string name;

        // Идентификатор клиента
        Guid id;

        // Менеджер клиента 
        Employee clientManager;

        // Идентификатор менеджера клиента
        Guid clientManagerId;

        // Признак, что клиент является резидентом
        bool isResident;

        // Статус клиента
        protected ClientStatus clientStatus;

        // Идентификатор статуса клиента
        protected Guid clientStatusId;

        #endregion Поля

        #region Свойства

        // Название клиента
        public string Name { get { return name; } set { name = value; } }

        // Идентификатор клиента
        public Guid ID { get { return id; } }

        // Менеджер клиента
        public Employee ClientManager 
        { 
            get { return clientManager; } 
            set 
            { 
                clientManager = value;
                clientManagerId = value.Id;
            } 
        }

        // Идентификатор менеджера клиента
        public Guid ClientManagerId { get { return clientManagerId; } }

        // Признак, что клиент является резидентом
        public bool IsResident { get { return isResident; } set { isResident = value; } }

        // Статус клиента
        public virtual ClientStatus ClientStatus 
        { 
            get { return clientStatus; }
            set
            {
                clientStatus = value;
                clientStatusId = value.ID;
            }
        }

        // Идентификатор статуса клиента
        public Guid ClientStatusId { get { return clientStatusId; } }

        #endregion Свойства

        #region Конструкторы

        public Client(string clientName, Guid clientId, bool clientIsResident = true)
        {
            name = clientName;
            id = clientId;
            isResident = clientIsResident;
        }
        
        public Client(XPathNavigator xPathNavigator)
        {
            XPathNavigator selectedNode = xPathNavigator.SelectSingleNode("//@id");
            if (selectedNode != null) id = new Guid(selectedNode.Value);

            selectedNode = xPathNavigator.SelectSingleNode("//Name");
            if (selectedNode != null) Name = selectedNode.Value;

            selectedNode = xPathNavigator.SelectSingleNode("//ClientManagerID");
            if (selectedNode != null) clientManagerId = new Guid(selectedNode.Value);

            selectedNode = xPathNavigator.SelectSingleNode("//IsResident");
            if (selectedNode != null) isResident = selectedNode.ValueAsBoolean;

            selectedNode = xPathNavigator.SelectSingleNode("//ClientStatusID");
            if (selectedNode != null) clientStatusId = new Guid(selectedNode.Value);
        }

        //public Client(JObject jClient)
        //{
        //    id = new Guid((string)jClient.SelectToken("id"));
        //    name = (string)jClient.SelectToken("Name");
        //    clientManagerId = new Guid((string)jClient.SelectToken("ClientManagerId"));
        //    IsResident = (bool)jClient.SelectToken("IsResident");
        //    clientStatusId = new Guid((string)jClient.SelectToken("ClientStatusId"));
        //}

        public Client() { }

        #endregion Конструкторы

        #region Запись в XML

        public abstract void WriteXml(XmlWriter writer);

        public void WriteXmlBasicProperties(XmlWriter writer)
        {

            string EmptyID = Common.EmptyIDString();
            
            writer.WriteAttributeString("id", ID.ToString());
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("ClientManagerID", ClientManager == null ? EmptyID : ClientManager.Id.ToString());
            Common.WriteXMLElement(writer, "IsResident", IsResident);
            writer.WriteElementString("ClientStatusID", ClientStatus == null ? EmptyID : ClientStatus.ID.ToString());
        }

        #endregion Запись в XML


        #region Запись в JSON

        public abstract void WriteJsonParticularProperties(JsonWriter writer);

        #endregion Запись в JSON


        #region Чтение из XML

        //private void ReadXmlBasicProperties(XmlReader reader)
        //{

        //    // Начинаем чтение, позиционируемся на первом элементе
        //    reader.Read();

        //    string nodeName = reader.Name;

        //    // Позиционируемся на атрибуте id, устанавливаем id этого сотрудника
        //    reader.MoveToAttribute("id");
        //    id = new Guid(reader.Value);

        //    // Позиционируемся в чтении на следующем элементе
        //    reader.Read();

        //    while (!(reader.Name == nodeName && reader.NodeType == XmlNodeType.EndElement))
        //    {
        //        switch (reader.Name)
        //        {
        //            case "Name":
        //                Name = reader.ReadElementContentAsString();
        //                break;
        //            case "ClientManagerID":
        //                clientManagerId = new Guid(reader.ReadElementContentAsString());
        //                break;
        //            case "IsResident":
        //                isResident = reader.ReadElementContentAsBoolean();
        //                break;
        //            case "ClientStatusID":
        //                clientStatusId = new Guid(reader.ReadElementContentAsString());
        //                break;
        //            default:
        //                reader.Skip();
        //                break;
        //        }
        //    }

        //}

        #endregion Чтение из XML


        #region Реализация интерфейса IJsonServices

        public virtual void SetDetails(JObject jClient)
        {
            id = new Guid((string)jClient.SelectToken("id"));
            name = (string)jClient.SelectToken("Name");
            clientManagerId = new Guid((string)jClient.SelectToken("ClientManagerId"));
            IsResident = (bool)jClient.SelectToken("IsResident");
            clientStatusId = new Guid((string)jClient.SelectToken("ClientStatusId"));
        }

        #endregion Реализация интерфейса IJsonServices


    }

}
