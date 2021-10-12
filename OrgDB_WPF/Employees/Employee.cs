using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OrgDB_WPF
{
    /// <summary>
    /// Базовый абстрактный класс "Сотрудники"
    /// </summary>
    public abstract class Employee : INotifyPropertyChanged, IXmlServices, IJsonServices
    {

        #region Поля

        // Список занятых имён
        protected static List<string> employeeNames;

        // Идентификатор
        Guid id;

        // Имя сотрудника
        protected string name;

        // Фамилия сотрудника
        protected string surname;

        // Возраст
        protected int age;

        // Зарплата
        protected int salary;

        // Чин
        protected string post;
        public enum post_enum 
        { 
            [Description("Управляющий")] manager=10, 
            [Description("Специалист")] specialist=5, 
            [Description("Интерн")] intern=1 
        }

        // ID департамента
        protected Guid departmentID;

        // Наименование департамента
        protected string departmentName;
        
        #endregion

        #region Свойства

        // Идентификатор
        public Guid Id { get { return id; } }
        // Имя
        public abstract string Name { get; set; }
        // Фамилия
        public string Surname { get { return surname; } set { surname = value; OnPropertyChanged("Surname"); } }
        // Возраст
        public int Age { get { return age; } set { age = value; OnPropertyChanged("Age"); } }
        // Заработная плата
        public abstract int Salary { get; set; }
        // ID департамента
        public Guid DepartmentID { get { return departmentID; } set { departmentID = value; OnPropertyChanged("DepartmentID"); } }
        //Должность
        public abstract string Post { get; }
        // Наименование департамента
        public string DepartmentName { get { return departmentName; } set { departmentName = value; OnPropertyChanged("DepartmentName"); } }

        public post_enum post_Enum { get; set; }

        public int Post_int { get; set; }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Статический конструктор
        /// </summary>
        static Employee()
        {
            employeeNames = new List<string>();
        }

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Surname">Фамилия</param>
        /// <param name="Age">Возраст</param>
        /// <param name="Salary">Зарплата</param>
        /// <param name="Post">Должность</param>
        /// <param name="DepartmentID">Идентификатор департамента</param>
        public Employee(string Name, string Surname, int Age, int Salary, string Post, Guid DepartmentID)
        {
            id = Guid.NewGuid();
            // Гарантируем уникальное имя
            name = Name;
            surname = Surname;
            age = Age;
            salary = Salary;
            post = Post;         
            departmentID = DepartmentID;
        }

        public Employee(string Name, string Surname, int Age, int Salary, post_enum Post, Guid DepartmentID)
        {
            id = Guid.NewGuid();
            // Гарантируем уникальное имя
            name = Name;
            surname = Surname;
            age = Age;
            salary = Salary;
            post_Enum = Post;
            post = EmployeePostDescriptionConverter.GetEnumDescription(post_Enum);
            Post_int = (int)post_Enum;
            departmentID = DepartmentID;
        }

        /// <summary>
        /// Сокращённый контруктор, имя и зарплата назначаются в конструкторе производного класса
        /// </summary>
        /// <param name="Surname"></param>
        /// <param name="Age"></param>
        /// <param name="Post"></param>
        /// <param name="DepartmentID"></param>
        public Employee(string Surname, int Age, string Post, Guid DepartmentID)
        {
            id = Guid.NewGuid();
            surname = Surname;
            age = Age;
            post = Post;
            departmentID = DepartmentID;
        }

        public Employee(string Surname, int Age, post_enum Post, Guid DepartmentID)
        {
            id = Guid.NewGuid();
            surname = Surname;
            age = Age;
            post_Enum = Post;
            post = EmployeePostDescriptionConverter.GetEnumDescription(post_Enum);
            Post_int = (int)post_Enum;
            departmentID = DepartmentID;
        }

        /// <summary>
        /// Конструктор по XML-читалке и должности
        /// </summary>
        /// <param name="reader">XML-читалка узла соответствующего сотрудника</param>
        /// <param name="Post">Строка - Должность сотрудника</param>
        public Employee(XmlReader reader, string Post)
        {
            // Начинаем чтение, позиционируемся на первом элементе
            reader.Read();

            // Позиционируемся на атрибуте id, устанавливаем id этого сотрудника
            reader.MoveToAttribute("id");
            id = new Guid(reader.Value);

            // Позиционируемся в чтении на следующем элементе
            reader.Read();

            post = Post;
            
            // Читаем, анализируем имя узла, устанавливаем значение соответствующего поля этого сотрудника
            while (!(reader.Name == Post && reader.NodeType == XmlNodeType.EndElement))
            {
                switch (reader.Name)
                {
                    case "Name":
                        Name = reader.ReadElementContentAsString();
                        break;
                    case "SurName":
                        Surname = reader.ReadElementContentAsString();
                        break;
                    case "Age":
                        Age = reader.ReadElementContentAsInt();
                        break;
                    case "Salary":
                        Salary = reader.ReadElementContentAsInt();
                        break;
                    case "DepartmentId":
                        DepartmentID = new Guid(reader.ReadElementContentAsString());
                        break;
                }
            }
        }

        public Employee(XmlReader reader, post_enum Post)
        {
            // Начинаем чтение, позиционируемся на первом элементе
            reader.Read();

            // Позиционируемся на атрибуте id, устанавливаем id этого сотрудника
            reader.MoveToAttribute("id");
            id = new Guid(reader.Value);

            // Позиционируемся в чтении на следующем элементе
            reader.Read();

            post_Enum = Post;
            post = EmployeePostDescriptionConverter.GetEnumDescription(post_Enum);

            // Читаем, анализируем имя узла, устанавливаем значение соответствующего поля этого сотрудника
            while (!(reader.Name == post_Enum.ToString() && reader.NodeType == XmlNodeType.EndElement))
            {
                switch (reader.Name)
                {
                    case "Name":
                        Name = reader.ReadElementContentAsString();
                        break;
                    case "SurName":
                        Surname = reader.ReadElementContentAsString();
                        break;
                    case "Age":
                        Age = reader.ReadElementContentAsInt();
                        break;
                    case "Salary":
                        Salary = reader.ReadElementContentAsInt();
                        break;
                    case "DepartmentId":
                        DepartmentID = new Guid(reader.ReadElementContentAsString());
                        break;
                }
            }
        }

        /// <summary>
        /// Конструктор по JSON - объекту (DTO) сотрудника 
        /// </summary>
        /// <param name="jEmployee"></param>
        public Employee(JObject jEmployee)
        {
            
            // по соответствующим токенам заполняем значения полей

            id = new Guid((string)jEmployee.SelectToken("id"));
            name = (string)jEmployee.SelectToken("Name");
            surname = (string)jEmployee.SelectToken("Surname");
            age = (int)jEmployee.SelectToken("Age");
            salary = (int)jEmployee.SelectToken("Salary");
            post_Enum = (post_enum)(int)jEmployee.SelectToken("Post");
            post = EmployeePostDescriptionConverter.GetEnumDescription(post_Enum);
            departmentID = new Guid((string)jEmployee.SelectToken("DepartmentID"));

        }

        public Employee() { }

        #endregion Конструкторы

        #region Публичные методы

        /// <summary>
        /// Генератор авто-имени
        /// Возвращает следующее имя из серии ("Name_1", "Name_2", ...)
        /// </summary>
        /// <param name="curName">предполагаемое имя</param>
        /// <returns></returns>
        public string NextAutoName()
        {
            return Common.NextAutoName(Name, employeeNames);            
        }

        /// <summary>
        /// Запись сотрудника в XML
        /// </summary>
        /// <param name="writer"></param>
        public abstract void WriteXml(XmlWriter writer);
        
        public void WriteXmlEmployee(XmlWriter writer, string NodeName)
        {
            writer.WriteStartElement(NodeName);
            WriteXmlBasicProperties(writer);
            writer.WriteEndElement();
        }

        public void WriteXmlBasicProperties(XmlWriter writer)
        {
            writer.WriteAttributeString("id", id.ToString());
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("SurName", Surname);
            Common.WriteXMLElement(writer, "Age", Age);
            Common.WriteXMLElement(writer, "Salary", Salary);
            writer.WriteElementString("DepartmentId", DepartmentID.ToString());
            
        }

        public virtual void SetDetails(XmlReader xmlReader)
        {

        }

        public virtual void SetDetails(JObject jEmployee) 
        {
            // по соответствующим токенам заполняем значения полей

            id = (Guid)jEmployee.SelectToken("id");
            name = (string)jEmployee.SelectToken("Name");
            surname = (string)jEmployee.SelectToken("Surname");
            age = (int)jEmployee.SelectToken("Age");
            salary = (int)jEmployee.SelectToken("Salary");
            post_Enum = (post_enum)(int)jEmployee.SelectToken("Post");
            post = EmployeePostDescriptionConverter.GetEnumDescription(post_Enum);
            departmentID = new Guid((string)jEmployee.SelectToken("DepartmentID"));
        }

        #endregion Публичные методы

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
