using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF.Clients
{
    
    public abstract class Client
    {

        #region Поля

        // Название клиента
        string name;

        // Идентификатор клиента
        Guid id;

        // Менеджер клиента 
        Employee clientManager;

        // Признак, что клиент является резидентом
        bool isResident;

        // Статус клиента
        ClientStatus clientStatus;

        #endregion Поля

        #region Свойства

        // Название клиента
        public string Name { get { return name; } set { name = value; } }

        // Идентификатор клиента
        public Guid ID { get { return id; } }

        // Менеджер клиента
        public Employee ClientManager { get { return clientManager; } set { clientManager = value; } }

        // Признак, что клиент является резидентом
        public bool IsResident { get { return isResident; } set { isResident = value; } }

        // Статус клиента
        public ClientStatus ClientStatus { get { return clientStatus; } set { clientStatus = value; } }

        #endregion Свойства

        #region Конструкторы

        public Client(string clientName, Guid clientId, bool clientIsResident = true)
        {
            name = clientName;
            id = clientId;
            isResident = clientIsResident;
        }

        #endregion Конструкторы

    }

}
