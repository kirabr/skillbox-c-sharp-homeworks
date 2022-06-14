using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OrgDB_WPF
{
    public class Department : INotifyPropertyChanged, IXmlServices, IIdentifyedObject
    {

        #region Поля

        // Идентификатор департамента
        public readonly Guid id;

        private string name;
        private string location;
        private Guid parentId;
        private string parentName;
        private bool isEditing;
        private static List<string> departmentNames = new List<string>();

        #endregion Поля

        #region Свойства

        // Id
        public Guid Id { get { return id; } }

        // Наименование
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        // Местоположение
        public string Location { get { return location; } set { location = value; OnPropertyChanged("Location"); } }

        // Идентификатор головного департамента
        public Guid ParentId { get { return parentId; } set { parentId = value; OnPropertyChanged("ParentId"); } }

        // Имя головного департамента
        public string ParentName { get { return parentName; } set { parentName = value; OnPropertyChanged("ParentName"); } }

        public bool IsEditing { get { return isEditing; } }

        // Список занятых наименований
        public static List<string> DepartmentNames { get { return departmentNames; } set { departmentNames = value; } }

        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="Name">Наименование</param>
        /// <param name="Location">Местоположение</param>
        /// <param name="ParentId">Идентификатор головного департамента</param>
        public Department(string Name, string Location, Guid ParentId, bool IsEditing = false)
        {
            id = Guid.NewGuid();

            this.Name = Name;            

            this.Location = Location;
            this.ParentId = ParentId;
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Department() : this("Department", "Undefined", Guid.Empty) { }
        
        /// <summary>
        /// Конструктор по назначению (основное / для редактирования)
        /// </summary>
        /// <param name="IsEditing">
        /// Признак, для чего создаётся экземпляр. false - значение по умолчанию - основное назначение, для помещения в базу
        /// true - временный экземпляр для редактирования, не предназначен для помещения в базу
        /// </param>
        public Department(bool IsEditing = false) : this("Department", "Undefined", Guid.Empty, IsEditing) 
        {
            this.isEditing = IsEditing;
        }

        /// <summary>
        /// Конструктор по XML-чтению, используется при восстановлении базы из XML-файла
        /// </summary>
        /// <param name="reader">
        /// XML-читалка
        /// </param>
        public Department(XmlReader reader)
        {
            reader.Read();
            reader.MoveToAttribute("id");
            id = new Guid(reader.Value);
            reader.Read();

            while (!(reader.Name == "Department" && reader.NodeType == XmlNodeType.EndElement))
            {
                switch (reader.Name)
                {
                    case "Name":
                        Name = reader.ReadElementContentAsString();
                        break;
                    case "Location":
                        Location = reader.ReadElementContentAsString();
                        break;
                    case "ParentId":
                        ParentId = new Guid(reader.ReadElementContentAsString());
                        break;
                }
            }
        }

        /// <summary>
        /// Конструктор по JSON (DTO) - объекту департамента. Используется при восстановлении базы из JSON-файла
        /// </summary>
        /// <param name="jDepartment"></param>
        public Department(JObject jDepartment)
        {
            id = new Guid((string)jDepartment.SelectToken("id"));
            Name = (string)jDepartment.SelectToken("Name");
            Location = (string)jDepartment.SelectToken("Location");
            ParentId = new Guid((string)jDepartment.SelectToken("ParentId"));
        }

        #endregion Конструкторы

        #region Запись в XML

        /// <summary>
        /// Записывает департамент в XML-запись
        /// </summary>
        /// <param name="writer">Запись XML</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            WriteXmlBasicProperties(writer);
            writer.WriteEndElement();
        }

        public void WriteXmlBasicProperties(XmlWriter writer)
        {
            writer.WriteAttributeString("id", id.ToString());
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("Location", Location);
            writer.WriteElementString("ParentId", ParentId.ToString());
        }

        #endregion Запись в XML

        #region API

        /// <summary>
        /// Возвращает копию департамента (свойства - по указателям)
        /// </summary>
        /// <returns></returns>
        public Department ShallowCopy()
        {
            return (Department)this.MemberwiseClone();
        } 

        #endregion API

        #region Реализация INotifyPropertyChanged

        /// <summary>
        /// Обработчик изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion Реализация INotifyPropertyChanged
    }
}
